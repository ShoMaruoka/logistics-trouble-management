import * as React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { useMasterData } from "@/hooks/useMasterData";
import { useAuth } from "@/contexts/AuthContext";
import type { Incident, IncidentWithWorkflow, Priority } from "@/lib/types";

interface IncidentClassificationFormProps {
  incident: Incident;
  onSubmit: (data: ClassificationData) => void;
  onCancel?: () => void;
  loading?: boolean;
}

interface ClassificationData {
  troubleTypeId: number;
  damageTypeId: number;
  warehouseId: number;
  shippingCompanyId: number;
  totalShipments: number;
  defectiveItems: number;
  priority: Priority;
  dueDate: string;
}

/**
 * インシデント管理者専用の分類フォーム
 * 仕様書2.3新しいワークフローに準拠：
 * - 操作者: インシデント管理者
 * - 条件: 未分類状態のインシデント
 * - 登録内容: トラブル種類、損傷種類、出荷総数、不良品数、出荷元倉庫、運送会社名、優先度、対応期限
 * - 遷移後ステータス: 未対応
 */
export function IncidentClassificationForm({ 
  incident, 
  onSubmit, 
  onCancel, 
  loading = false 
}: IncidentClassificationFormProps) {
  const { user } = useAuth();
  const { troubleTypes, damageTypes, warehouses, shippingCompanies, loading: masterDataLoading, error: masterDataError } = useMasterData();
  
  const [formData, setFormData] = React.useState<ClassificationData>({
    troubleTypeId: incident.troubleTypeId || 0,
    damageTypeId: incident.damageTypeId || 0,
    warehouseId: incident.warehouseId || 0,
    shippingCompanyId: incident.shippingCompanyId || 0,
    totalShipments: incident.totalShipments || 0,
    defectiveItems: incident.defectiveItems || 0,
    priority: incident.priority || 'Medium',
    dueDate: (incident as any).dueDate ? (incident as any).dueDate.split('T')[0] : '',
  });

  const [errors, setErrors] = React.useState<Record<string, string>>({});

  // 数値入力のバリデーション
  const validateNumericInput = (value: string): number => {
    const parsed = parseInt(value, 10);
    return isNaN(parsed) || parsed < 0 ? 0 : parsed;
  };

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.troubleTypeId || formData.troubleTypeId === 0) {
      newErrors.troubleTypeId = 'トラブル種類を選択してください';
    }

    if (!formData.damageTypeId || formData.damageTypeId === 0) {
      newErrors.damageTypeId = '損傷種類を選択してください';
    }

    if (!formData.warehouseId || formData.warehouseId === 0) {
      newErrors.warehouseId = '出荷元倉庫を選択してください';
    }

    if (!formData.shippingCompanyId || formData.shippingCompanyId === 0) {
      newErrors.shippingCompanyId = '運送会社を選択してください';
    }

    if (!formData.dueDate) {
      newErrors.dueDate = '対応期限を設定してください';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    onSubmit(formData);
  };

  if (masterDataLoading) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-logistics-blue mx-auto mb-4"></div>
          <p className="text-gray-600">マスタデータを読み込み中...</p>
        </div>
      </div>
    );
  }

  if (masterDataError) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-center text-red-600">
          <p>マスタデータの読み込みに失敗しました</p>
          <p className="text-sm mt-2">{masterDataError}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* インシデント情報表示 */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            インシデント分類作業
            <Badge variant="secondary">未分類</Badge>
          </CardTitle>
          <CardDescription>
            事務員が登録したインシデントの詳細分類を行ってください
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
            <div>
              <span className="font-medium text-gray-700">タイトル:</span>
              <p className="mt-1">{incident.title}</p>
            </div>
            <div>
              <span className="font-medium text-gray-700">発生日:</span>
              <p className="mt-1">{incident.occurrenceDate ? new Date(incident.occurrenceDate).toLocaleDateString('ja-JP') : '未設定'}</p>
            </div>
            <div className="md:col-span-2">
              <span className="font-medium text-gray-700">詳細説明:</span>
              <p className="mt-1 text-gray-600">{incident.description}</p>
            </div>
            <div className="md:col-span-2">
              <span className="font-medium text-gray-700">発生経緯:</span>
              <p className="mt-1 text-gray-600">{incident.incidentDetails || '未記載'}</p>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* 分類フォーム */}
      <form onSubmit={handleSubmit} className="space-y-6">
        <Card>
          <CardHeader>
            <CardTitle>分類情報</CardTitle>
            <CardDescription>
              インシデントの詳細分類を行ってください
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-6">
            {/* トラブル種類・損傷種類 */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <Label htmlFor="troubleType" className="text-sm font-medium text-gray-700">
                  トラブル種類 <span className="text-red-500">*</span>
                </Label>
                <Select 
                  value={formData.troubleTypeId.toString()} 
                  onValueChange={(value) => setFormData(prev => ({ ...prev, troubleTypeId: parseInt(value) }))}
                >
                  <SelectTrigger className={`${errors.troubleTypeId ? 'border-red-500' : ''}`}>
                    <SelectValue placeholder="トラブル種類を選択" />
                  </SelectTrigger>
                  <SelectContent>
                    {troubleTypes.map((troubleType) => (
                      <SelectItem key={troubleType.id} value={troubleType.id.toString()}>
                        <div className="flex items-center gap-2">
                          <div 
                            className="w-3 h-3 rounded-full" 
                            style={{ backgroundColor: troubleType.color }}
                          />
                          {troubleType.name}
                        </div>
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {errors.troubleTypeId && <p className="text-sm text-red-600">{errors.troubleTypeId}</p>}
              </div>

              <div className="space-y-2">
                <Label htmlFor="damageType" className="text-sm font-medium text-gray-700">
                  損傷種類 <span className="text-red-500">*</span>
                </Label>
                <Select 
                  value={formData.damageTypeId.toString()} 
                  onValueChange={(value) => setFormData(prev => ({ ...prev, damageTypeId: parseInt(value) }))}
                >
                  <SelectTrigger className={`${errors.damageTypeId ? 'border-red-500' : ''}`}>
                    <SelectValue placeholder="損傷種類を選択" />
                  </SelectTrigger>
                  <SelectContent>
                    {damageTypes.map((damageType) => (
                      <SelectItem key={damageType.id} value={damageType.id.toString()}>
                        <div className="flex items-center gap-2">
                          <span className="text-xs px-2 py-1 bg-gray-100 rounded">
                            {damageType.category}
                          </span>
                          {damageType.name}
                        </div>
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {errors.damageTypeId && <p className="text-sm text-red-600">{errors.damageTypeId}</p>}
              </div>
            </div>

            {/* 出荷総数・不良品数 */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <Label htmlFor="totalShipments" className="text-sm font-medium text-gray-700">
                  出荷総数
                </Label>
                <Input
                  id="totalShipments"
                  type="number"
                  min="0"
                  value={formData.totalShipments}
                  onChange={(e) => setFormData(prev => ({ ...prev, totalShipments: validateNumericInput(e.target.value) }))}
                  placeholder="0"
                  className="border-gray-300 focus:ring-2 focus:ring-logistics-blue"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="defectiveItems" className="text-sm font-medium text-gray-700">
                  不良品数
                </Label>
                <Input
                  id="defectiveItems"
                  type="number"
                  min="0"
                  value={formData.defectiveItems}
                  onChange={(e) => setFormData(prev => ({ ...prev, defectiveItems: validateNumericInput(e.target.value) }))}
                  placeholder="0"
                  className="border-gray-300 focus:ring-2 focus:ring-logistics-blue"
                />
              </div>
            </div>

            {/* 出荷元倉庫・運送会社 */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <Label htmlFor="warehouse" className="text-sm font-medium text-gray-700">
                  出荷元倉庫 <span className="text-red-500">*</span>
                </Label>
                <Select 
                  value={formData.warehouseId.toString()} 
                  onValueChange={(value) => setFormData(prev => ({ ...prev, warehouseId: parseInt(value) }))}
                >
                  <SelectTrigger className={`${errors.warehouseId ? 'border-red-500' : ''}`}>
                    <SelectValue placeholder="倉庫を選択" />
                  </SelectTrigger>
                  <SelectContent>
                    {warehouses.map((warehouse) => (
                      <SelectItem key={warehouse.id} value={warehouse.id.toString()}>
                        <div className="flex items-center gap-2">
                          {warehouse.name}
                          {warehouse.location && (
                            <span className="text-xs text-gray-500">({warehouse.location})</span>
                          )}
                        </div>
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {errors.warehouseId && <p className="text-sm text-red-600">{errors.warehouseId}</p>}
              </div>

              <div className="space-y-2">
                <Label htmlFor="shippingCompany" className="text-sm font-medium text-gray-700">
                  運送会社 <span className="text-red-500">*</span>
                </Label>
                <Select 
                  value={formData.shippingCompanyId.toString()} 
                  onValueChange={(value) => setFormData(prev => ({ ...prev, shippingCompanyId: parseInt(value) }))}
                >
                  <SelectTrigger className={`${errors.shippingCompanyId ? 'border-red-500' : ''}`}>
                    <SelectValue placeholder="運送会社を選択" />
                  </SelectTrigger>
                  <SelectContent>
                    {shippingCompanies.map((shippingCompany) => (
                      <SelectItem key={shippingCompany.id} value={shippingCompany.id.toString()}>
                        <div className="flex items-center gap-2">
                          {shippingCompany.name}
                          <span className="text-xs px-2 py-1 bg-gray-100 rounded">
                            {shippingCompany.companyType}
                          </span>
                        </div>
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {errors.shippingCompanyId && <p className="text-sm text-red-600">{errors.shippingCompanyId}</p>}
              </div>
            </div>

            {/* 優先度・対応期限 */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <Label htmlFor="priority" className="text-sm font-medium text-gray-700">
                  優先度 <span className="text-red-500">*</span>
                </Label>
                <Select 
                  value={formData.priority} 
                  onValueChange={(value) => setFormData(prev => ({ ...prev, priority: value as Priority }))}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="優先度を選択" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Critical">
                      <div className="flex items-center gap-2">
                        <div className="w-3 h-3 rounded-full bg-red-500" />
                        緊急
                      </div>
                    </SelectItem>
                    <SelectItem value="High">
                      <div className="flex items-center gap-2">
                        <div className="w-3 h-3 rounded-full bg-orange-500" />
                        高
                      </div>
                    </SelectItem>
                    <SelectItem value="Medium">
                      <div className="flex items-center gap-2">
                        <div className="w-3 h-3 rounded-full bg-yellow-500" />
                        中
                      </div>
                    </SelectItem>
                    <SelectItem value="Low">
                      <div className="flex items-center gap-2">
                        <div className="w-3 h-3 rounded-full bg-green-500" />
                        低
                      </div>
                    </SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-2">
                <Label htmlFor="dueDate" className="text-sm font-medium text-gray-700">
                  対応期限 <span className="text-red-500">*</span>
                </Label>
                <Input
                  id="dueDate"
                  type="date"
                  value={formData.dueDate}
                  onChange={(e) => setFormData(prev => ({ ...prev, dueDate: e.target.value }))}
                  className={`border-gray-300 focus:ring-2 focus:ring-logistics-blue ${
                    errors.dueDate ? 'border-red-500' : ''
                  }`}
                />
                {errors.dueDate && <p className="text-sm text-red-600">{errors.dueDate}</p>}
              </div>
            </div>
          </CardContent>
        </Card>

        {/* ボタン */}
        <div className="flex justify-end space-x-3">
          {onCancel && (
            <Button
              type="button"
              variant="outline"
              onClick={onCancel}
              disabled={loading}
            >
              キャンセル
            </Button>
          )}
          <Button
            type="submit"
            disabled={loading}
            className="bg-logistics-blue hover:bg-logistics-blue/90 text-white"
          >
            {loading ? '分類中...' : '分類完了'}
          </Button>
        </div>
      </form>
    </div>
  );
}
