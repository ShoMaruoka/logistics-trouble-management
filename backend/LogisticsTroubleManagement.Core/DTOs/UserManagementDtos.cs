using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.Core.DTOs;

/// <summary>
/// ユーザー作成要求DTO
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// ユーザー名
    /// </summary>
    [Required(ErrorMessage = "ユーザー名を入力してください")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "ユーザー名は3文字以上50文字以内で入力してください")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "ユーザー名は英数字とアンダースコアのみ使用できます")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    [Required(ErrorMessage = "メールアドレスを入力してください")]
    [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください")]
    [StringLength(100, ErrorMessage = "メールアドレスは100文字以内で入力してください")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// パスワード
    /// </summary>
    [Required(ErrorMessage = "パスワードを入力してください")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "パスワードは8文字以上100文字以内で入力してください")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$", 
        ErrorMessage = "パスワードは大文字・小文字・数字・記号を含む必要があります")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 姓
    /// </summary>
    [Required(ErrorMessage = "姓を入力してください")]
    [StringLength(50, ErrorMessage = "姓は50文字以内で入力してください")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// 名
    /// </summary>
    [Required(ErrorMessage = "名を入力してください")]
    [StringLength(50, ErrorMessage = "名は50文字以内で入力してください")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// ロールID
    /// </summary>
    [Required(ErrorMessage = "ロールを選択してください")]
    [Range(1, int.MaxValue, ErrorMessage = "有効なロールを選択してください")]
    public int RoleId { get; set; }
}

/// <summary>
/// ユーザー更新要求DTO
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// ユーザー名
    /// </summary>
    [Required(ErrorMessage = "ユーザー名を入力してください")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "ユーザー名は3文字以上50文字以内で入力してください")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "ユーザー名は英数字とアンダースコアのみ使用できます")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    [Required(ErrorMessage = "メールアドレスを入力してください")]
    [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください")]
    [StringLength(100, ErrorMessage = "メールアドレスは100文字以内で入力してください")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 姓
    /// </summary>
    [Required(ErrorMessage = "姓を入力してください")]
    [StringLength(50, ErrorMessage = "姓は50文字以内で入力してください")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// 名
    /// </summary>
    [Required(ErrorMessage = "名を入力してください")]
    [StringLength(50, ErrorMessage = "名は50文字以内で入力してください")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// ロールID
    /// </summary>
    [Required(ErrorMessage = "ロールを選択してください")]
    [Range(1, int.MaxValue, ErrorMessage = "有効なロールを選択してください")]
    public int RoleId { get; set; }

    /// <summary>
    /// 有効フラグ
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// ユーザー詳細DTO
/// </summary>
public class UserDetailDto
{
    /// <summary>
    /// ユーザーID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ユーザー名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 姓
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// 名
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// フルネーム
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// ロールID
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// ロール名
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 有効フラグ
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 最終ログイン日時
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// ユーザー一覧DTO
/// </summary>
public class UserListDto
{
    /// <summary>
    /// ユーザーID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ユーザー名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// フルネーム
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// ロール名
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 有効フラグ
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 最終ログイン日時
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// ユーザー検索DTO
/// </summary>
public class UserSearchDto
{
    /// <summary>
    /// 検索キーワード（ユーザー名、メールアドレス、名前）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// ロールID
    /// </summary>
    public int? RoleId { get; set; }

    /// <summary>
    /// 有効フラグ
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// ページ番号
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// ページサイズ
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// ソート項目
    /// </summary>
    public string SortBy { get; set; } = "Username";

    /// <summary>
    /// ソート順（asc/desc）
    /// </summary>
    public string SortOrder { get; set; } = "asc";
}

/// <summary>
/// ユーザー有効化/無効化DTO
/// </summary>
public class ToggleUserStatusDto
{
    /// <summary>
    /// 有効フラグ
    /// </summary>
    [Required]
    public bool IsActive { get; set; }

    /// <summary>
    /// 理由
    /// </summary>
    [StringLength(200, ErrorMessage = "理由は200文字以内で入力してください")]
    public string? Reason { get; set; }
}

/// <summary>
/// パスワードリセット要求DTO
/// </summary>
public class ResetUserPasswordRequest
{
    /// <summary>
    /// 新しいパスワード
    /// </summary>
    [Required(ErrorMessage = "新しいパスワードを入力してください")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "パスワードは8文字以上100文字以内で入力してください")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$", 
        ErrorMessage = "パスワードは大文字・小文字・数字・記号を含む必要があります")]
    public string NewPassword { get; set; } = string.Empty;
}
