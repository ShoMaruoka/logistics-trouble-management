'use client';

import React, { useState, useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { RoleBasedAccess, UserRole } from '@/components/RoleBasedAccess';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Badge } from '@/components/ui/badge';
import { 
  Shield, 
  Plus, 
  Search, 
  Edit, 
  Trash2, 
  Users,
  CheckCircle,
  XCircle,
  AlertTriangle
} from 'lucide-react';

// ロール管理画面
export default function RolesPage() {
  const { user, getAccessToken } = useAuth();
  const [roles, setRoles] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [showCreateForm, setShowCreateForm] = useState(false);

  // ロール一覧の取得（実際のAPIから）
  useEffect(() => {
    const fetchRoles = async () => {
      try {
        setLoading(true);
        // 実際のAPIからロール一覧を取得
        const response = await fetch('http://localhost:5169/api/roles', {
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
        console.log('取得したロールデータ:', data);
        console.log('データの型:', typeof data);
        console.log('配列かどうか:', Array.isArray(data));
        
        // PagedResultDtoのitemsプロパティからロール一覧を取得
        if (data && data.items && Array.isArray(data.items)) {
          console.log('itemsプロパティからロール一覧を取得:', data.items);
          setRoles(data.items);
        } else if (data && data.Items && Array.isArray(data.Items)) {
          // 大文字のItemsプロパティの場合
          console.log('Itemsプロパティからロール一覧を取得:', data.Items);
          setRoles(data.Items);
        } else if (Array.isArray(data)) {
          // 直接配列の場合はそのまま使用
          setRoles(data);
        } else {
          console.warn('APIレスポンスが期待される形式ではありません:', data);
          setRoles([]);
        }
      } catch (error) {
        console.error('ロール一覧の取得に失敗しました:', error);
        // エラー時はモックデータを表示
        const mockRoles = [
          {
            id: 1,
            name: 'Admin',
            displayName: 'システム管理者',
            description: 'システム全体の管理権限を持つ管理者ロール',
            isActive: true,
            userCount: 1,
            createdAt: '2025-09-01T00:00:00.000Z'
          }
        ];
        setRoles(mockRoles);
      } finally {
        setLoading(false);
      }
    };

    if (user) {
      fetchRoles();
    }
  }, [user]);

  // 検索フィルタリング
  const filteredRoles = Array.isArray(roles) ? roles.filter(role => 
    role.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    role.displayName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    role.description?.toLowerCase().includes(searchTerm.toLowerCase())
  ) : [];

  // ロールの色分け
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
          <p className="mt-2 text-gray-600">ロール一覧を読み込み中...</p>
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
                  <Shield className="h-8 w-8" />
                  <span>ロール管理</span>
                </h1>
                <p className="mt-1 text-sm text-gray-600">システムロールの管理を行います</p>
              </div>
              <Button 
                onClick={() => setShowCreateForm(true)}
                className="flex items-center space-x-2"
              >
                <Plus className="h-4 w-4" />
                <span>新規ロール作成</span>
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
                <span>ロール検索</span>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Input
                placeholder="ロール名、表示名、説明で検索..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="max-w-md"
              />
            </CardContent>
          </Card>

          {/* ロール一覧 */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {filteredRoles.map((role) => (
              <Card key={role.id} className="hover:shadow-md transition-shadow">
                <CardHeader className="pb-3">
                  <div className="flex items-center justify-between">
                    <CardTitle className="text-lg flex items-center space-x-2">
                      <Shield className="h-5 w-5" />
                      <span>{role.displayName}</span>
                    </CardTitle>
                    <div className="flex items-center space-x-2">
                      {role.isActive ? (
                        <CheckCircle className="h-4 w-4 text-green-500" />
                      ) : (
                        <XCircle className="h-4 w-4 text-red-500" />
                      )}
                    </div>
                  </div>
                  <CardDescription>{role.name}</CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    {/* 説明 */}
                    <p className="text-sm text-gray-600">
                      {role.description}
                    </p>

                    {/* ユーザー数 */}
                    <div className="flex items-center space-x-2 text-sm text-gray-600">
                      <Users className="h-4 w-4" />
                      <span>{role.userCount} 名のユーザー</span>
                    </div>

                    {/* ステータス */}
                    <div className="flex items-center space-x-2">
                      <Badge variant={getRoleBadgeVariant(role.id)}>
                        {role.isActive ? 'アクティブ' : '非アクティブ'}
                      </Badge>
                    </div>

                    {/* 警告（ユーザーがいる場合の削除制限） */}
                    {role.userCount > 0 && (
                      <div className="flex items-center space-x-2 text-sm text-amber-600 bg-amber-50 p-2 rounded">
                        <AlertTriangle className="h-4 w-4" />
                        <span>ユーザーが割り当てられているため削除できません</span>
                      </div>
                    )}

                    {/* アクションボタン */}
                    <div className="flex space-x-2 pt-3">
                      <Button variant="outline" size="sm" className="flex-1">
                        <Edit className="h-4 w-4 mr-1" />
                        編集
                      </Button>
                      <Button 
                        variant="outline" 
                        size="sm" 
                        className="flex-1"
                        disabled={role.userCount > 0}
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
          {filteredRoles.length === 0 && (
            <Card>
              <CardContent className="text-center py-12">
                <Shield className="h-12 w-12 text-gray-400 mx-auto mb-4" />
                <h3 className="text-lg font-medium text-gray-900 mb-2">
                  ロールが見つかりません
                </h3>
                <p className="text-gray-600">
                  検索条件に一致するロールがありません。
                </p>
              </CardContent>
            </Card>
          )}
        </div>
      </div>
    </RoleBasedAccess>
  );
}
