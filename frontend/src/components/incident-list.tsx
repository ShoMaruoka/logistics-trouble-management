import * as React from "react";
import { Button } from "@/components/ui/button";
import { Edit } from "lucide-react";
import type { Incident } from "@/lib/types";

interface IncidentListProps {
  incidents: Incident[];
  requestSort: (key: keyof Incident) => void;
  sortConfig: { key: keyof Incident; direction: 'ascending' | 'descending' } | null;
  onEdit: (incident: Incident) => void;
}

export function IncidentList({ incidents, requestSort, sortConfig, onEdit }: IncidentListProps) {
  const getSortIcon = (key: keyof Incident) => {
    if (sortConfig?.key !== key) return "↕";
    return sortConfig.direction === 'ascending' ? "↑" : "↓";
  };

  return (
    <div className="overflow-x-auto">
      <table className="w-full border-collapse border border-gray-300">
        <thead>
          <tr className="bg-gray-50">
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('date')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded"
              >
                日付 {getSortIcon('date')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('troubleType')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded"
              >
                トラブル種類 {getSortIcon('troubleType')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('damageType')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded"
              >
                損傷の種類 {getSortIcon('damageType')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('shippingWarehouse')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded"
              >
                出荷元倉庫 {getSortIcon('shippingWarehouse')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('shippingCompany')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded"
              >
                運送会社名 {getSortIcon('shippingCompany')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('effectivenessCheckStatus')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded"
              >
                有効性確認 {getSortIcon('effectivenessCheckStatus')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">アクション</th>
          </tr>
        </thead>
        <tbody>
          {incidents.map((incident) => (
            <tr 
              key={incident.id} 
              className="hover:bg-gray-50 cursor-pointer"
              onClick={() => onEdit(incident)}
            >
              <td className="border border-gray-300 px-4 py-2">{incident.date}</td>
              <td className="border border-gray-300 px-4 py-2">{incident.troubleType}</td>
              <td className="border border-gray-300 px-4 py-2">{incident.damageType}</td>
              <td className="border border-gray-300 px-4 py-2">{incident.shippingWarehouse || '-'}</td>
              <td className="border border-gray-300 px-4 py-2">{incident.shippingCompany || '-'}</td>
              <td className="border border-gray-300 px-4 py-2">{incident.effectivenessCheckStatus || '未実施'}</td>
              <td className="border border-gray-300 px-4 py-2">
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
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
