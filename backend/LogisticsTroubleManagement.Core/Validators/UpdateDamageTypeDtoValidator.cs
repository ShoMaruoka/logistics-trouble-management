using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validators
{
    public class UpdateDamageTypeDtoValidator : AbstractValidator<UpdateDamageTypeDto>
    {
        public UpdateDamageTypeDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("名称は必須です")
                .MaximumLength(100).WithMessage("名称は100文字以内で入力してください");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("説明は500文字以内で入力してください");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("カテゴリは必須です")
                .MaximumLength(50).WithMessage("カテゴリは50文字以内で入力してください");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("表示順序は0以上の値を入力してください");
        }
    }
}
