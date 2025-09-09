using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Repositories;
using LogisticsTroubleManagement.Domain.Services;
using Microsoft.Extensions.Logging;

namespace LogisticsTroubleManagement.Infrastructure.Services;

/// <summary>
/// マスタデータの解決を行うサービスの実装
/// </summary>
public class MasterDataResolverService : IMasterDataResolverService
{
    private readonly ITroubleTypeRepository _troubleTypeRepository;
    private readonly IDamageTypeRepository _damageTypeRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IShippingCompanyRepository _shippingCompanyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<MasterDataResolverService> _logger;

    public MasterDataResolverService(
        ITroubleTypeRepository troubleTypeRepository,
        IDamageTypeRepository damageTypeRepository,
        IWarehouseRepository warehouseRepository,
        IShippingCompanyRepository shippingCompanyRepository,
        IUserRepository userRepository,
        ILogger<MasterDataResolverService> logger)
    {
        _troubleTypeRepository = troubleTypeRepository;
        _damageTypeRepository = damageTypeRepository;
        _warehouseRepository = warehouseRepository;
        _shippingCompanyRepository = shippingCompanyRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// トラブルタイプの情報を取得
    /// </summary>
    public async Task<TroubleType?> GetTroubleTypeAsync(int id)
    {
        try
        {
            return await _troubleTypeRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "トラブルタイプの取得中にエラーが発生しました: ID={Id}", id);
            return null;
        }
    }

    /// <summary>
    /// ダメージタイプの情報を取得
    /// </summary>
    public async Task<DamageType?> GetDamageTypeAsync(int id)
    {
        try
        {
            _logger.LogInformation("ダメージタイプ取得開始: ID={Id}", id);
            
            // 直接DbSetを使用してテスト
            var result = await _damageTypeRepository.GetByIdAsync(id);
            
            if (result == null)
            {
                _logger.LogWarning("ダメージタイプが見つかりません: ID={Id}", id);
            }
            else
            {
                _logger.LogInformation("ダメージタイプ取得成功: ID={Id}, Name={Name}", id, result.Name);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ダメージタイプの取得中にエラーが発生しました: ID={Id}", id);
            return null;
        }
    }

    /// <summary>
    /// 倉庫の情報を取得
    /// </summary>
    public async Task<Warehouse?> GetWarehouseAsync(int id)
    {
        try
        {
            return await _warehouseRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "倉庫の取得中にエラーが発生しました: ID={Id}", id);
            return null;
        }
    }

    /// <summary>
    /// 配送会社の情報を取得
    /// </summary>
    public async Task<ShippingCompany?> GetShippingCompanyAsync(int id)
    {
        try
        {
            _logger.LogInformation("運送会社取得開始: ID={Id}", id);
            
            // 直接DbSetを使用してテスト
            var result = await _shippingCompanyRepository.GetByIdAsync(id);
            
            if (result == null)
            {
                _logger.LogWarning("運送会社が見つかりません: ID={Id}", id);
            }
            else
            {
                _logger.LogInformation("運送会社取得成功: ID={Id}, Name={Name}", id, result.Name);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "配送会社の取得中にエラーが発生しました: ID={Id}", id);
            return null;
        }
    }

