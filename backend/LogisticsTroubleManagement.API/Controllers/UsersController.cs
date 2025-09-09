using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogisticsTroubleManagement.API.Controllers;

/// <summary>
/// ユーザー管理コントローラー
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserManagementService userManagementService,
        ILogger<UsersController> logger)
    {
        _userManagementService = userManagementService;
        _logger = logger;
    }

    /// <summary>
    /// ユーザー一覧を取得
    /// </summary>
    /// <param name="searchDto">検索条件</param>
    /// <returns>ユーザー一覧</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<UserListDto>>> GetUsers([FromQuery] UserSearchDto searchDto)
    {
        try
        {
            var result = await _userManagementService.GetUsersAsync(searchDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザー一覧の取得中にエラーが発生しました");
            return StatusCode(500, new { error = "ユーザー一覧の取得中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// ユーザー詳細を取得
    /// </summary>
    /// <param name="id">ユーザーID</param>
    /// <returns>ユーザー詳細</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDetailDto>> GetUser(int id)
    {
        try
        {
            var user = await _userManagementService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { error = "ユーザーが見つかりません" });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザー詳細の取得中にエラーが発生しました。ユーザーID: {UserId}", id);
            return StatusCode(500, new { error = "ユーザー詳細の取得中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// ユーザーを作成
    /// </summary>
    /// <param name="createDto">ユーザー作成情報</param>
    /// <returns>作成されたユーザー</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDetailDto>> CreateUser([FromBody] CreateUserDto createDto)
    {
        try
        {
            var user = await _userManagementService.CreateUserAsync(createDto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーの作成中にエラーが発生しました");
            return StatusCode(500, new { error = "ユーザーの作成中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// ユーザーを更新
    /// </summary>
    /// <param name="id">ユーザーID</param>
    /// <param name="updateDto">ユーザー更新情報</param>
    /// <returns>更新されたユーザー</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDetailDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateDto)
    {
        try
        {
            var user = await _userManagementService.UpdateUserAsync(id, updateDto);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーの更新中にエラーが発生しました。ユーザーID: {UserId}", id);
            return StatusCode(500, new { error = "ユーザーの更新中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// ユーザーを削除
    /// </summary>
    /// <param name="id">ユーザーID</param>
    /// <returns>削除結果</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == id)
            {
                return BadRequest(new { error = "自分自身を削除することはできません" });
            }

            var result = await _userManagementService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound(new { error = "ユーザーが見つかりません" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーの削除中にエラーが発生しました。ユーザーID: {UserId}", id);
            return StatusCode(500, new { error = "ユーザーの削除中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// ユーザーの有効/無効を切り替え
    /// </summary>
    /// <param name="id">ユーザーID</param>
    /// <param name="toggleDto">有効/無効切り替え情報</param>
    /// <returns>更新されたユーザー</returns>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDetailDto>> ToggleUserStatus(int id, [FromBody] ToggleUserStatusDto toggleDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == id)
            {
                return BadRequest(new { error = "自分自身のステータスを変更することはできません" });
            }

            var user = await _userManagementService.ToggleUserStatusAsync(id, toggleDto);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーのステータス変更中にエラーが発生しました。ユーザーID: {UserId}", id);
            return StatusCode(500, new { error = "ユーザーのステータス変更中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// ユーザー名の重複チェック
    /// </summary>
    /// <param name="username">ユーザー名</param>
    /// <param name="excludeUserId">除外するユーザーID</param>
    /// <returns>重複チェック結果</returns>
    [HttpGet("check-username")]
    public async Task<ActionResult<object>> CheckUsername([FromQuery] string username, [FromQuery] int? excludeUserId = null)
    {
        try
        {
            var exists = await _userManagementService.IsUsernameExistsAsync(username, excludeUserId);
            return Ok(new { exists });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザー名の重複チェック中にエラーが発生しました。ユーザー名: {Username}", username);
            return StatusCode(500, new { error = "ユーザー名の重複チェック中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// メールアドレスの重複チェック
    /// </summary>
    /// <param name="email">メールアドレス</param>
    /// <param name="excludeUserId">除外するユーザーID</param>
    /// <returns>重複チェック結果</returns>
    [HttpGet("check-email")]
    public async Task<ActionResult<object>> CheckEmail([FromQuery] string email, [FromQuery] int? excludeUserId = null)
    {
        try
        {
            var exists = await _userManagementService.IsEmailExistsAsync(email, excludeUserId);
            return Ok(new { exists });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "メールアドレスの重複チェック中にエラーが発生しました。メールアドレス: {Email}", email);
            return StatusCode(500, new { error = "メールアドレスの重複チェック中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// ユーザーのパスワードをリセット
    /// </summary>
    /// <param name="id">ユーザーID</param>
    /// <param name="request">パスワードリセット要求</param>
    /// <returns>リセット結果</returns>
    [HttpPost("{id}/reset-password")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ResetUserPassword(int id, [FromBody] ResetUserPasswordRequest request)
    {
        try
        {
            var result = await _userManagementService.ResetUserPasswordAsync(id, request.NewPassword);
            if (!result)
            {
                return NotFound(new { error = "ユーザーが見つかりません" });
            }

            return Ok(new { message = "パスワードをリセットしました" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーのパスワードリセット中にエラーが発生しました。ユーザーID: {UserId}", id);
            return StatusCode(500, new { error = "ユーザーのパスワードリセット中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// ユーザーのログイン履歴を取得
    /// </summary>
    /// <param name="id">ユーザーID</param>
    /// <param name="page">ページ番号</param>
    /// <param name="pageSize">ページサイズ</param>
    /// <returns>ログイン履歴</returns>
    [HttpGet("{id}/login-history")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PagedResultDto<object>>> GetUserLoginHistory(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _userManagementService.GetUserLoginHistoryAsync(id, page, pageSize);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーのログイン履歴取得中にエラーが発生しました。ユーザーID: {UserId}", id);
            return StatusCode(500, new { error = "ユーザーのログイン履歴取得中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// 現在のユーザーIDを取得
    /// </summary>
    /// <returns>ユーザーID</returns>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            throw new UnauthorizedAccessException("ユーザーIDが取得できません");
        }
        return userId;
    }
}

