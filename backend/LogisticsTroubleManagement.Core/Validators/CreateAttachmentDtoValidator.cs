using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validators;

public class CreateAttachmentDtoValidator : AbstractValidator<CreateAttachmentDto>
{
    public CreateAttachmentDtoValidator()
    {
        RuleFor(x => x.IncidentId)
            .GreaterThan(0)
            .WithMessage("インシデントIDは必須です。");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("ファイル名は必須です。")
            .MaximumLength(255)
            .WithMessage("ファイル名は255文字以内で入力してください。");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("コンテンツタイプは必須です。")
            .MaximumLength(100)
            .WithMessage("コンテンツタイプは100文字以内で入力してください。");

        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .WithMessage("ファイルサイズは0より大きい値を入力してください。")
            .LessThanOrEqualTo(10 * 1024 * 1024) // 10MB
            .WithMessage("ファイルサイズは10MB以下で入力してください。");

        RuleFor(x => x.UploadedById)
            .GreaterThan(0)
            .WithMessage("アップロード者IDは必須です。");
    }
}
