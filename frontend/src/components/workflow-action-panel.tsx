import * as React from "react";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Badge } from "@/components/ui/badge";
import { AlertCircle, CheckCircle, Clock, Play, FileText, Shield } from "lucide-react";
import { useMasterData } from "@/hooks/useMasterData";

import type { 
  IncidentWithWorkflow, 
  WorkflowStatus,
  Priority,
  ClassifyIncidentDto,
  StartResponseDto,
  AnalyzeCauseDto,
  CompleteResponseDto,
  ProposePreventionDto,
  ConfirmEffectivenessDto
} from "@/lib/types";

interface WorkflowActionPanelProps {
  incident: IncidentWithWorkflow;
  userRole?: string;
  onWorkflowAction?: (action: string, data: any) => void;
  loading?: boolean;
}

export function WorkflowActionPanel({ 
  incident, 
  userRole, 
  onWorkflowAction,
  loading = false 
}: WorkflowActionPanelProps) {
  const { troubleTypes, damageTypes, warehouses, shippingCompanies } = useMasterData();
  const [actionLoading, setActionLoading] = useState<string | null>(null);

  // 分類フォーム用の状態
  const [classificationData, setClassificationData] = useState<Partial<ClassifyIncidentDto>>({
    troubleTypeId: incident.troubleTypeId || 0,
    damageTypeId: incident.damageTypeId || 0,
    warehouseId: incident.warehouseId || 0,
    shippingCompanyId: incident.shippingCompanyId || 0,
    totalShipments: incident.totalShipments || 0,
    defectiveItems: incident.defectiveItems || 0,
    priority: incident.priority || 'Medium',
    dueDate: incident.dueDate || new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0] // 1週間後
  });

  // 原因分析フォーム用の状態
  const [causeData, setCauseData] = useState<string>(incident.cause || '');

  // 対応完了フォーム用の状態
  const [responseData, setResponseData] = useState<string>(incident.responseContent || '');

  // 再発防止策フォーム用の状態
  const [preventionData, setPreventionData] = useState<string>(incident.preventionMeasures || '');

  // 有効性確認フォーム用の状態
  const [effectivenessData, setEffectivenessData] = useState({
    status: 'Implemented',
    comment: incident.effectivenessComment || ''
  });

  const handleAction = async (action: string, data: any) => {
    if (!onWorkflowAction) return;
    
    setActionLoading(action);
    try {
      await onWorkflowAction(action, data);
    } finally {
      setActionLoading(null);
    }
  };

  const getStatusIcon = (status?: WorkflowStatus) => {
    switch (status) {
      case 'Unclassified': return <AlertCircle className="w-4 h-4 text-gray-500" />;
      case 'Pending': return <Clock className="w-4 h-4 text-red-500" />;
      case 'InProgress': return <Play className="w-4 h-4 text-orange-500" />;
      case 'Completed': return <CheckCircle className="w-4 h-4 text-blue-500" />;
      case 'PreventionProposed': return <FileText className="w-4 h-4 text-purple-500" />;
      case 'EffectivenessConfirmed': return <Shield className="w-4 h-4 text-green-500" />;
      default: return <AlertCircle className="w-4 h-4 text-gray-500" />;
    }
  };

  const renderClassificationForm = () => {
    if (incident.workflowStatus !== 'Unclassified' || userRole !== 'Incident Manager') {
      return null;
    }

    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            {getStatusIcon('Unclassified')}
            インシデント分類
          </CardTitle>
          <CardDescription>
            インシデントの詳細情報を設定して分類を完了してください。
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <Label>トラブル種類 *</Label>
              <Select
                value={classificationData.troubleTypeId?.toString()}
                onValueChange={(value) => setClassificationData(prev => ({ ...prev, troubleTypeId: parseInt(value) }))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="トラブル種類を選択" />
                </SelectTrigger>
                <SelectContent>
                  {troubleTypes?.map((type) => (
                    <SelectItem key={type.id} value={type.id.toString()}>
                      <div className="flex items-center gap-2">
                        <div 
                          className="w-3 h-3 rounded-full" 
                          style={{ backgroundColor: type.color }}
                        />
                        {type.name}
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label>損傷種類 *</Label>
              <Select
                value={classificationData.damageTypeId?.toString()}
                onValueChange={(value) => setClassificationData(prev => ({ ...prev, damageTypeId: parseInt(value) }))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="損傷種類を選択" />
                </SelectTrigger>
                <SelectContent>
                  {damageTypes?.map((type) => (
                    <SelectItem key={type.id} value={type.id.toString()}>
                      {type.name} ({type.category})
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label>出荷元倉庫 *</Label>
              <Select
                value={classificationData.warehouseId?.toString()}
                onValueChange={(value) => setClassificationData(prev => ({ ...prev, warehouseId: parseInt(value) }))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="出荷元倉庫を選択" />
                </SelectTrigger>
                <SelectContent>
                  {warehouses?.map((warehouse) => (
                    <SelectItem key={warehouse.id} value={warehouse.id.toString()}>
                      {warehouse.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label>運送会社 *</Label>
              <Select
                value={classificationData.shippingCompanyId?.toString()}
                onValueChange={(value) => setClassificationData(prev => ({ ...prev, shippingCompanyId: parseInt(value) }))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="運送会社を選択" />
                </SelectTrigger>
                <SelectContent>
                  {shippingCompanies?.map((company) => (
                    <SelectItem key={company.id} value={company.id.toString()}>
                      {company.name} ({company.companyType})
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label>出荷総数</Label>
              <Input
                type="number"
                min="0"
                value={classificationData.totalShipments}
                onChange={(e) => setClassificationData(prev => ({ ...prev, totalShipments: parseInt(e.target.value) || 0 }))}
              />
            </div>

            <div>
              <Label>不良品数</Label>
              <Input
                type="number"
                min="0"
                max={classificationData.totalShipments}
                value={classificationData.defectiveItems}
                onChange={(e) => setClassificationData(prev => ({ ...prev, defectiveItems: parseInt(e.target.value) || 0 }))}
              />
            </div>

            <div>
              <Label>優先度</Label>
              <Select
                value={classificationData.priority}
                onValueChange={(value) => setClassificationData(prev => ({ ...prev, priority: value as Priority }))}
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Low">低</SelectItem>
                  <SelectItem value="Medium">中</SelectItem>
                  <SelectItem value="High">高</SelectItem>
                  <SelectItem value="Critical">緊急</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label>対応期限 *</Label>
              <Input
                type="date"
                value={classificationData.dueDate}
                onChange={(e) => setClassificationData(prev => ({ ...prev, dueDate: e.target.value }))}
              />
            </div>
          </div>

          <Button 
            onClick={() => handleAction('classify', { ...classificationData, incidentId: incident.id })}
            disabled={loading || actionLoading === 'classify'}
            className="w-full"
          >
            {actionLoading === 'classify' ? '分類中...' : 'インシデントを分類'}
          </Button>
        </CardContent>
      </Card>
    );
  };

  const renderStartResponseButton = () => {
    if (incident.workflowStatus !== 'Pending' || userRole !== 'Warehouse Staff') {
      return null;
    }

    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            {getStatusIcon('Pending')}
            対応開始
          </CardTitle>
          <CardDescription>
            このインシデントの対応を開始してください。
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Button 
            onClick={() => handleAction('start-response', { incidentId: incident.id })}
            disabled={loading || actionLoading === 'start-response'}
            className="w-full"
          >
            {actionLoading === 'start-response' ? '開始中...' : '対応を開始'}
          </Button>
        </CardContent>
      </Card>
    );
  };

  const renderCauseAnalysisForm = () => {
    if (incident.workflowStatus !== 'InProgress' || userRole !== 'Warehouse Staff') {
      return null;
    }

    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            {getStatusIcon('InProgress')}
            原因分析
          </CardTitle>
          <CardDescription>
            インシデントの原因を分析して入力してください。
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <Label>原因 *</Label>
            <textarea
              className="w-full min-h-[100px] p-3 border border-gray-300 rounded-md"
              value={causeData}
              onChange={(e) => setCauseData(e.target.value)}
              placeholder="インシデントの原因を詳しく入力してください"
              required
            />
          </div>

          <Button 
            onClick={() => handleAction('analyze-cause', { incidentId: incident.id, cause: causeData })}
            disabled={loading || actionLoading === 'analyze-cause' || !causeData.trim()}
            className="w-full"
          >
            {actionLoading === 'analyze-cause' ? '入力中...' : '原因を入力'}
          </Button>
        </CardContent>
      </Card>
    );
  };

  const renderCompleteResponseForm = () => {
    if (incident.workflowStatus !== 'InProgress' || userRole !== 'Warehouse Staff') {
      return null;
    }

    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            {getStatusIcon('InProgress')}
            対応完了
          </CardTitle>
          <CardDescription>
            実施した対応内容を入力して対応を完了してください。
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <Label>対応内容 *</Label>
            <textarea
              className="w-full min-h-[100px] p-3 border border-gray-300 rounded-md"
              value={responseData}
              onChange={(e) => setResponseData(e.target.value)}
              placeholder="実施した対応内容を詳しく入力してください"
              required
            />
          </div>

          <Button 
            onClick={() => handleAction('complete-response', { incidentId: incident.id, responseContent: responseData })}
            disabled={loading || actionLoading === 'complete-response' || !responseData.trim()}
            className="w-full"
          >
            {actionLoading === 'complete-response' ? '完了中...' : '対応を完了'}
          </Button>
        </CardContent>
      </Card>
    );
  };

  const renderPreventionMeasuresForm = () => {
    if (incident.workflowStatus !== 'Completed' || userRole !== 'Warehouse Staff') {
      return null;
    }

    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            {getStatusIcon('Completed')}
            再発防止策提案
          </CardTitle>
          <CardDescription>
            今後同様のインシデントを防ぐための対策を提案してください。
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <Label>再発防止策 *</Label>
            <textarea
              className="w-full min-h-[100px] p-3 border border-gray-300 rounded-md"
              value={preventionData}
              onChange={(e) => setPreventionData(e.target.value)}
              placeholder="再発防止策を詳しく入力してください"
              required
            />
          </div>

          <Button 
            onClick={() => handleAction('propose-prevention', { incidentId: incident.id, preventionMeasures: preventionData })}
            disabled={loading || actionLoading === 'propose-prevention' || !preventionData.trim()}
            className="w-full"
          >
            {actionLoading === 'propose-prevention' ? '提案中...' : '再発防止策を提案'}
          </Button>
        </CardContent>
      </Card>
    );
  };

  const renderEffectivenessConfirmationForm = () => {
    if (incident.workflowStatus !== 'PreventionProposed' || userRole !== 'Incident Manager') {
      return null;
    }

    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            {getStatusIcon('PreventionProposed')}
            有効性確認
          </CardTitle>
          <CardDescription>
            提案された再発防止策の有効性を確認してください。
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          {incident.preventionMeasures && (
            <div className="p-3 bg-gray-50 rounded-md">
              <Label className="font-medium">提案された再発防止策</Label>
              <p className="mt-1 text-sm">{incident.preventionMeasures}</p>
            </div>
          )}

          <div>
            <Label>有効性評価 *</Label>
            <Select
              value={effectivenessData.status}
              onValueChange={(value) => setEffectivenessData(prev => ({ ...prev, status: value }))}
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="Implemented">実施</SelectItem>
                <SelectItem value="NotImplemented">未実施</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div>
            <Label>確認コメント *</Label>
            <textarea
              className="w-full min-h-[100px] p-3 border border-gray-300 rounded-md"
              value={effectivenessData.comment}
              onChange={(e) => setEffectivenessData(prev => ({ ...prev, comment: e.target.value }))}
              placeholder="有効性確認の詳細コメントを入力してください"
              required
            />
          </div>

          <Button 
            onClick={() => handleAction('confirm-effectiveness', { 
              incidentId: incident.id, 
              effectivenessStatus: effectivenessData.status,
              effectivenessComment: effectivenessData.comment 
            })}
            disabled={loading || actionLoading === 'confirm-effectiveness' || !effectivenessData.comment.trim()}
            className="w-full"
          >
            {actionLoading === 'confirm-effectiveness' ? '確認中...' : '有効性を確認'}
          </Button>
        </CardContent>
      </Card>
    );
  };

  const renderCompletedState = () => {
    if (incident.workflowStatus !== 'EffectivenessConfirmed') {
      return null;
    }

    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            {getStatusIcon('EffectivenessConfirmed')}
            ワークフロー完了
          </CardTitle>
          <CardDescription>
            このインシデントのワークフローは完了しています。
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Badge className="bg-green-500 text-white">
            完了
          </Badge>
          <p className="mt-2 text-sm text-gray-600">
            すべてのワークフロー段階が完了しました。
          </p>
        </CardContent>
      </Card>
    );
  };

  // ワークフローが有効でない場合
  if (!incident.workflowStatus) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>ワークフロー無効</CardTitle>
          <CardDescription>
            このインシデントは従来のステータス管理を使用しています。
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Button 
            onClick={() => handleAction('enable-workflow', { incidentId: incident.id })}
            disabled={loading || actionLoading === 'enable-workflow'}
            variant="outline"
          >
            {actionLoading === 'enable-workflow' ? '有効化中...' : '新ワークフローを有効化'}
          </Button>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="space-y-4">
      {renderClassificationForm()}
      {renderStartResponseButton()}
      {renderCauseAnalysisForm()}
      {renderCompleteResponseForm()}
      {renderPreventionMeasuresForm()}
      {renderEffectivenessConfirmationForm()}
      {renderCompletedState()}
    </div>
  );
}
