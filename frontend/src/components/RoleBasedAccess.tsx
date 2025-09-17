'use client';

import React, { ReactNode } from 'react';
import { useAuth } from '@/contexts/AuthContext';

// ロール定義
export enum UserRole {
  Admin = 1,
  IncidentManager = 2,
  WarehouseStaff = 3,
  Clerk = 4,
}

// ロール名のマッピング
export const ROLE_NAMES: Record<UserRole, string> = {
  [UserRole.Admin]: 'システム管理者',
  [UserRole.Clerk]: '事務員',
  [UserRole.IncidentManager]: 'インシデント管理者',
  [UserRole.WarehouseStaff]: '倉庫担当',
};

// 権限レベル
export enum PermissionLevel {
  Low = 1,
  Medium = 2,
  High = 3,
}

// ロールの権限レベル
export const ROLE_PERMISSIONS: Record<UserRole, PermissionLevel> = {
  [UserRole.Admin]: PermissionLevel.High,
  [UserRole.Clerk]: PermissionLevel.Low,
  [UserRole.IncidentManager]: PermissionLevel.Medium,
  [UserRole.WarehouseStaff]: PermissionLevel.Medium,
};

// 権限ベースアクセス制御コンポーネントのプロパティ
interface RoleBasedAccessProps {
  children: ReactNode;
  allowedRoles?: UserRole[];
  requiredPermission?: PermissionLevel;
  fallback?: ReactNode;
}

// 権限ベースアクセス制御コンポーネント
export function RoleBasedAccess({ 
  children, 
  allowedRoles, 
  requiredPermission, 
  fallback = null 
}: RoleBasedAccessProps) {
  const { user, isAuthenticated } = useAuth();

  // 認証されていない場合は何も表示しない
  if (!isAuthenticated || !user) {
    return <>{fallback}</>;
  }

  const userRole = user.roleId as UserRole;
  const userPermission = ROLE_PERMISSIONS[userRole];

  // 特定のロールのみ許可する場合
  if (allowedRoles && !allowedRoles.includes(userRole)) {
    return <>{fallback}</>;
  }

  // 権限レベルが必要な場合
  if (requiredPermission && userPermission < requiredPermission) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
}

// ロール別表示用のヘルパー関数
export function useRolePermissions() {
  const { user, isAuthenticated } = useAuth();

  if (!isAuthenticated || !user) {
    return {
      role: null,
      roleName: '',
      permissionLevel: PermissionLevel.Low,
      canViewIncidents: false,
      canCreateIncidents: false,
      canEditIncidents: false,
      canDeleteIncidents: false,
      canClassifyIncidents: false,
      canRegisterSolutions: false,
      canManageEffectiveness: false,
      canViewStatistics: false,
      canManageMasters: false,
      canManageUsers: false,
    };
  }

  const role = user.roleId as UserRole;
  const roleName = ROLE_NAMES[role];
  const permissionLevel = ROLE_PERMISSIONS[role];

  return {
    role,
    roleName,
    permissionLevel,
    // 権限マトリックスに基づく権限チェック
    canViewIncidents: true, // 全ロールで閲覧可能
    canCreateIncidents: true, // 全ロールで登録可能
    canEditIncidents: true, // 全ロールで編集可能
    canDeleteIncidents: role === UserRole.Admin, // 管理者のみ削除可能
    canClassifyIncidents: role === UserRole.IncidentManager || role === UserRole.Admin, // インシデント管理者以上
    canRegisterSolutions: role === UserRole.WarehouseStaff || role === UserRole.Admin, // 倉庫担当以上
    canManageEffectiveness: role === UserRole.IncidentManager || role === UserRole.WarehouseStaff || role === UserRole.Admin, // インシデント管理者以上
    canViewStatistics: true, // 全ロールで統計閲覧可能
    canManageMasters: role === UserRole.Admin, // 管理者のみマスタ管理可能
    canManageUsers: role === UserRole.Admin, // 管理者のみユーザー管理可能
  };
}

// ロール別ダッシュボード用のヘルパー関数
export function useDashboardConfig() {
  const { user, isAuthenticated } = useAuth();

  if (!isAuthenticated || !user) {
    return {
      title: '物流トラブル管理システム',
      subtitle: 'ログインしてください',
      showStatistics: false,
      showIncidentManagement: false,
      showMasterManagement: false,
      showUserManagement: false,
      primaryActions: [],
    };
  }

  const role = user.roleId as UserRole;
  const roleName = ROLE_NAMES[role];

  switch (role) {
    case UserRole.Admin:
      return {
        title: `物流トラブル管理システム - ${roleName}`,
        subtitle: 'システム全体の管理を行います',
        showStatistics: true,
        showIncidentManagement: true,
        showMasterManagement: true,
        showUserManagement: true,
        primaryActions: [
          { label: 'ユーザー管理', action: 'manage-users' },
          { label: 'マスタ管理', action: 'manage-masters' },
          { label: 'システム設定', action: 'system-settings' },
        ],
      };

    case UserRole.Clerk:
      return {
        title: `物流トラブル管理システム - ${roleName}`,
        subtitle: '基本的なインシデント管理を行います',
        showStatistics: true,
        showIncidentManagement: true,
        showMasterManagement: false,
        showUserManagement: false,
        primaryActions: [
          { label: '新規インシデント登録', action: 'create-incident' },
          { label: '担当インシデント確認', action: 'view-assigned' },
        ],
      };

    case UserRole.IncidentManager:
      return {
        title: `物流トラブル管理システム - ${roleName}`,
        subtitle: 'インシデントの分類・管理を行います',
        showStatistics: true,
        showIncidentManagement: true,
        showMasterManagement: false,
        showUserManagement: false,
        primaryActions: [
          { label: 'インシデント分類', action: 'classify-incidents' },
          { label: '優先度設定', action: 'set-priority' },
          { label: '担当者割り当て', action: 'assign-responsible' },
        ],
      };

    case UserRole.WarehouseStaff:
      return {
        title: `物流トラブル管理システム - ${roleName}`,
        subtitle: '解決策の検討・登録を行います',
        showStatistics: true,
        showIncidentManagement: true,
        showMasterManagement: false,
        showUserManagement: false,
        primaryActions: [
          { label: '解決策登録', action: 'register-solution' },
          { label: '効果測定実施', action: 'measure-effectiveness' },
          { label: '再発防止策提案', action: 'prevent-recurrence' },
        ],
      };

    default:
      return {
        title: '物流トラブル管理システム',
        subtitle: 'ログインしてください',
        showStatistics: false,
        showIncidentManagement: false,
        showMasterManagement: false,
        showUserManagement: false,
        primaryActions: [],
      };
  }
}
