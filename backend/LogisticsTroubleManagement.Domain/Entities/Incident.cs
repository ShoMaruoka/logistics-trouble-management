using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Domain.Entities;

public class Incident : BaseEntity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public IncidentStatus Status { get; private set; }
    public Priority Priority { get; private set; }
    public string Category { get; private set; }
    public int ReportedById { get; private set; }
    public int? AssignedToId { get; private set; }
    public DateTime ReportedDate { get; private set; }
    public DateTime? ResolvedDate { get; private set; }
    public string? Resolution { get; private set; }

    // Navigation properties
    public virtual User ReportedBy { get; private set; } = null!;
    public virtual User? AssignedTo { get; private set; }
    public virtual ICollection<Attachment> Attachments { get; private set; } = new List<Attachment>();
    public virtual ICollection<AuditLog> AuditLogs { get; private set; } = new List<AuditLog>();
    public virtual ICollection<Effectiveness> Effectiveness { get; private set; } = new List<Effectiveness>();

    private Incident() { } // For EF Core

    public Incident(string title, string description, string category, int reportedById, Priority priority = Priority.Medium)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        ReportedById = reportedById;
        Priority = priority;
        Status = IncidentStatus.Open;
        ReportedDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Incident Create(string title, string description, string category, int reportedById, Priority priority = Priority.Medium)
    {
        return new Incident(title, description, category, reportedById, priority);
    }

    public void AssignTo(int userId)
    {
        AssignedToId = userId;
        Status = IncidentStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unassign()
    {
        AssignedToId = null;
        Status = IncidentStatus.Open;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(IncidentStatus newStatus)
    {
        if (Status == newStatus) return;

        Status = newStatus;
        
        if (newStatus == IncidentStatus.Resolved || newStatus == IncidentStatus.Closed)
        {
            ResolvedDate = DateTime.UtcNow;
        }
        else if (newStatus == IncidentStatus.Open || newStatus == IncidentStatus.InProgress)
        {
            ResolvedDate = null;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePriority(Priority newPriority)
    {
        Priority = newPriority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string description, string category)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        UpdatedAt = DateTime.UtcNow;
    }

    public void Resolve(string resolution)
    {
        if (string.IsNullOrWhiteSpace(resolution))
            throw new ArgumentException("Resolution cannot be empty", nameof(resolution));

        Resolution = resolution;
        Status = IncidentStatus.Resolved;
        ResolvedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        if (Status != IncidentStatus.Resolved)
            throw new InvalidOperationException("Incident must be resolved before it can be closed");

        Status = IncidentStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = IncidentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsResolved()
    {
        return Status == IncidentStatus.Resolved || Status == IncidentStatus.Closed;
    }

    public bool IsActive()
    {
        return Status == IncidentStatus.Open || Status == IncidentStatus.InProgress;
    }

    public TimeSpan GetResolutionTime()
    {
        if (!ResolvedDate.HasValue)
            return TimeSpan.Zero;

        return ResolvedDate.Value - ReportedDate;
    }

    public bool IsOverdue(TimeSpan expectedResolutionTime)
    {
        if (IsResolved())
            return false;

        return DateTime.UtcNow - ReportedDate > expectedResolutionTime;
    }

    public void AddAttachment(Attachment attachment)
    {
        if (attachment == null)
            throw new ArgumentNullException(nameof(attachment));

        Attachments.Add(attachment);
    }

    public void RemoveAttachment(int attachmentId)
    {
        var attachment = Attachments.FirstOrDefault(a => a.Id == attachmentId);
        if (attachment != null)
        {
            Attachments.Remove(attachment);
        }
    }
}
