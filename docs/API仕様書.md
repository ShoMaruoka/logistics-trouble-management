# 物流トラブル管理システム API仕様書

**作成日**: 2025-08-27  
**作成者**: システム開発チーム  
**バージョン**: 1.0

## 概要

本ドキュメントは、物流トラブル管理システムで提供されるREST APIの詳細仕様を定義します。各エンドポイントの目的、リクエスト・レスポンス形式、認証要件、エラーハンドリングについて説明します。

## 基本情報

### ベースURL
```
開発環境: http://localhost:5169
本番環境: https://api.logistics-trouble-management.com
```

### 共通仕様
- **認証方式**: 現在は認証なし（開発中）
- **データ形式**: JSON
- **文字エンコーディング**: UTF-8
- **HTTP バージョン**: HTTP/1.1
- **タイムゾーン**: UTC

### 共通レスポンスヘッダー
```
Content-Type: application/json; charset=utf-8
X-Request-ID: {UUID}
X-Response-Time: {ms}
```

### エラーレスポンス形式
```json
{
  "error": "エラーメッセージ",
  "details": "詳細情報（オプション）",
  "timestamp": "2025-08-27T11:16:47Z"
}
```

### HTTPステータスコード
- **200**: 成功
- **201**: 作成成功
- **400**: リクエストエラー
- **401**: 認証エラー
- **403**: 権限エラー
- **404**: リソースが見つからない
- **500**: サーバーエラー

---

## 1. インシデント管理API

### 1.1 インシデント一覧取得

**エンドポイント**: `GET /api/incidents`

**説明**: インシデントの一覧を取得します。検索条件、ソート、ページネーションに対応しています。

**クエリパラメータ**:

| パラメータ名 | 型 | 必須 | 説明 |
|--------------|----|------|------|
| searchTerm | string | 否 | 検索キーワード（タイトル、説明、カテゴリ） |
| status | int | 否 | ステータス（1:Open, 2:InProgress, 3:Resolved, 4:Closed, 5:Cancelled） |
| priority | int | 否 | 優先度（1:Low, 2:Medium, 3:High, 4:Critical） |
| category | string | 否 | カテゴリ |
| reportedById | int | 否 | 報告者ID |
| assignedToId | int | 否 | 担当者ID |
| fromDate | DateTime | 否 | 発生日（開始） |
| toDate | DateTime | 否 | 発生日（終了） |
| isOverdue | bool | 否 | 期限超過フラグ |
| page | int | 否 | ページ番号（デフォルト: 1） |
| pageSize | int | 否 | ページサイズ（デフォルト: 10） |
| sortBy | string | 否 | ソート項目（デフォルト: "reportedDate"） |
| ascending | bool | 否 | 昇順フラグ（デフォルト: false） |

**レスポンス**:

```json
{
  "items": [
    {
      "id": 1,
      "title": "商品破損トラブル",
      "description": "配送中の商品破損が発生",
      "status": 1,
      "priority": 3,
      "category": "品質トラブル",
      "troubleType": 0,
      "damageType": 4,
      "warehouse": 1,
      "shippingCompany": 2,
      "effectivenessStatus": 0,
      "incidentDetails": "配送中に商品が破損",
      "totalShipments": 1000,
      "defectiveItems": 50,
      "occurrenceDate": "2025-08-27T00:00:00Z",
      "occurrenceLocation": "配送センター",
      "summary": "商品破損トラブル",
      "cause": "梱包不備",
      "preventionMeasures": "梱包強化",
      "effectivenessDate": null,
      "effectivenessComment": "",
      "reportedById": 1,
      "reportedByName": "田中太郎",
      "assignedToId": 2,
      "assignedToName": "佐藤次郎",
      "reportedDate": "2025-08-27T11:16:47Z",
      "resolvedDate": null,
      "resolution": null,
      "createdAt": "2025-08-27T11:16:47Z",
      "updatedAt": "2025-08-27T11:16:47Z",
      "attachmentCount": 2,
      "isOverdue": false,
      "resolutionTime": null
    }
  ],
  "totalCount": 11,
  "page": 1,
  "pageSize": 10,
  "totalPages": 2
}
```

