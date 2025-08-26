-- 更新されたサンプルデータ作成スクリプト
-- 物流トラブル管理システム用サンプルデータ（画像の表示内容に合致）
-- 作成日: 2025-08-25

USE LTMDB;
GO

-- 既存データのクリア（外部キー制約を考慮して順序を守る）
DELETE FROM [dbo].[Attachments];
DELETE FROM [dbo].[Effectiveness];
DELETE FROM [dbo].[AuditLogs];
DELETE FROM [dbo].[Incidents];
DELETE FROM [dbo].[Users];
GO

-- ユーザーデータの挿入
INSERT INTO [dbo].[Users] ([Username], [Email], [FirstName], [LastName], [Role], [IsActive], [PhoneNumber])
VALUES 
    ('admin', 'admin@logistics.com', N'管理者', N'太郎', 1, 1, '090-1234-5678'),
    ('user1', 'user1@logistics.com', N'田中', N'一郎', 2, 1, '090-2345-6789'),
    ('user2', 'user2@logistics.com', N'佐藤', N'次郎', 2, 1, '090-3456-7890'),
    ('user3', 'user3@logistics.com', N'鈴木', N'三郎', 2, 1, '090-4567-8901'),
    ('manager1', 'manager1@logistics.com', N'高橋', N'四郎', 3, 1, '090-5678-9012'),
    ('manager2', 'manager2@logistics.com', N'渡辺', N'五郎', 3, 1, '090-6789-0123');
GO

-- トラブルデータの挿入（画像の表示内容に合致）
INSERT INTO [dbo].[Incidents] (
    [Title], [Description], [Status], [Priority], [Category], 
    [ReportedById], [AssignedToId], [ReportedDate], [ResolvedDate], [Resolution],
    [CreatedAt], [UpdatedAt], [DamageType], [EffectivenessStatus], [ShippingCompany], 
    [TroubleType], [Warehouse], [Cause], [DefectiveItems], [EffectivenessComment], 
    [EffectivenessDate], [IncidentDetails], [OccurrenceDate], [OccurrenceLocation], 
    [PreventionMeasures], [Summary], [TotalShipments]
)
VALUES 
    (N'設備トラブルの発生', N'C倉庫の設備でトラブルが発生しました。', 1, 2, N'設備トラブル', 2, 5, '2025-08-24', NULL, NULL, GETUTCDATE(), GETUTCDATE(), 0, 0, 0, 3, 3, N'設備の老朽化', 0, N'', NULL, N'設備の老朽化によりトラブルが発生', '2025-08-24', N'C倉庫', N'設備の定期点検強化', N'設備トラブルの発生', 0),
    
    (N'品質トラブルの発生', N'A倉庫で品質に関するトラブルが発生しました。', 1, 2, N'品質トラブル', 3, 6, '2025-08-23', NULL, NULL, GETUTCDATE(), GETUTCDATE(), 0, 0, 1, 4, 1, N'品質管理の不備', 0, N'', NULL, N'品質管理の不備によりトラブルが発生', '2025-08-23', N'A倉庫', N'品質管理体制の見直し', N'品質トラブルの発生', 0),
    
    (N'物流トラブルの発生', N'A倉庫で物流に関するトラブルが発生しました。', 1, 2, N'物流トラブル', 4, 6, '2025-08-23', NULL, NULL, GETUTCDATE(), GETUTCDATE(), 0, 0, 1, 5, 1, N'物流プロセスの不備', 0, N'', NULL, N'物流プロセスの不備によりトラブルが発生', '2025-08-23', N'A倉庫', N'物流プロセスの改善', N'物流トラブルの発生', 0),
    
    (N'配送トラブルの発生', N'B倉庫から配送トラブルが発生しました。', 1, 2, N'配送トラブル', 2, 5, '2025-08-22', NULL, NULL, GETUTCDATE(), GETUTCDATE(), 1, 0, 2, 1, 2, N'配送ルートの不備', 0, N'', NULL, N'配送ルートの不備によりトラブルが発生', '2025-08-22', N'B倉庫', N'配送ルートの見直し', N'配送トラブルの発生', 0),
    
    (N'物流トラブルの発生2', N'B倉庫で物流に関するトラブルが発生しました。', 1, 2, N'物流トラブル', 3, 6, '2025-08-21', NULL, NULL, GETUTCDATE(), GETUTCDATE(), 0, 0, 0, 5, 2, N'物流プロセスの不備', 0, N'', NULL, N'物流プロセスの不備によりトラブルが発生', '2025-08-21', N'B倉庫', N'物流プロセスの改善', N'物流トラブルの発生2', 0),
    
    (N'商品トラブルの発生', N'A倉庫で商品に関するトラブルが発生しました。', 1, 2, N'商品トラブル', 4, 6, '2025-08-20', NULL, NULL, GETUTCDATE(), GETUTCDATE(), 0, 0, 1, 0, 1, N'商品管理の不備', 0, N'', NULL, N'商品管理の不備によりトラブルが発生', '2025-08-20', N'A倉庫', N'商品管理体制の見直し', N'商品トラブルの発生', 0),
    
    (N'管理トラブルの発生', N'管理に関するトラブルが発生しました。', 1, 2, N'管理トラブル', 2, 5, '2025-08-19', NULL, NULL, GETUTCDATE(), GETUTCDATE(), 0, 0, 0, 6, 0, N'管理体制の不備', 0, N'', NULL, N'管理体制の不備によりトラブルが発生', '2025-08-19', N'本社', N'管理体制の見直し', N'管理トラブルの発生', 0),
    
    (N'安全トラブルの発生', N'安全に関するトラブルが発生しました。', 1, 2, N'安全トラブル', 3, 6, '2025-08-18', NULL, NULL, GETUTCDATE(), GETUTCDATE(), 0, 1, 0, 7, 0, N'安全管理の不備', 0, N'安全管理体制を改善', '2025-08-18', N'安全管理の不備によりトラブルが発生', '2025-08-18', N'本社', N'安全管理体制の強化', N'安全トラブルの発生', 0),
    
    (N'環境トラブルの発生', N'A倉庫で環境に関するトラブルが発生しました。', 1, 2, N'環境トラブル', 4, 6, '2025-08-17', NULL, NULL, GETUTCDATE(), GETUTCDATE(), 0, 0, 1, 8, 1, N'環境管理の不備', 0, N'', NULL, N'環境管理の不備によりトラブルが発生', '2025-08-17', N'A倉庫', N'環境管理体制の見直し', N'環境トラブルの発生', 0),
    
    (N'その他のトラブルの発生', N'その他のトラブルが発生しました。', 1, 2, N'その他のトラブル', 2, 5, '2025-08-15', NULL, NULL, GETUTCDATE(), GETUTCDATE(), 0, 1, 0, 9, 0, N'その他の要因', 0, N'その他の対策を実施', '2025-08-15', N'その他の要因によりトラブルが発生', '2025-08-15', N'本社', N'その他の対策の実施', N'その他のトラブルの発生', 0);
