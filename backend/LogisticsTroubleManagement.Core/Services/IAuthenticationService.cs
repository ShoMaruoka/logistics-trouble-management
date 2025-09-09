using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Services;

/// <summary>
/// 認証サービスインターフェース
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// ユーザー認証
    /// </summary>
    /// <param name="loginDto">ログイン情報</param>
    /// <returns>ログイン応答</returns>
    Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto);

    /// <summary>
    /// トークンリフレッシュ
    /// </summary>
    /// <param name="refreshTokenDto">リフレッシュトークン情報</param>
    /// <returns>新しいトークン情報</returns>
    Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);

    /// <summary>
    /// ログアウト
    /// </summary>
    /// <param name="logoutDto">ログアウト情報</param>
    /// <returns>処理結果</returns>
    Task<bool> LogoutAsync(LogoutDto logoutDto);

    /// <summary>
    /// パスワード変更
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="changePasswordDto">パスワード変更情報</param>
    /// <returns>処理結果</returns>
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);

    /// <summary>
    /// パスワードリセット要求
    /// </summary>
    /// <param name="forgotPasswordDto">パスワードリセット要求情報</param>
    /// <returns>処理結果</returns>
    Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);

    /// <summary>
    /// パスワードリセット
    /// </summary>
    /// <param name="resetPasswordDto">パスワードリセット情報</param>
    /// <returns>処理結果</returns>
    Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

    /// <summary>
    /// ユーザー情報取得
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>ユーザー情報</returns>
    Task<UserDto?> GetUserAsync(int userId);

    /// <summary>
    /// トークンの有効性確認
    /// </summary>
    /// <param name="token">トークン</param>
    /// <returns>有効性</returns>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// リフレッシュトークンを無効化
    /// </summary>
    /// <param name="token">無効化するトークン</param>
    /// <param name="ipAddress">クライアントIPアドレス</param>
    /// <param name="reason">無効化理由</param>
    /// <returns>無効化に成功したかどうか</returns>
    Task<bool> RevokeTokenAsync(string token, string ipAddress, string reason = "Revoked");
}
