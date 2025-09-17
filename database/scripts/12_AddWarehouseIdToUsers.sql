-- 物流トラブル管理システム 倉庫担当機能拡張
-- UsersテーブルにWarehouseIdカラムを追加するスクリプト
-- 作成日: 2025-09-09
-- 作成者: システム開発チーム

USE LogisticsTroubleManagement;
GO

-- 1. UsersテーブルにWarehouseIdカラムを追加
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'WarehouseId')
BEGIN
    ALTER TABLE [dbo].[Users] 
    ADD [WarehouseId] INT NULL;
    
    PRINT 'WarehouseIdカラムをUsersテーブルに追加しました。';
END
ELSE
BEGIN
    PRINT 'WarehouseIdカラムは既に存在します。';
END
GO

-- 2. 外部キー制約の追加
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_Warehouse')
BEGIN
    ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT [FK_Users_Warehouse] 
    FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouses]([Id]);
    
    PRINT '外部キー制約 FK_Users_Warehouse を追加しました。';
END
ELSE
BEGIN
    PRINT '外部キー制約 FK_Users_Warehouse は既に存在します。';
END
GO

-- 3. パフォーマンス用インデックスの作成
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_WarehouseId')
BEGIN
    CREATE INDEX [IX_Users_WarehouseId] 
    ON [dbo].[Users] ([WarehouseId]) 
    WHERE [WarehouseId] IS NOT NULL;
    
    PRINT 'インデックス IX_Users_WarehouseId を作成しました。';
END
ELSE
BEGIN
    PRINT 'インデックス IX_Users_WarehouseId は既に存在します。';
END
GO

-- 4. 既存の倉庫担当ユーザーに倉庫を割り当て（一時的）
-- 注意: 実際の運用では管理者が適切な倉庫を設定してください
UPDATE [dbo].[Users] 
SET [WarehouseId] = (
    SELECT TOP 1 [Id] 
    FROM [dbo].[Warehouses] 
    WHERE [IsActive] = 1 
    ORDER BY [Id]
)
WHERE [Role] = 3  -- 倉庫担当ロール
  AND [WarehouseId] IS NULL
  AND [IsActive] = 1;

PRINT '既存の倉庫担当ユーザーに倉庫を割り当てました。';
GO

-- 5. データ整合性の確認
PRINT '=== データ整合性チェック ===';

-- 倉庫担当ユーザーの確認
SELECT 
    u.[Id],
    u.[Username],
    u.[Email],
    u.[Role],
    u.[WarehouseId],
    w.[Name] AS [WarehouseName]
FROM [dbo].[Users] u
LEFT JOIN [dbo].[Warehouses] w ON u.[WarehouseId] = w.[Id]
WHERE u.[Role] = 3  -- 倉庫担当ロール
ORDER BY u.[Id];

-- 倉庫担当で倉庫が未設定のユーザーをチェック
IF EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Role] = 3 AND [WarehouseId] IS NULL AND [IsActive] = 1)
BEGIN
    PRINT '警告: 倉庫が未設定の倉庫担当ユーザーが存在します。';
    SELECT 
        [Id], [Username], [Email]
    FROM [dbo].[Users] 
    WHERE [Role] = 3 AND [WarehouseId] IS NULL AND [IsActive] = 1;
END
ELSE
BEGIN
    PRINT 'OK: すべての倉庫担当ユーザーに倉庫が設定されています。';
END

-- 無効な倉庫IDを参照しているユーザーをチェック
IF EXISTS (
    SELECT 1 
    FROM [dbo].[Users] u
    LEFT JOIN [dbo].[Warehouses] w ON u.[WarehouseId] = w.[Id]
    WHERE u.[WarehouseId] IS NOT NULL 
      AND w.[Id] IS NULL
)
BEGIN
    PRINT 'エラー: 存在しない倉庫IDを参照しているユーザーが存在します。';
    SELECT 
        u.[Id], u.[Username], u.[Email], u.[WarehouseId]
    FROM [dbo].[Users] u
    LEFT JOIN [dbo].[Warehouses] w ON u.[WarehouseId] = w.[Id]
    WHERE u.[WarehouseId] IS NOT NULL 
      AND w.[Id] IS NULL;
END
ELSE
BEGIN
    PRINT 'OK: すべてのユーザーが有効な倉庫IDを参照しています。';
END

PRINT '=== スクリプト実行完了 ===';
GO
