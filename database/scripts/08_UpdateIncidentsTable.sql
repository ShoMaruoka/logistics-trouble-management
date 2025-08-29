-- 物流トラブル管理システム マスタ管理機能拡張
-- Incidentsテーブル更新スクリプト
-- 作成日: 2025-08-29
-- 作成者: システム開発チーム

USE LogisticsTroubleManagement;
GO

-- 1. 新しいマスタIDカラムの追加
ALTER TABLE Incidents ADD TroubleTypeId INT NULL;
ALTER TABLE Incidents ADD DamageTypeId INT NULL;
ALTER TABLE Incidents ADD WarehouseId INT NULL;
ALTER TABLE Incidents ADD ShippingCompanyId INT NULL;

PRINT '新しいマスタIDカラムが追加されました。';
GO

-- 2. 既存データの移行（Nameベースでマッピング）
-- TroubleTypeの移行
UPDATE Incidents SET TroubleTypeId = (SELECT Id FROM TroubleTypes WHERE Name = '商品トラブル') WHERE TroubleType = 0;
UPDATE Incidents SET TroubleTypeId = (SELECT Id FROM TroubleTypes WHERE Name = '配送トラブル') WHERE TroubleType = 1;
UPDATE Incidents SET TroubleTypeId = (SELECT Id FROM TroubleTypes WHERE Name = 'システムトラブル') WHERE TroubleType = 2;
UPDATE Incidents SET TroubleTypeId = (SELECT Id FROM TroubleTypes WHERE Name = '設備トラブル') WHERE TroubleType = 3;
UPDATE Incidents SET TroubleTypeId = (SELECT Id FROM TroubleTypes WHERE Name = '品質トラブル') WHERE TroubleType = 4;
UPDATE Incidents SET TroubleTypeId = (SELECT Id FROM TroubleTypes WHERE Name = '物流トラブル') WHERE TroubleType = 5;
UPDATE Incidents SET TroubleTypeId = (SELECT Id FROM TroubleTypes WHERE Name = '管理トラブル') WHERE TroubleType = 6;
UPDATE Incidents SET TroubleTypeId = (SELECT Id FROM TroubleTypes WHERE Name = '安全トラブル') WHERE TroubleType = 7;
UPDATE Incidents SET TroubleTypeId = (SELECT Id FROM TroubleTypes WHERE Name = '環境トラブル') WHERE TroubleType = 8;
UPDATE Incidents SET TroubleTypeId = (SELECT Id FROM TroubleTypes WHERE Name = 'その他のトラブル') WHERE TroubleType = 9;

-- DamageTypeの移行
UPDATE Incidents SET DamageTypeId = (SELECT Id FROM DamageTypes WHERE Name = '該当なし') WHERE DamageType = 0;
UPDATE Incidents SET DamageTypeId = (SELECT Id FROM DamageTypes WHERE Name = '誤出荷') WHERE DamageType = 1;
UPDATE Incidents SET DamageTypeId = (SELECT Id FROM DamageTypes WHERE Name = '早着・延着') WHERE DamageType = 2;
UPDATE Incidents SET DamageTypeId = (SELECT Id FROM DamageTypes WHERE Name = '紛失') WHERE DamageType = 3;
UPDATE Incidents SET DamageTypeId = (SELECT Id FROM DamageTypes WHERE Name = '誤配送') WHERE DamageType = 4;
UPDATE Incidents SET DamageTypeId = (SELECT Id FROM DamageTypes WHERE Name = '破損・汚損') WHERE DamageType = 5;
UPDATE Incidents SET DamageTypeId = (SELECT Id FROM DamageTypes WHERE Name = 'その他の配送ミス') WHERE DamageType = 6;
UPDATE Incidents SET DamageTypeId = (SELECT Id FROM DamageTypes WHERE Name = 'その他の商品事故') WHERE DamageType = 7;

-- Warehouseの移行
UPDATE Incidents SET WarehouseId = (SELECT Id FROM Warehouses WHERE Name = '該当なし') WHERE Warehouse = 0;
UPDATE Incidents SET WarehouseId = (SELECT Id FROM Warehouses WHERE Name = 'A倉庫') WHERE Warehouse = 1;
UPDATE Incidents SET WarehouseId = (SELECT Id FROM Warehouses WHERE Name = 'B倉庫') WHERE Warehouse = 2;
UPDATE Incidents SET WarehouseId = (SELECT Id FROM Warehouses WHERE Name = 'C倉庫') WHERE Warehouse = 3;

-- ShippingCompanyの移行
UPDATE Incidents SET ShippingCompanyId = (SELECT Id FROM ShippingCompanies WHERE Name = '該当なし') WHERE ShippingCompany = 0;
UPDATE Incidents SET ShippingCompanyId = (SELECT Id FROM ShippingCompanies WHERE Name = '庫内') WHERE ShippingCompany = 1;
UPDATE Incidents SET ShippingCompanyId = (SELECT Id FROM ShippingCompanies WHERE Name = 'チャーター') WHERE ShippingCompany = 2;
UPDATE Incidents SET ShippingCompanyId = (SELECT Id FROM ShippingCompanies WHERE Name = 'A運輸') WHERE ShippingCompany = 3;
UPDATE Incidents SET ShippingCompanyId = (SELECT Id FROM ShippingCompanies WHERE Name = 'B急便') WHERE ShippingCompany = 4;

PRINT '既存データの移行が完了しました。';
GO

-- 3. 移行結果の確認
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
GO

-- 4. 外部キー制約の追加
ALTER TABLE Incidents ADD CONSTRAINT FK_Incidents_TroubleTypes 
    FOREIGN KEY (TroubleTypeId) REFERENCES TroubleTypes(Id);
ALTER TABLE Incidents ADD CONSTRAINT FK_Incidents_DamageTypes 
    FOREIGN KEY (DamageTypeId) REFERENCES DamageTypes(Id);
ALTER TABLE Incidents ADD CONSTRAINT FK_Incidents_Warehouses 
    FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id);
ALTER TABLE Incidents ADD CONSTRAINT FK_Incidents_ShippingCompanies 
    FOREIGN KEY (ShippingCompanyId) REFERENCES ShippingCompanies(Id);

PRINT '外部キー制約が追加されました。';
GO

-- 5. 古いEnumカラムの削除
ALTER TABLE Incidents DROP COLUMN TroubleType;
ALTER TABLE Incidents DROP COLUMN DamageType;
ALTER TABLE Incidents DROP COLUMN Warehouse;
ALTER TABLE Incidents DROP COLUMN ShippingCompany;

PRINT '古いEnumカラムが削除されました。';
GO

-- 6. 新しいカラムをNOT NULLに変更
ALTER TABLE Incidents ALTER COLUMN TroubleTypeId INT NOT NULL;
ALTER TABLE Incidents ALTER COLUMN DamageTypeId INT NOT NULL;
ALTER TABLE Incidents ALTER COLUMN WarehouseId INT NOT NULL;
ALTER TABLE Incidents ALTER COLUMN ShippingCompanyId INT NOT NULL;

PRINT '新しいカラムがNOT NULLに変更されました。';
GO

-- 7. 最終確認
SELECT 
    'Incidents' as TableName,
    COUNT(*) as TotalRecords,
    COUNT(TroubleTypeId) as TroubleTypeRecords,
    COUNT(DamageTypeId) as DamageTypeRecords,
    COUNT(WarehouseId) as WarehouseRecords,
    COUNT(ShippingCompanyId) as ShippingCompanyRecords
FROM Incidents;
GO

PRINT 'Incidentsテーブルの更新が完了しました。';
GO
