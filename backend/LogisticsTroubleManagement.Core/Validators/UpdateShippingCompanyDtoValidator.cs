using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validators
{
    public class UpdateShippingCompanyDtoValidator : AbstractValidator<UpdateShippingCompanyDto>
    {
        public UpdateShippingCompanyDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("名称は必須です")
                .MaximumLength(100).WithMessage("名称は100文字以内で入力してください");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("説明は500文字以内で入力してください");

            RuleFor(x => x.CompanyType)
                .NotEmpty().WithMessage("会社種別は必須です")
                .MaximumLength(50).WithMessage("会社種別は50文字以内で入力してください");

            RuleFor(x => x.ContactInfo)
                .MaximumLength(200).WithMessage("連絡先は200文字以内で入力してください");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("表示順序は0以上の値を入力してください");
        }
    }
}
