using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Services;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Expressions;

namespace LogisticsTroubleManagement.API.Controllers;

[Authorize] // 全エンドポイントに認証必須
[ApiController]
[Route("api/[controller]")]
public class IncidentsController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIncidentRepository _incidentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IncidentDomainService _incidentDomainService;
    private readonly IMasterDataResolverService _masterDataResolver;
    private readonly IWorkflowValidationService _workflowValidationService;
    private readonly ILogger<IncidentsController> _logger;

    // 期待される解決時間の定数（デフォルト3日）
    private static readonly TimeSpan DefaultExpectedResolutionTime = TimeSpan.FromDays(3);

    public IncidentsController(
        IUnitOfWork unitOfWork,
        IIncidentRepository incidentRepository,
        IUserRepository userRepository,
        IncidentDomainService incidentDomainService,
        IMasterDataResolverService masterDataResolver,
        IWorkflowValidationService workflowValidationService,
        ILogger<IncidentsController> logger)
    {
        _unitOfWork = unitOfWork;
        _incidentRepository = incidentRepository;
        _userRepository = userRepository;
        _incidentDomainService = incidentDomainService;
        _masterDataResolver = masterDataResolver;
        _workflowValidationService = workflowValidationService;
        _logger = logger;
    }

    // GET: api/incidents
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<IncidentDto>>> GetIncidents([FromQuery] IncidentSearchDto searchDto)
    {
        try
        {
            _logger.LogInformation("インシデント一覧取得リクエスト: {@SearchDto}", searchDto);

            // 検索条件の構築
            Expression<Func<Incident, bool>>? predicate = null;
            var predicates = new List<Expression<Func<Incident, bool>>>();

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.ToLower();
                predicates.Add(i => i.Title.ToLower().Contains(searchTerm) || 
                                   i.Description.ToLower().Contains(searchTerm) ||
                                   i.Category.ToLower().Contains(searchTerm));
            }

            if (searchDto.Status.HasValue)
            {
                predicates.Add(i => i.Status == searchDto.Status.Value);
            }

            if (searchDto.Priority.HasValue)
            {
                predicates.Add(i => i.Priority == searchDto.Priority.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.Category))
            {
                predicates.Add(i => i.Category == searchDto.Category);
            }

            if (searchDto.ReportedById.HasValue)
            {
                predicates.Add(i => i.ReportedById == searchDto.ReportedById.Value);
            }

            if (searchDto.AssignedToId.HasValue)
            {
                predicates.Add(i => i.AssignedToId == searchDto.AssignedToId.Value);
            }

            if (searchDto.FromDate.HasValue)
            {
                predicates.Add(i => i.OccurrenceDate >= searchDto.FromDate.Value);
            }

            if (searchDto.ToDate.HasValue)
            {
                predicates.Add(i => i.OccurrenceDate <= searchDto.ToDate.Value);
            }

            if (searchDto.IsOverdue.HasValue && searchDto.IsOverdue.Value)
            {
                predicates.Add(i => _incidentDomainService.IsIncidentOverdue(i, DefaultExpectedResolutionTime));
            }

            // 複数の条件をANDで結合
            if (predicates.Any())
            {
                predicate = predicates.Aggregate((current, next) => 
                    Expression.Lambda<Func<Incident, bool>>(
                        Expression.AndAlso(current.Body, next.Body),
                        current.Parameters));
            }

            // ソート条件の構築
            Expression<Func<Incident, object>>? orderBy = searchDto.SortBy?.ToLower() switch
            {
                "title" => i => i.Title,
                "status" => i => i.Status,
                "priority" => i => i.Priority,
                "category" => i => i.Category,
                "reporteddate" => i => i.ReportedDate,
                "occurrencedate" => i => i.OccurrenceDate,
                "resolveddate" => i => i.ResolvedDate ?? DateTime.MaxValue,
                "createdat" => i => i.CreatedAt,
                "updatedat" => i => i.UpdatedAt,
                _ => i => i.OccurrenceDate
            };

            // ページネーション実行
            var (incidents, totalCount) = await _incidentRepository.GetPagedAsync(
                predicate,
                orderBy,
                searchDto.Ascending,
                searchDto.Page,
                searchDto.PageSize);

            // DTOに変換
            var incidentDtos = await ConvertToDtoBatchAsync(incidents.ToList());

            var result = new PagedResultDto<IncidentDto>(
                incidentDtos,
                totalCount,
                searchDto.Page,
                searchDto.PageSize);

            _logger.LogInformation("インシデント一覧取得成功: {Count}件", incidentDtos.Count());
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "インシデント一覧取得中にエラーが発生しました。");
            return StatusCode(500, new { Error = "インシデント一覧の取得中にエラーが発生しました。" });
        }
    }

    // GET: api/incidents/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<IncidentDto>> GetIncident(int id)
    {
        try
        {
            _logger.LogInformation("インシデント詳細取得リクエスト: ID={Id}", id);

            var incident = await _incidentRepository.GetByIdWithIncludeAsync(
                id,
                i => i.ReportedBy,
                i => i.AssignedTo,
                i => i.Attachments);

            if (incident == null)
            {
                _logger.LogWarning("インシデントが見つかりません: ID={Id}", id);
                return NotFound(new { Error = "インシデントが見つかりません。" });
            }

            var incidentDto = await ConvertToDtoAsync(incident);

            _logger.LogInformation("インシデント詳細取得成功: ID={Id}", id);
            return Ok(incidentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "インシデント詳細取得中にエラーが発生しました: ID={Id}", id);
            return StatusCode(500, new { Error = "インシデント詳細の取得中にエラーが発生しました。" });
        }
    }

    // POST: api/incidents
    [HttpPost]
    public async Task<ActionResult<IncidentDto>> CreateIncident([FromBody] CreateIncidentDto createDto)
    {
        try
        {
            if (createDto == null)
            {
                return BadRequest(new { Error = "リクエストボディが空です。" });
            }

            _logger.LogInformation("インシデント作成リクエスト: {@CreateDto}", createDto);
            _logger.LogInformation("Priority値: {Priority}", createDto.Priority);
            _logger.LogInformation("Priority型: {PriorityType}", createDto.Priority.GetType().Name);

            // 有効性確認日の前処理（空文字列をnullに変換）
            if (createDto.EffectivenessDate.HasValue && createDto.EffectivenessDate.Value == DateTime.MinValue)
            {
                createDto.EffectivenessDate = null;
            }

            // 原因と再発防止策の前処理（空文字列をnullに変換）
            if (string.IsNullOrWhiteSpace(createDto.Cause))
            {
                createDto.Cause = null;
            }
            if (string.IsNullOrWhiteSpace(createDto.PreventionMeasures))
            {
                createDto.PreventionMeasures = null;
            }

            // 報告者の存在確認
            var reportedBy = await _userRepository.GetByIdAsync(createDto.ReportedById);
            if (reportedBy == null)
            {
                return BadRequest(new { Error = "指定された報告者が存在しません。" });
            }

            // 割り当て先の存在確認（指定されている場合）
            if (createDto.AssignedToId.HasValue)
            {
                var assignedTo = await _userRepository.GetByIdAsync(createDto.AssignedToId.Value);
                if (assignedTo == null)
                {
                    return BadRequest(new { Error = "指定された割り当て先が存在しません。" });
                }
            }

            // マスタデータの参照存在チェック
            var troubleType = await _masterDataResolver.GetTroubleTypeAsync(createDto.TroubleType);
            if (troubleType == null)
            {
                return BadRequest(new { Error = $"指定されたトラブル種類（ID: {createDto.TroubleType}）が存在しません。" });
            }

            var damageType = await _masterDataResolver.GetDamageTypeAsync(createDto.DamageType);
            if (damageType == null)
            {
                return BadRequest(new { Error = $"指定された損傷種類（ID: {createDto.DamageType}）が存在しません。" });
            }

            var warehouse = await _masterDataResolver.GetWarehouseAsync(createDto.Warehouse);
            if (warehouse == null)
            {
                return BadRequest(new { Error = $"指定された出荷元倉庫（ID: {createDto.Warehouse}）が存在しません。" });
            }

            var shippingCompany = await _masterDataResolver.GetShippingCompanyAsync(createDto.ShippingCompany);
            if (shippingCompany == null)
            {
                return BadRequest(new { Error = $"指定された運送会社（ID: {createDto.ShippingCompany}）が存在しません。" });
            }

            // インシデント作成
            var newIncident = Incident.Create(
                createDto.Title,
                createDto.Description,
                createDto.Category,
                createDto.ReportedById,
                createDto.TroubleType,
                createDto.DamageType,
                createDto.Warehouse,
                createDto.ShippingCompany,
                createDto.OccurrenceDate,
                createDto.Priority,
                createDto.IncidentDetails,
                createDto.TotalShipments,
                createDto.DefectiveItems,
                createDto.OccurrenceLocation,
                createDto.Summary,
                createDto.Cause,
                createDto.PreventionMeasures);

            // 割り当て先が指定されている場合は割り当て
            if (createDto.AssignedToId.HasValue)
            {
                newIncident.AssignTo(createDto.AssignedToId.Value);
            }

            await _incidentRepository.AddAsync(newIncident);
            await _unitOfWork.SaveChangesAsync();

            var incidentDto = await ConvertToDtoAsync(newIncident);

            _logger.LogInformation("インシデント作成成功: ID={Id}", newIncident.Id);
            return CreatedAtAction(nameof(GetIncident), new { id = newIncident.Id }, incidentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "インシデント作成中にエラーが発生しました。");
            return StatusCode(500, new { Error = "インシデントの作成中にエラーが発生しました。" });
        }
    }

    // POST: api/incidents/clerk
    [HttpPost("clerk")]
    public async Task<ActionResult<IncidentDto>> CreateClerkIncident([FromBody] ClerkIncidentDto clerkDto)
    {
        try
        {
            if (clerkDto == null)
            {
                return BadRequest(new { Error = "リクエストボディが空です。" });
            }

            _logger.LogInformation("事務員インシデント作成リクエスト: {@ClerkDto}", clerkDto);

            // 報告者の存在確認
            var reportedBy = await _userRepository.GetByIdAsync(clerkDto.ReportedById);
            if (reportedBy == null)
            {
                return BadRequest(new { Error = "指定された報告者が存在しません。" });
            }

            // 事務員専用のインシデント作成（enum検証をスキップ）
            var newIncident = Incident.CreateForClerk(
                clerkDto.Title,
                clerkDto.Description,
                clerkDto.ReportedById,
                clerkDto.IncidentDetails,
                clerkDto.OccurrenceDate,
                clerkDto.OccurrenceLocation
            );

            await _incidentRepository.AddAsync(newIncident);
            await _unitOfWork.SaveChangesAsync();

            var incidentDto = await ConvertToDtoAsync(newIncident);

            _logger.LogInformation("事務員インシデント作成成功: ID={Id}", newIncident.Id);
            return CreatedAtAction(nameof(GetIncident), new { id = newIncident.Id }, incidentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "事務員インシデント作成中にエラーが発生しました。");
            return StatusCode(500, new { Error = "インシデントの作成中にエラーが発生しました。" });
        }
    }

    // PUT: api/incidents/clerk/{id}
    [HttpPut("clerk/{id}")]
    public async Task<ActionResult<IncidentDto>> UpdateClerkIncident(int id, [FromBody] ClerkIncidentDto clerkDto)
    {
        try
        {
            if (clerkDto == null)
            {
                return BadRequest(new { Error = "リクエストボディが空です。" });
            }

            _logger.LogInformation("事務員インシデント更新リクエスト ID: {Id}, Data: {@ClerkDto}", id, clerkDto);

            // インシデントの存在確認
            var incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return NotFound(new { Error = "指定されたインシデントが見つかりません。" });
            }

            // 権限チェック：事務員は自分が報告したインシデントのみ編集可能
            var currentUserId = GetCurrentUserId();
            if (incident.ReportedById != currentUserId)
            {
                return Forbid("自分が報告したインシデントのみ編集できます。");
            }

            // 事務員が編集可能な項目のみ更新
            incident.UpdateClerkEditableFields(
                clerkDto.Title,
                clerkDto.Description,
                clerkDto.IncidentDetails,
                clerkDto.OccurrenceDate,
                clerkDto.OccurrenceLocation
            );

            await _incidentRepository.UpdateAsync(incident);
            await _unitOfWork.SaveChangesAsync();

            var incidentDto = await ConvertToDtoAsync(incident);

            _logger.LogInformation("事務員インシデント更新成功: ID={Id}", incident.Id);
            return Ok(incidentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "事務員インシデント更新中にエラーが発生しました。");
            return StatusCode(500, new { Error = "インシデントの更新中にエラーが発生しました。" });
        }
    }

    // PUT: api/incidents/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIncident(int id, UpdateIncidentDto updateDto)
    {
        try
        {
            _logger.LogInformation("インシデント更新リクエスト: ID={Id}, {@UpdateDto}", id, updateDto);

            var incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                _logger.LogWarning("インシデントが見つかりません: ID={Id}", id);
                return NotFound(new { Error = "インシデントが見つかりません。" });
            }

            // 割り当て先の存在確認（指定されている場合）
            if (updateDto.AssignedToId.HasValue)
            {
                var assignedTo = await _userRepository.GetByIdAsync(updateDto.AssignedToId.Value);
                if (assignedTo == null)
                {
                    return BadRequest(new { Error = "指定された割り当て先が存在しません。" });
                }
            }

            // インシデント更新
            incident.UpdateDetails(updateDto.Title, updateDto.Description, updateDto.Category);
            incident.UpdatePriority(updateDto.Priority);

            // 物流特化項目の更新
            if (updateDto.TroubleType.HasValue && updateDto.DamageType.HasValue && 
                updateDto.Warehouse.HasValue && updateDto.ShippingCompany.HasValue)
            {
                // マスタデータの参照存在チェック
                var troubleType = await _masterDataResolver.GetTroubleTypeAsync(updateDto.TroubleType.Value);
                if (troubleType == null)
                {
                    return BadRequest(new { Error = $"指定されたトラブル種類（ID: {updateDto.TroubleType.Value}）が存在しません。" });
                }

                var damageType = await _masterDataResolver.GetDamageTypeAsync(updateDto.DamageType.Value);
                if (damageType == null)
                {
                    return BadRequest(new { Error = $"指定された損傷種類（ID: {updateDto.DamageType.Value}）が存在しません。" });
                }

                var warehouse = await _masterDataResolver.GetWarehouseAsync(updateDto.Warehouse.Value);
                if (warehouse == null)
                {
                    return BadRequest(new { Error = $"指定された出荷元倉庫（ID: {updateDto.Warehouse.Value}）が存在しません。" });
                }

                var shippingCompany = await _masterDataResolver.GetShippingCompanyAsync(updateDto.ShippingCompany.Value);
                if (shippingCompany == null)
                {
                    return BadRequest(new { Error = $"指定された運送会社（ID: {updateDto.ShippingCompany.Value}）が存在しません。" });
                }

                incident.UpdateLogisticsDetails(
                    updateDto.TroubleType.Value,
                    updateDto.DamageType.Value,
                    updateDto.Warehouse.Value,
                    updateDto.ShippingCompany.Value);
            }

            if (updateDto.EffectivenessStatus.HasValue)
            {
                incident.UpdateEffectivenessStatus(updateDto.EffectivenessStatus.Value);
            }

            // 新規追加項目の更新
            if (updateDto.IncidentDetails != null || updateDto.TotalShipments.HasValue || 
                updateDto.DefectiveItems.HasValue || updateDto.OccurrenceDate.HasValue || 
                updateDto.OccurrenceLocation != null || updateDto.Summary != null || 
                updateDto.Cause != null || updateDto.PreventionMeasures != null || 
                updateDto.EffectivenessDate.HasValue || updateDto.EffectivenessComment != null)
            {
                incident.UpdateExtendedDetails(
                    updateDto.IncidentDetails ?? incident.IncidentDetails,
                    updateDto.TotalShipments ?? incident.TotalShipments,
                    updateDto.DefectiveItems ?? incident.DefectiveItems,
                    updateDto.OccurrenceDate ?? incident.OccurrenceDate,
                    updateDto.OccurrenceLocation ?? incident.OccurrenceLocation,
                    updateDto.Summary ?? incident.Summary,
                    updateDto.Cause ?? incident.Cause,
                    updateDto.PreventionMeasures ?? incident.PreventionMeasures,
                    updateDto.EffectivenessDate ?? incident.EffectivenessDate,
                    updateDto.EffectivenessComment ?? incident.EffectivenessComment);
            }

            // ステータス更新
            if (incident.Status != updateDto.Status)
            {
                incident.UpdateStatus(updateDto.Status);
            }

            // 割り当て先更新
            if (updateDto.AssignedToId != incident.AssignedToId)
            {
                if (updateDto.AssignedToId.HasValue)
                {
                    incident.AssignTo(updateDto.AssignedToId.Value);
                }
                else
                {
                    incident.Unassign();
                }
            }

            // 解決内容の更新
            if (!string.IsNullOrWhiteSpace(updateDto.Resolution))
            {
                incident.Resolve(updateDto.Resolution);
            }

            await _incidentRepository.UpdateAsync(incident);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("インシデント更新成功: ID={Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "インシデント更新中にエラーが発生しました: ID={Id}", id);
            return StatusCode(500, new { Error = "インシデントの更新中にエラーが発生しました。" });
        }
    }

    // DELETE: api/incidents/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIncident(int id)
    {
        try
        {
            _logger.LogInformation("インシデント削除リクエスト: ID={Id}", id);

            var incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                _logger.LogWarning("インシデントが見つかりません: ID={Id}", id);
                return NotFound(new { Error = "インシデントが見つかりません。" });
            }

            await _incidentRepository.DeleteAsync(incident);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("インシデント削除成功: ID={Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "インシデント削除中にエラーが発生しました: ID={Id}", id);
            return StatusCode(500, new { Error = "インシデントの削除中にエラーが発生しました。" });
        }
    }

    // GET: api/incidents/status/{status}
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<IncidentDto>>> GetIncidentsByStatus(IncidentStatus status)
    {
        try
        {
            _logger.LogInformation("ステータス別インシデント取得リクエスト: Status={Status}", status);

            var incidents = await _incidentRepository.GetByStatusAsync(status);
            var incidentDtos = await ConvertToDtoBatchAsync(incidents.ToList());

            _logger.LogInformation("ステータス別インシデント取得成功: Status={Status}, Count={Count}", status, incidentDtos.Count());
            return Ok(incidentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ステータス別インシデント取得中にエラーが発生しました: Status={Status}", status);
            return StatusCode(500, new { Error = "ステータス別インシデントの取得中にエラーが発生しました。" });
        }
    }

    // GET: api/incidents/priority/{priority}
    [HttpGet("priority/{priority}")]
    public async Task<ActionResult<IEnumerable<IncidentDto>>> GetIncidentsByPriority(Priority priority)
    {
        try
        {
            _logger.LogInformation("優先度別インシデント取得リクエスト: Priority={Priority}", priority);

            var incidents = await _incidentRepository.GetByPriorityAsync(priority);
            var incidentDtos = await ConvertToDtoBatchAsync(incidents.ToList());

            _logger.LogInformation("優先度別インシデント取得成功: Priority={Priority}, Count={Count}", priority, incidentDtos.Count());
            return Ok(incidentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "優先度別インシデント取得中にエラーが発生しました: Priority={Priority}", priority);
            return StatusCode(500, new { Error = "優先度別インシデントの取得中にエラーが発生しました。" });
        }
    }

    // GET: api/incidents/active
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<IncidentDto>>> GetActiveIncidents()
    {
        try
        {
            _logger.LogInformation("アクティブインシデント取得リクエスト");

            var incidents = await _incidentRepository.GetActiveIncidentsAsync();
            var incidentDtos = await ConvertToDtoBatchAsync(incidents.ToList());

            _logger.LogInformation("アクティブインシデント取得成功: Count={Count}", incidentDtos.Count());
            return Ok(incidentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "アクティブインシデント取得中にエラーが発生しました。");
            return StatusCode(500, new { Error = "アクティブインシデントの取得中にエラーが発生しました。" });
        }
    }

    // GET: api/incidents/resolved
    [HttpGet("resolved")]
    public async Task<ActionResult<IEnumerable<IncidentDto>>> GetResolvedIncidents()
    {
        try
        {
            _logger.LogInformation("解決済みインシデント取得リクエスト");

            var incidents = await _incidentRepository.GetResolvedIncidentsAsync();
            var incidentDtos = await ConvertToDtoBatchAsync(incidents.ToList());

            _logger.LogInformation("解決済みインシデント取得成功: Count={Count}", incidentDtos.Count());
            return Ok(incidentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解決済みインシデント取得中にエラーが発生しました。");
            return StatusCode(500, new { Error = "解決済みインシデントの取得中にエラーが発生しました。" });
        }
    }

    // プライベートメソッド: DTO変換
    private async Task<IncidentDto> ConvertToDtoAsync(Incident incident)
    {
        var reportedBy = await _userRepository.GetByIdAsync(incident.ReportedById);
        var assignedTo = incident.AssignedToId.HasValue 
            ? await _userRepository.GetByIdAsync(incident.AssignedToId.Value) 
            : null;

        // マスタ情報の取得
        _logger.LogInformation("インシデントマスタデータ取得: TroubleTypeId={TroubleTypeId}, DamageTypeId={DamageTypeId}, WarehouseId={WarehouseId}, ShippingCompanyId={ShippingCompanyId}", 
            incident.TroubleTypeId, incident.DamageTypeId, incident.WarehouseId, incident.ShippingCompanyId);
        
        var (troubleType, damageType, warehouse, shippingCompany) = await _masterDataResolver.GetIncidentMasterDataAsync(
            incident.TroubleTypeId, incident.DamageTypeId, incident.WarehouseId, incident.ShippingCompanyId);
        
        _logger.LogInformation("マスタデータ取得結果: TroubleType={TroubleType}, DamageType={DamageType}, Warehouse={Warehouse}, ShippingCompany={ShippingCompany}", 
            troubleType?.Name, damageType?.Name, warehouse?.Name, shippingCompany?.Name);

        return new IncidentDto
        {
            Id = incident.Id,
            Title = incident.Title,
            Description = incident.Description,
            Status = incident.Status,
            Priority = incident.Priority,
            Category = incident.Category,
            TroubleTypeId = incident.TroubleTypeId,
            DamageTypeId = incident.DamageTypeId,
            WarehouseId = incident.WarehouseId,
            ShippingCompanyId = incident.ShippingCompanyId,
            EffectivenessStatus = incident.EffectivenessStatus,
            // 表示用のマスタ情報
            TroubleTypeName = troubleType?.Name ?? "不明",
            TroubleTypeColor = troubleType?.Color ?? "#3B82F6",
            DamageTypeName = damageType?.Name ?? "不明",
            WarehouseName = warehouse?.Name ?? "不明",
            ShippingCompanyName = shippingCompany?.Name ?? "不明",
            IncidentDetails = incident.IncidentDetails,
            TotalShipments = incident.TotalShipments,
            DefectiveItems = incident.DefectiveItems,
            OccurrenceDate = incident.OccurrenceDate,
            OccurrenceLocation = incident.OccurrenceLocation,
            Summary = incident.Summary,
            Cause = incident.Cause,
            PreventionMeasures = incident.PreventionMeasures,
            EffectivenessDate = incident.EffectivenessDate,
            EffectivenessComment = incident.EffectivenessComment,
            ReportedById = incident.ReportedById,
            ReportedByName = reportedBy?.GetFullName() ?? "不明",
            AssignedToId = incident.AssignedToId,
            AssignedToName = assignedTo?.GetFullName(),
            ReportedDate = incident.ReportedDate,
            ResolvedDate = incident.ResolvedDate,
            Resolution = incident.Resolution,
            CreatedAt = incident.CreatedAt,
            UpdatedAt = incident.UpdatedAt,
            AttachmentCount = incident.Attachments.Count,
            IsOverdue = _incidentDomainService.IsIncidentOverdue(incident, DefaultExpectedResolutionTime),
            ResolutionTime = incident.IsResolved() ? incident.GetResolutionTime() : null
        };
    }

    private async Task<IEnumerable<IncidentDto>> ConvertToDtoAsync(IEnumerable<Incident> incidents)
    {
        var dtos = new List<IncidentDto>();
        foreach (var incident in incidents)
        {
            dtos.Add(await ConvertToDtoAsync(incident));
        }
        return dtos;
    }

    /// <summary>
    /// バッチ処理版のDTO変換（N+1問題を解決）
    /// </summary>
    private async Task<IEnumerable<IncidentDto>> ConvertToDtoBatchAsync(IEnumerable<Incident> incidents)
    {
        if (!incidents.Any())
        {
            return Enumerable.Empty<IncidentDto>();
        }

        try
        {
            // 個別取得と同じロジックを使用（バッチ処理の問題を回避）
            var dtos = new List<IncidentDto>();

            foreach (var incident in incidents)
            {
                var dto = await ConvertToDtoAsync(incident);
                dtos.Add(dto);
            }

            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "バッチDTO変換中にエラーが発生しました。個別変換にフォールバックします。");
            
            // フォールバック：個別変換
            var dtos = new List<IncidentDto>();
            foreach (var incident in incidents)
            {
                try
                {
                    var dto = await ConvertToDtoAsync(incident);
                    dtos.Add(dto);
                }
                catch (Exception innerEx)
                {
                    _logger.LogError(innerEx, "インシデント {Id} のDTO変換中にエラーが発生しました", incident.Id);
                }
            }
            return dtos;
        }
    }

    /// <summary>
    /// バッチ処理版のDTO変換（N+1問題を解決）- 旧実装（問題あり）
    /// </summary>
    private async Task<IEnumerable<IncidentDto>> ConvertToDtoBatchAsync_Old(IEnumerable<Incident> incidents)
    {
        if (!incidents.Any())
        {
            return Enumerable.Empty<IncidentDto>();
        }

        try
        {
            // マスタデータを一括取得
            var masterDataBatch = await _masterDataResolver.GetIncidentMasterDataBatchAsync(incidents);
            
            var dtos = new List<IncidentDto>();

            foreach (var incident in incidents)
            {
                // 辞書からマスタデータを取得（DBクエリなし）
                masterDataBatch.TroubleTypes.TryGetValue(incident.TroubleTypeId, out var troubleType);
                masterDataBatch.DamageTypes.TryGetValue(incident.DamageTypeId, out var damageType);
                masterDataBatch.Warehouses.TryGetValue(incident.WarehouseId, out var warehouse);
                masterDataBatch.ShippingCompanies.TryGetValue(incident.ShippingCompanyId, out var shippingCompany);
                masterDataBatch.Users.TryGetValue(incident.ReportedById, out var reportedBy);
                
                User? assignedTo = null;
                if (incident.AssignedToId.HasValue)
                {
                    masterDataBatch.Users.TryGetValue(incident.AssignedToId.Value, out assignedTo);
                }

                var dto = new IncidentDto
                {
                    Id = incident.Id,
                    Title = incident.Title,
                    Description = incident.Description,
                    Status = incident.Status,
                    Priority = incident.Priority,
                    Category = incident.Category,
                    TroubleTypeId = incident.TroubleTypeId,
                    DamageTypeId = incident.DamageTypeId,
                    WarehouseId = incident.WarehouseId,
                    ShippingCompanyId = incident.ShippingCompanyId,
                    EffectivenessStatus = incident.EffectivenessStatus,
                    // 表示用のマスタ情報
                    TroubleTypeName = troubleType?.Name ?? "不明",
                    TroubleTypeColor = troubleType?.Color ?? "#3B82F6",
                    DamageTypeName = damageType?.Name ?? "不明",
                    WarehouseName = warehouse?.Name ?? "不明",
                    ShippingCompanyName = shippingCompany?.Name ?? "不明",
                    IncidentDetails = incident.IncidentDetails,
                    TotalShipments = incident.TotalShipments,
                    DefectiveItems = incident.DefectiveItems,
                    OccurrenceDate = incident.OccurrenceDate,
                    OccurrenceLocation = incident.OccurrenceLocation,
                    Summary = incident.Summary,
                    Cause = incident.Cause,
                    PreventionMeasures = incident.PreventionMeasures,
                    EffectivenessDate = incident.EffectivenessDate,
                    EffectivenessComment = incident.EffectivenessComment,
                    ReportedById = incident.ReportedById,
                    ReportedByName = reportedBy?.GetFullName() ?? "不明",
                    AssignedToId = incident.AssignedToId,
                    AssignedToName = assignedTo?.GetFullName(),
                    ReportedDate = incident.ReportedDate,
                    ResolvedDate = incident.ResolvedDate,
                    Resolution = incident.Resolution,
                    CreatedAt = incident.CreatedAt,
                    UpdatedAt = incident.UpdatedAt,
                    AttachmentCount = incident.Attachments.Count,
                    IsOverdue = _incidentDomainService.IsIncidentOverdue(incident, DefaultExpectedResolutionTime),
                    ResolutionTime = incident.IsResolved() ? incident.GetResolutionTime() : null
                };

                dtos.Add(dto);
            }

            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "バッチDTO変換中にエラーが発生しました。フォールバックとして個別変換を使用します。");
            // エラーが発生した場合は、既存の個別変換にフォールバック
            return await ConvertToDtoAsync(incidents);
        }
    }

    #region 新ワークフロー関連エンドポイント

    /// <summary>
    /// 新ワークフローを有効化する
    /// </summary>
    [HttpPost("{id}/enable-workflow")]
    public async Task<ActionResult<WorkflowActionResultDto>> EnableWorkflow(int id)
    {
        try
        {
            var incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return NotFound(new { Error = "指定されたインシデントが見つかりません。" });
            }

            // 新仕様では常に新ワークフローのため、このチェックは不要

            // EnableNewWorkflowは削除 - 新仕様では常に新ワークフロー
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("インシデント {IncidentId} のワークフローを有効化しました", id);

            return Ok(new WorkflowActionResultDto
            {
                Success = true,
                Message = "ワークフローが有効化されました。",
                NewStatus = incident.Status,
                AvailableActions = incident.GetAvailableWorkflowActions(GetCurrentUserRole())
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ワークフロー有効化中にエラーが発生しました: ID={Id}", id);
            return StatusCode(500, new { Error = "ワークフローの有効化中にエラーが発生しました。" });
        }
    }

    /// <summary>
    /// インシデントを分類する（未分類 → 未対応）
    /// </summary>
    [HttpPost("{id}/classify")]
    [Authorize(Roles = "Admin,Incident Manager")]
    public async Task<ActionResult<WorkflowActionResultDto>> ClassifyIncident(int id, [FromBody] ClassifyIncidentDto dto)
    {
        try
        {
            var incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return NotFound(new { Error = "指定されたインシデントが見つかりません。" });
            }

            // 新仕様では常に新ワークフロー

            // マスタデータ存在チェック
            var masterDataValid = await _workflowValidationService.ValidateMasterDataReferencesAsync(
                dto.TroubleTypeId, dto.DamageTypeId, dto.WarehouseId, dto.ShippingCompanyId);
            if (!masterDataValid)
            {
                return BadRequest(new { Error = "指定されたマスタデータが存在しません。" });
            }

            // 分類データの妥当性チェック
            if (!_workflowValidationService.ValidateClassificationData(dto.TotalShipments, dto.DefectiveItems))
            {
                return BadRequest(new { Error = "分類データが不正です。不良品数は出荷総数以下である必要があります。" });
            }

            // 対応期限の妥当性チェック
            if (!_workflowValidationService.ValidateDueDate(dto.DueDate))
            {
                return BadRequest(new { Error = "対応期限が不正です。未来の日付を指定してください。" });
            }

            incident.Classify(dto.TroubleTypeId, dto.DamageTypeId, dto.WarehouseId, 
                dto.ShippingCompanyId, dto.TotalShipments, dto.DefectiveItems, 
                dto.Priority, dto.DueDate);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("インシデント {IncidentId} を分類しました", id);

            return Ok(new WorkflowActionResultDto
            {
                Success = true,
                Message = "インシデントが分類されました。",
                NewStatus = incident.Status,
                AvailableActions = incident.GetAvailableWorkflowActions(GetCurrentUserRole())
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "インシデント分類中にエラーが発生しました: ID={Id}", id);
            return StatusCode(500, new { Error = "インシデントの分類中にエラーが発生しました。" });
        }
    }

    /// <summary>
    /// 対応を開始する（未対応 → 対応中）
    /// </summary>
    [HttpPost("{id}/start-response")]
    [Authorize(Roles = "Admin,Warehouse Staff")]
    public async Task<ActionResult<WorkflowActionResultDto>> StartResponse(int id)
    {
        try
        {
            var incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return NotFound(new { Error = "指定されたインシデントが見つかりません。" });
            }

            incident.StartResponse();
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("インシデント {IncidentId} の対応を開始しました", id);

            return Ok(new WorkflowActionResultDto
            {
                Success = true,
                Message = "対応を開始しました。",
                NewStatus = incident.Status,
                AvailableActions = incident.GetAvailableWorkflowActions(GetCurrentUserRole())
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "対応開始中にエラーが発生しました: ID={Id}", id);
            return StatusCode(500, new { Error = "対応開始中にエラーが発生しました。" });
        }
    }

    /// <summary>
    /// 原因を入力する（対応中状態を維持）
    /// </summary>
    [HttpPost("{id}/analyze-cause")]
    [Authorize(Roles = "Admin,Warehouse Staff")]
    public async Task<ActionResult<WorkflowActionResultDto>> AnalyzeCause(int id, [FromBody] AnalyzeCauseDto dto)
    {
        try
        {
            var incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return NotFound(new { Error = "指定されたインシデントが見つかりません。" });
            }

            incident.AnalyzeCause(dto.Cause);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("インシデント {IncidentId} の原因を入力しました", id);

            return Ok(new WorkflowActionResultDto
            {
                Success = true,
                Message = "原因が入力されました。",
                NewStatus = incident.Status,
                AvailableActions = incident.GetAvailableWorkflowActions(GetCurrentUserRole())
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "原因入力中にエラーが発生しました: ID={Id}", id);
            return StatusCode(500, new { Error = "原因入力中にエラーが発生しました。" });
        }
    }

    /// <summary>
    /// 対応を完了する（対応中 → 対応済）
    /// </summary>
    [HttpPost("{id}/complete-response")]
    [Authorize(Roles = "Admin,Warehouse Staff")]
    public async Task<ActionResult<WorkflowActionResultDto>> CompleteResponse(int id, [FromBody] CompleteResponseDto dto)
    {
        try
        {
            var incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return NotFound(new { Error = "指定されたインシデントが見つかりません。" });
            }

            incident.CompleteResponse(dto.ResponseContent);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("インシデント {IncidentId} の対応を完了しました", id);

            return Ok(new WorkflowActionResultDto
            {
                Success = true,
                Message = "対応が完了しました。",
                NewStatus = incident.Status,
                AvailableActions = incident.GetAvailableWorkflowActions(GetCurrentUserRole())
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "対応完了中にエラーが発生しました: ID={Id}", id);
            return StatusCode(500, new { Error = "対応完了中にエラーが発生しました。" });
        }
    }

    /// <summary>
    /// 再発防止策を提案する（対応済 → 再発防止策提案済）
    /// </summary>
    [HttpPost("{id}/propose-prevention")]
    [Authorize(Roles = "Admin,Warehouse Staff")]
    public async Task<ActionResult<WorkflowActionResultDto>> ProposePreventionMeasures(int id, [FromBody] ProposePreventionDto dto)
    {
        try
        {
            var incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return NotFound(new { Error = "指定されたインシデントが見つかりません。" });
            }

            incident.ProposePreventionMeasures(dto.PreventionMeasures);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("インシデント {IncidentId} の再発防止策を提案しました", id);

            return Ok(new WorkflowActionResultDto
            {
                Success = true,
                Message = "再発防止策が提案されました。",
                NewStatus = incident.Status,
                AvailableActions = incident.GetAvailableWorkflowActions(GetCurrentUserRole())
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "再発防止策提案中にエラーが発生しました: ID={Id}", id);
            return StatusCode(500, new { Error = "再発防止策提案中にエラーが発生しました。" });
        }
    }

    /// <summary>
    /// 有効性を確認する（再発防止策提案済 → 有効性確認済）
    /// </summary>
    [HttpPost("{id}/confirm-effectiveness")]
    [Authorize(Roles = "Admin,Incident Manager")]
    public async Task<ActionResult<WorkflowActionResultDto>> ConfirmEffectiveness(int id, [FromBody] ConfirmEffectivenessDto dto)
    {
        try
        {
            var incident = await _incidentRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return NotFound(new { Error = "指定されたインシデントが見つかりません。" });
            }

            incident.ConfirmEffectiveness(dto.EffectivenessStatus, dto.EffectivenessComment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("インシデント {IncidentId} の有効性を確認しました", id);

            return Ok(new WorkflowActionResultDto
            {
                Success = true,
                Message = "有効性が確認されました。",
                NewStatus = incident.Status,
                AvailableActions = incident.GetAvailableWorkflowActions(GetCurrentUserRole())
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "有効性確認中にエラーが発生しました: ID={Id}", id);
            return StatusCode(500, new { Error = "有効性確認中にエラーが発生しました。" });
        }
    }

    /// <summary>
    /// ワークフロー統計を取得する
    /// </summary>
    [HttpGet("workflow-statistics")]
    public async Task<ActionResult<WorkflowStatisticsDto>> GetWorkflowStatistics()
    {
        try
        {
            var incidents = await _incidentRepository.GetAllAsync();

            var stats = new WorkflowStatisticsDto
            {
                UnclassifiedCount = incidents.Count(i => i.Status == IncidentStatus.Unclassified),
                PendingCount = incidents.Count(i => i.Status == IncidentStatus.Pending),
                InProgressCount = incidents.Count(i => i.Status == IncidentStatus.InProgress),
                CompletedCount = incidents.Count(i => i.Status == IncidentStatus.Completed),
                PreventionProposedCount = incidents.Count(i => i.Status == IncidentStatus.PreventionProposed),
                EffectivenessConfirmedCount = incidents.Count(i => i.Status == IncidentStatus.EffectivenessConfirmed),
                TotalWithWorkflow = incidents.Count(), // 新仕様では全て新ワークフロー
                TotalLegacyMode = 0 // レガシーモードはなし
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ワークフロー統計取得中にエラーが発生しました");
            return StatusCode(500, new { Error = "ワークフロー統計の取得中にエラーが発生しました。" });
        }
    }

    /// <summary>
    /// 現在のユーザーのロールを取得する（ヘルパーメソッド）
    /// </summary>
    private new string GetCurrentUserRole()
    {
        // BaseControllerから継承したGetCurrentUserRoleメソッドを使用
        return base.GetCurrentUserRole() ?? "Unknown";
    }

    #endregion
}
