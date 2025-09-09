using LogisticsTroubleManagement.Domain.Entities;

namespace LogisticsTroubleManagement.Domain.Services;

/// <summary>
/// リフレッシュトークン管理サービス
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// リフレッシュトークンを生成する
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="ipAddress">クライアントIPアドレス</param>
    /// <returns>生成されたリフレッシュトークン</returns>
    Task<RefreshToken> GenerateRefreshTokenAsync(int userId, string ipAddress);

    /// <summary>
    /// リフレッシュトークンでアクセストークンを更新する
    /// </summary>
    /// <param name="token">リフレッシュトークン</param>
    /// <param name="ipAddress">クライアントIPアドレス</param>
    /// <returns>新しいアクセストークンとリフレッシュトークン</returns>
    Task<(string AccessToken, RefreshToken RefreshToken)> RefreshAccessTokenAsync(string token, string ipAddress);

    /// <summary>
    /// リフレッシュトークンを無効化する
    /// </summary>
    /// <param name="token">リフレッシュトークン</param>
    /// <param name="ipAddress">クライアントIPアドレス</param>
    /// <param name="reason">無効化理由</param>
    /// <returns>無効化に成功したかどうか</returns>
    Task<bool> RevokeTokenAsync(string token, string ipAddress, string reason = "Revoked");

    /// <summary>
    /// ユーザーの全てのリフレッシュトークンを無効化する
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="ipAddress">クライアントIPアドレス</param>
    /// <param name="reason">無効化理由</param>
    /// <returns>無効化されたトークン数</returns>
    Task<int> RevokeAllTokensForUserAsync(int userId, string ipAddress, string reason = "Revoked");

    /// <summary>
    /// 期限切れのリフレッシュトークンを削除する
    /// </summary>
    /// <returns>削除されたトークン数</returns>
    Task<int> RemoveExpiredTokensAsync();

    /// <summary>
    /// ユーザーのアクティブなリフレッシュトークン一覧を取得する
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>アクティブなリフレッシュトークン一覧</returns>
    Task<IEnumerable<RefreshToken>> GetActiveTokensForUserAsync(int userId);
}
