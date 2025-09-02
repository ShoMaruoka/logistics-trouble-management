import * as React from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import { Edit, Trash2, TrendingUp, TrendingDown, Minus } from "lucide-react";
import type { EffectivenessDto } from "@/lib/types";

interface EffectivenessListProps {
  effectiveness: EffectivenessDto[];
  onEdit: (effectiveness: EffectivenessDto) => void;
  onDelete: (effectiveness: EffectivenessDto) => void;
  loading?: boolean;
}

export function EffectivenessList({ effectiveness, onEdit, onDelete, loading = false }: EffectivenessListProps) {
  const formatDate = (date: string) => {
    return new Date(date).toLocaleString('ja-JP', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const getImprovementIcon = (improvementRate: number) => {
    if (improvementRate > 0) {
      return <TrendingUp className="h-4 w-4 text-green-600" />;
    } else if (improvementRate < 0) {
      return <TrendingDown className="h-4 w-4 text-red-600" />;
    } else {
      return <Minus className="h-4 w-4 text-gray-600" />;
    }
  };

  const getImprovementBadge = (improvementRate: number) => {
    if (improvementRate > 0) {
      return <Badge variant="default" className="bg-green-100 text-green-800">改善</Badge>;
    } else if (improvementRate < 0) {
      return <Badge variant="destructive">悪化</Badge>;
    } else {
      return <Badge variant="secondary">変化なし</Badge>;
    }
  };

  const formatImprovementRate = (rate: number) => {
    const sign = rate > 0 ? '+' : '';
    return `${sign}${rate.toFixed(1)}%`;
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center py-8">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (effectiveness.length === 0) {
    return (
      <div className="text-center py-8 text-gray-500">
        効果測定データがありません
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {effectiveness.map((item) => (
        <Card key={item.id} className="hover:shadow-md transition-shadow">
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div className="flex-1">
                <div className="flex items-center space-x-2 mb-2">
                  <h3 className="text-lg font-medium text-gray-900">
                    {item.effectivenessType}
                  </h3>
                  {getImprovementBadge(item.improvementRate)}
                </div>
                
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-3">
                  <div className="text-sm">
                    <span className="text-gray-500">改善前: </span>
                    <span className="font-medium">{item.beforeValue}</span>
                  </div>
                  <div className="text-sm">
                    <span className="text-gray-500">改善後: </span>
                    <span className="font-medium">{item.afterValue}</span>
                  </div>
                  <div className="text-sm flex items-center space-x-1">
                    <span className="text-gray-500">改善率: </span>
                    {getImprovementIcon(item.improvementRate)}
                    <span className={`font-medium ${
                      item.improvementRate > 0 ? 'text-green-600' : 
                      item.improvementRate < 0 ? 'text-red-600' : 'text-gray-600'
                    }`}>
                      {formatImprovementRate(item.improvementRate)}
                    </span>
                  </div>
                </div>

                <div className="text-sm text-gray-600 mb-3">
                  <p className="mb-1">{item.description}</p>
                </div>

                <div className="text-xs text-gray-500 space-y-1">
                  <div>インシデント: {item.incidentTitle}</div>
                  <div>測定日時: {formatDate(item.measuredAt)}</div>
                  <div>測定者: {item.measuredBy}</div>
                </div>
              </div>

              <div className="flex items-center space-x-2 ml-4">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => onEdit(item)}
                  className="flex items-center space-x-1"
                >
                  <Edit className="h-4 w-4" />
                  <span>編集</span>
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => onDelete(item)}
                  className="flex items-center space-x-1 text-red-600 hover:text-red-700"
                >
                  <Trash2 className="h-4 w-4" />
                  <span>削除</span>
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
