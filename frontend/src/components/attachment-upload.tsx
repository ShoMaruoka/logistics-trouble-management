import * as React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Upload, X, FileText, Image, File } from "lucide-react";
import type { CreateAttachmentDto } from "@/lib/api-types";

interface AttachmentUploadProps {
  incidentId?: number;
  onUpload: (data: CreateAttachmentDto) => void;
  onCancel: () => void;
  loading?: boolean;
}

interface FileInfo {
  file: File;
  preview?: string;
}

export function AttachmentUpload({ incidentId, onUpload, onCancel, loading = false }: AttachmentUploadProps) {
  const [selectedFile, setSelectedFile] = React.useState<FileInfo | null>(null);
  const [incidentIdInput, setIncidentIdInput] = React.useState<string>(incidentId?.toString() || '');

  const handleFileSelect = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      // ファイルサイズチェック（10MB制限）
      if (file.size > 10 * 1024 * 1024) {
        alert('ファイルサイズは10MB以下にしてください。');
        return;
      }

      // ファイル形式チェック
      const allowedTypes = [
        'image/jpeg', 'image/png', 'image/gif', 'image/bmp',
        'application/pdf',
        'application/msword',
        'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
        'application/vnd.ms-excel',
        'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        'text/plain',
        'text/csv'
      ];

      if (!allowedTypes.includes(file.type)) {
        alert('対応していないファイル形式です。');
        return;
      }

      const fileInfo: FileInfo = { file };

      // 画像ファイルの場合はプレビューを生成
      if (file.type.startsWith('image/')) {
        const reader = new FileReader();
        reader.onload = (e) => {
          fileInfo.preview = e.target?.result as string;
          setSelectedFile(fileInfo);
        };
        reader.readAsDataURL(file);
      } else {
        setSelectedFile(fileInfo);
      }
    }
  };

  const handleUpload = () => {
    if (!selectedFile || !incidentIdInput) {
      alert('ファイルとインシデントIDを選択してください。');
      return;
    }

    const uploadData: CreateAttachmentDto = {
      incidentId: parseInt(incidentIdInput),
      fileName: selectedFile.file.name,
      contentType: selectedFile.file.type,
      fileSize: selectedFile.file.size,
      uploadedById: 1 // 仮のユーザーID
    };

    onUpload(uploadData);
  };

  const handleCancel = () => {
    setSelectedFile(null);
    onCancel();
  };

  const getFileIcon = (contentType: string) => {
    if (contentType.startsWith('image/')) {
      return <Image className="h-8 w-8" />;
    }
    if (contentType.includes('pdf') || contentType.includes('word') || contentType.includes('excel')) {
      return <FileText className="h-8 w-8" />;
    }
    return <File className="h-8 w-8" />;
  };

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i];
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <Upload className="h-5 w-5" />
          ファイルアップロード
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {/* インシデントID選択 */}
        <div className="space-y-2">
          <Label htmlFor="incident-id">インシデントID *</Label>
          <Input
            id="incident-id"
            type="number"
            value={incidentIdInput}
            onChange={(e) => setIncidentIdInput(e.target.value)}
            placeholder="インシデントIDを入力"
            disabled={!!incidentId}
          />
        </div>

        {/* ファイル選択 */}
        <div className="space-y-2">
          <Label htmlFor="file-upload">ファイル選択 *</Label>
          <div className="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center hover:border-gray-400 transition-colors">
            <input
              id="file-upload"
              type="file"
              onChange={handleFileSelect}
              className="hidden"
              accept="image/*,.pdf,.doc,.docx,.xls,.xlsx,.txt,.csv"
            />
            <label htmlFor="file-upload" className="cursor-pointer">
              <Upload className="h-8 w-8 mx-auto text-gray-400 mb-2" />
              <p className="text-sm text-gray-600">
                クリックしてファイルを選択、またはドラッグ&ドロップ
              </p>
              <p className="text-xs text-gray-500 mt-1">
                対応形式: 画像, PDF, Word, Excel, テキスト (最大10MB)
              </p>
            </label>
          </div>
        </div>

        {/* 選択されたファイルの表示 */}
        {selectedFile && (
          <div className="border rounded-lg p-4 bg-gray-50">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                {selectedFile.preview ? (
                  <img
                    src={selectedFile.preview}
                    alt="Preview"
                    className="w-12 h-12 object-cover rounded"
                  />
                ) : (
                  <div className="w-12 h-12 bg-gray-200 rounded flex items-center justify-center">
                    {getFileIcon(selectedFile.file.type)}
                  </div>
                )}
                <div>
                  <p className="text-sm font-medium">{selectedFile.file.name}</p>
                  <p className="text-xs text-gray-500">
                    {formatFileSize(selectedFile.file.size)} • {selectedFile.file.type}
                  </p>
                </div>
              </div>
              <Button
                variant="ghost"
                size="sm"
                onClick={() => setSelectedFile(null)}
                className="text-gray-400 hover:text-gray-600"
              >
                <X className="h-4 w-4" />
              </Button>
            </div>
          </div>
        )}

        {/* アクションボタン */}
        <div className="flex justify-end space-x-2 pt-4">
          <Button variant="outline" onClick={handleCancel} disabled={loading}>
            キャンセル
          </Button>
          <Button
            onClick={handleUpload}
            disabled={!selectedFile || !incidentIdInput || loading}
            className="flex items-center gap-2"
          >
            {loading ? (
              <>
                <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                アップロード中...
              </>
            ) : (
              <>
                <Upload className="h-4 w-4" />
                アップロード
              </>
            )}
          </Button>
        </div>
      </CardContent>
    </Card>
  );
}
