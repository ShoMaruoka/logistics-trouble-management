using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.Core.DTOs;

/// <summary>
/// トークン無効化DTO
/// </summary>
public class RevokeTokenDto
{
    /// <summary>
    /// 無効化するトークン
    /// </summary>
    [Required(ErrorMessage = "トークンは必須です")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 無効化理由
    /// </summary>
    public string? Reason { get; set; }
}
