-- データベース接続・動作確認スクリプト
-- 物流トラブル管理システム用テストスクリプト

PRINT '=== データベース接続・動作確認開始 ===';
GO

-- 1. SQL Server バージョン確認
PRINT '1. SQL Server バージョン確認';
SELECT @@VERSION as SQLServerVersion;
GO

-- 2. データベース確認
PRINT '2. データベース確認';
SELECT 
    name as DatabaseName,
    state_desc as State,
    recovery_model_desc as RecoveryModel
FROM sys.databases 
WHERE name = 'LTMDB';
GO

-- 3. テーブル一覧確認
PRINT '3. テーブル一覧確認';
USE LTMDB;
GO

SELECT 
    TABLE_SCHEMA as SchemaName,
    TABLE_NAME as TableName,
    TABLE_TYPE as TableType
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
GO

-- 4. 各テーブルのレコード数確認
PRINT '4. 各テーブルのレコード数確認';
SELECT 'Users' as TableName, COUNT(*) as RecordCount FROM Users
UNION ALL
SELECT 'Incidents', COUNT(*) FROM Incidents
UNION ALL
SELECT 'Attachments', COUNT(*) FROM Attachments
UNION ALL
SELECT 'AuditLogs', COUNT(*) FROM AuditLogs
UNION ALL
SELECT 'Effectiveness', COUNT(*) FROM Effectiveness;
GO

-- 5. サンプルデータ確認
PRINT '5. サンプルデータ確認';

-- ユーザーデータ
PRINT '--- ユーザーデータ ---';
SELECT TOP 3 Id, Username, Email, Role, IsActive FROM Users ORDER BY Id;

-- トラブルデータ
PRINT '--- トラブルデータ ---';
SELECT TOP 5 Id, Title, Status, Priority, Category, ReportedDate 
FROM Incidents 
ORDER BY ReportedDate DESC;

-- 効果測定データ
PRINT '--- 効果測定データ ---';
SELECT TOP 3 e.Id, i.Title, e.EffectivenessType, e.Value, e.Unit, e.Description
FROM Effectiveness e
JOIN Incidents i ON e.IncidentId = i.Id
ORDER BY e.MeasuredAt DESC;
GO

-- 6. インデックス確認
PRINT '6. インデックス確認';
SELECT 
    t.name as TableName,
    i.name as IndexName,
    i.type_desc as IndexType,
    i.is_unique as IsUnique
FROM sys.indexes i
JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name IN ('Users', 'Incidents', 'Attachments', 'AuditLogs', 'Effectiveness')
ORDER BY t.name, i.name;
GO

-- 7. 外部キー制約確認
PRINT '7. 外部キー制約確認';
SELECT 
    fk.name as ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) as TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) as ColumnName,
    OBJECT_NAME(fk.referenced_object_id) as ReferencedTableName,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) as ReferencedColumnName
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
ORDER BY TableName, ForeignKeyName;
GO

-- 8. パフォーマンステスト
PRINT '8. パフォーマンステスト';

-- トラブル検索テスト
PRINT '--- トラブル検索テスト ---';
SET STATISTICS TIME ON;
SELECT Id, Title, Status, Priority, Category, ReportedDate
FROM Incidents 
WHERE Status = 'Open' AND Priority = 'High'
ORDER BY ReportedDate DESC;
SET STATISTICS TIME OFF;
GO

-- 9. 接続情報確認
PRINT '9. 接続情報確認';
SELECT 
    @@SERVERNAME as ServerName,
    DB_NAME() as CurrentDatabase,
    SYSTEM_USER as CurrentUser,
    GETUTCDATE() as CurrentUTCTime;
GO

PRINT '=== データベース接続・動作確認完了 ===';
GO
