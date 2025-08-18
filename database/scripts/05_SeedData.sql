-- サンプルデータ作成スクリプト
-- 物流トラブル管理システム用サンプルデータ

USE LTMDB;
GO

-- ユーザーデータの挿入
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO [dbo].[Users] ([Username], [Email], [FirstName], [LastName], [Role], [IsActive])
    VALUES 
        ('admin', 'admin@logistics.com', '管理者', '太郎', 'Admin', 1),
        ('user1', 'user1@logistics.com', '田中', '一郎', 'User', 1),
        ('user2', 'user2@logistics.com', '佐藤', '次郎', 'User', 1),
        ('user3', 'user3@logistics.com', '鈴木', '三郎', 'User', 1),
        ('manager1', 'manager1@logistics.com', '高橋', '四郎', 'Manager', 1),
        ('manager2', 'manager2@logistics.com', '渡辺', '五郎', 'Manager', 1);
END
GO

-- トラブルデータの挿入
IF NOT EXISTS (SELECT * FROM Incidents WHERE Title = '配送遅延の発生')
BEGIN
    INSERT INTO [dbo].[Incidents] ([Title], [Description], [Status], [Priority], [Category], [ReportedBy], [AssignedTo], [ReportedDate])
    VALUES 
        ('配送遅延の発生', '東京から大阪への配送で2時間の遅延が発生しました。原因は高速道路の渋滞です。', 'Open', 'High', '配送遅延', 2, 5, DATEADD(day, -5, GETUTCDATE())),
        ('商品破損の報告', '配送中の商品が破損して到着しました。梱包の改善が必要です。', 'InProgress', 'Medium', '商品破損', 3, 6, DATEADD(day, -3, GETUTCDATE())),
        ('システムエラーの発生', '在庫管理システムでエラーが発生し、在庫数が正しく表示されません。', 'Resolved', 'High', 'システム障害', 4, 1, DATEADD(day, -7, GETUTCDATE())),
        ('倉庫の温度異常', '冷蔵倉庫の温度が設定値より高くなっています。緊急対応が必要です。', 'Open', 'Critical', '設備故障', 2, 5, DATEADD(day, -1, GETUTCDATE())),
        ('配送ルートの最適化', '配送ルートの見直しにより、配送時間を短縮できる可能性があります。', 'Open', 'Low', '改善提案', 3, NULL, DATEADD(day, -2, GETUTCDATE())),
        ('新商品の導入準備', '新商品の導入に伴い、倉庫のレイアウト変更が必要です。', 'InProgress', 'Medium', '業務改善', 4, 6, DATEADD(day, -4, GETUTCDATE())),
        ('従業員の安全教育', '新入社員向けの安全教育を実施する必要があります。', 'Open', 'Medium', '教育・研修', 2, 5, DATEADD(day, -6, GETUTCDATE())),
        ('IT機器の更新', '古いIT機器の更新が必要です。予算の確保も含めて検討してください。', 'Resolved', 'Low', '設備更新', 3, 1, DATEADD(day, -10, GETUTCDATE())),
        ('顧客からの苦情対応', '配送サービスの品質について顧客から苦情がありました。', 'InProgress', 'High', '顧客対応', 4, 6, DATEADD(day, -2, GETUTCDATE())),
        ('環境対策の実施', '環境に配慮した配送方法の導入を検討しています。', 'Open', 'Low', '環境対策', 2, NULL, DATEADD(day, -8, GETUTCDATE()));
END
GO

-- 解決済みトラブルの更新
UPDATE [dbo].[Incidents] 
SET [Status] = 'Resolved', 
    [ResolvedDate] = DATEADD(day, -2, GETUTCDATE()),
    [Resolution] = 'システムの再起動により問題を解決しました。再発防止のため監視を強化します。'
WHERE [Title] = 'システムエラーの発生';

UPDATE [dbo].[Incidents] 
SET [Status] = 'Resolved', 
    [ResolvedDate] = DATEADD(day, -5, GETUTCDATE()),
    [Resolution] = '新しいIT機器を導入し、作業効率が向上しました。'
WHERE [Title] = 'IT機器の更新';
GO

-- 効果測定データの挿入
IF NOT EXISTS (SELECT * FROM Effectiveness WHERE EffectivenessType = '配送時間短縮')
BEGIN
    INSERT INTO [dbo].[Effectiveness] ([IncidentId], [EffectivenessType], [Value], [Unit], [Description], [MeasuredBy], [MeasuredAt])
    VALUES 
        (1, '配送時間短縮', 30.5, '分', '新ルート導入により配送時間が短縮されました', 5, DATEADD(day, -1, GETUTCDATE())),
        (2, '破損率改善', 0.5, '%', '梱包改善により破損率が改善されました', 6, DATEADD(day, -1, GETUTCDATE())),
        (3, 'システム稼働率向上', 99.8, '%', 'システム改善により稼働率が向上しました', 1, DATEADD(day, -2, GETUTCDATE())),
        (4, '温度管理精度向上', 0.2, '℃', '温度管理システムの精度が向上しました', 5, DATEADD(day, -1, GETUTCDATE())),
        (6, '作業効率向上', 15.0, '%', 'レイアウト変更により作業効率が向上しました', 6, DATEADD(day, -1, GETUTCDATE())),
        (8, '処理速度向上', 25.0, '%', '新機器導入により処理速度が向上しました', 1, DATEADD(day, -3, GETUTCDATE()));
END
GO

-- 監査ログデータの挿入
IF NOT EXISTS (SELECT * FROM AuditLogs WHERE Action = 'INSERT')
BEGIN
    INSERT INTO [dbo].[AuditLogs] ([UserId], [Action], [TableName], [RecordId], [OldValues], [NewValues], [IpAddress], [UserAgent])
    VALUES 
        (2, 'INSERT', 'Incidents', 1, NULL, '{"Title":"配送遅延の発生","Status":"Open"}', '192.168.1.100', 'Mozilla/5.0'),
        (3, 'INSERT', 'Incidents', 2, NULL, '{"Title":"商品破損の報告","Status":"Open"}', '192.168.1.101', 'Mozilla/5.0'),
        (4, 'INSERT', 'Incidents', 3, NULL, '{"Title":"システムエラーの発生","Status":"Open"}', '192.168.1.102', 'Mozilla/5.0'),
        (1, 'UPDATE', 'Incidents', 3, '{"Status":"Open"}', '{"Status":"Resolved"}', '192.168.1.103', 'Mozilla/5.0'),
        (5, 'UPDATE', 'Incidents', 1, '{"Status":"Open"}', '{"Status":"InProgress"}', '192.168.1.104', 'Mozilla/5.0'),
        (6, 'UPDATE', 'Incidents', 2, '{"Status":"Open"}', '{"Status":"InProgress"}', '192.168.1.105', 'Mozilla/5.0');
END
GO

PRINT 'サンプルデータが正常に挿入されました。';
GO

-- データ確認クエリ
SELECT 'Users' as TableName, COUNT(*) as RecordCount FROM Users
UNION ALL
SELECT 'Incidents', COUNT(*) FROM Incidents
UNION ALL
SELECT 'Effectiveness', COUNT(*) FROM Effectiveness
UNION ALL
SELECT 'AuditLogs', COUNT(*) FROM AuditLogs;
GO
