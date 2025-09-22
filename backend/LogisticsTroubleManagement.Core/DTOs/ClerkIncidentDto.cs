using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.Core.DTOs;

/// <summary>
/// 事務員専用のインシデント登録DTO
/// 仕様書2.3新しいワークフローに準拠：基本情報のみ入力
/// </summary>
public class ClerkIncidentDto : IValidatableObject
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IncidentDetails { get; set; } = string.Empty; // 発生経緯
    public DateTime OccurrenceDate { get; set; } // 発生日
    public string OccurrenceLocation { get; set; } = string.Empty; // 発生場所
    public int ReportedById { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        // 基本的なバリデーションのみ実施
        if (string.IsNullOrWhiteSpace(Title))
        {
            results.Add(new ValidationResult("タイトルは必須です。", new[] { nameof(Title) }));
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            results.Add(new ValidationResult("詳細説明は必須です。", new[] { nameof(Description) }));
        }

        if (string.IsNullOrWhiteSpace(IncidentDetails))
        {
            results.Add(new ValidationResult("発生経緯は必須です。", new[] { nameof(IncidentDetails) }));
        }

        if (OccurrenceDate == DateTime.MinValue)
        {
            results.Add(new ValidationResult("発生日は必須です。", new[] { nameof(OccurrenceDate) }));
        }

        if (ReportedById <= 0)
        {
            results.Add(new ValidationResult("報告者IDは正の整数で入力してください。", new[] { nameof(ReportedById) }));
        }

        return results;
    }
}
