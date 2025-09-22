namespace LogisticsTroubleManagement.Domain.Enums;

/// <summary>
/// 新しいインシデント管理ワークフローのステータス（仕様書2.2準拠）
/// </summary>
public enum IncidentStatus
{
    Unclassified = 1,           // 未分類
    Pending = 2,                // 未対応
    InProgress = 3,             // 対応中
    Completed = 4,              // 対応済
    PreventionProposed = 5,     // 再発防止策提案済
    EffectivenessConfirmed = 6  // 有効性確認済
}
