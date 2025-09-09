using LogisticsTroubleManagement.Core.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LogisticsTroubleManagement.Core.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LogisticsTroubleManagement.Infrastructure.Services;

/// <summary>
/// JWTサービスの実装
/// </summary>
public class JwtService : IJwtService
{
    private readonly AuthenticationSettings _authSettings;
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<AuthenticationSettings> authSettings)
    {
        _authSettings = authSettings.Value;
        _jwtSettings = authSettings.Value.Jwt;
    }

    /// <summary>
    /// アクセストークンの生成
    /// </summary>
    /// <param name="claims">クレーム情報</param>
    /// <returns>アクセストークン</returns>
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials,
            notBefore: DateTime.UtcNow
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// リフレッシュトークンの生成
    /// </summary>
    /// <returns>リフレッシュトークン</returns>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// トークンからクレーム情報を取得
    /// </summary>
    /// <param name="token">トークン</param>
    /// <returns>クレーム情報</returns>
    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false, // 有効期限は手動でチェック
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                if (jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return principal;
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// トークンの有効期限を取得
    /// </summary>
    /// <param name="token">トークン</param>
    /// <returns>有効期限</returns>
    public DateTime? GetExpirationFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// トークンの有効性を検証
    /// </summary>
    /// <param name="token">トークン</param>
    /// <returns>有効性</returns>
    public bool ValidateToken(string token)
    {
        try
        {
            var principal = GetPrincipalFromToken(token);
            if (principal == null)
            {
                return false;
            }

            var expiration = GetExpirationFromToken(token);
            if (!expiration.HasValue)
            {
                return false;
            }

            // 有効期限チェック
            return expiration.Value > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// ユーザーIDからクレーム情報を生成
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="username">ユーザー名</param>
    /// <param name="role">ロール</param>
    /// <returns>クレーム情報</returns>
    public IEnumerable<Claim> GenerateClaims(int userId, string username, string role)
    {
        return new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim("sub", userId.ToString()),
            new Claim("jti", Guid.NewGuid().ToString()), // JWT ID
            new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64) // 発行時刻
        };
    }
}
