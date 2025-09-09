-- 認証機能実装用テーブル作成スクリプト
-- 作成日: 2025-09-03
-- 作成者: システム開発チーム

USE [LogisticsTroubleManagement]
GO

-- 1. Rolesテーブルの新規作成
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Roles] (
        [Id] INT PRIMARY KEY IDENTITY(1,1),
        [Name] NVARCHAR(50) NOT NULL UNIQUE,
        [Description] NVARCHAR(200) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL
    );
    
    PRINT 'Rolesテーブルを作成しました。';
END
ELSE
BEGIN
    PRINT 'Rolesテーブルは既に存在します。';
END

-- 2. Usersテーブルの拡張
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'PasswordHash')
BEGIN
    ALTER TABLE [dbo].[Users] ADD
        [PasswordHash] NVARCHAR(MAX) NOT NULL DEFAULT '',
        [LastLoginAt] DATETIME2 NULL,
        [TokenVersion] INT NOT NULL DEFAULT 1;
    
    PRINT 'Usersテーブルに認証関連フィールドを追加しました。';
END
ELSE
BEGIN
    PRINT 'Usersテーブルの認証関連フィールドは既に存在します。';
END

-- 3. RefreshTokensテーブルの新規作成
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefreshTokens]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RefreshTokens] (
        [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [TokenHash] VARBINARY(64) NOT NULL,
        [ExpiresAt] DATETIME2 NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [RevokedAt] DATETIME2 NULL,
        [ReplacedByTokenHash] VARBINARY(64) NULL,
        [Reason] NVARCHAR(100) NULL,
        [UserAgent] NVARCHAR(200) NULL,
        [IpAddress] NVARCHAR(45) NULL
    );
    
    -- 外部キー制約の追加
    ALTER TABLE [dbo].[RefreshTokens] 
    ADD CONSTRAINT [FK_RefreshTokens_Users] 
    FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]);
    
    -- インデックスの作成
    CREATE INDEX [IX_RefreshTokens_User_Active] 
    ON [dbo].[RefreshTokens] ([UserId], [ExpiresAt]) 
    WHERE [RevokedAt] IS NULL;
    
    PRINT 'RefreshTokensテーブルを作成しました。';
END
ELSE
BEGIN
    PRINT 'RefreshTokensテーブルは既に存在します。';
END

-- 4. 既存Roleフィールドの外部キー制約追加
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_Role')
BEGIN
    ALTER TABLE [dbo].[Users] 
    ADD CONSTRAINT [FK_Users_Role] 
    FOREIGN KEY ([Role]) REFERENCES [Roles]([Id]);
    
    PRINT 'UsersテーブルのRoleフィールドに外部キー制約を追加しました。';
END
ELSE
BEGIN
    PRINT 'UsersテーブルのRoleフィールドの外部キー制約は既に存在します。';
END

-- 5. インデックスの作成・最適化
-- Usersテーブルのインデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username')
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [dbo].[Users] ([Username]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email')
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
END

-- RefreshTokensテーブルの追加インデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RefreshTokens_TokenHash')
BEGIN
    CREATE INDEX [IX_RefreshTokens_TokenHash] ON [dbo].[RefreshTokens] ([TokenHash]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RefreshTokens_ExpiresAt')
BEGIN
    CREATE INDEX [IX_RefreshTokens_ExpiresAt] ON [dbo].[RefreshTokens] ([ExpiresAt]);
END

PRINT '認証機能用テーブルの作成が完了しました。';
GO
