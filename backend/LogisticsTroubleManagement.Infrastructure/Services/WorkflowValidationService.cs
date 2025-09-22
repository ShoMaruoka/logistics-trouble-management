using LogisticsTroubleManagement.Core.Services;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace LogisticsTroubleManagement.Infrastructure.Services;

/// <summary>
/// ワークフロー関連のバリデーションサービス実装
/// </summary>
public class WorkflowValidationService : IWorkflowValidationService
{
    private readonly ITroubleTypeRepository _troubleTypeRepository;
    private readonly IDamageTypeRepository _damageTypeRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IShippingCompanyRepository _shippingCompanyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IIncidentRepository _incidentRepository;
    private readonly ILogger<WorkflowValidationService> _logger;

    public WorkflowValidationService(
        ITroubleTypeRepository troubleTypeRepository,
        IDamageTypeRepository damageTypeRepository,
        IWarehouseRepository warehouseRepository,
        IShippingCompanyRepository shippingCompanyRepository,
        IUserRepository userRepository,
        IIncidentRepository incidentRepository,
        ILogger<WorkflowValidationService> logger)
    {
        _troubleTypeRepository = troubleTypeRepository;
        _damageTypeRepository = damageTypeRepository;
        _warehouseRepository = warehouseRepository;
        _shippingCompanyRepository = shippingCompanyRepository;
        _userRepository = userRepository;
        _incidentRepository = incidentRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> ValidateMasterDataReferencesAsync(int troubleTypeId, int damageTypeId, 
        int warehouseId, int shippingCompanyId)
    {
        try
        {
            _logger.LogDebug("マスタデータ参照チェック開始: TroubleType={TroubleTypeId}, DamageType={DamageTypeId}, Warehouse={WarehouseId}, ShippingCompany={ShippingCompanyId}",
                troubleTypeId, damageTypeId, warehouseId, shippingCompanyId);

            // 並行してマスタデータの存在チェックを実行
            var troubleTypeTask = _troubleTypeRepository.GetByIdAsync(troubleTypeId);
            var damageTypeTask = _damageTypeRepository.GetByIdAsync(damageTypeId);
            var warehouseTask = _warehouseRepository.GetByIdAsync(warehouseId);
            var shippingCompanyTask = _shippingCompanyRepository.GetByIdAsync(shippingCompanyId);

            await Task.WhenAll(troubleTypeTask, damageTypeTask, warehouseTask, shippingCompanyTask);

            var troubleType = await troubleTypeTask;
            var damageType = await damageTypeTask;
            var warehouse = await warehouseTask;
            var shippingCompany = await shippingCompanyTask;

            var isValid = troubleType != null && damageType != null && 
                         warehouse != null && shippingCompany != null;

            if (!isValid)
            {
                _logger.LogWarning("マスタデータ参照チェック失敗: TroubleType={TroubleTypeExists}, DamageType={DamageTypeExists}, Warehouse={WarehouseExists}, ShippingCompany={ShippingCompanyExists}",
                    troubleType != null, damageType != null, warehouse != null, shippingCompany != null);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "マスタデータ参照チェック中にエラーが発生しました");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ValidateWorkflowTransitionAsync(int incidentId, string targetAction, string userRole)
    {
        try
        {
            _logger.LogDebug("ワークフロー遷移チェック開始: IncidentId={IncidentId}, Action={Action}, Role={Role}",
                incidentId, targetAction, userRole);

            var incident = await _incidentRepository.GetByIdAsync(incidentId);
            if (incident == null)
            {
                _logger.LogWarning("インシデントが見つかりません: ID={IncidentId}", incidentId);
                return false;
            }

            // ワークフローが有効でない場合は有効化が必要
            // 新仕様では常に新ワークフロー（WorkflowEnabled削除）

            // 利用可能なアクションを取得
            var availableActions = incident.GetAvailableWorkflowActions(userRole);
            var isValid = availableActions.Contains(targetAction);

            if (!isValid)
            {
                _logger.LogWarning("ワークフロー遷移が許可されていません: IncidentId={IncidentId}, CurrentStatus={Status}, Action={Action}, Role={Role}, AvailableActions={AvailableActions}",
                    incidentId, incident.Status, targetAction, userRole, string.Join(",", availableActions));
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ワークフロー遷移チェック中にエラーが発生しました: IncidentId={IncidentId}", incidentId);
            return false;
        }
    }

    /// <inheritdoc />
    public bool ValidateClassificationData(int totalShipments, int defectiveItems)
    {
        try
        {
            // 基本的なデータ妥当性チェック
            if (totalShipments < 0 || defectiveItems < 0)
            {
                _logger.LogWarning("負の値が指定されました: TotalShipments={TotalShipments}, DefectiveItems={DefectiveItems}",
                    totalShipments, defectiveItems);
                return false;
            }

            // 不良品数が出荷総数を超えていないかチェック
            if (defectiveItems > totalShipments)
            {
                _logger.LogWarning("不良品数が出荷総数を超えています: TotalShipments={TotalShipments}, DefectiveItems={DefectiveItems}",
                    totalShipments, defectiveItems);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分類データバリデーション中にエラーが発生しました");
            return false;
        }
    }

    /// <inheritdoc />
    public bool ValidateDueDate(DateTime dueDate, DateTime? occurrenceDate = null)
    {
        try
        {
            var now = DateTime.UtcNow;
            
            // 過去の日付は無効
            if (dueDate <= now)
            {
                _logger.LogWarning("対応期限が過去の日付です: DueDate={DueDate}", dueDate);
                return false;
            }

            // 発生日が指定されている場合、発生日より後でなければならない
            if (occurrenceDate.HasValue && dueDate <= occurrenceDate.Value)
            {
                _logger.LogWarning("対応期限が発生日より前です: DueDate={DueDate}, OccurrenceDate={OccurrenceDate}",
                    dueDate, occurrenceDate.Value);
                return false;
            }

            // あまりに遠い未来（1年以上先）は警告
            if (dueDate > now.AddYears(1))
            {
                _logger.LogWarning("対応期限が1年以上先に設定されています: DueDate={DueDate}", dueDate);
                // 警告だが有効とする
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "対応期限バリデーション中にエラーが発生しました");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ValidateUserPermissionAsync(int userId, string requiredRole)
    {
        try
        {
            _logger.LogDebug("ユーザー権限チェック開始: UserId={UserId}, RequiredRole={RequiredRole}",
                userId, requiredRole);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("ユーザーが見つかりません: ID={UserId}", userId);
                return false;
            }

            // Adminは全権限を持つ
            if (user.Role?.Name == "Admin")
            {
                return true;
            }

            // 必要なロールと一致するかチェック
            var hasPermission = user.Role?.Name == requiredRole;

            if (!hasPermission)
            {
                _logger.LogWarning("ユーザーに必要な権限がありません: UserId={UserId}, UserRole={UserRole}, RequiredRole={RequiredRole}",
                    userId, user.Role?.Name, requiredRole);
            }

            return hasPermission;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザー権限チェック中にエラーが発生しました: UserId={UserId}", userId);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ValidateWarehouseStaffAssignmentAsync(int userId, int warehouseId)
    {
        try
        {
            _logger.LogDebug("倉庫担当者チェック開始: UserId={UserId}, WarehouseId={WarehouseId}",
                userId, warehouseId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("ユーザーが見つかりません: ID={UserId}", userId);
                return false;
            }

            // Adminは全倉庫にアクセス可能
            if (user.Role?.Name == "Admin")
            {
                return true;
            }

            // 倉庫担当者でない場合は無条件で許可（他のロールは倉庫制限なし）
            if (user.Role?.Name != "Warehouse Staff")
            {
                return true;
            }

            // 倉庫担当者の場合、担当倉庫のチェック
            var isAssigned = user.WarehouseId == warehouseId;

            if (!isAssigned)
            {
                _logger.LogWarning("倉庫担当者が担当外の倉庫にアクセスしようとしました: UserId={UserId}, UserWarehouse={UserWarehouse}, RequestedWarehouse={RequestedWarehouse}",
                    userId, user.WarehouseId, warehouseId);
            }

            return isAssigned;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "倉庫担当者チェック中にエラーが発生しました: UserId={UserId}", userId);
            return false;
        }
    }
}
