# インシデント管理ワークフロー改修 - データベース変更手順書

## 概要
新しい6段階ワークフロー（未分類→未対応→対応中→対応済→再発防止策提案済→有効性確認済）を実装するため、Incidentsテーブルに新しいフィールドを追加します。

## 実行スクリプト
- `13_AddWorkflowFields.sql`

## 変更内容

### 追加されるフィールド

| フィールド名 | データ型 | NULL許可 | 説明 |
|-------------|----------|----------|------|
| WorkflowStatus | INT | YES | 新ワークフローステータス（1-6） |
| DueDate | DATETIME2 | YES | 対応期限 |
| ResponseStartDate | DATETIME2 | YES | 対応開始日 |
| CauseAnalysisDate | DATETIME2 | YES | 原因入力日 |
| CompletionDate | DATETIME2 | YES | 対応完了日 |
| PreventionProposalDate | DATETIME2 | YES | 再発防止策提案日 |
| EffectivenessConfirmationDate | DATETIME2 | YES | 有効性確認日 |
| ResponseContent | NVARCHAR(2000) | YES | 対応内容 |

### WorkflowStatus値の定義

| 値 | 名前 | 説明 |
|----|------|------|
| 1 | Unclassified | 未分類 - インシデントが登録されたが、まだ分類されていない |
| 2 | Pending | 未対応 - 分類は完了したが、まだ対応が開始されていない |
| 3 | InProgress | 対応中 - 対応が開始され、現在進行中 |
| 4 | Completed | 対応済 - 対応は完了したが、再発防止策がまだ提案されていない |
| 5 | PreventionProposed | 再発防止策提案済 - 再発防止策が提案され、有効性確認待ち |
| 6 | EffectivenessConfirmed | 有効性確認済 - 再発防止策の有効性が確認され、ワークフロー完了 |
| NULL | (レガシーモード) | 既存の5段階ステータスを使用 |

### 追加される制約
- `CK_Incidents_WorkflowStatus`: WorkflowStatusが1-6の範囲内またはNULLであることを保証

### 追加されるインデックス
- `IX_Incidents_WorkflowStatus`: WorkflowStatusでの検索用
- `IX_Incidents_DueDate`: DueDateでの検索用  
- `IX_Incidents_ResponseStartDate`: ResponseStartDateでの検索用
- `IX_Incidents_CompletionDate`: CompletionDateでの検索用

## 実行手順

### 1. 事前確認
```sql
-- 現在のIncidentsテーブル構造を確認
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Incidents'
ORDER BY ORDINAL_POSITION;

-- 現在のレコード数を確認
SELECT COUNT(*) AS CurrentIncidentCount FROM Incidents;
```

### 2. バックアップ作成
```sql
-- 実行前に必ずバックアップを作成
BACKUP DATABASE LogisticsTroubleManagement 
TO DISK = 'C:\Backup\LogisticsTroubleManagement_PreWorkflowFields_20250919.bak'
WITH FORMAT, INIT, NAME = 'Pre-Workflow Fields Backup';
```

### 3. スクリプト実行
```bash
# SQL Server Management Studio または sqlcmd で実行
sqlcmd -S localhost -d LogisticsTroubleManagement -i 13_AddWorkflowFields.sql
```

### 4. 実行後確認
```sql
-- 新しいフィールドが追加されたことを確認
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Incidents' 
AND COLUMN_NAME IN (
    'WorkflowStatus', 'DueDate', 'ResponseStartDate', 
    'CauseAnalysisDate', 'CompletionDate', 'PreventionProposalDate',
    'EffectivenessConfirmationDate', 'ResponseContent'
)
ORDER BY ORDINAL_POSITION;

-- CHECK制約が追加されたことを確認
SELECT CONSTRAINT_NAME, CHECK_CLAUSE
FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS 
WHERE TABLE_NAME = 'Incidents'
AND CONSTRAINT_NAME = 'CK_Incidents_WorkflowStatus';

-- インデックスが追加されたことを確認
SELECT name, type_desc, is_unique, has_filter, filter_definition
FROM sys.indexes 
WHERE object_id = OBJECT_ID('Incidents')
AND name LIKE 'IX_Incidents_%Workflow%' 
OR name LIKE 'IX_Incidents_%Due%'
OR name LIKE 'IX_Incidents_%Response%'
OR name LIKE 'IX_Incidents_%Completion%';

-- データの整合性確認
SELECT 
    COUNT(*) AS TotalIncidents,
    COUNT(WorkflowStatus) AS WithWorkflowStatus,
    COUNT(*) - COUNT(WorkflowStatus) AS WithoutWorkflowStatus
FROM Incidents;
```

## 重要な注意事項

### 後方互換性
- **既存データは完全に保護されます** - すべての新フィールドはNULL許可
- **既存機能への影響なし** - WorkflowStatusがNULLの場合は従来の5段階ステータスを使用
- **段階的移行** - 新ワークフローは必要に応じて有効化

### パフォーマンス考慮事項
- 新しいインデックスにより、ワークフローステータスや日付での検索が高速化
- フィルタ付きインデックスによりストレージ効率を最適化
- 既存クエリのパフォーマンスへの影響は最小限

### セキュリティ
- CHECK制約により不正なWorkflowStatus値を防止
- トランザクション内での実行によりデータ整合性を保証

## ロールバック手順

万が一問題が発生した場合のロールバック手順：

```sql
-- 緊急時のロールバック（新フィールドとインデックスを削除）
BEGIN TRANSACTION;

-- インデックス削除
DROP INDEX IF EXISTS IX_Incidents_WorkflowStatus ON Incidents;
DROP INDEX IF EXISTS IX_Incidents_DueDate ON Incidents;
DROP INDEX IF EXISTS IX_Incidents_ResponseStartDate ON Incidents;
DROP INDEX IF EXISTS IX_Incidents_CompletionDate ON Incidents;

-- CHECK制約削除
ALTER TABLE Incidents DROP CONSTRAINT IF EXISTS CK_Incidents_WorkflowStatus;

-- 新フィールド削除
ALTER TABLE Incidents DROP COLUMN IF EXISTS WorkflowStatus;
ALTER TABLE Incidents DROP COLUMN IF EXISTS DueDate;
ALTER TABLE Incidents DROP COLUMN IF EXISTS ResponseStartDate;
ALTER TABLE Incidents DROP COLUMN IF EXISTS CauseAnalysisDate;
ALTER TABLE Incidents DROP COLUMN IF EXISTS CompletionDate;
ALTER TABLE Incidents DROP COLUMN IF EXISTS PreventionProposalDate;
ALTER TABLE Incidents DROP COLUMN IF EXISTS EffectivenessConfirmationDate;
ALTER TABLE Incidents DROP COLUMN IF EXISTS ResponseContent;

COMMIT TRANSACTION;
```

または、事前に作成したバックアップから復元：

```sql
RESTORE DATABASE LogisticsTroubleManagement 
FROM DISK = 'C:\Backup\LogisticsTroubleManagement_PreWorkflowFields_20250919.bak'
WITH REPLACE;
```

## 次のステップ

1. **Entity Framework Core設定更新**: Incidentエンティティの設定を更新
2. **API実装**: 新しいワークフローエンドポイントの実装
3. **フロントエンド対応**: 新しいワークフロー機能のUI実装
4. **テスト実行**: 既存機能への影響確認と新機能テスト

## 関連ドキュメント
- `docs/インシデント管理ワークフロー改修_変更概要仕様書.md`
- `todo.md` - フェーズ3.31の実装計画
