using FluentValidation;
using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Validators;

/// <summary>
/// ロール作成DTOバリデーター
/// </summary>
public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("ロール名を入力してください")
            .Length(2, 50).WithMessage("ロール名は2文字以上50文字以内で入力してください")
            .Matches(@"^[a-zA-Z0-9_\-\s]+$").WithMessage("ロール名は英数字、アンダースコア、ハイフン、スペースのみ使用できます");

        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("説明は200文字以内で入力してください");
    }
}

/// <summary>
/// ロール更新DTOバリデーター
/// </summary>
public class UpdateRoleDtoValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("ロール名を入力してください")
            .Length(2, 50).WithMessage("ロール名は2文字以上50文字以内で入力してください")
            .Matches(@"^[a-zA-Z0-9_\-\s]+$").WithMessage("ロール名は英数字、アンダースコア、ハイフン、スペースのみ使用できます");

        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("説明は200文字以内で入力してください");
    }
}

/// <summary>
/// ロール検索DTOバリデーター
/// </summary>
public class RoleSearchDtoValidator : AbstractValidator<RoleSearchDto>
{
    public RoleSearchDtoValidator()
    {
        RuleFor(x => x.Keyword)
            .MaximumLength(100).WithMessage("検索キーワードは100文字以内で入力してください");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("ページ番号は1以上の値を入力してください");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("ページサイズは1以上100以下の値を入力してください");

        RuleFor(x => x.SortBy)
            .MaximumLength(50).WithMessage("ソートフィールドは50文字以内で入力してください")
            .Must(BeValidSortField).WithMessage("無効なソートフィールドです");

        RuleFor(x => x.SortOrder)
            .MaximumLength(10).WithMessage("ソート順は10文字以内で入力してください")
            .Must(BeValidSortOrder).WithMessage("ソート順は 'asc' または 'desc' を指定してください");
    }

    private static bool BeValidSortField(string? sortBy)
    {
        if (string.IsNullOrEmpty(sortBy))
            return true;

        var validFields = new[] { "name", "createdat", "usercount" };
        return validFields.Contains(sortBy.ToLower());
    }

    private static bool BeValidSortOrder(string? sortOrder)
    {
        if (string.IsNullOrEmpty(sortOrder))
            return true;

        var validOrders = new[] { "asc", "desc" };
        return validOrders.Contains(sortOrder.ToLower());
    }
}

/// <summary>
/// ロールステータス切り替えDTOバリデーター
/// </summary>
public class ToggleRoleStatusDtoValidator : AbstractValidator<ToggleRoleStatusDto>
{
    public ToggleRoleStatusDtoValidator()
    {
        RuleFor(x => x.Reason)
            .MaximumLength(200).WithMessage("変更理由は200文字以内で入力してください");
    }
}

/// <summary>
/// ロール名重複チェックDTOバリデーター
/// </summary>
public class RoleNameCheckDtoValidator : AbstractValidator<RoleNameCheckDto>
{
    public RoleNameCheckDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("ロール名を入力してください")
            .Length(2, 50).WithMessage("ロール名は2文字以上50文字以内で入力してください")
            .Matches(@"^[a-zA-Z0-9_\-\s]+$").WithMessage("ロール名は英数字、アンダースコア、ハイフン、スペースのみ使用できます");

        RuleFor(x => x.ExcludeRoleId)
            .GreaterThan(0).WithMessage("除外ロールIDは1以上の値を入力してください")
            .When(x => x.ExcludeRoleId.HasValue);
    }
}
