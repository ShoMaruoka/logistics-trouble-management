using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.Core.DTOs;

public class UpdateIncidentDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public IncidentStatus Status { get; set; }
    
    // 物流特化項目
    [EnumValue(typeof(TroubleType))]
    public int? TroubleType { get; set; }
    [EnumValue(typeof(DamageType))]
    public int? DamageType { get; set; }
    public int? Warehouse { get; set; }
    public int? ShippingCompany { get; set; }
    public EffectivenessStatus? EffectivenessStatus { get; set; }
    
    // 新規追加項目（提供サイト対応）
    public string? IncidentDetails { get; set; } // 発生経緯
    public int? TotalShipments { get; set; } // 出荷総数
    public int? DefectiveItems { get; set; } // 不良品数
    public DateTime? OccurrenceDate { get; set; } // 発生日
    public string? OccurrenceLocation { get; set; } // 発生場所
    public string? Summary { get; set; } // 概要
    public string? Cause { get; set; } // 原因
    public string? PreventionMeasures { get; set; } // 再発防止策
    public DateTime? EffectivenessDate { get; set; } // 有効性確認日
    public string? EffectivenessComment { get; set; } // 有効性確認コメント
    
    public int? AssignedToId { get; set; }
    public string? Resolution { get; set; }
}
