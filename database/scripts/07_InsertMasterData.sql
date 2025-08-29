-- 物流トラブル管理システム マスタ管理機能拡張
-- 初期データ投入スクリプト
-- 作成日: 2025-08-29
-- 作成者: システム開発チーム

USE LogisticsTroubleManagement;
GO

-- 1. TroubleTypes初期データ
INSERT INTO TroubleTypes (Name, Description, Color, SortOrder) VALUES
(N'商品トラブル', N'商品自体に関するトラブル', '#EF4444', 1),
(N'配送トラブル', N'配送プロセスに関するトラブル', '#F97316', 2),
(N'システムトラブル', N'システム・ITに関するトラブル', '#8B5CF6', 3),
(N'設備トラブル', N'設備・機械に関するトラブル', '#06B6D4', 4),
(N'品質トラブル', N'品質に関するトラブル', '#10B981', 5),
(N'物流トラブル', N'物流プロセスに関するトラブル', '#3B82F6', 6),
(N'管理トラブル', N'管理・運営に関するトラブル', '#F59E0B', 7),
(N'安全トラブル', N'安全に関するトラブル', '#84CC16', 8),
(N'環境トラブル', N'環境に関するトラブル', '#22C55E', 9),
(N'その他のトラブル', N'その他のカテゴリに該当しないトラブル', '#6B7280', 10);

-- 2. DamageTypes初期データ
INSERT INTO DamageTypes (Name, Description, Category, SortOrder) VALUES
(N'該当なし', N'損傷・問題なし', 'General', 1),
(N'誤出荷', N'誤った商品の出荷', 'Product', 2),
(N'早着・延着', N'配送の時間に関する問題', 'Delivery', 3),
(N'紛失', N'商品の紛失', 'Product', 4),
(N'誤配送', N'誤った場所への配送', 'Delivery', 5),
(N'破損・汚損', N'商品の破損・汚損', 'Product', 6),
(N'その他の配送ミス', N'その他の配送に関する問題', 'Delivery', 7),
(N'その他の商品事故', N'その他の商品に関する問題', 'Product', 8);

-- 3. Warehouses初期データ
INSERT INTO Warehouses (Name, Description, Location, SortOrder) VALUES
(N'該当なし', N'倉庫に関係ない', NULL, 1),
(N'A倉庫', N'A倉庫からの出荷', N'東京都○○区', 2),
(N'B倉庫', N'B倉庫からの出荷', N'大阪府○○市', 3),
(N'C倉庫', N'C倉庫からの出荷', N'愛知県○○市', 4);

-- 4. ShippingCompanies初期データ
INSERT INTO ShippingCompanies (Name, Description, CompanyType, SortOrder) VALUES
(N'該当なし', N'運送会社に関係ない', 'Internal', 1),
(N'庫内', N'自社庫内での処理', 'Internal', 2),
(N'チャーター', N'チャーター便の利用', 'External', 3),
(N'A運輸', N'A運輸会社の利用', 'External', 4),
(N'B急便', N'B急便の利用', 'External', 5);

PRINT 'マスタデータの投入が完了しました。';
GO

-- 投入されたデータの確認
SELECT 'TroubleTypes' as TableName, COUNT(*) as RecordCount FROM TroubleTypes
UNION ALL
SELECT 'DamageTypes', COUNT(*) FROM DamageTypes
UNION ALL
SELECT 'Warehouses', COUNT(*) FROM Warehouses
UNION ALL
SELECT 'ShippingCompanies', COUNT(*) FROM ShippingCompanies;
GO
