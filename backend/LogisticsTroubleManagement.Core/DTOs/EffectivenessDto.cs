using System.Text.Json.Serialization;

namespace LogisticsTroubleManagement.Core.DTOs;

public class EffectivenessDto
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public string IncidentTitle { get; set; } = string.Empty;
    public string EffectivenessType { get; set; } = string.Empty;
    public decimal BeforeValue { get; set; }
    public decimal AfterValue { get; set; }
    public decimal ImprovementRate { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime MeasuredAt { get; set; }
    public string MeasuredBy { get; set; } = string.Empty;
    
    // サーバーサイドで管理されるフィールド - クライアント入力から保護
    [JsonIgnore]
    public DateTime CreatedAt { get; set; }
    [JsonIgnore]
    public DateTime UpdatedAt { get; set; }
}

public class CreateEffectivenessDto
{
    public int IncidentId { get; set; }
    public string EffectivenessType { get; set; } = string.Empty;
    public decimal BeforeValue { get; set; }
    public decimal AfterValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public int MeasuredById { get; set; }
}

public class UpdateEffectivenessDto
{
    public string EffectivenessType { get; set; } = string.Empty;
    public decimal BeforeValue { get; set; }
    public decimal AfterValue { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class EffectivenessSearchDto
{
    public int? IncidentId { get; set; }
    public string? EffectivenessType { get; set; }
    public int? MeasuredById { get; set; }
    public DateTime? MeasuredFrom { get; set; }
    public DateTime? MeasuredTo { get; set; }
    public decimal? MinImprovementRate { get; set; }
    public decimal? MaxImprovementRate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "MeasuredAt";
    public bool Ascending { get; set; } = false;
}

public class EffectivenessSummaryDto
{
    public int TotalMeasurements { get; set; }
    public decimal AverageImprovementRate { get; set; }
    public decimal MaxImprovementRate { get; set; }
    public decimal MinImprovementRate { get; set; }
    public Dictionary<string, int> EffectivenessTypeCounts { get; set; } = new Dictionary<string, int>();
    public List<EffectivenessDto> RecentMeasurements { get; set; } = new List<EffectivenessDto>();
}
