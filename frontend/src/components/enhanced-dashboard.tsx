import * as React from "react";
import { useState, useEffect } from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Label } from "@/components/ui/label";
import { BarChart3, PieChart, TrendingUp, Users, AlertCircle, CheckCircle, Clock, Play, FileText, Shield } from "lucide-react";

import type { 
  StatisticsSummaryDto, 
  WorkflowStatisticsDto,
  WorkflowMode,
  WorkflowStatus
} from "@/lib/types";

interface EnhancedDashboardProps {
  legacyStats?: StatisticsSummaryDto;
  workflowStats?: WorkflowStatisticsDto;
  onModeChange?: (mode: WorkflowMode) => void;
  onRefresh?: () => void;
  loading?: boolean;
}

export function EnhancedDashboard({ 
  legacyStats, 
  workflowStats, 
  onModeChange,
  onRefresh,
  loading = false 
}: EnhancedDashboardProps) {
  const [workflowMode, setWorkflowMode] = useState<WorkflowMode>('legacy');

  const handleModeChange = (mode: WorkflowMode) => {
    setWorkflowMode(mode);
    onModeChange?.(mode);
  };

  const getWorkflowStatusIcon = (status: WorkflowStatus) => {
    switch (status) {
      case 'Unclassified': return <AlertCircle className="w-5 h-5" />;
      case 'Pending': return <Clock className="w-5 h-5" />;
      case 'InProgress': return <Play className="w-5 h-5" />;
      case 'Completed': return <CheckCircle className="w-5 h-5" />;
      case 'PreventionProposed': return <FileText className="w-5 h-5" />;
      case 'EffectivenessConfirmed': return <Shield className="w-5 h-5" />;
    }
  };

  const getWorkflowStatusColor = (status: WorkflowStatus) => {
    switch (status) {
      case 'Unclassified': return 'text-gray-500 bg-gray-50';
      case 'Pending': return 'text-red-500 bg-red-50';
      case 'InProgress': return 'text-orange-500 bg-orange-50';
      case 'Completed': return 'text-blue-500 bg-blue-50';
      case 'PreventionProposed': return 'text-purple-500 bg-purple-50';
      case 'EffectivenessConfirmed': return 'text-green-500 bg-green-50';
    }
  };

  const getWorkflowStatusLabel = (status: WorkflowStatus) => {
    switch (status) {
      case 'Unclassified': return '未分類';
      case 'Pending': return '未対応';
      case 'InProgress': return '対応中';
      case 'Completed': return '対応済';
      case 'PreventionProposed': return '再発防止策提案済';
      case 'EffectivenessConfirmed': return '有効性確認済';
    }
  };

  const renderLegacyStatCards = () => {
    if (!legacyStats) return null;

    return (
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">総インシデント</CardTitle>
            <BarChart3 className="h-4 w-4 text-blue-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-blue-600">{legacyStats.totalIncidents}</div>
            <p className="text-xs text-gray-600">全期間</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">未解決</CardTitle>
            <AlertCircle className="h-4 w-4 text-red-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-red-600">{legacyStats.openCount}</div>
            <p className="text-xs text-gray-600">対応が必要</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">対応中</CardTitle>
            <Play className="h-4 w-4 text-orange-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-orange-600">{legacyStats.inProgressCount}</div>
            <p className="text-xs text-gray-600">進行中</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">解決済み</CardTitle>
            <CheckCircle className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">{legacyStats.resolvedCount}</div>
            <p className="text-xs text-gray-600">完了</p>
          </CardContent>
        </Card>
      </div>
    );
  };

  const renderWorkflowStatCards = () => {
    if (!workflowStats) return null;

    const statItems = [
      { status: 'Unclassified' as WorkflowStatus, count: workflowStats.unclassifiedCount },
      { status: 'Pending' as WorkflowStatus, count: workflowStats.pendingCount },
      { status: 'InProgress' as WorkflowStatus, count: workflowStats.inProgressCount },
      { status: 'Completed' as WorkflowStatus, count: workflowStats.completedCount },
      { status: 'PreventionProposed' as WorkflowStatus, count: workflowStats.preventionProposedCount },
      { status: 'EffectivenessConfirmed' as WorkflowStatus, count: workflowStats.effectivenessConfirmedCount }
    ];

    return (
      <div className="grid grid-cols-1 md:grid-cols-6 gap-4">
        {statItems.map(({ status, count }) => (
          <Card key={status}>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">{getWorkflowStatusLabel(status)}</CardTitle>
              <div className={getWorkflowStatusColor(status)}>
                {getWorkflowStatusIcon(status)}
              </div>
            </CardHeader>
            <CardContent>
              <div className={`text-2xl font-bold ${getWorkflowStatusColor(status).split(' ')[0]}`}>
                {count}
              </div>
              <p className="text-xs text-gray-600">件</p>
            </CardContent>
          </Card>
        ))}
      </div>
    );
  };

  const renderModeInfo = () => {
    if (workflowMode === 'legacy') {
      return (
        <Card className="bg-blue-50 border-blue-200">
          <CardContent className="pt-6">
            <div className="flex items-center gap-2">
              <Badge variant="outline" className="bg-blue-100 text-blue-800">
                レガシーモード
              </Badge>
              <span className="text-sm text-blue-700">
                従来の5段階ステータス管理を表示しています
              </span>
            </div>
          </CardContent>
        </Card>
      );
    } else {
      return (
        <Card className="bg-green-50 border-green-200">
          <CardContent className="pt-6">
            <div className="space-y-2">
              <div className="flex items-center gap-2">
                <Badge variant="outline" className="bg-green-100 text-green-800">
                  新ワークフローモード
                </Badge>
                <span className="text-sm text-green-700">
                  6段階ワークフロー管理を表示しています
                </span>
              </div>
              {workflowStats && (
                <div className="flex items-center gap-4 text-sm text-green-700">
                  <span>ワークフロー有効: {workflowStats.totalWithWorkflow}件</span>
                  <span>レガシーモード: {workflowStats.totalLegacyMode}件</span>
                </div>
              )}
            </div>
          </CardContent>
        </Card>
      );
    }
  };

  return (
    <div className="space-y-6">
      {/* ヘッダー */}
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold">ダッシュボード</h1>
        <div className="flex items-center space-x-4">
          <div className="flex items-center space-x-2">
            <Label>表示モード:</Label>
            <Select value={workflowMode} onValueChange={handleModeChange}>
              <SelectTrigger className="w-40">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="legacy">従来表示</SelectItem>
                <SelectItem value="new">ワークフロー表示</SelectItem>
              </SelectContent>
            </Select>
          </div>
          {onRefresh && (
            <Button onClick={onRefresh} variant="outline" size="sm">
              更新
            </Button>
          )}
        </div>
      </div>

      {/* モード情報 */}
      {renderModeInfo()}

      {/* 統計カード */}
      {loading ? (
        <div className="flex justify-center p-8">
          <div className="text-gray-500">統計データを読み込み中...</div>
        </div>
      ) : (
        <>
          {workflowMode === 'legacy' && renderLegacyStatCards()}
          {workflowMode === 'new' && renderWorkflowStatCards()}
        </>
      )}

      {/* ワークフローの進捗情報 */}
      {workflowMode === 'new' && workflowStats && (
        <Card>
          <CardHeader>
            <CardTitle>ワークフロー進捗</CardTitle>
            <CardDescription>
              新ワークフロー機能の利用状況
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div className="flex justify-between items-center">
                <span className="text-sm font-medium">ワークフロー有効化率</span>
                <span className="text-sm text-gray-600">
                  {workflowStats.totalWithWorkflow + workflowStats.totalLegacyMode > 0 
                    ? Math.round((workflowStats.totalWithWorkflow / (workflowStats.totalWithWorkflow + workflowStats.totalLegacyMode)) * 100)
                    : 0}%
                </span>
              </div>
              
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div 
                  className="bg-green-500 h-2 rounded-full transition-all duration-300" 
                  style={{ 
                    width: `${workflowStats.totalWithWorkflow + workflowStats.totalLegacyMode > 0 
                      ? (workflowStats.totalWithWorkflow / (workflowStats.totalWithWorkflow + workflowStats.totalLegacyMode)) * 100
                      : 0}%` 
                  }}
                />
              </div>
              
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div className="flex justify-between">
                  <span>新ワークフロー:</span>
                  <span className="font-medium">{workflowStats.totalWithWorkflow}件</span>
                </div>
                <div className="flex justify-between">
                  <span>レガシーモード:</span>
                  <span className="font-medium">{workflowStats.totalLegacyMode}件</span>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