    /// <summary>
    /// インシデントに関連するマスタデータを一括取得
    /// </summary>
    public async Task<(TroubleType? troubleType, DamageType? damageType, Warehouse? warehouse, ShippingCompany? shippingCompany)> 
        GetIncidentMasterDataAsync(int troubleTypeId, int damageTypeId, int warehouseId, int shippingCompanyId)
    {
        try
        {
            // 個別に取得（並行処理の問題を回避）
            var troubleType = await GetTroubleTypeAsync(troubleTypeId);
            var damageType = await GetDamageTypeAsync(damageTypeId);
            var warehouse = await GetWarehouseAsync(warehouseId);
            var shippingCompany = await GetShippingCompanyAsync(shippingCompanyId);

            return (troubleType, damageType, warehouse, shippingCompany);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "インシデントマスタデータの一括取得中にエラーが発生しました: TroubleTypeId={TroubleTypeId}, DamageTypeId={DamageTypeId}, WarehouseId={WarehouseId}, ShippingCompanyId={ShippingCompanyId}", 
                troubleTypeId, damageTypeId, warehouseId, shippingCompanyId);
            
            return (null, null, null, null);
        }
    }

    /// <summary>
    /// 複数のインシデントに関連するマスタデータを一括取得
    /// </summary>
    public async Task<IncidentMasterDataBatch> GetIncidentMasterDataBatchAsync(IEnumerable<Incident> incidents)
    {
        try
        {
            var batch = new IncidentMasterDataBatch();

            // ユニークなIDを収集
            var troubleTypeIds = incidents.Select(i => i.TroubleTypeId).Distinct().ToList();
            var damageTypeIds = incidents.Select(i => i.DamageTypeId).Distinct().ToList();
            var warehouseIds = incidents.Select(i => i.WarehouseId).Distinct().ToList();
            var shippingCompanyIds = incidents.Select(i => i.ShippingCompanyId).Distinct().ToList();
            var userIds = incidents.SelectMany(i => new[] { i.ReportedById }
                .Concat(i.AssignedToId.HasValue ? new[] { i.AssignedToId.Value } : Enumerable.Empty<int>()))
                .Distinct().ToList();

            _logger.LogInformation("マスタデータバッチ取得開始: TroubleTypeIds={TroubleTypeIds}, DamageTypeIds={DamageTypeIds}, WarehouseIds={WarehouseIds}, ShippingCompanyIds={ShippingCompanyIds}", 
                string.Join(",", troubleTypeIds), string.Join(",", damageTypeIds), string.Join(",", warehouseIds), string.Join(",", shippingCompanyIds));

            // 並行してマスタデータを取得
            var tasks = new List<Task>();

            if (troubleTypeIds.Any())
            {
                tasks.Add(GetTroubleTypesBatchAsync(troubleTypeIds, batch));
            }

            if (damageTypeIds.Any())
            {
                tasks.Add(GetDamageTypesBatchAsync(damageTypeIds, batch));
            }

            if (warehouseIds.Any())
            {
                tasks.Add(GetWarehousesBatchAsync(warehouseIds, batch));
            }

            if (shippingCompanyIds.Any())
            {
                tasks.Add(GetShippingCompaniesBatchAsync(shippingCompanyIds, batch));
            }

            if (userIds.Any())
            {
                tasks.Add(GetUsersBatchAsync(userIds, batch));
            }

            // すべてのタスクが完了するまで待機
            if (tasks.Any())
            {
                await Task.WhenAll(tasks);
            }

            _logger.LogInformation("マスタデータバッチ取得完了: TroubleTypes={TroubleTypesCount}, DamageTypes={DamageTypesCount}, Warehouses={WarehousesCount}, ShippingCompanies={ShippingCompaniesCount}", 
                batch.TroubleTypes.Count, batch.DamageTypes.Count, batch.Warehouses.Count, batch.ShippingCompanies.Count);

            return batch;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "インシデントマスタデータのバッチ取得中にエラーが発生しました");
            return new IncidentMasterDataBatch();
        }
    }

    private async Task GetTroubleTypesBatchAsync(List<int> ids, IncidentMasterDataBatch batch)
    {
        try
        {
            var troubleTypes = await _troubleTypeRepository.GetByIdsAsync(ids);
            foreach (var troubleType in troubleTypes)
            {
                if (troubleType != null)
                {
                    batch.TroubleTypes[troubleType.Id] = troubleType;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "トラブルタイプのバッチ取得中にエラーが発生しました: IDs={Ids}", string.Join(",", ids));
        }
    }

    private async Task GetDamageTypesBatchAsync(List<int> ids, IncidentMasterDataBatch batch)
    {
        try
        {
            _logger.LogInformation("ダメージタイプのバッチ取得開始: IDs={Ids}", string.Join(",", ids));
            var damageTypes = await _damageTypeRepository.GetByIdsAsync(ids);
            _logger.LogInformation("ダメージタイプ取得結果: {Count}件", damageTypes.Count());
            foreach (var damageType in damageTypes)
            {
                if (damageType != null)
                {
                    _logger.LogInformation("ダメージタイプ追加: ID={Id}, Name={Name}", damageType.Id, damageType.Name);
                    batch.DamageTypes[damageType.Id] = damageType;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ダメージタイプのバッチ取得中にエラーが発生しました: IDs={Ids}", string.Join(",", ids));
        }
    }

    private async Task GetWarehousesBatchAsync(List<int> ids, IncidentMasterDataBatch batch)
    {
        try
        {
            var warehouses = await _warehouseRepository.GetByIdsAsync(ids);
            foreach (var warehouse in warehouses)
            {
                if (warehouse != null)
                {
                    batch.Warehouses[warehouse.Id] = warehouse;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "倉庫のバッチ取得中にエラーが発生しました: IDs={Ids}", string.Join(",", ids));
        }
    }

    private async Task GetShippingCompaniesBatchAsync(List<int> ids, IncidentMasterDataBatch batch)
    {
        try
        {
            _logger.LogInformation("配送会社のバッチ取得開始: IDs={Ids}", string.Join(",", ids));
            var shippingCompanies = await _shippingCompanyRepository.GetByIdsAsync(ids);
            _logger.LogInformation("配送会社取得結果: {Count}件", shippingCompanies.Count());
            foreach (var shippingCompany in shippingCompanies)
            {
                if (shippingCompany != null)
                {
                    _logger.LogInformation("配送会社追加: ID={Id}, Name={Name}", shippingCompany.Id, shippingCompany.Name);
                    batch.ShippingCompanies[shippingCompany.Id] = shippingCompany;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "配送会社のバッチ取得中にエラーが発生しました: IDs={Ids}", string.Join(",", ids));
        }
    }

    private async Task GetUsersBatchAsync(List<int> ids, IncidentMasterDataBatch batch)
    {
        try
        {
            var users = await _userRepository.GetByIdsAsync(ids);
            foreach (var user in users)
            {
                if (user != null)
                {
                    batch.Users[user.Id] = user;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ユーザーのバッチ取得中にエラーが発生しました: IDs={Ids}", string.Join(",", ids));
        }
    }
}
