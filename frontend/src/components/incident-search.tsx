import * as React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Search, Filter, X } from "lucide-react";
import type { IncidentSearchDto } from "@/lib/types";
import { useMasterData } from "@/hooks/useMasterData";

interface IncidentSearchProps {
  searchParams: IncidentSearchDto;
  onSearchChange: (params: IncidentSearchDto) => void;
  onClear: () => void;
}

export function IncidentSearch({ searchParams, onSearchChange, onClear }: IncidentSearchProps) {
  const [isExpanded, setIsExpanded] = React.useState(false);
  const { troubleTypes, damageTypes, warehouses, shippingCompanies, loading: masterDataLoading, error: masterDataError, refetch: refetchMasterData } = useMasterData();

  const handleInputChange = (field: keyof IncidentSearchDto, value: string) => {
    onSearchChange({
      ...searchParams,
      [field]: value || undefined,
      page: 1, // 検索時は1ページ目に戻す
    });
  };

  const handleClear = () => {
    onClear();
    setIsExpanded(false);
  };

  const handleRetry = () => {
    refetchMasterData();
  };

  if (masterDataLoading) {
    return (
      <div className="bg-white p-4 rounded-lg border shadow-sm">
        <div className="flex justify-center items-center py-8">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-logistics-blue"></div>
          <span className="ml-2">マスタデータを読み込み中...</span>
        </div>
      </div>
    );
  }

  if (masterDataError) {
    return (
      <div className="bg-white p-4 rounded-lg border shadow-sm">
        <div className="flex flex-col items-center justify-center py-8 space-y-4">
          <div className="flex items-center text-red-600">
            <svg className="w-6 h-6 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
            </svg>
            <span className="font-medium">マスタデータの読み込みに失敗しました</span>
          </div>
          <p className="text-gray-600 text-center max-w-md">
            マスタデータの取得中にエラーが発生しました。しばらく時間をおいてから再度お試しください。
          </p>
          <Button
            onClick={handleRetry}
            variant="outline"
            className="flex items-center gap-2"
          >
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
            </svg>
            再試行
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white p-4 rounded-lg border shadow-sm">
      {/* 基本検索 */}
      <div className="flex gap-2 mb-4">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
          <Input
            placeholder="タイトルで検索..."
            value={searchParams.searchTerm || ''}
            onChange={(e) => handleInputChange('searchTerm', e.target.value)}
            className="pl-10"
          />
        </div>
        <Button
          variant="outline"
          onClick={() => setIsExpanded(!isExpanded)}
          className="flex items-center gap-2"
        >
          <Filter className="h-4 w-4" />
          詳細検索
        </Button>
        {(searchParams.searchTerm || searchParams.status || searchParams.priority || searchParams.category || 
          searchParams.troubleTypeId || searchParams.damageTypeId || searchParams.warehouseId || searchParams.shippingCompanyId ||
          searchParams.fromDate || searchParams.toDate) && (
          <Button
            variant="outline"
            onClick={handleClear}
            className="flex items-center gap-2"
          >
            <X className="h-4 w-4" />
            クリア
          </Button>
        )}
      </div>

      {/* 詳細検索 */}
      {isExpanded && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 pt-4 border-t">
          <div className="space-y-2">
            <Label htmlFor="status-filter">ステータス</Label>
            <Select
              value={searchParams.status || ''}
              onValueChange={(value) => handleInputChange('status', value)}
            >
              <SelectTrigger>
                <SelectValue placeholder="すべて" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">すべて</SelectItem>
                <SelectItem value="Open">未対応</SelectItem>
                <SelectItem value="InProgress">対応中</SelectItem>
                <SelectItem value="Resolved">解決済み</SelectItem>
                <SelectItem value="Closed">完了</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="priority-filter">優先度</Label>
            <Select
              value={searchParams.priority || ''}
              onValueChange={(value) => handleInputChange('priority', value)}
            >
              <SelectTrigger>
                <SelectValue placeholder="すべて" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">すべて</SelectItem>
                <SelectItem value="Low">低</SelectItem>
                <SelectItem value="Medium">中</SelectItem>
                <SelectItem value="High">高</SelectItem>
                <SelectItem value="Critical">緊急</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="category-filter">カテゴリ</Label>
            <Input
              id="category-filter"
              placeholder="カテゴリで検索"
              value={searchParams.category || ''}
              onChange={(e) => handleInputChange('category', e.target.value)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="reported-from">報告日（開始）</Label>
            <Input
              id="reported-from"
              type="date"
              value={searchParams.fromDate || ''}
              onChange={(e) => handleInputChange('fromDate', e.target.value)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="reported-to">報告日（終了）</Label>
            <Input
              id="reported-to"
              type="date"
              value={searchParams.toDate || ''}
              onChange={(e) => handleInputChange('toDate', e.target.value)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="trouble-type-filter">トラブル種類</Label>
            <Select
              value={searchParams.troubleTypeId?.toString() || ''}
              onValueChange={(value) => handleInputChange('troubleTypeId', value)}
            >
              <SelectTrigger>
                <SelectValue placeholder="すべて" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">すべて</SelectItem>
                {troubleTypes.map((troubleType) => (
                  <SelectItem key={troubleType.id} value={troubleType.id.toString()}>
                    <div className="flex items-center gap-2">
                      <div
                        className="w-3 h-3 rounded-full"
                        style={{ backgroundColor: troubleType.color }}
                      />
                      {troubleType.name}
                    </div>
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="damage-type-filter">損傷の種類</Label>
            <Select
              value={searchParams.damageTypeId?.toString() || ''}
              onValueChange={(value) => handleInputChange('damageTypeId', value)}
            >
              <SelectTrigger>
                <SelectValue placeholder="すべて" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">すべて</SelectItem>
                {damageTypes.map((damageType) => (
                  <SelectItem key={damageType.id} value={damageType.id.toString()}>
                    {damageType.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="warehouse-filter">出荷元倉庫</Label>
            <Select
              value={searchParams.warehouseId?.toString() || ''}
              onValueChange={(value) => handleInputChange('warehouseId', value)}
            >
              <SelectTrigger>
                <SelectValue placeholder="すべて" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">すべて</SelectItem>
                {warehouses.map((warehouse) => (
                  <SelectItem key={warehouse.id} value={warehouse.id.toString()}>
                    {warehouse.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="shipping-company-filter">運送会社</Label>
            <Select
              value={searchParams.shippingCompanyId?.toString() || ''}
              onValueChange={(value) => handleInputChange('shippingCompanyId', value)}
            >
              <SelectTrigger>
                <SelectValue placeholder="すべて" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">すべて</SelectItem>
                {shippingCompanies.map((shippingCompany) => (
                  <SelectItem key={shippingCompany.id} value={shippingCompany.id.toString()}>
                    {shippingCompany.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>
      )}
    </div>
  );
}
