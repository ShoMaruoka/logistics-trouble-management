using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Domain.Services;
using LogisticsTroubleManagement.Core.Services;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace LogisticsTroubleManagement.Infrastructure.Services;

/// <summary>
/// リフレッシュトークン管理サービスの実装
/// </summary>
public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtTokenService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RefreshTokenService> _logger;

    public RefreshTokenService(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtService jwtTokenService,
        ApplicationDbContext context,
        ILogger<RefreshTokenService> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _context = context;
        _logger = logger;
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(int userId, string ipAddress)
    {
        _logger.LogInformation("ユーザーID {UserId} のリフレッシュトークンを生成します。IP: {IpAddress}", userId, ipAddress);

        // ランダムなトークンを生成
        var token = GenerateRandomToken();

        // 7日間有効なリフレッシュトークンを作成
        var refreshToken = RefreshToken.Create(userId, token, DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        _logger.LogInformation("ユーザーID {UserId} のリフレッシュトークンを生成しました。トークンID: {TokenId}", userId, refreshToken.Id);

        return refreshToken;
    }

    public async Task<(string AccessToken, RefreshToken RefreshToken)> RefreshAccessTokenAsync(string token, string ipAddress)
    {
        _logger.LogInformation("リフレッシュトークンでアクセストークンを更新します。IP: {IpAddress}", ipAddress);

        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
        if (refreshToken == null)
        {
            _logger.LogWarning("無効なリフレッシュトークンが使用されました。IP: {IpAddress}", ipAddress);
            throw new UnauthorizedAccessException("無効なリフレッシュトークンです。");
        }

        if (refreshToken.IsRevoked)
        {
            _logger.LogWarning("取り消されたリフレッシュトークンが使用されました。トークンID: {TokenId}, IP: {IpAddress}", refreshToken.Id, ipAddress);
            throw new UnauthorizedAccessException("取り消されたリフレッシュトークンです。");
        }

        if (refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("期限切れのリフレッシュトークンが使用されました。トークンID: {TokenId}, IP: {IpAddress}", refreshToken.Id, ipAddress);
            throw new UnauthorizedAccessException("期限切れのリフレッシュトークンです。");
        }

        // ユーザー情報を取得
        var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("無効なユーザーまたは非アクティブユーザーのリフレッシュトークンが使用されました。ユーザーID: {UserId}, IP: {IpAddress}", refreshToken.UserId, ipAddress);
            throw new UnauthorizedAccessException("無効なユーザーです。");
        }

        // ユーザーのロール情報を取得
        var role = user.Role;
        var roleName = role?.Name ?? "User";

        // 新しいリフレッシュトークンを生成
        var newRefreshToken = await GenerateRefreshTokenAsync(user.Id, ipAddress);

        // 古いリフレッシュトークンを無効化（新しいトークンで置き換え）
        refreshToken.Revoke(ipAddress, "Replaced by new token", newRefreshToken.Token);

        await _context.SaveChangesAsync();

        // 新しいアクセストークンを生成
        var claims = _jwtTokenService.GenerateClaims(user.Id, user.Username, roleName);
        var accessToken = _jwtTokenService.GenerateAccessToken(claims);

        _logger.LogInformation("ユーザーID {UserId} のアクセストークンを更新しました。IP: {IpAddress}", user.Id, ipAddress);

        return (accessToken, newRefreshToken);
    }

    public async Task<bool> RevokeTokenAsync(string token, string ipAddress, string reason = "Revoked")
    {
        _logger.LogInformation("リフレッシュトークンを無効化します。IP: {IpAddress}, 理由: {Reason}", ipAddress, reason);

        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
        if (refreshToken == null)
        {
            _logger.LogWarning("無効化対象のリフレッシュトークンが見つかりません。IP: {IpAddress}", ipAddress);
            return false;
        }

        if (refreshToken.IsRevoked)
        {
            _logger.LogInformation("既に無効化されているリフレッシュトークンです。トークンID: {TokenId}", refreshToken.Id);
            return true;
        }

        refreshToken.Revoke(ipAddress, reason);

        await _context.SaveChangesAsync();

        _logger.LogInformation("リフレッシュトークンを無効化しました。トークンID: {TokenId}, 理由: {Reason}", refreshToken.Id, reason);

        return true;
    }

    public async Task<int> RevokeAllTokensForUserAsync(int userId, string ipAddress, string reason = "Revoked")
    {
        _logger.LogInformation("ユーザーID {UserId} の全てのリフレッシュトークンを無効化します。IP: {IpAddress}, 理由: {Reason}", userId, ipAddress, reason);

        var activeTokens = await _refreshTokenRepository.GetActiveTokensForUserAsync(userId);
        var revokedCount = 0;

        foreach (var token in activeTokens)
        {
            token.Revoke(ipAddress, reason);
            revokedCount++;
        }

        if (revokedCount > 0)
        {
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("ユーザーID {UserId} の {Count} 個のリフレッシュトークンを無効化しました。", userId, revokedCount);

        return revokedCount;
    }

    public async Task<int> RemoveExpiredTokensAsync()
    {
        _logger.LogInformation("期限切れのリフレッシュトークンを削除します。");

        var expiredTokens = await _refreshTokenRepository.GetExpiredTokensAsync();
        var removedCount = expiredTokens.Count();

        if (removedCount > 0)
        {
            foreach (var token in expiredTokens)
            {
                _context.RefreshTokens.Remove(token);
            }
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("{Count} 個の期限切れリフレッシュトークンを削除しました。", removedCount);

        return removedCount;
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensForUserAsync(int userId)
    {
        _logger.LogDebug("ユーザーID {UserId} のアクティブなリフレッシュトークン一覧を取得します。", userId);

        return await _refreshTokenRepository.GetActiveTokensForUserAsync(userId);
    }

    /// <summary>
    /// ランダムなトークン文字列を生成する
    /// </summary>
    /// <returns>ランダムなトークン文字列</returns>
    private static string GenerateRandomToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[32];
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
