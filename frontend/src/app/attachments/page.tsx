'use client';

import * as React from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Plus, AlertCircle } from "lucide-react";
import { AttachmentList } from "@/components/attachment-list";
import { AttachmentSearch } from "@/components/attachment-search";
import { AttachmentUpload } from "@/components/attachment-upload";
import { Pagination } from "@/components/pagination";
import {
  useAttachments,
  useCreateAttachment,
  useDeleteAttachment,
  useDownloadAttachment
} from "@/lib/hooks";
import type {
  AttachmentDto,
  AttachmentSearchDto,
  CreateAttachmentDto
} from "@/lib/types";

export default function AttachmentsPage() {
  // 状態管理
  const [searchParams, setSearchParams] = React.useState<AttachmentSearchDto>({
    page: 1,
    pageSize: 10,
    sortBy: 'UploadedAt',
    ascending: false,
  });

  const [showUpload, setShowUpload] = React.useState(false);

  // API Hooks
  const { data: attachmentsData, loading: attachmentsLoading, error: attachmentsError, refetch: refetchAttachments } = useAttachments(searchParams);
  const { createAttachment, loading: createLoading } = useCreateAttachment();
  const { deleteAttachment, loading: deleteLoading } = useDeleteAttachment();
  const { downloadAttachment, loading: downloadLoading } = useDownloadAttachment();

  // 検索処理
  const handleSearchChange = (params: AttachmentSearchDto) => {
    setSearchParams(prev => ({ ...prev, ...params }));
  };

  const handleSearchClear = () => {
    setSearchParams({
      page: 1,
      pageSize: 10,
      sortBy: 'UploadedAt',
      ascending: false,
    });
  };

  // ページネーション処理
  const handlePageChange = (page: number) => {
    setSearchParams(prev => ({ ...prev, page }));
  };

  // ファイル操作
  const handleDownload = async (attachment: AttachmentDto) => {
    try {
      await downloadAttachment(attachment.id, attachment.fileName);
    } catch (error) {
      console.error('ダウンロードエラー:', error);
      alert('ダウンロードに失敗しました');
    }
  };

  const handleDelete = async (attachment: AttachmentDto) => {
    if (window.confirm(`ファイル「${attachment.fileName}」を削除しますか？`)) {
      try {
        await deleteAttachment(attachment.id);
        refetchAttachments();
      } catch (error) {
        console.error('削除エラー:', error);
        alert('削除に失敗しました');
      }
    }
  };

  const handleUpload = async (data: CreateAttachmentDto) => {
    try {
      await createAttachment(data);
      setShowUpload(false);
      refetchAttachments();
    } catch (error) {
      console.error('アップロードエラー:', error);
      alert('アップロードに失敗しました');
    }
  };

  const handleUploadCancel = () => {
    setShowUpload(false);
  };

  return (
    <div className="container mx-auto p-6 space-y-6">
      {/* ヘッダー */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">ファイル管理</h1>
          <p className="text-gray-600 mt-2">インシデント関連ファイルの管理を行います</p>
        </div>
        <Button onClick={() => setShowUpload(true)} className="flex items-center gap-2">
          <Plus className="h-4 w-4" />
          ファイルアップロード
        </Button>
      </div>

      {/* アップロードフォーム */}
      {showUpload && (
        <AttachmentUpload
          onUpload={handleUpload}
          onCancel={handleUploadCancel}
          loading={createLoading}
        />
      )}

      {/* 検索・フィルタリング */}
      <AttachmentSearch
        searchParams={searchParams}
        onSearchChange={handleSearchChange}
        onClear={handleSearchClear}
      />

      {/* エラー表示 */}
      {attachmentsError && (
        <Card className="border-red-200 bg-red-50">
          <CardContent className="pt-6">
            <div className="flex items-center gap-2 text-red-600">
              <AlertCircle className="h-5 w-5" />
              <span>エラー: {attachmentsError}</span>
            </div>
          </CardContent>
        </Card>
      )}

      {/* ファイル一覧 */}
      <Card>
        <CardHeader>
          <CardTitle>ファイル一覧</CardTitle>
        </CardHeader>
        <CardContent>
          <AttachmentList
            attachments={attachmentsData?.items || []}
            onDownload={handleDownload}
            onDelete={handleDelete}
            loading={attachmentsLoading || deleteLoading || downloadLoading}
          />
        </CardContent>
      </Card>

      {/* ページネーション */}
      {attachmentsData && attachmentsData.totalPages > 1 && (
        <div className="flex justify-center">
          <Pagination
            currentPage={attachmentsData.pageNumber}
            totalPages={attachmentsData.totalPages}
            onPageChange={handlePageChange}
          />
        </div>
      )}
    </div>
  );
}
