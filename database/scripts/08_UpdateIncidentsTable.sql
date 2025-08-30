-- 物流トラブル管理システム マスタ管理機能拡張
-- Incidentsテーブル更新スクリプト（依存関係対応版）
-- 作成日: 2025-08-30
-- 作成者: システム開発チーム

USE LogisticsTroubleManagement;
GO

-- 0. 事前チェック
PRINT N'=== 事前チェック開始 ===';

-- マスタテーブルの存在確認
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TroubleTypes')
BEGIN
    RAISERROR (N'TroubleTypesテーブルが存在しません。先にマスタテーブルを作成してください。', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DamageTypes')
BEGIN
    RAISERROR (N'DamageTypesテーブルが存在しません。先にマスタテーブルを作成してください。', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Warehouses')
BEGIN
    RAISERROR (N'Warehousesテーブルが存在しません。先にマスタテーブルを作成してください。', 16, 1);
    RETURN;
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ShippingCompanies')
BEGIN
    RAISERROR (N'ShippingCompaniesテーブルが存在しません。先にマスタテーブルを作成してください。', 16, 1);
    RETURN;
END

-- マスタデータの存在確認
IF NOT EXISTS (SELECT * FROM TroubleTypes)
BEGIN
    RAISERROR (N'TroubleTypesテーブルにデータが存在しません。先にマスタデータを挿入してください。', 16, 1);
    RETURN;
END

-- 現在のEnum値の確認
PRINT N'現在のIncidentsテーブルのEnum値を確認中...';
SELECT 
    'TroubleType' as FieldName,
    TroubleType as EnumValue,
    COUNT(*) as RecordCount
FROM Incidents 
GROUP BY TroubleType
ORDER BY TroubleType;

SELECT 
    'DamageType' as FieldName,
    DamageType as EnumValue,
    COUNT(*) as RecordCount
FROM Incidents 
GROUP BY DamageType
ORDER BY DamageType;

SELECT 
    'Warehouse' as FieldName,
    Warehouse as EnumValue,
    COUNT(*) as RecordCount
FROM Incidents 
GROUP BY Warehouse
ORDER BY Warehouse;

SELECT 
    'ShippingCompany' as FieldName,
    ShippingCompany as EnumValue,
    COUNT(*) as RecordCount
FROM Incidents 
GROUP BY ShippingCompany
ORDER BY ShippingCompany;

-- マスタテーブルの内容確認
PRINT N'マスタテーブルの内容を確認中...';
SELECT 'TroubleTypes' as TableName, Id, Name, Description FROM TroubleTypes ORDER BY Id;
SELECT 'DamageTypes' as TableName, Id, Name, Description FROM DamageTypes ORDER BY Id;
SELECT 'Warehouses' as TableName, Id, Name, Description FROM Warehouses ORDER BY Id;
SELECT 'ShippingCompanies' as TableName, Id, Name, Description FROM ShippingCompanies ORDER BY Id;

PRINT N'=== 事前チェック完了 ===';
GO

-- 1. 新しいマスタIDカラムの追加
PRINT N'新しいマスタIDカラムを追加中...';

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Incidents' AND COLUMN_NAME = 'TroubleTypeId')
BEGIN
    ALTER TABLE Incidents ADD TroubleTypeId INT NULL;
    PRINT N'TroubleTypeIdカラムが追加されました。';
END
ELSE
BEGIN
    PRINT N'TroubleTypeIdカラムは既に存在します。';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Incidents' AND COLUMN_NAME = 'DamageTypeId')
BEGIN
    ALTER TABLE Incidents ADD DamageTypeId INT NULL;
    PRINT N'DamageTypeIdカラムが追加されました。';
END
ELSE
BEGIN
    PRINT N'DamageTypeIdカラムは既に存在します。';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Incidents' AND COLUMN_NAME = 'WarehouseId')
BEGIN
    ALTER TABLE Incidents ADD WarehouseId INT NULL;
    PRINT N'WarehouseIdカラムが追加されました。';
END
ELSE
BEGIN
    PRINT N'WarehouseIdカラムは既に存在します。';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Incidents' AND COLUMN_NAME = 'ShippingCompanyId')
BEGIN
    ALTER TABLE Incidents ADD ShippingCompanyId INT NULL;
    PRINT N'ShippingCompanyIdカラムが追加されました。';
END
ELSE
BEGIN
    PRINT N'ShippingCompanyIdカラムは既に存在します。';
END

GO

-- 2. 既存データの移行（IDベースでマッピング）
PRINT N'既存データの移行を開始します...';

-- TroubleTypeの移行（IDベース）
PRINT N'TroubleTypeの移行中...';
UPDATE Incidents SET TroubleTypeId = 1 WHERE TroubleType = 0; -- 商品トラブル
UPDATE Incidents SET TroubleTypeId = 2 WHERE TroubleType = 1; -- 配送トラブル
UPDATE Incidents SET TroubleTypeId = 3 WHERE TroubleType = 2; -- システムトラブル
UPDATE Incidents SET TroubleTypeId = 4 WHERE TroubleType = 3; -- 設備トラブル
UPDATE Incidents SET TroubleTypeId = 5 WHERE TroubleType = 4; -- 品質トラブル
UPDATE Incidents SET TroubleTypeId = 6 WHERE TroubleType = 5; -- 物流トラブル
UPDATE Incidents SET TroubleTypeId = 7 WHERE TroubleType = 6; -- 管理トラブル
UPDATE Incidents SET TroubleTypeId = 8 WHERE TroubleType = 7; -- 安全トラブル
UPDATE Incidents SET TroubleTypeId = 9 WHERE TroubleType = 8; -- 環境トラブル
UPDATE Incidents SET TroubleTypeId = 10 WHERE TroubleType = 9; -- その他のトラブル

-- DamageTypeの移行（IDベース）
PRINT N'DamageTypeの移行中...';
UPDATE Incidents SET DamageTypeId = 1 WHERE DamageType = 0; -- 該当なし
UPDATE Incidents SET DamageTypeId = 2 WHERE DamageType = 1; -- 誤出荷
UPDATE Incidents SET DamageTypeId = 3 WHERE DamageType = 2; -- 早着・延着
UPDATE Incidents SET DamageTypeId = 4 WHERE DamageType = 3; -- 紛失
UPDATE Incidents SET DamageTypeId = 5 WHERE DamageType = 4; -- 誤配送
UPDATE Incidents SET DamageTypeId = 6 WHERE DamageType = 5; -- 破損・汚損
UPDATE Incidents SET DamageTypeId = 7 WHERE DamageType = 6; -- その他の配送ミス
UPDATE Incidents SET DamageTypeId = 8 WHERE DamageType = 7; -- その他の商品事故

-- Warehouseの移行（IDベース）
PRINT N'Warehouseの移行中...';
UPDATE Incidents SET WarehouseId = 1 WHERE Warehouse = 0; -- 該当なし
UPDATE Incidents SET WarehouseId = 2 WHERE Warehouse = 1; -- A倉庫
UPDATE Incidents SET WarehouseId = 3 WHERE Warehouse = 2; -- B倉庫
UPDATE Incidents SET WarehouseId = 4 WHERE Warehouse = 3; -- C倉庫

-- ShippingCompanyの移行（IDベース）
PRINT N'ShippingCompanyの移行中...';
UPDATE Incidents SET ShippingCompanyId = 1 WHERE ShippingCompany = 0; -- 該当なし
UPDATE Incidents SET ShippingCompanyId = 2 WHERE ShippingCompany = 1; -- 庫内
UPDATE Incidents SET ShippingCompanyId = 3 WHERE ShippingCompany = 2; -- チャーター
UPDATE Incidents SET ShippingCompanyId = 4 WHERE ShippingCompany = 3; -- A運輸
UPDATE Incidents SET ShippingCompanyId = 5 WHERE ShippingCompany = 4; -- B急便

PRINT N'既存データの移行が完了しました。';
GO

-- 3. 移行結果の確認
PRINT N'移行結果を確認中...';
SELECT 
    'TroubleType' as FieldName,
    COUNT(*) as TotalRecords,
    COUNT(TroubleTypeId) as MigratedRecords,
    COUNT(*) - COUNT(TroubleTypeId) as UnmigratedRecords
FROM Incidents
UNION ALL
SELECT 
    'DamageType',
    COUNT(*),
    COUNT(DamageTypeId),
    COUNT(*) - COUNT(DamageTypeId)
FROM Incidents
UNION ALL
SELECT 
    'Warehouse',
    COUNT(*),
    COUNT(WarehouseId),
    COUNT(*) - COUNT(WarehouseId)
FROM Incidents
UNION ALL
SELECT 
    'ShippingCompany',
    COUNT(*),
    COUNT(ShippingCompanyId),
    COUNT(*) - COUNT(ShippingCompanyId)
FROM Incidents;

-- 移行されていないレコードの詳細確認
PRINT N'移行されていないレコードの詳細:';
SELECT 
    Id,
    TroubleType,
    TroubleTypeId,
    DamageType,
    DamageTypeId,
    Warehouse,
    WarehouseId,
    ShippingCompany,
    ShippingCompanyId
FROM Incidents 
WHERE TroubleTypeId IS NULL 
   OR DamageTypeId IS NULL 
   OR WarehouseId IS NULL 
   OR ShippingCompanyId IS NULL;

GO

-- 4. 外部キー制約の追加
PRINT N'外部キー制約を追加中...';

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_Incidents_TroubleTypes')
BEGIN
    ALTER TABLE Incidents ADD CONSTRAINT FK_Incidents_TroubleTypes 
        FOREIGN KEY (TroubleTypeId) REFERENCES TroubleTypes(Id);
    PRINT N'FK_Incidents_TroubleTypes制約が追加されました。';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_Incidents_DamageTypes')
BEGIN
    ALTER TABLE Incidents ADD CONSTRAINT FK_Incidents_DamageTypes 
        FOREIGN KEY (DamageTypeId) REFERENCES DamageTypes(Id);
    PRINT N'FK_Incidents_DamageTypes制約が追加されました。';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_Incidents_Warehouses')
BEGIN
    ALTER TABLE Incidents ADD CONSTRAINT FK_Incidents_Warehouses 
        FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id);
    PRINT N'FK_Incidents_Warehouses制約が追加されました。';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_Incidents_ShippingCompanies')
BEGIN
    ALTER TABLE Incidents ADD CONSTRAINT FK_Incidents_ShippingCompanies 
        FOREIGN KEY (ShippingCompanyId) REFERENCES ShippingCompanies(Id);
    PRINT N'FK_Incidents_ShippingCompanies制約が追加されました。';
END

GO

-- 5. 依存関係の確認と削除
PRINT N'依存関係を確認中...';

-- デフォルト制約の確認
SELECT 
    'Default Constraints' as ConstraintType,
    dc.name as ConstraintName,
    c.name as ColumnName
FROM sys.default_constraints dc
JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
WHERE c.object_id = OBJECT_ID('Incidents')
    AND c.name IN ('TroubleType', 'DamageType', 'Warehouse', 'ShippingCompany');

-- インデックスの確認
SELECT 
    'Indexes' as IndexType,
    i.name as IndexName,
    c.name as ColumnName
FROM sys.indexes i
JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE i.object_id = OBJECT_ID('Incidents')
    AND c.name IN ('TroubleType', 'DamageType', 'Warehouse', 'ShippingCompany');

-- チェック制約の確認
SELECT 
    'Check Constraints' as ConstraintType,
    cc.name as ConstraintName,
    c.name as ColumnName
FROM sys.check_constraints cc
JOIN sys.columns c ON cc.parent_object_id = c.object_id
WHERE c.object_id = OBJECT_ID('Incidents')
    AND c.name IN ('TroubleType', 'DamageType', 'Warehouse', 'ShippingCompany');

GO

-- 6. 依存関係の削除
PRINT N'依存関係を削除中...';

-- デフォルト制約の削除
DECLARE @sql NVARCHAR(MAX) = '';

-- TroubleTypeのデフォルト制約を削除
SELECT @sql = @sql + 'ALTER TABLE Incidents DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.default_constraints dc
JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
WHERE c.object_id = OBJECT_ID('Incidents') AND c.name = 'TroubleType';

-- DamageTypeのデフォルト制約を削除
SELECT @sql = @sql + 'ALTER TABLE Incidents DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.default_constraints dc
JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
WHERE c.object_id = OBJECT_ID('Incidents') AND c.name = 'DamageType';

-- Warehouseのデフォルト制約を削除
SELECT @sql = @sql + 'ALTER TABLE Incidents DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.default_constraints dc
JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
WHERE c.object_id = OBJECT_ID('Incidents') AND c.name = 'Warehouse';

-- ShippingCompanyのデフォルト制約を削除
SELECT @sql = @sql + 'ALTER TABLE Incidents DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.default_constraints dc
JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
WHERE c.object_id = OBJECT_ID('Incidents') AND c.name = 'ShippingCompany';

IF @sql <> ''
BEGIN
    PRINT N'デフォルト制約を削除中...';
    EXEC sp_executesql @sql;
    PRINT N'デフォルト制約の削除が完了しました。';
END
ELSE
BEGIN
    PRINT N'削除対象のデフォルト制約はありません。';
END

GO

-- 7. 古いEnumカラムの削除（移行が完了している場合のみ）
PRINT N'古いEnumカラムを削除中...';

-- 移行が完了しているか確認
IF EXISTS (SELECT * FROM Incidents WHERE TroubleTypeId IS NULL OR DamageTypeId IS NULL OR WarehouseId IS NULL OR ShippingCompanyId IS NULL)
BEGIN
    PRINT N'警告: 一部のレコードが移行されていません。古いEnumカラムは削除しません。';
    RETURN;
END

-- 古いEnumカラムの削除
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Incidents' AND COLUMN_NAME = 'TroubleType')
BEGIN
    ALTER TABLE Incidents DROP COLUMN TroubleType;
    PRINT N'古いTroubleTypeカラムが削除されました。';
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Incidents' AND COLUMN_NAME = 'DamageType')
BEGIN
    ALTER TABLE Incidents DROP COLUMN DamageType;
    PRINT N'古いDamageTypeカラムが削除されました。';
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Incidents' AND COLUMN_NAME = 'Warehouse')
BEGIN
    ALTER TABLE Incidents DROP COLUMN Warehouse;
    PRINT N'古いWarehouseカラムが削除されました。';
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Incidents' AND COLUMN_NAME = 'ShippingCompany')
BEGIN
    ALTER TABLE Incidents DROP COLUMN ShippingCompany;
    PRINT N'古いShippingCompanyカラムが削除されました。';
END

GO

-- 8. 新しいカラムをNOT NULLに変更（移行が完了している場合のみ）
PRINT N'新しいカラムをNOT NULLに変更中...';

-- 移行が完了しているか再確認
IF EXISTS (SELECT * FROM Incidents WHERE TroubleTypeId IS NULL OR DamageTypeId IS NULL OR WarehouseId IS NULL OR ShippingCompanyId IS NULL)
BEGIN
    PRINT N'警告: 一部のレコードが移行されていません。NOT NULL制約は追加しません。';
    RETURN;
END

-- NOT NULL制約の追加
ALTER TABLE Incidents ALTER COLUMN TroubleTypeId INT NOT NULL;
ALTER TABLE Incidents ALTER COLUMN DamageTypeId INT NOT NULL;
ALTER TABLE Incidents ALTER COLUMN WarehouseId INT NOT NULL;
ALTER TABLE Incidents ALTER COLUMN ShippingCompanyId INT NOT NULL;

PRINT N'新しいカラムがNOT NULLに変更されました。';
GO

-- 9. 最終確認
PRINT N'=== 最終確認 ===';
SELECT 
    'Incidents' as TableName,
    COUNT(*) as TotalRecords,
    COUNT(TroubleTypeId) as TroubleTypeRecords,
    COUNT(DamageTypeId) as DamageTypeRecords,
    COUNT(WarehouseId) as WarehouseRecords,
    COUNT(ShippingCompanyId) as ShippingCompanyRecords
FROM Incidents;

-- 外部キー制約の確認
SELECT 
    tc.CONSTRAINT_NAME,
    tc.TABLE_NAME,
    kcu.COLUMN_NAME,
    ccu.TABLE_NAME AS FOREIGN_TABLE_NAME,
    ccu.COLUMN_NAME AS FOREIGN_COLUMN_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu
    ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE AS ccu
    ON ccu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
WHERE tc.CONSTRAINT_TYPE = 'FOREIGN KEY'
    AND tc.TABLE_NAME = 'Incidents';

-- 現在のテーブル構造の確認
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Incidents'
ORDER BY ORDINAL_POSITION;

PRINT N'Incidentsテーブルの更新が完了しました。';
GO
