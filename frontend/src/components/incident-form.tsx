import * as React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import type { IncidentDto, CreateIncidentDto, UpdateIncidentDto } from "@/lib/api-types";

interface IncidentFormProps {
  incident?: IncidentDto | null;
  onSubmit: (data: CreateIncidentDto | UpdateIncidentDto) => void;
  onCancel?: () => void;
  loading?: boolean;
}

export function IncidentForm({ incident, onSubmit, onCancel, loading = false }: IncidentFormProps) {
  const [formData, setFormData] = React.useState({
    title: incident?.title || '',
    description: incident?.description || '',
    category: incident?.category || '',
    priority: incident?.priority || 'Medium' as 'Low' | 'Medium' | 'High' | 'Critical',
    resolution: incident?.resolution || '',
  });

  React.useEffect(() => {
    if (incident) {
      setFormData({
        title: incident.title,
        description: incident.description,
        category: incident.category,
        priority: incident.priority,
        resolution: incident.resolution || '',
      });
    }
  }, [incident]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div className="space-y-2">
        <Label htmlFor="title">タイトル *</Label>
        <Input
          id="title"
          value={formData.title}
          onChange={(e) => setFormData(prev => ({ ...prev, title: e.target.value }))}
          placeholder="インシデントのタイトルを入力"
          required
        />
      </div>

      <div className="space-y-2">
        <Label htmlFor="description">詳細 *</Label>
        <textarea
          className="w-full min-h-[100px] p-3 border rounded-md"
          value={formData.description}
          onChange={(e) => setFormData(prev => ({ ...prev, description: e.target.value }))}
          placeholder="インシデントの詳細を入力"
          required
        />
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="category">カテゴリ *</Label>
          <Input
            id="category"
            value={formData.category}
            onChange={(e) => setFormData(prev => ({ ...prev, category: e.target.value }))}
            placeholder="カテゴリを入力"
            required
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="priority">優先度 *</Label>
          <Select 
            value={formData.priority} 
            onValueChange={(value) => setFormData(prev => ({ ...prev, priority: value as 'Low' | 'Medium' | 'High' | 'Critical' }))}
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
      </div>

      {/* 解決内容（編集時のみ表示） */}
      {incident && (
        <div className="space-y-2">
          <Label htmlFor="resolution">解決内容</Label>
          <textarea
            id="resolution"
            className="w-full min-h-[100px] p-3 border rounded-md"
            value={formData.resolution}
            onChange={(e) => setFormData(prev => ({ ...prev, resolution: e.target.value }))}
            placeholder="解決内容を入力"
          />
        </div>
      )}

      <div className="flex justify-end gap-2 pt-4">
        {onCancel && (
          <Button type="button" variant="outline" onClick={onCancel}>
            キャンセル
          </Button>
        )}
        <Button type="submit" disabled={loading}>
          {loading ? '保存中...' : (incident ? '更新' : '作成')}
        </Button>
      </div>
    </form>
  );
}
