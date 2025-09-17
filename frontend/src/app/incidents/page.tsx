'use client';

import * as React from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Plus, AlertCircle } from "lucide-react";
import { IncidentList } from "@/components/incident-list";
import { IncidentSearch } from "@/components/incident-search";
import { Pagination } from "@/components/pagination";
import { IncidentDetail } from "@/components/incident-detail";
import { IncidentModal } from "@/components/IncidentModal";
import { FilterControls } from "@/components/filter-controls";
import { useFilter } from "@/contexts/FilterContext";
import { 
  useIncidents, 
  useCreateIncident, 
  useUpdateIncident, 
  useDeleteIncident 
} from "@/lib/hooks";
import type { 
  Incident, 
  IncidentSearchDto, 
  CreateIncidentDto, 
  UpdateIncidentDto 
} from "@/lib/types";

export default function IncidentsPage() {
  // 共通フィルタ状態
  const { selectedYear, selectedMonth } = useFilter();
  
  // 状態管理
  const [searchParams, setSearchParams] = React.useState<IncidentSearchDto>({
    page: 1,
    pageSize: 10,
    sortBy: 'occurrenceDate',
    ascending: false,
  });
  
  const [sortConfig, setSortConfig] = React.useState<{
    key: keyof Incident;
    direction: 'ascending' | 'descending';
  } | null>({
    key: 'occurrenceDate',
    direction: 'descending',
  });

  const [selectedIncident, setSelectedIncident] = React.useState<Incident | null>(null);
  const [showDetail, setShowDetail] = React.useState(false);
  const [showForm, setShowForm] = React.useState(false);
  const [editingIncident, setEditingIncident] = React.useState<Incident | null>(null);

  // API Hooks
  const { data: incidentsData, loading: incidentsLoading, error: incidentsError, refetch: refetchIncidents } = useIncidents(searchParams);
  const { createIncident, loading: createLoading } = useCreateIncident();
  const { updateIncident, loading: updateLoading } = useUpdateIncident();
  const { deleteIncident, loading: deleteLoading } = useDeleteIncident();

  // ソート処理
  const handleSort = (key: keyof Incident) => {
    const direction = sortConfig?.key === key && sortConfig.direction === 'ascending' ? 'descending' : 'ascending';
    setSortConfig({ key, direction });
    setSearchParams(prev => ({
      ...prev,
      sortBy: key,
      ascending: direction === 'ascending',
    }));
  };

  // 検索処理
  const handleSearchChange = (params: IncidentSearchDto) => {
    setSearchParams(prev => ({ ...prev, ...params }));
  };

  const handleSearchClear = () => {
    setSearchParams({
      page: 1,
      pageSize: 10,
      sortBy: 'occurrenceDate',
      ascending: false,
    });
    setSortConfig({
      key: 'occurrenceDate',
      direction: 'descending',
    });
  };

  // フィルタ状態に応じた検索パラメータの更新
  React.useEffect(() => {
    setSearchParams(prev => {
      const newSearchParams: IncidentSearchDto = {
        ...prev,
        page: 1, // フィルタ変更時は1ページ目に戻す
      };

      // 年度・月フィルタの適用
      if (selectedYear > 0) {
        const startDate = new Date(selectedYear, 0, 1); // 1月1日
        const endDate = new Date(selectedYear, 11, 31); // 12月31日
        
        if (selectedMonth > 0) {
          // 月間表示の場合
          startDate.setMonth(selectedMonth - 1);
          endDate.setMonth(selectedMonth - 1);
          endDate.setDate(new Date(selectedYear, selectedMonth, 0).getDate()); // 月末日
        }
        
        // ローカル日付ゲッターを使用してYYYY-MM-DD形式に変換
        const formatDate = (date: Date): string => {
          const year = date.getFullYear();
          const month = String(date.getMonth() + 1).padStart(2, '0');
          const day = String(date.getDate()).padStart(2, '0');
          return `${year}-${month}-${day}`;
        };
        
        newSearchParams.fromDate = formatDate(startDate);
        newSearchParams.toDate = formatDate(endDate);
      } else {
        // フィルタをクリア
        newSearchParams.fromDate = undefined;
        newSearchParams.toDate = undefined;
      }

      return newSearchParams;
    });
  }, [selectedYear, selectedMonth]);

  // エラーメッセージを安全に正規化する関数
  const normalizeErrorMessage = (error: unknown): string => {
    if (typeof error === 'string') {
      return error;
    }
    
    if (error && typeof error === 'object' && 'message' in error) {
      return String(error.message);
    }
    
    try {
      return JSON.stringify(error);
    } catch {
      return String(error);
    }
  };

  // ページネーション処理
  const handlePageChange = (page: number) => {
    setSearchParams(prev => ({ ...prev, page }));
  };

  // インシデント操作
  const handleIncidentClick = (incident: Incident) => {
    setSelectedIncident(incident);
    setShowDetail(true);
  };

  const handleDeleteIncident = async (incident: Incident) => {
    if (window.confirm(`インシデント「${incident.title}」を削除しますか？`)) {
      try {
        await deleteIncident(incident.id);
        refetchIncidents();
      } catch (error) {
        console.error('削除エラー:', error);
        alert('削除に失敗しました');
      }
    }
  };

  const handleCreateIncident = () => {
    setEditingIncident(null);
    setShowForm(true);
  };

  const handleFormSubmit = async (data: CreateIncidentDto | UpdateIncidentDto) => {
    console.log('handleFormSubmit called:', { editingIncident, data });
    try {
      if (editingIncident) {
        console.log('Updating incident:', editingIncident.id, data);
        await updateIncident(editingIncident.id, data as UpdateIncidentDto);
        console.log('Update completed successfully');
      } else {
        console.log('Creating new incident:', data);
        await createIncident(data as CreateIncidentDto);
        console.log('Create completed successfully');
      }
      setShowForm(false);
      setEditingIncident(null);
      refetchIncidents();
    } catch (error) {
      console.error('保存エラー:', error);
      alert('保存に失敗しました');
    }
  };

  const handleFormCancel = () => {
    setShowForm(false);
    setEditingIncident(null);
  };

  const handleDetailEdit = () => {
    if (selectedIncident) {
      setEditingIncident(selectedIncident);
      setShowDetail(false);
      setShowForm(true);
    }
  };

  const handleDetailClose = () => {
    setShowDetail(false);
    setSelectedIncident(null);
  };

  const handleCSVExport = () => {
    // CSV出力機能（将来的に実装）
    console.log('CSV出力機能');
  };

  return (
    <div className="container mx-auto p-6 space-y-6">
      {/* ヘッダー */}
      <div className="bg-white p-6 rounded-lg shadow-sm">
        <h1 className="text-3xl font-bold text-gray-900 mb-2">インシデント管理</h1>
        <h2 className="text-xl font-bold text-gray-900">物流トラブル一覧</h2>
      </div>

      {/* 年度・月フィルタ */}
      <FilterControls />

      {/* 検索・CSV出力 */}
      <div className="bg-white p-6 rounded-lg shadow-sm">
        <div className="flex gap-4 items-center">
          <div className="flex-1 relative">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <svg className="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </div>
            <input
              type="text"
              placeholder="物流トラブルを検索..."
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              onChange={(e) => handleSearchChange({ ...searchParams, searchTerm: e.target.value })}
            />
          </div>
          <Button 
            onClick={handleCSVExport} 
            className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700"
          >
            <svg className="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
            CSV出力
          </Button>
        </div>
      </div>

      {/* エラー表示 */}
      {incidentsError && (
        <Card className="border-red-200 bg-red-50">
          <CardContent className="pt-6">
            <div className="flex items-center gap-2 text-red-600">
              <AlertCircle className="h-5 w-4" />
              <span>エラー: {normalizeErrorMessage(incidentsError)}</span>
            </div>
          </CardContent>
        </Card>
      )}

      {/* インシデント一覧 */}
      <div className="bg-white rounded-lg shadow-sm overflow-hidden">
        <IncidentList
          incidents={incidentsData?.items || []}
          requestSort={handleSort}
          sortConfig={sortConfig}
          onEdit={handleIncidentClick}
          onDelete={handleDeleteIncident}
          loading={incidentsLoading || deleteLoading}
        />
      </div>

      {/* ページネーション */}
      {incidentsData && (
        <div className="bg-white px-6 py-3 flex items-center justify-between border-t border-gray-200">
          <div className="flex-1 flex justify-between sm:hidden">
            <Button
              variant="outline"
              onClick={() => setSearchParams(prev => ({ ...prev, page: Math.max(1, prev.page - 1) }))}
              disabled={searchParams.page <= 1}
            >
              前へ
            </Button>
            <Button
              variant="outline"
              onClick={() => setSearchParams(prev => ({ ...prev, page: Math.min(incidentsData.totalPages, prev.page + 1) }))}
              disabled={searchParams.page >= incidentsData.totalPages}
            >
              次へ
            </Button>
          </div>
          <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
            <div>
              <p className="text-sm text-gray-700">
                <span className="font-medium">{incidentsData.totalCount}</span>件中
                <span className="font-medium">{(searchParams.page - 1) * searchParams.pageSize + 1}</span>-
                <span className="font-medium">{Math.min(searchParams.page * searchParams.pageSize, incidentsData.totalCount)}</span>件を表示
              </p>
            </div>
            <div>
              <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
                <Button
                  variant="outline"
                  onClick={() => setSearchParams(prev => ({ ...prev, page: Math.max(1, prev.page - 1) }))}
                  disabled={searchParams.page <= 1}
                  className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50"
                >
                  前へ
                </Button>
                <Button
                  variant="outline"
                  onClick={() => setSearchParams(prev => ({ ...prev, page: Math.min(incidentsData.totalPages, prev.page + 1) }))}
                  disabled={searchParams.page >= incidentsData.totalPages}
                  className="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50"
                >
                  次へ
                </Button>
              </nav>
            </div>
          </div>
        </div>
      )}

      {/* 詳細モーダル */}
      {showDetail && selectedIncident && (
        <IncidentDetail
          incident={selectedIncident}
          onEdit={handleDetailEdit}
          onClose={handleDetailClose}
        />
      )}

      {/* フォームモーダル */}
      <IncidentModal
        open={showForm}
        onOpenChange={(open) => {
          if (!open) {
            handleFormCancel();
          }
        }}
        incident={editingIncident}
        onSubmit={handleFormSubmit}
        loading={createLoading || updateLoading}
        title={editingIncident ? 'インシデント編集' : '新規インシデント作成'}
        description={editingIncident ? 'インシデントの情報を編集してください。' : '新しいインシデントを作成してください。'}
      />
    </div>
  );
}
