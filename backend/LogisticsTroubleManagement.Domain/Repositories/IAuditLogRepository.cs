using LogisticsTroubleManagement.Domain.Entities;

namespace LogisticsTroubleManagement.Domain.Repositories;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    // 監査ログ固有のクエリメソッド
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId);
    Task<IEnumerable<AuditLog>> GetByActionAsync(string action);
    Task<IEnumerable<AuditLog>> GetByTableNameAsync(string tableName);
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<AuditLog>> GetByIpAddressAsync(string ipAddress);
    Task<IEnumerable<AuditLog>> GetLoginLogsAsync();
    Task<IEnumerable<AuditLog>> GetLogoutLogsAsync();
}
