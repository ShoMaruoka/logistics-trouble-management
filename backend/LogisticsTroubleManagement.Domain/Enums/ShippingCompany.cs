namespace LogisticsTroubleManagement.Domain.Enums;

/// <summary>
/// 運送会社名
/// </summary>
public enum ShippingCompany
{
    /// <summary>
    /// 庫内
    /// </summary>
    InHouse = 1,

    /// <summary>
    /// チャーター
    /// </summary>
    Charter = 2,

    /// <summary>
    /// A運輸
    /// </summary>
    ATransport = 3,

    /// <summary>
    /// B急便
    /// </summary>
    BExpress = 4
}
