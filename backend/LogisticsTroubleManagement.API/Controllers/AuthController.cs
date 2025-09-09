using Microsoft.AspNetCore.Mvc;
using LogisticsTroubleManagement.Core.Services;
using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Exceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace LogisticsTroubleManagement.API.Controllers;

/// <summary>
/// 認証コントローラー
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// ログイン
    /// </summary>
    /// <param name="loginDto">ログイン情報</param>
    /// <returns>ログイン応答</returns>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var response = await _authService.AuthenticateAsync(loginDto);
            
            // リフレッシュトークンをセキュアクッキーで設定
            var refreshTokenCookie = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // HTTPS必須
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(7),
                Path = "/"
            };
            
            Response.Cookies.Append("refresh_token", response.RefreshToken, refreshTokenCookie);
            
            // アクセストークンとリフレッシュトークンをレスポンスボディで返す
            return Ok(new { 
                accessToken = response.AccessToken,
                refreshToken = response.RefreshToken,
                expiresAt = response.ExpiresAt,
                user = response.User
            });
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UserInactiveException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidPasswordException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ログイン処理中にエラーが発生しました: {Message}", ex.Message);
            return StatusCode(500, new { error = "ログイン処理中にエラーが発生しました", details = ex.Message });
        }
    }

    /// <summary>
    /// トークンリフレッシュ
    /// </summary>
    /// <returns>新しいトークン情報</returns>
    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponseDto>> Refresh()
    {
        try
        {
            // リフレッシュトークンをクッキーから取得
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { error = "リフレッシュトークンがありません" });
            }

            var refreshTokenDto = new RefreshTokenDto { RefreshToken = refreshToken };
            var response = await _authService.RefreshTokenAsync(refreshTokenDto);
            
            // 新しいリフレッシュトークンをセキュアクッキーで設定
            var newRefreshTokenCookie = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(7),
                Path = "/"
            };
            
            Response.Cookies.Append("refresh_token", response.RefreshToken, newRefreshTokenCookie);
            
            return Ok(new { 
                accessToken = response.AccessToken,
                expiresAt = response.ExpiresAt
            });
        }
        catch (InvalidTokenException ex)
        {
            // 無効なトークンの場合、クッキーを削除
            Response.Cookies.Delete("refresh_token");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "トークンリフレッシュ処理中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// ログアウト
    /// </summary>
    /// <returns>処理結果</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        try
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var logoutDto = new LogoutDto { RefreshToken = refreshToken };
                await _authService.LogoutAsync(logoutDto);
            }
            
            // クッキーの削除
            Response.Cookies.Delete("refresh_token");
            
            return Ok(new { message = "ログアウトしました" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "ログアウト処理中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// パスワード変更
    /// </summary>
    /// <param name="changePasswordDto">パスワード変更情報</param>
    /// <returns>処理結果</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
            
            if (result)
            {
                return Ok(new { message = "パスワードを変更しました" });
            }
            
            return BadRequest(new { error = "パスワードの変更に失敗しました" });
        }
        catch (InvalidPasswordException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "パスワード変更処理中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// パスワードリセット要求
    /// </summary>
    /// <param name="forgotPasswordDto">パスワードリセット要求情報</param>
    /// <returns>処理結果</returns>
    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
            
            if (result)
            {
                return Ok(new { message = "パスワードリセットの手順をメールで送信しました" });
            }
            
            return BadRequest(new { error = "パスワードリセット要求の処理に失敗しました" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "パスワードリセット要求処理中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// パスワードリセット
    /// </summary>
    /// <param name="resetPasswordDto">パスワードリセット情報</param>
    /// <returns>処理結果</returns>
    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        try
        {
            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            
            if (result)
            {
                return Ok(new { message = "パスワードをリセットしました" });
            }
            
            return BadRequest(new { error = "パスワードのリセットに失敗しました" });
        }
        catch (InvalidTokenException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidPasswordException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "パスワードリセット処理中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// プロフィール取得
    /// </summary>
    /// <returns>ユーザー情報</returns>
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _authService.GetUserAsync(userId);
            
            if (user == null)
            {
                return NotFound(new { error = "ユーザーが見つかりません" });
            }
            
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "プロフィール取得処理中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// リフレッシュトークンでアクセストークンを更新（リクエストボディ版）
    /// </summary>
    /// <param name="refreshTokenDto">リフレッシュトークン情報</param>
    /// <returns>新しいトークン情報</returns>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenDto);
            return Ok(result);
        }
        catch (InvalidPasswordException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "トークン更新中にエラーが発生しました" });
        }
    }

    /// <summary>
    /// リフレッシュトークンを無効化
    /// </summary>
    /// <param name="revokeTokenDto">無効化するトークン情報</param>
    /// <returns>処理結果</returns>
    [HttpPost("revoke")]
    public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenDto revokeTokenDto)
    {
        try
        {
            var result = await _authService.RevokeTokenAsync(revokeTokenDto.Token, "127.0.0.1", revokeTokenDto.Reason);
            
            if (result)
            {
                return Ok(new { message = "トークンを無効化しました" });
            }
            
            return BadRequest(new { error = "トークンの無効化に失敗しました" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "トークン無効化中にエラーが発生しました" });
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
