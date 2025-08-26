-- 更新されたインデックス作成スクリプト
-- 物流トラブル管理システム用インデックス（現在のDB状態に合わせて更新）
-- 作成日: 2025-08-25

USE LTMDB;
GO

-- Users テーブルのインデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [dbo].[Users] ([Username]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Role' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX [IX_Users_Role] ON [dbo].[Users] ([Role]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_IsActive' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX [IX_Users_IsActive] ON [dbo].[Users] ([IsActive]);
END

-- Incidents テーブルのインデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_Status' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_Status] ON [dbo].[Incidents] ([Status]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_Priority' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_Priority] ON [dbo].[Incidents] ([Priority]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_Category' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_Category] ON [dbo].[Incidents] ([Category]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_ReportedById' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_ReportedById] ON [dbo].[Incidents] ([ReportedById]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_AssignedToId' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_AssignedToId] ON [dbo].[Incidents] ([AssignedToId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_ReportedDate' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_ReportedDate] ON [dbo].[Incidents] ([ReportedDate]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_ResolvedDate' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_ResolvedDate] ON [dbo].[Incidents] ([ResolvedDate]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_DamageType' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_DamageType] ON [dbo].[Incidents] ([DamageType]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_EffectivenessStatus' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_EffectivenessStatus] ON [dbo].[Incidents] ([EffectivenessStatus]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_ShippingCompany' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_ShippingCompany] ON [dbo].[Incidents] ([ShippingCompany]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_TroubleType' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_TroubleType] ON [dbo].[Incidents] ([TroubleType]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_Warehouse' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_Warehouse] ON [dbo].[Incidents] ([Warehouse]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_OccurrenceDate' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_OccurrenceDate] ON [dbo].[Incidents] ([OccurrenceDate]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_EffectivenessDate' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_EffectivenessDate] ON [dbo].[Incidents] ([EffectivenessDate]);
END

-- Attachments テーブルのインデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Attachments_IncidentId' AND object_id = OBJECT_ID('Attachments'))
BEGIN
    CREATE INDEX [IX_Attachments_IncidentId] ON [dbo].[Attachments] ([IncidentId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Attachments_UploadedById' AND object_id = OBJECT_ID('Attachments'))
BEGIN
    CREATE INDEX [IX_Attachments_UploadedById] ON [dbo].[Attachments] ([UploadedById]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Attachments_UploadedAt' AND object_id = OBJECT_ID('Attachments'))
BEGIN
    CREATE INDEX [IX_Attachments_UploadedAt] ON [dbo].[Attachments] ([UploadedAt]);
END

-- Effectiveness テーブルのインデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Effectiveness_IncidentId' AND object_id = OBJECT_ID('Effectiveness'))
BEGIN
    CREATE INDEX [IX_Effectiveness_IncidentId] ON [dbo].[Effectiveness] ([IncidentId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Effectiveness_EffectivenessType' AND object_id = OBJECT_ID('Effectiveness'))
BEGIN
    CREATE INDEX [IX_Effectiveness_EffectivenessType] ON [dbo].[Effectiveness] ([EffectivenessType]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Effectiveness_MeasuredById' AND object_id = OBJECT_ID('Effectiveness'))
BEGIN
    CREATE INDEX [IX_Effectiveness_MeasuredById] ON [dbo].[Effectiveness] ([MeasuredById]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Effectiveness_MeasuredAt' AND object_id = OBJECT_ID('Effectiveness'))
BEGIN
    CREATE INDEX [IX_Effectiveness_MeasuredAt] ON [dbo].[Effectiveness] ([MeasuredAt]);
END

-- AuditLogs テーブルのインデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_UserId' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs] ([UserId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_Action' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX [IX_AuditLogs_Action] ON [dbo].[AuditLogs] ([Action]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_TableName' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX [IX_AuditLogs_TableName] ON [dbo].[AuditLogs] ([TableName]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_RecordId' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX [IX_AuditLogs_RecordId] ON [dbo].[AuditLogs] ([RecordId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_IncidentId' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX [IX_AuditLogs_IncidentId] ON [dbo].[AuditLogs] ([IncidentId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_CreatedAt' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE INDEX [IX_AuditLogs_CreatedAt] ON [dbo].[AuditLogs] ([CreatedAt]);
END

-- 複合インデックス（パフォーマンス向上のため）
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_Status_Priority' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_Status_Priority] ON [dbo].[Incidents] ([Status], [Priority]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_Category_Status' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_Category_Status] ON [dbo].[Incidents] ([Category], [Status]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_ReportedDate_Status' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE INDEX [IX_Incidents_ReportedDate_Status] ON [dbo].[Incidents] ([ReportedDate], [Status]);
END

PRINT N'すべてのインデックスが正常に作成されました。';
GO
