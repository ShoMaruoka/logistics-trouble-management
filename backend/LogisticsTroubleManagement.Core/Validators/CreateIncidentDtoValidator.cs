using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validators;

public class CreateIncidentDtoValidator : AbstractValidator<CreateIncidentDto>
{
    public CreateIncidentDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("タイトルは必須です。")
            .MaximumLength(200).WithMessage("タイトルは200文字以内で入力してください。");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("説明は必須です。")
            .MaximumLength(2000).WithMessage("説明は2000文字以内で入力してください。");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("カテゴリは必須です。")
            .MaximumLength(50).WithMessage("カテゴリは50文字以内で入力してください。");

        RuleFor(x => x.ReportedById)
            .GreaterThan(0).WithMessage("報告者IDは正の整数で入力してください。");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("優先度は有効な値を選択してください。");

        RuleFor(x => x.TroubleType)
            .IsInEnum().WithMessage("トラブル種類は有効な値を選択してください。");

        RuleFor(x => x.DamageType)
            .IsInEnum().WithMessage("損傷の種類は有効な値を選択してください。");

        RuleFor(x => x.Warehouse)
            .IsInEnum().WithMessage("出荷元倉庫は有効な値を選択してください。");

        RuleFor(x => x.ShippingCompany)
            .IsInEnum().WithMessage("運送会社名は有効な値を選択してください。");

        // 新規追加項目のバリデーション
        RuleFor(x => x.IncidentDetails)
            .NotEmpty().WithMessage("発生経緯は必須です。")
            .MaximumLength(5000).WithMessage("発生経緯は5000文字以内で入力してください。");

        RuleFor(x => x.TotalShipments)
            .GreaterThanOrEqualTo(0).WithMessage("出荷総数は0以上の値を入力してください。");

        RuleFor(x => x.DefectiveItems)
            .GreaterThanOrEqualTo(0).WithMessage("不良品数は0以上の値を入力してください。");

        RuleFor(x => x.OccurrenceDate)
            .NotNull().WithMessage("発生日は必須です。")
            .Must(date => date != DateTime.MinValue)
            .WithMessage("発生日は有効な日付を入力してください。");

        RuleFor(x => x.OccurrenceLocation)
            .MaximumLength(200).WithMessage("発生場所は200文字以内で入力してください。");

        RuleFor(x => x.Summary)
            .MaximumLength(1000).WithMessage("概要は1000文字以内で入力してください。");

        RuleFor(x => x.Cause)
            .MaximumLength(2000).WithMessage("原因は2000文字以内で入力してください。");

        RuleFor(x => x.PreventionMeasures)
            .MaximumLength(2000).WithMessage("再発防止策は2000文字以内で入力してください。");

        RuleFor(x => x.EffectivenessStatus)
            .IsInEnum().WithMessage("有効性評価は有効な値を選択してください。");

        RuleFor(x => x.EffectivenessComment)
            .MaximumLength(1000).WithMessage("有効性確認コメントは1000文字以内で入力してください。");
    }
}
