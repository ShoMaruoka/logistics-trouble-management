-- ID=0の「未設定」レコードをマスタテーブルに追加
-- 事務員が登録したインシデントで外部キー制約エラーを回避するため

USE LTMDB;
GO

-- Warehousesテーブルに未設定レコードを追加
SET IDENTITY_INSERT Warehouses ON;

-- 既存のID=0レコードがあれば削除（重複回避）
IF EXISTS (SELECT 1 FROM Warehouses WHERE Id = 0)
BEGIN
    DELETE FROM Warehouses WHERE Id = 0;
END

INSERT INTO Warehouses (Id, Name, Location, CreatedAt, UpdatedAt) 
VALUES (0, '未設定', '未設定', GETDATE(), GETDATE());

SET IDENTITY_INSERT Warehouses OFF;
GO

-- ShippingCompaniesテーブルに未設定レコードを追加
SET IDENTITY_INSERT ShippingCompanies ON;

-- 既存のID=0レコードがあれば削除（重複回避）
IF EXISTS (SELECT 1 FROM ShippingCompanies WHERE Id = 0)
BEGIN
    DELETE FROM ShippingCompanies WHERE Id = 0;
END

INSERT INTO ShippingCompanies (Id, Name, ContactInfo, CreatedAt, UpdatedAt) 
VALUES (0, '未設定', '未設定', GETDATE(), GETDATE());

SET IDENTITY_INSERT ShippingCompanies OFF;
GO

-- TroubleTypesテーブルにも未設定レコードを追加（念のため）
SET IDENTITY_INSERT TroubleTypes ON;

IF EXISTS (SELECT 1 FROM TroubleTypes WHERE Id = 0)
BEGIN
    DELETE FROM TroubleTypes WHERE Id = 0;
END

INSERT INTO TroubleTypes (Id, Name, Description, CreatedAt, UpdatedAt) 
VALUES (0, '未設定', '分類待ち', GETDATE(), GETDATE());

SET IDENTITY_INSERT TroubleTypes OFF;
GO

-- DamageTypesテーブルにも未設定レコードを追加（念のため）
SET IDENTITY_INSERT DamageTypes ON;

IF EXISTS (SELECT 1 FROM DamageTypes WHERE Id = 0)
BEGIN
    DELETE FROM DamageTypes WHERE Id = 0;
END

INSERT INTO DamageTypes (Id, Name, Description, CreatedAt, UpdatedAt) 
VALUES (0, '未設定', '分類待ち', GETDATE(), GETDATE());

SET IDENTITY_INSERT DamageTypes OFF;
GO

PRINT '未設定マスタデータの挿入が完了しました。';
