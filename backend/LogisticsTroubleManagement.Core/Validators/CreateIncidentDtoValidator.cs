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
    }
}
