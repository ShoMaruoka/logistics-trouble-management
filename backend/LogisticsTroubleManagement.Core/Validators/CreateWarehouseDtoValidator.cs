using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validators
{
    public class CreateWarehouseDtoValidator : AbstractValidator<CreateWarehouseDto>
    {
        public CreateWarehouseDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("名称は必須です")
                .MaximumLength(100).WithMessage("名称は100文字以内で入力してください");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("説明は500文字以内で入力してください");

            RuleFor(x => x.Location)
                .MaximumLength(200).WithMessage("所在地は200文字以内で入力してください");

            RuleFor(x => x.ContactInfo)
                .MaximumLength(200).WithMessage("連絡先は200文字以内で入力してください");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("表示順序は0以上の値を入力してください");
        }
    }
}
