# 物流トラブル管理システム 機能仕様書

## 概要

本ドキュメントは、物流トラブル管理システムの機能要件を定義するものです。既存のフロントエンド実装を基に、バックエンドAPIとデータベース設計を含む完全なシステム仕様を示します。

## システム概要

### 目的
物流業務におけるトラブル（商品トラブル・配送トラブル）を一元管理し、再発防止策の実施と効果測定を行うシステム

### 対象ユーザー
- 物流管理者
- 倉庫担当者
- 配送担当者
- 品質管理担当者

### 主要機能
1. トラブル情報の登録・編集・削除
2. トラブル一覧の表示・検索・ソート
3. 統計分析・ダッシュボード
4. 再発防止策の管理
5. 効果測定の実施・記録
6. データエクスポート（CSV）

## データモデル

### 1. トラブル情報（Incident）

#### 基本情報
| 項目名 | データ型 | 必須 | 説明 |
|--------|----------|------|------|
| id | string | ○ | トラブルID（自動採番） |
| troubleType | enum | ○ | トラブル種別（商品トラブル/配送トラブル） |
| damageType | string | ○ | 損害の種類 |
| location | string | ○ | 発生場所 |
| date | date | ○ | 発生日 |
| description | text | ○ | トラブル概要 |
| cause | text | ○ | 原因 |
| recurrencePreventionMeasures | text | ○ | 再発防止策 |
| processDescription | text | ○ | プロセス説明 |

#### 数量情報
| 項目名 | データ型 | 必須 | 説明 |
|--------|----------|------|------|
| shipmentVolume | int | × | 出荷総数 |
| defectiveUnits | int | × | 不良品数 |

#### 物流情報
| 項目名 | データ型 | 必須 | 説明 |
|--------|----------|------|------|
| shippingWarehouse | string | × | 出荷元倉庫 |
| shippingCompany | string | × | 運送会社名 |

#### 効果測定情報
| 項目名 | データ型 | 必須 | 説明 |
|--------|----------|------|------|
| effectivenessCheckStatus | enum | × | 有効性確認状況（未実施/実施中/実施） |
| effectivenessCheckDate | date | × | 有効性確認日 |
| effectivenessCheckComment | text | × | 有効性確認コメント |

#### 管理情報
| 項目名 | データ型 | 必須 | 説明 |
|--------|----------|------|------|
| createdBy | string | ○ | 作成者 |
| createdAt | datetime | ○ | 作成日時 |
| updatedBy | string | × | 更新者 |
| updatedAt | datetime | × | 更新日時 |

### 2. ユーザー情報（User）

| 項目名 | データ型 | 必須 | 説明 |
|--------|----------|------|------|
| id | string | ○ | ユーザーID |
| username | string | ○ | ユーザー名 |
| email | string | ○ | メールアドレス |
| passwordHash | string | ○ | パスワードハッシュ |
| role | enum | ○ | 権限（Admin/Manager/User） |
| department | string | × | 部署 |
| isActive | boolean | ○ | 有効フラグ |
| lastLoginAt | datetime | × | 最終ログイン日時 |
| createdAt | datetime | ○ | 作成日時 |
| updatedAt | datetime | × | 更新日時 |

### 3. 添付ファイル（Attachment）

| 項目名 | データ型 | 必須 | 説明 |
|--------|----------|------|------|
| id | string | ○ | ファイルID |
| incidentId | string | ○ | トラブルID（外部キー） |
| fileName | string | ○ | ファイル名 |
| filePath | string | ○ | ファイルパス |
| fileSize | long | ○ | ファイルサイズ（バイト） |
| contentType | string | ○ | MIMEタイプ |
| uploadedBy | string | ○ | アップロード者 |
| uploadedAt | datetime | ○ | アップロード日時 |

### 4. 監査ログ（AuditLog）

