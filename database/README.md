# データベース (SQL Server 2022)

## 概要
物流トラブル管理システムのデータベース環境

## 技術スタック
- SQL Server 2022
- Docker
- Docker Compose

## 環境構成
```
database/
├── docker-compose.yml          # Docker Compose設定
├── scripts/
│   ├── init.sql               # 初期化スクリプト
│   ├── sample-data.sql        # サンプルデータ
│   └── migrations/            # マイグレーションファイル
└── data/                      # データファイル（永続化）
```

## 開発環境セットアップ

### 1. Docker環境の起動
```bash
cd database
docker-compose up -d
```

### 2. データベース接続情報
- **Server**: localhost,1434
- **Database**: LTMDB
- **Username**: sa
- **Password**: Medicalsv@6087!!
- **Connection String**: 
  ```
  Server=localhost,1434;Database=LTMDB;User Id=sa;Password=Medicalsv@6087!!;TrustServerCertificate=true;
  ```

### 3. 初期化スクリプトの実行
```bash
# データベースの初期化
docker exec -i logistics-trouble-management-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -i /scripts/init.sql
```

## データベース設計

### 主要テーブル
- **Incidents**: トラブル情報
- **Users**: ユーザー情報
- **Attachments**: 添付ファイル
- **AuditLogs**: 監査ログ
- **Effectiveness**: 効果測定

### インデックス戦略
- 主キーインデックス
- 外部キーインデックス
- 検索用複合インデックス
- 統計情報用インデックス

## バックアップ・復元

### バックアップ
```bash
docker exec logistics-trouble-management-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "BACKUP DATABASE LTMDB TO DISK = '/var/opt/mssql/backup/backup.bak'"
```

### 復元
```bash
docker exec logistics-trouble-management-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "RESTORE DATABASE LTMDB FROM DISK = '/var/opt/mssql/backup/backup.bak'"
```

## パフォーマンス監視
- クエリ実行時間の監視
- インデックス使用状況の確認
- デッドロックの監視
- 接続数の監視

## セキュリティ
- SAアカウントの強力なパスワード
- ファイアウォール設定
- SSL/TLS接続
- 最小権限の原則
