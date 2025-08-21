using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Core.DTOs;

public class IncidentSearchDto
{
    public string? SearchTerm { get; set; }
    public IncidentStatus? Status { get; set; }
    public Priority? Priority { get; set; }
    public string? Category { get; set; }
    public int? ReportedById { get; set; }
    public int? AssignedToId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IsOverdue { get; set; }
    public string? SortBy { get; set; } = "OccurrenceDate";
    public bool Ascending { get; set; } = false;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
