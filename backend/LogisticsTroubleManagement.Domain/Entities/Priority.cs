namespace LogisticsTroubleManagement.Domain.Entities;

/// <summary>
/// 優先度マスタエンティティ
/// </summary>
public class PriorityMaster : BaseEntity
{
    public string Name { get; private set; }
    public int DisplayOrder { get; private set; }
    public string? Color { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public virtual ICollection<Incident> Incidents { get; private set; } = new List<Incident>();

    private PriorityMaster() { } // For EF Core

    public PriorityMaster(string name, int displayOrder, string? color = null, string? description = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DisplayOrder = displayOrder;
        Color = color;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static PriorityMaster Create(string name, int displayOrder, string? color = null, string? description = null)
    {
        return new PriorityMaster(name, displayOrder, color, description);
    }

    public void UpdateDetails(string name, int displayOrder, string? color = null, string? description = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DisplayOrder = displayOrder;
        Color = color;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDisplayOrder(int newDisplayOrder)
    {
        DisplayOrder = newDisplayOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}
