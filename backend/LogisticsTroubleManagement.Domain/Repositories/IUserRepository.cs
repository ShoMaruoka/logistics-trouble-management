using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    // ユーザー固有のクエリメソッド
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<IEnumerable<User>> GetUsersByDepartmentAsync(string department);
}
