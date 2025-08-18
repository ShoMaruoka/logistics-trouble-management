# 物流トラブル管理システム 技術スタック仕様書

## 概要

本ドキュメントは、物流トラブル管理システムの技術スタックについて定義するものです。Windows Server環境での安定稼働を前提として、ASP.NET Core 8.0 LTSをバックエンド、Next.js 15.4.6をフロントエンドとして採用します。

## システム構成

### 全体アーキテクチャ
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │   Backend       │    │   Database      │
│   (Next.js)     │◄──►│   (ASP.NET Core)│◄──►│   (SQL Server)  │
│   React 19.1.0  │    │   .NET 8.0 LTS  │    │   2022          │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## フロントエンド技術スタック

### 主要技術
- **フレームワーク**: Next.js 15.4.6
- **UI ライブラリ**: React 19.1.0
- **言語**: TypeScript 5
- **スタイリング**: Tailwind CSS 4
- **UI コンポーネント**: Radix UI
- **アイコン**: Lucide React
- **グラフ**: Recharts

### 開発環境
- **Node.js**: 18.0.0以上
- **パッケージマネージャー**: npm
- **開発サーバー**: Next.js Dev Server (Turbopack)

### 主要依存関係
```json
{
  "next": "15.4.6",
  "react": "19.1.0",
  "react-dom": "19.1.0",
  "typescript": "^5",
  "tailwindcss": "^4",
  "@radix-ui/react-dialog": "^1.1.14",
  "@radix-ui/react-label": "^2.1.7",
  "@radix-ui/react-select": "^2.2.5",
  "@radix-ui/react-tabs": "^1.1.12",
  "lucide-react": "^0.537.0",
  "recharts": "^3.1.2"
}
```

## バックエンド技術スタック

### 主要技術
- **フレームワーク**: ASP.NET Core 8.0 LTS
- **言語**: C# 12
- **ORM**: Entity Framework Core 8.0
- **認証**: JWT Bearer Token
- **API仕様**: OpenAPI 3.0 (Swagger)
- **ログ**: Serilog
- **設定管理**: Microsoft.Extensions.Configuration

### 開発環境
- **.NET SDK**: 8.0.0以上
- **IDE**: Visual Studio 2022 / Visual Studio Code
- **データベース**: SQL Server 2022 Developer Edition

### 推奨プロジェクト構成
```
LogisticsTroubleManagement/
├── LogisticsTroubleManagement.API/          # Web API プロジェクト
├── LogisticsTroubleManagement.Core/         # ビジネスロジック層
├── LogisticsTroubleManagement.Infrastructure/ # データアクセス層
├── LogisticsTroubleManagement.Domain/       # ドメインモデル層
└── LogisticsTroubleManagement.Tests/        # テストプロジェクト
```

### 主要NuGetパッケージ
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.0" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.0.0" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
```

## データベース技術スタック

### 主要技術
- **RDBMS**: Microsoft SQL Server 2022
- **ORM**: Entity Framework Core 8.0
- **マイグレーション**: EF Core Migrations
- **接続プール**: SQL Server Connection Pooling

### データベース設計方針
- **正規化**: 第3正規形まで適用
- **インデックス**: 適切なインデックス設計
- **ストアドプロシージャ**: 複雑なクエリに使用
- **トランザクション**: ACID特性の保証

### 主要テーブル設計例
```sql
-- 物流トラブル管理テーブル
CREATE TABLE Incidents (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Status NVARCHAR(50) NOT NULL,
    Priority NVARCHAR(20) NOT NULL,
    Category NVARCHAR(100),
    Location NVARCHAR(200),
    AssignedTo NVARCHAR(100),
    CreatedBy NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    ResolvedAt DATETIME2 NULL
);

