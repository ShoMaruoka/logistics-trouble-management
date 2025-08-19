"use client";

import * as React from "react";
import { useMemo, useState } from "react";
import { PlusCircle, Download, Search } from "lucide-react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  XAxis,
  YAxis,
  PieChart,
  Pie,
  Cell,
  Legend,
  Tooltip as RechartsTooltip,
} from "recharts";
import { format, subDays, startOfMonth, endOfMonth, eachDayOfInterval } from 'date-fns';

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { IncidentForm } from "@/components/incident-form";
import { IncidentList } from "@/components/incident-list";
import type { IncidentDto, IncidentSearchDto, CreateIncidentDto, UpdateIncidentDto } from "@/lib/api-types";
import { Logo } from "@/components/icons";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import {
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
} from "@/components/ui/chart";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Label } from "@/components/ui/label";
import { 
  useIncidents, 
  useCreateIncident, 
  useUpdateIncident, 
  useDeleteIncident,
  useStatisticsSummary,
  useDamageTypesChart,
  useTroubleTypesChart,
  useMonthlyIncidentsChart
} from "@/lib/hooks";

const PIE_CHART_COLORS = [
  "#3b82f6", // blue-500
  "#ef4444", // red-500
  "#10b981", // emerald-500
  "#f59e0b", // amber-500
  "#8b5cf6", // violet-500
  "#06b6d4", // cyan-500
  "#84cc16", // lime-500
  "#f97316", // orange-500
  "#ec4899", // pink-500
  "#6366f1", // indigo-500
];

