using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Core.DTOs;

public class CreateIncidentDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    public int ReportedById { get; set; }
    public int? AssignedToId { get; set; }
}