| 項目名 | データ型 | 必須 | 説明 |
|--------|----------|------|------|
| id | string | ○ | ログID |
| userId | string | ○ | ユーザーID |
| action | string | ○ | 実行アクション |
| entityType | string | ○ | 対象エンティティ |
| entityId | string | ○ | 対象ID |
| oldValues | json | × | 変更前値 |
| newValues | json | × | 変更後値 |
| ipAddress | string | × | IPアドレス |
| userAgent | string | × | ユーザーエージェント |
| createdAt | datetime | ○ | 作成日時 |

## 機能仕様

### 1. 認証・認可機能

#### 1.1 ユーザー認証
- **機能**: JWTベースの認証
- **入力**: ユーザー名/メールアドレス、パスワード
- **出力**: JWTトークン
- **有効期限**: 24時間
- **リフレッシュトークン**: 7日間

#### 1.2 権限管理
- **Admin**: 全機能利用可能、ユーザー管理
- **Manager**: トラブル管理、統計閲覧、効果測定
- **User**: トラブル登録・編集、閲覧

### 2. トラブル管理機能

#### 2.1 トラブル登録
- **機能**: 新しいトラブルの登録
- **入力項目**:
  - トラブル種別（必須）
  - 損害の種類（必須）
  - 発生場所（必須）
  - 発生日（必須）
  - トラブル概要（必須）
  - 原因（必須）
  - 再発防止策（必須）
  - プロセス説明（必須）
  - 出荷総数（任意）
  - 不良品数（任意）
  - 出荷元倉庫（任意）
  - 運送会社名（任意）
- **バリデーション**:
  - 必須項目の入力チェック
  - 日付形式の妥当性チェック
  - 数値項目の範囲チェック（0以上）

#### 2.2 トラブル編集
- **機能**: 既存トラブルの編集
- **制限**: 作成者またはManager以上のみ編集可能
- **監査**: 変更履歴の記録

#### 2.3 トラブル削除
- **機能**: トラブルの論理削除
- **制限**: Adminのみ削除可能
- **監査**: 削除履歴の記録

#### 2.4 トラブル一覧表示
- **機能**: トラブル一覧の表示
- **表示項目**:
  - 発生日
  - トラブル種別
  - 損害の種類
  - 出荷元倉庫
  - 運送会社名
  - 有効性確認状況
- **ソート機能**: 各項目での昇順・降順ソート
- **ページネーション**: 20件/ページ

#### 2.5 トラブル検索
- **機能**: キーワード検索
- **検索対象**: 全項目
- **検索方式**: 部分一致（大文字小文字区別なし）
- **フィルタ機能**:
  - 出荷元倉庫
  - 運送会社名
  - トラブル種別
  - 発生日範囲
  - 有効性確認状況

### 3. 効果測定機能

#### 3.1 効果測定登録
- **機能**: 再発防止策の効果測定
- **入力項目**:
  - 有効性確認状況
  - 有効性確認日
  - 有効性確認コメント
- **制限**: Manager以上のみ登録可能

#### 3.2 効果測定一覧
- **機能**: 効果測定状況の一覧表示
- **表示項目**:
  - トラブル概要
  - 再発防止策
  - 有効性確認状況
  - 有効性確認日
  - 有効性確認コメント

### 4. 統計分析機能

#### 4.1 ダッシュボード
- **機能**: 統計情報の表示
- **表示項目**:
  - 当月PPM（Parts Per Million）
  - 当月の商品トラブル件数
  - 当月の配送トラブル件数

#### 4.2 グラフ分析
- **損害の種類別分析**: 円グラフ
- **トラブル種別分析**: 円グラフ
- **月間発生件数**: 棒グラフ

#### 4.3 フィルタ機能
- **出荷元倉庫**: 全倉庫/個別倉庫
- **運送会社名**: 全社/個別会社

### 5. データエクスポート機能

#### 5.1 CSV出力
- **機能**: トラブル一覧のCSV出力
- **出力項目**: 全項目
- **ファイル名**: `物流トラブル管理_YYYYMMDD.csv`
- **文字エンコーディング**: UTF-8

### 6. ファイル管理機能

