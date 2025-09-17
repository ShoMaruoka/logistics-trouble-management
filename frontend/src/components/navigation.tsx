'use client';

import * as React from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { cn } from "@/lib/utils";
import { useAuth } from "@/contexts/AuthContext";
import { useRolePermissions } from "@/components/RoleBasedAccess";
import { Button } from "@/components/ui/button";
import { 
  Home, 
  AlertTriangle, 
  BarChart3, 
  FileText, 
  TrendingUp,
  Settings,
  Database,
  LogOut,
  User,
  Users,
  Shield,
  Warehouse,
  CheckCircle2,
  Clock,
  Filter
} from "lucide-react";

// ナビゲーション項目の定義（権限チェック付き）
const getNavigationItems = (permissions: ReturnType<typeof useRolePermissions>, userRoleId?: number) => [
  { name: 'ダッシュボード', href: '/', icon: Home, show: true },
  { name: 'トラブル管理', href: '/incidents', icon: AlertTriangle, show: permissions.canViewIncidents },
  { name: '統計・分析', href: '/statistics', icon: BarChart3, show: permissions.canViewStatistics },
  { name: 'ファイル管理', href: '/attachments', icon: FileText, show: permissions.canViewIncidents },
  { name: '効果測定', href: '/effectiveness', icon: TrendingUp, show: permissions.canManageEffectiveness },
  // 倉庫担当専用メニュー
  { name: '未解決インシデント', href: '/warehouse-staff/unresolved', icon: AlertTriangle, show: userRoleId === 3 },
  { name: '対応中インシデント', href: '/warehouse-staff/in-progress', icon: Clock, show: userRoleId === 3 },
  { name: '担当倉庫インシデント', href: '/warehouse-staff/assigned-warehouse', icon: Warehouse, show: userRoleId === 3 },
  { name: '分類済みインシデント', href: '/warehouse-staff/classified', icon: CheckCircle2, show: userRoleId === 3 },
  // 管理者専用メニュー
  { name: 'マスタ管理', href: '/master', icon: Database, show: permissions.canManageMasters },
  { name: 'ユーザー管理', href: '/users', icon: Users, show: permissions.canManageUsers },
  { name: 'ロール管理', href: '/roles', icon: Shield, show: permissions.canManageUsers },
  { name: '設定', href: '/settings', icon: Settings, show: permissions.canManageMasters },
];

export function Navigation() {
  const pathname = usePathname();
  const { user, isAuthenticated, logout } = useAuth();
  const permissions = useRolePermissions();

  const handleLogout = async () => {
    try {
      await logout();
    } catch (error) {
      console.error('ログアウトエラー:', error);
    }
  };

  // 認証されていない場合はナビゲーションを表示しない
  if (!isAuthenticated) {
    return null;
  }

  // 権限に基づいてナビゲーション項目をフィルタリング
  const navigationItems = getNavigationItems(permissions, user?.roleId).filter(item => item.show);

  return (
    <nav className="bg-white shadow-sm border-b">
      <div className="container mx-auto px-6">
        <div className="flex items-center justify-between h-16">
          {/* ロゴ */}
          <div className="flex items-center">
            <Link href="/" className="flex items-center space-x-2">
              <AlertTriangle className="h-8 w-8 text-blue-600" />
              <span className="text-xl font-bold text-gray-900">
                物流トラブル管理
              </span>
            </Link>
          </div>

          {/* ナビゲーションリンク */}
          <div className="hidden md:flex items-center space-x-8">
            {navigationItems.map((item) => {
              const isActive = pathname === item.href;
              return (
                <Link
                  key={item.name}
                  href={item.href}
                  className={cn(
                    "flex items-center space-x-2 px-3 py-2 rounded-md text-sm font-medium transition-colors",
                    isActive
                      ? "bg-blue-100 text-blue-700"
                      : "text-gray-600 hover:text-gray-900 hover:bg-gray-100"
                  )}
                >
                  <item.icon className="h-4 w-4" />
                  <span>{item.name}</span>
                </Link>
              );
            })}
          </div>

          {/* ユーザー情報とログアウトボタン */}
          <div className="hidden md:flex items-center space-x-4">
            {user && (
              <div className="flex items-center space-x-2 text-sm text-gray-600">
                <User className="h-4 w-4" />
                <span>{user.username}</span>
                <span className="text-gray-400">({user.roleName})</span>
              </div>
            )}
            <Button
              variant="outline"
              size="sm"
              onClick={handleLogout}
              className="flex items-center space-x-2"
            >
              <LogOut className="h-4 w-4" />
              <span>ログアウト</span>
            </Button>
          </div>

          {/* モバイルメニューボタン */}
          <div className="md:hidden">
            <button className="text-gray-600 hover:text-gray-900">
              <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
}
