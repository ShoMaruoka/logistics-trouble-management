using LogisticsTroubleManagement.Core.DTOs;

namespace LogisticsTroubleManagement.Core.Services;

/// <summary>
/// 倉庫担当専用サービスインターフェース
/// </summary>
public interface IWarehouseStaffService
{
    /// <summary>
    /// 倉庫担当ダッシュボードの統計情報を取得
    /// </summary>
    Task<WarehouseStaffDashboardDto> GetDashboardAsync(int userId);

    /// <summary>
    /// 担当倉庫のインシデント一覧を取得
    /// </summary>
    Task<PagedResultDto<WarehouseStaffIncidentDto>> GetAssignedWarehouseIncidentsAsync(
        int userId, WarehouseStaffIncidentSearchDto searchDto);

    /// <summary>
    /// 分類済みインシデント一覧を取得
    /// </summary>
    Task<PagedResultDto<WarehouseStaffIncidentDto>> GetClassifiedIncidentsAsync(
        int userId, WarehouseStaffIncidentSearchDto searchDto);

    /// <summary>
    /// 未解決インシデント一覧を取得
    /// </summary>
    Task<PagedResultDto<WarehouseStaffIncidentDto>> GetUnresolvedIncidentsAsync(
        int userId, WarehouseStaffIncidentSearchDto searchDto);

    /// <summary>
    /// 対応中インシデント一覧を取得
    /// </summary>
    Task<PagedResultDto<WarehouseStaffIncidentDto>> GetInProgressIncidentsAsync(
        int userId, WarehouseStaffIncidentSearchDto searchDto);

    /// <summary>
    /// インシデントを分類する
    /// </summary>
    Task<bool> ClassifyIncidentAsync(int userId, ClassifyIncidentDto classifyDto);

    /// <summary>
    /// インシデントを更新する（倉庫担当権限内）
    /// </summary>
    Task<bool> UpdateIncidentAsync(int userId, int incidentId, WarehouseStaffUpdateIncidentDto updateDto);

    /// <summary>
    /// ユーザーが倉庫担当かどうかを確認
    /// </summary>
    Task<bool> IsWarehouseStaffAsync(int userId);

    /// <summary>
    /// ユーザーの担当倉庫IDを取得
    /// </summary>
    Task<int?> GetUserWarehouseIdAsync(int userId);

    /// <summary>
    /// ユーザーが指定された倉庫にアクセス権限があるか確認
    /// </summary>
    Task<bool> HasWarehouseAccessAsync(int userId, int warehouseId);
}
