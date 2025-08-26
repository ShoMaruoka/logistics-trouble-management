namespace LogisticsTroubleManagement.Domain.Enums;

/// <summary>
/// 損傷の種類
/// </summary>
public enum DamageType
{
    /// <summary>
    /// 該当なし
    /// </summary>
    None,

    /// <summary>
    /// 誤出荷
    /// </summary>
    WrongShipment,

    /// <summary>
    /// 早着・延着
    /// </summary>
    EarlyOrLateArrival,

    /// <summary>
    /// 紛失
    /// </summary>
    Lost,

    /// <summary>
    /// 誤配送
    /// </summary>
    WrongDelivery,

    /// <summary>
    /// 破損・汚損
    /// </summary>
    DamageOrContamination,

    /// <summary>
    /// その他の配送ミス
    /// </summary>
    OtherDeliveryMistake,

    /// <summary>
    /// その他の商品事故
    /// </summary>
    OtherProductAccident
}
