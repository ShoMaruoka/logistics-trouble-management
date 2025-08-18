# デプロイメント

## 概要
物流トラブル管理システムのデプロイメント設定

## 環境構成
```
deployment/
├── docker/                     # Docker関連ファイル
│   ├── Dockerfile.backend     # バックエンド用Dockerfile
│   ├── Dockerfile.frontend    # フロントエンド用Dockerfile
│   └── docker-compose.prod.yml # 本番用Docker Compose
├── iis/                       # IIS設定
│   ├── web.config            # Web.config
│   └── applicationHost.config # ApplicationHost.config
├── scripts/                   # デプロイスクリプト
│   ├── deploy.ps1            # PowerShellデプロイスクリプト
│   ├── backup.ps1            # バックアップスクリプト
│   └── rollback.ps1          # ロールバックスクリプト
└── config/                    # 環境別設定
    ├── appsettings.Development.json
    ├── appsettings.Staging.json
    └── appsettings.Production.json
```

## デプロイメント環境

### 開発環境
- **OS**: Windows 11
- **Web Server**: IIS Express / Kestrel
- **Database**: SQL Server 2022 (Docker)
- **Frontend**: Next.js Development Server

### ステージング環境
- **OS**: Windows Server 2019/2022
- **Web Server**: IIS 10
- **Database**: SQL Server 2022
- **Frontend**: Next.js Static Export

### 本番環境
- **OS**: Windows Server 2019/2022
- **Web Server**: IIS 10
- **Database**: SQL Server 2022
- **Frontend**: Next.js Static Export
- **Load Balancer**: Application Request Routing (ARR)

## デプロイメント手順

### 1. ビルド
```bash
# バックエンドのビルド
dotnet build backend/src/LogisticsTroubleManagement.API -c Release

# フロントエンドのビルド
cd frontend
npm run build
npm run export
```

### 2. デプロイ
```powershell
# PowerShellスクリプトの実行
.\deployment\scripts\deploy.ps1 -Environment Production
```

### 3. データベースマイグレーション
```bash
# 本番データベースへのマイグレーション
dotnet ef database update --project backend/src/LogisticsTroubleManagement.Infrastructure --startup-project backend/src/LogisticsTroubleManagement.API
```

## 監視・ログ

### Application Insights
- パフォーマンス監視
- エラー追跡
- ユーザー行動分析
- 依存関係マップ

### ログ設定
- Serilog設定
- 構造化ログ
- ログローテーション
- ログアグリゲーション

## バックアップ戦略

### データベースバックアップ
- フルバックアップ: 日次
- 差分バックアップ: 4時間毎
- ログバックアップ: 15分毎
- 保持期間: 30日

### アプリケーションバックアップ
- 設定ファイル
- 静的ファイル
- ログファイル
- 証明書

## セキュリティ

### SSL/TLS
- HTTPS強制
- HSTS設定
- 証明書管理
- 暗号化設定

### ファイアウォール
- ポート制限
- IP制限
- アプリケーションファイアウォール
- DDoS対策

## 障害対応

### ロールバック手順
```powershell
# ロールバックスクリプトの実行
.\deployment\scripts\rollback.ps1 -Version 1.0.0
```

### 緊急時対応
- 障害検知
- 自動復旧
- 手動復旧手順
- 連絡体制

## パフォーマンス最適化

### フロントエンド
- 静的ファイルの最適化
- CDN利用
- キャッシュ戦略
- 画像最適化

### バックエンド
- データベースクエリ最適化
- キャッシュ戦略
- 非同期処理
- 負荷分散
