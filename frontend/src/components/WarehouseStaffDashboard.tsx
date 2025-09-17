'use client';

import React, { useState, useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { apiClient } from '@/lib/api-client';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { 
  AlertTriangle, 
  Clock, 
  Warehouse, 
  CheckCircle2,
  TrendingUp,
  FileText,
  Filter,
  RefreshCw
} from 'lucide-react';
import type { Incident } from '@/lib/types';
import { IncidentList } from '@/components/incident-list';

// 倉庫担当ダッシュボード用の型定義
interface WarehouseStaffDashboardData {
  warehouseName: string;
  totalIncidents: number;
  unresolvedIncidents: number;
  inProgressIncidents: number;
  assignedWarehouseIncidents: number;
  classifiedIncidents: number;
}

interface WarehouseStaffIncident {
  id: number;
  title: string;
  description: string;
  status: string;
  priority: string;
  category: string;
  reportedDate: string;
  occurrenceDate: string;
  warehouseName: string;
  troubleTypeName: string;
  damageTypeName: string;
  shippingCompanyName: string;
}

// 倉庫担当専用ダッシュボードコンポーネント
export function WarehouseStaffDashboard() {
  const { user, getAccessToken } = useAuth();
  const [dashboardData, setDashboardData] = useState<WarehouseStaffDashboardData | null>(null);
  const [incidents, setIncidents] = useState<WarehouseStaffIncident[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState('unresolved');
  const [sortConfig, setSortConfig] = useState<{
    key: keyof Incident;
    direction: 'ascending' | 'descending';
  } | null>({
    key: 'occurrenceDate',
    direction: 'descending',
  });

  // ダッシュボードデータの取得
  const fetchDashboardData = async () => {
    try {
      setLoading(true);
      setError(null);

      const data = await apiClient.get<WarehouseStaffDashboardData>('/api/WarehouseStaff/dashboard');
      setDashboardData(data);
    } catch (err) {
      console.error('ダッシュボードデータの取得エラー:', err);
      setError('ダッシュボードデータの取得に失敗しました。');
    } finally {
      setLoading(false);
    }
  };

  // インシデント一覧の取得
  const fetchIncidents = async (type: string) => {
    try {
      setLoading(true);
      setError(null);

      let endpoint = '';
      switch (type) {
        case 'unresolved':
          endpoint = 'api/WarehouseStaff/incidents/unresolved';
          break;
        case 'in-progress':
          endpoint = 'api/WarehouseStaff/incidents/in-progress';
          break;
        case 'assigned-warehouse':
          endpoint = 'api/WarehouseStaff/incidents/assigned-warehouse';
          break;
        case 'classified':
          endpoint = 'api/WarehouseStaff/incidents/classified';
          break;
        default:
          endpoint = 'api/WarehouseStaff/incidents/assigned-warehouse';
      }

      const response = await fetch(`http://localhost:5169/${endpoint}`, {
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
      setIncidents(data.items || []);
    } catch (err) {
      console.error('インシデント一覧の取得エラー:', err);
      setError('インシデント一覧の取得に失敗しました。');
    } finally {
      setLoading(false);
    }
  };

  // 初期データ読み込み
  useEffect(() => {
    fetchDashboardData();
  }, []);

  // タブ変更時のインシデント取得
  useEffect(() => {
    if (dashboardData) {
      fetchIncidents(activeTab);
    }
  }, [activeTab, dashboardData]);

  // リフレッシュ処理
  const handleRefresh = () => {
    fetchDashboardData();
    fetchIncidents(activeTab);
  };

  // ソート処理
  const handleSort = (key: keyof Incident) => {
    const direction = sortConfig?.key === key && sortConfig.direction === 'ascending' ? 'descending' : 'ascending';
    setSortConfig({ key, direction });
  };

  // インシデント編集ハンドラー
  const handleIncidentEdit = (incident: any) => {
    console.log('インシデント編集:', incident);
    // ここで編集モーダルを開くなどの処理を実装
  };

  // インシデント削除ハンドラー
  const handleIncidentDelete = (incident: any) => {
    console.log('インシデント削除:', incident);
    // ここで削除確認ダイアログを表示するなどの処理を実装
  };

  // WarehouseStaffIncidentをIncident型に変換する関数
  const convertToIncident = (warehouseIncident: WarehouseStaffIncident): any => {
    return {
      id: warehouseIncident.id,
      title: warehouseIncident.title,
      description: warehouseIncident.description,
      category: warehouseIncident.category,
      reportedById: 0, // デフォルト値
      reportedByName: 'Unknown', // デフォルト値
      assignedToId: undefined,
      assignedToName: undefined,
      troubleTypeId: 0, // デフォルト値
      damageTypeId: 0, // デフォルト値
      warehouseId: 0, // デフォルト値
      shippingCompanyId: 0, // デフォルト値
      effectivenessStatus: 'NotImplemented' as const,
      effectivenessDate: null,
      effectivenessComment: '',
      priority: warehouseIncident.priority as 'Low' | 'Medium' | 'High' | 'Critical',
      status: warehouseIncident.status as 'Open' | 'InProgress' | 'Resolved' | 'Closed',
      occurrenceDate: warehouseIncident.occurrenceDate,
      incidentDetails: warehouseIncident.description,
      // 表示用の追加プロパティ
      troubleTypeName: warehouseIncident.troubleTypeName,
      damageTypeName: warehouseIncident.damageTypeName,
      warehouseName: warehouseIncident.warehouseName,
      shippingCompanyName: warehouseIncident.shippingCompanyName,
      troubleTypeColor: '#6b7280', // デフォルト色
    };
  };

  if (loading && !dashboardData) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-2 text-gray-600">データを読み込み中...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-center">
          <AlertTriangle className="h-12 w-12 text-red-500 mx-auto mb-4" />
          <p className="text-red-600 mb-4">{error}</p>
          <Button onClick={handleRefresh} variant="outline">
            <RefreshCw className="h-4 w-4 mr-2" />
            再試行
          </Button>
        </div>
      </div>
    );
  }

  if (!dashboardData) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-center">
          <p className="text-gray-600">ダッシュボードデータが見つかりません。</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* ヘッダー情報 */}
      <div className="bg-white rounded-lg shadow p-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold text-gray-900 mb-2">インシデント管理</h1>
            <h2 className="text-xl font-bold text-gray-900">物流トラブル一覧</h2>
            <p className="text-gray-600 mt-1">
              担当倉庫: <span className="font-semibold text-blue-600">{dashboardData.warehouseName}</span>
            </p>
          </div>
          <Button onClick={handleRefresh} variant="outline" size="sm">
            <RefreshCw className="h-4 w-4 mr-2" />
            更新
          </Button>
        </div>
      </div>

      {/* 統計カード */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard
          title="未解決"
          value={dashboardData.unresolvedIncidents}
          icon={AlertTriangle}
          color="text-red-600"
          bgColor="bg-red-50"
          description="対応が必要なインシデント"
        />
        <StatCard
          title="対応中"
          value={dashboardData.inProgressIncidents}
          icon={Clock}
          color="text-yellow-600"
          bgColor="bg-yellow-50"
          description="現在対応中のインシデント"
        />
        <StatCard
          title="担当倉庫"
          value={dashboardData.assignedWarehouseIncidents}
          icon={Warehouse}
          color="text-blue-600"
          bgColor="bg-blue-50"
          description="担当倉庫の全インシデント"
        />
        <StatCard
          title="分類済み"
          value={dashboardData.classifiedIncidents}
          icon={CheckCircle2}
          color="text-green-600"
          bgColor="bg-green-50"
          description="分類が完了したインシデント"
        />
      </div>

      {/* 検索・CSV出力 */}
      <div className="bg-white p-6 rounded-lg shadow-sm">
        <div className="flex gap-4 items-center">
          <div className="flex-1 relative">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <svg className="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </div>
            <input
              type="text"
              placeholder="物流トラブルを検索..."
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <Button 
            onClick={() => console.log('CSV出力機能')} 
            className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700"
          >
            <svg className="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
            CSV出力
          </Button>
        </div>
      </div>

      {/* インシデント一覧タブ */}
      <div className="bg-white rounded-lg shadow-sm overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-200">
          <div className="flex items-center space-x-2">
            <FileText className="h-5 w-5" />
            <span className="text-lg font-semibold">インシデント一覧</span>
          </div>
          <p className="text-sm text-gray-600 mt-1">担当倉庫のインシデントを管理します</p>
        </div>
        
        <Tabs value={activeTab} onValueChange={setActiveTab}>
          <div className="px-6 py-3 border-b border-gray-200">
            <TabsList className="grid w-full grid-cols-4">
              <TabsTrigger value="unresolved">
                未解決 ({dashboardData.unresolvedIncidents})
              </TabsTrigger>
              <TabsTrigger value="in-progress">
                対応中 ({dashboardData.inProgressIncidents})
              </TabsTrigger>
              <TabsTrigger value="assigned-warehouse">
                担当倉庫 ({dashboardData.assignedWarehouseIncidents})
              </TabsTrigger>
              <TabsTrigger value="classified">
                分類済み ({dashboardData.classifiedIncidents})
              </TabsTrigger>
            </TabsList>
          </div>

          <TabsContent value="unresolved" className="mt-0">
            <IncidentList 
              incidents={incidents.map(convertToIncident)} 
              loading={loading}
              requestSort={handleSort}
              sortConfig={sortConfig}
              onEdit={handleIncidentEdit}
              onDelete={handleIncidentDelete}
            />
          </TabsContent>
          <TabsContent value="in-progress" className="mt-0">
            <IncidentList 
              incidents={incidents.map(convertToIncident)} 
              loading={loading}
              requestSort={handleSort}
              sortConfig={sortConfig}
              onEdit={handleIncidentEdit}
              onDelete={handleIncidentDelete}
            />
          </TabsContent>
          <TabsContent value="assigned-warehouse" className="mt-0">
            <IncidentList 
              incidents={incidents.map(convertToIncident)} 
              loading={loading}
              requestSort={handleSort}
              sortConfig={sortConfig}
              onEdit={handleIncidentEdit}
              onDelete={handleIncidentDelete}
            />
          </TabsContent>
          <TabsContent value="classified" className="mt-0">
            <IncidentList 
              incidents={incidents.map(convertToIncident)} 
              loading={loading}
              requestSort={handleSort}
              sortConfig={sortConfig}
              onEdit={handleIncidentEdit}
              onDelete={handleIncidentDelete}
            />
          </TabsContent>
        </Tabs>
      </div>
    </div>
  );
}

// 統計カードコンポーネント
interface StatCardProps {
  title: string;
  value: number;
  icon: React.ComponentType<{ className?: string }>;
  color: string;
  bgColor: string;
  description: string;
}

function StatCard({ title, value, icon: Icon, color, bgColor, description }: StatCardProps) {
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
