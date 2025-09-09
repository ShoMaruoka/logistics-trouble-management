using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Services;
using LogisticsTroubleManagement.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogisticsTroubleManagement.API.Controllers;

/// <summary>
/// パスワード管理APIコントローラー
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PasswordController : BaseController
{
    private readonly IPasswordManagementService _passwordManagementService;
    private readonly ILogger<PasswordController> _logger;

    public PasswordController(
        IPasswordManagementService passwordManagementService,
        ILogger<PasswordController> logger)
    {
        _passwordManagementService = passwordManagementService;
        _logger = logger;
    }

    /// <summary>
    /// パスワードを変更
    /// </summary>
    /// <param name="changePasswordDto">パスワード変更データ</param>
    /// <returns>変更結果</returns>
    [HttpPost("change")]
    public async Task<ActionResult<object>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { error = "認証が必要です。" });
            }

            var result = await _passwordManagementService.ChangePasswordAsync(userId.Value, changePasswordDto);
            if (!result)
            {
                return BadRequest(new { error = "パスワードの変更に失敗しました。現在のパスワードが正しくないか、新しいパスワードが要件を満たしていません。" });
            }

            return Ok(new { message = "パスワードが正常に変更されました。" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "パスワード変更中にエラーが発生しました");
            return StatusCode(500, new { error = "パスワードの変更に失敗しました。" });
        }
    }

    /// <summary>
    /// パスワードをリセット（管理者用）
    /// </summary>
    /// <param name="resetPasswordDto">パスワードリセットデータ</param>
    /// <returns>リセット結果</returns>
    [HttpPost("reset")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> ResetPassword([FromBody] AdminResetPasswordDto resetPasswordDto)
    {
        try
        {
            var resetByUserId = GetCurrentUserId();
            if (resetByUserId == null)
            {
                return Unauthorized(new { error = "認証が必要です。" });
            }

            var result = await _passwordManagementService.ResetPasswordAsync(resetPasswordDto, resetByUserId.Value);
            if (!result)
            {
                return BadRequest(new { error = "パスワードのリセットに失敗しました。ユーザーが見つからないか、新しいパスワードが要件を満たしていません。" });
            }

            return Ok(new { message = "パスワードが正常にリセットされました。" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "パスワードリセット中にエラーが発生しました");
            return StatusCode(500, new { error = "パスワードのリセットに失敗しました。" });
        }
    }

    /// <summary>
    /// パスワードの強度をチェック
    /// </summary>
    /// <param name="password">パスワード</param>
    /// <returns>強度チェック結果</returns>
    [HttpPost("check-strength")]
    public async Task<ActionResult<PasswordStrengthResultDto>> CheckPasswordStrength([FromBody] PasswordStrengthCheckDto dto)
    {
        try
        {
            var result = await _passwordManagementService.CheckPasswordStrengthAsync(dto.Password);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "パスワード強度チェック中にエラーが発生しました");
            return StatusCode(500, new { error = "パスワード強度のチェックに失敗しました。" });
        }
    }

    /// <summary>
    /// パスワードを検証
    /// </summary>
    /// <param name="password">パスワード</param>
    /// <returns>検証結果</returns>
    [HttpPost("validate")]
    public async Task<ActionResult<PasswordValidationResultDto>> ValidatePassword([FromBody] PasswordStrengthCheckDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _passwordManagementService.ValidatePasswordAsync(dto.Password, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "パスワード検証中にエラーが発生しました");
            return StatusCode(500, new { error = "パスワードの検証に失敗しました。" });
        }
    }

    /// <summary>
    /// パスワード履歴を取得
    /// </summary>
    /// <param name="page">ページ番号</param>
    /// <param name="pageSize">ページサイズ</param>
    /// <returns>パスワード履歴</returns>
    [HttpGet("history")]
    public async Task<ActionResult<PagedResultDto<PasswordHistoryDto>>> GetPasswordHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { error = "認証が必要です。" });
            }

            var result = await _passwordManagementService.GetPasswordHistoryAsync(userId.Value, page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "パスワード履歴取得中にエラーが発生しました");
            return StatusCode(500, new { error = "パスワード履歴の取得に失敗しました。" });
        }
    }

    /// <summary>
    /// パスワード変更履歴を取得
    /// </summary>
    /// <param name="page">ページ番号</param>
    /// <param name="pageSize">ページサイズ</param>
    /// <returns>パスワード変更履歴</returns>
    [HttpGet("change-history")]
    public async Task<ActionResult<PagedResultDto<PasswordChangeHistoryDto>>> GetPasswordChangeHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { error = "認証が必要です。" });
            }

            var result = await _passwordManagementService.GetPasswordChangeHistoryAsync(userId.Value, page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "パスワード変更履歴取得中にエラーが発生しました");
            return StatusCode(500, new { error = "パスワード変更履歴の取得に失敗しました。" });
        }
    }

    /// <summary>
    /// パスワードポリシーを取得
    /// </summary>
    /// <returns>パスワードポリシー</returns>
    [HttpGet("policy")]
    public async Task<ActionResult<PasswordPolicyDto>> GetPasswordPolicy()
    {
        try
        {
            var result = await _passwordManagementService.GetPasswordPolicyAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "パスワードポリシー取得中にエラーが発生しました");
            return StatusCode(500, new { error = "パスワードポリシーの取得に失敗しました。" });
        }
    }

    /// <summary>
    /// パスワードポリシーを更新（管理者用）
    /// </summary>
    /// <param name="policyDto">パスワードポリシー</param>
    /// <returns>更新結果</returns>
    [HttpPut("policy")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> UpdatePasswordPolicy([FromBody] PasswordPolicyDto policyDto)
    {
        try
        {
            var result = await _passwordManagementService.UpdatePasswordPolicyAsync(policyDto);
            if (!result)
            {
                return BadRequest(new { error = "パスワードポリシーの更新に失敗しました。" });
            }

            return Ok(new { message = "パスワードポリシーが正常に更新されました。" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "パスワードポリシー更新中にエラーが発生しました");
            return StatusCode(500, new { error = "パスワードポリシーの更新に失敗しました。" });
        }
    }

    /// <summary>
    /// パスワードが期限切れかチェック
    /// </summary>
    /// <returns>期限切れ情報</returns>
    [HttpGet("expiration")]
    public async Task<ActionResult<object>> CheckPasswordExpiration()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { error = "認証が必要です。" });
            }

            var result = await _passwordManagementService.CheckPasswordExpirationAsync(userId.Value);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "パスワード期限チェック中にエラーが発生しました");
            return StatusCode(500, new { error = "パスワード期限のチェックに失敗しました。" });
        }
    }

    /// <summary>
    /// 現在のユーザーIDを取得
    /// </summary>
    /// <returns>ユーザーID</returns>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}
