import * as React from "react";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { AlertCircle } from "lucide-react";
import { useMasterData } from "@/hooks/useMasterData";

import type { 
  IncidentWithWorkflow, 
  ClassifyIncidentDto,
  Priority
} from "@/lib/types";

interface ClassificationFormProps {
  incident: IncidentWithWorkflow;
  onSubmit: (action: string, data: any) => void;
  loading?: boolean;
}

export function ClassificationForm({ incident, onSubmit, loading = false }: ClassificationFormProps) {
  const { troubleTypes, damageTypes, warehouses, shippingCompanies, loading: masterDataLoading } = useMasterData();
  
  const [formData, setFormData] = useState<Omit<ClassifyIncidentDto, 'incidentId'>>({
    troubleTypeId: incident.troubleTypeId || 0,
    damageTypeId: incident.damageTypeId || 0,
    warehouseId: incident.warehouseId || 0,
    shippingCompanyId: incident.shippingCompanyId || 0,
    totalShipments: incident.totalShipments || 0,
    defectiveItems: incident.defectiveItems || 0,
    priority: incident.priority || 'Medium',
    dueDate: incident.dueDate || new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit('classify', { ...formData, incidentId: incident.id });
  };

  const handleInputChange = (field: keyof typeof formData, value: any) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  if (masterDataLoading) {
    return <div className="p-4">マスタデータを読み込み中...</div>;
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <AlertCircle className="w-5 h-5 text-gray-500" />
          インシデント分類
        </CardTitle>
        <CardDescription>
          インシデントの詳細情報を設定して分類を完了してください。
        </CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <Label>トラブル種類 *</Label>
              <Select
                value={formData.troubleTypeId.toString()}
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
              <Label>損傷種類 *</Label>
              <Select
                value={formData.damageTypeId.toString()}
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
              <Label>出荷元倉庫 *</Label>
              <Select
                value={formData.warehouseId.toString()}
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
              <Label>運送会社 *</Label>
              <Select
                value={formData.shippingCompanyId.toString()}
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

            <div>
              <Label>出荷総数</Label>
              <Input
                type="number"
                min="0"
                value={formData.totalShipments}
                onChange={(e) => handleInputChange('totalShipments', parseInt(e.target.value) || 0)}
              />
            </div>

            <div>
              <Label>不良品数</Label>
              <Input
                type="number"
                min="0"
                max={formData.totalShipments}
                value={formData.defectiveItems}
                onChange={(e) => handleInputChange('defectiveItems', parseInt(e.target.value) || 0)}
              />
            </div>

            <div>
              <Label>優先度</Label>
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
              <Label>対応期限 *</Label>
              <Input
                type="date"
                value={formData.dueDate}
                onChange={(e) => handleInputChange('dueDate', e.target.value)}
                min={new Date().toISOString().split('T')[0]}
              />
            </div>
          </div>

          <Button 
            type="submit"
            disabled={loading}
            className="w-full"
          >
            {loading ? '分類中...' : 'インシデントを分類'}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
