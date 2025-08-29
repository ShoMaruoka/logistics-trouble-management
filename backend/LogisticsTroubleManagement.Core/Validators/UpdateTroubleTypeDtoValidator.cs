using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validators
{
    public class UpdateTroubleTypeDtoValidator : AbstractValidator<UpdateTroubleTypeDto>
    {
        public UpdateTroubleTypeDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("名称は必須です")
                .MaximumLength(100).WithMessage("名称は100文字以内で入力してください");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("説明は500文字以内で入力してください");

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("色は必須です")
                .Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("色は有効なHEX形式で入力してください（例：#3B82F6）");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("表示順序は0以上の値を入力してください");
        }
    }
}
