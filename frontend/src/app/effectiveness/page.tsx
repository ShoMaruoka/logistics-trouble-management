'use client';

import * as React from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Plus, AlertCircle, TrendingUp } from "lucide-react";
import { EffectivenessList } from "@/components/effectiveness-list";
import { EffectivenessSearch } from "@/components/effectiveness-search";
import { EffectivenessForm } from "@/components/effectiveness-form";
import { Pagination } from "@/components/pagination";
import {
  useEffectiveness,
  useCreateEffectiveness,
  useUpdateEffectiveness,
  useDeleteEffectiveness,
  useEffectivenessSummary
} from "@/lib/hooks";
import type {
  EffectivenessDto,
  EffectivenessSearchDto,
  CreateEffectivenessDto,
  UpdateEffectivenessDto
} from "@/lib/types";

export default function EffectivenessPage() {
  // 状態管理
  const [searchParams, setSearchParams] = React.useState<EffectivenessSearchDto>({
    page: 1,
    pageSize: 10,
    sortBy: 'MeasuredAt',
    ascending: false,
  });

  const [showForm, setShowForm] = React.useState(false);
  const [editingEffectiveness, setEditingEffectiveness] = React.useState<EffectivenessDto | undefined>(undefined);

  // API Hooks
  const { data: effectivenessData, loading: effectivenessLoading, error: effectivenessError, refetch: refetchEffectiveness } = useEffectiveness(searchParams);
  const { data: summaryData, loading: summaryLoading, error: summaryError } = useEffectivenessSummary();
  const { createEffectiveness, loading: createLoading } = useCreateEffectiveness();
  const { updateEffectiveness, loading: updateLoading } = useUpdateEffectiveness();
  const { deleteEffectiveness, loading: deleteLoading } = useDeleteEffectiveness();

  // 検索処理
  const handleSearchChange = (params: EffectivenessSearchDto) => {
    setSearchParams(prev => ({ ...prev, ...params }));
  };

  const handleSearchClear = () => {
    setSearchParams({
      page: 1,
      pageSize: 10,
      sortBy: 'MeasuredAt',
      ascending: false,
    });
  };

  // ページネーション処理
  const handlePageChange = (page: number) => {
    setSearchParams(prev => ({ ...prev, page }));
  };

  // 効果測定操作
  const handleEdit = (effectiveness: EffectivenessDto) => {
    setEditingEffectiveness(effectiveness);
    setShowForm(true);
  };

  const handleDelete = async (effectiveness: EffectivenessDto) => {
    if (window.confirm(`効果測定「${effectiveness.effectivenessType}」を削除しますか？`)) {
      try {
        await deleteEffectiveness(effectiveness.id);
        refetchEffectiveness();
      } catch (error) {
        console.error('削除エラー:', error);
        alert('削除に失敗しました');
      }
    }
  };

  const handleSave = async (data: CreateEffectivenessDto | UpdateEffectivenessDto) => {
    try {
      if (editingEffectiveness) {
        // 更新
        await updateEffectiveness(editingEffectiveness.id, data as UpdateEffectivenessDto);
      } else {
        // 新規作成
        await createEffectiveness(data as CreateEffectivenessDto);
      }
      setShowForm(false);
      setEditingEffectiveness(undefined);
      refetchEffectiveness();
    } catch (error) {
      console.error('保存エラー:', error);
      alert('保存に失敗しました');
    }
  };

  const handleCancel = () => {
    setShowForm(false);
    setEditingEffectiveness(undefined);
  };

  const handleCreateNew = () => {
    setEditingEffectiveness(undefined);
    setShowForm(true);
  };

  return (
    <div className="container mx-auto p-6 space-y-6">
      {/* ヘッダー */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">効果測定管理</h1>
          <p className="text-gray-600 mt-2">インシデント対応の効果を測定・管理します</p>
        </div>
        <Button onClick={handleCreateNew} className="flex items-center gap-2">
          <Plus className="h-4 w-4" />
          効果測定登録
        </Button>
      </div>

      {/* サマリー情報 */}
      {summaryData && (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center space-x-2">
                <TrendingUp className="h-5 w-5 text-blue-600" />
                <div>
                  <p className="text-sm font-medium text-gray-600">総測定数</p>
                  <p className="text-2xl font-bold text-gray-900">{summaryData.totalMeasurements}</p>
                </div>
              </div>
            </CardContent>
          </Card>
          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center space-x-2">
                <TrendingUp className="h-5 w-5 text-green-600" />
                <div>
                  <p className="text-sm font-medium text-gray-600">平均改善率</p>
                  <p className="text-2xl font-bold text-gray-900">{summaryData.averageImprovementRate.toFixed(1)}%</p>
                </div>
              </div>
            </CardContent>
          </Card>
          <Card>
            <CardContent className="pt-6">
              <div className="flex items-center space-x-2">
                <TrendingUp className="h-5 w-5 text-orange-600" />
                <div>
                  <p className="text-sm font-medium text-gray-600">最大改善率</p>
                  <p className="text-2xl font-bold text-gray-900">{summaryData.maxImprovementRate.toFixed(1)}%</p>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      )}

      {/* フォーム */}
      {showForm && (
        <EffectivenessForm
          effectiveness={editingEffectiveness}
          onSave={handleSave}
          onCancel={handleCancel}
          loading={createLoading || updateLoading}
        />
      )}

      {/* 検索・フィルタリング */}
      <EffectivenessSearch
        searchParams={searchParams}
        onSearchChange={handleSearchChange}
        onClear={handleSearchClear}
      />

      {/* エラー表示 */}
      {(effectivenessError || summaryError) && (
        <Card className="border-red-200 bg-red-50">
          <CardContent className="pt-6">
            <div className="flex items-center gap-2 text-red-600">
              <AlertCircle className="h-5 w-5" />
              <span>エラー: {effectivenessError || summaryError}</span>
            </div>
          </CardContent>
        </Card>
      )}

      {/* 効果測定一覧 */}
      <Card>
        <CardHeader>
          <CardTitle>効果測定一覧</CardTitle>
        </CardHeader>
        <CardContent>
          <EffectivenessList
            effectiveness={effectivenessData?.items || []}
            onEdit={handleEdit}
            onDelete={handleDelete}
            loading={effectivenessLoading || deleteLoading}
          />
        </CardContent>
      </Card>

      {/* ページネーション */}
      {effectivenessData && effectivenessData.totalPages > 1 && (
        <div className="flex justify-center">
          <Pagination
            currentPage={effectivenessData.page}
            totalPages={effectivenessData.totalPages}
            onPageChange={handlePageChange}
          />
        </div>
      )}
    </div>
  );
}
