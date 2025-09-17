using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Core.DTOs;

/// <summary>
/// 倉庫担当ダッシュボード用の統計情報DTO
/// </summary>
public class WarehouseStaffDashboardDto
{
    public int TotalIncidents { get; set; }
    public int UnresolvedIncidents { get; set; }
    public int InProgressIncidents { get; set; }
    public int AssignedWarehouseIncidents { get; set; }
    public int ClassifiedIncidents { get; set; }
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
}

/// <summary>
/// 倉庫担当用インシデント一覧DTO
/// </summary>
public class WarehouseStaffIncidentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime OccurrenceDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ReportedBy { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string TroubleTypeName { get; set; } = string.Empty;
    public string DamageTypeName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsClassified { get; set; }
}

/// <summary>
/// 倉庫担当用インシデント検索DTO
/// </summary>
public class WarehouseStaffIncidentSearchDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public IncidentStatus? Status { get; set; }
    public Priority? Priority { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? TroubleTypeId { get; set; }
    public int? DamageTypeId { get; set; }
    public bool? IsClassified { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortOrder { get; set; } = "desc";
}

/// <summary>
/// 倉庫担当用インシデント分類DTO
/// </summary>
public class ClassifyIncidentDto
{
    public int IncidentId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string ClassificationNotes { get; set; } = string.Empty;
}

/// <summary>
/// 倉庫担当用インシデント更新DTO
/// </summary>
public class WarehouseStaffUpdateIncidentDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Cause { get; set; } = string.Empty;
    public string PreventionMeasures { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ClassificationNotes { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime? ExpectedResolutionDate { get; set; }
}
