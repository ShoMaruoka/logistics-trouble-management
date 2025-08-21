using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Core.DTOs;

public class CreateIncidentDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    
    // 物流特化項目
    public TroubleType TroubleType { get; set; }
    public DamageType DamageType { get; set; }
    public Warehouse Warehouse { get; set; }
    public ShippingCompany ShippingCompany { get; set; }
    
    // 新規追加項目（提供サイト対応）
    public string IncidentDetails { get; set; } = string.Empty; // 発生経緯
    public int TotalShipments { get; set; } = 0; // 出荷総数
    public int DefectiveItems { get; set; } = 0; // 不良品数
    public DateTime OccurrenceDate { get; set; } // 発生日
    public string OccurrenceLocation { get; set; } = string.Empty; // 発生場所
    public string Summary { get; set; } = string.Empty; // 概要
    public string Cause { get; set; } = string.Empty; // 原因
    public string PreventionMeasures { get; set; } = string.Empty; // 再発防止策
    public EffectivenessStatus EffectivenessStatus { get; set; } = EffectivenessStatus.NotImplemented; // 有効性評価
    public DateTime? EffectivenessDate { get; set; } // 有効性確認日
    public string EffectivenessComment { get; set; } = string.Empty; // 有効性確認コメント
    
    public int ReportedById { get; set; }
    public int? AssignedToId { get; set; }
}