#### 6.1 ファイルアップロード
- **対応形式**: PDF, JPG, PNG, DOC, DOCX, XLS, XLSX
- **最大サイズ**: 10MB
- **保存先**: サーバー内ファイルシステム

#### 6.2 ファイル表示
- **機能**: 添付ファイルの表示・ダウンロード
- **制限**: トラブル閲覧権限を持つユーザーのみ

## API仕様

### 1. 認証API

#### POST /api/auth/login
```json
{
  "username": "string",
  "password": "string"
}
```

#### POST /api/auth/register
```json
{
  "username": "string",
  "email": "string",
  "password": "string",
  "department": "string"
}
```

#### POST /api/auth/refresh
```json
{
  "refreshToken": "string"
}
```

### 2. トラブル管理API

#### GET /api/incidents
- **クエリパラメータ**:
  - `page`: ページ番号
  - `size`: ページサイズ
  - `search`: 検索キーワード
  - `warehouse`: 出荷元倉庫
  - `company`: 運送会社名
  - `type`: トラブル種別
  - `status`: 有効性確認状況
  - `sortBy`: ソート項目
  - `sortOrder`: ソート順序

#### POST /api/incidents
```json
{
  "troubleType": "商品トラブル",
  "damageType": "破損・汚損",
  "location": "東京倉庫",
  "date": "2024-12-01",
  "description": "商品の破損",
  "cause": "輸送中の振動",
  "recurrencePreventionMeasures": "緩衝材の強化",
  "processDescription": "詳細なプロセス",
  "shipmentVolume": 1000,
  "defectiveUnits": 50,
  "shippingWarehouse": "A倉庫",
  "shippingCompany": "A運輸"
}
```

#### PUT /api/incidents/{id}
- トラブル情報の更新

#### DELETE /api/incidents/{id}
- トラブルの論理削除

#### GET /api/incidents/{id}
- トラブル詳細の取得

### 3. 効果測定API

#### PUT /api/incidents/{id}/effectiveness
```json
{
  "effectivenessCheckStatus": "実施",
  "effectivenessCheckDate": "2024-12-15",
  "effectivenessCheckComment": "対策が効果的だった"
}
```

### 4. 統計API

#### GET /api/statistics/dashboard
```json
{
  "ppm": "227",
  "productTroublesCount": 5,
  "deliveryTroublesCount": 3
}
```

#### GET /api/statistics/charts
```json
{
  "damageTypeData": [...],
  "troubleTypeData": [...],
  "monthlyTrendData": [...]
}
```

### 5. ファイル管理API

#### POST /api/incidents/{id}/attachments
- ファイルアップロード

#### GET /api/incidents/{id}/attachments
- 添付ファイル一覧取得

#### GET /api/attachments/{id}
- ファイルダウンロード

#### DELETE /api/attachments/{id}
- ファイル削除

## 画面仕様

### 1. ログイン画面
- **URL**: `/login`
- **機能**: ユーザー認証
- **入力項目**: ユーザー名/メールアドレス、パスワード

### 2. ダッシュボード画面
- **URL**: `/`
- **機能**: 統計情報とグラフの表示
- **コンポーネント**:
  - 統計カード（PPM、トラブル件数）
  - グラフタブ（損害の種類、トラブル種別、月間発生件数）
  - フィルタ（出荷元倉庫、運送会社名）

### 3. トラブル一覧画面
- **URL**: `/incidents`
- **機能**: トラブル一覧の表示・検索・ソート
- **コンポーネント**:
  - 検索ボックス
  - ソート可能なテーブル
  - ページネーション
  - CSV出力ボタン

### 4. トラブル登録・編集画面
- **URL**: `/incidents/new`, `/incidents/{id}/edit`
- **機能**: トラブル情報の登録・編集
- **コンポーネント**:
  - フォーム（全入力項目）
  - ファイルアップロード
  - 保存・キャンセルボタン

### 5. トラブル詳細画面
- **URL**: `/incidents/{id}`
- **機能**: トラブル詳細の表示
- **コンポーネント**:
  - 詳細情報表示
  - 添付ファイル一覧
  - 効果測定情報
  - 編集ボタン

