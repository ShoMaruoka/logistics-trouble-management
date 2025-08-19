using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validators;

public class CreateEffectivenessDtoValidator : AbstractValidator<CreateEffectivenessDto>
{
    public CreateEffectivenessDtoValidator()
    {
        RuleFor(x => x.IncidentId)
            .GreaterThan(0)
            .WithMessage("インシデントIDは必須です。");

        RuleFor(x => x.EffectivenessType)
            .NotEmpty()
            .WithMessage("効果測定タイプは必須です。")
            .MaximumLength(100)
            .WithMessage("効果測定タイプは100文字以内で入力してください。");

        RuleFor(x => x.BeforeValue)
            .GreaterThanOrEqualTo(0)
            .WithMessage("改善前の値は0以上で入力してください。");

        RuleFor(x => x.AfterValue)
            .GreaterThanOrEqualTo(0)
            .WithMessage("改善後の値は0以上で入力してください。");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("説明は1000文字以内で入力してください。");

        RuleFor(x => x.MeasuredById)
            .GreaterThan(0)
            .WithMessage("測定者IDは必須です。");
    }
}
