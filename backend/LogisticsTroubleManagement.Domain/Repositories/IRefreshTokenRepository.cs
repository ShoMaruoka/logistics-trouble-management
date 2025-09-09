using LogisticsTroubleManagement.Domain.Entities;

namespace LogisticsTroubleManagement.Domain.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId);
    Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(int userId);
    Task<IEnumerable<RefreshToken>> GetActiveTokensForUserAsync(int userId);
    Task<IEnumerable<RefreshToken>> GetExpiredTokensAsync();
    Task RevokeAllUserTokensAsync(int userId, string? revokedByIp = null, string? reasonRevoked = null);
}
