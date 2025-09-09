using LogisticsTroubleManagement.Core.Services;
using System.Security.Cryptography;
using System.Text;

namespace LogisticsTroubleManagement.Infrastructure.Services;

/// <summary>
/// パスワードハッシュ化サービスの実装
/// </summary>
public class PasswordService : IPasswordService
{
    /// <summary>
    /// パスワードのハッシュ化
    /// </summary>
    /// <param name="password">平文パスワード</param>
    /// <returns>ハッシュ化されたパスワード</returns>
    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("パスワードが空です", nameof(password));
        }

        // ソルトの生成（32バイト）
        byte[] salt = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // パスワードとソルトを結合してハッシュ化
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] combinedBytes = new byte[salt.Length + passwordBytes.Length];
        Buffer.BlockCopy(salt, 0, combinedBytes, 0, salt.Length);
        Buffer.BlockCopy(passwordBytes, 0, combinedBytes, salt.Length, passwordBytes.Length);

        // SHA256でハッシュ化
        byte[] hash;
        using (var sha256 = SHA256.Create())
        {
            hash = sha256.ComputeHash(combinedBytes);
        }

        // ソルトとハッシュをBase64エンコードして結合
        string saltString = Convert.ToBase64String(salt);
        string hashString = Convert.ToBase64String(hash);
        
        return $"{saltString}:{hashString}";
    }

    /// <summary>
    /// パスワードの検証
    /// </summary>
    /// <param name="password">平文パスワード</param>
    /// <param name="hashedPassword">ハッシュ化されたパスワード</param>
    /// <returns>検証結果</returns>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
        {
            return false;
        }

        try
        {
            // ソルトとハッシュを分離
            var parts = hashedPassword.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }

            string saltString = parts[0];
            string hashString = parts[1];

            // Base64デコード
            byte[] salt = Convert.FromBase64String(saltString);
            byte[] storedHash = Convert.FromBase64String(hashString);

            // 入力パスワードを同じ方法でハッシュ化
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] combinedBytes = new byte[salt.Length + passwordBytes.Length];
            Buffer.BlockCopy(salt, 0, combinedBytes, 0, salt.Length);
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, salt.Length, passwordBytes.Length);

            byte[] computedHash;
            using (var sha256 = SHA256.Create())
            {
                computedHash = sha256.ComputeHash(combinedBytes);
            }

            // ハッシュの比較（タイミング攻撃対策）
            return storedHash.SequenceEqual(computedHash);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// パスワードの強度チェック
    /// </summary>
    /// <param name="password">パスワード</param>
    /// <returns>強度レベル</returns>
    public IPasswordService.PasswordStrength CheckPasswordStrength(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return IPasswordService.PasswordStrength.Weak;
        }

        int score = 0;

        // 長さチェック
        if (password.Length >= 8) score++;
        if (password.Length >= 12) score++;

        // 文字種チェック
        if (password.Any(char.IsLower)) score++;
        if (password.Any(char.IsUpper)) score++;
        if (password.Any(char.IsDigit)) score++;
        if (password.Any(c => !char.IsLetterOrDigit(c))) score++;

        // 連続文字チェック（減点）
        if (HasConsecutiveChars(password)) score--;

        // スコアに基づく強度判定
        return score switch
        {
            0 or 1 => IPasswordService.PasswordStrength.Weak,
            2 or 3 => IPasswordService.PasswordStrength.Medium,
            4 or 5 => IPasswordService.PasswordStrength.Strong,
            _ => IPasswordService.PasswordStrength.VeryStrong
        };
    }

    /// <summary>
    /// 連続文字のチェック
    /// </summary>
    /// <param name="password">パスワード</param>
    /// <returns>連続文字が含まれているか</returns>
    private static bool HasConsecutiveChars(string password)
    {
        for (int i = 0; i < password.Length - 2; i++)
        {
            if (password[i] + 1 == password[i + 1] && password[i + 1] + 1 == password[i + 2])
            {
                return true;
            }
        }
        return false;
    }
}
