using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Services;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogisticsTroubleManagement.Infrastructure.Services;

/// <summary>
/// ロール管理サービス実装
/// </summary>
public class RoleManagementService : IRoleManagementService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RoleManagementService> _logger;

    public RoleManagementService(
        IRoleRepository roleRepository,
        IUserRepository userRepository,
        ApplicationDbContext context,
        ILogger<RoleManagementService> logger)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResultDto<RoleListDto>> GetRolesAsync(RoleSearchDto searchDto)
    {
        var query = _context.Roles.AsQueryable();

        // キーワード検索
        if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
        {
            var keyword = searchDto.Keyword.Trim();
            query = query.Where(r => r.Name.Contains(keyword) || 
                                   (r.Description != null && r.Description.Contains(keyword)));
        }

        // 有効フラグでフィルタ
        if (searchDto.IsActive.HasValue)
        {
            query = query.Where(r => r.IsActive == searchDto.IsActive.Value);
        }

        // ソート
        query = searchDto.SortBy?.ToLower() switch
        {
            "name" => searchDto.SortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(r => r.Name)
                : query.OrderBy(r => r.Name),
            "createdat" => searchDto.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(r => r.CreatedAt)
                : query.OrderBy(r => r.CreatedAt),
            "usercount" => searchDto.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(r => r.Users.Count)
                : query.OrderBy(r => r.Users.Count),
            _ => query.OrderBy(r => r.Name)
        };

        // 総件数を取得
        var totalCount = await query.CountAsync();

        // ページング
        var roles = await query
            .Include(r => r.Users)
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        var roleListDtos = roles.Select(ConvertToRoleListDto).ToList();

        return new PagedResultDto<RoleListDto>(
            roleListDtos,
            searchDto.Page,
            searchDto.PageSize,
            totalCount);
    }

    public async Task<RoleDetailDto?> GetRoleByIdAsync(int id)
    {
        var role = await _context.Roles
            .Include(r => r.Users)
            .FirstOrDefaultAsync(r => r.Id == id);

        return role != null ? ConvertToRoleDetailDto(role) : null;
    }

    public async Task<RoleDetailDto> CreateRoleAsync(CreateRoleDto createDto)
    {
        // ロール名の重複チェック
        if (await IsRoleNameExistsAsync(createDto.Name))
        {
            throw new InvalidOperationException($"ロール名 '{createDto.Name}' は既に存在します。");
        }

        var role = Role.Create(createDto.Name, createDto.Description);
        
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        _logger.LogInformation("ロール '{RoleName}' を作成しました。ID: {RoleId}", role.Name, role.Id);

        return ConvertToRoleDetailDto(role);
    }

    public async Task<RoleDetailDto> UpdateRoleAsync(int id, UpdateRoleDto updateDto)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
        {
            throw new InvalidOperationException($"ロールID {id} が見つかりません。");
        }

        // ロール名の重複チェック（自分以外）
        if (await IsRoleNameExistsAsync(updateDto.Name, id))
        {
            throw new InvalidOperationException($"ロール名 '{updateDto.Name}' は既に存在します。");
        }

        role.UpdateName(updateDto.Name);
        role.UpdateDescription(updateDto.Description);

        await _context.SaveChangesAsync();

        _logger.LogInformation("ロール '{RoleName}' を更新しました。ID: {RoleId}", role.Name, role.Id);

        return ConvertToRoleDetailDto(role);
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
        {
            return false;
        }

        // 使用中のロールは削除できない
        if (await IsRoleInUseAsync(id))
        {
            throw new InvalidOperationException($"ロール '{role.Name}' は使用中のため削除できません。");
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        _logger.LogInformation("ロール '{RoleName}' を削除しました。ID: {RoleId}", role.Name, role.Id);

        return true;
    }

    public async Task<RoleDetailDto> ToggleRoleStatusAsync(int id, ToggleRoleStatusDto toggleDto)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role == null)
        {
            throw new InvalidOperationException($"ロールID {id} が見つかりません。");
        }

        if (toggleDto.IsActive)
        {
            role.Activate();
        }
        else
        {
            role.Deactivate();
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("ロール '{RoleName}' のステータスを {Status} に変更しました。ID: {RoleId}", 
            role.Name, toggleDto.IsActive ? "有効" : "無効", role.Id);

        return ConvertToRoleDetailDto(role);
    }

    public async Task<bool> IsRoleNameExistsAsync(string name, int? excludeRoleId = null)
    {
        var query = _context.Roles.Where(r => r.Name == name);
        
        if (excludeRoleId.HasValue)
        {
            query = query.Where(r => r.Id != excludeRoleId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<RoleListDto>> GetActiveRolesAsync()
    {
        var roles = await _roleRepository.GetActiveRolesAsync();
        return roles.Select(ConvertToRoleListDto);
    }

    public async Task<bool> IsRoleInUseAsync(int id)
    {
        return await _context.Users.AnyAsync(u => u.RoleId == id);
    }

    private static RoleListDto ConvertToRoleListDto(Role role)
    {
        return new RoleListDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            UserCount = role.Users?.Count ?? 0,
            CreatedAt = role.CreatedAt
        };
    }

    private static RoleDetailDto ConvertToRoleDetailDto(Role role)
    {
        return new RoleDetailDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt,
            UserCount = role.Users?.Count ?? 0
        };
    }
}
