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
        return await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email.Value == email);
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
    {
        // まずユーザー名で検索
        var user = await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == usernameOrEmail);
        
        if (user != null)
        {
            return user;
        }
        
        // ユーザー名で見つからない場合は、メールアドレスで検索
        var allUsers = await _dbSet
            .Include(u => u.Role)
            .ToListAsync();
            
        return allUsers.FirstOrDefault(u => u.Email.Value == usernameOrEmail);
    }

    public async Task<IEnumerable<User>> GetByRoleIdAsync(int roleId)
    {
        return await _dbSet
            .Include(u => u.Role)
            .Where(u => u.RoleId == roleId)
            .ToListAsync();
    }

    public override async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<User>> GetByRoleNameAsync(string roleName)
    {
        return await _dbSet
            .Include(u => u.Role)
            .Where(u => u.Role.Name == roleName)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _dbSet
            .Include(u => u.Role)
            .Where(u => u.IsActive)
            .ToListAsync();
    }

    public override async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbSet
            .Include(u => u.Role)
            .ToListAsync();
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
