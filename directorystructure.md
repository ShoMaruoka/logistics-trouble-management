# 物流トラブル管理システム ディレクトリ構成

## 概要

本ドキュメントは、物流トラブル管理システムの完全なディレクトリ構成を定義するものです。フロントエンド（Next.js）とバックエンド（ASP.NET Core）を含む全体の構造を示します。

## 全体ディレクトリ構成

```
物流トラブル管理システム/
├── frontend/                                    # フロントエンド（Next.js）
│   ├── .next/                                 # Next.jsビルドファイル
│   ├── node_modules/                          # Node.js依存関係
│   ├── public/                                # 静的ファイル
│   │   ├── file.svg
│   │   ├── globe.svg
│   │   ├── next.svg
│   │   ├── vercel.svg
│   │   └── window.svg
│   ├── src/                                   # ソースコード
│   │   ├── app/                               # Next.js App Router
│   │   │   ├── favicon.ico
│   │   │   ├── globals.css
│   │   │   ├── layout.tsx
│   │   │   └── page.tsx
│   │   ├── components/                        # Reactコンポーネント
│   │   │   ├── icons.tsx
│   │   │   ├── incident-form.tsx
│   │   │   ├── incident-list.tsx
│   │   │   └── ui/                            # UIコンポーネント
│   │   │       ├── button.tsx
│   │   │       ├── card.tsx
│   │   │       ├── chart.tsx
│   │   │       ├── dialog.tsx
│   │   │       ├── input.tsx
│   │   │       ├── label.tsx
│   │   │       ├── select.tsx
│   │   │       └── tabs.tsx
│   │   └── lib/                               # ユーティリティ
│   │       ├── types.ts
│   │       └── utils.ts
│   ├── .gitignore
│   ├── eslint.config.mjs
│   ├── next.config.ts
│   ├── next-env.d.ts
│   ├── package.json
│   ├── package-lock.json
│   ├── postcss.config.mjs
│   ├── README.md
│   ├── tsconfig.json
│   └── tsconfig.tsbuildinfo
├── backend/                                   # バックエンド（ASP.NET Core）
│   ├── LogisticsTroubleManagement.API/        # Web API プロジェクト
│   │   ├── Controllers/                       # APIコントローラー
│   │   │   ├── AuthController.cs
│   │   │   ├── IncidentsController.cs
│   │   │   ├── UsersController.cs
│   │   │   └── HealthController.cs
│   │   ├── Middleware/                        # カスタムミドルウェア
│   │   │   ├── ErrorHandlingMiddleware.cs
│   │   │   ├── JwtMiddleware.cs
│   │   │   └── LoggingMiddleware.cs
│   │   ├── Extensions/                        # 拡張メソッド
│   │   │   ├── ServiceCollectionExtensions.cs
│   │   │   └── ApplicationBuilderExtensions.cs
│   │   ├── Program.cs                         # アプリケーションエントリーポイント
│   │   ├── appsettings.json                   # アプリケーション設定
│   │   ├── appsettings.Development.json
│   │   ├── appsettings.Production.json
│   │   ├── LogisticsTroubleManagement.API.csproj
│   │   └── serilog.json                       # ログ設定
│   ├── LogisticsTroubleManagement.Core/       # ビジネスロジック層
│   │   ├── Services/                          # ビジネスサービス
│   │   │   ├── IIncidentService.cs
│   │   │   ├── IncidentService.cs
│   │   │   ├── IUserService.cs
│   │   │   ├── UserService.cs
│   │   │   ├── IAuthService.cs
│   │   │   └── AuthService.cs
│   │   ├── Interfaces/                        # サービスインターフェース
│   │   │   ├── IRepository.cs
│   │   │   ├── IUnitOfWork.cs
│   │   │   └── IEmailService.cs
│   │   ├── DTOs/                              # データ転送オブジェクト
│   │   │   ├── IncidentDto.cs
│   │   │   ├── CreateIncidentDto.cs
│   │   │   ├── UpdateIncidentDto.cs
│   │   │   ├── UserDto.cs
│   │   │   └── AuthDto.cs
│   │   ├── Mappers/                           # AutoMapper設定
│   │   │   ├── IncidentMapper.cs
│   │   │   ├── UserMapper.cs
│   │   │   └── AutoMapperProfile.cs
│   │   ├── Validators/                        # FluentValidation
│   │   │   ├── IncidentValidator.cs
│   │   │   ├── UserValidator.cs
│   │   │   └── AuthValidator.cs
│   │   ├── Exceptions/                        # カスタム例外
│   │   │   ├── BusinessException.cs
│   │   │   ├── NotFoundException.cs
│   │   │   └── ValidationException.cs
│   │   └── LogisticsTroubleManagement.Core.csproj
│   ├── LogisticsTroubleManagement.Infrastructure/ # データアクセス層
│   │   ├── Data/                              # データベースコンテキスト
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── Configurations/                # Entity Framework設定
│   │   │   │   ├── IncidentConfiguration.cs
│   │   │   │   ├── UserConfiguration.cs
│   │   │   │   └── AttachmentConfiguration.cs
│   │   │   └── Migrations/                    # EF Coreマイグレーション
│   │   │       ├── InitialCreate.cs
│   │   │       ├── AddUserTable.cs
│   │   │       └── AddAttachmentTable.cs
│   │   ├── Repositories/                      # リポジトリパターン
│   │   │   ├── IIncidentRepository.cs
│   │   │   ├── IncidentRepository.cs
│   │   │   ├── IUserRepository.cs
│   │   │   ├── UserRepository.cs
│   │   │   ├── IAttachmentRepository.cs
│   │   │   └── AttachmentRepository.cs
│   │   ├── Services/                          # インフラサービス
│   │   │   ├── EmailService.cs
│   │   │   ├── FileStorageService.cs
│   │   │   └── NotificationService.cs
│   │   ├── UnitOfWork/                        # ユニットオブワーク
│   │   │   ├── IUnitOfWork.cs
│   │   │   └── UnitOfWork.cs
│   │   └── LogisticsTroubleManagement.Infrastructure.csproj
│   ├── LogisticsTroubleManagement.Domain/     # ドメインモデル層
│   │   ├── Entities/                          # エンティティ
│   │   │   ├── BaseEntity.cs
│   │   │   ├── Incident.cs
│   │   │   ├── User.cs
│   │   │   ├── Attachment.cs
│   │   │   └── AuditLog.cs
│   │   ├── Enums/                             # 列挙型
│   │   │   ├── IncidentStatus.cs
│   │   │   ├── IncidentPriority.cs
│   │   │   ├── UserRole.cs
│   │   │   └── IncidentCategory.cs
│   │   ├── ValueObjects/                      # 値オブジェクト
│   │   │   ├── Email.cs
│   │   │   ├── PhoneNumber.cs
│   │   │   └── Location.cs
│   │   ├── Events/                            # ドメインイベント
│   │   │   ├── IncidentCreatedEvent.cs
│   │   │   ├── IncidentUpdatedEvent.cs
│   │   │   └── IncidentResolvedEvent.cs
│   │   └── LogisticsTroubleManagement.Domain.csproj
│   ├── LogisticsTroubleManagement.Tests/      # テストプロジェクト
│   │   ├── Unit/                              # 単体テスト
│   │   │   ├── Services/
│   │   │   │   ├── IncidentServiceTests.cs
│   │   │   │   ├── UserServiceTests.cs
│   │   │   │   └── AuthServiceTests.cs
│   │   │   ├── Validators/
│   │   │   │   ├── IncidentValidatorTests.cs
│   │   │   │   └── UserValidatorTests.cs
│   │   │   └── Mappers/
│   │   │       ├── IncidentMapperTests.cs
│   │   │       └── UserMapperTests.cs
│   │   ├── Integration/                       # 統合テスト
│   │   │   ├── Controllers/
│   │   │   │   ├── IncidentsControllerTests.cs
│   │   │   │   ├── UsersControllerTests.cs
│   │   │   │   └── AuthControllerTests.cs
│   │   │   └── Repositories/
│   │   │       ├── IncidentRepositoryTests.cs
│   │   │       └── UserRepositoryTests.cs
│   │   ├── TestHelpers/                       # テストヘルパー
│   │   │   ├── TestDatabase.cs
│   │   │   ├── TestDataBuilder.cs
│   │   │   └── MockHelper.cs
│   │   └── LogisticsTroubleManagement.Tests.csproj
│   ├── LogisticsTroubleManagement.sln         # ソリューションファイル
│   └── README.md                              # バックエンド説明
├── database/                                  # データベース関連
│   ├── scripts/                               # SQLスクリプト
│   │   ├── 01_CreateDatabase.sql
│   │   ├── 02_CreateTables.sql
│   │   ├── 03_CreateIndexes.sql
│   │   ├── 04_CreateStoredProcedures.sql
│   │   ├── 05_SeedData.sql
│   │   └── 06_CreateViews.sql
│   ├── migrations/                            # 手動マイグレーション
│   │   ├── 001_InitialSchema.sql
│   │   ├── 002_AddUserManagement.sql
│   │   └── 003_AddAuditLogging.sql
│   └── backup/                                # バックアップスクリプト
│       ├── FullBackup.sql
│       └── LogBackup.sql
├── deployment/                                # デプロイメント関連
│   ├── scripts/                               # デプロイスクリプト
│   │   ├── build.ps1                          # PowerShellビルドスクリプト
│   │   ├── deploy.ps1                         # PowerShellデプロイスクリプト
│   │   ├── setup-iis.ps1                      # IIS設定スクリプト
│   │   └── setup-database.ps1                 # データベース設定スクリプト
│   ├── configs/                               # 設定ファイル
│   │   ├── web.config                         # IIS設定
│   │   ├── applicationHost.config             # IISアプリケーションホスト設定
│   │   └── nginx.conf                         # Nginx設定（代替）
│   ├── docker/                                # Docker関連（開発用）
│   │   ├── Dockerfile.backend
│   │   ├── Dockerfile.frontend
│   │   ├── docker-compose.yml
│   │   └── docker-compose.override.yml
│   └── monitoring/                            # 監視設定
│       ├── applicationinsights.config
│       ├── serilog.config
│       └── healthcheck.config
├── docs/                                      # ドキュメント
│   ├── api/                                   # APIドキュメント
│   │   ├── swagger.json                       # Swagger仕様
│   │   ├── postman-collection.json            # Postmanコレクション
│   │   └── api-endpoints.md                   # APIエンドポイント一覧
│   ├── database/                              # データベースドキュメント
│   │   ├── schema.md                          # スキーマ設計書
│   │   ├── erd.md                             # ER図
│   │   └── stored-procedures.md               # ストアドプロシージャ仕様
│   ├── deployment/                            # デプロイメントドキュメント
│   │   ├── installation-guide.md              # インストールガイド
│   │   ├── configuration-guide.md             # 設定ガイド
│   │   └── troubleshooting.md                 # トラブルシューティング
│   ├── development/                           # 開発ドキュメント
│   │   ├── development-setup.md               # 開発環境セットアップ
│   │   ├── coding-standards.md                # コーディング規約
│   │   ├── testing-guide.md                   # テストガイド
│   │   └── git-workflow.md                    # Gitワークフロー
│   └── user/                                  # ユーザードキュメント
│       ├── user-manual.md                     # ユーザーマニュアル
│       ├── admin-manual.md                    # 管理者マニュアル
│       └── faq.md                             # よくある質問
├── tools/                                     # 開発・運用ツール
│   ├── scripts/                               # ユーティリティスクリプト
│   │   ├── generate-api-client.ps1            # APIクライアント生成
│   │   ├── backup-database.ps1                # データベースバックアップ
│   │   ├── restore-database.ps1               # データベース復元
│   │   └── cleanup-logs.ps1                   # ログクリーンアップ
│   ├── templates/                             # テンプレート
│   │   ├── incident-report-template.docx
│   │   ├── email-template.html
│   │   └── notification-template.txt
│   └── configs/                               # ツール設定
│       ├── sonarqube.properties
│       ├── stylecop.json
│       └── editorconfig
├── .gitignore                                 # Git除外設定
├── README.md                                  # プロジェクト概要
├── technologystack.md                         # 技術スタック仕様書
├── directorystructure.md                      # ディレクトリ構成書（本ファイル）
└── CHANGELOG.md                               # 変更履歴
```

