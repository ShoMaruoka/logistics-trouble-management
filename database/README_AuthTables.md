# 認証機能用テーブル作成手順書

**作成日**: 2025-09-03  
**作成者**: システム開発チーム  
**バージョン**: 1.0

## 概要

本ドキュメントは、物流トラブル管理システムの認証機能実装に必要なデータベーステーブルの作成手順を説明します。

## 前提条件

- SQL Server 2022が起動していること
- LogisticsTroubleManagementデータベースが存在すること
- 既存のテーブル（Users、Incidents等）が存在すること

## 実行手順

### 1. 認証機能用テーブルの作成

```bash
# SQL Serverに接続してスクリプトを実行
sqlcmd -S localhost -d LogisticsTroubleManagement -i database/scripts/10_CreateAuthTables.sql
```

**実行内容:**
- Rolesテーブルの作成（ロール管理用）
- Usersテーブルの拡張（パスワードハッシュ、最終ログイン日時、トークンバージョン）
- RefreshTokensテーブルの作成（リフレッシュトークン管理用）
- 外部キー制約の追加
- インデックスの作成・最適化

### 2. 初期データの投入

```bash
# 初期データの投入
sqlcmd -S localhost -d LogisticsTroubleManagement -i database/scripts/11_InsertAuthData.sql
```

**投入内容:**
- 4つのロール（事務員、インシデント管理者、倉庫担当、システム管理者）
- システム管理者ユーザー（admin）
- テスト用ユーザー（clerk1、manager1、warehouse1）
- 既存インシデントデータの担当者設定

## 作成されるテーブル構造

### Rolesテーブル
| カラム名 | データ型 | 説明 |
|----------|----------|------|
| Id | INT | 主キー（自動採番） |
| Name | NVARCHAR(50) | ロール名（一意） |
| Description | NVARCHAR(200) | ロールの説明 |
| IsActive | BIT | 有効フラグ |
| CreatedAt | DATETIME2 | 作成日時 |
| UpdatedAt | DATETIME2 | 更新日時 |

### Usersテーブル（拡張後）
| カラム名 | データ型 | 説明 |
|----------|----------|------|
| Id | INT | 主キー（既存） |
| Username | NVARCHAR(100) | ユーザー名（既存） |
| Email | NVARCHAR(100) | メールアドレス（既存） |
| Role | INT | ロールID（既存、外部キー） |
| IsActive | BIT | 有効フラグ（既存） |
| CreatedAt | DATETIME2 | 作成日時（既存） |
| UpdatedAt | DATETIME2 | 更新日時（既存） |
| **PasswordHash** | **NVARCHAR(MAX)** | **パスワードハッシュ（新規）** |
| **LastLoginAt** | **DATETIME2** | **最終ログイン日時（新規）** |
| **TokenVersion** | **INT** | **トークンバージョン（新規）** |

### RefreshTokensテーブル
| カラム名 | データ型 | 説明 |
|----------|----------|------|
| Id | BIGINT | 主キー（自動採番） |
| UserId | INT | ユーザーID（外部キー） |
| TokenHash | VARBINARY(64) | トークンハッシュ |
| ExpiresAt | DATETIME2 | 有効期限 |
| CreatedAt | DATETIME2 | 作成日時 |
| RevokedAt | DATETIME2 | 失効日時 |
| ReplacedByTokenHash | VARBINARY(64) | 置換トークンハッシュ |
| Reason | NVARCHAR(100) | 失効理由 |
| UserAgent | NVARCHAR(200) | ユーザーエージェント |
| IpAddress | NVARCHAR(45) | IPアドレス |

## 初期データ

### ロール
1. **Clerk** (事務員) - 基本的なインシデント管理
2. **IncidentManager** (インシデント管理者) - インシデント分類・管理
3. **WarehouseStaff** (倉庫担当) - 解決策検討・登録
4. **Admin** (システム管理者) - 全機能・ユーザー管理

### テストユーザー
- **admin** (Admin123!) - システム管理者
- **clerk1** (Test123!) - 事務員
- **manager1** (Test123!) - インシデント管理者
- **warehouse1** (Test123!) - 倉庫担当

## 確認方法

### 1. テーブル作成の確認
```sql
-- テーブルの存在確認
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('Roles', 'RefreshTokens');

-- カラムの確認
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;
```

### 2. データ投入の確認
```sql
-- ロールデータの確認
SELECT * FROM Roles;

-- ユーザーデータの確認
SELECT u.Username, u.Email, r.Name AS RoleName, u.IsActive
FROM Users u
JOIN Roles r ON u.Role = r.Id;

-- インシデントデータの確認
SELECT i.Title, u.Username AS CreatedBy
FROM Incidents i
LEFT JOIN Users u ON i.CreatedByUserId = u.Id;
```

## トラブルシューティング

### よくある問題と対処法

#### 1. 外部キー制約エラー
**エラー**: `The ALTER TABLE statement conflicted with the FOREIGN KEY constraint`
**原因**: 既存データに無効なロールIDが含まれている
**対処法**: 
```sql
-- 無効なロールIDの確認
SELECT DISTINCT Role FROM Users WHERE Role NOT IN (SELECT Id FROM Roles);

-- 必要に応じてロールIDを修正
UPDATE Users SET Role = 1 WHERE Role NOT IN (SELECT Id FROM Roles);
```

#### 2. パスワードハッシュの設定
**注意**: 初期データのパスワードハッシュは仮の値です
**対処法**: 実際の認証システム実装時に、適切なハッシュ化処理で更新してください

#### 3. インデックス作成エラー
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

## バックアップとロールバック

### バックアップ
```sql
-- 実行前のバックアップ
BACKUP DATABASE LogisticsTroubleManagement 
TO DISK = 'C:\Backup\LogisticsTroubleManagement_BeforeAuth.bak'
WITH FORMAT, INIT;
```

### ロールバック
```sql
-- テーブルの削除（注意: データが失われます）
DROP TABLE RefreshTokens;
ALTER TABLE Users DROP COLUMN PasswordHash, LastLoginAt, TokenVersion;
DROP TABLE Roles;
```

## 次のステップ

1. **Entity Framework Core設定の更新**
   - 新しいエンティティクラスの作成
   - DbContextの更新
   - マイグレーションの作成

2. **認証サービスの実装**
   - JWT認証サービスの実装
   - パスワードハッシュ化サービスの実装

3. **APIコントローラーの更新**
   - 認証・認可属性の追加
   - ユーザーコンテキストの取得

## 注意事項

- 本スクリプトは開発環境での実行を想定しています
- 本番環境での実行前には、必ずバックアップを取得してください
- パスワードハッシュは仮の値です。実際の認証システム実装時に更新してください
- 既存データがある場合は、データの整合性を確認してから実行してください

---

**文書履歴**

| 日付 | バージョン | 変更内容 | 変更者 |
|------|------------|----------|--------|
| 2025-09-03 | 1.0 | 初版作成 | システム開発チーム |
