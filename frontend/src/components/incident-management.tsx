"use client";

import * as React from "react";
import { useMemo, useState } from "react";
import { Search, Download } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { IncidentList } from "@/components/incident-list";
import type { IncidentDto, IncidentSearchDto } from "@/lib/api-types";
import { useIncidents } from "@/lib/hooks";

interface IncidentManagementProps {
  onEdit: (incident: IncidentDto) => void;
  onDelete: (incident: IncidentDto) => void;
}

export function IncidentManagement({ onEdit, onDelete }: IncidentManagementProps) {
  const [searchTerm, setSearchTerm] = useState("");
  const [sortConfig, setSortConfig] = useState<{ key: keyof IncidentDto; direction: 'ascending' | 'descending' } | null>({ key: 'occurrenceDate', direction: 'descending' });
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(10);

  // API Hooks
  const incidentSearchParams = useMemo<IncidentSearchDto>(() => ({
    searchTerm: searchTerm || undefined,
    page: currentPage,
    pageSize,
    sortBy: sortConfig?.key,
    ascending: sortConfig?.direction === 'ascending'
  }), [searchTerm, currentPage, pageSize, sortConfig?.key, sortConfig?.direction]);

  const { data: incidentsData, loading: incidentsLoading, error: incidentsError, refetch: refetchIncidents } = useIncidents(incidentSearchParams);

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

  const handleCSVExport = () => {
    // CSV出力機能（将来的に実装）
    console.log('CSV出力機能');
  };

  const incidents = incidentsData?.items || [];
  const totalPages = incidentsData?.totalPages || 0;

  return (
    <Card className="mb-6">
      <CardHeader>
        <CardTitle className="text-xl font-bold">物流トラブル一覧</CardTitle>
      </CardHeader>
      <CardContent>
        {/* 検索とCSV出力 */}
        <div className="flex gap-4 mb-6">
          <div className="flex-1">
            <Label htmlFor="search" className="sr-only">物流トラブルを検索</Label>
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
              <Input
                id="search"
                placeholder="物流トラブルを検索..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
                className="pl-10"
              />
            </div>
          </div>
          <Button 
            onClick={handleCSVExport}
            variant="outline"
            className="flex items-center gap-2"
          >
            <Download className="h-4 w-4" />
            CSV出力
          </Button>
        </div>

        <IncidentList 
          incidents={incidents}
          requestSort={handleSort}
          sortConfig={sortConfig}
          onEdit={onEdit}
          onDelete={onDelete}
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
  );
}