## レイヤー別アーキテクチャ

### バックエンドレイヤー構成

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│                 (LogisticsTroubleManagement.API)            │
├─────────────────────────────────────────────────────────────┤
│                     Business Layer                          │
│                (LogisticsTroubleManagement.Core)            │
├─────────────────────────────────────────────────────────────┤
│                  Infrastructure Layer                       │
│           (LogisticsTroubleManagement.Infrastructure)       │
├─────────────────────────────────────────────────────────────┤
│                     Domain Layer                            │
│               (LogisticsTroubleManagement.Domain)           │
└─────────────────────────────────────────────────────────────┘
```

### 各レイヤーの責務

#### 1. **Presentation Layer (API)**
- HTTPリクエスト/レスポンスの処理
- 認証・認可の制御
- 入力値の検証
- エラーハンドリング
- Swaggerドキュメント生成

#### 2. **Business Layer (Core)**
- ビジネスロジックの実装
- サービス間の調整
- DTOの変換
- バリデーション
- 例外処理

#### 3. **Infrastructure Layer**
- データベースアクセス
- 外部サービス連携
- ファイル操作
- ログ出力
- 設定管理

#### 4. **Domain Layer**
- エンティティ定義
- ドメインロジック
- 値オブジェクト
- ドメインイベント

## 命名規則

### プロジェクト名
- **API**: `LogisticsTroubleManagement.API`
- **Core**: `LogisticsTroubleManagement.Core`
- **Infrastructure**: `LogisticsTroubleManagement.Infrastructure`
- **Domain**: `LogisticsTroubleManagement.Domain`
- **Tests**: `LogisticsTroubleManagement.Tests`

### ファイル名
- **コントローラー**: `{Entity}Controller.cs`
- **サービス**: `{Entity}Service.cs`
- **リポジトリ**: `{Entity}Repository.cs`
- **エンティティ**: `{Entity}.cs`
- **DTO**: `{Entity}Dto.cs`

### 名前空間
```csharp
// API層
LogisticsTroubleManagement.API.Controllers
LogisticsTroubleManagement.API.Middleware