-- 添付ファイルテーブル
CREATE TABLE Attachments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IncidentId INT NOT NULL,
    FileName NVARCHAR(255) NOT NULL,
    FilePath NVARCHAR(500) NOT NULL,
    FileSize BIGINT NOT NULL,
    ContentType NVARCHAR(100),
    UploadedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (IncidentId) REFERENCES Incidents(Id)
);
```

## インフラストラクチャ

### ホスティング環境
- **OS**: Windows Server 2022
- **Webサーバー**: IIS 10.0
- **アプリケーションプール**: .NET CLR Version "No Managed Code"
- **SSL証明書**: 社内証明書またはLet's Encrypt

### システム要件
- **CPU**: 4コア以上
- **メモリ**: 8GB以上
- **ストレージ**: SSD 100GB以上
- **ネットワーク**: 1Gbps以上

### セキュリティ要件
- **認証**: Windows認証 + JWT
- **認可**: Role-based Access Control (RBAC)
- **暗号化**: TLS 1.3
- **ファイアウォール**: Windows Firewall設定

## 開発・運用ツール

### 開発ツール
- **IDE**: Visual Studio 2022 / Cursor
- **バージョン管理**: Git
- **CI/CD**: Azure DevOps / GitHub Actions
- **コード品質**: SonarQube / StyleCop

### 監視・ログ
- **アプリケーション監視**: Application Insights
- **ログ管理**: Serilog + ELK Stack
- **パフォーマンス監視**: PerfView
- **データベース監視**: SQL Server Profiler

### テスト戦略
- **単体テスト**: xUnit
- **統合テスト**: TestServer
- **E2Eテスト**: Playwright
- **パフォーマンステスト**: NBomber

## デプロイメント

### デプロイ方式
- **開発環境**: ローカル開発サーバー
- **ステージング環境**: IIS + アプリケーションプール
- **本番環境**: IIS + アプリケーションプール

### デプロイ手順
1. **ビルド**: `dotnet build --configuration Release`
2. **テスト**: `dotnet test`
3. **パッケージ**: `dotnet publish`
4. **デプロイ**: IISへの配置

### 環境設定
```json
// appsettings.Production.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;Trusted_Connection=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## パフォーマンス要件

### レスポンス時間
- **API応答**: 500ms以内
- **ページ読み込み**: 2秒以内
- **データベースクエリ**: 100ms以内

### スループット
- **同時接続数**: 100ユーザー
- **リクエスト/秒**: 1000 req/s
- **データベース接続**: 50接続

## セキュリティ要件

### 認証・認可
- **認証方式**: JWT Bearer Token
- **トークン有効期限**: 24時間
- **リフレッシュトークン**: 7日間
- **パスワードポリシー**: 複雑性要件

### データ保護
- **暗号化**: AES-256
- **ハッシュ**: bcrypt
- **HTTPS**: 必須
- **CORS**: 適切な設定

## 保守・運用

### バックアップ戦略
- **データベース**: 日次フルバックアップ + 1時間間隔ログバックアップ
- **アプリケーション**: 設定ファイルのバックアップ
- **ログ**: 30日間保持

### 監視項目
- **アプリケーション**: レスポンス時間、エラー率
- **データベース**: 接続数、クエリ実行時間
- **サーバー**: CPU、メモリ、ディスク使用率
- **ネットワーク**: 帯域幅使用率

### 障害対応
- **自動復旧**: アプリケーションプールの自動再起動
- **手動復旧**: 手順書に基づく復旧作業
- **エスカレーション**: 障害レベルに応じた報告体制

## バージョン管理

### サポート期間
- **ASP.NET Core 8.0 LTS**: 2026年11月まで
- **.NET 8.0 LTS**: 2026年11月まで
- **SQL Server 2022**: 2033年7月まで

### アップグレード計画
- **年次レビュー**: 技術スタックの見直し
- **セキュリティアップデート**: 月次適用
- **機能アップデート**: 四半期ごと検討

---

**文書作成日**: 2024年12月
**バージョン**: 1.0
**作成者**: 開発チーム
