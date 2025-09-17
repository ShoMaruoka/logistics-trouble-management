using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogisticsTroubleManagement.API.Controllers;

/// <summary>
/// 倉庫担当専用APIコントローラー
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehouseStaffController : ControllerBase
{
    private readonly IWarehouseStaffService _warehouseStaffService;
    private readonly ILogger<WarehouseStaffController> _logger;

    public WarehouseStaffController(IWarehouseStaffService warehouseStaffService, ILogger<WarehouseStaffController> logger)
    {
        _warehouseStaffService = warehouseStaffService;
        _logger = logger;
    }

    /// <summary>
    /// 倉庫担当ダッシュボードの統計情報を取得
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult<WarehouseStaffDashboardDto>> GetDashboard()
    {
        try
        {
            var userId = GetCurrentUserId();
            var dashboard = await _warehouseStaffService.GetDashboardAsync(userId);
            return Ok(dashboard);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "ユーザー {UserId} が倉庫担当ダッシュボードにアクセスしようとしましたが、権限がありません。", GetCurrentUserId());
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "倉庫担当ダッシュボードの取得中にエラーが発生しました。");
            return StatusCode(500, "内部サーバーエラーが発生しました。");
        }
    }

    /// <summary>
    /// 担当倉庫のインシデント一覧を取得
    /// </summary>
    [HttpGet("incidents/assigned-warehouse")]
    public async Task<ActionResult<PagedResultDto<WarehouseStaffIncidentDto>>> GetAssignedWarehouseIncidents(
        [FromQuery] WarehouseStaffIncidentSearchDto searchDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var incidents = await _warehouseStaffService.GetAssignedWarehouseIncidentsAsync(userId, searchDto);
            return Ok(incidents);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "ユーザー {UserId} が担当倉庫インシデントにアクセスしようとしましたが、権限がありません。", GetCurrentUserId());
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "担当倉庫インシデントの取得中にエラーが発生しました。");
            return StatusCode(500, "内部サーバーエラーが発生しました。");
        }
    }

    /// <summary>
    /// 分類済みインシデント一覧を取得
    /// </summary>
    [HttpGet("incidents/classified")]
    public async Task<ActionResult<PagedResultDto<WarehouseStaffIncidentDto>>> GetClassifiedIncidents(
        [FromQuery] WarehouseStaffIncidentSearchDto searchDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var incidents = await _warehouseStaffService.GetClassifiedIncidentsAsync(userId, searchDto);
            return Ok(incidents);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "ユーザー {UserId} が分類済みインシデントにアクセスしようとしましたが、権限がありません。", GetCurrentUserId());
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分類済みインシデントの取得中にエラーが発生しました。");
            return StatusCode(500, "内部サーバーエラーが発生しました。");
        }
    }

    /// <summary>
    /// 未解決インシデント一覧を取得
    /// </summary>
    [HttpGet("incidents/unresolved")]
    public async Task<ActionResult<PagedResultDto<WarehouseStaffIncidentDto>>> GetUnresolvedIncidents(
        [FromQuery] WarehouseStaffIncidentSearchDto searchDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var incidents = await _warehouseStaffService.GetUnresolvedIncidentsAsync(userId, searchDto);
            return Ok(incidents);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "ユーザー {UserId} が未解決インシデントにアクセスしようとしましたが、権限がありません。", GetCurrentUserId());
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "未解決インシデントの取得中にエラーが発生しました。");
            return StatusCode(500, "内部サーバーエラーが発生しました。");
        }
    }

    /// <summary>
    /// 対応中インシデント一覧を取得
    /// </summary>
    [HttpGet("incidents/in-progress")]
    public async Task<ActionResult<PagedResultDto<WarehouseStaffIncidentDto>>> GetInProgressIncidents(
        [FromQuery] WarehouseStaffIncidentSearchDto searchDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var incidents = await _warehouseStaffService.GetInProgressIncidentsAsync(userId, searchDto);
            return Ok(incidents);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "ユーザー {UserId} が対応中インシデントにアクセスしようとしましたが、権限がありません。", GetCurrentUserId());
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "対応中インシデントの取得中にエラーが発生しました。");
            return StatusCode(500, "内部サーバーエラーが発生しました。");
        }
    }

    /// <summary>
    /// インシデントを分類する
    /// </summary>
    [HttpPost("incidents/classify")]
    public async Task<ActionResult> ClassifyIncident([FromBody] ClassifyIncidentDto classifyDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _warehouseStaffService.ClassifyIncidentAsync(userId, classifyDto);
            
            if (result)
            {
                return Ok(new { message = "インシデントの分類が完了しました。" });
            }
            else
            {
                return BadRequest(new { message = "インシデントの分類に失敗しました。" });
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "ユーザー {UserId} がインシデント {IncidentId} の分類を試行しましたが、権限がありません。", GetCurrentUserId(), classifyDto.IncidentId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "インシデントの分類中にエラーが発生しました。");
            return StatusCode(500, "内部サーバーエラーが発生しました。");
        }
    }

    /// <summary>
    /// インシデントを更新する（倉庫担当権限内）
    /// </summary>
    [HttpPut("incidents/{incidentId}")]
    public async Task<ActionResult> UpdateIncident(int incidentId, [FromBody] WarehouseStaffUpdateIncidentDto updateDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _warehouseStaffService.UpdateIncidentAsync(userId, incidentId, updateDto);
            
            if (result)
            {
                return Ok(new { message = "インシデントの更新が完了しました。" });
            }
            else
            {
                return BadRequest(new { message = "インシデントの更新に失敗しました。" });
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "ユーザー {UserId} がインシデント {IncidentId} の更新を試行しましたが、権限がありません。", GetCurrentUserId(), incidentId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "インシデントの更新中にエラーが発生しました。");
            return StatusCode(500, "内部サーバーエラーが発生しました。");
        }
    }

    /// <summary>
    /// ユーザーが倉庫担当かどうかを確認
    /// </summary>
    [HttpGet("is-warehouse-staff")]
    public async Task<ActionResult<bool>> IsWarehouseStaff()
    {
        try
        {
            var userId = GetCurrentUserId();
            var isWarehouseStaff = await _warehouseStaffService.IsWarehouseStaffAsync(userId);
            return Ok(isWarehouseStaff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "倉庫担当権限の確認中にエラーが発生しました。");
            return StatusCode(500, "内部サーバーエラーが発生しました。");
        }
    }

    /// <summary>
    /// ユーザーの担当倉庫IDを取得
    /// </summary>
    [HttpGet("warehouse-id")]
    public async Task<ActionResult<int?>> GetWarehouseId()
    {
        try
        {
            var userId = GetCurrentUserId();
            var warehouseId = await _warehouseStaffService.GetUserWarehouseIdAsync(userId);
            return Ok(warehouseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "担当倉庫IDの取得中にエラーが発生しました。");
            return StatusCode(500, "内部サーバーエラーが発生しました。");
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("ユーザーIDが取得できません。");
        }
        return userId;
    }
}
