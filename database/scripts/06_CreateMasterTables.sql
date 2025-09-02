-- 物流トラブル管理システム マスタ管理機能拡張
-- マスタテーブル作成スクリプト
-- 作成日: 2025-08-29
-- 作成者: システム開発チーム

USE LogisticsTroubleManagement;
GO

-- 1. TroubleTypesテーブル（トラブル種類マスタ）
CREATE TABLE TroubleTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),           -- 自動採番（1から開始）
    Name NVARCHAR(100) NOT NULL,                -- 表示名（商品トラブル等）
    Description NVARCHAR(500) NULL,             -- 説明文
    Color NVARCHAR(7) NOT NULL DEFAULT '#3B82F6', -- 表示色（HEX）
    SortOrder INT NOT NULL DEFAULT 0,           -- 表示順序
    IsActive BIT NOT NULL DEFAULT 1,            -- 有効フラグ
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- 2. DamageTypesテーブル（損傷種類マスタ）
CREATE TABLE DamageTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),           -- 自動採番（1から開始）
    Name NVARCHAR(100) NOT NULL,                -- 表示名（該当なし、誤出荷等）
    Description NVARCHAR(500) NULL,             -- 説明文
    Category NVARCHAR(50) NOT NULL DEFAULT 'General', -- カテゴリ（General, Delivery, Product等）
    SortOrder INT NOT NULL DEFAULT 0,           -- 表示順序
    IsActive BIT NOT NULL DEFAULT 1,            -- 有効フラグ
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- 3. Warehousesテーブル（出荷元倉庫マスタ）
CREATE TABLE Warehouses (
    Id INT PRIMARY KEY IDENTITY(1,1),           -- 自動採番（1から開始）
    Name NVARCHAR(100) NOT NULL,                -- 表示名（該当なし、A倉庫等）
    Description NVARCHAR(500) NULL,             -- 説明文
    Location NVARCHAR(200) NULL,                -- 所在地
    ContactInfo NVARCHAR(200) NULL,             -- 連絡先情報
    SortOrder INT NOT NULL DEFAULT 0,           -- 表示順序
    IsActive BIT NOT NULL DEFAULT 1,            -- 有効フラグ
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- 4. ShippingCompaniesテーブル（運送会社マスタ）
CREATE TABLE ShippingCompanies (
    Id INT PRIMARY KEY IDENTITY(1,1),           -- 自動採番（1から開始）
    Name NVARCHAR(100) NOT NULL,                -- 表示名（該当なし、庫内等）
    Description NVARCHAR(500) NULL,             -- 説明文
    CompanyType NVARCHAR(50) NOT NULL DEFAULT 'External', -- 会社種別（Internal, External, Charter）
    ContactInfo NVARCHAR(200) NULL,             -- 連絡先情報
    SortOrder INT NOT NULL DEFAULT 0,           -- 表示順序
    IsActive BIT NOT NULL DEFAULT 1,            -- 有効フラグ
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- インデックスの作成
-- TroubleTypes
CREATE INDEX IX_TroubleTypes_IsActive_SortOrder ON TroubleTypes (IsActive, SortOrder);
CREATE INDEX IX_TroubleTypes_Name ON TroubleTypes (Name);

-- DamageTypes
CREATE INDEX IX_DamageTypes_IsActive_SortOrder ON DamageTypes (IsActive, SortOrder);
CREATE INDEX IX_DamageTypes_Category ON DamageTypes (Category);
CREATE INDEX IX_DamageTypes_Name ON DamageTypes (Name);

-- Warehouses
CREATE INDEX IX_Warehouses_IsActive_SortOrder ON Warehouses (IsActive, SortOrder);
CREATE INDEX IX_Warehouses_Name ON Warehouses (Name);

-- ShippingCompanies
CREATE INDEX IX_ShippingCompanies_IsActive_SortOrder ON ShippingCompanies (IsActive, SortOrder);
CREATE INDEX IX_ShippingCompanies_CompanyType ON ShippingCompanies (CompanyType);
CREATE INDEX IX_ShippingCompanies_Name ON ShippingCompanies (Name);

-- 一意制約の追加
ALTER TABLE TroubleTypes ADD CONSTRAINT UQ_TroubleTypes_Name UNIQUE (Name);
ALTER TABLE DamageTypes ADD CONSTRAINT UQ_DamageTypes_Name UNIQUE (Name);
ALTER TABLE Warehouses ADD CONSTRAINT UQ_Warehouses_Name UNIQUE (Name);
ALTER TABLE ShippingCompanies ADD CONSTRAINT UQ_ShippingCompanies_Name UNIQUE (Name);

PRINT N'マスタテーブルの作成が完了しました。';
GO
