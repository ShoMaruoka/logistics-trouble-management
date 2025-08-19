using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Core.DTOs;

public class IncidentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; }
    public Priority Priority { get; set; }
    public string Category { get; set; } = string.Empty;
    public int ReportedById { get; set; }
    public string ReportedByName { get; set; } = string.Empty;
    public int? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime ReportedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? Resolution { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int AttachmentCount { get; set; }
    public bool IsOverdue { get; set; }
    public TimeSpan? ResolutionTime { get; set; }
}
