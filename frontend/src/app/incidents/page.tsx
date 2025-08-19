'use client';

import * as React from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Plus, AlertCircle } from "lucide-react";
import { IncidentList } from "@/components/incident-list";
import { IncidentSearch } from "@/components/incident-search";
import { Pagination } from "@/components/pagination";
import { IncidentDetail } from "@/components/incident-detail";
import { IncidentForm } from "@/components/incident-form";
import { 
  useIncidents, 
  useCreateIncident, 
  useUpdateIncident, 
  useDeleteIncident 
} from "@/lib/hooks";
import type { 
  IncidentDto, 
  IncidentSearchDto, 
  CreateIncidentDto, 
  UpdateIncidentDto 
} from "@/lib/api-types";

export default function IncidentsPage() {
  // 状態管理
  const [searchParams, setSearchParams] = React.useState<IncidentSearchDto>({
    page: 1,
    pageSize: 10,
    sortBy: 'reportedDate',
    ascending: false,
  });
  
  const [sortConfig, setSortConfig] = React.useState<{
    key: keyof IncidentDto;
    direction: 'ascending' | 'descending';
  } | null>({
    key: 'reportedDate',
    direction: 'descending',
  });

  const [selectedIncident, setSelectedIncident] = React.useState<IncidentDto | null>(null);
  const [showDetail, setShowDetail] = React.useState(false);
  const [showForm, setShowForm] = React.useState(false);
  const [editingIncident, setEditingIncident] = React.useState<IncidentDto | null>(null);

  // API Hooks
  const { data: incidentsData, loading: incidentsLoading, error: incidentsError, refetch: refetchIncidents } = useIncidents(searchParams);
  const { createIncident, loading: createLoading } = useCreateIncident();
  const { updateIncident, loading: updateLoading } = useUpdateIncident();
  const { deleteIncident, loading: deleteLoading } = useDeleteIncident();

  // ソート処理
  const handleSort = (key: keyof IncidentDto) => {
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
      sortBy: 'reportedDate',
      ascending: false,
    });
    setSortConfig({
      key: 'reportedDate',
      direction: 'descending',
    });
  };

  // ページネーション処理
  const handlePageChange = (page: number) => {
    setSearchParams(prev => ({ ...prev, page }));
  };

  // インシデント操作
  const handleIncidentClick = (incident: IncidentDto) => {
    setSelectedIncident(incident);
    setShowDetail(true);
  };



  const handleDeleteIncident = async (incident: IncidentDto) => {
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
    try {
      if (editingIncident) {
        await updateIncident(editingIncident.id, data as UpdateIncidentDto);
      } else {
        await createIncident(data as CreateIncidentDto);
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

  return (
    <div className="container mx-auto p-6 space-y-6">
      {/* ヘッダー */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">トラブル管理</h1>
          <p className="text-gray-600 mt-2">インシデントの一覧、検索、管理を行います</p>
        </div>
        <Button onClick={handleCreateIncident} className="flex items-center gap-2">
          <Plus className="h-4 w-4" />
          新規インシデント
        </Button>
      </div>

      {/* 検索・フィルタリング */}
      <IncidentSearch
        searchParams={searchParams}
        onSearchChange={handleSearchChange}
        onClear={handleSearchClear}
      />

      {/* エラー表示 */}
      {incidentsError && (
        <Card className="border-red-200 bg-red-50">
          <CardContent className="pt-6">
            <div className="flex items-center gap-2 text-red-600">
              <AlertCircle className="h-5 w-5" />
              <span>エラー: {incidentsError}</span>
            </div>
          </CardContent>
        </Card>
      )}

      {/* インシデント一覧 */}
      <Card>
        <CardHeader>
          <CardTitle>インシデント一覧</CardTitle>
        </CardHeader>
        <CardContent>
          <IncidentList
            incidents={incidentsData?.items || []}
            requestSort={handleSort}
            sortConfig={sortConfig}
            onEdit={handleIncidentClick}
            onDelete={handleDeleteIncident}
            loading={incidentsLoading || deleteLoading}
          />
        </CardContent>
      </Card>

      {/* ページネーション */}
      {incidentsData && incidentsData.totalPages > 1 && (
        <div className="flex justify-center">
          <Pagination
            currentPage={incidentsData.page}
            totalPages={incidentsData.totalPages}
            onPageChange={handlePageChange}
          />
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
      {showForm && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-xl font-bold">
                  {editingIncident ? 'インシデント編集' : '新規インシデント作成'}
                </h2>
                <Button variant="outline" onClick={handleFormCancel}>
                  閉じる
                </Button>
              </div>
              <IncidentForm
                incident={editingIncident}
                onSubmit={handleFormSubmit}
                onCancel={handleFormCancel}
                loading={createLoading || updateLoading}
              />
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
