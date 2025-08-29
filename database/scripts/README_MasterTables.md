# マスタ管理機能拡張 データベーススクリプト実行手順

## 概要
物流トラブル管理システムのマスタ管理機能拡張に必要なデータベース変更を実行する手順書です。

## 対象スクリプト
1. `06_CreateMasterTables.sql` - マスタテーブルの作成
2. `07_InsertMasterData.sql` - 初期データの投入
3. `08_UpdateIncidentsTable.sql` - Incidentsテーブルの更新

## 実行前の確認事項

### 1. データベースのバックアップ
```sql
-- 実行前に必ずバックアップを取得してください
BACKUP DATABASE LogisticsTroubleManagement 
TO DISK = 'C:\Backup\LogisticsTroubleManagement_BeforeMasterUpdate.bak'
WITH FORMAT, INIT, NAME = 'LogisticsTroubleManagement-Full Database Backup';
```

### 2. 現在のデータ状態確認
```sql
-- 現在のIncidentsテーブルの構造確認
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Incidents' 
ORDER BY ORDINAL_POSITION;

-- 現在のデータ件数確認
SELECT COUNT(*) as TotalIncidents FROM Incidents;
```

## 実行手順

### ステップ1: マスタテーブルの作成
```sql
-- SQL Server Management StudioまたはAzure Data Studioで実行
-- ファイル: 06_CreateMasterTables.sql
```

**実行内容:**
- TroubleTypesテーブルの作成
- DamageTypesテーブルの作成
- Warehousesテーブルの作成
- ShippingCompaniesテーブルの作成
- インデックスと制約の設定

**確認方法:**
```sql
-- テーブルが作成されたか確認
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('TroubleTypes', 'DamageTypes', 'Warehouses', 'ShippingCompanies');

-- 各テーブルの構造確認
EXEC sp_help 'TroubleTypes';
EXEC sp_help 'DamageTypes';
EXEC sp_help 'Warehouses';
EXEC sp_help 'ShippingCompanies';
```

### ステップ2: 初期データの投入
```sql
-- ファイル: 07_InsertMasterData.sql
```

**実行内容:**
- トラブル種類の初期データ（10件）
- 損傷種類の初期データ（8件）
- 出荷元倉庫の初期データ（4件）
- 運送会社の初期データ（5件）

**確認方法:**
```sql
-- 各テーブルのデータ件数確認
SELECT 'TroubleTypes' as TableName, COUNT(*) as RecordCount FROM TroubleTypes
UNION ALL
SELECT 'DamageTypes', COUNT(*) FROM DamageTypes
UNION ALL
SELECT 'Warehouses', COUNT(*) FROM Warehouses
UNION ALL
SELECT 'ShippingCompanies', COUNT(*) FROM ShippingCompanies;

-- データ内容の確認
SELECT * FROM TroubleTypes ORDER BY SortOrder;
SELECT * FROM DamageTypes ORDER BY SortOrder;
SELECT * FROM Warehouses ORDER BY SortOrder;
SELECT * FROM ShippingCompanies ORDER BY SortOrder;
```

### ステップ3: Incidentsテーブルの更新
```sql
-- ファイル: 08_UpdateIncidentsTable.sql
```

**実行内容:**
1. 新しいマスタIDカラムの追加
2. 既存データの移行（Enum値からマスタIDへの変換）
3. 外部キー制約の追加
4. 古いEnumカラムの削除
5. 新しいカラムをNOT NULLに変更

**確認方法:**
```sql
-- 移行後のデータ確認
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
    fk.name as ConstraintName,
    OBJECT_NAME(fk.parent_object_id) as TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) as ColumnName,
    OBJECT_NAME(fk.referenced_object_id) as ReferencedTableName
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) = 'Incidents';
```

## 実行時の注意事項

### 1. 実行順序の厳守
スクリプトは必ず以下の順序で実行してください：
1. `06_CreateMasterTables.sql`
2. `07_InsertMasterData.sql`
3. `08_UpdateIncidentsTable.sql`

### 2. トランザクション管理
各スクリプトは独立したトランザクションとして実行されます。
エラーが発生した場合は、該当スクリプトの実行を中止し、問題を解決してから再実行してください。

### 3. 権限の確認
実行ユーザーに以下の権限があることを確認してください：
- CREATE TABLE
- ALTER TABLE
- INSERT
- UPDATE
- DELETE
- CREATE INDEX
- REFERENCES

### 4. データ整合性の確認
移行後、以下の点を必ず確認してください：
- 全インシデントレコードが正しく移行されている
- 外部キー制約が正しく設定されている
- 既存のアプリケーションが正常に動作する

## トラブルシューティング

### よくある問題と対処法

#### 1. 外部キー制約エラー
```sql
-- 制約を一時的に無効化
ALTER TABLE Incidents NOCHECK CONSTRAINT ALL;

-- データ修正後、制約を再有効化
ALTER TABLE Incidents CHECK CONSTRAINT ALL;
```

#### 2. データ移行の不整合
```sql
-- 移行されていないレコードの確認
SELECT * FROM Incidents WHERE TroubleTypeId IS NULL;

-- 手動での移行
UPDATE Incidents SET TroubleTypeId = 1 WHERE Id = [対象レコードID];
```

#### 3. ロールバックが必要な場合
```sql
-- バックアップからの復元
RESTORE DATABASE LogisticsTroubleManagement 
FROM DISK = 'C:\Backup\LogisticsTroubleManagement_BeforeMasterUpdate.bak'
WITH REPLACE;
```

## 実行後の確認事項

### 1. アプリケーションの動作確認
- インシデント一覧の表示
- インシデントの新規作成
- インシデントの編集
- 統計情報の表示

### 2. パフォーマンスの確認
```sql
-- クエリ実行時間の確認
SET STATISTICS TIME ON;
SELECT * FROM Incidents i
INNER JOIN TroubleTypes tt ON i.TroubleTypeId = tt.Id
INNER JOIN DamageTypes dt ON i.DamageTypeId = dt.Id
INNER JOIN Warehouses w ON i.WarehouseId = w.Id
INNER JOIN ShippingCompanies sc ON i.ShippingCompanyId = sc.Id;
SET STATISTICS TIME OFF;
```

### 3. ログの確認
- アプリケーションログ
- データベースログ
- エラーログ

## 連絡先
問題が発生した場合や、追加のサポートが必要な場合は、開発チームまでご連絡ください。

---

**作成日**: 2025-08-29  
**作成者**: システム開発チーム  
**バージョン**: 1.0
