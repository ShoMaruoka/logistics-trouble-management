using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validation;

public class ClassifyIncidentDtoValidator : AbstractValidator<ClassifyIncidentDto>
{
    public ClassifyIncidentDtoValidator()
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
