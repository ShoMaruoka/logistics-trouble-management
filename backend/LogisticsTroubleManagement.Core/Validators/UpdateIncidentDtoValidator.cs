using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validators;

public class UpdateIncidentDtoValidator : AbstractValidator<UpdateIncidentDto>
{
    public UpdateIncidentDtoValidator()
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

        RuleFor(x => x.Resolution)
            .MaximumLength(2000).WithMessage("解決内容は2000文字以内で入力してください。")
            .When(x => !string.IsNullOrEmpty(x.Resolution));
    }
}
