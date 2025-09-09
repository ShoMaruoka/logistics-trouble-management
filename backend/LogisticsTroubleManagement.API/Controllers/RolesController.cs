using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Services;
using LogisticsTroubleManagement.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsTroubleManagement.API.Controllers;

/// <summary>
/// ロール管理APIコントローラー
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RolesController : BaseController
{
    private readonly IRoleManagementService _roleManagementService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(
        IRoleManagementService roleManagementService,
        ILogger<RolesController> logger)
    {
        _roleManagementService = roleManagementService;
        _logger = logger;
    }

    /// <summary>
    /// ロール一覧を取得
    /// </summary>
    /// <param name="searchDto">検索条件</param>
    /// <returns>ページングされたロール一覧</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<RoleListDto>>> GetRoles([FromQuery] RoleSearchDto searchDto)
    {
        try
        {
            var result = await _roleManagementService.GetRolesAsync(searchDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ロール一覧取得中にエラーが発生しました");
            return StatusCode(500, new { error = "ロール一覧の取得に失敗しました。" });
        }
    }

    /// <summary>
    /// ロール詳細を取得
    /// </summary>
    /// <param name="id">ロールID</param>
    /// <returns>ロール詳細</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDetailDto>> GetRole(int id)
    {
        try
        {
            var role = await _roleManagementService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound(new { error = "指定されたロールが見つかりません。" });
            }

            return Ok(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ロール詳細取得中にエラーが発生しました。ID: {RoleId}", id);
            return StatusCode(500, new { error = "ロール詳細の取得に失敗しました。" });
        }
    }

    /// <summary>
    /// ロールを作成
    /// </summary>
    /// <param name="createDto">作成データ</param>
    /// <returns>作成されたロール</returns>
    [HttpPost]
    public async Task<ActionResult<RoleDetailDto>> CreateRole([FromBody] CreateRoleDto createDto)
    {
        try
        {
            var role = await _roleManagementService.CreateRoleAsync(createDto);
            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ロール作成中にエラーが発生しました");
            return StatusCode(500, new { error = "ロールの作成に失敗しました。" });
        }
    }

    /// <summary>
    /// ロールを更新
    /// </summary>
    /// <param name="id">ロールID</param>
    /// <param name="updateDto">更新データ</param>
    /// <returns>更新されたロール</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<RoleDetailDto>> UpdateRole(int id, [FromBody] UpdateRoleDto updateDto)
    {
        try
        {
            var role = await _roleManagementService.UpdateRoleAsync(id, updateDto);
            return Ok(role);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ロール更新中にエラーが発生しました。ID: {RoleId}", id);
            return StatusCode(500, new { error = "ロールの更新に失敗しました。" });
        }
    }

    /// <summary>
    /// ロールを削除
    /// </summary>
    /// <param name="id">ロールID</param>
    /// <returns>削除結果</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRole(int id)
    {
        try
        {
            var result = await _roleManagementService.DeleteRoleAsync(id);
            if (!result)
            {
                return NotFound(new { error = "指定されたロールが見つかりません。" });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ロール削除中にエラーが発生しました。ID: {RoleId}", id);
            return StatusCode(500, new { error = "ロールの削除に失敗しました。" });
        }
    }

    /// <summary>
    /// ロールのステータスを切り替え
    /// </summary>
    /// <param name="id">ロールID</param>
    /// <param name="toggleDto">ステータス切り替えデータ</param>
    /// <returns>更新されたロール</returns>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<RoleDetailDto>> ToggleRoleStatus(int id, [FromBody] ToggleRoleStatusDto toggleDto)
    {
        try
        {
            var role = await _roleManagementService.ToggleRoleStatusAsync(id, toggleDto);
            return Ok(role);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ロールステータス切り替え中にエラーが発生しました。ID: {RoleId}", id);
            return StatusCode(500, new { error = "ロールステータスの切り替えに失敗しました。" });
        }
    }

    /// <summary>
    /// ロール名の重複チェック
    /// </summary>
    /// <param name="name">ロール名</param>
    /// <param name="excludeRoleId">除外するロールID</param>
    /// <returns>重複チェック結果</returns>
    [HttpGet("check-name")]
    public async Task<ActionResult<object>> CheckRoleName([FromQuery] string name, [FromQuery] int? excludeRoleId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { error = "ロール名を入力してください。" });
            }

            var exists = await _roleManagementService.IsRoleNameExistsAsync(name, excludeRoleId);
            return Ok(new { exists });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ロール名重複チェック中にエラーが発生しました。Name: {RoleName}", name);
            return StatusCode(500, new { error = "ロール名の重複チェックに失敗しました。" });
        }
    }

    /// <summary>
    /// アクティブロール一覧を取得
    /// </summary>
    /// <returns>アクティブロール一覧</returns>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<RoleListDto>>> GetActiveRoles()
    {
        try
        {
            var roles = await _roleManagementService.GetActiveRolesAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "アクティブロール一覧取得中にエラーが発生しました");
            return StatusCode(500, new { error = "アクティブロール一覧の取得に失敗しました。" });
        }
    }

    /// <summary>
    /// ロールが使用中かチェック
    /// </summary>
    /// <param name="id">ロールID</param>
    /// <returns>使用中チェック結果</returns>
    [HttpGet("{id}/in-use")]
    public async Task<ActionResult<object>> CheckRoleInUse(int id)
    {
        try
        {
            var inUse = await _roleManagementService.IsRoleInUseAsync(id);
            return Ok(new { inUse });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ロール使用中チェック中にエラーが発生しました。ID: {RoleId}", id);
            return StatusCode(500, new { error = "ロール使用中チェックに失敗しました。" });
        }
    }
}
