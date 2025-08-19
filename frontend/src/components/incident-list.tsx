import * as React from "react";
import { Button } from "@/components/ui/button";
import { Edit, Trash2 } from "lucide-react";
import type { IncidentDto } from "@/lib/api-types";

interface IncidentListProps {
  incidents: IncidentDto[];
  requestSort: (key: keyof IncidentDto) => void;
  sortConfig: { key: keyof IncidentDto; direction: 'ascending' | 'descending' } | null;
  onEdit: (incident: IncidentDto) => void;
  onDelete?: (incident: IncidentDto) => void;
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
  const getSortIcon = (key: keyof IncidentDto) => {
    if (sortConfig?.key !== key) return "↕";
    return sortConfig.direction === 'ascending' ? "↑" : "↓";
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Open':
        return 'bg-blue-100 text-blue-800';
      case 'InProgress':
        return 'bg-yellow-100 text-yellow-800';
      case 'Resolved':
        return 'bg-green-100 text-green-800';
      case 'Closed':
        return 'bg-gray-100 text-gray-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'Critical':
        return 'bg-red-100 text-red-800';
      case 'High':
        return 'bg-orange-100 text-orange-800';
      case 'Medium':
        return 'bg-yellow-100 text-yellow-800';
      case 'Low':
        return 'bg-green-100 text-green-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center py-8">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <span className="ml-2">読み込み中...</span>
      </div>
    );
  }

  if (incidents.length === 0) {
    return (
      <div className="text-center py-8 text-gray-500">
        インシデントが見つかりません
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
                onClick={() => requestSort('reportedDate')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded"
              >
                報告日 {getSortIcon('reportedDate')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('title')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded"
              >
                タイトル {getSortIcon('title')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('category')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded"
              >
                カテゴリ {getSortIcon('category')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('priority')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded"
              >
                優先度 {getSortIcon('priority')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('status')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded"
              >
                ステータス {getSortIcon('status')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('reportedByName')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded"
              >
                報告者 {getSortIcon('reportedByName')}
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
              <td className="border border-gray-300 px-4 py-2">
                {new Date(incident.reportedDate).toLocaleDateString('ja-JP')}
              </td>
              <td className="border border-gray-300 px-4 py-2">
                <div className="max-w-xs truncate" title={incident.title}>
                  {incident.title}
                </div>
              </td>
              <td className="border border-gray-300 px-4 py-2">{incident.category}</td>
              <td className="border border-gray-300 px-4 py-2">
                <span className={`px-2 py-1 rounded-full text-xs font-medium ${getPriorityColor(incident.priority)}`}>
                  {incident.priority}
                </span>
              </td>
              <td className="border border-gray-300 px-4 py-2">
                <span className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(incident.status)}`}>
                  {incident.status}
                </span>
              </td>
              <td className="border border-gray-300 px-4 py-2">{incident.reportedByName}</td>
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
