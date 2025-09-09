-- 認証機能実装用初期データ投入スクリプト
-- 作成日: 2025-09-03
-- 作成者: システム開発チーム

USE [LogisticsTroubleManagement]
GO

-- 1. ロールの初期データ投入
IF NOT EXISTS (SELECT * FROM [Roles] WHERE [Name] = 'Clerk')
BEGIN
    INSERT INTO [Roles] ([Name], [Description], [IsActive], [CreatedAt], [UpdatedAt]) VALUES
    ('Clerk', '事務員 - 基本的なインシデント管理', 1, GETUTCDATE(), GETUTCDATE()),
    ('IncidentManager', 'インシデント管理者 - インシデント分類・管理', 1, GETUTCDATE(), GETUTCDATE()),
    ('WarehouseStaff', '倉庫担当 - 解決策検討・登録', 1, GETUTCDATE(), GETUTCDATE()),
    ('Admin', 'システム管理者 - 全機能・ユーザー管理', 1, GETUTCDATE(), GETUTCDATE());
    
    PRINT 'ロールの初期データを投入しました。';
END
ELSE
BEGIN
    PRINT 'ロールの初期データは既に存在します。';
END

-- 2. システム管理者ユーザーの作成（初期パスワード: Admin123!）
-- 注意: 実際の運用では、より強力なパスワードを使用してください
IF NOT EXISTS (SELECT * FROM [Users] WHERE [Username] = 'admin')
BEGIN
    -- パスワードハッシュ: Admin123! (bcrypt)
    -- 実際の実装では、適切なハッシュ化処理を使用してください
    INSERT INTO [Users] ([Username], [Email], [Role], [PasswordHash], [TokenVersion], [IsActive], [CreatedAt], [UpdatedAt]) VALUES
    ('admin', 'admin@example.com', 4, '$2a$11$YourHashedPasswordHere', 1, 1, GETUTCDATE(), GETUTCDATE());
    
    PRINT 'システム管理者ユーザーを作成しました。';
END
ELSE
BEGIN
    PRINT 'システム管理者ユーザーは既に存在します。';
END

-- 3. テスト用ユーザーの作成（初期パスワード: Test123!）
-- 注意: 実際の運用では、より強力なパスワードを使用してください
IF NOT EXISTS (SELECT * FROM [Users] WHERE [Username] = 'clerk1')
BEGIN
    -- パスワードハッシュ: Test123! (bcrypt)
    -- 実際の実装では、適切なハッシュ化処理を使用してください
    INSERT INTO [Users] ([Username], [Email], [Role], [PasswordHash], [TokenVersion], [IsActive], [CreatedAt], [UpdatedAt]) VALUES
    ('clerk1', 'clerk1@example.com', 1, '$2a$11$YourHashedPasswordHere', 1, 1, GETUTCDATE(), GETUTCDATE()),
    ('manager1', 'manager1@example.com', 2, '$2a$11$YourHashedPasswordHere', 1, 1, GETUTCDATE(), GETUTCDATE()),
    ('warehouse1', 'warehouse1@example.com', 3, '$2a$11$YourHashedPasswordHere', 1, 1, GETUTCDATE(), GETUTCDATE());
    
    PRINT 'テスト用ユーザーを作成しました。';
END
ELSE
BEGIN
    PRINT 'テスト用ユーザーは既に存在します。';
END

-- 4. 既存インシデントデータの更新（CreatedByUserIdの設定）
-- 既存のインシデントデータがある場合、システム管理者を担当者として設定
IF EXISTS (SELECT * FROM [Incidents] WHERE [CreatedByUserId] IS NULL)
BEGIN
    DECLARE @AdminUserId INT = (SELECT [Id] FROM [Users] WHERE [Username] = 'admin');
    
    IF @AdminUserId IS NOT NULL
    BEGIN
        UPDATE [Incidents] 
        SET [CreatedByUserId] = @AdminUserId,
            [UpdatedAt] = GETUTCDATE()
        WHERE [CreatedByUserId] IS NULL;
        
        PRINT '既存インシデントデータの担当者を更新しました。';
    END
END

-- 5. データ投入結果の確認
PRINT '=== 認証機能初期データ投入結果 ===';
PRINT 'ロール数: ' + CAST((SELECT COUNT(*) FROM [Roles]) AS NVARCHAR(10));
PRINT 'ユーザー数: ' + CAST((SELECT COUNT(*) FROM [Users]) AS NVARCHAR(10));
PRINT 'インシデント数: ' + CAST((SELECT COUNT(*) FROM [Incidents]) AS NVARCHAR(10));

-- ロール別ユーザー数の表示
SELECT 
    r.[Name] AS RoleName,
    COUNT(u.[Id]) AS UserCount
FROM [Roles] r
LEFT JOIN [Users] u ON r.[Id] = u.[Role]
GROUP BY r.[Id], r.[Name]
ORDER BY r.[Id];

PRINT '認証機能初期データの投入が完了しました。';
GO
