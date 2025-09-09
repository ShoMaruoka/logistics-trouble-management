using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validators;

/// <summary>
/// ユーザー作成DTOのバリデーター
/// </summary>
public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("ユーザー名を入力してください")
            .Length(3, 50).WithMessage("ユーザー名は3文字以上50文字以内で入力してください")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("ユーザー名は英数字とアンダースコアのみ使用できます");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("メールアドレスを入力してください")
            .EmailAddress().WithMessage("有効なメールアドレスを入力してください")
            .MaximumLength(100).WithMessage("メールアドレスは100文字以内で入力してください");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("パスワードを入力してください")
            .Length(8, 100).WithMessage("パスワードは8文字以上100文字以内で入力してください")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$")
            .WithMessage("パスワードは大文字・小文字・数字・記号を含む必要があります");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("姓を入力してください")
            .MaximumLength(50).WithMessage("姓は50文字以内で入力してください");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("名を入力してください")
            .MaximumLength(50).WithMessage("名は50文字以内で入力してください");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("有効なロールを選択してください");
    }
}

/// <summary>
/// ユーザー更新DTOのバリデーター
/// </summary>
public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("ユーザー名を入力してください")
            .Length(3, 50).WithMessage("ユーザー名は3文字以上50文字以内で入力してください")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("ユーザー名は英数字とアンダースコアのみ使用できます");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("メールアドレスを入力してください")
            .EmailAddress().WithMessage("有効なメールアドレスを入力してください")
            .MaximumLength(100).WithMessage("メールアドレスは100文字以内で入力してください");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("姓を入力してください")
            .MaximumLength(50).WithMessage("姓は50文字以内で入力してください");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("名を入力してください")
            .MaximumLength(50).WithMessage("名は50文字以内で入力してください");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("有効なロールを選択してください");
    }
}

/// <summary>
/// ユーザー検索DTOのバリデーター
/// </summary>
public class UserSearchDtoValidator : AbstractValidator<UserSearchDto>
{
    public UserSearchDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("ページ番号は1以上である必要があります");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("ページサイズは1以上100以下である必要があります");

        RuleFor(x => x.SortBy)
            .Must(BeValidSortField).WithMessage("無効なソート項目です");

        RuleFor(x => x.SortOrder)
            .Must(BeValidSortOrder).WithMessage("ソート順は 'asc' または 'desc' である必要があります");
    }

    private bool BeValidSortField(string sortBy)
    {
        var validFields = new[] { "Username", "Email", "FirstName", "LastName", "LastLoginAt", "CreatedAt" };
        return validFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }

    private bool BeValidSortOrder(string sortOrder)
    {
        return sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
               sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// ユーザーステータス切り替えDTOのバリデーター
/// </summary>
public class ToggleUserStatusDtoValidator : AbstractValidator<ToggleUserStatusDto>
{
    public ToggleUserStatusDtoValidator()
    {
        RuleFor(x => x.Reason)
            .MaximumLength(200).WithMessage("理由は200文字以内で入力してください")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}

/// <summary>
/// パスワードリセット要求DTOのバリデーター
/// </summary>
public class ResetUserPasswordRequestValidator : AbstractValidator<ResetUserPasswordRequest>
{
    public ResetUserPasswordRequestValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("新しいパスワードを入力してください")
            .Length(8, 100).WithMessage("パスワードは8文字以上100文字以内で入力してください")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$")
            .WithMessage("パスワードは大文字・小文字・数字・記号を含む必要があります");
    }
}
