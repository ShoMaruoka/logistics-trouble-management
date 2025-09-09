using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validators;

/// <summary>
/// パスワード変更DTOバリデーター
/// </summary>
public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("現在のパスワードを入力してください");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("新しいパスワードを入力してください")
            .Length(8, 100).WithMessage("パスワードは8文字以上100文字以内で入力してください")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$")
            .WithMessage("パスワードは大文字・小文字・数字・記号を含む必要があります")
            .NotEqual(x => x.CurrentPassword).WithMessage("新しいパスワードは現在のパスワードと異なる必要があります");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("新しいパスワード（確認用）を入力してください")
            .Equal(x => x.NewPassword).WithMessage("新しいパスワードと確認用パスワードが一致しません");
    }
}

/// <summary>
/// パスワードリセットDTOバリデーター（管理者用）
/// </summary>
public class AdminResetPasswordDtoValidator : AbstractValidator<AdminResetPasswordDto>
{
    public AdminResetPasswordDtoValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("有効なユーザーIDを入力してください");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("新しいパスワードを入力してください")
            .Length(8, 100).WithMessage("パスワードは8文字以上100文字以内で入力してください")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$")
            .WithMessage("パスワードは大文字・小文字・数字・記号を含む必要があります");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("新しいパスワード（確認用）を入力してください")
            .Equal(x => x.NewPassword).WithMessage("新しいパスワードと確認用パスワードが一致しません");

        RuleFor(x => x.Reason)
            .MaximumLength(200).WithMessage("リセット理由は200文字以内で入力してください");
    }
}

/// <summary>
/// パスワード強度チェックDTOバリデーター
/// </summary>
public class PasswordStrengthCheckDtoValidator : AbstractValidator<PasswordStrengthCheckDto>
{
    public PasswordStrengthCheckDtoValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("パスワードを入力してください");
    }
}

/// <summary>
/// パスワードポリシーDTOバリデーター
/// </summary>
public class PasswordPolicyDtoValidator : AbstractValidator<PasswordPolicyDto>
{
    public PasswordPolicyDtoValidator()
    {
        RuleFor(x => x.MinLength)
            .InclusiveBetween(4, 50).WithMessage("最小文字数は4文字以上50文字以下で入力してください");

        RuleFor(x => x.MaxLength)
            .InclusiveBetween(8, 200).WithMessage("最大文字数は8文字以上200文字以下で入力してください");

        RuleFor(x => x.MaxLength)
            .GreaterThanOrEqualTo(x => x.MinLength).WithMessage("最大文字数は最小文字数以上である必要があります");

        RuleFor(x => x.HistoryCount)
            .InclusiveBetween(0, 20).WithMessage("履歴保持数は0以上20以下で入力してください");

        RuleFor(x => x.ExpirationDays)
            .GreaterThan(0).WithMessage("パスワード有効期限は1日以上で入力してください")
            .LessThanOrEqualTo(365).WithMessage("パスワード有効期限は365日以内で入力してください")
            .When(x => x.ExpirationDays.HasValue);

        RuleFor(x => x.MinStrength)
            .Must(BeValidStrength).WithMessage("有効な強度レベルを指定してください");
    }

    private static bool BeValidStrength(string strength)
    {
        var validStrengths = new[] { "Weak", "Medium", "Strong", "VeryStrong" };
        return validStrengths.Contains(strength);
    }
}
