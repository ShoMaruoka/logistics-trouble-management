'use client';

import React from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { useDashboardConfig, useRolePermissions } from './RoleBasedAccess';
import { Dashboard } from './dashboard';
import { IncidentManagement } from './incident-management';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { IncidentForm } from '@/components/incident-form';
import type { Incident, CreateIncidentDto, UpdateIncidentDto } from '@/lib/types';
import { 
  User, 
  Shield, 
  BarChart3, 
  FileText, 
  Settings, 
  Users,
  AlertTriangle,
  CheckCircle,
  Clock,
  TrendingUp
} from 'lucide-react';

// ロール別ダッシュボードコンポーネント
export function RoleBasedDashboard() {
  const { user } = useAuth();
  const dashboardConfig = useDashboardConfig();
  const permissions = useRolePermissions();
  
  // インシデント編集の状態管理
  const [isDialogOpen, setIsDialogOpen] = React.useState(false);
  const [editingIncident, setEditingIncident] = React.useState<any>(null);

  // インシデント編集のハンドラー
  const handleEditIncident = (incident: Incident) => {
    setEditingIncident(incident);
    setIsDialogOpen(true);
  };

  const handleFormSubmit = async (data: CreateIncidentDto | UpdateIncidentDto) => {
    // TODO: インシデント更新のAPI呼び出し
    console.log('インシデントデータ:', data);
    setIsDialogOpen(false);
    setEditingIncident(null);
  };

  if (!user) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-2 text-gray-600">認証状態を確認中...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* ヘッダーセクション */}
      <div className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-2xl font-bold text-gray-900">{dashboardConfig.title}</h1>
              <p className="mt-1 text-sm text-gray-600">{dashboardConfig.subtitle}</p>
            </div>
            <div className="flex items-center space-x-4">
              <Badge variant="outline" className="flex items-center space-x-1">
                <User className="h-4 w-4" />
                <span>{user.username}</span>
              </Badge>
              <Badge variant="secondary" className="flex items-center space-x-1">
                <Shield className="h-4 w-4" />
                <span>{permissions.roleName}</span>
              </Badge>
            </div>
          </div>
        </div>
      </div>

      {/* メインコンテンツ */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* 主要アクションカード */}
        {dashboardConfig.primaryActions.length > 0 && (
          <div className="mb-8">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">主要アクション</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {dashboardConfig.primaryActions.map((action, index) => (
                <Card key={index} className="hover:shadow-md transition-shadow cursor-pointer">
                  <CardHeader className="pb-3">
                    <CardTitle className="text-sm font-medium text-gray-900">
                      {action.label}
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <Button 
                      variant="outline" 
                      size="sm" 
                      className="w-full"
                      onClick={() => handlePrimaryAction(action.action)}
                    >
                      実行
                    </Button>
                  </CardContent>
                </Card>
              ))}
            </div>
          </div>
        )}

        {/* ロール別統計カード */}
        <div className="mb-8">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">概要</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <RoleBasedStatCard
              title="未解決インシデント"
              value="8"
              icon={AlertTriangle}
              color="text-red-600"
              bgColor="bg-red-50"
              description="対応が必要なインシデント"
            />
            <RoleBasedStatCard
              title="対応中インシデント"
              value="3"
              icon={Clock}
              color="text-yellow-600"
              bgColor="bg-yellow-50"
              description="現在対応中のインシデント"
            />
            <RoleBasedStatCard
              title="解決済みインシデント"
              value="2"
              icon={CheckCircle}
              color="text-green-600"
              bgColor="bg-green-50"
              description="今月解決されたインシデント"
            />
            <RoleBasedStatCard
              title="解決率"
              value="20%"
              icon={TrendingUp}
              color="text-blue-600"
              bgColor="bg-blue-50"
              description="今月の解決率"
            />
          </div>
        </div>

        {/* 統計セクション */}
        {dashboardConfig.showStatistics && (
          <div className="mb-8">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">統計・分析</h2>
            <Dashboard />
          </div>
        )}

        {/* インシデント管理セクション */}
        {dashboardConfig.showIncidentManagement && (
          <div className="mb-8">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">インシデント管理</h2>
            <IncidentManagement 
              onEdit={handleEditIncident}
              onDelete={(incident) => {
                console.log('Delete incident:', incident);
                // 削除確認ダイアログを表示
                if (confirm(`インシデント「${incident.title}」を削除しますか？`)) {
                  alert('削除機能は開発中です。');
                }
              }}
            />
          </div>
        )}

        {/* 管理者専用セクション */}
        {permissions.canManageMasters && (
          <div className="mb-8">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">マスタ管理</h2>
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center space-x-2">
                  <Settings className="h-5 w-5" />
                  <span>システム管理</span>
                </CardTitle>
                <CardDescription>
                  マスタデータとユーザーの管理を行います
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <Button variant="outline" className="flex items-center space-x-2">
                    <Settings className="h-4 w-4" />
                    <span>マスタ管理</span>
                  </Button>
                  {permissions.canManageUsers && (
                    <Button variant="outline" className="flex items-center space-x-2">
                      <Users className="h-4 w-4" />
                      <span>ユーザー管理</span>
                    </Button>
                  )}
                </div>
              </CardContent>
            </Card>
          </div>
        )}

        {/* インシデント登録・編集ダイアログ */}
        <Dialog open={isDialogOpen} onOpenChange={(open) => {
          setIsDialogOpen(open);
          if (!open) {
            setEditingIncident(null);
          }
        }}>
          <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle>
                {editingIncident ? '物流トラブル編集' : '物流トラブル登録'}
              </DialogTitle>
              <DialogDescription>
                {editingIncident ? '物流トラブルの情報を編集してください。' : '新しい物流トラブルを登録してください。'}
              </DialogDescription>
            </DialogHeader>
            <IncidentForm
              incident={editingIncident}
              onSubmit={handleFormSubmit}
            />
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}

