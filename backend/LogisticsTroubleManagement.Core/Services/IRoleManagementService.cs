using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Services;

/// <summary>
/// ロール管理サービスインターフェース
/// </summary>
public interface IRoleManagementService
{
    /// <summary>
    /// ロール一覧を取得
    /// </summary>
    /// <param name="searchDto">検索条件</param>
    /// <returns>ページングされたロール一覧</returns>
    Task<PagedResultDto<RoleListDto>> GetRolesAsync(RoleSearchDto searchDto);

    /// <summary>
    /// ロール詳細を取得
    /// </summary>
    /// <param name="id">ロールID</param>
    /// <returns>ロール詳細</returns>
    Task<RoleDetailDto?> GetRoleByIdAsync(int id);

    /// <summary>
    /// ロールを作成
    /// </summary>
    /// <param name="createDto">作成データ</param>
    /// <returns>作成されたロール</returns>
    Task<RoleDetailDto> CreateRoleAsync(CreateRoleDto createDto);

    /// <summary>
    /// ロールを更新
    /// </summary>
    /// <param name="id">ロールID</param>
    /// <param name="updateDto">更新データ</param>
    /// <returns>更新されたロール</returns>
    Task<RoleDetailDto> UpdateRoleAsync(int id, UpdateRoleDto updateDto);

    /// <summary>
    /// ロールを削除
    /// </summary>
    /// <param name="id">ロールID</param>
    /// <returns>削除成功フラグ</returns>
    Task<bool> DeleteRoleAsync(int id);

    /// <summary>
    /// ロールのステータスを切り替え
    /// </summary>
    /// <param name="id">ロールID</param>
    /// <param name="toggleDto">ステータス切り替えデータ</param>
    /// <returns>更新されたロール</returns>
    Task<RoleDetailDto> ToggleRoleStatusAsync(int id, ToggleRoleStatusDto toggleDto);

    /// <summary>
    /// ロール名の重複チェック
    /// </summary>
    /// <param name="name">ロール名</param>
    /// <param name="excludeRoleId">除外するロールID</param>
    /// <returns>重複フラグ</returns>
    Task<bool> IsRoleNameExistsAsync(string name, int? excludeRoleId = null);

    /// <summary>
    /// アクティブロール一覧を取得
    /// </summary>
    /// <returns>アクティブロール一覧</returns>
    Task<IEnumerable<RoleListDto>> GetActiveRolesAsync();

    /// <summary>
    /// ロールが使用中かチェック
    /// </summary>
    /// <param name="id">ロールID</param>
    /// <returns>使用中フラグ</returns>
    Task<bool> IsRoleInUseAsync(int id);
}
