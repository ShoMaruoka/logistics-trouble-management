import * as React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useAuth } from "@/contexts/AuthContext";
import type { CreateIncidentDto, Incident } from "@/lib/types";

interface ClerkIncidentFormProps {
  onSubmit: (data: CreateIncidentDto) => void;
  onCancel?: () => void;
  loading?: boolean;
  hideButtons?: boolean;
  incident?: Incident; // 編集時の既存データ
  isEditMode?: boolean; // 編集モードフラグ
}

/**
 * 事務員専用の簡易インシデント登録フォーム
 * 仕様書2.3新しいワークフローに準拠：
 * - 事務員は「タイトル、詳細説明、発生経緯、発生日、発生場所」のみ入力
 * - 遷移後ステータス: 未分類
 * - 分類作業はインシデント管理者が後で実施
 */
export function ClerkIncidentForm({ 
  onSubmit, 
  onCancel, 
  loading = false, 
  hideButtons = false, 
  incident, 
  isEditMode = false 
}: ClerkIncidentFormProps) {
  const { user } = useAuth();
  
  const [formData, setFormData] = React.useState({
    title: incident?.title || '',
    description: incident?.description || '',
    incidentDetails: incident?.incidentDetails || '', // 発生経緯
    occurrenceDate: incident?.occurrenceDate ? new Date(incident.occurrenceDate).toISOString().split('T')[0] : '', // 発生日
    occurrenceLocation: incident?.occurrenceLocation || '', // 発生場所
  });

  const [errors, setErrors] = React.useState<Record<string, string>>({});

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.title.trim()) {
      newErrors.title = 'タイトルは必須です';
    }

    if (!formData.description.trim()) {
      newErrors.description = '詳細説明は必須です';
    }

    if (!formData.incidentDetails.trim()) {
      newErrors.incidentDetails = '発生経緯は必須です';
    }

    if (!formData.occurrenceDate) {
      newErrors.occurrenceDate = '発生日は必須です';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    // 事務員用の最小限データを構築
    const clerkData = {
      title: formData.title.trim(),
      description: formData.description.trim(),
      incidentDetails: formData.incidentDetails.trim(),
      occurrenceDate: formData.occurrenceDate,
      occurrenceLocation: formData.occurrenceLocation.trim(),
      reportedById: user?.id || 1,
    };

    // 編集モードの場合はインシデントIDを追加
    if (isEditMode && incident) {
      (clerkData as any).id = incident.id;
    }

    // 事務員専用の送信処理
    onSubmit(clerkData as any);
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6 bg-white p-6 rounded-lg shadow-sm">
      {/* ヘッダー */}
      <div className="border-b pb-4">
        <h2 className="text-xl font-semibold text-gray-900">
          {isEditMode ? '物流トラブル編集' : '物流トラブル新規登録'}
        </h2>
        <p className="text-sm text-gray-600 mt-1">
          {isEditMode 
            ? '基本情報を修正してください。分類情報の変更はインシデント管理者にお問い合わせください。'
            : '基本情報を入力してください。詳細な分類は後でインシデント管理者が行います。'
          }
        </p>
      </div>

      {/* タイトル */}
      <div className="space-y-2">
        <Label htmlFor="title" className="text-sm font-medium text-gray-700">
          タイトル <span className="text-red-500">*</span>
        </Label>
        <Input
          id="title"
          value={formData.title}
          onChange={(e) => setFormData(prev => ({ ...prev, title: e.target.value }))}
          placeholder="トラブルのタイトルを入力してください"
          className={`border-gray-300 focus:ring-2 focus:ring-logistics-blue ${
            errors.title ? 'border-red-500' : ''
          }`}
        />
        {errors.title && <p className="text-sm text-red-600">{errors.title}</p>}
      </div>

      {/* 詳細説明 */}
      <div className="space-y-2">
        <Label htmlFor="description" className="text-sm font-medium text-gray-700">
          詳細説明 <span className="text-red-500">*</span>
        </Label>
        <textarea
          id="description"
          className={`w-full min-h-[100px] p-3 border border-gray-300 rounded-md focus:ring-2 focus:ring-logistics-blue focus:border-transparent ${
            errors.description ? 'border-red-500' : ''
          }`}
          value={formData.description}
          onChange={(e) => setFormData(prev => ({ ...prev, description: e.target.value }))}
          placeholder="トラブルの詳細な説明を入力してください"
        />
        {errors.description && <p className="text-sm text-red-600">{errors.description}</p>}
      </div>

      {/* 発生経緯 */}
      <div className="space-y-2">
        <Label htmlFor="incidentDetails" className="text-sm font-medium text-gray-700">
          発生経緯 <span className="text-red-500">*</span>
        </Label>
        <textarea
          id="incidentDetails"
          className={`w-full min-h-[100px] p-3 border border-gray-300 rounded-md focus:ring-2 focus:ring-logistics-blue focus:border-transparent ${
            errors.incidentDetails ? 'border-red-500' : ''
          }`}
          value={formData.incidentDetails}
          onChange={(e) => setFormData(prev => ({ ...prev, incidentDetails: e.target.value }))}
          placeholder="何が起こったかを詳しく記述してください"
        />
        {errors.incidentDetails && <p className="text-sm text-red-600">{errors.incidentDetails}</p>}
      </div>

      {/* 発生日・発生場所 */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="space-y-2">
          <Label htmlFor="occurrenceDate" className="text-sm font-medium text-gray-700">
            発生日 <span className="text-red-500">*</span>
          </Label>
          <Input
            id="occurrenceDate"
            type="date"
            value={formData.occurrenceDate}
            onChange={(e) => setFormData(prev => ({ ...prev, occurrenceDate: e.target.value }))}
            className={`border-gray-300 focus:ring-2 focus:ring-logistics-blue ${
              errors.occurrenceDate ? 'border-red-500' : ''
            }`}
          />
          {errors.occurrenceDate && <p className="text-sm text-red-600">{errors.occurrenceDate}</p>}
        </div>

        <div className="space-y-2">
          <Label htmlFor="occurrenceLocation" className="text-sm font-medium text-gray-700">
            発生場所
          </Label>
          <Input
            id="occurrenceLocation"
            value={formData.occurrenceLocation}
            onChange={(e) => setFormData(prev => ({ ...prev, occurrenceLocation: e.target.value }))}
            placeholder="発生場所を入力してください"
            className="border-gray-300 focus:ring-2 focus:ring-logistics-blue"
          />
        </div>
      </div>

      {/* 注意事項 */}
      {!isEditMode && (
        <div className="bg-blue-50 border border-blue-200 rounded-md p-4">
          <div className="flex items-start">
            <div className="flex-shrink-0">
              <svg className="h-5 w-5 text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <div className="ml-3">
              <h3 className="text-sm font-medium text-blue-800">登録後の流れ</h3>
              <div className="mt-2 text-sm text-blue-700">
                <p>登録されたインシデントは「未分類」状態となります。</p>
                <p>インシデント管理者が後で詳細な分類作業を行います。</p>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* ボタン */}
      {!hideButtons && (
        <div className="flex justify-end space-x-3 pt-4 border-t">
          {onCancel && (
            <Button
              type="button"
              variant="outline"
              onClick={onCancel}
              disabled={loading}
            >
              キャンセル
            </Button>
          )}
          <Button
            type="submit"
            disabled={loading}
            className="bg-logistics-blue hover:bg-logistics-blue/90 text-white"
          >
            {loading 
              ? (isEditMode ? '更新中...' : '登録中...') 
              : (isEditMode ? '更新' : '登録')
            }
          </Button>
        </div>
      )}
    </form>
  );
}
