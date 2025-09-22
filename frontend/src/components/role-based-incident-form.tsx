import * as React from "react";
import { useAuth } from "@/contexts/AuthContext";
import { ClerkIncidentForm } from "./clerk-incident-form";
import { IncidentForm } from "./incident-form";
import type { Incident, CreateIncidentDto, UpdateIncidentDto } from "@/lib/types";

interface RoleBasedIncidentFormProps {
  incident?: Incident | null;
  onSubmit: (data: CreateIncidentDto | UpdateIncidentDto) => void;
  onCancel?: () => void;
  loading?: boolean;
  hideButtons?: boolean;
}

/**
 * ロールに基づいてインシデントフォームを切り替えるコンポーネント
 * 
 * 仕様書2.3新しいワークフローに準拠：
 * - 事務員: 簡易登録フォーム（タイトル、詳細説明、発生経緯、発生日、発生場所のみ）
 * - その他のロール: 従来の詳細フォーム
 */
export function RoleBasedIncidentForm({ 
  incident, 
  onSubmit, 
  onCancel, 
  loading = false, 
  hideButtons = false 
}: RoleBasedIncidentFormProps) {
  const { user } = useAuth();

  // ユーザー情報が取得できない場合はローディング表示
  if (!user) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-logistics-blue mx-auto mb-4"></div>
          <p className="text-gray-600">認証情報を確認中...</p>
        </div>
      </div>
    );
  }

  // 事務員の場合
  if (user.roleId === 4) {
    // 新規作成時は簡易登録フォーム
    if (!incident) {
      return (
        <ClerkIncidentForm
          onSubmit={onSubmit}
          onCancel={onCancel}
          loading={loading}
          hideButtons={hideButtons}
        />
      );
    }
    
    // 編集時：自分が報告したインシデントのみ編集可能
    if (incident.reportedById === user.id) {
      return (
        <ClerkIncidentForm
          incident={incident}
          isEditMode={true}
          onSubmit={onSubmit}
          onCancel={onCancel}
          loading={loading}
          hideButtons={hideButtons}
        />
      );
    }
    
    // 他人のインシデントは表示のみ（編集不可）
    return (
      <IncidentForm
        incident={incident}
        onSubmit={onSubmit}
        onCancel={onCancel}
        loading={loading}
        hideButtons={true} // 編集ボタンを非表示
      />
    );
  }

  // その他のロールまたは編集時は従来のフォームを使用
  return (
    <IncidentForm
      incident={incident}
      onSubmit={onSubmit}
      onCancel={onCancel}
      loading={loading}
      hideButtons={hideButtons}
    />
  );
}
