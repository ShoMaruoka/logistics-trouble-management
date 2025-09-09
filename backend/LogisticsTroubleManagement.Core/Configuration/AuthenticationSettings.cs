namespace LogisticsTroubleManagement.Core.Configuration;

/// <summary>
/// 認証設定クラス
/// </summary>
public class AuthenticationSettings
{
    /// <summary>
    /// 認証の有効/無効フラグ
    /// </summary>
    public bool RequireAuth { get; set; } = false;

    /// <summary>
    /// JWT設定
    /// </summary>
    public JwtSettings Jwt { get; set; } = new();
}

/// <summary>
/// JWT設定クラス
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// 発行者
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// 対象者
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// 署名キー（環境変数またはSecret Managerで管理）
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// アクセストークンの有効期限（分）
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// リフレッシュトークンの有効期限（日）
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;

    /// <summary>
    /// キーローテーションの有効/無効フラグ
    /// </summary>
    public bool KeyRotationEnabled { get; set; } = true;

    /// <summary>
    /// キーローテーション間隔（日）
    /// </summary>
    public int KeyRotationIntervalDays { get; set; } = 90;
}
