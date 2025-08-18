import * as React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import type { Incident } from "@/lib/types";

interface IncidentFormProps {
  onSave: (data: Omit<Incident, 'id'> & { id?: string }) => void;
  incidents: Incident[];
  onCancel: () => void;
  incidentToEdit: Incident | null;
}

export function IncidentForm({ onSave, incidents, onCancel, incidentToEdit }: IncidentFormProps) {
  const [formData, setFormData] = React.useState({
    troubleType: incidentToEdit?.troubleType || '商品トラブル',
    processDescription: incidentToEdit?.processDescription || '',
    damageType: incidentToEdit?.damageType || '',
    location: incidentToEdit?.location || '',
    date: incidentToEdit?.date || new Date().toISOString().split('T')[0],
    description: incidentToEdit?.description || '',
    cause: incidentToEdit?.cause || '',
    recurrencePreventionMeasures: incidentToEdit?.recurrencePreventionMeasures || '',
    shipmentVolume: incidentToEdit?.shipmentVolume || 0,
    defectiveUnits: incidentToEdit?.defectiveUnits || 0,
    shippingWarehouse: incidentToEdit?.shippingWarehouse || '',
    shippingCompany: incidentToEdit?.shippingCompany || '',
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSave({
      ...formData,
      id: incidentToEdit?.id,
    });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="troubleType">トラブル種別</Label>
          <Select value={formData.troubleType} onValueChange={(value) => setFormData(prev => ({ ...prev, troubleType: value as '配送トラブル' | '商品トラブル' }))}>
            <SelectTrigger>
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="商品トラブル">商品トラブル</SelectItem>
              <SelectItem value="配送トラブル">配送トラブル</SelectItem>
            </SelectContent>
          </Select>
        </div>
        <div className="space-y-2">
          <Label htmlFor="damageType">損害の種類</Label>
          <Input
            id="damageType"
            value={formData.damageType}
            onChange={(e) => setFormData(prev => ({ ...prev, damageType: e.target.value }))}
            placeholder="損害の種類を入力"
          />
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="processDescription">プロセス説明</Label>
        <textarea
          className="w-full min-h-[100px] p-3 border rounded-md"
          value={formData.processDescription}
          onChange={(e) => setFormData(prev => ({ ...prev, processDescription: e.target.value }))}
          placeholder="トラブルの詳細なプロセスを説明してください"
        />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="location">発生場所</Label>
          <Input
            id="location"
            value={formData.location}
            onChange={(e) => setFormData(prev => ({ ...prev, location: e.target.value }))}
            placeholder="発生場所を入力"
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="date">発生日</Label>
          <Input
            id="date"
            type="date"
            value={formData.date}
            onChange={(e) => setFormData(prev => ({ ...prev, date: e.target.value }))}
          />
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="description">トラブル概要</Label>
        <textarea
          className="w-full min-h-[80px] p-3 border rounded-md"
          value={formData.description}
          onChange={(e) => setFormData(prev => ({ ...prev, description: e.target.value }))}
          placeholder="トラブルの概要を入力"
        />
      </div>

      <div className="space-y-2">
        <Label htmlFor="cause">原因</Label>
        <textarea
          className="w-full min-h-[80px] p-3 border rounded-md"
          value={formData.cause}
          onChange={(e) => setFormData(prev => ({ ...prev, cause: e.target.value }))}
          placeholder="トラブルの原因を入力"
        />
      </div>

      <div className="space-y-2">
        <Label htmlFor="recurrencePreventionMeasures">再発防止策</Label>
        <textarea
          className="w-full min-h-[100px] p-3 border rounded-md"
          value={formData.recurrencePreventionMeasures}
          onChange={(e) => setFormData(prev => ({ ...prev, recurrencePreventionMeasures: e.target.value }))}
          placeholder="再発防止策を入力"
        />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="shipmentVolume">出荷数量</Label>
          <Input
            id="shipmentVolume"
            type="number"
            value={formData.shipmentVolume}
            onChange={(e) => setFormData(prev => ({ ...prev, shipmentVolume: parseInt(e.target.value) || 0 }))}
            placeholder="出荷数量"
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="defectiveUnits">不良品数</Label>
          <Input
            id="defectiveUnits"
            type="number"
            value={formData.defectiveUnits}
            onChange={(e) => setFormData(prev => ({ ...prev, defectiveUnits: parseInt(e.target.value) || 0 }))}
            placeholder="不良品数"
          />
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="shippingWarehouse">出荷倉庫</Label>
          <Select value={formData.shippingWarehouse} onValueChange={(value) => setFormData(prev => ({ ...prev, shippingWarehouse: value }))}>
            <SelectTrigger>
              <SelectValue placeholder="倉庫を選択" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="A倉庫">A倉庫</SelectItem>
              <SelectItem value="B倉庫">B倉庫</SelectItem>
              <SelectItem value="C倉庫">C倉庫</SelectItem>
            </SelectContent>
          </Select>
        </div>
        <div className="space-y-2">
          <Label htmlFor="shippingCompany">配送会社</Label>
          <Select value={formData.shippingCompany} onValueChange={(value) => setFormData(prev => ({ ...prev, shippingCompany: value }))}>
            <SelectTrigger>
              <SelectValue placeholder="会社を選択" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="A社">A社</SelectItem>
              <SelectItem value="B社">B社</SelectItem>
              <SelectItem value="ヤマト">ヤマト</SelectItem>
              <SelectItem value="佐川">佐川</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </div>

      <div className="flex justify-end space-x-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          キャンセル
        </Button>
        <Button type="submit">
          {incidentToEdit ? '更新' : '登録'}
        </Button>
      </div>
    </form>
  );
}
