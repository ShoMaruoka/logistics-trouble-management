'use client';

import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { TokenManager, apiClient } from '@/lib/api-client';
import { RefreshTokenManager } from '@/lib/refresh-token-manager';

// 認証関連の型定義
export interface User {
  id: number;
  username: string;
  email: string;
  roleName: string;
  roleId: number;
  lastLoginAt: string;
  isActive: boolean;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface AuthState {
  user: User | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}

export interface AuthContextType extends AuthState {
  login: (usernameOrEmail: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  refreshToken: () => Promise<void>;
  getAccessToken: () => string | null;
}

// 認証コンテキストの作成
const AuthContext = createContext<AuthContextType | undefined>(undefined);

// 認証プロバイダーのプロパティ
interface AuthProviderProps {
  children: ReactNode;
}

// 認証プロバイダーコンポーネント
export function AuthProvider({ children }: AuthProviderProps) {
  const [authState, setAuthState] = useState<AuthState>({
    user: null,
    accessToken: null,
    isAuthenticated: false,
    isLoading: true,
  });

  // 初期化時にローカルストレージから認証状態を復元
  useEffect(() => {
    const initializeAuth = async () => {
      try {
        // メモリ内トークン管理のため、ローカルストレージは使用しない
        // 代わりに、アプリケーション起動時に認証状態をクリア
        setAuthState({
          user: null,
          accessToken: null,
          isAuthenticated: false,
          isLoading: false,
        });
      } catch (error) {
        console.error('認証状態の初期化に失敗しました:', error);
        setAuthState({
          user: null,
          accessToken: null,
          isAuthenticated: false,
          isLoading: false,
        });
      }
    };

    initializeAuth();
  }, []);

  // ログイン処理
  const login = async (usernameOrEmail: string, password: string): Promise<void> => {
    try {
      setAuthState(prev => ({ ...prev, isLoading: true }));

      const loginData = await apiClient.post<LoginResponse>('/api/auth/login', {
        usernameOrEmail,
        password,
      });

      // 認証状態を更新（メモリ内のみ）
      setAuthState({
        user: loginData.user,
        accessToken: loginData.accessToken,
        isAuthenticated: true,
        isLoading: false,
      });

      // TokenManagerにトークンを設定
      TokenManager.setAccessToken(loginData.accessToken);

      // リフレッシュトークンを保存
      RefreshTokenManager.setRefreshToken(loginData.refreshToken);
    } catch (error) {
      console.error('ログインエラー:', error);
      setAuthState(prev => ({ ...prev, isLoading: false }));
      throw error;
    }
  };

  // ログアウト処理
  const logout = async (): Promise<void> => {
    try {
      const refreshToken = RefreshTokenManager.getRefreshToken();
      if (refreshToken) {
        // リフレッシュトークンを無効化
        await apiClient.revokeToken(refreshToken, 'User logout');
      }
    } catch (error) {
      console.error('ログアウトAPI呼び出しエラー:', error);
    } finally {
      // 認証状態をクリア
      setAuthState({
        user: null,
        accessToken: null,
        isAuthenticated: false,
        isLoading: false,
      });

      // TokenManagerからトークンをクリア
      TokenManager.clearToken();
      
      // リフレッシュトークンをクリア
      RefreshTokenManager.clearRefreshToken();
    }
  };

  // トークンリフレッシュ処理
  const refreshToken = async (): Promise<void> => {
    try {
      const currentRefreshToken = RefreshTokenManager.getRefreshToken();
      if (!currentRefreshToken) {
        throw new Error('No refresh token available');
      }

      const refreshData = await apiClient.refreshToken(currentRefreshToken);

      // 認証状態を更新
      setAuthState(prev => ({
        ...prev,
        accessToken: refreshData.accessToken,
        isAuthenticated: true,
      }));

      // TokenManagerにトークンを設定
      TokenManager.setAccessToken(refreshData.accessToken);
      
      // 新しいリフレッシュトークンを保存
      RefreshTokenManager.setRefreshToken(refreshData.refreshToken);
    } catch (error) {
      console.error('トークンリフレッシュエラー:', error);
      // リフレッシュに失敗した場合はログアウト
      await logout();
      throw error;
    }
  };

  // 自動リフレッシュ機能
  useEffect(() => {
    let refreshInterval: NodeJS.Timeout | null = null;

    if (authState.isAuthenticated && authState.accessToken) {
      // アクセストークンの有効期限をチェック（5分前にリフレッシュ）
      const checkAndRefreshToken = async () => {
        try {
          // トークンの有効期限をチェック（簡易版）
          const token = authState.accessToken;
          if (token) {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const expirationTime = payload.exp * 1000;
            const currentTime = Date.now();
            const timeUntilExpiry = expirationTime - currentTime;
            
            // 5分前になったらリフレッシュ
            if (timeUntilExpiry < 5 * 60 * 1000) {
              await refreshToken();
            }
          }
        } catch (error) {
          console.error('自動リフレッシュエラー:', error);
        }
      };

      // 1分ごとにチェック
      refreshInterval = setInterval(checkAndRefreshToken, 60 * 1000);
    }

    return () => {
      if (refreshInterval) {
        clearInterval(refreshInterval);
      }
    };
  }, [authState.isAuthenticated, authState.accessToken, refreshToken]);

  const getAccessToken = () => {
    return authState.accessToken;
  };

  const contextValue: AuthContextType = {
    ...authState,
    login,
    logout,
    refreshToken,
    getAccessToken,
  };

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  );
}

// 認証フック
export function useAuth(): AuthContextType {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

// 認証が必要なコンポーネント用のヘルパー
export function useRequireAuth(): AuthContextType {
  const auth = useAuth();
  
  useEffect(() => {
    if (!auth.isLoading && !auth.isAuthenticated) {
      // 認証されていない場合はログインページにリダイレクト
      window.location.href = '/login';
    }
  }, [auth.isLoading, auth.isAuthenticated]);

  return auth;
}
