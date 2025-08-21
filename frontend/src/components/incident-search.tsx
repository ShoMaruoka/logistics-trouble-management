import * as React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Search, Filter, X } from "lucide-react";
import type { IncidentSearchDto } from "@/lib/api-types";

interface IncidentSearchProps {
  searchParams: IncidentSearchDto;
  onSearchChange: (params: IncidentSearchDto) => void;
  onClear: () => void;
}

export function IncidentSearch({ searchParams, onSearchChange, onClear }: IncidentSearchProps) {
  const [isExpanded, setIsExpanded] = React.useState(false);

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
        {(searchParams.searchTerm || searchParams.status || searchParams.priority || searchParams.category) && (
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
        </div>
      )}
    </div>
  );
}
