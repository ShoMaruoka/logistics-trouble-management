using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validation;

public class LegacyClassifyIncidentDtoValidator : AbstractValidator<LegacyClassifyIncidentDto>
{
    public LegacyClassifyIncidentDtoValidator()
    {
        RuleFor(x => x.IncidentId)
            .GreaterThan(0)
            .WithMessage("インシデントIDは必須です。");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("分類は必須です。")
            .MaximumLength(100)
            .WithMessage("分類は100文字以内で入力してください。");

        RuleFor(x => x.ClassificationNotes)
            .MaximumLength(500)
            .WithMessage("分類メモは500文字以内で入力してください。");
    }
}

/// <summary>
/// 新ワークフロー版インシデント分類DTOバリデーター
/// </summary>
public class ClassifyIncidentDtoValidator : AbstractValidator<ClassifyIncidentDto>
{
    public ClassifyIncidentDtoValidator()
    {
        RuleFor(x => x.IncidentId)
            .GreaterThan(0)
            .WithMessage("インシデントIDは必須です。");

        RuleFor(x => x.TroubleTypeId)
            .GreaterThan(0)
            .WithMessage("トラブル種類は必須です。");

        RuleFor(x => x.DamageTypeId)
            .GreaterThan(0)
            .WithMessage("損傷種類は必須です。");

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .WithMessage("出荷元倉庫は必須です。");

        RuleFor(x => x.ShippingCompanyId)
            .GreaterThan(0)
            .WithMessage("運送会社は必須です。");

        RuleFor(x => x.TotalShipments)
            .GreaterThanOrEqualTo(0)
            .WithMessage("出荷総数は0以上の値を入力してください。");

        RuleFor(x => x.DefectiveItems)
            .GreaterThanOrEqualTo(0)
            .WithMessage("不良品数は0以上の値を入力してください。");

        RuleFor(x => x.DefectiveItems)
            .LessThanOrEqualTo(x => x.TotalShipments)
            .WithMessage("不良品数は出荷総数以下である必要があります。");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("対応期限は未来の日付を指定してください。");
    }
}