// ロール別統計カードコンポーネント
interface RoleBasedStatCardProps {
  title: string;
  value: string;
  icon: React.ComponentType<{ className?: string }>;
  color: string;
  bgColor: string;
  description: string;
}

function RoleBasedStatCard({ 
  title, 
  value, 
  icon: Icon, 
  color, 
  bgColor, 
  description 
}: RoleBasedStatCardProps) {
  return (
    <Card>
      <CardContent className="p-6">
        <div className="flex items-center">
          <div className={`p-3 rounded-lg ${bgColor}`}>
            <Icon className={`h-6 w-6 ${color}`} />
          </div>
          <div className="ml-4">
            <p className="text-sm font-medium text-gray-600">{title}</p>
            <p className="text-2xl font-bold text-gray-900">{value}</p>
            <p className="text-xs text-gray-500">{description}</p>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}

// 主要アクションのハンドラー
function handlePrimaryAction(action: string) {
  switch (action) {
    case 'create-incident':
      // 新規インシデント作成ダイアログを開く
      console.log('新規インシデント作成');
      break;
    case 'view-assigned':
      // 担当インシデント一覧を表示
      console.log('担当インシデント確認');
      break;
    case 'classify-incidents':
      // インシデント分類画面に移動
      console.log('インシデント分類');
      break;
    case 'set-priority':
      // 優先度設定画面に移動
      console.log('優先度設定');
      break;
    case 'assign-responsible':
      // 担当者割り当て画面に移動
      console.log('担当者割り当て');
      break;
    case 'register-solution':
      // 解決策登録画面に移動
      console.log('解決策登録');
      break;
    case 'measure-effectiveness':
      // 効果測定画面に移動
      console.log('効果測定実施');
      break;
    case 'prevent-recurrence':
      // 再発防止策提案画面に移動
      console.log('再発防止策提案');
      break;
    case 'manage-users':
      // ユーザー管理画面に移動
      console.log('ユーザー管理');
      break;
    case 'manage-masters':
      // マスタ管理画面に移動
      console.log('マスタ管理');
      break;
    case 'system-settings':
      // システム設定画面に移動
      console.log('システム設定');
      break;
    default:
      console.log('不明なアクション:', action);
  }
}
