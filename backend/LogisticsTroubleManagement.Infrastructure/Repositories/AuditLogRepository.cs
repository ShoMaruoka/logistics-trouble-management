using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Infrastructure.Repositories;

public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId)
    {
        return await _dbSet.Where(al => al.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByActionAsync(string action)
    {
        return await _dbSet.Where(al => al.Action == action).ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByTableNameAsync(string tableName)
    {
        return await _dbSet.Where(al => al.TableName == tableName).ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet.Where(al => al.CreatedAt >= startDate && al.CreatedAt <= endDate).ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByIpAddressAsync(string ipAddress)
    {
        return await _dbSet.Where(al => al.IpAddress == ipAddress).ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetLoginLogsAsync()
    {
        return await _dbSet.Where(al => al.Action == "LOGIN").ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetLogoutLogsAsync()
    {
        return await _dbSet.Where(al => al.Action == "LOGOUT").ToListAsync();
    }
}
