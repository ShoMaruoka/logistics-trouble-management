namespace LogisticsTroubleManagement.Core.Services;

/// <summary>
/// ワークフロー関連のバリデーションサービス
/// </summary>
public interface IWorkflowValidationService
{
    /// <summary>
    /// マスタデータの参照存在チェック
    /// </summary>
    /// <param name="troubleTypeId">トラブル種類ID</param>
    /// <param name="damageTypeId">損傷種類ID</param>
    /// <param name="warehouseId">出荷元倉庫ID</param>
    /// <param name="shippingCompanyId">運送会社ID</param>
    /// <returns>すべてのマスタデータが存在する場合はtrue</returns>
    Task<bool> ValidateMasterDataReferencesAsync(int troubleTypeId, int damageTypeId, 
        int warehouseId, int shippingCompanyId);

    /// <summary>
    /// ワークフロー遷移の妥当性チェック
    /// </summary>
    /// <param name="incidentId">インシデントID</param>
    /// <param name="targetAction">実行しようとするアクション</param>
    /// <param name="userRole">実行者のロール</param>
    /// <returns>遷移が可能な場合はtrue</returns>
    Task<bool> ValidateWorkflowTransitionAsync(int incidentId, string targetAction, string userRole);

    /// <summary>
    /// 分類データの妥当性チェック
    /// </summary>
    /// <param name="totalShipments">出荷総数</param>
    /// <param name="defectiveItems">不良品数</param>
    /// <returns>データが妥当な場合はtrue</returns>
    bool ValidateClassificationData(int totalShipments, int defectiveItems);

    /// <summary>
    /// 対応期限の妥当性チェック
    /// </summary>
    /// <param name="dueDate">対応期限</param>
    /// <param name="occurrenceDate">発生日（オプション）</param>
    /// <returns>期限が妥当な場合はtrue</returns>
    bool ValidateDueDate(DateTime dueDate, DateTime? occurrenceDate = null);

    /// <summary>
    /// ユーザーの権限チェック
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="requiredRole">必要なロール</param>
    /// <returns>権限がある場合はtrue</returns>
    Task<bool> ValidateUserPermissionAsync(int userId, string requiredRole);

    /// <summary>
    /// 倉庫担当者の担当倉庫チェック
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="warehouseId">倉庫ID</param>
    /// <returns>担当倉庫の場合はtrue</returns>
    Task<bool> ValidateWarehouseStaffAssignmentAsync(int userId, int warehouseId);
}