GO

-- 効果測定データの挿入
INSERT INTO [dbo].[Effectiveness] (
    [IncidentId], [EffectivenessType], [BeforeValue], [Description], 
    [MeasuredById], [MeasuredAt], [AfterValue], [ImprovementRate]
)
VALUES 
    (8, N'安全管理改善', 70.0, N'安全管理体制の強化により安全性が向上しました', 6, '2025-08-18', 95.0, 35.7),
    (10, N'その他対策効果', 60.0, N'その他の対策により問題が改善されました', 5, '2025-08-15', 80.0, 33.3);
GO

-- 監査ログデータの挿入
INSERT INTO [dbo].[AuditLogs] ([UserId], [Action], [TableName], [RecordId], [OldValues], [NewValues], [IpAddress], [UserAgent], [IncidentId])
VALUES 
    (2, 'INSERT', 'Incidents', 1, NULL, N'{"Title":"設備トラブルの発生","Status":1}', '192.168.1.100', 'Mozilla/5.0', 1),
    (3, 'INSERT', 'Incidents', 2, NULL, N'{"Title":"品質トラブルの発生","Status":1}', '192.168.1.101', 'Mozilla/5.0', 2),
    (4, 'INSERT', 'Incidents', 3, NULL, N'{"Title":"物流トラブルの発生","Status":1}', '192.168.1.102', 'Mozilla/5.0', 3),
    (2, 'INSERT', 'Incidents', 4, NULL, N'{"Title":"配送トラブルの発生","Status":1}', '192.168.1.103', 'Mozilla/5.0', 4),
    (3, 'INSERT', 'Incidents', 5, NULL, N'{"Title":"物流トラブルの発生2","Status":1}', '192.168.1.104', 'Mozilla/5.0', 5),
    (4, 'INSERT', 'Incidents', 6, NULL, N'{"Title":"商品トラブルの発生","Status":1}', '192.168.1.105', 'Mozilla/5.0', 6),
    (2, 'INSERT', 'Incidents', 7, NULL, N'{"Title":"管理トラブルの発生","Status":1}', '192.168.1.106', 'Mozilla/5.0', 7),
    (3, 'INSERT', 'Incidents', 8, NULL, N'{"Title":"安全トラブルの発生","Status":1}', '192.168.1.107', 'Mozilla/5.0', 8),
    (4, 'INSERT', 'Incidents', 9, NULL, N'{"Title":"環境トラブルの発生","Status":1}', '192.168.1.108', 'Mozilla/5.0', 9),
    (2, 'INSERT', 'Incidents', 10, NULL, N'{"Title":"その他のトラブルの発生","Status":1}', '192.168.1.109', 'Mozilla/5.0', 10);
