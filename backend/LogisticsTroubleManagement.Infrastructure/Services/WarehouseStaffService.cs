using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Services;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogisticsTroubleManagement.Infrastructure.Services;

/// <summary>
/// 倉庫担当専用サービス実装
/// </summary>
public class WarehouseStaffService : IWarehouseStaffService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WarehouseStaffService> _logger;

    public WarehouseStaffService(ApplicationDbContext context, ILogger<WarehouseStaffService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<WarehouseStaffDashboardDto> GetDashboardAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Warehouse)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.WarehouseId == null)
        {
            throw new UnauthorizedAccessException("ユーザーに担当倉庫が設定されていません。");
        }

        var warehouseId = user.WarehouseId.Value;
        var warehouseName = user.Warehouse?.Name ?? "不明";

        // 担当倉庫のインシデント統計
        var totalIncidents = await _context.Incidents
            .CountAsync(i => i.WarehouseId == warehouseId);

        var unresolvedIncidents = await _context.Incidents
            .CountAsync(i => i.WarehouseId == warehouseId && i.Status == IncidentStatus.Open);

        var inProgressIncidents = await _context.Incidents
            .CountAsync(i => i.WarehouseId == warehouseId && i.Status == IncidentStatus.InProgress);

        var assignedWarehouseIncidents = totalIncidents; // 担当倉庫の全インシデント

        var classifiedIncidents = await _context.Incidents
            .CountAsync(i => i.WarehouseId == warehouseId && !string.IsNullOrEmpty(i.Category));

        return new WarehouseStaffDashboardDto
        {
            TotalIncidents = totalIncidents,
            UnresolvedIncidents = unresolvedIncidents,
            InProgressIncidents = inProgressIncidents,
            AssignedWarehouseIncidents = assignedWarehouseIncidents,
            ClassifiedIncidents = classifiedIncidents,
            WarehouseId = warehouseId,
            WarehouseName = warehouseName
        };
    }

    public async Task<PagedResultDto<WarehouseStaffIncidentDto>> GetAssignedWarehouseIncidentsAsync(
        int userId, WarehouseStaffIncidentSearchDto searchDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.WarehouseId == null)
        {
            throw new UnauthorizedAccessException("ユーザーに担当倉庫が設定されていません。");
        }

        var query = _context.Incidents
            .Include(i => i.ReportedBy)
            .Include(i => i.AssignedTo)
            .Include(i => i.Warehouse)
            .Include(i => i.TroubleType)
            .Include(i => i.DamageType)
            .Where(i => i.WarehouseId == user.WarehouseId);

        // 検索条件の適用
        query = ApplySearchFilters(query, searchDto);

        // ソート
        query = ApplySorting(query, searchDto.SortBy, searchDto.SortOrder);

        var totalCount = await query.CountAsync();

        var incidents = await query
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .Select(i => new WarehouseStaffIncidentDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                Status = i.Status,
                Priority = i.Priority,
                OccurrenceDate = i.OccurrenceDate,
                CreatedAt = i.CreatedAt,
                ReportedBy = i.ReportedBy != null ? i.ReportedBy.GetFullName() : "不明",
                AssignedTo = i.AssignedTo != null ? i.AssignedTo.GetFullName() : "未割り当て",
                WarehouseName = i.Warehouse != null ? i.Warehouse.Name : "不明",
                TroubleTypeName = i.TroubleType != null ? i.TroubleType.Name : "不明",
                DamageTypeName = i.DamageType != null ? i.DamageType.Name : "不明",
                Category = i.Category ?? "未分類",
                IsClassified = !string.IsNullOrEmpty(i.Category)
            })
            .ToListAsync();

        return new PagedResultDto<WarehouseStaffIncidentDto>(
            incidents,
            totalCount,
            searchDto.Page,
            searchDto.PageSize
        );
    }

    public async Task<PagedResultDto<WarehouseStaffIncidentDto>> GetClassifiedIncidentsAsync(
        int userId, WarehouseStaffIncidentSearchDto searchDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.WarehouseId == null)
        {
            throw new UnauthorizedAccessException("ユーザーに担当倉庫が設定されていません。");
        }

        var query = _context.Incidents
            .Include(i => i.ReportedBy)
            .Include(i => i.AssignedTo)
            .Include(i => i.Warehouse)
            .Include(i => i.TroubleType)
            .Include(i => i.DamageType)
            .Where(i => i.WarehouseId == user.WarehouseId && !string.IsNullOrEmpty(i.Category));

        // 検索条件の適用
        query = ApplySearchFilters(query, searchDto);

        // ソート
        query = ApplySorting(query, searchDto.SortBy, searchDto.SortOrder);

        var totalCount = await query.CountAsync();

        var incidents = await query
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .Select(i => new WarehouseStaffIncidentDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                Status = i.Status,
                Priority = i.Priority,
                OccurrenceDate = i.OccurrenceDate,
                CreatedAt = i.CreatedAt,
                ReportedBy = i.ReportedBy != null ? i.ReportedBy.GetFullName() : "不明",
                AssignedTo = i.AssignedTo != null ? i.AssignedTo.GetFullName() : "未割り当て",
                WarehouseName = i.Warehouse != null ? i.Warehouse.Name : "不明",
                TroubleTypeName = i.TroubleType != null ? i.TroubleType.Name : "不明",
                DamageTypeName = i.DamageType != null ? i.DamageType.Name : "不明",
                Category = i.Category ?? "未分類",
                IsClassified = true
            })
            .ToListAsync();

        return new PagedResultDto<WarehouseStaffIncidentDto>(
            incidents,
            totalCount,
            searchDto.Page,
            searchDto.PageSize
        );
    }

    public async Task<PagedResultDto<WarehouseStaffIncidentDto>> GetUnresolvedIncidentsAsync(
        int userId, WarehouseStaffIncidentSearchDto searchDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.WarehouseId == null)
        {
            throw new UnauthorizedAccessException("ユーザーに担当倉庫が設定されていません。");
        }

        var query = _context.Incidents
            .Include(i => i.ReportedBy)
            .Include(i => i.AssignedTo)
            .Include(i => i.Warehouse)
            .Include(i => i.TroubleType)
            .Include(i => i.DamageType)
            .Where(i => i.WarehouseId == user.WarehouseId && i.Status == IncidentStatus.Open);

        // 検索条件の適用
        query = ApplySearchFilters(query, searchDto);

        // ソート
        query = ApplySorting(query, searchDto.SortBy, searchDto.SortOrder);

        var totalCount = await query.CountAsync();

        var incidents = await query
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .Select(i => new WarehouseStaffIncidentDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                Status = i.Status,
                Priority = i.Priority,
                OccurrenceDate = i.OccurrenceDate,
                CreatedAt = i.CreatedAt,
                ReportedBy = i.ReportedBy != null ? i.ReportedBy.GetFullName() : "不明",
                AssignedTo = i.AssignedTo != null ? i.AssignedTo.GetFullName() : "未割り当て",
                WarehouseName = i.Warehouse != null ? i.Warehouse.Name : "不明",
                TroubleTypeName = i.TroubleType != null ? i.TroubleType.Name : "不明",
                DamageTypeName = i.DamageType != null ? i.DamageType.Name : "不明",
                Category = i.Category ?? "未分類",
                IsClassified = !string.IsNullOrEmpty(i.Category)
            })
            .ToListAsync();

        return new PagedResultDto<WarehouseStaffIncidentDto>(
            incidents,
            totalCount,
            searchDto.Page,
            searchDto.PageSize
        );
    }

    public async Task<PagedResultDto<WarehouseStaffIncidentDto>> GetInProgressIncidentsAsync(
        int userId, WarehouseStaffIncidentSearchDto searchDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.WarehouseId == null)
        {
            throw new UnauthorizedAccessException("ユーザーに担当倉庫が設定されていません。");
        }

        var query = _context.Incidents
            .Include(i => i.ReportedBy)
            .Include(i => i.AssignedTo)
            .Include(i => i.Warehouse)
            .Include(i => i.TroubleType)
            .Include(i => i.DamageType)
            .Where(i => i.WarehouseId == user.WarehouseId && i.Status == IncidentStatus.InProgress);

        // 検索条件の適用
        query = ApplySearchFilters(query, searchDto);

        // ソート
        query = ApplySorting(query, searchDto.SortBy, searchDto.SortOrder);

        var totalCount = await query.CountAsync();

        var incidents = await query
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .Select(i => new WarehouseStaffIncidentDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                Status = i.Status,
                Priority = i.Priority,
                OccurrenceDate = i.OccurrenceDate,
                CreatedAt = i.CreatedAt,
                ReportedBy = i.ReportedBy != null ? i.ReportedBy.GetFullName() : "不明",
                AssignedTo = i.AssignedTo != null ? i.AssignedTo.GetFullName() : "未割り当て",
                WarehouseName = i.Warehouse != null ? i.Warehouse.Name : "不明",
                TroubleTypeName = i.TroubleType != null ? i.TroubleType.Name : "不明",
                DamageTypeName = i.DamageType != null ? i.DamageType.Name : "不明",
                Category = i.Category ?? "未分類",
                IsClassified = !string.IsNullOrEmpty(i.Category)
            })
            .ToListAsync();

        return new PagedResultDto<WarehouseStaffIncidentDto>(
            incidents,
            totalCount,
            searchDto.Page,
            searchDto.PageSize
        );
    }

    public async Task<bool> ClassifyIncidentAsync(int userId, ClassifyIncidentDto classifyDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.WarehouseId == null)
        {
            throw new UnauthorizedAccessException("ユーザーに担当倉庫が設定されていません。");
        }

        var incident = await _context.Incidents
            .FirstOrDefaultAsync(i => i.Id == classifyDto.IncidentId && i.WarehouseId == user.WarehouseId);

        if (incident == null)
        {
            throw new UnauthorizedAccessException("指定されたインシデントにアクセス権限がありません。");
        }

        incident.UpdateCategory(classifyDto.Category, classifyDto.ClassificationNotes);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateIncidentAsync(int userId, int incidentId, WarehouseStaffUpdateIncidentDto updateDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.WarehouseId == null)
        {
            throw new UnauthorizedAccessException("ユーザーに担当倉庫が設定されていません。");
        }

        var incident = await _context.Incidents
            .FirstOrDefaultAsync(i => i.Id == incidentId && i.WarehouseId == user.WarehouseId);

        if (incident == null)
        {
            throw new UnauthorizedAccessException("指定されたインシデントにアクセス権限がありません。");
        }

        incident.UpdateDetails(
            updateDto.Title,
            updateDto.Description,
            updateDto.Summary,
            updateDto.Cause,
            updateDto.PreventionMeasures,
            updateDto.Category,
            updateDto.ClassificationNotes,
            updateDto.Status,
            updateDto.Priority,
            updateDto.ExpectedResolutionDate
        );

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsWarehouseStaffAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.IsWarehouseStaff() == true;
    }

    public async Task<int?> GetUserWarehouseIdAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.WarehouseId;
    }

    public async Task<bool> HasWarehouseAccessAsync(int userId, int warehouseId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.HasWarehouseAccess(warehouseId) == true;
    }

    private IQueryable<Incident> ApplySearchFilters(IQueryable<Incident> query, WarehouseStaffIncidentSearchDto searchDto)
    {
        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            query = query.Where(i => i.Title.Contains(searchDto.SearchTerm) ||
                                   i.Description.Contains(searchDto.SearchTerm) ||
                                   i.Summary.Contains(searchDto.SearchTerm));
        }

        if (searchDto.Status.HasValue)
        {
            query = query.Where(i => i.Status == searchDto.Status.Value);
        }

        if (searchDto.Priority.HasValue)
        {
            query = query.Where(i => i.Priority == searchDto.Priority.Value);
        }

        if (searchDto.FromDate.HasValue)
        {
            query = query.Where(i => i.OccurrenceDate >= searchDto.FromDate.Value);
        }

        if (searchDto.ToDate.HasValue)
        {
            query = query.Where(i => i.OccurrenceDate <= searchDto.ToDate.Value);
        }

        if (searchDto.TroubleTypeId.HasValue)
        {
            query = query.Where(i => i.TroubleTypeId == searchDto.TroubleTypeId.Value);
        }

        if (searchDto.DamageTypeId.HasValue)
        {
            query = query.Where(i => i.DamageTypeId == searchDto.DamageTypeId.Value);
        }

        if (searchDto.IsClassified.HasValue)
        {
            if (searchDto.IsClassified.Value)
            {
                query = query.Where(i => !string.IsNullOrEmpty(i.Category));
            }
            else
            {
                query = query.Where(i => string.IsNullOrEmpty(i.Category));
            }
        }

        return query;
    }

    private IQueryable<Incident> ApplySorting(IQueryable<Incident> query, string? sortBy, string? sortOrder)
    {
        var isDescending = sortOrder?.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "title" => isDescending ? query.OrderByDescending(i => i.Title) : query.OrderBy(i => i.Title),
            "status" => isDescending ? query.OrderByDescending(i => i.Status) : query.OrderBy(i => i.Status),
            "priority" => isDescending ? query.OrderByDescending(i => i.Priority) : query.OrderBy(i => i.Priority),
            "occurrencedate" => isDescending ? query.OrderByDescending(i => i.OccurrenceDate) : query.OrderBy(i => i.OccurrenceDate),
            "createdat" => isDescending ? query.OrderByDescending(i => i.CreatedAt) : query.OrderBy(i => i.CreatedAt),
            _ => isDescending ? query.OrderByDescending(i => i.CreatedAt) : query.OrderBy(i => i.CreatedAt)
        };
    }
}
