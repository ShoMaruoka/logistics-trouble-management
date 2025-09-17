using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Services;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogisticsTroubleManagement.Infrastructure.Services;

/// <summary>
/// ユーザー管理サービスの実装
/// </summary>
public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordService _passwordService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordService passwordService,
        ApplicationDbContext context,
        ILogger<UserManagementService> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordService = passwordService;
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResultDto<UserListDto>> GetUsersAsync(UserSearchDto searchDto)
    {
        _logger.LogInformation("ユーザー一覧を取得します。検索条件: {SearchDto}", searchDto);

        try
        {
            var users = await _userRepository.GetAllAsync();
            var roles = await _roleRepository.GetAllAsync();
            var roleDict = roles.ToDictionary(r => r.Id, r => r.Name);

            // 検索条件でフィルタリング
            var filteredUsers = users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
            {
                var keyword = searchDto.Keyword.ToLower();
                filteredUsers = filteredUsers.Where(u => 
                    u.Username.ToLower().Contains(keyword) ||
                    u.Email.Value.ToLower().Contains(keyword) ||
                    u.FirstName.ToLower().Contains(keyword) ||
                    u.LastName.ToLower().Contains(keyword));
            }

            if (searchDto.RoleId.HasValue)
            {
                filteredUsers = filteredUsers.Where(u => u.RoleId == searchDto.RoleId.Value);
            }

            if (searchDto.IsActive.HasValue)
            {
                filteredUsers = filteredUsers.Where(u => u.IsActive == searchDto.IsActive.Value);
            }

            // ソート
            filteredUsers = searchDto.SortBy.ToLower() switch
            {
                "username" => searchDto.SortOrder.ToLower() == "desc" 
                    ? filteredUsers.OrderByDescending(u => u.Username)
                    : filteredUsers.OrderBy(u => u.Username),
                "email" => searchDto.SortOrder.ToLower() == "desc"
                    ? filteredUsers.OrderByDescending(u => u.Email.Value)
                    : filteredUsers.OrderBy(u => u.Email.Value),
                "lastlogin" => searchDto.SortOrder.ToLower() == "desc"
                    ? filteredUsers.OrderByDescending(u => u.LastLoginAt)
                    : filteredUsers.OrderBy(u => u.LastLoginAt),
                _ => filteredUsers.OrderBy(u => u.Username)
            };

            // ページング
            var totalCount = filteredUsers.Count();
            var pagedUsers = filteredUsers
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            // 倉庫情報を取得
            var warehouseIds = pagedUsers.Where(u => u.WarehouseId.HasValue).Select(u => u.WarehouseId!.Value).Distinct().ToList();
            var warehouses = warehouseIds.Any() 
                ? await _context.Warehouses.Where(w => warehouseIds.Contains(w.Id)).ToListAsync()
                : new List<Warehouse>();
            var warehouseDict = warehouses.ToDictionary(w => w.Id, w => w.Name);

            var userListDtos = pagedUsers.Select(u => new UserListDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email.Value,
                FullName = $"{u.FirstName} {u.LastName}",
                RoleName = roleDict.GetValueOrDefault(u.RoleId, "Unknown"),
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt,
                WarehouseId = u.WarehouseId,
                WarehouseName = u.WarehouseId.HasValue ? warehouseDict.GetValueOrDefault(u.WarehouseId.Value) : null
            }).ToList();

            var result = new PagedResultDto<UserListDto>(userListDtos, totalCount, searchDto.Page, searchDto.PageSize);

            _logger.LogInformation("ユーザー一覧を取得しました。件数: {Count}", result.TotalCount);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザー一覧の取得中にエラーが発生しました");
            throw;
        }
    }

    public async Task<UserDetailDto?> GetUserByIdAsync(int userId)
    {
        _logger.LogInformation("ユーザー詳細を取得します。ユーザーID: {UserId}", userId);

        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("ユーザーが見つかりません。ユーザーID: {UserId}", userId);
                return null;
            }

            // 倉庫情報を取得
            string? warehouseName = null;
            if (user.WarehouseId.HasValue)
            {
                var warehouse = await _context.Warehouses.FindAsync(user.WarehouseId.Value);
                warehouseName = warehouse?.Name;
            }

            var userDetailDto = new UserDetailDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? "Unknown",
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                WarehouseId = user.WarehouseId,
                WarehouseName = warehouseName
            };

            _logger.LogInformation("ユーザー詳細を取得しました。ユーザーID: {UserId}", userId);
            return userDetailDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザー詳細の取得中にエラーが発生しました。ユーザーID: {UserId}", userId);
            throw;
        }
    }

    public async Task<UserDetailDto> CreateUserAsync(CreateUserDto createDto)
    {
        _logger.LogInformation("ユーザーを作成します。ユーザー名: {Username}", createDto.Username);

        try
        {
            // 重複チェック
            if (await IsUsernameExistsAsync(createDto.Username))
            {
                throw new InvalidOperationException($"ユーザー名 '{createDto.Username}' は既に使用されています。");
            }

            if (await IsEmailExistsAsync(createDto.Email))
            {
                throw new InvalidOperationException($"メールアドレス '{createDto.Email}' は既に使用されています。");
            }

            // ロールの存在チェック
            var role = await _roleRepository.GetByIdAsync(createDto.RoleId);
            if (role == null)
            {
                throw new InvalidOperationException($"ロールID {createDto.RoleId} が見つかりません。");
            }

            // 倉庫担当ユーザーの場合、倉庫の存在チェック
            if (createDto.RoleId == 3 && createDto.WarehouseId.HasValue) // 3 = 倉庫担当
            {
                var warehouse = await _context.Warehouses.FindAsync(createDto.WarehouseId.Value);
                if (warehouse == null)
                {
                    throw new InvalidOperationException($"倉庫ID {createDto.WarehouseId.Value} が見つかりません。");
                }
            }

            // パスワードをハッシュ化
            var hashedPassword = _passwordService.HashPassword(createDto.Password);

            // ユーザーを作成
            var user = User.Create(
                createDto.Username,
                createDto.Email,
                createDto.FirstName,
                createDto.LastName,
                createDto.RoleId);

            // パスワードを設定
            user.SetPasswordHash(hashedPassword);

            // 倉庫担当ユーザーの場合、倉庫を設定
            if (createDto.RoleId == 3 && createDto.WarehouseId.HasValue)
            {
                user.SetWarehouse((int?)createDto.WarehouseId.Value);
            }

            await _userRepository.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("ユーザーを作成しました。ユーザーID: {UserId}, ユーザー名: {Username}", user.Id, user.Username);

            // 倉庫情報を取得
            string? warehouseName = null;
            if (user.WarehouseId.HasValue)
            {
                var warehouse = await _context.Warehouses.FindAsync(user.WarehouseId.Value);
                warehouseName = warehouse?.Name;
            }

            return new UserDetailDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = user.RoleId,
                RoleName = role.Name,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                WarehouseId = user.WarehouseId,
                WarehouseName = warehouseName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーの作成中にエラーが発生しました。ユーザー名: {Username}", createDto.Username);
            throw;
        }
    }

    public async Task<UserDetailDto> UpdateUserAsync(int userId, UpdateUserDto updateDto)
    {
        _logger.LogInformation("ユーザーを更新します。ユーザーID: {UserId}", userId);

        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"ユーザーID {userId} が見つかりません。");
            }

            // 重複チェック（自分以外）
            if (await IsUsernameExistsAsync(updateDto.Username, userId))
            {
                throw new InvalidOperationException($"ユーザー名 '{updateDto.Username}' は既に使用されています。");
            }

            if (await IsEmailExistsAsync(updateDto.Email, userId))
            {
                throw new InvalidOperationException($"メールアドレス '{updateDto.Email}' は既に使用されています。");
            }

            // ロールの存在チェック
            var role = await _roleRepository.GetByIdAsync(updateDto.RoleId);
            if (role == null)
            {
                throw new InvalidOperationException($"ロールID {updateDto.RoleId} が見つかりません。");
            }

            // 倉庫担当ユーザーの場合、倉庫の存在チェック
            if (updateDto.RoleId == 3 && updateDto.WarehouseId.HasValue) // 3 = 倉庫担当
            {
                var warehouse = await _context.Warehouses.FindAsync(updateDto.WarehouseId.Value);
                if (warehouse == null)
                {
                    throw new InvalidOperationException($"倉庫ID {updateDto.WarehouseId.Value} が見つかりません。");
                }
            }

            // ユーザー情報を更新
            user.UpdateUserInfo(updateDto.Username, updateDto.Email, updateDto.FirstName, updateDto.LastName);
            user.UpdateRole(updateDto.RoleId);
            user.SetActiveStatus(updateDto.IsActive);

            // 倉庫担当ユーザーの場合、倉庫を設定
            if (updateDto.RoleId == 3 && updateDto.WarehouseId.HasValue)
            {
                user.SetWarehouse((int?)updateDto.WarehouseId.Value);
            }
            else if (updateDto.RoleId != 3)
            {
                // 倉庫担当以外の場合は倉庫をクリア
                user.SetWarehouse((int?)null);
            }

            await _userRepository.UpdateAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("ユーザーを更新しました。ユーザーID: {UserId}", userId);

            // 倉庫情報を取得
            string? warehouseName = null;
            if (user.WarehouseId.HasValue)
            {
                var warehouse = await _context.Warehouses.FindAsync(user.WarehouseId.Value);
                warehouseName = warehouse?.Name;
            }

            return new UserDetailDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = user.RoleId,
                RoleName = role.Name,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                WarehouseId = user.WarehouseId,
                WarehouseName = warehouseName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーの更新中にエラーが発生しました。ユーザーID: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        _logger.LogInformation("ユーザーを削除します。ユーザーID: {UserId}", userId);

        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("削除対象のユーザーが見つかりません。ユーザーID: {UserId}", userId);
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("ユーザーを削除しました。ユーザーID: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーの削除中にエラーが発生しました。ユーザーID: {UserId}", userId);
            throw;
        }
    }

    public async Task<UserDetailDto> ToggleUserStatusAsync(int userId, ToggleUserStatusDto toggleDto)
    {
        _logger.LogInformation("ユーザーの有効/無効を切り替えます。ユーザーID: {UserId}, 有効: {IsActive}", userId, toggleDto.IsActive);

        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"ユーザーID {userId} が見つかりません。");
            }

            user.SetActiveStatus(toggleDto.IsActive);

            await _userRepository.UpdateAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("ユーザーの有効/無効を切り替えました。ユーザーID: {UserId}, 有効: {IsActive}", userId, toggleDto.IsActive);

            return new UserDetailDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? "Unknown",
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーの有効/無効切り替え中にエラーが発生しました。ユーザーID: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> IsUsernameExistsAsync(string username, int? excludeUserId = null)
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            return users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                                 (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザー名の重複チェック中にエラーが発生しました。ユーザー名: {Username}", username);
            throw;
        }
    }

    public async Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null)
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            return users.Any(u => u.Email.Value.Equals(email, StringComparison.OrdinalIgnoreCase) && 
                                 (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "メールアドレスの重複チェック中にエラーが発生しました。メールアドレス: {Email}", email);
            throw;
        }
    }

    public async Task<bool> ResetUserPasswordAsync(int userId, string newPassword)
    {
        _logger.LogInformation("ユーザーのパスワードをリセットします。ユーザーID: {UserId}", userId);

        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("パスワードリセット対象のユーザーが見つかりません。ユーザーID: {UserId}", userId);
                return false;
            }

            var hashedPassword = _passwordService.HashPassword(newPassword);
            user.SetPasswordHash(hashedPassword);

            await _userRepository.UpdateAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("ユーザーのパスワードをリセットしました。ユーザーID: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーのパスワードリセット中にエラーが発生しました。ユーザーID: {UserId}", userId);
            throw;
        }
    }

    public async Task<PagedResultDto<object>> GetUserLoginHistoryAsync(int userId, int page = 1, int pageSize = 10)
    {
        _logger.LogInformation("ユーザーのログイン履歴を取得します。ユーザーID: {UserId}", userId);

        try
        {
            // 簡易版：最終ログイン日時のみ返す
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"ユーザーID {userId} が見つかりません。");
            }

            var loginHistory = new List<object>();
            if (user.LastLoginAt.HasValue)
            {
                loginHistory.Add(new
                {
                    LoginAt = user.LastLoginAt.Value,
                    IpAddress = "127.0.0.1", // 簡易版
                    UserAgent = "Unknown" // 簡易版
                });
            }

            var result = new PagedResultDto<object>(loginHistory, loginHistory.Count, page, pageSize);

            _logger.LogInformation("ユーザーのログイン履歴を取得しました。ユーザーID: {UserId}, 件数: {Count}", userId, result.TotalCount);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーのログイン履歴取得中にエラーが発生しました。ユーザーID: {UserId}", userId);
            throw;
        }
    }
}
