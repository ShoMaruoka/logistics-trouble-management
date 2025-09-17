using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validation;

public class WarehouseStaffUpdateIncidentDtoValidator : AbstractValidator<WarehouseStaffUpdateIncidentDto>
{
    public WarehouseStaffUpdateIncidentDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("タイトルは必須です。")
            .MaximumLength(200)
            .WithMessage("タイトルは200文字以内で入力してください。");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("説明は必須です。")
            .MaximumLength(1000)
            .WithMessage("説明は1000文字以内で入力してください。");

        RuleFor(x => x.Summary)
            .NotEmpty()
            .WithMessage("概要は必須です。")
            .MaximumLength(500)
            .WithMessage("概要は500文字以内で入力してください。");

        RuleFor(x => x.Cause)
            .NotEmpty()
            .WithMessage("原因は必須です。")
            .MaximumLength(1000)
            .WithMessage("原因は1000文字以内で入力してください。");

        RuleFor(x => x.PreventionMeasures)
            .NotEmpty()
            .WithMessage("再発防止策は必須です。")
            .MaximumLength(1000)
            .WithMessage("再発防止策は1000文字以内で入力してください。");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("分類は必須です。")
            .MaximumLength(100)
            .WithMessage("分類は100文字以内で入力してください。");

        RuleFor(x => x.ClassificationNotes)
            .MaximumLength(500)
            .WithMessage("分類メモは500文字以内で入力してください。");

        RuleFor(x => x.ExpectedResolutionDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("期待解決日は現在日時より後の日付を入力してください。")
            .When(x => x.ExpectedResolutionDate.HasValue);
    }
}