**エラーレスポンス**:
- **400**: 検索条件が不正
- **500**: サーバーエラー

---

### 1.2 インシデント詳細取得

**エンドポイント**: `GET /api/incidents/{id}`

**説明**: 指定されたIDのインシデントの詳細情報を取得します。

**パスパラメータ**:
- `id`: インシデントID（int）

**レスポンス**: 1.1と同様のIncidentDto

**エラーレスポンス**:
- **404**: インシデントが見つからない
- **500**: サーバーエラー

---

### 1.3 インシデント作成

**エンドポイント**: `POST /api/incidents`

**説明**: 新しいインシデントを作成します。

**リクエストボディ**:

```json
{
  "title": "商品破損トラブル",
  "description": "配送中の商品破損が発生",
  "category": "品質トラブル",
  "troubleType": 0,
  "damageType": 4,
  "warehouse": 1,
  "shippingCompany": 2,
  "priority": 3,
  "incidentDetails": "配送中に商品が破損",
  "totalShipments": 1000,
  "defectiveItems": 50,
  "occurrenceDate": "2025-08-27T00:00:00Z",
  "occurrenceLocation": "配送センター",
  "summary": "商品破損トラブル",
  "cause": "梱包不備",
  "preventionMeasures": "梱包強化"
}
```

**レスポンス**: 作成されたインシデントの詳細（IncidentDto）

**エラーレスポンス**:
- **400**: バリデーションエラー
- **500**: サーバーエラー

---

### 1.4 インシデント更新

**エンドポイント**: `PUT /api/incidents/{id}`

**説明**: 指定されたIDのインシデントを更新します。

**パスパラメータ**:
- `id`: インシデントID（int）

**リクエストボディ**: 1.3と同様の形式（更新したい項目のみ）

**レスポンス**: 更新されたインシデントの詳細（IncidentDto）

**エラーレスポンス**:
- **400**: バリデーションエラー
- **404**: インシデントが見つからない
- **500**: サーバーエラー

---

### 1.5 インシデント削除

**エンドポイント**: `DELETE /api/incidents/{id}`

**説明**: 指定されたIDのインシデントを削除します。

**パスパラメータ**:
- `id`: インシデントID（int）

**レスポンス**: 204 No Content

**エラーレスポンス**:
- **404**: インシデントが見つからない
- **500**: サーバーエラー

---

## 2. 統計API

### 2.1 統計サマリー取得

**エンドポイント**: `GET /api/statistics/summary`

**説明**: インシデントの統計サマリーを取得します。

**クエリパラメータ**:

| パラメータ名 | 型 | 必須 | 説明 |
|--------------|----|------|------|
| year | int | 否 | 年度（0の場合は全期間） |
| month | int | 否 | 月（0の場合は年間、1-12の場合は月間） |
| totalShipments | int | 否 | 出荷総数（PPM計算用、デフォルト: 100000） |

**レスポンス**:

```json
{
  "totalIncidents": 11,
  "openCount": 8,
  "inProgressCount": 1,
  "resolvedCount": 1,
  "closedCount": 1,
  "criticalCount": 2,
  "highCount": 3,
  "mediumCount": 4,
  "lowCount": 2,
  "averageResolutionTime": 2.5,
  "ppm": 110.0
}
```

---

### 2.2 損傷種類別グラフデータ取得

**エンドポイント**: `GET /api/statistics/charts/damage-types`

**説明**: 損傷種類別のグラフデータを取得します。

**クエリパラメータ**: 2.1と同様

**レスポンス**:

```json
{
  "items": [
    {
      "label": "該当なし",
      "value": 5
    },
    {
      "label": "破損・汚損",
      "value": 3
    },
    {
      "label": "紛失",
      "value": 2
    }
  ]
}
```

---

