-- テーブル作成スクリプト
-- 物流トラブル管理システム用テーブル

USE LTMDB;
GO

-- ユーザーテーブル
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Username] NVARCHAR(50) NOT NULL UNIQUE,
        [Email] NVARCHAR(100) NOT NULL UNIQUE,
        [FirstName] NVARCHAR(50) NOT NULL,
        [LastName] NVARCHAR(50) NOT NULL,
        [Role] NVARCHAR(20) NOT NULL DEFAULT 'User',
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- トラブル管理テーブル
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Incidents]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Incidents] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Title] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(MAX) NOT NULL,
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'Open',
        [Priority] NVARCHAR(20) NOT NULL DEFAULT 'Medium',
        [Category] NVARCHAR(50) NOT NULL,
        [ReportedBy] INT NOT NULL,
        [AssignedTo] INT NULL,
        [ReportedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ResolvedDate] DATETIME2 NULL,
        [Resolution] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_Incidents_ReportedBy] FOREIGN KEY ([ReportedBy]) REFERENCES [dbo].[Users]([Id]),
        CONSTRAINT [FK_Incidents_AssignedTo] FOREIGN KEY ([AssignedTo]) REFERENCES [dbo].[Users]([Id])
    );
END
GO

-- 添付ファイルテーブル
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Attachments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Attachments] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [IncidentId] INT NOT NULL,
        [FileName] NVARCHAR(255) NOT NULL,
        [FilePath] NVARCHAR(500) NOT NULL,
        [FileSize] BIGINT NOT NULL,
        [ContentType] NVARCHAR(100) NOT NULL,
        [UploadedBy] INT NOT NULL,
        [UploadedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_Attachments_IncidentId] FOREIGN KEY ([IncidentId]) REFERENCES [dbo].[Incidents]([Id]),
        CONSTRAINT [FK_Attachments_UploadedBy] FOREIGN KEY ([UploadedBy]) REFERENCES [dbo].[Users]([Id])
    );
END
GO

-- 監査ログテーブル
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AuditLogs] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId] INT NULL,
        [Action] NVARCHAR(50) NOT NULL,
        [TableName] NVARCHAR(50) NOT NULL,
        [RecordId] INT NULL,
        [OldValues] NVARCHAR(MAX) NULL,
        [NewValues] NVARCHAR(MAX) NULL,
        [IpAddress] NVARCHAR(45) NULL,
        [UserAgent] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_AuditLogs_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
    );
END
GO

-- 効果測定テーブル
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Effectiveness]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Effectiveness] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [IncidentId] INT NOT NULL,
        [EffectivenessType] NVARCHAR(50) NOT NULL,
        [Value] DECIMAL(10,2) NOT NULL,
        [Unit] NVARCHAR(20) NULL,
        [Description] NVARCHAR(500) NULL,
        [MeasuredBy] INT NOT NULL,
        [MeasuredAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_Effectiveness_IncidentId] FOREIGN KEY ([IncidentId]) REFERENCES [dbo].[Incidents]([Id]),
        CONSTRAINT [FK_Effectiveness_MeasuredBy] FOREIGN KEY ([MeasuredBy]) REFERENCES [dbo].[Users]([Id])
    );
END
GO

PRINT 'すべてのテーブルが正常に作成されました。';
GO
