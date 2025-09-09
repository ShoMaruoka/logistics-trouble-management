using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Services;

/// <summary>
/// パスワード管理サービスインターフェース
/// </summary>
public interface IPasswordManagementService
{
    /// <summary>
    /// パスワードを変更
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="changePasswordDto">パスワード変更データ</param>
    /// <returns>変更結果</returns>
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);

    /// <summary>
    /// パスワードをリセット（管理者用）
    /// </summary>
    /// <param name="resetPasswordDto">パスワードリセットデータ</param>
    /// <param name="resetByUserId">リセット実行者ID</param>
    /// <returns>リセット結果</returns>
    Task<bool> ResetPasswordAsync(AdminResetPasswordDto resetPasswordDto, int resetByUserId);

    /// <summary>
    /// パスワードの強度をチェック
    /// </summary>
    /// <param name="password">パスワード</param>
    /// <returns>強度チェック結果</returns>
    Task<PasswordStrengthResultDto> CheckPasswordStrengthAsync(string password);

    /// <summary>
    /// パスワードを検証
    /// </summary>
    /// <param name="password">パスワード</param>
    /// <param name="userId">ユーザーID（履歴チェック用）</param>
    /// <returns>検証結果</returns>
    Task<PasswordValidationResultDto> ValidatePasswordAsync(string password, int? userId = null);

    /// <summary>
    /// パスワード履歴を取得
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="page">ページ番号</param>
    /// <param name="pageSize">ページサイズ</param>
    /// <returns>パスワード履歴</returns>
    Task<PagedResultDto<PasswordHistoryDto>> GetPasswordHistoryAsync(int userId, int page = 1, int pageSize = 10);

    /// <summary>
    /// パスワード変更履歴を取得
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="page">ページ番号</param>
    /// <param name="pageSize">ページサイズ</param>
    /// <returns>パスワード変更履歴</returns>
    Task<PagedResultDto<PasswordChangeHistoryDto>> GetPasswordChangeHistoryAsync(int userId, int page = 1, int pageSize = 10);

    /// <summary>
    /// パスワードポリシーを取得
    /// </summary>
    /// <returns>パスワードポリシー</returns>
    Task<PasswordPolicyDto> GetPasswordPolicyAsync();

    /// <summary>
    /// パスワードポリシーを更新
    /// </summary>
    /// <param name="policyDto">パスワードポリシー</param>
    /// <returns>更新結果</returns>
    Task<bool> UpdatePasswordPolicyAsync(PasswordPolicyDto policyDto);

    /// <summary>
    /// パスワードが期限切れかチェック
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>期限切れ情報</returns>
    Task<object> CheckPasswordExpirationAsync(int userId);

    /// <summary>
    /// パスワード履歴に追加
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="passwordHash">パスワードハッシュ</param>
    /// <param name="changedBy">変更者ID</param>
    /// <param name="reason">変更理由</param>
    /// <returns>追加結果</returns>
    Task<bool> AddPasswordToHistoryAsync(int userId, string passwordHash, int changedBy, string? reason = null);

    /// <summary>
    /// 古いパスワード履歴を削除
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>削除件数</returns>
    Task<int> CleanupOldPasswordHistoryAsync(int userId);
}