### 2.3 トラブル種類別グラフデータ取得

**エンドポイント**: `GET /api/statistics/charts/trouble-types`

**説明**: トラブル種類別のグラフデータを取得します。

**クエリパラメータ**: 2.1と同様

**レスポンス**: 2.2と同様の形式

---

### 2.4 月間発生件数グラフデータ取得

**エンドポイント**: `GET /api/statistics/charts/monthly`

**説明**: 月間発生件数のグラフデータを取得します。

**クエリパラメータ**: 2.1と同様

**レスポンス**:

```json
{
  "items": [
    {
      "label": "1月",
      "value": 2
    },
    {
      "label": "2月",
      "value": 1
    }
  ]
}
```

---

## 3. ファイル管理API

### 3.1 ファイル一覧取得

**エンドポイント**: `GET /api/attachments`

**説明**: 添付ファイルの一覧を取得します。

**クエリパラメータ**:

| パラメータ名 | 型 | 必須 | 説明 |
|--------------|----|------|------|
| incidentId | int | 否 | インシデントID |
| uploadedById | int | 否 | アップロード者ID |
| fileName | string | 否 | ファイル名 |
| contentType | string | 否 | MIMEタイプ |
| page | int | 否 | ページ番号 |
| pageSize | int | 否 | ページサイズ |
| sortBy | string | 否 | ソート項目 |
| ascending | bool | 否 | 昇順フラグ |

**レスポンス**:

