namespace LogisticsTroubleManagement.Domain.Entities;

public class Effectiveness : BaseEntity
{
    public int IncidentId { get; private set; }
    public string EffectivenessType { get; private set; } = string.Empty;
    public decimal BeforeValue { get; private set; }
    public decimal AfterValue { get; private set; }
    public decimal ImprovementRate { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int MeasuredById { get; private set; }
    public DateTime MeasuredAt { get; private set; }

    // Navigation properties
    public virtual Incident Incident { get; private set; } = null!;
    public virtual User MeasuredBy { get; private set; } = null!;

    private Effectiveness() { } // For EF Core

    public Effectiveness(int incidentId, string effectivenessType, decimal beforeValue, decimal afterValue, decimal improvementRate, string description, int measuredById)
    {
        IncidentId = incidentId;
        EffectivenessType = effectivenessType ?? throw new ArgumentNullException(nameof(effectivenessType));
        BeforeValue = beforeValue;
        AfterValue = afterValue;
        ImprovementRate = improvementRate;
        Description = description ?? throw new ArgumentNullException(nameof(description));
        MeasuredById = measuredById;
        MeasuredAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Effectiveness Create(int incidentId, string effectivenessType, decimal beforeValue, decimal afterValue, decimal improvementRate, string description, int measuredById)
    {
        if (beforeValue < 0)
            throw new ArgumentException("Before value cannot be negative", nameof(beforeValue));

        if (afterValue < 0)
            throw new ArgumentException("After value cannot be negative", nameof(afterValue));

        if (string.IsNullOrWhiteSpace(effectivenessType))
            throw new ArgumentException("Effectiveness type cannot be empty", nameof(effectivenessType));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        return new Effectiveness(incidentId, effectivenessType, beforeValue, afterValue, improvementRate, description, measuredById);
    }

    public void UpdateEffectiveness(string effectivenessType, decimal beforeValue, decimal afterValue, decimal improvementRate, string description)
    {
        if (beforeValue < 0)
            throw new ArgumentException("Before value cannot be negative", nameof(beforeValue));

        if (afterValue < 0)
            throw new ArgumentException("After value cannot be negative", nameof(afterValue));

        if (string.IsNullOrWhiteSpace(effectivenessType))
            throw new ArgumentException("Effectiveness type cannot be empty", nameof(effectivenessType));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        EffectivenessType = effectivenessType;
        BeforeValue = beforeValue;
        AfterValue = afterValue;
        ImprovementRate = improvementRate;
        Description = description;
        MeasuredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetFormattedImprovementRate()
    {
        return $"{ImprovementRate:F2}%";
    }

    public bool IsPositiveEffect()
    {
        return ImprovementRate > 0;
    }

    public bool IsNegativeEffect()
    {
        return ImprovementRate < 0;
    }

    public bool IsNeutralEffect()
    {
        return ImprovementRate == 0;
    }

    public string GetEffectivenessCategory()
    {
        return EffectivenessType switch
        {
            "COST_REDUCTION" => "コスト削減",
            "TIME_SAVING" => "時間短縮",
            "QUALITY_IMPROVEMENT" => "品質向上",
            "CUSTOMER_SATISFACTION" => "顧客満足度",
            "PRODUCTIVITY" => "生産性向上",
            "SAFETY" => "安全性向上",
            "ENVIRONMENTAL" => "環境改善",
            _ => EffectivenessType
        };
    }

    public string GetEffectivenessDescription()
    {
        var category = GetEffectivenessCategory();
        var formattedValue = GetFormattedImprovementRate();
        
        if (string.IsNullOrEmpty(Description))
            return $"{category}: {formattedValue}";
        
        return $"{category}: {formattedValue} - {Description}";
    }
}
