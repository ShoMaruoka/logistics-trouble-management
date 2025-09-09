using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.Core.DTOs;

/// <summary>
/// パスワードリセット用DTO（管理者用）
/// </summary>
public class AdminResetPasswordDto
{
    /// <summary>
    /// ユーザーID
    /// </summary>
    [Required(ErrorMessage = "ユーザーIDを入力してください")]
    [Range(1, int.MaxValue, ErrorMessage = "有効なユーザーIDを入力してください")]
    public int UserId { get; set; }

    /// <summary>
    /// 新しいパスワード
    /// </summary>
    [Required(ErrorMessage = "新しいパスワードを入力してください")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "パスワードは8文字以上100文字以内で入力してください")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$",
        ErrorMessage = "パスワードは大文字・小文字・数字・記号を含む必要があります")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新しいパスワード（確認用）
    /// </summary>
    [Required(ErrorMessage = "新しいパスワード（確認用）を入力してください")]
    [Compare(nameof(NewPassword), ErrorMessage = "新しいパスワードと確認用パスワードが一致しません")]
    public string ConfirmNewPassword { get; set; } = string.Empty;

    /// <summary>
    /// リセット理由
    /// </summary>
    [StringLength(200, ErrorMessage = "リセット理由は200文字以内で入力してください")]
    public string? Reason { get; set; }
}

/// <summary>
/// パスワード強度チェック用DTO
/// </summary>
public class PasswordStrengthCheckDto
{
    /// <summary>
    /// パスワード
    /// </summary>
    [Required(ErrorMessage = "パスワードを入力してください")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// パスワード強度チェック結果DTO
/// </summary>
public class PasswordStrengthResultDto
{
    /// <summary>
    /// 強度レベル
    /// </summary>
    public string Strength { get; set; } = string.Empty;

    /// <summary>
    /// 強度スコア（0-5）
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// 強度の説明
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 改善提案
    /// </summary>
    public List<string> Suggestions { get; set; } = new List<string>();

    /// <summary>
    /// 有効性
    /// </summary>
    public bool IsValid { get; set; }
}

/// <summary>
/// パスワード履歴DTO
/// </summary>
public class PasswordHistoryDto
{
    /// <summary>
    /// パスワードID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ユーザーID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// パスワードハッシュ
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 使用終了日時
    /// </summary>
    public DateTime? ExpiredAt { get; set; }
}

/// <summary>
/// パスワードポリシーDTO
/// </summary>
public class PasswordPolicyDto
{
    /// <summary>
    /// 最小文字数
    /// </summary>
    public int MinLength { get; set; } = 8;

    /// <summary>
    /// 最大文字数
    /// </summary>
    public int MaxLength { get; set; } = 100;

    /// <summary>
    /// 大文字を含む必要があるか
    /// </summary>
    public bool RequireUppercase { get; set; } = true;

    /// <summary>
    /// 小文字を含む必要があるか
    /// </summary>
    public bool RequireLowercase { get; set; } = true;

    /// <summary>
    /// 数字を含む必要があるか
    /// </summary>
    public bool RequireDigit { get; set; } = true;

    /// <summary>
    /// 記号を含む必要があるか
    /// </summary>
    public bool RequireSpecialChar { get; set; } = true;

    /// <summary>
    /// 履歴保持数（同じパスワードの再利用防止）
    /// </summary>
    public int HistoryCount { get; set; } = 5;

    /// <summary>
    /// パスワード有効期限（日数）
    /// </summary>
    public int? ExpirationDays { get; set; }

    /// <summary>
    /// 最小強度レベル
    /// </summary>
    public string MinStrength { get; set; } = "Medium";
}

/// <summary>
/// パスワード変更履歴DTO
/// </summary>
public class PasswordChangeHistoryDto
{
    /// <summary>
    /// 変更日時
    /// </summary>
    public DateTime ChangedAt { get; set; }

    /// <summary>
    /// 変更者ID
    /// </summary>
    public int ChangedBy { get; set; }

    /// <summary>
    /// 変更者名
    /// </summary>
    public string ChangedByName { get; set; } = string.Empty;

    /// <summary>
    /// 変更理由
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// 変更タイプ（自己変更、管理者リセット）
    /// </summary>
    public string ChangeType { get; set; } = string.Empty;
}

/// <summary>
/// パスワード検証結果DTO
/// </summary>
public class PasswordValidationResultDto
{
    /// <summary>
    /// 有効性
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// エラーメッセージ
    /// </summary>
    public List<string> Errors { get; set; } = new List<string>();

    /// <summary>
    /// 警告メッセージ
    /// </summary>
    public List<string> Warnings { get; set; } = new List<string>();

    /// <summary>
    /// 強度レベル
    /// </summary>
    public string Strength { get; set; } = string.Empty;
}
