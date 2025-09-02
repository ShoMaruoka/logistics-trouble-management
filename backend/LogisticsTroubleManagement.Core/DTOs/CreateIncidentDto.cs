using LogisticsTroubleManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.Core.DTOs;

public class CreateIncidentDto : IValidatableObject
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Medium;
    
    // 物流特化項目
    public int TroubleType { get; set; }
    public int DamageType { get; set; }
    public int Warehouse { get; set; }
    public int ShippingCompany { get; set; }
    
    // 新規追加項目（提供サイト対応）
    public string IncidentDetails { get; set; } = string.Empty; // 発生経緯
    [Range(0, int.MaxValue, ErrorMessage = "出荷総数は0以上の値を入力してください。")]
    public int TotalShipments { get; set; } = 0; // 出荷総数
    [Range(0, int.MaxValue, ErrorMessage = "不良品数は0以上の値を入力してください。")]
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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        // Enum値の検証
        if (!Enum.IsDefined(typeof(TroubleType), TroubleType))
        {
            results.Add(new ValidationResult(
                $"トラブル種類の値 '{TroubleType}' は無効です。",
                new[] { nameof(TroubleType) }));
        }

        if (!Enum.IsDefined(typeof(DamageType), DamageType))
        {
            results.Add(new ValidationResult(
                $"損傷種類の値 '{DamageType}' は無効です。",
                new[] { nameof(DamageType) }));
        }

        // クロスフィールドルール: DefectiveItems <= TotalShipments
        if (DefectiveItems > TotalShipments)
        {
            results.Add(new ValidationResult(
                "不良品数は出荷総数以下である必要があります。",
                new[] { nameof(DefectiveItems), nameof(TotalShipments) }));
        }

        // クロスフィールドルール: EffectivenessStatusがImplementedの場合、EffectivenessDateが必須
        if (EffectivenessStatus == EffectivenessStatus.Implemented && !EffectivenessDate.HasValue)
        {
            results.Add(new ValidationResult(
                "有効性評価が実施済みの場合、有効性確認日は必須です。",
                new[] { nameof(EffectivenessDate) }));
        }

        // 参照存在チェックはアプリケーション/サービス層で実行
        // WarehouseとShippingCompanyの存在チェックはリポジトリ層で確認し、
        // 存在しない場合は400/422エラーを返す

        return results;
    }
}
