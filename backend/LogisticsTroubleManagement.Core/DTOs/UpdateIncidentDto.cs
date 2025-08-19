using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Core.DTOs;

public class UpdateIncidentDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public IncidentStatus Status { get; set; }
    public int? AssignedToId { get; set; }
    public string? Resolution { get; set; }
}
