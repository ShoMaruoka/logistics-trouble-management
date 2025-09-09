using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Services;

/// <summary>
/// ユーザー管理サービスインターフェース
/// </summary>
public interface IUserManagementService
{
    /// <summary>
    /// ユーザー一覧を取得する
    /// </summary>
    /// <param name="searchDto">検索条件</param>
    /// <returns>ユーザー一覧とページング情報</returns>
    Task<PagedResultDto<UserListDto>> GetUsersAsync(UserSearchDto searchDto);

    /// <summary>
    /// ユーザー詳細を取得する
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>ユーザー詳細情報</returns>
    Task<UserDetailDto?> GetUserByIdAsync(int userId);

    /// <summary>
    /// ユーザーを作成する
    /// </summary>
    /// <param name="createDto">ユーザー作成情報</param>
    /// <returns>作成されたユーザー情報</returns>
    Task<UserDetailDto> CreateUserAsync(CreateUserDto createDto);

    /// <summary>
    /// ユーザーを更新する
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="updateDto">ユーザー更新情報</param>
    /// <returns>更新されたユーザー情報</returns>
    Task<UserDetailDto> UpdateUserAsync(int userId, UpdateUserDto updateDto);

    /// <summary>
    /// ユーザーを削除する
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <returns>削除に成功したかどうか</returns>
    Task<bool> DeleteUserAsync(int userId);

    /// <summary>
    /// ユーザーの有効/無効を切り替える
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="toggleDto">有効/無効切り替え情報</param>
    /// <returns>更新されたユーザー情報</returns>
    Task<UserDetailDto> ToggleUserStatusAsync(int userId, ToggleUserStatusDto toggleDto);

    /// <summary>
    /// ユーザー名の重複チェック
    /// </summary>
    /// <param name="username">ユーザー名</param>
    /// <param name="excludeUserId">除外するユーザーID（更新時）</param>
    /// <returns>重複しているかどうか</returns>
    Task<bool> IsUsernameExistsAsync(string username, int? excludeUserId = null);

    /// <summary>
    /// メールアドレスの重複チェック
    /// </summary>
    /// <param name="email">メールアドレス</param>
    /// <param name="excludeUserId">除外するユーザーID（更新時）</param>
    /// <returns>重複しているかどうか</returns>
    Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null);

    /// <summary>
    /// ユーザーのパスワードをリセットする
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="newPassword">新しいパスワード</param>
    /// <returns>リセットに成功したかどうか</returns>
    Task<bool> ResetUserPasswordAsync(int userId, string newPassword);

    /// <summary>
    /// ユーザーのログイン履歴を取得する
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="page">ページ番号</param>
    /// <param name="pageSize">ページサイズ</param>
    /// <returns>ログイン履歴</returns>
    Task<PagedResultDto<object>> GetUserLoginHistoryAsync(int userId, int page = 1, int pageSize = 10);
}
