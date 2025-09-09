'use client';

import React, { ReactNode } from 'react';
import { useAuth } from '@/contexts/AuthContext';

interface ProtectedRouteProps {
  children: ReactNode;
  fallback?: ReactNode;
}

export function ProtectedRoute({ children, fallback }: ProtectedRouteProps) {
  const { isAuthenticated, isLoading } = useAuth();

  // 認証状態の確認中
  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-2 text-gray-600">認証状態を確認中...</p>
        </div>
      </div>
    );
  }

  // 認証されていない場合
  if (!isAuthenticated) {
    if (fallback) {
      return <>{fallback}</>;
    }
    
    // デフォルトのリダイレクト処理
    if (typeof window !== 'undefined') {
      window.location.href = '/login';
    }
    
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <p className="text-gray-600">ログインページにリダイレクト中...</p>
        </div>
      </div>
    );
  }

  // 認証済みの場合は子コンポーネントを表示
  return <>{children}</>;
}

// ロールベースの保護ルート
interface RoleProtectedRouteProps extends ProtectedRouteProps {
  requiredRoles?: string[];
}

export function RoleProtectedRoute({ 
  children, 
  fallback, 
  requiredRoles = [] 
}: RoleProtectedRouteProps) {
  const { isAuthenticated, isLoading, user } = useAuth();

  // 認証状態の確認中
  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-2 text-gray-600">認証状態を確認中...</p>
        </div>
      </div>
    );
  }

  // 認証されていない場合
  if (!isAuthenticated) {
    if (fallback) {
      return <>{fallback}</>;
    }
    
    if (typeof window !== 'undefined') {
      window.location.href = '/login';
    }
    
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <p className="text-gray-600">ログインページにリダイレクト中...</p>
        </div>
      </div>
    );
  }

  // ロールチェック
  if (requiredRoles.length > 0 && user) {
    const hasRequiredRole = requiredRoles.includes(user.roleName);
    
    if (!hasRequiredRole) {
      return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50">
          <div className="text-center">
            <div className="bg-red-50 border border-red-200 text-red-700 px-6 py-4 rounded-lg">
              <h2 className="text-lg font-semibold mb-2">アクセス権限がありません</h2>
              <p className="text-sm">
                このページにアクセスするには以下のロールが必要です: {requiredRoles.join(', ')}
              </p>
              <p className="text-sm mt-1">
                現在のロール: {user.roleName}
              </p>
            </div>
          </div>
        </div>
      );
    }
  }

  // 認証済みかつロールチェック通過の場合は子コンポーネントを表示
  return <>{children}</>;
}
