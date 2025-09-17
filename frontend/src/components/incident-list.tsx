import * as React from "react";
import { Button } from "@/components/ui/button";
import { Edit, Trash2 } from "lucide-react";
import type { Incident } from "@/lib/types";

interface IncidentListProps {
  incidents: Incident[];
  requestSort: (key: keyof Incident) => void;
  sortConfig: { key: keyof Incident; direction: 'ascending' | 'descending' } | null;
  onEdit: (incident: Incident) => void;
  onDelete?: (incident: Incident) => void;
  loading?: boolean;
}

export function IncidentList({ 
  incidents, 
  requestSort, 
  sortConfig, 
  onEdit, 
  onDelete,
  loading = false 
}: IncidentListProps) {
  const getSortIcon = (key: keyof Incident) => {
    if (sortConfig?.key !== key) return "↕";
    return sortConfig.direction === 'ascending' ? "↑" : "↓";
  };

  const getTroubleTypeColor = (troubleTypeName: string | undefined) => {
    switch (troubleTypeName) {
      case '品質不良':
        return '#fbbf24'; // 黄色
      case '破損':
        return '#ef4444'; // 赤色
      case '遅延':
        return '#14b8a6'; // ティール色
      default:
        return '#6b7280'; // グレー
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center py-8">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-logistics-blue"></div>
        <span className="ml-2">読み込み中...</span>
      </div>
    );
  }

  if (incidents.length === 0) {
    return (
      <div className="text-center py-8 text-gray-500">
        物流トラブルが見つかりません
      </div>
    );
  }

  return (
    <div className="overflow-x-auto">
      <table className="w-full">
        <thead>
          <tr className="bg-gray-50 border-b">
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              <button
                onClick={() => requestSort('occurrenceDate')}
                className="flex items-center gap-1 hover:text-gray-700 font-medium"
              >
                発生日 {getSortIcon('occurrenceDate')}
              </button>
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              <button
                onClick={() => requestSort('troubleTypeId')}
                className="flex items-center gap-1 hover:text-gray-700 font-medium"
              >
                トラブル種類 {getSortIcon('troubleTypeId')}
              </button>
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              <button
                onClick={() => requestSort('damageTypeId')}
                className="flex items-center gap-1 hover:text-gray-700 font-medium"
              >
                損傷の種類 {getSortIcon('damageTypeId')}
              </button>
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              <button
                onClick={() => requestSort('warehouseId')}
                className="flex items-center gap-1 hover:text-gray-700 font-medium"
              >
                出荷元倉庫 {getSortIcon('warehouseId')}
              </button>
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              <button
                onClick={() => requestSort('shippingCompanyId')}
                className="flex items-center gap-1 hover:text-gray-700 font-medium"
              >
                運送会社名 {getSortIcon('shippingCompanyId')}
              </button>
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              <button
                onClick={() => requestSort('effectivenessStatus')}
                className="flex items-center gap-1 hover:text-gray-700 font-medium"
              >
                有効性確認 {getSortIcon('effectivenessStatus')}
              </button>
            </th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">アクション</th>
          </tr>
        </thead>
        <tbody className="bg-white divide-y divide-gray-200">
          {incidents.map((incident) => (
            <tr 
              key={incident.id} 
              className="hover:bg-gray-50"
            >
              <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {incident.occurrenceDate ? new Date(incident.occurrenceDate).toLocaleDateString('ja-JP') : '未設定'}
              </td>
              <td className="px-6 py-4 whitespace-nowrap">
                <span 
                  className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium text-white"
                  style={{ backgroundColor: getTroubleTypeColor(incident.troubleTypeName) }}
                >
                  {incident.troubleTypeName || '未設定'}
                </span>
              </td>
              <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {incident.damageTypeName || '未設定'}
              </td>
              <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {incident.warehouseName || '未設定'}
              </td>
              <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {incident.shippingCompanyName || '未設定'}
              </td>
              <td className="px-6 py-4 whitespace-nowrap">
                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                  incident.effectivenessStatus === 'Implemented' 
                    ? 'bg-green-100 text-green-800' 
                    : 'bg-gray-100 text-gray-800'
                }`}>
                  {incident.effectivenessStatus === 'Implemented' 
                    ? '実施済み' 
                    : '未実施'}
                </span>
              </td>
              <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                <div className="flex gap-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={(e) => {
                      e.stopPropagation();
                      onEdit(incident);
                    }}
                    className="flex items-center gap-1"
                  >
                    <Edit className="h-4 w-4" />
                    表示・編集
                  </Button>
                  {onDelete && (
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={(e) => {
                        e.stopPropagation();
                        onDelete(incident);
                      }}
                      className="flex items-center gap-1 text-red-600 hover:text-red-700 hover:bg-red-50"
                    >
                      <Trash2 className="h-4 w-4" />
                      削除
                    </Button>
                  )}
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
