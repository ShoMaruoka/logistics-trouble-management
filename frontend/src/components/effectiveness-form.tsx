import * as React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { TrendingUp, X } from "lucide-react";
import type { EffectivenessDto, CreateEffectivenessDto, UpdateEffectivenessDto } from "@/lib/types";

interface EffectivenessFormProps {
  effectiveness?: EffectivenessDto;
  onSave: (data: CreateEffectivenessDto | UpdateEffectivenessDto) => void;
  onCancel: () => void;
  loading?: boolean;
}

const EFFECTIVENESS_TYPES = [
  { value: 'CostReduction', label: 'コスト削減' },
  { value: 'TimeReduction', label: '時間短縮' },
  { value: 'QualityImprovement', label: '品質向上' },
  { value: 'SafetyImprovement', label: '安全性向上' },
  { value: 'EfficiencyImprovement', label: '効率性向上' },
  { value: 'CustomerSatisfaction', label: '顧客満足度' },
  { value: 'ProcessOptimization', label: 'プロセス最適化' },
  { value: 'ResourceUtilization', label: 'リソース活用率' },
  { value: 'ErrorReduction', label: 'エラー削減' },
  { value: 'Other', label: 'その他' }
];

export function EffectivenessForm({ effectiveness, onSave, onCancel, loading = false }: EffectivenessFormProps) {
  const [formData, setFormData] = React.useState({
    incidentId: effectiveness?.incidentId || 0,
    effectivenessType: effectiveness?.effectivenessType || '',
    beforeValue: effectiveness?.beforeValue || 0,
    afterValue: effectiveness?.afterValue || 0,
    description: effectiveness?.description || '',
    measuredById: 1 // 仮のユーザーID（編集時は変更不可）
  });

  const [errors, setErrors] = React.useState<Record<string, string>>({});

  React.useEffect(() => {
    if (effectiveness) {
      setFormData({
        incidentId: effectiveness.incidentId,
        effectivenessType: effectiveness.effectivenessType,
        beforeValue: effectiveness.beforeValue,
        afterValue: effectiveness.afterValue,
        description: effectiveness.description,
        measuredById: 1 // 編集時は仮のユーザーIDを使用
      });
    }
  }, [effectiveness]);

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.incidentId || formData.incidentId <= 0) {
      newErrors.incidentId = 'インシデントIDを入力してください';
    }

    if (!formData.effectivenessType) {
      newErrors.effectivenessType = '効果測定タイプを選択してください';
    }

    if (formData.beforeValue < 0) {
      newErrors.beforeValue = '改善前の値は0以上で入力してください';
    }

    if (formData.afterValue < 0) {
      newErrors.afterValue = '改善後の値は0以上で入力してください';
    }

    if (!formData.description.trim()) {
      newErrors.description = '説明を入力してください';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    if (effectiveness) {
      // 更新
      const updateData: UpdateEffectivenessDto = {
        effectivenessType: formData.effectivenessType,
        beforeValue: formData.beforeValue,
        afterValue: formData.afterValue,
        description: formData.description
      };
      onSave(updateData);
    } else {
      // 新規作成
      const createData: CreateEffectivenessDto = {
        incidentId: formData.incidentId,
        effectivenessType: formData.effectivenessType,
        beforeValue: formData.beforeValue,
        afterValue: formData.afterValue,
        improvementRate: calculateImprovementRate(),
        description: formData.description,
        measuredById: formData.measuredById
      };
      onSave(createData);
    }
  };

  const handleInputChange = (field: string, value: string | number) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    // エラーをクリア
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }));
    }
  };

  const calculateImprovementRate = () => {
    if (formData.beforeValue === 0) return 0;
    return ((formData.afterValue - formData.beforeValue) / formData.beforeValue) * 100;
  };

  const improvementRate = calculateImprovementRate();

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <TrendingUp className="h-5 w-5" />
          {effectiveness ? '効果測定編集' : '効果測定登録'}
        </CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          {/* インシデントID */}
          <div className="space-y-2">
            <Label htmlFor="incident-id">インシデントID *</Label>
            <Input
              id="incident-id"
              type="number"
              value={formData.incidentId}
              onChange={(e) => handleInputChange('incidentId', parseInt(e.target.value) || 0)}
              placeholder="インシデントIDを入力"
              disabled={!!effectiveness} // 編集時は変更不可
              className={errors.incidentId ? 'border-red-500' : ''}
            />
            {errors.incidentId && (
              <p className="text-sm text-red-500">{errors.incidentId}</p>
            )}
          </div>

          {/* 効果測定タイプ */}
          <div className="space-y-2">
            <Label htmlFor="effectiveness-type">効果測定タイプ *</Label>
            <Select
              value={formData.effectivenessType}
              onValueChange={(value) => handleInputChange('effectivenessType', value)}
            >
              <SelectTrigger className={errors.effectivenessType ? 'border-red-500' : ''}>
                <SelectValue placeholder="効果測定タイプを選択" />
              </SelectTrigger>
              <SelectContent>
                {EFFECTIVENESS_TYPES.map((type) => (
                  <SelectItem key={type.value} value={type.value}>
                    {type.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {errors.effectivenessType && (
              <p className="text-sm text-red-500">{errors.effectivenessType}</p>
            )}
          </div>

          {/* 改善前・改善後の値 */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="before-value">改善前の値 *</Label>
              <Input
                id="before-value"
                type="number"
                step="0.01"
                value={formData.beforeValue}
                onChange={(e) => handleInputChange('beforeValue', parseFloat(e.target.value) || 0)}
                placeholder="0.00"
                className={errors.beforeValue ? 'border-red-500' : ''}
              />
              {errors.beforeValue && (
                <p className="text-sm text-red-500">{errors.beforeValue}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="after-value">改善後の値 *</Label>
              <Input
                id="after-value"
                type="number"
                step="0.01"
                value={formData.afterValue}
                onChange={(e) => handleInputChange('afterValue', parseFloat(e.target.value) || 0)}
                placeholder="0.00"
                className={errors.afterValue ? 'border-red-500' : ''}
              />
              {errors.afterValue && (
                <p className="text-sm text-red-500">{errors.afterValue}</p>
              )}
            </div>
          </div>

          {/* 改善率の表示 */}
          <div className="p-3 bg-gray-50 rounded-lg">
            <div className="text-sm text-gray-600 mb-1">改善率</div>
            <div className={`text-lg font-semibold ${
              improvementRate > 0 ? 'text-green-600' : 
              improvementRate < 0 ? 'text-red-600' : 'text-gray-600'
            }`}>
              {improvementRate > 0 ? '+' : ''}{improvementRate.toFixed(1)}%
            </div>
          </div>

          {/* 説明 */}
          <div className="space-y-2">
            <Label htmlFor="description">説明 *</Label>
            <Textarea
              id="description"
              value={formData.description}
              onChange={(e) => handleInputChange('description', e.target.value)}
              placeholder="効果測定の詳細を入力してください"
              rows={4}
              className={errors.description ? 'border-red-500' : ''}
            />
            {errors.description && (
              <p className="text-sm text-red-500">{errors.description}</p>
            )}
          </div>

          {/* アクションボタン */}
          <div className="flex justify-end space-x-2 pt-4">
            <Button type="button" variant="outline" onClick={onCancel} disabled={loading}>
              キャンセル
            </Button>
            <Button
              type="submit"
              disabled={loading}
              className="flex items-center gap-2"
            >
              {loading ? (
                <>
                  <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                  保存中...
                </>
              ) : (
                <>
                  <TrendingUp className="h-4 w-4" />
                  {effectiveness ? '更新' : '登録'}
                </>
              )}
            </Button>
          </div>
        </form>
      </CardContent>
    </Card>
  );
}