GO

-- 添付ファイルデータの挿入
INSERT INTO [dbo].[Attachments] ([IncidentId], [FileName], [FilePath], [FileSize], [ContentType], [UploadedById], [UploadedAt])
VALUES 
    (1, N'設備トラブル報告書.pdf', '/attachments/incident_1/report.pdf', 1024000, 'application/pdf', 2, '2025-08-24'),
    (2, N'品質トラブル報告書.pdf', '/attachments/incident_2/report.pdf', 1024000, 'application/pdf', 3, '2025-08-23'),
    (3, N'物流トラブル報告書.pdf', '/attachments/incident_3/report.pdf', 1024000, 'application/pdf', 4, '2025-08-23'),
    (4, N'配送トラブル報告書.pdf', '/attachments/incident_4/report.pdf', 1024000, 'application/pdf', 2, '2025-08-22'),
    (5, N'物流トラブル報告書2.pdf', '/attachments/incident_5/report.pdf', 1024000, 'application/pdf', 3, '2025-08-21'),
    (6, N'商品トラブル報告書.pdf', '/attachments/incident_6/report.pdf', 1024000, 'application/pdf', 4, '2025-08-20'),
    (7, N'管理トラブル報告書.pdf', '/attachments/incident_7/report.pdf', 1024000, 'application/pdf', 2, '2025-08-19'),
    (8, N'安全トラブル報告書.pdf', '/attachments/incident_8/report.pdf', 1024000, 'application/pdf', 3, '2025-08-18'),
    (9, N'環境トラブル報告書.pdf', '/attachments/incident_9/report.pdf', 1024000, 'application/pdf', 4, '2025-08-17'),
    (10, N'その他トラブル報告書.pdf', '/attachments/incident_10/report.pdf', 1024000, 'application/pdf', 2, '2025-08-15');
GO

PRINT N'画像の表示内容に合致するサンプルデータが正常に挿入されました。';
GO

-- データ確認クエリ
SELECT 'Users' as TableName, COUNT(*) as RecordCount FROM Users
UNION ALL
SELECT 'Incidents', COUNT(*) FROM Incidents
UNION ALL
SELECT 'Effectiveness', COUNT(*) FROM Effectiveness
UNION ALL
SELECT 'AuditLogs', COUNT(*) FROM AuditLogs
UNION ALL
SELECT 'Attachments', COUNT(*) FROM Attachments;
GO

-- トラブル種類別の件数確認
SELECT 
    CASE 
        WHEN TroubleType = 0 THEN N'商品トラブル'
        WHEN TroubleType = 1 THEN N'配送トラブル'
        WHEN TroubleType = 2 THEN N'システムトラブル'
        WHEN TroubleType = 3 THEN N'設備トラブル'
        WHEN TroubleType = 4 THEN N'品質トラブル'
        WHEN TroubleType = 5 THEN N'物流トラブル'
        WHEN TroubleType = 6 THEN N'管理トラブル'
        WHEN TroubleType = 7 THEN N'安全トラブル'
        WHEN TroubleType = 8 THEN N'環境トラブル'
        WHEN TroubleType = 9 THEN N'その他のトラブル'
        ELSE N'不明'
    END as TroubleTypeName,
    COUNT(*) as Count
FROM Incidents 
GROUP BY TroubleType 
ORDER BY TroubleType;
GO