export default function Home() {
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingIncident, setEditingIncident] = useState<IncidentDto | null>(null);
  const [searchTerm, setSearchTerm] = useState("");
  const [sortConfig, setSortConfig] = useState<{ key: keyof IncidentDto; direction: 'ascending' | 'descending' } | null>({ key: 'reportedDate', direction: 'descending' });
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(10);

  // 年度フィルタ
  const thisYear = new Date().getFullYear();
  const [selectedYear, setSelectedYear] = useState<number>(thisYear);

  // API Hooks
  const incidentSearchParams = useMemo<IncidentSearchDto>(() => ({
    title: searchTerm || undefined,
    page: currentPage,
    pageSize,
    sortBy: sortConfig?.key,
    ascending: sortConfig?.direction === 'ascending'
  }), [searchTerm, currentPage, pageSize, sortConfig?.key, sortConfig?.direction]);

  const { data: incidentsData, loading: incidentsLoading, error: incidentsError, refetch: refetchIncidents } = useIncidents(incidentSearchParams);

  const { createIncident, loading: createLoading } = useCreateIncident();
  const { updateIncident, loading: updateLoading } = useUpdateIncident();
  const { deleteIncident, loading: deleteLoading } = useDeleteIncident();

  // 統計データ（年度フィルタ対応）
  const { data: statisticsData, loading: statisticsLoading } = useStatisticsSummary(selectedYear);
  const { data: damageTypesChart, loading: damageTypesLoading } = useDamageTypesChart(selectedYear);
  const { data: troubleTypesChart, loading: troubleTypesLoading } = useTroubleTypesChart(selectedYear);
  const { data: monthlyChart, loading: monthlyLoading } = useMonthlyIncidentsChart(selectedYear);

  const handleCreateIncident = async (data: CreateIncidentDto) => {
    try {
      await createIncident({
        title: data.title,
        description: data.description,
        category: data.category,
        priority: data.priority,
        reportedById: 1 // 仮のユーザーID
      });
      setIsDialogOpen(false);
      refetchIncidents();
    } catch (error) {
      console.error('インシデント作成エラー:', error);
    }
  };

  const handleUpdateIncident = async (data: UpdateIncidentDto) => {
    if (!editingIncident) return;
    
    try {
      await updateIncident(editingIncident.id, {
        title: data.title,
        description: data.description,
        category: data.category,
        priority: data.priority
      });
      setIsDialogOpen(false);
      setEditingIncident(null);
      refetchIncidents();
    } catch (error) {
      console.error('インシデント更新エラー:', error);
    }
  };

  // フォームsubmitラッパー（型整合のため）
  const handleFormSubmit = async (data: CreateIncidentDto | UpdateIncidentDto) => {
    if (editingIncident) {
      await handleUpdateIncident(data as UpdateIncidentDto);
    } else {
      await handleCreateIncident(data as CreateIncidentDto);
    }
  };

  const handleDeleteIncident = async (incident: IncidentDto) => {
    if (confirm('このインシデントを削除しますか？')) {
      try {
        await deleteIncident(incident.id);
        refetchIncidents();
      } catch (error) {
        console.error('インシデント削除エラー:', error);
      }
    }
  };

  const handleEdit = (incident: IncidentDto) => {
    setEditingIncident(incident);
    setIsDialogOpen(true);
  };

  const handleSort = (key: keyof IncidentDto) => {
    setSortConfig(current => {
      if (current?.key === key) {
        return {
          key,
          direction: current.direction === 'ascending' ? 'descending' : 'ascending'
        };
      }
      return { key, direction: 'ascending' };
    });
  };

  const handleSearch = () => {
    setCurrentPage(1);
    refetchIncidents();
  };

  const incidents = incidentsData?.items || [];
  const totalPages = incidentsData?.totalPages || 0;

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="container mx-auto px-4 py-8">
        {/* ヘッダー */}
        <div className="flex justify-between items-center mb-8">
          <div className="flex items-center gap-4">
            <Logo className="h-8 w-8" />
            <h1 className="text-3xl font-bold text-gray-900">物流トラブル管理システム</h1>
          </div>
          <Button
            onClick={() => {
              setEditingIncident(null);
              setIsDialogOpen(true);
            }}
            className="flex items-center gap-2"
          >
            <PlusCircle className="h-5 w-5" />
            新規インシデント
          </Button>
        </div>

        {/* 年度フィルタ */}
        <div className="flex items-center gap-3 mb-6">
          <Label>年度</Label>
          <Select value={String(selectedYear)} onValueChange={(v) => setSelectedYear(Number(v))}>
            <SelectTrigger className="w-[160px]">
              <SelectValue placeholder="年度を選択" />
            </SelectTrigger>
            <SelectContent>
              {[0,1,2,3,4].map(offset => {
                const y = thisYear - offset;
                return (
                  <SelectItem key={y} value={String(y)}>{y}年</SelectItem>
                );
              })}
            </SelectContent>
          </Select>
        </div>

        {/* 統計カード */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">総インシデント</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{statisticsData?.totalIncidents ?? '-'}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">未解決</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{statisticsData?.openIncidents ?? '-'}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">解決済み</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{statisticsData?.resolvedIncidents ?? '-'}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">平均解決時間</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{statisticsData?.averageResolutionTime ?? '-'}</div>
            </CardContent>
          </Card>
        </div>

        {/* グラフタブ */}
        <Card className="mb-6">
          <CardHeader>
            <CardTitle>統計グラフ（{selectedYear}年）</CardTitle>
          </CardHeader>
          <CardContent>
            <Tabs defaultValue="damage-types" className="w-full">
              <TabsList>
                <TabsTrigger value="damage-types">損傷種類</TabsTrigger>
                <TabsTrigger value="trouble-types">トラブル種類</TabsTrigger>
                <TabsTrigger value="monthly">月間発生件数</TabsTrigger>
              </TabsList>

              <TabsContent value="damage-types" className="space-y-4">
                {damageTypesLoading ? (
                  <div className="flex justify-center items-center py-8">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
                  </div>
                ) : damageTypesChart ? (
                  <div className="flex justify-center">
                    <PieChart width={400} height={300}>
                      <Pie
                        data={damageTypesChart.items}
                        cx={200}
                        cy={150}
                        labelLine={false}
                        label={({ name, percent }) => `${name} ${((percent || 0) * 100).toFixed(0)}%`}
                        outerRadius={80}
                        fill="#8884d8"
                        dataKey="value"
                      >
                        {damageTypesChart.items.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={PIE_CHART_COLORS[index % PIE_CHART_COLORS.length]} />
                        ))}
                      </Pie>
                      <RechartsTooltip />
                    </PieChart>
                  </div>
                ) : (
                  <div className="text-center py-8 text-gray-500">データがありません</div>
                )}
              </TabsContent>

              <TabsContent value="trouble-types" className="space-y-4">
                {troubleTypesLoading ? (
                  <div className="flex justify-center items-center py-8">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
                  </div>
                ) : troubleTypesChart ? (
                  <div className="flex justify-center">
                    <PieChart width={400} height={300}>
                      <Pie
                        data={troubleTypesChart.items}
                        cx={200}
                        cy={150}
                        labelLine={false}
                        label={({ name, percent }) => `${name} ${((percent || 0) * 100).toFixed(0)}%`}
                        outerRadius={80}
                        fill="#8884d8"
                        dataKey="value"
                      >
                        {troubleTypesChart.items.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={PIE_CHART_COLORS[index % PIE_CHART_COLORS.length]} />
                        ))}
                      </Pie>
                      <RechartsTooltip />
                    </PieChart>
                  </div>
                ) : (
                  <div className="text-center py-8 text-gray-500">データがありません</div>
                )}
              </TabsContent>

              <TabsContent value="monthly" className="space-y-4">
                {monthlyLoading ? (
                  <div className="flex justify-center items-center py-8">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
                  </div>
                ) : monthlyChart ? (
                  <BarChart width={800} height={300} data={monthlyChart.labels.map((label, index) => ({
                    month: label,
                    incidents: monthlyChart.series[0]?.data[index] || 0
                  }))}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="month" />
                    <YAxis />
                    <RechartsTooltip />
                    <Bar dataKey="incidents" fill="#3b82f6" />
                  </BarChart>
                ) : (
                  <div className="text-center py-8 text-gray-500">データがありません</div>
                )}
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>

        {/* 検索・フィルタ */}
        <Card className="mb-6">
          <CardHeader>
            <CardTitle>インシデント一覧</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex gap-4 mb-4">
              <div className="flex-1">
                <Label htmlFor="search">検索</Label>
                <div className="flex gap-2">
                  <Input
                    id="search"
                    placeholder="タイトルで検索..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
                  />
                  <Button onClick={handleSearch}>
                    <Search className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </div>

            <IncidentList
              incidents={incidents}
              requestSort={handleSort}
              sortConfig={sortConfig}
              onEdit={handleEdit}
              onDelete={handleDeleteIncident}
              loading={incidentsLoading}
            />

            {/* ページネーション（簡易） */}
            <div className="flex justify-between items-center mt-4">
              <div className="text-sm text-gray-500">{currentPage} / {totalPages} ページ</div>
              <div className="flex gap-2">
                <Button variant="outline" disabled={currentPage <= 1} onClick={() => setCurrentPage(p => Math.max(1, p - 1))}>前へ</Button>
                <Button variant="outline" disabled={currentPage >= totalPages} onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}>次へ</Button>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* 作成・編集ダイアログ */}
        <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>{editingIncident ? 'インシデント編集' : '新規インシデント作成'}</DialogTitle>
              <DialogDescription>
                インシデントの{editingIncident ? '内容を更新' : '内容を入力'}してください。
              </DialogDescription>
            </DialogHeader>
            <IncidentForm
              incident={editingIncident}
              onSubmit={handleFormSubmit}
              loading={createLoading || updateLoading}
            />
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}