namespace LogisticsTroubleManagement.Domain.Enums;

/// <summary>
/// 損傷の種類
/// </summary>
public enum DamageType
{
    /// <summary>
    /// 誤出荷
    /// </summary>
    WrongShipment = 1,

    /// <summary>
    /// 早着・延着
    /// </summary>
    EarlyOrLateArrival = 2,

    /// <summary>
    /// 紛失
    /// </summary>
    Lost = 3,

    /// <summary>
    /// 誤配送
    /// </summary>
    WrongDelivery = 4,

    /// <summary>
    /// 破損・汚損
    /// </summary>
    DamageOrContamination = 5,

    /// <summary>
    /// その他の配送ミス
    /// </summary>
    OtherDeliveryMistake = 6,

    /// <summary>
    /// その他の商品事故
    /// </summary>
    OtherProductAccident = 7
}
