# UpdatedAtフィールドNULL許可化 実行手順書

## 概要
このドキュメントは、物流トラブル管理システムのデータベースにおいて、`UpdatedAt`フィールドをNULL許可に変更するための手順を説明します。

## 背景
- **問題**: データベーススキーマでは`UpdatedAt`フィールドが`NOT NULL DEFAULT GETUTCDATE()`として定義されているが、DTOでは`DateTime`（非nullable）として定義されている
- **解決策**: データベーススキーマを修正して`UpdatedAt`フィールドをNULL許可に変更し、初期レコードにNULL値を設定可能にする

## 対象テーブル
以下のテーブルの`UpdatedAt`フィールドが対象となります：

1. **Incidents** - トラブル管理テーブル
2. **Users** - ユーザーテーブル
3. **Attachments** - 添付ファイルテーブル
4. **AuditLogs** - 監査ログテーブル
5. **Effectiveness** - 効果測定テーブル
6. **TroubleTypes** - トラブル種類マスタ
7. **DamageTypes** - 損傷種類マスタ
8. **Warehouses** - 出荷元倉庫マスタ
9. **ShippingCompanies** - 運送会社マスタ

## 実行手順

### 1. 事前準備
- データベースのバックアップを取得
- アプリケーションを停止
- データベース接続の確認

### 2. スクリプトの実行
```sql
-- データベース更新スクリプトを実行
-- ファイル: 09_UpdateUpdatedAtNullable.sql
USE LogisticsTroubleManagement;
GO

-- スクリプトの内容を確認後、実行
```

### 3. 実行結果の確認
スクリプト実行後、以下のメッセージが表示されることを確認：
```
UpdatedAtフィールドのNULL許可化を開始します...
IncidentsテーブルのUpdatedAtフィールドをNULL許可に変更しました。
UsersテーブルのUpdatedAtフィールドをNULL許可に変更しました。
...
UpdatedAtフィールドのNULL許可化が完了しました。
```

### 4. データベース構造の確認
```sql
-- 各テーブルのUpdatedAtフィールドの設定を確認
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    IS_NULLABLE,
    DATA_TYPE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE COLUMN_NAME = 'UpdatedAt' 
    AND TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME;
```

期待される結果：
- `IS_NULLABLE`: YES
- `COLUMN_DEFAULT`: NULL（または未設定）

## 初期データの更新（オプション）

必要に応じて、初期レコードの`UpdatedAt`フィールドをNULLに設定できます：

```sql
-- 09_UpdateUpdatedAtNullable.sqlの該当部分のコメントアウトを解除
UPDATE [dbo].[Incidents] SET [UpdatedAt] = NULL WHERE [UpdatedAt] = [CreatedAt];
UPDATE [dbo].[Users] SET [UpdatedAt] = NULL WHERE [UpdatedAt] = [CreatedAt];
-- 他のテーブルも同様
```

## 注意事項

### 実行前の確認事項
- データベースのバックアップが取得済みであること
- アプリケーションが停止していること
- 十分なディスク容量があること

### 実行後の確認事項
- アプリケーションが正常に起動することを確認
- 既存機能が正常に動作することを確認
- 新規レコード作成時に`UpdatedAt`フィールドが適切に処理されることを確認

### ロールバック手順
問題が発生した場合のロールバック手順：

```sql
-- 各テーブルのUpdatedAtフィールドを元に戻す
ALTER TABLE [dbo].[Incidents] ALTER COLUMN [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE();
ALTER TABLE [dbo].[Users] ALTER COLUMN [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE();
-- 他のテーブルも同様
```

## 技術的詳細

### 変更内容
- `UpdatedAt`フィールドの制約を`NOT NULL DEFAULT GETUTCDATE()`から`NULL`に変更
- Entity Framework Core設定ファイルの更新
- データベーススクリプトの更新

### 影響範囲
- 新規テーブル作成時：`UpdatedAt`フィールドがNULL許可で作成される
- 既存データ：`UpdatedAt`フィールドの値は保持される
- アプリケーション：`UpdatedAt`フィールドがNULLの場合の処理が必要

## トラブルシューティング

### よくある問題と対処法

1. **権限エラー**
   - データベース管理者権限で実行していることを確認
   - 適切なロールに所属していることを確認

2. **テーブルが存在しない**
   - 対象テーブルが存在することを確認
   - テーブル名のスペルを確認

3. **アプリケーションエラー**
   - Entity Framework Core設定ファイルが更新されていることを確認
   - アプリケーションの再起動を実行

## 関連ドキュメント
- [テーブル仕様書](../docs/テーブル仕様書.md)
- [ER図](../docs/ER図.md)
- [API仕様書](../docs/API仕様書.md)

## 更新履歴
- **2025-09-02**: 初版作成
- **作成者**: システム開発チーム
