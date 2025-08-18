# バックエンド (ASP.NET Core 8.0 LTS)

## 概要
物流トラブル管理システムのバックエンドAPI

## 技術スタック
- ASP.NET Core 8.0 LTS
- Entity Framework Core
- SQL Server 2022
- Clean Architecture

## プロジェクト構造
```
backend/
├── src/
│   ├── LogisticsTroubleManagement.API/          # Web API プロジェクト
│   ├── LogisticsTroubleManagement.Core/         # ドメイン層
│   ├── LogisticsTroubleManagement.Infrastructure/ # インフラ層
│   └── LogisticsTroubleManagement.Application/  # アプリケーション層
└── tests/
    ├── LogisticsTroubleManagement.UnitTests/    # 単体テスト
    └── LogisticsTroubleManagement.IntegrationTests/ # 統合テスト
```

## 開発環境
- Visual Studio 2022
- .NET 8.0 SDK
- SQL Server 2022 (Docker)

## セットアップ手順
1. .NET 8.0 SDKのインストール
2. Visual Studio 2022のインストール
3. ソリューションの作成
4. 各プロジェクトの作成
5. NuGetパッケージの追加
6. データベース接続設定

## API仕様
- RESTful API
- JSON形式でのデータ交換
- JWT認証（後期実装）
- Swagger/OpenAPI仕様書

## データベース
- SQL Server 2022
- Entity Framework Core
- Code First アプローチ
- マイグレーション管理
