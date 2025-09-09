using LogisticsTroubleManagement.Core.Services;
using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Exceptions;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Services;
using Microsoft.Extensions.Options;
using LogisticsTroubleManagement.Core.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace LogisticsTroubleManagement.Infrastructure.Services;

/// <summary>
/// 簡易版認証サービス（基本的な認証機能のみ）
/// </summary>
public class SimpleAuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly AuthenticationSettings _authSettings;

    public SimpleAuthenticationService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IPasswordService passwordService,
        IRefreshTokenService refreshTokenService,
        IOptions<AuthenticationSettings> authSettings)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _refreshTokenService = refreshTokenService;
        _authSettings = authSettings.Value;
    }

    /// <summary>
    /// ユーザー認証
    /// </summary>
    /// <param name="loginDto">ログイン情報</param>
    /// <returns>ログイン応答</returns>
    public async Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto)
    {
        try
        {
            // ユーザー名またはメールアドレスでユーザーを検索
            Console.WriteLine($"Searching for user: {loginDto.UsernameOrEmail}");
            var user = await _userRepository.GetByUsernameOrEmailAsync(loginDto.UsernameOrEmail);
            if (user == null)
            {
                Console.WriteLine($"User not found: {loginDto.UsernameOrEmail}");
                throw new InvalidPasswordException("ユーザー名またはパスワードが無効です");
            }
            
            Console.WriteLine($"User found: {user.Username}, Email: {user.Email.Value}, PasswordHash: {user.PasswordHash?.Substring(0, Math.Min(20, user.PasswordHash?.Length ?? 0))}...");

            // パスワードの検証
            Console.WriteLine($"Verifying password for user: {user.Username}");
            var passwordValid = _passwordService.VerifyPassword(loginDto.Password, user.PasswordHash);
            Console.WriteLine($"Password verification result: {passwordValid}");
            if (!passwordValid)
            {
                Console.WriteLine($"Password verification failed for user: {user.Username}");
                throw new InvalidPasswordException("ユーザー名またはパスワードが無効です");
            }

            // ユーザーがアクティブかチェック
            if (!user.IsActive)
            {
                throw new InvalidPasswordException("アカウントが無効です");
            }

            // ユーザーのロール情報を取得
            var role = user.Role;
            var roleName = role?.Name ?? "User";
            var roleId = role?.Id ?? 1;

            // JWTクレームを生成
            var claims = _jwtService.GenerateClaims(user.Id, user.Username, roleName);
            var accessToken = _jwtService.GenerateAccessToken(claims);

            // リフレッシュトークンを生成
            var refreshTokenEntity = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, "127.0.0.1");

            // 最終ログイン時刻を更新
            user.UpdateLastLoginAt();
            await _userRepository.UpdateAsync(user);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenEntity.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_authSettings.Jwt.AccessTokenExpirationMinutes),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email.Value,
                    RoleName = roleName,
                    RoleId = roleId,
                    LastLoginAt = user.LastLoginAt,
                    IsActive = user.IsActive
                }
            };
        }
        catch (Exception ex)
        {
            // ログ出力（実際の実装では適切なログライブラリを使用）
            Console.WriteLine($"Authentication error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// トークンリフレッシュ
    /// </summary>
    /// <param name="refreshTokenDto">リフレッシュトークン情報</param>
    /// <returns>新しいトークン情報</returns>
    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        try
        {
            // リフレッシュトークンサービスを使用してトークンを更新
            var (newAccessToken, newRefreshToken) = await _refreshTokenService.RefreshAccessTokenAsync(
                refreshTokenDto.RefreshToken, 
                "127.0.0.1");

            return new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_authSettings.Jwt.AccessTokenExpirationMinutes)
            };
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token refresh error: {ex.Message}");
            throw new InvalidPasswordException("トークンの更新に失敗しました");
        }
    }

    /// <summary>
    /// ログアウト
    /// </summary>
    /// <param name="logoutDto">ログアウト情報</param>
    /// <returns>処理結果</returns>
    public async Task<bool> LogoutAsync(LogoutDto logoutDto)
    {
        try
        {
            if (!string.IsNullOrEmpty(logoutDto.RefreshToken))
            {
                // リフレッシュトークンを無効化
                await _refreshTokenService.RevokeTokenAsync(logoutDto.RefreshToken, "127.0.0.1", "User logout");
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logout error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// パスワード変更（簡易版）
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="changePasswordDto">パスワード変更情報</param>
    /// <returns>処理結果</returns>
    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        // 簡易版：常に成功を返す
        return true;
    }

    /// <summary>
    /// パスワードリセット要求（簡易版）
    /// </summary>
    /// <param name="forgotPasswordDto">パスワードリセット要求情報</param>
    /// <returns>処理結果</returns>
    public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        // 簡易版：常に成功を返す
        return true;
    }

    /// <summary>
    /// パスワードリセット（簡易版）
    /// </summary>
    /// <param name="resetPasswordDto">パスワードリセット情報</param>
    /// <returns>処理結果</returns>
    public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        // 簡易版：常に成功を返す
        return true;
    }

    /// <summary>
    /// ユーザー情報取得
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>ユーザー情報</returns>
    public async Task<UserDto?> GetUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        // ユーザーのロール情報を取得
        var role = user.Role;
        var roleName = role?.Name ?? "User";
        var roleId = role?.Id ?? 1;

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email.Value,
            RoleName = roleName,
            RoleId = roleId,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive
        };
    }

    /// <summary>
    /// トークンの有効性確認（簡易版）
    /// </summary>
    /// <param name="token">トークン</param>
    /// <returns>有効性</returns>
    public async Task<bool> ValidateTokenAsync(string token)
    {
        return _jwtService.ValidateToken(token);
    }

    /// <summary>
    /// リフレッシュトークンを無効化
    /// </summary>
    /// <param name="token">無効化するトークン</param>
    /// <param name="ipAddress">クライアントIPアドレス</param>
    /// <param name="reason">無効化理由</param>
    /// <returns>無効化に成功したかどうか</returns>
    public async Task<bool> RevokeTokenAsync(string token, string ipAddress, string reason = "Revoked")
    {
        try
        {
            return await _refreshTokenService.RevokeTokenAsync(token, ipAddress, reason);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token revocation error: {ex.Message}");
            return false;
        }
    }
}
