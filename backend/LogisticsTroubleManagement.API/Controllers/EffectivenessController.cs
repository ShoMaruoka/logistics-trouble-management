using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsTroubleManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EffectivenessController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEffectivenessRepository _effectivenessRepository;
    private readonly IIncidentRepository _incidentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<EffectivenessController> _logger;

    public EffectivenessController(
        IUnitOfWork unitOfWork,
        IEffectivenessRepository effectivenessRepository,
        IIncidentRepository incidentRepository,
        IUserRepository userRepository,
        ILogger<EffectivenessController> logger)
    {
        _unitOfWork = unitOfWork;
        _effectivenessRepository = effectivenessRepository;
        _incidentRepository = incidentRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    // GET: api/effectiveness
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<EffectivenessDto>>> GetEffectiveness([FromQuery] EffectivenessSearchDto searchDto)
    {
        try
        {
            _logger.LogInformation("効果測定一覧取得リクエスト");

            var predicate = BuildSearchPredicate(searchDto);
            var orderBy = GetOrderByExpression(searchDto.SortBy);

            var (effectivenessList, totalCount) = await _effectivenessRepository.GetPagedAsync(
                predicate,
                orderBy,
                searchDto.Ascending,
                searchDto.Page,
                searchDto.PageSize);

            var effectivenessDtos = new List<EffectivenessDto>();
            foreach (var effectiveness in effectivenessList)
            {
                var dto = await ConvertToDtoAsync(effectiveness);
                effectivenessDtos.Add(dto);
            }

            var result = new PagedResultDto<EffectivenessDto>(
                effectivenessDtos,
                totalCount,
                searchDto.Page,
                searchDto.PageSize);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "効果測定一覧取得中にエラーが発生しました。");
            return StatusCode(500, new { Error = "効果測定一覧取得中にエラーが発生しました。" });
        }
    }

    // GET: api/effectiveness/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<EffectivenessDto>> GetEffectiveness(int id)
    {
        try
        {
            _logger.LogInformation($"効果測定詳細取得リクエスト (ID: {id})");

            var effectiveness = await _effectivenessRepository.GetByIdWithIncludeAsync(id, e => e.Incident, e => e.MeasuredBy);
            if (effectiveness == null)
            {
                return NotFound(new { Error = "指定された効果測定が見つかりません。" });
            }

            var dto = await ConvertToDtoAsync(effectiveness);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"効果測定詳細取得中にエラーが発生しました。 (ID: {id})");
            return StatusCode(500, new { Error = "効果測定詳細取得中にエラーが発生しました。" });
        }
    }

    // GET: api/effectiveness/incident/{incidentId}
    [HttpGet("incident/{incidentId}")]
    public async Task<ActionResult<IEnumerable<EffectivenessDto>>> GetEffectivenessByIncident(int incidentId)
    {
        try
        {
            _logger.LogInformation($"インシデント別効果測定一覧取得リクエスト (IncidentID: {incidentId})");

            var effectivenessList = await _effectivenessRepository.GetEffectivenessByIncidentAsync(incidentId);
            var effectivenessDtos = new List<EffectivenessDto>();

            foreach (var effectiveness in effectivenessList)
            {
                var dto = await ConvertToDtoAsync(effectiveness);
                effectivenessDtos.Add(dto);
            }

            return Ok(effectivenessDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"インシデント別効果測定一覧取得中にエラーが発生しました。 (IncidentID: {incidentId})");
            return StatusCode(500, new { Error = "インシデント別効果測定一覧取得中にエラーが発生しました。" });
        }
    }

    // GET: api/effectiveness/summary
    [HttpGet("summary")]
    public async Task<ActionResult<EffectivenessSummaryDto>> GetEffectivenessSummary()
    {
        try
        {
            _logger.LogInformation("効果測定サマリー取得リクエスト");

            var allEffectiveness = await _effectivenessRepository.GetAllAsync();
            var effectivenessList = allEffectiveness.ToList();

            if (!effectivenessList.Any())
            {
                return Ok(new EffectivenessSummaryDto());
            }

            var summary = new EffectivenessSummaryDto
            {
                TotalMeasurements = effectivenessList.Count,
                AverageImprovementRate = effectivenessList.Average(e => e.ImprovementRate),
                MaxImprovementRate = effectivenessList.Max(e => e.ImprovementRate),
                MinImprovementRate = effectivenessList.Min(e => e.ImprovementRate),
                EffectivenessTypeCounts = effectivenessList
                    .GroupBy(e => e.EffectivenessType)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            // 最近の測定結果（最新5件）
            var recentEffectiveness = effectivenessList
                .OrderByDescending(e => e.MeasuredAt)
                .Take(5);

            foreach (var effectiveness in recentEffectiveness)
            {
                var dto = await ConvertToDtoAsync(effectiveness);
                summary.RecentMeasurements.Add(dto);
            }

            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "効果測定サマリー取得中にエラーが発生しました。");
            return StatusCode(500, new { Error = "効果測定サマリー取得中にエラーが発生しました。" });
        }
    }

    // POST: api/effectiveness
    [HttpPost]
    public async Task<ActionResult<EffectivenessDto>> CreateEffectiveness([FromBody] CreateEffectivenessDto createDto)
    {
        try
        {
            _logger.LogInformation("効果測定登録リクエスト");

            // インシデントの存在確認
            var incident = await _incidentRepository.GetByIdAsync(createDto.IncidentId);
            if (incident == null)
            {
                return BadRequest(new { Error = "指定されたインシデントが見つかりません。" });
            }

            // ユーザーの存在確認
            var user = await _userRepository.GetByIdAsync(createDto.MeasuredById);
            if (user == null)
            {
                return BadRequest(new { Error = "指定されたユーザーが見つかりません。" });
            }

            // 改善率の計算
            var improvementRate = CalculateImprovementRate(createDto.BeforeValue, createDto.AfterValue);

            var effectiveness = Domain.Entities.Effectiveness.Create(
                createDto.IncidentId,
                createDto.EffectivenessType,
                createDto.BeforeValue,
                createDto.AfterValue,
                improvementRate,
                createDto.Description,
                createDto.MeasuredById);

            var createdEffectiveness = await _effectivenessRepository.AddAsync(effectiveness);
            await _unitOfWork.SaveChangesAsync();

            var dto = await ConvertToDtoAsync(createdEffectiveness);
            return CreatedAtAction(nameof(GetEffectiveness), new { id = dto.Id }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "効果測定登録中にエラーが発生しました。");
            return StatusCode(500, new { Error = "効果測定登録中にエラーが発生しました。" });
        }
    }

    // PUT: api/effectiveness/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<EffectivenessDto>> UpdateEffectiveness(int id, [FromBody] UpdateEffectivenessDto updateDto)
    {
        try
        {
            _logger.LogInformation($"効果測定更新リクエスト (ID: {id})");

            var effectiveness = await _effectivenessRepository.GetByIdAsync(id);
            if (effectiveness == null)
            {
                return NotFound(new { Error = "指定された効果測定が見つかりません。" });
            }

            // 改善率の再計算
            var improvementRate = CalculateImprovementRate(updateDto.BeforeValue, updateDto.AfterValue);

            effectiveness.UpdateEffectiveness(
                updateDto.EffectivenessType,
                updateDto.BeforeValue,
                updateDto.AfterValue,
                improvementRate,
                updateDto.Description);

            var updatedEffectiveness = await _effectivenessRepository.UpdateAsync(effectiveness);
            await _unitOfWork.SaveChangesAsync();

            var dto = await ConvertToDtoAsync(updatedEffectiveness);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"効果測定更新中にエラーが発生しました。 (ID: {id})");
            return StatusCode(500, new { Error = "効果測定更新中にエラーが発生しました。" });
        }
    }

    // DELETE: api/effectiveness/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEffectiveness(int id)
    {
        try
        {
            _logger.LogInformation($"効果測定削除リクエスト (ID: {id})");

            var effectiveness = await _effectivenessRepository.GetByIdAsync(id);
            if (effectiveness == null)
            {
                return NotFound(new { Error = "指定された効果測定が見つかりません。" });
            }

            await _effectivenessRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"効果測定削除中にエラーが発生しました。 (ID: {id})");
            return StatusCode(500, new { Error = "効果測定削除中にエラーが発生しました。" });
        }
    }

    private async Task<EffectivenessDto> ConvertToDtoAsync(Domain.Entities.Effectiveness effectiveness)
    {
        var incident = await _incidentRepository.GetByIdAsync(effectiveness.IncidentId);
        var measuredBy = await _userRepository.GetByIdAsync(effectiveness.MeasuredById);

        return new EffectivenessDto
        {
            Id = effectiveness.Id,
            IncidentId = effectiveness.IncidentId,
            IncidentTitle = incident?.Title ?? "不明",
            EffectivenessType = effectiveness.EffectivenessType,
            BeforeValue = effectiveness.BeforeValue,
            AfterValue = effectiveness.AfterValue,
            ImprovementRate = effectiveness.ImprovementRate,
            Description = effectiveness.Description,
            MeasuredAt = effectiveness.MeasuredAt,
            MeasuredBy = measuredBy?.GetFullName() ?? "不明",
            CreatedAt = effectiveness.CreatedAt,
            UpdatedAt = effectiveness.UpdatedAt
        };
    }

    private decimal CalculateImprovementRate(decimal beforeValue, decimal afterValue)
    {
        if (beforeValue == 0)
        {
            return afterValue > 0 ? 100 : 0;
        }

        return Math.Round(((afterValue - beforeValue) / beforeValue) * 100, 2);
    }

    private System.Linq.Expressions.Expression<Func<Domain.Entities.Effectiveness, bool>>? BuildSearchPredicate(EffectivenessSearchDto searchDto)
    {
        if (searchDto.IncidentId.HasValue)
        {
            return e => e.IncidentId == searchDto.IncidentId.Value;
        }

        if (!string.IsNullOrEmpty(searchDto.EffectivenessType))
        {
            return e => e.EffectivenessType.Contains(searchDto.EffectivenessType);
        }

        if (searchDto.MeasuredById.HasValue)
        {
            return e => e.MeasuredById == searchDto.MeasuredById.Value;
        }

        if (searchDto.MeasuredFrom.HasValue || searchDto.MeasuredTo.HasValue)
        {
            return e => (!searchDto.MeasuredFrom.HasValue || e.MeasuredAt >= searchDto.MeasuredFrom.Value) &&
                       (!searchDto.MeasuredTo.HasValue || e.MeasuredAt <= searchDto.MeasuredTo.Value);
        }

        if (searchDto.MinImprovementRate.HasValue || searchDto.MaxImprovementRate.HasValue)
        {
            return e => (!searchDto.MinImprovementRate.HasValue || e.ImprovementRate >= searchDto.MinImprovementRate.Value) &&
                       (!searchDto.MaxImprovementRate.HasValue || e.ImprovementRate <= searchDto.MaxImprovementRate.Value);
        }

        return null;
    }

    private System.Linq.Expressions.Expression<Func<Domain.Entities.Effectiveness, object>>? GetOrderByExpression(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "effectivenesstype" => e => e.EffectivenessType,
            "beforevalue" => e => e.BeforeValue,
            "aftervalue" => e => e.AfterValue,
            "improvementrate" => e => e.ImprovementRate,
            "incidentid" => e => e.IncidentId,
            "measuredby" => e => e.MeasuredById,
            _ => e => e.MeasuredAt
        };
    }
}
