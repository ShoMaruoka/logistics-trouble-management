# 倉庫担当機能拡張 データベース更新手順書

**作成日**: 2025-09-09  
**作成者**: システム開発チーム  
**バージョン**: 1.0

## 概要

本ドキュメントは、物流トラブル管理システムの倉庫担当機能拡張に必要なデータベース更新手順を説明します。UsersテーブルにWarehouseIdカラムを追加し、倉庫担当ユーザーと倉庫の関連付けを実現します。

## 前提条件

- SQL Server 2022が起動していること
- LogisticsTroubleManagementデータベースが存在すること
- 既存のテーブル（Users、Warehouses、Roles等）が存在すること
- 認証機能用テーブル（Roles、RefreshTokens）が作成済みであること

## 実行手順

### 1. 倉庫担当機能用データベース更新

```bash
# SQL Serverに接続してスクリプトを実行
sqlcmd -S localhost -d LogisticsTroubleManagement -i database/scripts/12_AddWarehouseIdToUsers.sql
```

**実行内容:**
- UsersテーブルにWarehouseIdカラムの追加
- 外部キー制約の設定（FK_Users_Warehouse）
- パフォーマンス用インデックスの作成
- 既存倉庫担当ユーザーの倉庫割り当て
- データ整合性の確認

## 作成・更新されるテーブル構造

### Usersテーブル（更新後）
| カラム名 | データ型 | 説明 |
|----------|----------|------|
| Id | INT | 主キー（既存） |
| Username | NVARCHAR(100) | ユーザー名（既存） |
| Email | NVARCHAR(100) | メールアドレス（既存） |
| Role | INT | ロールID（既存、外部キー） |
| IsActive | BIT | 有効フラグ（既存） |
| PasswordHash | NVARCHAR(MAX) | パスワードハッシュ（既存） |
| LastLoginAt | DATETIME2 | 最終ログイン日時（既存） |
| TokenVersion | INT | トークンバージョン（既存） |
| CreatedAt | DATETIME2 | 作成日時（既存） |
| UpdatedAt | DATETIME2 | 更新日時（既存） |
| **WarehouseId** | **INT** | **担当倉庫ID（新規追加）** |

### 外部キー制約
- **FK_Users_Warehouse**: Users.WarehouseId → Warehouses.Id

### インデックス
- **IX_Users_WarehouseId**: WarehouseId（NULL以外の値のみ）

## データ整合性ルール

### 1. 倉庫担当ユーザーの制約
- **Role = 3（倉庫担当）**: WarehouseIdは必須（NOT NULL）
- **その他のロール**: WarehouseIdは任意（NULL許可）

### 2. 外部キー制約
- WarehouseIdは存在する倉庫IDのみ許可
- 倉庫削除時はSET NULL（倉庫担当の倉庫設定をクリア）

### 3. データ移行
- 既存の倉庫担当ユーザーには最初の有効な倉庫を自動割り当て
- 実際の運用では管理者が適切な倉庫に再設定

## 確認方法

### 1. テーブル構造の確認
```sql
-- WarehouseIdカラムの確認
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'WarehouseId';

-- 外部キー制約の確認
SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTableName,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumnName
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE fk.name = 'FK_Users_Warehouse';

-- インデックスの確認
SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    c.name AS ColumnName
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE i.name = 'IX_Users_WarehouseId';
```

### 2. データの確認
```sql
-- 倉庫担当ユーザーの確認
SELECT 
    u.Id,
    u.Username,
    u.Email,
    r.Name AS RoleName,
    u.WarehouseId,
    w.Name AS WarehouseName
FROM Users u
LEFT JOIN Roles r ON u.Role = r.Id
LEFT JOIN Warehouses w ON u.WarehouseId = w.Id
WHERE u.Role = 3  -- 倉庫担当ロール
ORDER BY u.Id;

-- 倉庫が未設定の倉庫担当ユーザーをチェック
SELECT 
    Id, Username, Email
FROM Users 
WHERE Role = 3 AND WarehouseId IS NULL AND IsActive = 1;
```

## トラブルシューティング

### よくある問題と対処法

#### 1. 外部キー制約エラー
**エラー**: `The ALTER TABLE statement conflicted with the FOREIGN KEY constraint`
**原因**: 既存データに無効な倉庫IDが含まれている
**対処法**: 
```sql
-- 無効な倉庫IDの確認
SELECT DISTINCT WarehouseId 
FROM Users 
WHERE WarehouseId IS NOT NULL 
  AND WarehouseId NOT IN (SELECT Id FROM Warehouses);

-- 無効な倉庫IDをNULLに設定
UPDATE Users 
SET WarehouseId = NULL 
WHERE WarehouseId IS NOT NULL 
  AND WarehouseId NOT IN (SELECT Id FROM Warehouses);
```

#### 2. インデックス作成エラー
**エラー**: `Cannot create the index. The following SET options have incorrect settings`
**対処法**: 
```sql
-- セッション設定の確認・修正
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
```

#### 3. 倉庫担当ユーザーの倉庫未設定
**問題**: 倉庫担当ユーザーに倉庫が設定されていない
**対処法**:
```sql
-- 倉庫担当ユーザーに倉庫を手動設定
UPDATE Users 
SET WarehouseId = @WarehouseId  -- 適切な倉庫IDを指定
WHERE Id = @UserId;  -- 対象ユーザーIDを指定
```

## バックアップとロールバック

### バックアップ
```sql
-- 実行前のバックアップ
BACKUP DATABASE LogisticsTroubleManagement 
TO DISK = 'C:\Backup\LogisticsTroubleManagement_BeforeWarehouseId.bak'
WITH FORMAT, INIT;
```

### ロールバック
```sql
-- WarehouseIdカラムの削除（注意: データが失われます）
ALTER TABLE Users DROP CONSTRAINT FK_Users_Warehouse;
DROP INDEX IX_Users_WarehouseId ON Users;
ALTER TABLE Users DROP COLUMN WarehouseId;
```

## 次のステップ

1. **Entity Framework Coreモデルの更新**
   - UserエンティティにWarehouseIdプロパティを追加
   - ナビゲーションプロパティの追加
   - DbContextの更新

2. **バックエンドAPIの実装**
   - 倉庫担当専用APIエンドポイントの実装
   - 担当倉庫のインシデント取得API
   - ロール別データアクセス制御

3. **フロントエンドの実装**
   - 倉庫担当専用ダッシュボードの実装
   - 4つの表示項目の実装
   - ロール別UI制御

## 注意事項

- 本スクリプトは開発環境での実行を想定しています
- 本番環境での実行前には、必ずバックアップを取得してください
- 既存の倉庫担当ユーザーには一時的に最初の倉庫が割り当てられます
- 実際の運用では、管理者が適切な倉庫に再設定してください
- 倉庫担当ユーザーのWarehouseIdは必須です（NULL不可）

---

**文書履歴**

| 日付 | バージョン | 変更内容 | 変更者 |
|------|------------|----------|--------|
| 2025-09-09 | 1.0 | 初版作成 | システム開発チーム |