## 非機能要件

### 1. パフォーマンス
- **API応答時間**: 500ms以内
- **ページ読み込み時間**: 2秒以内
- **同時接続数**: 100ユーザー

### 2. セキュリティ
- **認証**: JWT Bearer Token
- **認可**: Role-based Access Control
- **データ暗号化**: HTTPS必須
- **入力値検証**: サーバーサイドバリデーション

### 3. 可用性
- **稼働率**: 99.5%以上
- **バックアップ**: 日次フルバックアップ
- **障害復旧**: 4時間以内

### 4. 保守性
- **ログ出力**: 構造化ログ（Serilog）
- **監視**: Application Insights
- **エラーハンドリング**: グローバルエラーハンドリング

## データベース設計

### テーブル定義

#### Incidents
```sql
CREATE TABLE Incidents (
    Id NVARCHAR(450) PRIMARY KEY,
    TroubleType NVARCHAR(50) NOT NULL,
    DamageType NVARCHAR(100) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Date DATE NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Cause NVARCHAR(MAX) NOT NULL,
    RecurrencePreventionMeasures NVARCHAR(MAX) NOT NULL,
    ProcessDescription NVARCHAR(MAX) NOT NULL,
    ShipmentVolume INT NULL,
    DefectiveUnits INT NULL,
    ShippingWarehouse NVARCHAR(100) NULL,
    ShippingCompany NVARCHAR(100) NULL,
    EffectivenessCheckStatus NVARCHAR(20) NULL,
    EffectivenessCheckDate DATE NULL,
    EffectivenessCheckComment NVARCHAR(MAX) NULL,
    CreatedBy NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    IsDeleted BIT NOT NULL DEFAULT 0
);
```

#### Users
```sql
CREATE TABLE Users (
    Id NVARCHAR(450) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    Department NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    LastLoginAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL
);
```

#### Attachments
```sql
CREATE TABLE Attachments (
    Id NVARCHAR(450) PRIMARY KEY,
    IncidentId NVARCHAR(450) NOT NULL,
    FileName NVARCHAR(255) NOT NULL,
    FilePath NVARCHAR(500) NOT NULL,
    FileSize BIGINT NOT NULL,
    ContentType NVARCHAR(100) NOT NULL,
    UploadedBy NVARCHAR(100) NOT NULL,
    UploadedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (IncidentId) REFERENCES Incidents(Id)
);
```

#### AuditLogs
```sql
CREATE TABLE AuditLogs (
    Id NVARCHAR(450) PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    Action NVARCHAR(50) NOT NULL,
    EntityType NVARCHAR(50) NOT NULL,
    EntityId NVARCHAR(450) NOT NULL,
    OldValues NVARCHAR(MAX) NULL,
    NewValues NVARCHAR(MAX) NULL,
    IpAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
```

### インデックス
```sql
-- Incidents
CREATE INDEX IX_Incidents_Date ON Incidents(Date);
CREATE INDEX IX_Incidents_TroubleType ON Incidents(TroubleType);
CREATE INDEX IX_Incidents_ShippingWarehouse ON Incidents(ShippingWarehouse);
CREATE INDEX IX_Incidents_ShippingCompany ON Incidents(ShippingCompany);
CREATE INDEX IX_Incidents_CreatedBy ON Incidents(CreatedBy);
CREATE INDEX IX_Incidents_IsDeleted ON Incidents(IsDeleted);

-- Users
CREATE INDEX IX_Users_Username ON Users(Username);
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_Role ON Users(Role);

-- Attachments
CREATE INDEX IX_Attachments_IncidentId ON Attachments(IncidentId);

-- AuditLogs
CREATE INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);
CREATE INDEX IX_AuditLogs_EntityType_EntityId ON AuditLogs(EntityType, EntityId);
CREATE INDEX IX_AuditLogs_CreatedAt ON AuditLogs(CreatedAt);
```

---

**文書作成日**: 2025-08-12
**バージョン**: 1.0
**作成者**: 開発チーム
