namespace LogisticsTroubleManagement.Domain.Enums;

/// <summary>
/// 運送会社名
/// </summary>
public enum ShippingCompany
{
    /// <summary>
    /// 該当なし
    /// </summary>
    None,

    /// <summary>
    /// 庫内
    /// </summary>
    InHouse,

    /// <summary>
    /// チャーター
    /// </summary>
    Charter,

    /// <summary>
    /// A運輸
    /// </summary>
    ATransport,

    /// <summary>
    /// B急便
    /// </summary>
    BExpress
}
