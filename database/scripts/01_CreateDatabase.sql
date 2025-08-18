-- データベース作成スクリプト
-- 物流トラブル管理システム用データベース

-- データベースが存在しない場合のみ作成
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'LTMDB')
BEGIN
    CREATE DATABASE LTMDB
    ON PRIMARY (
        NAME = LTMDB_Data,
        FILENAME = '/var/opt/mssql/data/LTMDB_Data.mdf',
        SIZE = 100MB,
        MAXSIZE = UNLIMITED,
        FILEGROWTH = 10MB
    )
    LOG ON (
        NAME = LTMDB_Log,
        FILENAME = '/var/opt/mssql/data/LTMDB_Log.ldf',
        SIZE = 50MB,
        MAXSIZE = UNLIMITED,
        FILEGROWTH = 10MB
    );
END
GO

-- データベースを使用
USE LTMDB;
GO

-- データベースの照合順序を確認
SELECT name, collation_name FROM sys.databases WHERE name = 'LTMDB';
GO

PRINT 'データベース LTMDB が正常に作成されました。';
GO
