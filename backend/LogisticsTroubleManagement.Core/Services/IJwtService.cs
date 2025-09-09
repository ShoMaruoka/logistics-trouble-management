using System.Security.Claims;

namespace LogisticsTroubleManagement.Core.Services;

/// <summary>
/// JWTサービスインターフェース
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// アクセストークンの生成
    /// </summary>
    /// <param name="claims">クレーム情報</param>
    /// <returns>アクセストークン</returns>
    string GenerateAccessToken(IEnumerable<Claim> claims);

    /// <summary>
    /// リフレッシュトークンの生成
    /// </summary>
    /// <returns>リフレッシュトークン</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// トークンからクレーム情報を取得
    /// </summary>
    /// <param name="token">トークン</param>
    /// <returns>クレーム情報</returns>
    ClaimsPrincipal? GetPrincipalFromToken(string token);

    /// <summary>
    /// トークンの有効期限を取得
    /// </summary>
    /// <param name="token">トークン</param>
    /// <returns>有効期限</returns>
    DateTime? GetExpirationFromToken(string token);

    /// <summary>
    /// トークンの有効性を検証
    /// </summary>
    /// <param name="token">トークン</param>
    /// <returns>有効性</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// ユーザーIDからクレーム情報を生成
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="username">ユーザー名</param>
    /// <param name="role">ロール</param>
    /// <returns>クレーム情報</returns>
    IEnumerable<Claim> GenerateClaims(int userId, string username, string role);
}
