-- 物流トラブル管理システム UpdatedAtフィールドNULL許可化
-- データベース更新スクリプト
-- 作成日: 2025-09-02
-- 作成者: システム開発チーム

USE LogisticsTroubleManagement;
GO

PRINT N'UpdatedAtフィールドのNULL許可化を開始します...';

-- 1. IncidentsテーブルのUpdatedAtフィールドをNULL許可に変更
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Incidents]') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE [dbo].[Incidents] ALTER COLUMN [UpdatedAt] DATETIME2 NULL;
    PRINT N'IncidentsテーブルのUpdatedAtフィールドをNULL許可に変更しました。';
END
ELSE
BEGIN
    PRINT N'IncidentsテーブルのUpdatedAtフィールドが見つかりません。';
END

-- 2. UsersテーブルのUpdatedAtフィールドをNULL許可に変更
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE [dbo].[Users] ALTER COLUMN [UpdatedAt] DATETIME2 NULL;
    PRINT N'UsersテーブルのUpdatedAtフィールドをNULL許可に変更しました。';
END
ELSE
BEGIN
    PRINT N'UsersテーブルのUpdatedAtフィールドが見つかりません。';
END

-- 3. AttachmentsテーブルのUpdatedAtフィールドをNULL許可に変更
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Attachments]') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE [dbo].[Attachments] ALTER COLUMN [UpdatedAt] DATETIME2 NULL;
    PRINT N'AttachmentsテーブルのUpdatedAtフィールドをNULL許可に変更しました。';
END
ELSE
BEGIN
    PRINT N'AttachmentsテーブルのUpdatedAtフィールドが見つかりません。';
END

-- 4. AuditLogsテーブルのUpdatedAtフィールドをNULL許可に変更
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE [dbo].[AuditLogs] ALTER COLUMN [UpdatedAt] DATETIME2 NULL;
    PRINT N'AuditLogsテーブルのUpdatedAtフィールドをNULL許可に変更しました。';
END
ELSE
BEGIN
    PRINT N'AuditLogsテーブルのUpdatedAtフィールドが見つかりません。';
END

-- 5. EffectivenessテーブルのUpdatedAtフィールドをNULL許可に変更
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Effectiveness]') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE [dbo].[Effectiveness] ALTER COLUMN [UpdatedAt] DATETIME2 NULL;
    PRINT N'EffectivenessテーブルのUpdatedAtフィールドをNULL許可に変更しました。';
END
ELSE
BEGIN
    PRINT N'EffectivenessテーブルのUpdatedAtフィールドが見つかりません。';
END

-- 6. マスタテーブルのUpdatedAtフィールドをNULL許可に変更
-- TroubleTypes
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TroubleTypes]') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE [dbo].[TroubleTypes] ALTER COLUMN [UpdatedAt] DATETIME2 NULL;
    PRINT N'TroubleTypesテーブルのUpdatedAtフィールドをNULL許可に変更しました。';
END

-- DamageTypes
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[DamageTypes]') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE [dbo].[DamageTypes] ALTER COLUMN [UpdatedAt] DATETIME2 NULL;
    PRINT N'DamageTypesテーブルのUpdatedAtフィールドをNULL許可に変更しました。';
END

-- Warehouses
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Warehouses]') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE [dbo].[Warehouses] ALTER COLUMN [UpdatedAt] DATETIME2 NULL;
    PRINT N'WarehousesテーブルのUpdatedAtフィールドをNULL許可に変更しました。';
END

-- ShippingCompanies
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ShippingCompanies]') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE [dbo].[ShippingCompanies] ALTER COLUMN [UpdatedAt] DATETIME2 NULL;
    PRINT N'ShippingCompaniesテーブルのUpdatedAtフィールドをNULL許可に変更しました。';
END

-- 7. 初期レコードのUpdatedAtフィールドをNULLに設定（オプション）
-- 必要に応じて以下のコメントアウトを解除してください
/*
UPDATE [dbo].[Incidents] SET [UpdatedAt] = NULL WHERE [UpdatedAt] = [CreatedAt];
UPDATE [dbo].[Users] SET [UpdatedAt] = NULL WHERE [UpdatedAt] = [CreatedAt];
UPDATE [dbo].[Attachments] SET [UpdatedAt] = NULL WHERE [UpdatedAt] = [CreatedAt];
UPDATE [dbo].[AuditLogs] SET [UpdatedAt] = NULL WHERE [UpdatedAt] = [CreatedAt];
UPDATE [dbo].[Effectiveness] SET [UpdatedAt] = NULL WHERE [UpdatedAt] = [CreatedAt];
UPDATE [dbo].[TroubleTypes] SET [UpdatedAt] = NULL WHERE [UpdatedAt] = [CreatedAt];
UPDATE [dbo].[DamageTypes] SET [UpdatedAt] = NULL WHERE [UpdatedAt] = [CreatedAt];
UPDATE [dbo].[Warehouses] SET [UpdatedAt] = NULL WHERE [UpdatedAt] = [CreatedAt];
UPDATE [dbo].[ShippingCompanies] SET [UpdatedAt] = NULL WHERE [UpdatedAt] = [CreatedAt];
PRINT N'初期レコードのUpdatedAtフィールドをNULLに設定しました。';
*/

PRINT N'UpdatedAtフィールドのNULL許可化が完了しました。';
GO
