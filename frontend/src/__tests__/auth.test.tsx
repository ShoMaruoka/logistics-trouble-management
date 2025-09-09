import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { AuthProvider } from '@/contexts/AuthContext';
import LoginPage from '@/app/login/page';
import '@testing-library/jest-dom';

// APIクライアントをモック
jest.mock('@/lib/api-client', () => ({
  apiClient: {
    post: jest.fn(),
  },
  TokenManager: {
    setAccessToken: jest.fn(),
    clearToken: jest.fn(),
  },
}));

const mockApiClient = require('@/lib/api-client').apiClient;

describe('認証機能テスト', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test('ログインページが正常にレンダリングされる', () => {
    render(
      <AuthProvider>
        <LoginPage />
      </AuthProvider>
    );

    expect(screen.getByText('物流トラブル管理システム')).toBeInTheDocument();
    expect(screen.getByLabelText('ユーザー名またはメールアドレス')).toBeInTheDocument();
    expect(screen.getByLabelText('パスワード')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'ログイン' })).toBeInTheDocument();
  });

  test('ログインフォームの入力が正常に動作する', () => {
    render(
      <AuthProvider>
        <LoginPage />
      </AuthProvider>
    );

    const usernameInput = screen.getByLabelText('ユーザー名またはメールアドレス');
    const passwordInput = screen.getByLabelText('パスワード');

    fireEvent.change(usernameInput, { target: { value: 'admin' } });
    fireEvent.change(passwordInput, { target: { value: 'password123' } });

    expect(usernameInput).toHaveValue('admin');
    expect(passwordInput).toHaveValue('password123');
  });

  test('ログイン成功時の動作', async () => {
    const mockLoginResponse = {
      accessToken: 'mock-access-token',
      expiresAt: '2025-09-04T01:25:19.3357007Z',
      user: {
        id: 1,
        username: 'admin',
        email: 'admin@example.com',
        roleName: 'Admin',
        roleId: 1,
        lastLoginAt: '2025-09-04T01:10:19.3280334Z',
        isActive: true,
      },
    };

    mockApiClient.post.mockResolvedValue(mockLoginResponse);

    render(
      <AuthProvider>
        <LoginPage />
      </AuthProvider>
    );

    const usernameInput = screen.getByLabelText('ユーザー名またはメールアドレス');
    const passwordInput = screen.getByLabelText('パスワード');
    const loginButton = screen.getByRole('button', { name: 'ログイン' });

    fireEvent.change(usernameInput, { target: { value: 'admin' } });
    fireEvent.change(passwordInput, { target: { value: 'password123' } });
    fireEvent.click(loginButton);

    await waitFor(() => {
      expect(mockApiClient.post).toHaveBeenCalledWith('/api/auth/login', {
        usernameOrEmail: 'admin',
        password: 'password123',
      });
    });
  });

  test('ログイン失敗時のエラー表示', async () => {
    const mockError = new Error('認証に失敗しました');
    mockApiClient.post.mockRejectedValue(mockError);

    render(
      <AuthProvider>
        <LoginPage />
      </AuthProvider>
    );

    const usernameInput = screen.getByLabelText('ユーザー名またはメールアドレス');
    const passwordInput = screen.getByLabelText('パスワード');
    const loginButton = screen.getByRole('button', { name: 'ログイン' });

    fireEvent.change(usernameInput, { target: { value: 'invalid' } });
    fireEvent.change(passwordInput, { target: { value: 'invalid' } });
    fireEvent.click(loginButton);

    await waitFor(() => {
      expect(screen.getByText('認証に失敗しました')).toBeInTheDocument();
    });
  });

  test('テストアカウント情報が表示される', () => {
    render(
      <AuthProvider>
        <LoginPage />
      </AuthProvider>
    );

    expect(screen.getByText('テスト用アカウント:')).toBeInTheDocument();
    expect(screen.getByText('ユーザー名: admin')).toBeInTheDocument();
    expect(screen.getByText('パスワード: Admin123!')).toBeInTheDocument();
  });
});
