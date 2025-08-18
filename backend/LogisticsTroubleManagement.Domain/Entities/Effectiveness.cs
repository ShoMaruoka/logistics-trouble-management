namespace LogisticsTroubleManagement.Domain.Entities;

public class Effectiveness : BaseEntity
{
    public int IncidentId { get; private set; }
    public string EffectivenessType { get; private set; }
    public decimal Value { get; private set; }
    public string? Unit { get; private set; }
    public string? Description { get; private set; }
    public int MeasuredById { get; private set; }
    public DateTime MeasuredAt { get; private set; }

    // Navigation properties
    public virtual Incident Incident { get; private set; } = null!;
    public virtual User MeasuredBy { get; private set; } = null!;

    private Effectiveness() { } // For EF Core

    public Effectiveness(int incidentId, string effectivenessType, decimal value, int measuredById, string? unit = null, string? description = null)
    {
        IncidentId = incidentId;
        EffectivenessType = effectivenessType ?? throw new ArgumentNullException(nameof(effectivenessType));
        Value = value;
        MeasuredById = measuredById;
        Unit = unit;
        Description = description;
        MeasuredAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Effectiveness Create(int incidentId, string effectivenessType, decimal value, int measuredById, string? unit = null, string? description = null)
    {
        if (value < 0)
            throw new ArgumentException("Value cannot be negative", nameof(value));

        return new Effectiveness(incidentId, effectivenessType, value, measuredById, unit, description);
    }

    public void UpdateValue(decimal newValue)
    {
        if (newValue < 0)
            throw new ArgumentException("Value cannot be negative", nameof(newValue));

        Value = newValue;
        MeasuredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateUnit(string? unit)
    {
        Unit = unit;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetFormattedValue()
    {
        if (string.IsNullOrEmpty(Unit))
            return Value.ToString("F2");

        return $"{Value:F2} {Unit}";
    }

    public bool IsPositiveEffect()
    {
        return Value > 0;
    }

    public bool IsNegativeEffect()
    {
        return Value < 0;
    }

    public bool IsNeutralEffect()
    {
        return Value == 0;
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
        var formattedValue = GetFormattedValue();
        
        if (string.IsNullOrEmpty(Description))
            return $"{category}: {formattedValue}";
        
        return $"{category}: {formattedValue} - {Description}";
    }
}
