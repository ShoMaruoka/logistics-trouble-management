'use client';

import React, { useState, useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Clock, RefreshCw, FileText } from 'lucide-react';

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

export default function InProgressIncidentsPage() {
  const { user, getAccessToken } = useAuth();
  const [incidents, setIncidents] = useState<WarehouseStaffIncident[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // 対応中インシデント一覧の取得
  const fetchInProgressIncidents = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await fetch('http://localhost:5169/api/WarehouseStaff/incidents/in-progress', {
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
      console.error('対応中インシデントの取得エラー:', err);
      setError('対応中インシデントの取得に失敗しました。');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchInProgressIncidents();
  }, []);

  if (loading) {
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
          <Clock className="h-12 w-12 text-red-500 mx-auto mb-4" />
          <p className="text-red-600 mb-4">{error}</p>
          <Button onClick={fetchInProgressIncidents} variant="outline">
            <RefreshCw className="h-4 w-4 mr-2" />
            再試行
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="container mx-auto px-4 py-8">
        {/* ヘッダー */}
        <div className="mb-8">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">対応中インシデント</h1>
              <p className="text-gray-600 mt-1">
                担当倉庫: <span className="font-semibold text-blue-600">{user?.warehouseName || '不明'}</span>
              </p>
            </div>
            <Button onClick={fetchInProgressIncidents} variant="outline" size="sm">
              <RefreshCw className="h-4 w-4 mr-2" />
              更新
            </Button>
          </div>
        </div>

        {/* インシデント一覧 */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center space-x-2">
              <Clock className="h-5 w-5" />
              <span>対応中インシデント一覧</span>
            </CardTitle>
            <CardDescription>
              現在対応中のインシデントを管理します
            </CardDescription>
          </CardHeader>
          <CardContent>
            {incidents.length === 0 ? (
              <div className="text-center py-8">
                <FileText className="h-12 w-12 text-gray-400 mx-auto mb-4" />
                <p className="text-gray-500">対応中のインシデントがありません</p>
              </div>
            ) : (
              <div className="space-y-4">
                {incidents.map((incident) => (
                  <Card key={incident.id} className="hover:shadow-md transition-shadow">
                    <CardContent className="p-4">
                      <div className="flex items-start justify-between">
                        <div className="flex-1">
                          <div className="flex items-center space-x-2 mb-2">
                            <h3 className="font-semibold text-gray-900">{incident.title}</h3>
                            <Badge variant={getStatusVariant(incident.status)}>
                              {incident.status}
                            </Badge>
                            <Badge variant={getPriorityVariant(incident.priority)}>
                              {incident.priority}
                            </Badge>
                          </div>
                          <p className="text-sm text-gray-600 mb-2">{incident.description}</p>
                          <div className="flex items-center space-x-4 text-xs text-gray-500">
                            <span>発生日: {formatDate(incident.occurrenceDate)}</span>
                            <span>報告日: {formatDate(incident.reportedDate)}</span>
                            <span>カテゴリ: {incident.category}</span>
                          </div>
                          <div className="flex items-center space-x-4 text-xs text-gray-500 mt-1">
                            <span>トラブル種類: {incident.troubleTypeName}</span>
                            <span>損傷種類: {incident.damageTypeName}</span>
                            <span>運送会社: {incident.shippingCompanyName}</span>
                          </div>
                        </div>
                        <div className="flex space-x-2">
                          <Button size="sm" variant="outline">
                            詳細
                          </Button>
                          <Button size="sm" variant="outline">
                            更新
                          </Button>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

// ヘルパー関数
function getStatusVariant(status: string): "default" | "secondary" | "destructive" | "outline" {
  switch (status.toLowerCase()) {
    case 'open':
      return 'destructive';
    case 'inprogress':
      return 'secondary';
    case 'resolved':
      return 'default';
    default:
      return 'outline';
  }
}

function getPriorityVariant(priority: string): "default" | "secondary" | "destructive" | "outline" {
  switch (priority.toLowerCase()) {
    case 'high':
      return 'destructive';
    case 'medium':
      return 'secondary';
    case 'low':
      return 'default';
    default:
      return 'outline';
  }
}

function formatDate(dateString: string): string {
  try {
    const date = new Date(dateString);
    return date.toLocaleDateString('ja-JP');
  } catch {
    return dateString;
  }
}