```json
{
  "items": [
    {
      "id": 1,
      "incidentId": 1,
      "fileName": "破損写真.jpg",
      "filePath": "/uploads/incident1/photo1.jpg",
      "fileSize": 1024000,
      "contentType": "image/jpeg",
      "uploadedById": 1,
      "uploadedByName": "田中太郎",
      "uploadedAt": "2025-08-27T11:16:47Z",
      "createdAt": "2025-08-27T11:16:47Z",
      "updatedAt": "2025-08-27T11:16:47Z"
    }
  ],
  "totalCount": 5,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

---

### 3.2 ファイル詳細取得

**エンドポイント**: `GET /api/attachments/{id}`

**説明**: 指定されたIDのファイルの詳細情報を取得します。

**パスパラメータ**:
- `id`: ファイルID（int）

**レスポンス**: 3.1と同様のAttachmentDto

---

### 3.3 インシデント別ファイル取得

**エンドポイント**: `GET /api/attachments/incident/{incidentId}`

**説明**: 指定されたインシデントに関連するファイル一覧を取得します。

**パスパラメータ**:
- `incidentId`: インシデントID（int）

**レスポンス**: AttachmentDtoの配列

---

### 3.4 ファイルアップロード

**エンドポイント**: `POST /api/attachments`

**説明**: 新しいファイルをアップロードします。

**リクエスト**: multipart/form-data

**フォームデータ**:
- `file`: ファイル
- `incidentId`: インシデントID
- `description`: 説明（オプション）

**レスポンス**: アップロードされたファイルの詳細（AttachmentDto）

---

### 3.5 ファイル削除

**エンドポイント**: `DELETE /api/attachments/{id}`

**説明**: 指定されたIDのファイルを削除します。

**パスパラメータ**:
- `id`: ファイルID（int）

**レスポンス**: 204 No Content

---

## 4. 効果測定API

### 4.1 効果測定一覧取得

**エンドポイント**: `GET /api/effectiveness`

**説明**: 効果測定の一覧を取得します。

**クエリパラメータ**:

| パラメータ名 | 型 | 必須 | 説明 |
|--------------|----|------|------|
| incidentId | int | 否 | インシデントID |
| effectivenessType | string | 否 | 効果測定タイプ |
| measuredById | int | 否 | 測定者ID |
| page | int | 否 | ページ番号 |
| pageSize | int | 否 | ページサイズ |
| sortBy | string | 否 | ソート項目 |
| ascending | bool | 否 | 昇順フラグ |

**レスポンス**:

```json
{
  "items": [
    {
      "id": 1,
      "incidentId": 1,
      "effectivenessType": "COST_REDUCTION",
      "beforeValue": 1000000,
      "afterValue": 800000,
      "improvementRate": 20.0,
      "description": "梱包コストの削減",
      "measuredById": 1,
      "measuredByName": "田中太郎",
      "measuredAt": "2025-08-27T11:16:47Z",
      "createdAt": "2025-08-27T11:16:47Z",
      "updatedAt": "2025-08-27T11:16:47Z"
    }
  ],
  "totalCount": 3,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

---

### 4.2 効果測定詳細取得

**エンドポイント**: `GET /api/effectiveness/{id}`

**説明**: 指定されたIDの効果測定の詳細情報を取得します。

**パスパラメータ**:
- `id`: 効果測定ID（int）

**レスポンス**: 4.1と同様のEffectivenessDto

---

### 4.3 インシデント別効果測定取得

**エンドポイント**: `GET /api/effectiveness/incident/{incidentId}`

**説明**: 指定されたインシデントに関連する効果測定一覧を取得します。

**パスパラメータ**:
- `incidentId`: インシデントID（int）

**レスポンス**: EffectivenessDtoの配列

---

### 4.4 効果測定作成

**エンドポイント**: `POST /api/effectiveness`

**説明**: 新しい効果測定を作成します。

**リクエストボディ**:

```json
{
  "incidentId": 1,
  "effectivenessType": "COST_REDUCTION",
  "beforeValue": 1000000,
  "afterValue": 800000,
  "improvementRate": 20.0,
  "description": "梱包コストの削減"
}
```

**レスポンス**: 作成された効果測定の詳細（EffectivenessDto）

---

### 4.5 効果測定更新

**エンドポイント**: `PUT /api/effectiveness/{id}`

**説明**: 指定されたIDの効果測定を更新します。

**パスパラメータ**:
- `id`: 効果測定ID（int）

**リクエストボディ**: 4.4と同様の形式

**レスポンス**: 更新された効果測定の詳細（EffectivenessDto）

---

### 4.6 効果測定削除

**エンドポイント**: `DELETE /api/effectiveness/{id}`

**説明**: 指定されたIDの効果測定を削除します。

**パスパラメータ**:
- `id`: 効果測定ID（int）

**レスポンス**: 204 No Content

---

## 5. システム管理API

### 5.1 ヘルスチェック

**エンドポイント**: `GET /api/health`

**説明**: システムの健全性を確認します。

**レスポンス**:

```json
{
  "status": "Healthy",
  "timestamp": "2025-08-27T11:16:47Z",
  "version": "1.0.0",
  "environment": "Development"
}
```

---

### 5.2 サンプルデータ投入

**エンドポイント**: `POST /api/seed`

**説明**: 開発・テスト用のサンプルデータを投入します。

**レスポンス**:

```json
{
  "message": "サンプルデータの投入が完了しました。",
  "timestamp": "2025-08-27T11:16:47Z"
}
```

---

### 5.3 サンプルデータ投入状態確認

**エンドポイント**: `GET /api/seed/status`

**説明**: サンプルデータ投入エンドポイントの利用可能性を確認します。

**レスポンス**:

```json
{
  "message": "サンプルデータ投入エンドポイントが利用可能です。",
  "endpoint": "POST /api/seed",
  "timestamp": "2025-08-27T11:16:47Z"
}
```

---

## 6. 共通データ型

### 6.1 PagedResultDto<T>

ページネーション結果の共通形式です。

```json
{
  "items": [T],
  "totalCount": 0,
  "page": 1,
  "pageSize": 10,
  "totalPages": 0
}
```

### 6.2 ChartDataDto

グラフデータの共通形式です。

```json
{
  "items": [
    {
      "label": "string",
      "value": 0
    }
  ]
}
```

### 6.3 PieChartDataDto

円グラフデータの形式です（ChartDataDtoと同様）。

---

## 7. エラーハンドリング

### 7.1 バリデーションエラー

```json
{
  "error": "バリデーションエラー",
  "details": {
    "title": ["タイトルは必須です"],
    "occurrenceDate": ["発生日は必須です"]
  },
  "timestamp": "2025-08-27T11:16:47Z"
}
```

### 7.2 ビジネスルールエラー

```json
{
  "error": "インシデントは解決済みの状態でないとクローズできません",
  "timestamp": "2025-08-27T11:16:47Z"
}
```

### 7.3 システムエラー

```json
{
  "error": "内部サーバーエラーが発生しました",
  "timestamp": "2025-08-27T11:16:47Z"
}
```

---

## 8. 認証・認可（将来実装予定）

### 8.1 JWT認証

現在は認証なしで動作していますが、将来的に以下の認証方式を実装予定です：

- **認証方式**: JWT (JSON Web Token)
- **トークン有効期限**: 24時間
- **リフレッシュトークン**: 7日間

### 8.2 権限管理

- **User**: 自分のインシデントの閲覧・作成
- **Manager**: 担当インシデントの管理
- **Admin**: 全インシデント・ユーザーの管理

---

## 9. レート制限（将来実装予定）

### 9.1 API制限

- **認証なし**: 100リクエスト/分
- **認証済み**: 1000リクエスト/分
- **管理者**: 制限なし

### 9.2 ファイルアップロード制限

- **ファイルサイズ**: 最大10MB
- **ファイル形式**: 画像、PDF、Word、Excel、テキスト
- **アップロード頻度**: 10ファイル/分

---

## 10. 監査ログ

### 10.1 自動記録

以下の操作は自動的に監査ログに記録されます：

- インシデントの作成・更新・削除
- ファイルのアップロード・削除
- 効果測定の作成・更新・削除
- ユーザーのログイン・ログアウト

### 10.2 ログ項目

- 操作者ID
- 操作種別（CREATE, UPDATE, DELETE, LOGIN, LOGOUT）
- 操作対象テーブル
- 操作対象レコードID
- 変更前後の値
- IPアドレス
- ユーザーエージェント
- タイムスタンプ

---

## 11. パフォーマンス

### 11.1 レスポンス時間目標

- **単純なGET**: 100ms以内
- **検索・フィルタ**: 500ms以内
- **ファイルアップロード**: 2秒以内
- **統計計算**: 1秒以内

### 11.2 最適化手法

- データベースインデックスの活用
- クエリの最適化
- ページネーションの実装
- キャッシュの活用（将来実装予定）

---

## 12. セキュリティ

### 12.1 現在の状況

- 認証・認可は一時的に無効化
- CORS設定は開発環境向け
- 入力値のバリデーションは実装済み

### 12.2 将来実装予定

- JWT認証の実装
- HTTPS強制
- APIキー管理
- レート制限
- 入力値サニタイゼーション

---

## 13. 開発・テスト

### 13.1 開発環境

- **ベースURL**: http://localhost:5169
- **データベース**: SQL Server 2022 (Docker)
- **ログレベル**: Debug

### 13.2 テスト

- **単体テスト**: xUnit
- **統合テスト**: xUnit + TestServer
- **E2Eテスト**: Playwright

### 13.3 サンプルデータ

- インシデント: 11件
- ユーザー: 3件
- ファイル: 5件
- 効果測定: 3件

---

## 14. 今後の拡張予定

### 14.1 短期（1-2ヶ月）

- JWT認証の実装
- ユーザー管理API
- 通知システム

### 14.2 中期（3-6ヶ月）

- レポート機能
- データエクスポート
- モバイルアプリ対応

### 14.3 長期（6ヶ月以上）

- 機械学習による予測分析
- 外部システム連携
- クラウド対応

---

**文書履歴**

| 日付 | バージョン | 変更内容 | 変更者 |
|------|------------|----------|--------|
| 2025-08-27 | 1.0 | 初版作成 | システム開発チーム |
