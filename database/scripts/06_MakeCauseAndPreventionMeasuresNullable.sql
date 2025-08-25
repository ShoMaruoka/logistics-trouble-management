-- CauseとPreventionMeasuresフィールドをNULL許可にするスクリプト
-- 物流トラブル管理システム用

USE LTMDB;
GO

-- CauseフィールドをNULL許可に変更
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Incidents]') AND name = 'Cause')
BEGIN
    ALTER TABLE [dbo].[Incidents] ALTER COLUMN [Cause] NVARCHAR(MAX) NULL;
    PRINT 'CauseフィールドをNULL許可に変更しました。';
END
ELSE
BEGIN
    PRINT 'Causeフィールドが見つかりません。';
END

-- PreventionMeasuresフィールドをNULL許可に変更
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Incidents]') AND name = 'PreventionMeasures')
BEGIN
    ALTER TABLE [dbo].[Incidents] ALTER COLUMN [PreventionMeasures] NVARCHAR(MAX) NULL;
    PRINT 'PreventionMeasuresフィールドをNULL許可に変更しました。';
END
ELSE
BEGIN
    PRINT 'PreventionMeasuresフィールドが見つかりません。';
END

-- 既存の空文字列データをNULLに更新（オプション）
UPDATE [dbo].[Incidents] 
SET [Cause] = NULL 
WHERE [Cause] = '';

UPDATE [dbo].[Incidents] 
SET [PreventionMeasures] = NULL 
WHERE [PreventionMeasures] = '';

PRINT '空文字列のデータをNULLに更新しました。';

-- 変更結果の確認
SELECT 
    COLUMN_NAME,
    IS_NULLABLE,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Incidents' 
AND COLUMN_NAME IN ('Cause', 'PreventionMeasures');

PRINT 'スクリプトの実行が完了しました。';
