import * as React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Search, Filter, X } from "lucide-react";
import type { AttachmentSearchDto } from "@/lib/types";

interface AttachmentSearchProps {
  searchParams: AttachmentSearchDto;
  onSearchChange: (params: AttachmentSearchDto) => void;
  onClear: () => void;
}

export function AttachmentSearch({ searchParams, onSearchChange, onClear }: AttachmentSearchProps) {
  const [isExpanded, setIsExpanded] = React.useState(false);

  const handleInputChange = (field: keyof AttachmentSearchDto, value: string | number | undefined) => {
    onSearchChange({
      ...searchParams,
      [field]: value,
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
            placeholder="ファイル名で検索..."
            value={searchParams.fileName || ''}
            onChange={(e) => handleInputChange('fileName', e.target.value)}
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
        {(searchParams.fileName || searchParams.incidentId || searchParams.contentType) && (
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
            <Label htmlFor="incident-id">インシデントID</Label>
            <Input
              id="incident-id"
              type="number"
              placeholder="インシデントID"
              value={searchParams.incidentId || ''}
              onChange={(e) => handleInputChange('incidentId', e.target.value ? parseInt(e.target.value) : undefined)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="content-type">ファイル形式</Label>
            <Select
              value={searchParams.contentType || ''}
              onValueChange={(value) => handleInputChange('contentType', value)}
            >
              <SelectTrigger>
                <SelectValue placeholder="すべて" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">すべて</SelectItem>
                <SelectItem value="image/">画像</SelectItem>
                <SelectItem value="application/pdf">PDF</SelectItem>
                <SelectItem value="application/msword">Word</SelectItem>
                <SelectItem value="application/vnd.ms-excel">Excel</SelectItem>
                <SelectItem value="text/">テキスト</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="uploaded-from">アップロード日（開始）</Label>
            <Input
              id="uploaded-from"
              type="date"
              value={searchParams.uploadedFrom ? new Date(searchParams.uploadedFrom).toISOString().split('T')[0] : ''}
              onChange={(e) => handleInputChange('uploadedFrom', e.target.value || undefined)}
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="uploaded-to">アップロード日（終了）</Label>
            <Input
              id="uploaded-to"
              type="date"
              value={searchParams.uploadedTo ? new Date(searchParams.uploadedTo).toISOString().split('T')[0] : ''}
              onChange={(e) => handleInputChange('uploadedTo', e.target.value || undefined)}
            />
          </div>
        </div>
      )}
    </div>
  );
}
