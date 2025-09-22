-- 既存データを新仕様のIncidentStatusに完全移行
-- 平行運用は考慮せず、新仕様に完全準拠

-- 新しいIncidentStatusの定義（仕様書2.2準拠）:
-- 1: Unclassified (未分類)
-- 2: Pending (未対応)
-- 3: InProgress (対応中)
-- 4: Completed (対応済)
-- 5: PreventionProposed (再発防止策提案済)
-- 6: EffectivenessConfirmed (有効性確認済)

-- 旧IncidentStatusの定義:
-- 1: Open (未解決)
-- 2: InProgress (対応中)
-- 3: Resolved (解決済み)
-- 4: Closed (完了)
-- 5: Cancelled (キャンセル)

BEGIN TRANSACTION;

-- 1. 既存データのステータス移行
UPDATE Incidents 
SET Status = CASE 
    WHEN Status = 1 AND (Category IS NULL OR Category = '') THEN 1  -- Open → Unclassified (未分類)
    WHEN Status = 1 THEN 2                                         -- Open → Pending (未対応)
    WHEN Status = 2 THEN 3                                         -- InProgress → InProgress (対応中)
    WHEN Status = 3 THEN 4                                         -- Resolved → Completed (対応済)
    WHEN Status = 4 THEN 6                                         -- Closed → EffectivenessConfirmed (有効性確認済)
    WHEN Status = 5 THEN 6                                         -- Cancelled → EffectivenessConfirmed (有効性確認済)
    ELSE Status
END,
UpdatedAt = GETUTCDATE()
WHERE Status IN (1, 2, 3, 4, 5);

-- 2. WorkflowStatusカラムを削除（新仕様ではStatusに統合）
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Incidents') AND name = 'WorkflowStatus')
BEGIN
    ALTER TABLE Incidents DROP COLUMN WorkflowStatus;
    PRINT 'WorkflowStatusカラムを削除しました';
END

-- 3. 移行結果の確認
SELECT 
    Status,
    CASE Status
        WHEN 1 THEN '未分類 (Unclassified)'
        WHEN 2 THEN '未対応 (Pending)'
        WHEN 3 THEN '対応中 (InProgress)'
        WHEN 4 THEN '対応済 (Completed)'
        WHEN 5 THEN '再発防止策提案済 (PreventionProposed)'
        WHEN 6 THEN '有効性確認済 (EffectivenessConfirmed)'
        ELSE '不明'
    END AS StatusName,
    COUNT(*) as Count
FROM Incidents 
GROUP BY Status
ORDER BY Status;

COMMIT TRANSACTION;

PRINT '新ワークフロー仕様への完全移行が完了しました';
