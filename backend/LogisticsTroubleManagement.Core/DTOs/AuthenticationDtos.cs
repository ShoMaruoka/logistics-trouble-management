using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.Core.DTOs;

/// <summary>
/// ログイン要求DTO
/// </summary>
public class LoginDto
{
    /// <summary>
    /// ユーザー名またはメールアドレス
    /// </summary>
    [Required(ErrorMessage = "ユーザー名またはメールアドレスを入力してください")]
    [StringLength(100, ErrorMessage = "ユーザー名またはメールアドレスは100文字以内で入力してください")]
    public string UsernameOrEmail { get; set; } = string.Empty;

    /// <summary>
    /// パスワード
    /// </summary>
    [Required(ErrorMessage = "パスワードを入力してください")]
    [StringLength(100, ErrorMessage = "パスワードは100文字以内で入力してください")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// リメンバーミー（ログイン状態を保持するか）
    /// </summary>
    public bool RememberMe { get; set; } = false;
}

/// <summary>
/// ログイン応答DTO
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// アクセストークン
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// リフレッシュトークン
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// アクセストークンの有効期限
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// ユーザー情報
    /// </summary>
    public UserDto User { get; set; } = new();
}

/// <summary>
    /// ユーザーDTO
    /// </summary>
public class UserDto
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
    /// ロール名
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// ロールID
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// 最終ログイン日時
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// 有効フラグ
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// トークンリフレッシュ要求DTO
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// リフレッシュトークン
    /// </summary>
    [Required(ErrorMessage = "リフレッシュトークンを入力してください")]
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// トークンリフレッシュ応答DTO
/// </summary>
public class RefreshTokenResponseDto
{
    /// <summary>
    /// 新しいアクセストークン
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 新しいリフレッシュトークン
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// アクセストークンの有効期限
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// パスワード変更要求DTO
/// </summary>
public class ChangePasswordDto
{
    /// <summary>
    /// 現在のパスワード
    /// </summary>
    [Required(ErrorMessage = "現在のパスワードを入力してください")]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新しいパスワード
    /// </summary>
    [Required(ErrorMessage = "新しいパスワードを入力してください")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "パスワードは8文字以上100文字以内で入力してください")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", 
        ErrorMessage = "パスワードは大文字・小文字・数字・記号を含む必要があります")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新しいパスワードの確認
    /// </summary>
    [Required(ErrorMessage = "新しいパスワードの確認を入力してください")]
    [Compare("NewPassword", ErrorMessage = "新しいパスワードと確認が一致しません")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// パスワードリセット要求DTO
/// </summary>
public class ForgotPasswordDto
{
    /// <summary>
    /// メールアドレス
    /// </summary>
    [Required(ErrorMessage = "メールアドレスを入力してください")]
    [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// パスワードリセットDTO
/// </summary>
public class ResetPasswordDto
{
    /// <summary>
    /// リセットトークン
    /// </summary>
    [Required(ErrorMessage = "リセットトークンを入力してください")]
    public string ResetToken { get; set; } = string.Empty;

    /// <summary>
    /// 新しいパスワード
    /// </summary>
    [Required(ErrorMessage = "新しいパスワードを入力してください")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "パスワードは8文字以上100文字以内で入力してください")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", 
        ErrorMessage = "パスワードは大文字・小文字・数字・記号を含む必要があります")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新しいパスワードの確認
    /// </summary>
    [Required(ErrorMessage = "新しいパスワードの確認を入力してください")]
    [Compare("NewPassword", ErrorMessage = "新しいパスワードと確認が一致しません")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// ログアウト要求DTO
/// </summary>
public class LogoutDto
{
    /// <summary>
    /// リフレッシュトークン
    /// </summary>
    [Required(ErrorMessage = "リフレッシュトークンを入力してください")]
    public string RefreshToken { get; set; } = string.Empty;
}
