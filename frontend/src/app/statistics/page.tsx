'use client';

import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';
import { FilterControls } from '@/components/filter-controls';
import { useFilter } from '@/contexts/FilterContext';
import { 
  useStatisticsSummary,
  useDamageTypesChart,
  useTroubleTypesChart,
  useMonthlyIncidentsChart
} from '@/lib/hooks';

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

export default function StatisticsPage() {
  // 共通フィルタ状態
  const { selectedYear, selectedMonth } = useFilter();

  // 統計データ（年度・月フィルタ対応）
  const { data: statisticsData, loading: statisticsLoading } = useStatisticsSummary(selectedYear, selectedMonth > 0 ? selectedMonth : undefined);
  const { data: damageTypesChart, loading: damageTypesLoading } = useDamageTypesChart(selectedYear, selectedMonth > 0 ? selectedMonth : undefined);
  const { data: troubleTypesChart, loading: troubleTypesLoading } = useTroubleTypesChart(selectedYear, selectedMonth > 0 ? selectedMonth : undefined);
  const { data: monthlyChart, loading: monthlyLoading } = useMonthlyIncidentsChart(selectedYear, selectedMonth > 0 ? selectedMonth : undefined);

  return (
    <div className="container mx-auto p-6">
      {/* 年度・月フィルタ */}
      <FilterControls />

      <div className="mb-6">
        <h1 className="text-3xl font-bold">統計・分析</h1>
        <p className="text-gray-600">インシデントの統計情報と分析結果を表示します</p>
      </div>

      <div className="grid gap-6">
        {/* サマリー統計 */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
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
              <div className="text-2xl font-bold">{statisticsData?.openCount ?? '-'}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">解決済み</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{(statisticsData?.resolvedCount ?? 0) + (statisticsData?.closedCount ?? 0)}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">平均解決時間</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{statisticsData?.averageResolutionTime ?? '-'}日</div>
            </CardContent>
          </Card>
        </div>

        {/* グラフ */}
        <Card>
          <CardHeader>
            <CardTitle>
              統計グラフ（{selectedYear}年{selectedMonth > 0 ? `${selectedMonth}月` : ''}）
            </CardTitle>
          </CardHeader>
          <CardContent>
            <Tabs defaultValue="damage-types" className="w-full">
              <TabsList className="grid w-full grid-cols-3">
                <TabsTrigger value="damage-types">損傷種類</TabsTrigger>
                <TabsTrigger value="trouble-types">トラブル種類</TabsTrigger>
                <TabsTrigger value="monthly">{selectedMonth > 0 ? '日別発生件数' : '月間発生件数'}</TabsTrigger>
              </TabsList>
              
              <TabsContent value="damage-types" className="space-y-4">
                {damageTypesLoading ? (
                  <div className="flex justify-center items-center py-8">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
                  </div>
                ) : damageTypesChart ? (
                  <div className="h-[400px]">
                    <ResponsiveContainer width="100%" height="100%">
                      <PieChart>
                        <Pie
                          data={damageTypesChart.items}
                          cx="50%"
                          cy="50%"
                          labelLine={false}
                          label={({ label, percent }) => `${label} ${((percent || 0) * 100).toFixed(0)}%`}
                          outerRadius={80}
                          fill="#8884d8"
                          dataKey="value"
                        >
                          {damageTypesChart.items?.map((entry, index) => (
                            <Cell key={`cell-${index}`} fill={PIE_CHART_COLORS[index % PIE_CHART_COLORS.length]} />
                          ))}
                        </Pie>
                        <Tooltip />
                      </PieChart>
                    </ResponsiveContainer>
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
                  <div className="h-[400px]">
                    <ResponsiveContainer width="100%" height="100%">
                      <PieChart>
                        <Pie
                          data={troubleTypesChart.items}
                          cx="50%"
                          cy="50%"
                          labelLine={false}
                          label={({ label, percent }) => `${label} ${((percent || 0) * 100).toFixed(0)}%`}
                          outerRadius={80}
                          fill="#8884d8"
                          dataKey="value"
                        >
                          {troubleTypesChart.items?.map((entry, index) => (
                            <Cell key={`cell-${index}`} fill={PIE_CHART_COLORS[index % PIE_CHART_COLORS.length]} />
                          ))}
                        </Pie>
                        <Tooltip />
                      </PieChart>
                    </ResponsiveContainer>
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
                  <div className="h-[400px]">
                    <ResponsiveContainer width="100%" height="100%">
                      <BarChart data={monthlyChart.labels?.map((label, index) => ({
                        period: label,
                        incidents: monthlyChart.series[0]?.data[index] || 0
                      })) || []}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="period" />
                        <YAxis />
                        <Tooltip />
                        <Legend />
                        <Bar dataKey="incidents" fill="#3b82f6" />
                      </BarChart>
                    </ResponsiveContainer>
                  </div>
                ) : (
                  <div className="text-center py-8 text-gray-500">データがありません</div>
                )}
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
