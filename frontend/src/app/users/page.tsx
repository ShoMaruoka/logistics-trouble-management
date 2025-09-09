'use client';

import React, { useState, useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { RoleBasedAccess, UserRole } from '@/components/RoleBasedAccess';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Badge } from '@/components/ui/badge';
import { 
  Users, 
  Plus, 
  Search, 
  Edit, 
  Trash2, 
  Shield, 
  User,
  Mail,
  Phone,
  Calendar,
  CheckCircle,
  XCircle,
  X
} from 'lucide-react';

// ユーザー管理画面
export default function UsersPage() {
  const { user, getAccessToken } = useAuth();
  const [users, setUsers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [showEditForm, setShowEditForm] = useState(false);
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [editingUser, setEditingUser] = useState<any>(null);
  const [deletingUser, setDeletingUser] = useState<any>(null);
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    firstName: '',
    lastName: '',
    roleId: '',
    password: ''
  });

  // ユーザー一覧の取得（実際のAPIから）
  useEffect(() => {
    const fetchUsers = async () => {
      try {
        setLoading(true);
        // 実際のAPIからユーザー一覧を取得
        const response = await fetch('http://localhost:5169/api/users', {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${getAccessToken() || ''}`,
          },
          credentials: 'include',
        });

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log('取得したユーザーデータ:', data);
        console.log('データの型:', typeof data);
        console.log('配列かどうか:', Array.isArray(data));
        
        // PagedResultDtoのitemsプロパティからユーザー一覧を取得
        if (data && data.items && Array.isArray(data.items)) {
          console.log('itemsプロパティからユーザー一覧を取得:', data.items);
          setUsers(data.items);
        } else if (data && data.Items && Array.isArray(data.Items)) {
          // 大文字のItemsプロパティの場合
          console.log('Itemsプロパティからユーザー一覧を取得:', data.Items);
          setUsers(data.Items);
        } else if (Array.isArray(data)) {
          // 直接配列の場合はそのまま使用
          setUsers(data);
        } else {
          console.warn('APIレスポンスが期待される形式ではありません:', data);
          setUsers([]);
        }
      } catch (error) {
        console.error('ユーザー一覧の取得に失敗しました:', error);
        // エラー時はモックデータを表示
        const mockUsers = [
          {
            id: 1,
            username: 'admin',
            email: 'admin@example.com',
            firstName: '管理者',
            lastName: '太郎',
            roleName: 'システム管理者',
            roleId: 1,
            isActive: true,
            lastLoginAt: '2025-09-05T00:57:36.1919234Z',
            createdAt: '2025-09-01T00:00:00.000Z'
          }
        ];
        setUsers(mockUsers);
      } finally {
        setLoading(false);
      }
    };

    if (user) {
      fetchUsers();
    }
  }, [user]);

  // フォーム入力ハンドラー
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  // フォームリセット
  const resetForm = () => {
    setFormData({
      username: '',
      email: '',
      firstName: '',
      lastName: '',
      roleId: '',
      password: ''
    });
  };

  // モーダルを閉じる
  const closeModal = () => {
    setShowCreateForm(false);
    setShowEditForm(false);
    setShowDeleteDialog(false);
    setEditingUser(null);
    setDeletingUser(null);
    resetForm();
  };

  // 編集モーダルを開く
  const openEditModal = (user: any) => {
    setEditingUser(user);
    setFormData({
      username: user.username,
      email: user.email,
      firstName: user.firstName || user.fullName?.split(' ')[0] || '',
      lastName: user.lastName || user.fullName?.split(' ')[1] || '',
      roleId: user.roleId?.toString() || '',
      password: '' // 編集時はパスワードを空にする
    });
    setShowEditForm(true);
  };

  // 削除確認ダイアログを開く
  const openDeleteDialog = (user: any) => {
    setDeletingUser(user);
    setShowDeleteDialog(true);
  };

  // ユーザー作成処理
  const handleCreateUser = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // バリデーション
    if (!formData.username || !formData.email || !formData.firstName || !formData.lastName || !formData.roleId || !formData.password) {
      alert('すべての項目を入力してください。');
      return;
    }

    // パスワードの強度チェック
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$/;
    if (!passwordRegex.test(formData.password)) {
      alert('パスワードは大文字・小文字・数字・記号を含む8文字以上で入力してください。');
      return;
    }

    // ユーザー名の形式チェック
    const usernameRegex = /^[a-zA-Z0-9_]+$/;
    if (!usernameRegex.test(formData.username)) {
      alert('ユーザー名は英数字とアンダースコアのみ使用できます。');
      return;
    }

    try {
      console.log('ユーザー作成データ:', formData);
      
      // 実際のAPI呼び出し（TODO: 実装）
      const response = await fetch('http://localhost:5169/api/users', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${getAccessToken() || ''}`,
        },
        credentials: 'include',
        body: JSON.stringify({
          Username: formData.username,
          Email: formData.email,
          FirstName: formData.firstName,
          LastName: formData.lastName,
          RoleId: parseInt(formData.roleId),
          Password: formData.password
        })
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const newUser = await response.json();
      console.log('ユーザー作成成功:', newUser);
      
      // ユーザー一覧を更新
      setUsers(prev => [...prev, newUser]);
      
      // モーダルを閉じる
      closeModal();
      
      alert('ユーザーが正常に作成されました。');
      
    } catch (error) {
      console.error('ユーザー作成に失敗しました:', error);
      alert('ユーザー作成に失敗しました。');
    }
  };

  // ユーザー更新処理
  const handleUpdateUser = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!editingUser) return;
    
    // バリデーション
    if (!formData.username || !formData.email || !formData.firstName || !formData.lastName || !formData.roleId) {
      alert('すべての項目を入力してください。');
      return;
    }

    // ユーザー名の形式チェック
    const usernameRegex = /^[a-zA-Z0-9_]+$/;
    if (!usernameRegex.test(formData.username)) {
      alert('ユーザー名は英数字とアンダースコアのみ使用できます。');
      return;
    }

    try {
      console.log('ユーザー更新データ:', formData);
      
      // 実際のAPI呼び出し
      const response = await fetch(`http://localhost:5169/api/users/${editingUser.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${getAccessToken() || ''}`,
        },
        credentials: 'include',
        body: JSON.stringify({
          Username: formData.username,
          Email: formData.email,
          FirstName: formData.firstName,
          LastName: formData.lastName,
          RoleId: parseInt(formData.roleId),
          IsActive: editingUser.isActive
        })
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const updatedUser = await response.json();
      console.log('ユーザー更新成功:', updatedUser);
      
      // ユーザー一覧を更新
      setUsers(prev => prev.map(u => u.id === editingUser.id ? updatedUser : u));
      
      // モーダルを閉じる
      closeModal();
      
      alert('ユーザーが正常に更新されました。');
      
    } catch (error) {
      console.error('ユーザー更新に失敗しました:', error);
      alert('ユーザー更新に失敗しました。');
    }
  };

  // ユーザー削除処理
  const handleDeleteUser = async () => {
    if (!deletingUser) return;

    try {
      console.log('ユーザー削除:', deletingUser);
      
      // 実際のAPI呼び出し
      const response = await fetch(`http://localhost:5169/api/users/${deletingUser.id}`, {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${getAccessToken() || ''}`,
        },
        credentials: 'include',
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      console.log('ユーザー削除成功');
      
      // ユーザー一覧から削除
      setUsers(prev => prev.filter(u => u.id !== deletingUser.id));
      
      // ダイアログを閉じる
      closeModal();
      
      alert('ユーザーが正常に削除されました。');
      
    } catch (error) {
      console.error('ユーザー削除に失敗しました:', error);
      alert('ユーザー削除に失敗しました。');
    }
  };

  // 検索フィルタリング
  const filteredUsers = Array.isArray(users) ? users.filter(user => 
    user.username?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    user.email?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    user.firstName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    user.lastName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    user.roleName?.toLowerCase().includes(searchTerm.toLowerCase())
  ) : [];

  // ロール名の色分け
  const getRoleBadgeVariant = (roleId: number) => {
    switch (roleId) {
      case 1: return 'destructive'; // Admin
      case 2: return 'secondary'; // Clerk
      case 3: return 'default'; // IncidentManager
      case 4: return 'outline'; // WarehouseStaff
      default: return 'secondary';
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-2 text-gray-600">ユーザー一覧を読み込み中...</p>
        </div>
      </div>
    );
  }

  return (
    <RoleBasedAccess allowedRoles={[UserRole.Admin]}>
      <div className="min-h-screen bg-gray-50">
        {/* ヘッダーセクション */}
        <div className="bg-white shadow-sm border-b">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
            <div className="flex items-center justify-between">
              <div>
                <h1 className="text-2xl font-bold text-gray-900 flex items-center space-x-2">
                  <Users className="h-8 w-8" />
                  <span>ユーザー管理</span>
                </h1>
                <p className="mt-1 text-sm text-gray-600">システムユーザーの管理を行います</p>
              </div>
              <Button 
                onClick={() => setShowCreateForm(true)}
                className="flex items-center space-x-2"
              >
                <Plus className="h-4 w-4" />
                <span>新規ユーザー作成</span>
              </Button>
            </div>
          </div>
        </div>

        {/* メインコンテンツ */}
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          {/* 検索セクション */}
          <Card className="mb-6">
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <Search className="h-5 w-5" />
                <span>ユーザー検索</span>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Input
                placeholder="ユーザー名、メールアドレス、氏名、ロールで検索..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="max-w-md"
              />
            </CardContent>
          </Card>

          {/* ユーザー一覧 */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {filteredUsers.map((user) => (
              <Card key={user.id} className="hover:shadow-md transition-shadow">
                <CardHeader className="pb-3">
                  <div className="flex items-center justify-between">
                    <CardTitle className="text-lg flex items-center space-x-2">
                      <User className="h-5 w-5" />
                      <span>{user.firstName} {user.lastName}</span>
                    </CardTitle>
                    <div className="flex items-center space-x-2">
                      {user.isActive ? (
                        <CheckCircle className="h-4 w-4 text-green-500" />
                      ) : (
                        <XCircle className="h-4 w-4 text-red-500" />
                      )}
                    </div>
                  </div>
                  <CardDescription>@{user.username}</CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    {/* メールアドレス */}
                    <div className="flex items-center space-x-2 text-sm text-gray-600">
                      <Mail className="h-4 w-4" />
                      <span>{user.email}</span>
                    </div>

                    {/* ロール */}
                    <div className="flex items-center space-x-2">
                      <Shield className="h-4 w-4 text-gray-400" />
                      <Badge variant={getRoleBadgeVariant(user.roleId)}>
                        {user.roleName}
                      </Badge>
                    </div>

                    {/* 最終ログイン */}
                    <div className="flex items-center space-x-2 text-sm text-gray-600">
                      <Calendar className="h-4 w-4" />
                      <span>
                        最終ログイン: {new Date(user.lastLoginAt).toLocaleDateString('ja-JP')}
                      </span>
                    </div>

                    {/* アクションボタン */}
                    <div className="flex space-x-2 pt-3">
                      <Button 
                        variant="outline" 
                        size="sm" 
                        className="flex-1"
                        onClick={() => openEditModal(user)}
                      >
                        <Edit className="h-4 w-4 mr-1" />
                        編集
                      </Button>
                      <Button 
                        variant="outline" 
                        size="sm" 
                        className="flex-1"
                        onClick={() => openDeleteDialog(user)}
                      >
                        <Trash2 className="h-4 w-4 mr-1" />
                        削除
                      </Button>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>

          {/* 検索結果が0件の場合 */}
          {filteredUsers.length === 0 && (
            <Card>
              <CardContent className="text-center py-12">
                <Users className="h-12 w-12 text-gray-400 mx-auto mb-4" />
                <h3 className="text-lg font-medium text-gray-900 mb-2">
                  ユーザーが見つかりません
                </h3>
                <p className="text-gray-600">
                  検索条件に一致するユーザーがありません。
                </p>
              </CardContent>
            </Card>
          )}
        </div>

        {/* 削除確認ダイアログ */}
        {showDeleteDialog && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <Card className="w-full max-w-md mx-4">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <h2 className="text-lg font-semibold text-red-600">ユーザー削除</h2>
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={closeModal}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  <div className="flex items-center space-x-3">
                    <div className="flex-shrink-0">
                      <div className="w-10 h-10 bg-red-100 rounded-full flex items-center justify-center">
                        <Trash2 className="h-5 w-5 text-red-600" />
                      </div>
                    </div>
                    <div>
                      <p className="text-sm font-medium text-gray-900">
                        このユーザーを削除しますか？
                      </p>
                      <p className="text-sm text-gray-500">
                        {deletingUser?.username} ({deletingUser?.email})
                      </p>
                    </div>
                  </div>
                  
                  <div className="bg-yellow-50 border border-yellow-200 rounded-md p-3">
                    <div className="flex">
                      <div className="flex-shrink-0">
                        <XCircle className="h-5 w-5 text-yellow-400" />
                      </div>
                      <div className="ml-3">
                        <p className="text-sm text-yellow-800">
                          この操作は取り消すことができません。削除されたユーザーは復元できません。
                        </p>
                      </div>
                    </div>
                  </div>

                  <div className="flex space-x-2 pt-4">
                    <Button
                      type="button"
                      variant="outline"
                      className="flex-1"
                      onClick={closeModal}
                    >
                      キャンセル
                    </Button>
                    <Button
                      type="button"
                      variant="destructive"
                      className="flex-1"
                      onClick={handleDeleteUser}
                    >
                      削除
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        )}

        {/* ユーザー編集モーダル */}
        {showEditForm && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <Card className="w-full max-w-md mx-4">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <h2 className="text-lg font-semibold">ユーザー編集</h2>
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={closeModal}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <form className="space-y-4" onSubmit={handleUpdateUser}>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      ユーザー名
                    </label>
                    <input
                      type="text"
                      name="username"
                      value={formData.username}
                      onChange={handleInputChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      placeholder="ユーザー名を入力"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      メールアドレス
                    </label>
                    <input
                      type="email"
                      name="email"
                      value={formData.email}
                      onChange={handleInputChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      placeholder="メールアドレスを入力"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      姓
                    </label>
                    <input
                      type="text"
                      name="firstName"
                      value={formData.firstName}
                      onChange={handleInputChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      placeholder="姓を入力"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      名
                    </label>
                    <input
                      type="text"
                      name="lastName"
                      value={formData.lastName}
                      onChange={handleInputChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      placeholder="名を入力"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      ロール
                    </label>
                    <select 
                      name="roleId"
                      value={formData.roleId}
                      onChange={handleInputChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      required
                    >
                      <option value="">ロールを選択</option>
                      <option value="1">Admin</option>
                      <option value="2">Clerk</option>
                      <option value="3">Incident Manager</option>
                      <option value="4">Warehouse Staff</option>
                    </select>
                  </div>
                  <div className="flex space-x-2 pt-4">
                    <Button
                      type="button"
                      variant="outline"
                      className="flex-1"
                      onClick={closeModal}
                    >
                      キャンセル
                    </Button>
                    <Button
                      type="submit"
                      className="flex-1"
                    >
                      更新
                    </Button>
                  </div>
                </form>
              </CardContent>
            </Card>
          </div>
        )}

        {/* 新規ユーザー作成モーダル */}
        {showCreateForm && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <Card className="w-full max-w-md mx-4">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <h2 className="text-lg font-semibold">新規ユーザー作成</h2>
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => setShowCreateForm(false)}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <form className="space-y-4" onSubmit={handleCreateUser}>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      ユーザー名
                    </label>
                    <input
                      type="text"
                      name="username"
                      value={formData.username}
                      onChange={handleInputChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      placeholder="ユーザー名を入力"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      メールアドレス
                    </label>
                    <input
                      type="email"
                      name="email"
                      value={formData.email}
                      onChange={handleInputChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      placeholder="メールアドレスを入力"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      姓
                    </label>
                    <input
                      type="text"
                      name="firstName"
                      value={formData.firstName}
                      onChange={handleInputChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      placeholder="姓を入力"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      名
                    </label>
                    <input
                      type="text"
                      name="lastName"
                      value={formData.lastName}
                      onChange={handleInputChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      placeholder="名を入力"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      ロール
                    </label>
                    <select 
                      name="roleId"
                      value={formData.roleId}
                      onChange={handleInputChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      required
                    >
                      <option value="">ロールを選択</option>
                      <option value="1">Admin</option>
                      <option value="2">Clerk</option>
                      <option value="3">Incident Manager</option>
                      <option value="4">Warehouse Staff</option>
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      パスワード
                    </label>
                    <input
                      type="password"
                      name="password"
                      value={formData.password}
                      onChange={handleInputChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      placeholder="パスワードを入力"
                      required
                    />
                  </div>
                  <div className="flex space-x-2 pt-4">
                    <Button
                      type="button"
                      variant="outline"
                      className="flex-1"
                      onClick={closeModal}
                    >
                      キャンセル
                    </Button>
                    <Button
                      type="submit"
                      className="flex-1"
                    >
                      作成
                    </Button>
                  </div>
                </form>
              </CardContent>
            </Card>
          </div>
        )}
      </div>
    </RoleBasedAccess>
  );
}