// Core層
LogisticsTroubleManagement.Core.Services
LogisticsTroubleManagement.Core.DTOs
LogisticsTroubleManagement.Core.Interfaces

// Infrastructure層
LogisticsTroubleManagement.Infrastructure.Data
LogisticsTroubleManagement.Infrastructure.Repositories

// Domain層
LogisticsTroubleManagement.Domain.Entities
LogisticsTroubleManagement.Domain.Enums
```

## 依存関係

### プロジェクト間の依存関係
```
API → Core → Domain
API → Infrastructure → Domain
Infrastructure → Core
Tests → API
Tests → Core
Tests → Infrastructure
Tests → Domain
```

### 外部依存関係
- **Entity Framework Core**: データアクセス
- **AutoMapper**: オブジェクトマッピング
- **FluentValidation**: バリデーション
- **Serilog**: ログ出力
- **Swashbuckle**: APIドキュメント
- **JWT Bearer**: 認証

## 設定ファイル構成

### アプリケーション設定
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;Trusted_Connection=true;"
  },
  "JwtSettings": {
    "SecretKey": "...",
    "Issuer": "...",
    "Audience": "...",
    "ExpirationHours": 24
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "FileStorage": {
    "BasePath": "C:\\LogisticsFiles",
    "MaxFileSize": 10485760
  }
}
```

### 環境別設定
- `appsettings.Development.json`: 開発環境
- `appsettings.Staging.json`: ステージング環境
- `appsettings.Production.json`: 本番環境

## データベース構成

### 主要テーブル
1. **Incidents**: トラブル管理テーブル
2. **Users**: ユーザー管理テーブル
3. **Attachments**: 添付ファイルテーブル
4. **AuditLogs**: 監査ログテーブル
5. **Categories**: カテゴリマスタテーブル

### インデックス戦略
- 主キー: クラスター化インデックス
- 外部キー: 非クラスター化インデックス
- 検索条件: 複合インデックス
- 日付範囲: パーティション化インデックス

## セキュリティ構成

### 認証・認可
- **JWT Bearer Token**: API認証
- **Windows認証**: データベース認証
- **Role-based Access Control**: 権限管理
- **CORS設定**: クロスオリジン制御

### データ保護
- **HTTPS**: 通信暗号化
- **SQL Injection対策**: パラメータ化クエリ
- **XSS対策**: 入力値サニタイゼーション
- **CSRF対策**: トークン検証

---

**文書作成日**: 2024年12月
**バージョン**: 1.0
**作成者**: 開発チーム
