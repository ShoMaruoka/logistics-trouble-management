using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validators;

/// <summary>
/// 事務員専用インシデント登録DTOのバリデータ
/// 仕様書2.3新しいワークフローに準拠：基本情報のみ検証
/// </summary>
public class ClerkIncidentDtoValidator : AbstractValidator<ClerkIncidentDto>
{
    public ClerkIncidentDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("タイトルは必須です。")
            .MaximumLength(200).WithMessage("タイトルは200文字以内で入力してください。");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("詳細説明は必須です。")
            .MaximumLength(2000).WithMessage("詳細説明は2000文字以内で入力してください。");

        RuleFor(x => x.IncidentDetails)
            .NotEmpty().WithMessage("発生経緯は必須です。")
            .MaximumLength(5000).WithMessage("発生経緯は5000文字以内で入力してください。");

        RuleFor(x => x.OccurrenceDate)
            .NotNull().WithMessage("発生日は必須です。")
            .Must(date => date != DateTime.MinValue)
            .WithMessage("発生日は有効な日付を入力してください。");

        RuleFor(x => x.OccurrenceLocation)
            .MaximumLength(200).WithMessage("発生場所は200文字以内で入力してください。");

        RuleFor(x => x.ReportedById)
            .GreaterThan(0).WithMessage("報告者IDは正の整数で入力してください。");
    }
}
