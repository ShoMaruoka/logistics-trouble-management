namespace LogisticsTroubleManagement.Core.Services;

/// <summary>
/// パスワードサービスインターフェース
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// パスワードのハッシュ化
    /// </summary>
    /// <param name="password">平文パスワード</param>
    /// <returns>ハッシュ化されたパスワード</returns>
    string HashPassword(string password);

    /// <summary>
    /// パスワードの検証
    /// </summary>
    /// <param name="password">平文パスワード</param>
    /// <param name="hashedPassword">ハッシュ化されたパスワード</param>
    /// <returns>検証結果</returns>
    bool VerifyPassword(string password, string hashedPassword);

    /// <summary>
    /// パスワードの強度チェック
    /// </summary>
    /// <param name="password">パスワード</param>
    /// <returns>強度レベル</returns>
    PasswordStrength CheckPasswordStrength(string password);

    /// <summary>
    /// パスワードの強度レベル
    /// </summary>
    public enum PasswordStrength
    {
        /// <summary>
        /// 弱い
        /// </summary>
        Weak = 0,

        /// <summary>
        /// 普通
        /// </summary>
        Medium = 1,

        /// <summary>
        /// 強い
        /// </summary>
        Strong = 2,

        /// <summary>
        /// 非常に強い
        /// </summary>
        VeryStrong = 3
    }
}
