-- インデックス作成スクリプト
-- 物流トラブル管理システム用インデックス

USE LTMDB;
GO

-- Users テーブルのインデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Users_Username] ON [dbo].[Users] ([Username]);
END

-- Incidents テーブルのインデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_Status' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Incidents_Status] ON [dbo].[Incidents] ([Status]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_Priority' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Incidents_Priority] ON [dbo].[Incidents] ([Priority]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_Category' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Incidents_Category] ON [dbo].[Incidents] ([Category]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_ReportedDate' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Incidents_ReportedDate] ON [dbo].[Incidents] ([ReportedDate]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_AssignedTo' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Incidents_AssignedTo] ON [dbo].[Incidents] ([AssignedTo]);
END

-- 複合インデックス（検索用）
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Incidents_Status_Priority_Date' AND object_id = OBJECT_ID('Incidents'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Incidents_Status_Priority_Date] ON [dbo].[Incidents] ([Status], [Priority], [ReportedDate]);
END

-- Attachments テーブルのインデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Attachments_IncidentId' AND object_id = OBJECT_ID('Attachments'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Attachments_IncidentId] ON [dbo].[Attachments] ([IncidentId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Attachments_UploadedAt' AND object_id = OBJECT_ID('Attachments'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Attachments_UploadedAt] ON [dbo].[Attachments] ([UploadedAt]);
END

-- AuditLogs テーブルのインデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_UserId' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs] ([UserId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_CreatedAt' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_CreatedAt] ON [dbo].[AuditLogs] ([CreatedAt]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_TableName_RecordId' AND object_id = OBJECT_ID('AuditLogs'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AuditLogs_TableName_RecordId] ON [dbo].[AuditLogs] ([TableName], [RecordId]);
END

-- Effectiveness テーブルのインデックス
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Effectiveness_IncidentId' AND object_id = OBJECT_ID('Effectiveness'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Effectiveness_IncidentId] ON [dbo].[Effectiveness] ([IncidentId]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Effectiveness_Type' AND object_id = OBJECT_ID('Effectiveness'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Effectiveness_Type] ON [dbo].[Effectiveness] ([EffectivenessType]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Effectiveness_MeasuredAt' AND object_id = OBJECT_ID('Effectiveness'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Effectiveness_MeasuredAt] ON [dbo].[Effectiveness] ([MeasuredAt]);
END

PRINT 'すべてのインデックスが正常に作成されました。';
GO
