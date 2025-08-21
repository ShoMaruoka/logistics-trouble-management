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

  // 物流特化フィールドの日本語表示用ヘルパー関数
  const getTroubleTypeLabel = (troubleType: string) => {
    switch (troubleType) {
      case 'ProductTrouble':
        return '商品トラブル';
      case 'DeliveryTrouble':
        return '配送トラブル';
      default:
        return troubleType;
    }
  };

  const getDamageTypeLabel = (damageType: string) => {
    switch (damageType) {
      case 'WrongShipment':
        return '誤出荷';
      case 'EarlyOrLateArrival':
        return '早着・延着';
      case 'Lost':
        return '紛失';
      case 'WrongDelivery':
        return '誤配送';
      case 'DamageOrContamination':
        return '破損・汚損';
      case 'OtherDeliveryMistake':
        return 'その他の配送ミス';
      case 'OtherProductAccident':
        return 'その他の商品事故';
      default:
        return damageType;
    }
  };

  const getWarehouseLabel = (warehouse: string) => {
    switch (warehouse) {
      case 'WarehouseA':
        return 'A倉庫';
      case 'WarehouseB':
        return 'B倉庫';
      case 'WarehouseC':
        return 'C倉庫';
      default:
        return warehouse;
    }
  };

  const getShippingCompanyLabel = (shippingCompany: string) => {
    switch (shippingCompany) {
      case 'InHouse':
        return '庫内';
      case 'Charter':
        return 'チャーター';
      case 'ATransport':
        return 'A運輸';
      case 'BExpress':
        return 'B急便';
      default:
        return shippingCompany;
    }
  };

  const getEffectivenessStatusLabel = (effectivenessStatus: string) => {
    switch (effectivenessStatus) {
      case 'NotImplemented':
        return '未実施';
      case 'Implemented':
        return '実施';
      default:
        return effectivenessStatus;
    }
  };

  const getEffectivenessStatusBadge = (effectivenessStatus: string) => {
    const isImplemented = effectivenessStatus === 'Implemented';
    return (
      <span className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${
        isImplemented 
          ? 'bg-logistics-green text-white' 
          : 'bg-gray-100 text-gray-800'
      }`}>
        {getEffectivenessStatusLabel(effectivenessStatus)}
      </span>
    );
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
                日付 {getSortIcon('occurrenceDate')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('troubleType')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded font-medium"
              >
                トラブル種類 {getSortIcon('troubleType')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('damageType')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded font-medium"
              >
                損傷の種類 {getSortIcon('damageType')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('warehouse')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded font-medium"
              >
                出荷元倉庫 {getSortIcon('warehouse')}
              </button>
            </th>
            <th className="border border-gray-300 px-4 py-2 text-left">
              <button
                onClick={() => requestSort('shippingCompany')}
                className="flex items-center gap-1 hover:bg-gray-100 px-2 py-1 rounded font-medium"
              >
                運送会社名 {getSortIcon('shippingCompany')}
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
                <span className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${
                  incident.troubleType === 'ProductTrouble' 
                    ? 'bg-logistics-red text-white' 
                    : 'bg-logistics-orange text-white'
                }`}>
                  {getTroubleTypeLabel(incident.troubleType)}
                </span>
              </td>
              <td className="border border-gray-300 px-4 py-2">
                {getDamageTypeLabel(incident.damageType)}
              </td>
              <td className="border border-gray-300 px-4 py-2">
                {getWarehouseLabel(incident.warehouse)}
              </td>
              <td className="border border-gray-300 px-4 py-2">
                {getShippingCompanyLabel(incident.shippingCompany)}
              </td>
              <td className="border border-gray-300 px-4 py-2">
                {getEffectivenessStatusBadge(incident.effectivenessStatus)}
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
