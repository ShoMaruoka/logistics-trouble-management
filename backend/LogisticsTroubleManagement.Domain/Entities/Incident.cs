using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Domain.Entities;

public class Incident : BaseEntity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public IncidentStatus Status { get; private set; }
    public Priority Priority { get; private set; }
    public string Category { get; private set; }
    
    // 物流特化項目
    public TroubleType TroubleType { get; private set; }
    public DamageType DamageType { get; private set; }
    public Warehouse Warehouse { get; private set; }
    public ShippingCompany ShippingCompany { get; private set; }
    public EffectivenessStatus EffectivenessStatus { get; private set; }
    
    // 新規追加項目（提供サイト対応）
    public string IncidentDetails { get; private set; } = string.Empty; // 発生経緯
    public int TotalShipments { get; private set; } = 0; // 出荷総数
    public int DefectiveItems { get; private set; } = 0; // 不良品数
    public DateTime OccurrenceDate { get; private set; } // 発生日
    public string OccurrenceLocation { get; private set; } = string.Empty; // 発生場所
    public string Summary { get; private set; } = string.Empty; // 概要
    public string? Cause { get; private set; } // 原因
    public string? PreventionMeasures { get; private set; } // 再発防止策
    public DateTime? EffectivenessDate { get; private set; } // 有効性確認日
    public string EffectivenessComment { get; private set; } = string.Empty; // 有効性確認コメント
    
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

    public Incident(string title, string description, string category, int reportedById, 
        TroubleType troubleType, DamageType damageType, Warehouse warehouse, 
        ShippingCompany shippingCompany, DateTime occurrenceDate, Priority priority = Priority.Medium,
        string incidentDetails = "", int totalShipments = 0, int defectiveItems = 0,
        string occurrenceLocation = "", string summary = "",
        string cause = "", string preventionMeasures = "")
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        ReportedById = reportedById;
        TroubleType = troubleType;
        DamageType = damageType;
        Warehouse = warehouse;
        ShippingCompany = shippingCompany;
        EffectivenessStatus = EffectivenessStatus.NotImplemented;
        Priority = priority;
        Status = IncidentStatus.Open;
        
        // 新規追加項目
        IncidentDetails = incidentDetails;
        TotalShipments = totalShipments;
        DefectiveItems = defectiveItems;
        OccurrenceDate = occurrenceDate;
        OccurrenceLocation = occurrenceLocation;
        Summary = summary;
        Cause = cause;
        PreventionMeasures = preventionMeasures;
        
        ReportedDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Incident Create(string title, string description, string category, int reportedById, 
        TroubleType troubleType, DamageType damageType, Warehouse warehouse, 
        ShippingCompany shippingCompany, DateTime occurrenceDate, Priority priority = Priority.Medium,
        string incidentDetails = "", int totalShipments = 0, int defectiveItems = 0,
        string occurrenceLocation = "", string summary = "",
        string cause = "", string preventionMeasures = "")
    {
        return new Incident(title, description, category, reportedById, troubleType, damageType, 
            warehouse, shippingCompany, occurrenceDate, priority, incidentDetails, totalShipments, defectiveItems,
            occurrenceLocation, summary, cause, preventionMeasures);
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

    public void UpdateLogisticsDetails(TroubleType troubleType, DamageType damageType, 
        Warehouse warehouse, ShippingCompany shippingCompany)
    {
        TroubleType = troubleType;
        DamageType = damageType;
        Warehouse = warehouse;
        ShippingCompany = shippingCompany;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEffectivenessStatus(EffectivenessStatus status)
    {
        EffectivenessStatus = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateExtendedDetails(string incidentDetails, int totalShipments, int defectiveItems,
        DateTime occurrenceDate, string occurrenceLocation, string summary, string cause, 
        string preventionMeasures, DateTime? effectivenessDate, string effectivenessComment)
    {
        IncidentDetails = incidentDetails;
        TotalShipments = totalShipments;
        DefectiveItems = defectiveItems;
        OccurrenceDate = occurrenceDate;
        OccurrenceLocation = occurrenceLocation;
        Summary = summary;
        Cause = cause;
        PreventionMeasures = preventionMeasures;
        EffectivenessDate = effectivenessDate;
        EffectivenessComment = effectivenessComment;
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
