import * as React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Search, Filter, X } from "lucide-react";
import type { EffectivenessSearchDto } from "@/lib/api-types";

interface EffectivenessSearchProps {
  searchParams: EffectivenessSearchDto;
  onSearchChange: (params: EffectivenessSearchDto) => void;
  onClear: () => void;
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

export function EffectivenessSearch({ searchParams, onSearchChange, onClear }: EffectivenessSearchProps) {
  const [isExpanded, setIsExpanded] = React.useState(false);

  const handleInputChange = (field: keyof EffectivenessSearchDto, value: string | number | undefined) => {
    onSearchChange({
      ...searchParams,
      [field]: value,
      page: 1, // 検索時は1ページ目に戻す
    });
  };

  const handleClear = () => {
    onClear();
    setIsExpanded(false);
  };

  return (
    <div className="bg-white p-4 rounded-lg border shadow-sm">
      {/* 基本検索 */}
      <div className="flex gap-2 mb-4">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
          <Input
            placeholder="インシデントIDで検索..."
            value={searchParams.incidentId?.toString() || ''}
            onChange={(e) => handleInputChange('incidentId', e.target.value ? parseInt(e.target.value) : undefined)}
            className="pl-10"
          />
        </div>
        <Button
          variant="outline"
          onClick={() => setIsExpanded(!isExpanded)}
          className="flex items-center gap-2"
        >
          <Filter className="h-4 w-4" />
          詳細検索
        </Button>
        {(searchParams.incidentId || searchParams.effectivenessType || searchParams.minImprovementRate || searchParams.maxImprovementRate) && (
          <Button
            variant="outline"
            onClick={handleClear}
            className="flex items-center gap-2"
          >
            <X className="h-4 w-4" />
            クリア
          </Button>
        )}
      </div>

      {/* 詳細検索 */}
      {isExpanded && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 pt-4 border-t">
          <div className="space-y-2">
            <Label htmlFor="effectiveness-type">効果測定タイプ</Label>
            <Select
              value={searchParams.effectivenessType || ''}
              onValueChange={(value) => handleInputChange('effectivenessType', value)}
            >
              <SelectTrigger>
                <SelectValue placeholder="すべて" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">すべて</SelectItem>
                {EFFECTIVENESS_TYPES.map((type) => (
                  <SelectItem key={type.value} value={type.value}>
                    {type.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="min-improvement-rate">最小改善率 (%)</Label>
            <Input
              id="min-improvement-rate"
              type="number"
              step="0.1"
              placeholder="例: 10.0"
              value={searchParams.minImprovementRate || ''}
              onChange={(e) => handleInputChange('minImprovementRate', e.target.value ? parseFloat(e.target.value) : undefined)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="max-improvement-rate">最大改善率 (%)</Label>
            <Input
              id="max-improvement-rate"
              type="number"
              step="0.1"
              placeholder="例: 50.0"
              value={searchParams.maxImprovementRate || ''}
              onChange={(e) => handleInputChange('maxImprovementRate', e.target.value ? parseFloat(e.target.value) : undefined)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="measured-from">測定日（開始）</Label>
            <Input
              id="measured-from"
              type="date"
              value={searchParams.measuredFrom ? new Date(searchParams.measuredFrom).toISOString().split('T')[0] : ''}
              onChange={(e) => handleInputChange('measuredFrom', e.target.value || undefined)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="measured-to">測定日（終了）</Label>
            <Input
              id="measured-to"
              type="date"
              value={searchParams.measuredTo ? new Date(searchParams.measuredTo).toISOString().split('T')[0] : ''}
              onChange={(e) => handleInputChange('measuredTo', e.target.value || undefined)}
            />
          </div>
        </div>
      )}
    </div>
  );
}
