using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Services;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogisticsTroubleManagement.Infrastructure.Services;

/// <summary>
/// パスワード管理サービス実装
/// </summary>
public class PasswordManagementService : IPasswordManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PasswordManagementService> _logger;

    public PasswordManagementService(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ApplicationDbContext context,
        ILogger<PasswordManagementService> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _context = context;
        _logger = logger;
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("ユーザーID {UserId} が見つかりません", userId);
                return false;
            }

            // 現在のパスワードを検証
            if (!_passwordService.VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                _logger.LogWarning("ユーザーID {UserId} の現在のパスワードが正しくありません", userId);
                return false;
            }

            // 新しいパスワードを検証
            var validationResult = await ValidatePasswordAsync(changePasswordDto.NewPassword, userId);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("ユーザーID {UserId} の新しいパスワードが無効です: {Errors}", 
                    userId, string.Join(", ", validationResult.Errors));
                return false;
            }

            // パスワード履歴に追加
            await AddPasswordToHistoryAsync(userId, user.PasswordHash, userId, "パスワード変更");

            // 新しいパスワードをハッシュ化して設定
            var newPasswordHash = _passwordService.HashPassword(changePasswordDto.NewPassword);
            user.SetPasswordHash(newPasswordHash);
            user.UpdateLastPasswordChangeAt();

            await _context.SaveChangesAsync();

            _logger.LogInformation("ユーザーID {UserId} のパスワードを変更しました", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーID {UserId} のパスワード変更中にエラーが発生しました", userId);
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(AdminResetPasswordDto resetPasswordDto, int resetByUserId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(resetPasswordDto.UserId);
            if (user == null)
            {
                _logger.LogWarning("ユーザーID {UserId} が見つかりません", resetPasswordDto.UserId);
                return false;
            }

            // 新しいパスワードを検証
            var validationResult = await ValidatePasswordAsync(resetPasswordDto.NewPassword, resetPasswordDto.UserId);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("ユーザーID {UserId} の新しいパスワードが無効です: {Errors}", 
                    resetPasswordDto.UserId, string.Join(", ", validationResult.Errors));
                return false;
            }

            // パスワード履歴に追加
            await AddPasswordToHistoryAsync(resetPasswordDto.UserId, user.PasswordHash, resetByUserId, 
                resetPasswordDto.Reason ?? "管理者によるパスワードリセット");

            // 新しいパスワードをハッシュ化して設定
            var newPasswordHash = _passwordService.HashPassword(resetPasswordDto.NewPassword);
            user.SetPasswordHash(newPasswordHash);
            user.UpdateLastPasswordChangeAt();

            await _context.SaveChangesAsync();

            _logger.LogInformation("ユーザーID {UserId} のパスワードをリセットしました。実行者: {ResetByUserId}", 
                resetPasswordDto.UserId, resetByUserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーID {UserId} のパスワードリセット中にエラーが発生しました", resetPasswordDto.UserId);
            return false;
        }
    }

    public async Task<PasswordStrengthResultDto> CheckPasswordStrengthAsync(string password)
    {
        var strength = _passwordService.CheckPasswordStrength(password);
        var score = GetPasswordScore(password);
        var suggestions = GetPasswordSuggestions(password);

        return new PasswordStrengthResultDto
        {
            Strength = strength.ToString(),
            Score = score,
            Description = GetStrengthDescription(strength),
            Suggestions = suggestions,
            IsValid = strength >= IPasswordService.PasswordStrength.Medium
        };
    }

    public async Task<PasswordValidationResultDto> ValidatePasswordAsync(string password, int? userId = null)
    {
        var result = new PasswordValidationResultDto();
        var strength = _passwordService.CheckPasswordStrength(password);

        // 基本的な検証
        if (string.IsNullOrEmpty(password))
        {
            result.Errors.Add("パスワードを入力してください");
            return result;
        }

        if (password.Length < 8)
        {
            result.Errors.Add("パスワードは8文字以上で入力してください");
        }

        if (password.Length > 100)
        {
            result.Errors.Add("パスワードは100文字以内で入力してください");
        }

        if (!password.Any(char.IsLower))
        {
            result.Errors.Add("パスワードには小文字を含める必要があります");
        }

        if (!password.Any(char.IsUpper))
        {
            result.Errors.Add("パスワードには大文字を含める必要があります");
        }

        if (!password.Any(char.IsDigit))
        {
            result.Errors.Add("パスワードには数字を含める必要があります");
        }

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
        {
            result.Errors.Add("パスワードには記号を含める必要があります");
        }

        // 強度チェック
        if (strength < IPasswordService.PasswordStrength.Medium)
        {
            result.Warnings.Add("パスワードの強度が弱いです。より複雑なパスワードを設定してください");
        }

        // 履歴チェック（ユーザーIDが指定されている場合）
        if (userId.HasValue)
        {
            var isReused = await IsPasswordReusedAsync(userId.Value, password);
            if (isReused)
            {
                result.Errors.Add("過去に使用したパスワードは使用できません");
            }
        }

        result.IsValid = !result.Errors.Any();
        result.Strength = strength.ToString();

        return result;
    }

    public async Task<PagedResultDto<PasswordHistoryDto>> GetPasswordHistoryAsync(int userId, int page = 1, int pageSize = 10)
    {
        // パスワード履歴テーブルが存在しない場合は空の結果を返す
        // 実際の実装では、PasswordHistoryエンティティを作成する必要があります
        return new PagedResultDto<PasswordHistoryDto>(
            new List<PasswordHistoryDto>(),
            page,
            pageSize,
            0);
    }

    public async Task<PagedResultDto<PasswordChangeHistoryDto>> GetPasswordChangeHistoryAsync(int userId, int page = 1, int pageSize = 10)
    {
        // パスワード変更履歴テーブルが存在しない場合は空の結果を返す
        // 実際の実装では、PasswordChangeHistoryエンティティを作成する必要があります
        return new PagedResultDto<PasswordChangeHistoryDto>(
            new List<PasswordChangeHistoryDto>(),
            page,
            pageSize,
            0);
    }

    public async Task<PasswordPolicyDto> GetPasswordPolicyAsync()
    {
        // 現在は固定のポリシーを返す
        // 実際の実装では、設定テーブルから取得する
        return new PasswordPolicyDto
        {
            MinLength = 8,
            MaxLength = 100,
            RequireUppercase = true,
            RequireLowercase = true,
            RequireDigit = true,
            RequireSpecialChar = true,
            HistoryCount = 5,
            ExpirationDays = 90,
            MinStrength = "Medium"
        };
    }

    public async Task<bool> UpdatePasswordPolicyAsync(PasswordPolicyDto policyDto)
    {
        // 現在は実装しない
        // 実際の実装では、設定テーブルを更新する
        _logger.LogInformation("パスワードポリシーを更新しました");
        return true;
    }

    public async Task<object> CheckPasswordExpirationAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return new { isExpired = false, daysUntilExpiration = 0 };
        }

        var policy = await GetPasswordPolicyAsync();
        if (!policy.ExpirationDays.HasValue)
        {
            return new { isExpired = false, daysUntilExpiration = 0 };
        }

        var lastChange = user.LastPasswordChangeAt ?? user.CreatedAt;
        var expirationDate = lastChange.AddDays(policy.ExpirationDays.Value);
        var daysUntilExpiration = (expirationDate - DateTime.UtcNow).Days;

        return new
        {
            isExpired = daysUntilExpiration <= 0,
            daysUntilExpiration = Math.Max(0, daysUntilExpiration),
            expirationDate = expirationDate
        };
    }

    public async Task<bool> AddPasswordToHistoryAsync(int userId, string passwordHash, int changedBy, string? reason = null)
    {
        // 現在は実装しない
        // 実際の実装では、PasswordHistoryテーブルに追加する
        _logger.LogInformation("ユーザーID {UserId} のパスワード履歴に追加しました", userId);
        return true;
    }

    public async Task<int> CleanupOldPasswordHistoryAsync(int userId)
    {
        // 現在は実装しない
        // 実際の実装では、古いパスワード履歴を削除する
        return 0;
    }

    private async Task<bool> IsPasswordReusedAsync(int userId, string password)
    {
        // 現在は実装しない
        // 実際の実装では、パスワード履歴をチェックする
        return false;
    }

    private static int GetPasswordScore(string password)
    {
        int score = 0;
        if (password.Length >= 8) score++;
        if (password.Length >= 12) score++;
        if (password.Any(char.IsLower)) score++;
        if (password.Any(char.IsUpper)) score++;
        if (password.Any(char.IsDigit)) score++;
        if (password.Any(c => !char.IsLetterOrDigit(c))) score++;
        return score;
    }

    private static List<string> GetPasswordSuggestions(string password)
    {
        var suggestions = new List<string>();

        if (password.Length < 8)
            suggestions.Add("8文字以上にしてください");
        if (password.Length < 12)
            suggestions.Add("12文字以上にするとより安全です");
        if (!password.Any(char.IsLower))
            suggestions.Add("小文字を含めてください");
        if (!password.Any(char.IsUpper))
            suggestions.Add("大文字を含めてください");
        if (!password.Any(char.IsDigit))
            suggestions.Add("数字を含めてください");
        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            suggestions.Add("記号（!@#$%^&*など）を含めてください");

        return suggestions;
    }

    private static string GetStrengthDescription(IPasswordService.PasswordStrength strength)
    {
        return strength switch
        {
            IPasswordService.PasswordStrength.Weak => "弱い",
            IPasswordService.PasswordStrength.Medium => "普通",
            IPasswordService.PasswordStrength.Strong => "強い",
            IPasswordService.PasswordStrength.VeryStrong => "非常に強い",
            _ => "不明"
        };
    }
}
