-- ===============================================
-- インシデント管理ワークフロー改修
-- 新ワークフロー関連フィールドの追加
-- ===============================================
-- 作成日: 2025-09-19
-- 説明: 新しい6段階ワークフロー対応のためのフィールドを追加
--       既存データを保護するため、すべてのフィールドはNULL許可
-- ===============================================

USE LTMDB;
GO

-- トランザクション開始
BEGIN TRANSACTION;

BEGIN TRY
    PRINT '=== インシデント管理ワークフロー改修 - フィールド追加開始 ===';

    -- Incidentsテーブルに新ワークフロー関連フィールドを追加
    PRINT '新ワークフロー関連フィールドを追加中...';
    
    ALTER TABLE Incidents ADD
        -- 新ワークフローステータス（NULL許可）
        WorkflowStatus INT NULL,
        
        -- 対応期限
        DueDate DATETIME2 NULL,
        
        -- ワークフロー進捗日付
        ResponseStartDate DATETIME2 NULL,           -- 対応開始日
        CauseAnalysisDate DATETIME2 NULL,          -- 原因入力日
        CompletionDate DATETIME2 NULL,             -- 対応完了日
        PreventionProposalDate DATETIME2 NULL,     -- 再発防止策提案日
        EffectivenessConfirmationDate DATETIME2 NULL, -- 有効性確認日
        
        -- 新作業内容フィールド
        ResponseContent NVARCHAR(2000) NULL;       -- 対応内容

    PRINT '新フィールドの追加が完了しました。';

    -- WorkflowStatusフィールドにCHECK制約を追加（1-6の値のみ許可）
    PRINT 'WorkflowStatusフィールドにCHECK制約を追加中...';
    
    ALTER TABLE Incidents 
    ADD CONSTRAINT CK_Incidents_WorkflowStatus 
    CHECK (WorkflowStatus IS NULL OR WorkflowStatus BETWEEN 1 AND 6);

    PRINT 'CHECK制約の追加が完了しました。';

    -- インデックスの追加（パフォーマンス向上のため）
    PRINT 'パフォーマンス向上のためのインデックスを追加中...';
    
    -- WorkflowStatusでの検索用インデックス
    CREATE NONCLUSTERED INDEX IX_Incidents_WorkflowStatus 
    ON Incidents (WorkflowStatus) 
    WHERE WorkflowStatus IS NOT NULL;

    -- DueDateでの検索用インデックス
    CREATE NONCLUSTERED INDEX IX_Incidents_DueDate 
    ON Incidents (DueDate) 
    WHERE DueDate IS NOT NULL;

    -- 対応開始日での検索用インデックス
    CREATE NONCLUSTERED INDEX IX_Incidents_ResponseStartDate 
    ON Incidents (ResponseStartDate) 
    WHERE ResponseStartDate IS NOT NULL;

    -- 対応完了日での検索用インデックス
    CREATE NONCLUSTERED INDEX IX_Incidents_CompletionDate 
    ON Incidents (CompletionDate) 
    WHERE CompletionDate IS NOT NULL;

    PRINT 'インデックスの追加が完了しました。';

    -- 変更内容の確認
    PRINT '=== 追加されたフィールドの確認 ===';
    SELECT 
        COLUMN_NAME,
        DATA_TYPE,
        IS_NULLABLE,
        COLUMN_DEFAULT
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Incidents' 
    AND COLUMN_NAME IN (
        'WorkflowStatus',
        'DueDate',
        'ResponseStartDate',
        'CauseAnalysisDate',
        'CompletionDate',
        'PreventionProposalDate',
        'EffectivenessConfirmationDate',
        'ResponseContent'
    )
    ORDER BY ORDINAL_POSITION;

    -- 制約の確認
    PRINT '=== 追加されたCHECK制約の確認 ===';
    SELECT 
        CONSTRAINT_NAME,
        CHECK_CLAUSE
    FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS 
    WHERE TABLE_NAME = 'Incidents'
    AND CONSTRAINT_NAME = 'CK_Incidents_WorkflowStatus';

    -- インデックスの確認
    PRINT '=== 追加されたインデックスの確認 ===';
    SELECT 
        i.name AS IndexName,
        i.type_desc AS IndexType,
        i.is_unique AS IsUnique,
        i.has_filter AS HasFilter,
        i.filter_definition AS FilterDefinition
    FROM sys.indexes i
    INNER JOIN sys.objects o ON i.object_id = o.object_id
    WHERE o.name = 'Incidents'
    AND i.name LIKE 'IX_Incidents_%WorkflowStatus%'
    OR i.name LIKE 'IX_Incidents_%DueDate%'
    OR i.name LIKE 'IX_Incidents_%ResponseStartDate%'
    OR i.name LIKE 'IX_Incidents_%CompletionDate%';

    -- 既存データの確認
    PRINT '=== 既存データの確認 ===';
    SELECT 
        COUNT(*) AS TotalIncidents,
        COUNT(WorkflowStatus) AS WithWorkflowStatus,
        COUNT(*) - COUNT(WorkflowStatus) AS WithoutWorkflowStatus
    FROM Incidents;

    PRINT '=== インシデント管理ワークフロー改修 - フィールド追加完了 ===';

    -- トランザクションをコミット
    COMMIT TRANSACTION;
    PRINT 'トランザクションがコミットされました。';

END TRY
BEGIN CATCH
    -- エラーが発生した場合はロールバック
    ROLLBACK TRANSACTION;
    
    PRINT 'エラーが発生しました。トランザクションをロールバックしました。';
    PRINT 'エラー詳細:';
    PRINT 'エラー番号: ' + CAST(ERROR_NUMBER() AS NVARCHAR(50));
    PRINT 'エラーメッセージ: ' + ERROR_MESSAGE();
    PRINT 'エラー行: ' + CAST(ERROR_LINE() AS NVARCHAR(50));
    
    -- エラーを再発生させる
    THROW;
END CATCH;

GO

-- 実行後の確認用クエリ（コメントアウト）
/*
-- 新しいフィールドが正しく追加されたかを確認
SELECT TOP 5 
    Id,
    Title,
    Status,
    WorkflowStatus,
    DueDate,
    ResponseStartDate,
    CauseAnalysisDate,
    CompletionDate,
    PreventionProposalDate,
    EffectivenessConfirmationDate,
    ResponseContent
FROM Incidents
ORDER BY CreatedAt DESC;

-- WorkflowStatus の値分布を確認
SELECT 
    WorkflowStatus,
    COUNT(*) AS Count,
    CASE WorkflowStatus
        WHEN 1 THEN 'Unclassified (未分類)'
        WHEN 2 THEN 'Pending (未対応)'
        WHEN 3 THEN 'InProgress (対応中)'
        WHEN 4 THEN 'Completed (対応済)'
        WHEN 5 THEN 'PreventionProposed (再発防止策提案済)'
        WHEN 6 THEN 'EffectivenessConfirmed (有効性確認済)'
        WHEN NULL THEN 'NULL (レガシーモード)'
        ELSE 'Unknown'
    END AS StatusDescription
FROM Incidents
GROUP BY WorkflowStatus
ORDER BY WorkflowStatus;
*/
