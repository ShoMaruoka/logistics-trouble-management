using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LogisticsTroubleManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncidentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIncidentRepository _incidentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IncidentDomainService _incidentDomainService;
    private readonly ITroubleTypeRepository _troubleTypeRepository;
    private readonly IDamageTypeRepository _damageTypeRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IShippingCompanyRepository _shippingCompanyRepository;
    private readonly ILogger<IncidentsController> _logger;

    public IncidentsController(
        IUnitOfWork unitOfWork,
        IIncidentRepository incidentRepository,
        IUserRepository userRepository,
        IncidentDomainService incidentDomainService,
        ITroubleTypeRepository troubleTypeRepository,
        IDamageTypeRepository damageTypeRepository,
        IWarehouseRepository warehouseRepository,
        IShippingCompanyRepository shippingCompanyRepository,
        ILogger<IncidentsController> logger)
    {
        _unitOfWork = unitOfWork;
        _incidentRepository = incidentRepository;
        _userRepository = userRepository;
        _incidentDomainService = incidentDomainService;
        _troubleTypeRepository = troubleTypeRepository;
        _damageTypeRepository = damageTypeRepository;
        _warehouseRepository = warehouseRepository;
        _shippingCompanyRepository = shippingCompanyRepository;
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
                var expectedResolutionTime = TimeSpan.FromDays(3); // デフォルト3日
                predicates.Add(i => _incidentDomainService.IsIncidentOverdue(i, expectedResolutionTime));
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
            var incidentDtos = await ConvertToDtoAsync(incidents.ToList());

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
            if (!string.IsNullOrWhiteSpace(updateDto.Resolution) && 
                (updateDto.Status == IncidentStatus.Resolved || updateDto.Status == IncidentStatus.Closed))
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
            var incidentDtos = await ConvertToDtoAsync(incidents.ToList());

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
            var incidentDtos = await ConvertToDtoAsync(incidents.ToList());

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
            var incidentDtos = await ConvertToDtoAsync(incidents.ToList());

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
            var incidentDtos = await ConvertToDtoAsync(incidents.ToList());

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
        var troubleType = await _troubleTypeRepository.GetByIdAsync(incident.TroubleTypeId);
        var damageType = await _damageTypeRepository.GetByIdAsync(incident.DamageTypeId);
        var warehouse = await _warehouseRepository.GetByIdAsync(incident.WarehouseId);
        var shippingCompany = await _shippingCompanyRepository.GetByIdAsync(incident.ShippingCompanyId);

        var expectedResolutionTime = TimeSpan.FromDays(3); // デフォルト3日

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
            IsOverdue = _incidentDomainService.IsIncidentOverdue(incident, expectedResolutionTime),
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
}
