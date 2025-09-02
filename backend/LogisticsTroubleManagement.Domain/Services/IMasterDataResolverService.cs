using LogisticsTroubleManagement.Domain.Entities;

namespace LogisticsTroubleManagement.Domain.Services;

/// <summary>
/// マスタデータの解決を行うサービスのインターフェース
/// </summary>
public interface IMasterDataResolverService
{
    /// <summary>
    /// トラブルタイプの情報を取得
    /// </summary>
    /// <param name="id">トラブルタイプID</param>
    /// <returns>トラブルタイプエンティティ</returns>
    Task<TroubleType?> GetTroubleTypeAsync(int id);

    /// <summary>
    /// ダメージタイプの情報を取得
    /// </summary>
    /// <param name="id">ダメージタイプID</param>
    /// <returns>ダメージタイプエンティティ</returns>
    Task<DamageType?> GetDamageTypeAsync(int id);

    /// <summary>
    /// 倉庫の情報を取得
    /// </summary>
    /// <param name="id">倉庫ID</param>
    /// <returns>倉庫エンティティ</returns>
    Task<Warehouse?> GetWarehouseAsync(int id);

    /// <summary>
    /// 配送会社の情報を取得
    /// </summary>
    /// <param name="id">配送会社ID</param>
    /// <returns>配送会社エンティティ</returns>
    Task<ShippingCompany?> GetShippingCompanyAsync(int id);

    /// <summary>
    /// インシデントに関連するマスタデータを一括取得
    /// </summary>
    /// <param name="troubleTypeId">トラブルタイプID</param>
    /// <param name="damageTypeId">ダメージタイプID</param>
    /// <param name="warehouseId">倉庫ID</param>
    /// <param name="shippingCompanyId">配送会社ID</param>
    /// <returns>マスタデータのタプル</returns>
    Task<(TroubleType? troubleType, DamageType? damageType, Warehouse? warehouse, ShippingCompany? shippingCompany)> 
        GetIncidentMasterDataAsync(int troubleTypeId, int damageTypeId, int warehouseId, int shippingCompanyId);

    /// <summary>
    /// 複数のインシデントに関連するマスタデータを一括取得
    /// </summary>
    /// <param name="incidents">インシデントのコレクション</param>
    /// <returns>マスタデータの辞書</returns>
    Task<IncidentMasterDataBatch> GetIncidentMasterDataBatchAsync(IEnumerable<Incident> incidents);
}

/// <summary>
/// インシデントのマスタデータをバッチで管理するクラス
/// </summary>
public class IncidentMasterDataBatch
{
    public Dictionary<int, TroubleType> TroubleTypes { get; set; } = new();
    public Dictionary<int, DamageType> DamageTypes { get; set; } = new();
    public Dictionary<int, Warehouse> Warehouses { get; set; } = new();
    public Dictionary<int, ShippingCompany> ShippingCompanies { get; set; } = new();
    public Dictionary<int, User> Users { get; set; } = new();
}
