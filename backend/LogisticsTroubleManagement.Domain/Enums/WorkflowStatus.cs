namespace LogisticsTroubleManagement.Domain.Enums;

/// <summary>
/// 新しいインシデント管理ワークフローのステータス
/// 既存のIncidentStatusと並行運用し、段階的に移行する
/// </summary>
public enum WorkflowStatus
{
    /// <summary>
    /// 未分類 - インシデントが登録されたが、まだ分類されていない状態
    /// </summary>
    Unclassified = 1,

    /// <summary>
    /// 未対応 - 分類は完了したが、まだ対応が開始されていない状態
    /// </summary>
    Pending = 2,

    /// <summary>
    /// 対応中 - 対応が開始され、現在進行中の状態
    /// </summary>
    InProgress = 3,

    /// <summary>
    /// 対応済 - 対応は完了したが、再発防止策がまだ提案されていない状態
    /// </summary>
    Completed = 4,

    /// <summary>
    /// 再発防止策提案済 - 再発防止策が提案され、有効性確認待ちの状態
    /// </summary>
    PreventionProposed = 5,

    /// <summary>
    /// 有効性確認済 - 再発防止策の有効性が確認され、ワークフローが完了した状態
    /// </summary>
    EffectivenessConfirmed = 6
}
