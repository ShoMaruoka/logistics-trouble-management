using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.Core.DTOs;

/// <summary>
/// ロール作成用DTO
/// </summary>
public class CreateRoleDto
{
    /// <summary>
    /// ロール名
    /// </summary>
    [Required(ErrorMessage = "ロール名を入力してください")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "ロール名は2文字以上50文字以内で入力してください")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 説明
    /// </summary>
    [StringLength(200, ErrorMessage = "説明は200文字以内で入力してください")]
    public string? Description { get; set; }
}

/// <summary>
/// ロール更新用DTO
/// </summary>
public class UpdateRoleDto
{
    /// <summary>
    /// ロール名
    /// </summary>
    [Required(ErrorMessage = "ロール名を入力してください")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "ロール名は2文字以上50文字以内で入力してください")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 説明
    /// </summary>
    [StringLength(200, ErrorMessage = "説明は200文字以内で入力してください")]
    public string? Description { get; set; }
}

/// <summary>
/// ロール詳細DTO
/// </summary>
public class RoleDetailDto
{
    /// <summary>
    /// ロールID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ロール名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 説明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 有効フラグ
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// ユーザー数
    /// </summary>
    public int UserCount { get; set; }
}

/// <summary>
/// ロール一覧DTO
/// </summary>
public class RoleListDto
{
    /// <summary>
    /// ロールID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ロール名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 説明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 有効フラグ
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// ユーザー数
    /// </summary>
    public int UserCount { get; set; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// ロール検索DTO
/// </summary>
public class RoleSearchDto
{
    /// <summary>
    /// 検索キーワード（ロール名、説明）
    /// </summary>
    [StringLength(100, ErrorMessage = "検索キーワードは100文字以内で入力してください")]
    public string? Keyword { get; set; }

    /// <summary>
    /// 有効フラグでフィルタ
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// ページ番号
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "ページ番号は1以上の値を入力してください")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// ページサイズ
    /// </summary>
    [Range(1, 100, ErrorMessage = "ページサイズは1以上100以下の値を入力してください")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// ソートフィールド
    /// </summary>
    [StringLength(50, ErrorMessage = "ソートフィールドは50文字以内で入力してください")]
    public string? SortBy { get; set; } = "Name";

    /// <summary>
    /// ソート順（asc, desc）
    /// </summary>
    [StringLength(10, ErrorMessage = "ソート順は10文字以内で入力してください")]
    public string SortOrder { get; set; } = "asc";
}

/// <summary>
/// ロールステータス切り替えDTO
/// </summary>
public class ToggleRoleStatusDto
{
    /// <summary>
    /// 有効フラグ
    /// </summary>
    [Required(ErrorMessage = "有効フラグを指定してください")]
    public bool IsActive { get; set; }

    /// <summary>
    /// 変更理由
    /// </summary>
    [StringLength(200, ErrorMessage = "変更理由は200文字以内で入力してください")]
    public string? Reason { get; set; }
}

/// <summary>
/// ロール名重複チェックDTO
/// </summary>
public class RoleNameCheckDto
{
    /// <summary>
    /// ロール名
    /// </summary>
    [Required(ErrorMessage = "ロール名を入力してください")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "ロール名は2文字以上50文字以内で入力してください")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 除外するロールID（更新時に使用）
    /// </summary>
    public int? ExcludeRoleId { get; set; }
}
