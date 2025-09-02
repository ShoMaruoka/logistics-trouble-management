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
      <table className="w-full border-collapse border border-gray-300">
        <thead>
          <tr className="bg-gray-50">
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('occurrenceDate')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded font-medium"
              >
                発生日 {getSortIcon('occurrenceDate')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('troubleTypeId')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded font-medium"
              >
                トラブル種類 {getSortIcon('troubleTypeId')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('damageTypeId')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded font-medium"
              >
                損傷の種類 {getSortIcon('damageTypeId')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('warehouseId')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded font-medium"
              >
                出荷元倉庫 {getSortIcon('warehouseId')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('shippingCompanyId')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded font-medium"
              >
                運送会社名 {getSortIcon('shippingCompanyId')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('effectivenessStatus')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded font-medium"
              >
                有効性確認 {getSortIcon('effectivenessStatus')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left font-medium">アクション</th>
          </tr>
        </thead>
        <tbody>
          {incidents.map((incident) => (
            <tr 
              key={incident.id} 
              className="hover:bg-gray-50 cursor-pointer"
              onClick={() => onEdit(incident)}
            >
              <td className="border border-gray-300 px-4 py-2">
                {new Date(incident.occurrenceDate).toLocaleDateString('ja-JP')}
              </td>
              <td className="border border-gray-300 px-4 py-2">
                <span 
                  className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium text-white"
                  style={{ backgroundColor: incident.troubleTypeColor }}
                >
                  {incident.troubleTypeName}
                </span>
              </td>
              <td className="border border-gray-300 px-4 py-2">
                {incident.damageTypeName}
              </td>
              <td className="border border-gray-300 px-4 py-2">
                {incident.warehouseName}
              </td>
              <td className="border border-gray-300 px-4 py-2">
                {incident.shippingCompanyName}
              </td>
              <td className="border border-gray-300 px-4 py-2">
                <span className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${
                  incident.effectivenessStatus === 'Implemented' 
                    ? 'bg-green-100 text-green-800' 
                    : 'bg-gray-100 text-gray-800'
                }`}>
                  {incident.effectivenessStatus === 'Implemented' 
                    ? '実施済み' 
                    : '未実施'}
                </span>
              </td>
              <td className="border border-gray-300 px-4 py-2">
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
                      className="flex items-center gap-1 text-red-600 hover:text-red-700"
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
