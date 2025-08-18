using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email.Value == email);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
    {
        return await _dbSet.Where(u => u.Role == role).ToListAsync();
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _dbSet.Where(u => u.IsActive).ToListAsync();
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email.Value == email);
    }

    public async Task<IEnumerable<User>> GetUsersByDepartmentAsync(string department)
    {
        // 注: 現在のUserエンティティにはDepartmentプロパティがないため、
        // 将来的にDepartmentプロパティが追加された場合の実装例
        // return await _dbSet.Where(u => u.Department == department).ToListAsync();
        
        // 現在は全ユーザーを返す（実装例として）
        return await GetAllAsync();
    }
}
