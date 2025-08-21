"use client";

import * as React from "react";
import { useMemo, useState } from "react";
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
  ResponsiveContainer,
} from "recharts";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Label } from "@/components/ui/label";
import { 
  useStatisticsSummary,
  useDamageTypesChart,
  useTroubleTypesChart,
  useMonthlyIncidentsChart
} from "@/lib/hooks";

// 画像のデザインに合わせたカラーパレット
const PIE_CHART_COLORS = [
  "#ff6b6b", // 破損・汚損 (red-orange)
  "#4ecdc4", // その他の配送ミス (dark teal)
  "#45b7d1", // 誤出荷 (dark blue)
  "#f7b731", // 早着・延着 (light orange)
  "#fdcb6e", // 紛失 (yellow)
  "#00b894", // 誤配送 (green)
  "#6c5ce7", // その他の商品事故 (light green)
];

export function Dashboard() {
  // 年度・月フィルタ
  const thisYear = new Date().getFullYear();
  const thisMonth = new Date().getMonth() + 1;
  const [selectedYear, setSelectedYear] = useState<number>(thisYear);
  const [selectedMonth, setSelectedMonth] = useState<number>(thisMonth); // 当月を初期値に変更

  // 統計データ（年度・月フィルタ対応）
  const { data: statisticsData, loading: statisticsLoading } = useStatisticsSummary(selectedYear, selectedMonth > 0 ? selectedMonth : undefined);
  const { data: damageTypesChart, loading: damageTypesLoading } = useDamageTypesChart(selectedYear, selectedMonth > 0 ? selectedMonth : undefined);
  const { data: troubleTypesChart, loading: troubleTypesLoading } = useTroubleTypesChart(selectedYear, selectedMonth > 0 ? selectedMonth : undefined);
  const { data: monthlyChart, loading: monthlyLoading } = useMonthlyIncidentsChart(selectedYear, selectedMonth > 0 ? selectedMonth : undefined);

  return (
    <>
      {/* 年度・月フィルタ */}
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
        
        <Label>月</Label>
        <Select value={String(selectedMonth)} onValueChange={(v) => setSelectedMonth(Number(v))}>
          <SelectTrigger className="w-[160px]">
            <SelectValue placeholder="月を選択" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="0">年間表示</SelectItem>
            {[1,2,3,4,5,6,7,8,9,10,11,12].map(m => (
              <SelectItem key={m} value={String(m)}>{m}月</SelectItem>
            ))}
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
            <div className="text-2xl font-bold text-logistics-blue">{statisticsData?.totalIncidents ?? '-'}</div>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">未解決</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-logistics-red">{statisticsData?.openCount ?? '-'}</div>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">解決済み</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-logistics-green">{(statisticsData?.resolvedCount ?? 0) + (statisticsData?.closedCount ?? 0)}</div>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">平均解決時間</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-logistics-orange">{statisticsData?.averageResolutionTime ?? '-'}</div>
          </CardContent>
        </Card>
      </div>

      {/* グラフタブ */}
      <Card className="mb-6">
        <CardHeader>
          <CardTitle className="text-xl font-bold">
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
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-logistics-blue"></div>
                </div>
              ) : damageTypesChart ? (
                <div className="flex justify-center">
                  <PieChart width={400} height={300}>
                    <Pie
                      data={damageTypesChart.items}
                      cx={200}
                      cy={150}
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
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-logistics-blue"></div>
                </div>
              ) : troubleTypesChart ? (
                <div className="flex justify-center">
                  <PieChart width={400} height={300}>
                    <Pie
                      data={troubleTypesChart.items}
                      cx={200}
                      cy={150}
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
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-logistics-blue"></div>
                </div>
              ) : monthlyChart ? (
                <div className="w-full h-[300px]">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={monthlyChart.labels?.map((label, index) => ({
                      month: label,
                      incidents: monthlyChart.series[0]?.data[index] || 0
                    })) || []}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="month" />
                      <YAxis />
                      <RechartsTooltip />
                      <Bar dataKey="incidents" fill="var(--logistics-blue)" />
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
    </>
  );
}
