using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validators;

public class UpdateEffectivenessDtoValidator : AbstractValidator<UpdateEffectivenessDto>
{
    public UpdateEffectivenessDtoValidator()
    {
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
    }
}
