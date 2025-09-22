import * as React from "react";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { CalendarIcon } from "lucide-react";
import { useMasterData } from "@/hooks/useMasterData";
import { useAuth } from "@/contexts/AuthContext";

import type { 
  IncidentWithWorkflow, 
  CreateIncidentDto, 
  UpdateIncidentDto, 
  Priority,
  EffectivenessStatus,
  WorkflowMode,
  WorkflowStatus
} from "@/lib/types";

interface IncidentFormEnhancedProps {
  incident?: IncidentWithWorkflow | null;
  workflowMode?: WorkflowMode;
  onSubmit: (data: CreateIncidentDto | UpdateIncidentDto) => void;
  onWorkflowAction?: (action: string, data: any) => void;
  onCancel?: () => void;
  loading?: boolean;
}

export function IncidentFormEnhanced({ 
  incident, 
  workflowMode = 'legacy', 
  onSubmit, 
  onWorkflowAction,
  onCancel, 
  loading = false 
}: IncidentFormEnhancedProps) {
  const { user } = useAuth();
  const { 
    troubleTypes, 
    damageTypes, 
    warehouses, 
    shippingCompanies, 
    loading: masterDataLoading, 
    error: masterDataError 
  } = useMasterData();
  
  const [currentWorkflowMode, setCurrentWorkflowMode] = useState<WorkflowMode>(workflowMode);
  const [formData, setFormData] = useState<CreateIncidentDto | UpdateIncidentDto>({
    title: incident?.title || '',
    description: incident?.description || '',
    category: incident?.category || '',
    troubleTypeId: incident?.troubleTypeId || 0,
    damageTypeId: incident?.damageTypeId || 0,
    warehouseId: incident?.warehouseId || 0,
    shippingCompanyId: incident?.shippingCompanyId || 0,
    priority: incident?.priority || 'Medium',
    effectivenessStatus: incident?.effectivenessStatus || 'NotImplemented',
    incidentDetails: incident?.incidentDetails || '',
    totalShipments: incident?.totalShipments || 0,
    defectiveItems: incident?.defectiveItems || 0,
    occurrenceDate: incident?.occurrenceDate ? incident.occurrenceDate.split('T')[0] : '',
    occurrenceLocation: incident?.occurrenceLocation || '',
    summary: incident?.summary || '',
    cause: incident?.cause || '',
    preventionMeasures: incident?.preventionMeasures || '',
    effectivenessComment: incident?.effectivenessComment || '',
    reportedById: incident?.reportedById || user?.id || 1
  });

  const handleInputChange = (field: string, value: any) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
  };

  const getWorkflowStatusBadgeColor = (status?: WorkflowStatus) => {
    switch (status) {
      case 'Unclassified': return 'bg-gray-500';
      case 'Pending': return 'bg-red-500';
      case 'InProgress': return 'bg-orange-500';
      case 'Completed': return 'bg-blue-500';
      case 'PreventionProposed': return 'bg-purple-500';
      case 'EffectivenessConfirmed': return 'bg-green-500';
      default: return 'bg-gray-500';
    }
  };

  const getWorkflowStatusLabel = (status?: WorkflowStatus) => {
    switch (status) {
      case 'Unclassified': return '未分類';
      case 'Pending': return '未対応';
      case 'InProgress': return '対応中';
      case 'Completed': return '対応済';
      case 'PreventionProposed': return '再発防止策提案済';
      case 'EffectivenessConfirmed': return '有効性確認済';
      default: return '不明';
    }
  };

  const renderWorkflowStatus = () => {
    if (currentWorkflowMode !== 'new' || !incident?.workflowStatus) return null;

    return (
      <Card className="mb-6">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            ワークフロー状況
            <Badge className={getWorkflowStatusBadgeColor(incident.workflowStatus)}>
              {getWorkflowStatusLabel(incident.workflowStatus)}
            </Badge>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {incident.dueDate && (
              <div>
                <Label>対応期限</Label>
                <div className="flex items-center gap-2 mt-1">
                  <CalendarIcon className="w-4 h-4 text-gray-500" />
                  <span>{new Date(incident.dueDate).toLocaleDateString('ja-JP')}</span>
                </div>
              </div>
            )}
            
            {incident.responseStartDate && (
              <div>
                <Label>対応開始日</Label>
                <div className="flex items-center gap-2 mt-1">
                  <CalendarIcon className="w-4 h-4 text-gray-500" />
                  <span>{new Date(incident.responseStartDate).toLocaleDateString('ja-JP')}</span>
                </div>
              </div>
            )}
            
            {incident.completionDate && (
              <div>
                <Label>対応完了日</Label>
                <div className="flex items-center gap-2 mt-1">
                  <CalendarIcon className="w-4 h-4 text-gray-500" />
                  <span>{new Date(incident.completionDate).toLocaleDateString('ja-JP')}</span>
                </div>
              </div>
            )}
            
            {incident.effectivenessConfirmationDate && (
              <div>
                <Label>有効性確認日</Label>
                <div className="flex items-center gap-2 mt-1">
                  <CalendarIcon className="w-4 h-4 text-gray-500" />
                  <span>{new Date(incident.effectivenessConfirmationDate).toLocaleDateString('ja-JP')}</span>
                </div>
              </div>
            )}
          </div>
          
          {incident.responseContent && (
            <div className="mt-4">
              <Label>対応内容</Label>
              <div className="mt-1 p-3 bg-gray-50 rounded-md">
                {incident.responseContent}
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    );
  };

  const renderLegacyForm = () => {
    return (
      <div className="space-y-6">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="space-y-4">
            <div>
              <Label htmlFor="title">タイトル *</Label>
              <Input
                id="title"
                value={formData.title}
                onChange={(e) => handleInputChange('title', e.target.value)}
                placeholder="インシデントのタイトルを入力"
                required
              />
            </div>

            <div>
              <Label htmlFor="category">分類 *</Label>
              <Input
                id="category"
                value={formData.category}
                onChange={(e) => handleInputChange('category', e.target.value)}
                placeholder="インシデントの分類を入力"
                required
              />
            </div>

            <div>
              <Label htmlFor="troubleType">トラブル種類 *</Label>
              <Select
                value={formData.troubleTypeId?.toString()}
                onValueChange={(value) => handleInputChange('troubleTypeId', parseInt(value))}
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
              <Label htmlFor="priority">優先度</Label>
              <Select
                value={formData.priority}
                onValueChange={(value) => handleInputChange('priority', value as Priority)}
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
              <Label htmlFor="occurrenceDate">発生日 *</Label>
              <div className="flex items-center gap-2">
                <Input
                  id="occurrenceDate"
                  type="date"
                  value={formData.occurrenceDate}
                  onChange={(e) => handleInputChange('occurrenceDate', e.target.value)}
                  required
                />
                <CalendarIcon className="w-4 h-4 text-gray-500" />
              </div>
              <p className="text-sm text-gray-500 mt-1">カレンダーから日付を選択してください</p>
            </div>
          </div>

          <div className="space-y-4">
            <div>
              <Label htmlFor="damageType">損傷種類 *</Label>
              <Select
                value={formData.damageTypeId?.toString()}
                onValueChange={(value) => handleInputChange('damageTypeId', parseInt(value))}
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
              <Label htmlFor="warehouse">出荷元倉庫 *</Label>
              <Select
                value={formData.warehouseId?.toString()}
                onValueChange={(value) => handleInputChange('warehouseId', parseInt(value))}
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
              <Label htmlFor="shippingCompany">運送会社 *</Label>
              <Select
                value={formData.shippingCompanyId?.toString()}
                onValueChange={(value) => handleInputChange('shippingCompanyId', parseInt(value))}
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

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <Label htmlFor="totalShipments">出荷総数</Label>
                <Input
                  id="totalShipments"
                  type="number"
                  min="0"
                  value={formData.totalShipments}
                  onChange={(e) => handleInputChange('totalShipments', parseInt(e.target.value) || 0)}
                />
              </div>

              <div>
                <Label htmlFor="defectiveItems">不良品数</Label>
                <Input
                  id="defectiveItems"
                  type="number"
                  min="0"
                  value={formData.defectiveItems}
                  onChange={(e) => handleInputChange('defectiveItems', parseInt(e.target.value) || 0)}
                />
              </div>
            </div>
          </div>
        </div>

        <div>
          <Label htmlFor="description">詳細説明 *</Label>
          <textarea
            id="description"
            className="w-full min-h-[100px] p-3 border border-gray-300 rounded-md"
            value={formData.description}
            onChange={(e) => handleInputChange('description', e.target.value)}
            placeholder="インシデントの詳細を入力してください"
            required
          />
        </div>

        <div>
          <Label htmlFor="incidentDetails">発生経緯</Label>
          <textarea
            id="incidentDetails"
            className="w-full min-h-[80px] p-3 border border-gray-300 rounded-md"
            value={formData.incidentDetails}
            onChange={(e) => handleInputChange('incidentDetails', e.target.value)}
            placeholder="インシデントの発生経緯を入力してください"
          />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <Label htmlFor="occurrenceLocation">発生場所</Label>
            <Input
              id="occurrenceLocation"
              value={formData.occurrenceLocation}
              onChange={(e) => handleInputChange('occurrenceLocation', e.target.value)}
              placeholder="発生場所を入力"
            />
          </div>

          <div>
            <Label htmlFor="summary">概要</Label>
            <Input
              id="summary"
              value={formData.summary}
              onChange={(e) => handleInputChange('summary', e.target.value)}
              placeholder="インシデントの概要を入力"
            />
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <Label htmlFor="cause">原因</Label>
            <textarea
              id="cause"
              className="w-full min-h-[80px] p-3 border border-gray-300 rounded-md"
              value={formData.cause}
              onChange={(e) => handleInputChange('cause', e.target.value)}
              placeholder="原因を入力してください"
            />
          </div>

          <div>
            <Label htmlFor="preventionMeasures">再発防止策</Label>
            <textarea
              id="preventionMeasures"
              className="w-full min-h-[80px] p-3 border border-gray-300 rounded-md"
              value={formData.preventionMeasures}
              onChange={(e) => handleInputChange('preventionMeasures', e.target.value)}
              placeholder="再発防止策を入力してください"
            />
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <Label htmlFor="effectivenessStatus">有効性評価</Label>
            <Select
              value={formData.effectivenessStatus}
              onValueChange={(value) => handleInputChange('effectivenessStatus', value as EffectivenessStatus)}
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="NotImplemented">未実施</SelectItem>
                <SelectItem value="Implemented">実施</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div>
            <Label htmlFor="effectivenessComment">有効性確認コメント</Label>
            <Input
              id="effectivenessComment"
              value={formData.effectivenessComment}
              onChange={(e) => handleInputChange('effectivenessComment', e.target.value)}
              placeholder="有効性確認コメントを入力"
            />
          </div>
        </div>
      </div>
    );
  };

  if (masterDataLoading) {
    return <div className="flex justify-center p-8">マスタデータを読み込み中...</div>;
  }

  if (masterDataError) {
    return <div className="text-red-500 p-4">マスタデータの取得に失敗しました: {masterDataError}</div>;
  }

  return (
    <div className="space-y-6">
      {/* ワークフローモード切り替えボタン */}
      <div className="flex justify-between items-center">
        <h2 className="text-xl font-semibold">
          インシデント{incident ? '編集' : '新規作成'}
        </h2>
        <div className="flex items-center gap-4">
          <Button
            type="button"
            variant="outline"
            onClick={() => setCurrentWorkflowMode(prev => prev === 'legacy' ? 'new' : 'legacy')}
          >
            {currentWorkflowMode === 'legacy' ? '新ワークフローを有効化' : '従来モードに戻る'}
          </Button>
          <Badge variant="outline">
            {currentWorkflowMode === 'legacy' ? 'レガシーモード' : '新ワークフローモード'}
          </Badge>
        </div>
      </div>

      {/* ワークフロー状況表示 */}
      {renderWorkflowStatus()}

      {/* フォーム内容 */}
      <form onSubmit={handleSubmit} className="space-y-6">
        {renderLegacyForm()}

        {/* アクションボタン */}
        <div className="flex justify-end space-x-4 pt-6 border-t">
          {onCancel && (
            <Button type="button" variant="outline" onClick={onCancel}>
              キャンセル
            </Button>
          )}
          <Button type="submit" disabled={loading}>
            {loading ? '保存中...' : incident ? '更新' : '作成'}
          </Button>
        </div>
      </form>
    </div>
  );
}
