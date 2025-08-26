# 更新されたデータベーススクリプト

## 概要

このディレクトリには、現在のLTMDBデータベースの状態に合わせて更新されたスクリプトファイルが含まれています。

## ファイル一覧

### 1. `02_CreateTables_Updated.sql`
現在のデータベースの状態に合わせて更新されたテーブル作成スクリプト

**主な変更点:**
- Users テーブル: PhoneNumberフィールドの追加、Roleフィールドのデータ型をintに変更
- Incidents テーブル: 15個の拡張フィールドを追加（DamageType, EffectivenessStatus, ShippingCompany等）
- Attachments テーブル: CreatedAt, UpdatedAtフィールドの追加
- Effectiveness テーブル: BeforeValue, AfterValue, ImprovementRateフィールドの追加
- AuditLogs テーブル: IncidentIdフィールドの追加、UpdatedAtフィールドの追加

### 2. `03_CreateIndexes_Updated.sql`
更新されたテーブル構造に対応したインデックス作成スクリプト

**主な変更点:**
- 新しく追加されたフィールドに対するインデックスの追加
- 複合インデックスの追加（パフォーマンス向上のため）
- 物流トラブル管理に特化した検索最適化

### 3. `05_SeedData_Updated.sql`
拡張されたテーブル構造に対応したサンプルデータ作成スクリプト

**主な変更点:**
- 拡張フィールドを含むサンプルデータの追加
- より現実的な物流トラブル事例の追加
- 効果測定データの詳細化

## 実行手順

### 前提条件
- SQL Server 2022 (Docker) が起動していること
- LTMDBデータベースが存在すること
- 適切な権限を持つユーザーで接続できること

### 実行順序
1. **テーブル作成**
   ```sql
   sqlcmd -S localhost,1434 -U sa -P "Medicalsv@6087!!" -d LTMDB -i 02_CreateTables_Updated.sql
   ```

2. **インデックス作成**
   ```sql
   sqlcmd -S localhost,1434 -U sa -P "Medicalsv@6087!!" -d LTMDB -i 03_CreateIndexes_Updated.sql
   ```

3. **サンプルデータ作成**
   ```sql
   sqlcmd -S localhost,1434 -U sa -P "Medicalsv@6087!!" -d LTMDB -i 05_SeedData_Updated.sql
   ```

### 注意事項
- 既存のデータがある場合は、バックアップを取得してから実行してください
- スクリプトは冪等性を保つように設計されています（IF NOT EXISTS を使用）
- 外部キー制約が含まれているため、テーブルの作成順序に注意してください

## テーブル構造の詳細

### Users テーブル
- 基本情報: Id, Username, Email, FirstName, LastName
- 権限・状態: Role (int), IsActive
- 連絡先: PhoneNumber
- 監査: CreatedAt, UpdatedAt

### Incidents テーブル
- 基本情報: Title, Description, Status, Priority, Category
- 担当者: ReportedById, AssignedToId
- 日時: ReportedDate, ResolvedDate, OccurrenceDate, EffectivenessDate
- 詳細情報: DamageType, EffectivenessStatus, ShippingCompany, TroubleType, Warehouse
- 原因・対策: Cause, PreventionMeasures
- 効果測定: EffectivenessComment, IncidentDetails, Summary
- 数量: DefectiveItems, TotalShipments
- 監査: CreatedAt, UpdatedAt

### Attachments テーブル
- ファイル情報: FileName, FilePath, FileSize, ContentType
- 関連情報: IncidentId, UploadedById
- 日時: UploadedAt, CreatedAt, UpdatedAt

### Effectiveness テーブル
- 測定情報: EffectivenessType, BeforeValue, AfterValue, ImprovementRate
- 詳細: Description
- 担当者: MeasuredById
- 日時: MeasuredAt, CreatedAt, UpdatedAt

### AuditLogs テーブル
- 操作情報: Action, TableName, RecordId
- 変更内容: OldValues, NewValues
- アクセス情報: IpAddress, UserAgent
- 関連情報: UserId, IncidentId
- 日時: CreatedAt, UpdatedAt

## トラブルシューティング

### よくある問題
1. **権限エラー**: saユーザーでログインできない場合は、パスワードを確認してください
2. **接続エラー**: Dockerコンテナが起動しているか確認してください
3. **外部キーエラー**: テーブルの作成順序を確認してください

### ログの確認
各スクリプトの実行結果は、PRINT文で出力されます。エラーが発生した場合は、詳細なログを確認してください。

## 更新履歴

- **2025-08-25**: 現在のDB状態に合わせてスクリプトを完全更新
- 拡張フィールドの追加
- インデックスの最適化
- サンプルデータの充実化
