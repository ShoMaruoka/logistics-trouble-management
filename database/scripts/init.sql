-- データベース初期化スクリプト
-- 物流トラブル管理システム用データベース初期化

PRINT '=== 物流トラブル管理システム データベース初期化開始 ===';
GO

-- 1. データベース作成
PRINT '1. データベース作成中...';
:r ./01_CreateDatabase.sql
GO

-- 2. テーブル作成
PRINT '2. テーブル作成中...';
:r ./02_CreateTables.sql
GO

-- 3. インデックス作成
PRINT '3. インデックス作成中...';
:r ./03_CreateIndexes.sql
GO

-- 4. サンプルデータ挿入
PRINT '4. サンプルデータ挿入中...';
:r ./05_SeedData.sql
GO

PRINT '=== データベース初期化完了 ===';
GO

-- 初期化結果の確認
USE LTMDB;
GO

PRINT '=== 初期化結果確認 ===';
SELECT 
    'Users' as TableName, 
    COUNT(*) as RecordCount 
FROM Users
UNION ALL
SELECT 'Incidents', COUNT(*) FROM Incidents
UNION ALL
SELECT 'Attachments', COUNT(*) FROM Attachments
UNION ALL
SELECT 'AuditLogs', COUNT(*) FROM AuditLogs
UNION ALL
SELECT 'Effectiveness', COUNT(*) FROM Effectiveness;
GO
