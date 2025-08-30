import * as React from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Download, Trash2, FileText, Image, File } from "lucide-react";
import type { AttachmentDto } from "@/lib/types";

interface AttachmentListProps {
  attachments: AttachmentDto[];
  onDownload: (attachment: AttachmentDto) => void;
  onDelete: (attachment: AttachmentDto) => void;
  loading?: boolean;
}

export function AttachmentList({ attachments, onDownload, onDelete, loading = false }: AttachmentListProps) {
  const getFileIcon = (contentType: string) => {
    if (contentType.startsWith('image/')) {
      return <Image className="h-4 w-4" />;
    }
    if (contentType.includes('pdf') || contentType.includes('word') || contentType.includes('excel')) {
      return <FileText className="h-4 w-4" />;
    }
    return <File className="h-4 w-4" />;
  };

  const getFileTypeBadge = (contentType: string) => {
    if (contentType.startsWith('image/')) {
      return <Badge variant="secondary">画像</Badge>;
    }
    if (contentType.includes('pdf')) {
      return <Badge variant="secondary">PDF</Badge>;
    }
    if (contentType.includes('word')) {
      return <Badge variant="secondary">Word</Badge>;
    }
    if (contentType.includes('excel')) {
      return <Badge variant="secondary">Excel</Badge>;
    }
    return <Badge variant="secondary">その他</Badge>;
  };

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i];
  };

  const formatDate = (date: string) => {
    return new Date(date).toLocaleString('ja-JP', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center py-8">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (attachments.length === 0) {
    return (
      <div className="text-center py-8 text-gray-500">
        ファイルがありません
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {attachments.map((attachment) => (
        <Card key={attachment.id} className="hover:shadow-md transition-shadow">
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3 flex-1">
                <div className="flex items-center justify-center w-10 h-10 bg-gray-100 rounded-lg">
                  {getFileIcon(attachment.contentType)}
                </div>
                <div className="flex-1 min-w-0">
                  <div className="flex items-center space-x-2 mb-1">
                    <h3 className="text-sm font-medium text-gray-900 truncate">
                      {attachment.fileName}
                    </h3>
                    {getFileTypeBadge(attachment.contentType)}
                  </div>
                  <div className="text-xs text-gray-500 space-y-1">
                    <div>サイズ: {formatFileSize(attachment.fileSize)}</div>
                    <div>アップロード: {formatDate(attachment.uploadedAt)}</div>
                    <div>アップロード者: {attachment.uploadedBy}</div>
                    <div>インシデント: {attachment.incidentTitle}</div>
                  </div>
                </div>
              </div>
              <div className="flex items-center space-x-2">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => onDownload(attachment)}
                  className="flex items-center space-x-1"
                >
                  <Download className="h-4 w-4" />
                  <span>ダウンロード</span>
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => onDelete(attachment)}
                  className="flex items-center space-x-1 text-red-600 hover:text-red-700"
                >
                  <Trash2 className="h-4 w-4" />
                  <span>削除</span>
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
