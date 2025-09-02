import * as React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { useMasterData } from "@/hooks/useMasterData";

import type { 
  Incident, 
  CreateIncidentDto, 
  UpdateIncidentDto, 
  Priority,
  EffectivenessStatus 
} from "@/lib/types";

// 数値入力のバリデーション関数
const validateNumericInput = (value: string, min: number = 0, max: number = Number.MAX_SAFE_INTEGER): number => {
  // 入力値をトリム
  const trimmedValue = value.trim();
  
  // 空文字列の場合は0を返す
  if (trimmedValue === '') {
    return 0;
  }
  
  // 10進数としてパース
  const parsedValue = parseInt(trimmedValue, 10);
  
  // 数値として有効かチェック
  if (!Number.isFinite(parsedValue) || !Number.isInteger(parsedValue)) {
    return 0;
  }
  
  // 範囲チェックとクリッピング
  if (parsedValue < min) {
    return min;
  }
  
  if (parsedValue > max) {
    return max;
  }
  
  return parsedValue;
};

// マスタデータIDの検証関数
const validateMasterDataId = (id: any): boolean => {
  return id != null && id !== undefined && Number.isInteger(id) && id > 0;
};

// 有効なIDを持つ最初のマスタデータ要素を取得する関数
const getFirstValidId = <T extends { id: any }>(items: T[]): number | null => {
  if (!Array.isArray(items) || items.length === 0) {
    return null;
  }
  
  const validItem = items.find(item => validateMasterDataId(item?.id));
  return validItem ? validItem.id : null;
};

interface IncidentFormProps {
  incident?: Incident | null;
  onSubmit: (data: CreateIncidentDto | UpdateIncidentDto) => void;
  onCancel?: () => void;
  loading?: boolean;
  hideButtons?: boolean;
}

export function IncidentForm({ incident, onSubmit, onCancel, loading = false, hideButtons = false }: IncidentFormProps) {
  const { troubleTypes, damageTypes, warehouses, shippingCompanies, loading: masterDataLoading, error: masterDataError } = useMasterData();
  
  const [formData, setFormData] = React.useState({
    title: '',
    description: '',
    category: '', // This will be auto-generated
    priority: 'Medium' as Priority,
    troubleTypeId: 0,
    damageTypeId: 0,
    warehouseId: 0,
    shippingCompanyId: 0,
    effectivenessStatus: 'NotImplemented' as EffectivenessStatus,
    resolution: '',
    
    // 新規追加項目（提供サイト対応）
    incidentDetails: '', // 発生経緯
    totalShipments: 0, // 出荷総数
    defectiveItems: 0, // 不良品数
    occurrenceDate: '', // 発生日
    occurrenceLocation: '', // 発生場所
    summary: '', // 概要
    cause: '', // 原因
    preventionMeasures: '', // 再発防止策
    effectivenessDate: '', // 有効性確認日
    effectivenessComment: '', // 有効性確認コメント
    hasMasterDataError: false, // マスタデータエラーを追加
  });

  React.useEffect(() => {
    if (incident) {
      setFormData({
        title: incident.title || '',
        description: incident.description || '',
        category: incident.category || '',
        priority: incident.priority || 'Medium',
        troubleTypeId: incident.troubleTypeId || 0,
        damageTypeId: incident.damageTypeId || 0,
        warehouseId: incident.warehouseId || 0,
        shippingCompanyId: incident.shippingCompanyId || 0,
        effectivenessStatus: incident.effectivenessStatus || 'NotImplemented',
        resolution: incident.resolution || '',
        
        // 新規追加項目
        incidentDetails: incident.incidentDetails || '',
        totalShipments: incident.totalShipments || 0,
        defectiveItems: incident.defectiveItems || 0,
        occurrenceDate: incident.occurrenceDate ? new Date(incident.occurrenceDate).toISOString().split('T')[0] : '',
        occurrenceLocation: incident.occurrenceLocation || '',
        summary: incident.summary || '',
        cause: incident.cause || '',
        preventionMeasures: incident.preventionMeasures || '',
        effectivenessDate: incident.effectivenessDate ? new Date(incident.effectivenessDate).toISOString().split('T')[0] : '',
        effectivenessComment: incident.effectivenessComment || '',
        hasMasterDataError: false, // 編集時はエラーをリセット
      });
    } else {
      // 新規作成時はフォームをリセット
      setFormData({
        title: '',
        description: '',
        category: '',
        priority: 'Medium',
        troubleTypeId: 0,
        damageTypeId: 0,
        warehouseId: 0,
        shippingCompanyId: 0,
        effectivenessStatus: 'NotImplemented',
        resolution: '',
        
        // 新規追加項目
        incidentDetails: '',
        totalShipments: 0,
        defectiveItems: 0,
        occurrenceDate: '',
        occurrenceLocation: '',
        summary: '',
        cause: '',
        preventionMeasures: '',
        effectivenessDate: '',
        effectivenessComment: '',
        hasMasterDataError: false, // 新規作成時はエラーをリセット
      });
    }
  }, [incident]);

  // マスタデータが読み込まれた後に初期値を設定
  React.useEffect(() => {
    if (!masterDataLoading && troubleTypes.length > 0 && damageTypes.length > 0 && warehouses.length > 0 && shippingCompanies.length > 0) {
      if (!incident) {
        try {
          // 新規作成時は有効なIDを持つ最初のマスタデータを選択
          const validTroubleTypeId = getFirstValidId(troubleTypes);
          const validDamageTypeId = getFirstValidId(damageTypes);
          const validWarehouseId = getFirstValidId(warehouses);
          const validShippingCompanyId = getFirstValidId(shippingCompanies);
          
          // 検証されたIDのみを設定
          setFormData(prev => ({
            ...prev,
            troubleTypeId: validTroubleTypeId || 0,
            damageTypeId: validDamageTypeId || 0,
            warehouseId: validWarehouseId || 0,
            shippingCompanyId: validShippingCompanyId || 0,
            hasMasterDataError: false, // エラーフラグをリセット
          }));
          
          // デバッグ用ログ（開発環境のみ）
          if (process.env.NODE_ENV === 'development') {
            console.log('マスタデータID設定完了:', {
              troubleTypeId: validTroubleTypeId,
              damageTypeId: validDamageTypeId,
              warehouseId: validWarehouseId,
              shippingCompanyId: validShippingCompanyId
            });
          }
        } catch (error) {
          console.error('マスタデータID設定中にエラーが発生しました:', error);
          // エラーが発生した場合はデフォルト値（0）を設定
          setFormData(prev => ({
            ...prev,
            troubleTypeId: 0,
            damageTypeId: 0,
            warehouseId: 0,
            shippingCompanyId: 0,
            hasMasterDataError: true, // エラーフラグを設定
          }));
        }
      } else {
        // 編集時もマスタデータが正常に読み込まれている場合はエラーフラグをリセット
        setFormData(prev => ({
          ...prev,
          hasMasterDataError: false,
        }));
      }
    } else {
      // マスタデータが読み込まれていない場合はエラーフラグを設定
      setFormData(prev => ({
        ...prev,
        hasMasterDataError: true,
      }));
    }
  }, [masterDataLoading, troubleTypes, damageTypes, warehouses, shippingCompanies, incident]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    // カテゴリを自動生成（トラブル種類 + 損傷種類）
    const troubleType = troubleTypes.find(t => t.id === formData.troubleTypeId);
    const damageType = damageTypes.find(d => d.id === formData.damageTypeId);
    
    // データ整合性チェック
    let hasMissingMasterData = false;
    let missingDataWarnings: string[] = [];
    
    if (!troubleType) {
      hasMissingMasterData = true;
      missingDataWarnings.push('トラブル種類');
      console.warn('トラブル種類のマスタデータが見つかりません。ID:', formData.troubleTypeId);
    }
    
    if (!damageType) {
      hasMissingMasterData = true;
      missingDataWarnings.push('損傷種類');
      console.warn('損傷種類のマスタデータが見つかりません。ID:', formData.damageTypeId);
    }
    
    // 曖昧性のないラベルを使用
    const troubleTypeLabel = troubleType?.name || '未設定';
    const damageTypeLabel = damageType?.name || '未設定';
    
    // autoCategory構築時の曖昧な値の連結を回避
    let autoCategory: string;
    if (troubleType && damageType) {
      autoCategory = `${troubleTypeLabel} - ${damageTypeLabel}`;
    } else if (troubleType) {
      autoCategory = `${troubleTypeLabel} - 損傷種類未設定`;
    } else if (damageType) {
      autoCategory = `トラブル種類未設定 - ${damageTypeLabel}`;
    } else {
      autoCategory = 'カテゴリ未設定';
    }
    
    // マスタデータが不足している場合は警告を表示
    if (hasMissingMasterData) {
      console.warn(`マスタデータが不足しています: ${missingDataWarnings.join(', ')}`);
      // フォームエラーフラグを設定（必要に応じてUIに表示）
      setFormData(prev => ({ ...prev, hasMasterDataError: true }));
      
      // ユーザーに確認を求める
      const confirmSubmit = window.confirm(
        `マスタデータが不足しています（${missingDataWarnings.join(', ')}）。\n` +
        'カテゴリが正しく生成されない可能性がありますが、送信を続行しますか？'
      );
      
      if (!confirmSubmit) {
        return; // 送信をキャンセル
      }
    } else {
      setFormData(prev => ({ ...prev, hasMasterDataError: false }));
    }
    
    onSubmit({
      ...formData,
      category: autoCategory,
      occurrenceDate: formData.occurrenceDate,
      effectivenessDate: formData.effectivenessDate || null
    });
  };



  // マスタデータの読み込み中またはエラー状態の場合はローディング表示
  if (masterDataLoading) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-logistics-blue mx-auto mb-4"></div>
          <p className="text-gray-600">マスタデータを読み込み中...</p>
        </div>
      </div>
    );
  }

  if (masterDataError) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-center">
          <div className="text-red-500 mb-4">
            <svg className="w-12 h-12 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
            </svg>
          </div>
          <p className="text-red-600 font-medium mb-2">エラーが発生しました</p>
          <p className="text-gray-600">{masterDataError}</p>
          <Button 
            onClick={() => window.location.reload()} 
            className="mt-4 bg-logistics-blue hover:bg-logistics-blue/90 text-white"
          >
            再試行
          </Button>
        </div>
      </div>
    );
  }

  return (
         <form onSubmit={handleSubmit} className="space-y-4 bg-gray-100 p-4 rounded-lg w-full">
      {/* マスタデータエラーメッセージ */}
      {formData.hasMasterDataError && (
        <div className="bg-yellow-50 border border-yellow-200 rounded-md p-4 mb-4">
          <div className="flex items-center">
            <div className="flex-shrink-0">
              <svg className="h-5 w-5 text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
              </svg>
            </div>
            <div className="ml-3">
              <h3 className="text-sm font-medium text-yellow-800">マスタデータの警告</h3>
              <div className="mt-2 text-sm text-yellow-700">
                <p>一部のマスタデータが見つかりません。カテゴリが正しく生成されない可能性があります。</p>
                <p className="mt-1">詳細はブラウザのコンソールを確認してください。</p>
              </div>
            </div>
          </div>
        </div>
      )}
      
      <div className="grid grid-cols-1 xl:grid-cols-2 gap-12">
                 {/* 左列: 発生経緯・トラブル詳細 */}
         <div className="space-y-4">
          <div className="bg-white p-4 rounded-lg shadow-sm">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">1. 発生経緯・トラブル詳細</h3>
            
            {/* タイトル */}
            <div className="space-y-3 mb-6">
              <Label htmlFor="title" className="text-sm font-medium text-gray-700">タイトル *</Label>
              <Input
                id="title"
                value={formData.title}
                onChange={(e) => setFormData(prev => ({ ...prev, title: e.target.value }))}
                placeholder="トラブルのタイトルを入力してください..."
                required
                className="border-gray-300 focus:ring-2 focus:ring-logistics-blue"
              />
            </div>
            
            {/* 詳細説明 */}
            <div className="space-y-3 mb-6">
              <Label htmlFor="description" className="text-sm font-medium text-gray-700">詳細説明 *</Label>
              <textarea
                id="description"
                className="w-full min-h-[80px] p-3 border border-gray-300 rounded-md focus:ring-2 focus:ring-logistics-blue focus:border-transparent"
                value={formData.description}
                onChange={(e) => setFormData(prev => ({ ...prev, description: e.target.value }))}
                placeholder="トラブルの詳細な説明を入力してください..."
                required
              />
            </div>
            
            {/* 発生経緯 */}
            <div className="space-y-3 mb-6">
              <Label htmlFor="incidentDetails" className="text-sm font-medium text-gray-700">発生経緯 *</Label>
                             <textarea
                 id="incidentDetails"
                 className="w-full min-h-[80px] p-3 border border-gray-300 rounded-md focus:ring-2 focus:ring-logistics-blue focus:border-transparent"
                 value={formData.incidentDetails}
                 onChange={(e) => setFormData(prev => ({ ...prev, incidentDetails: e.target.value }))}
                 placeholder="何が起こったかを記述してください..."
                 required
               />
            </div>

                         {/* 写真添付 */}
             <div className="space-y-3 mb-6">
               <Label className="text-sm font-medium text-gray-700">写真</Label>
               <div className="flex items-center gap-3">
                 <Button type="button" variant="outline" size="sm" className="border-gray-300">
                   ファイルを選択
                 </Button>
                 <span className="text-sm text-gray-500">選択されていません</span>
               </div>
             </div>

                         {/* トラブル種類 */}
             <div className="space-y-3 mb-6">
               <Label htmlFor="troubleType" className="text-sm font-medium text-gray-700">トラブル種類 *</Label>
               <Select 
                 value={formData.troubleTypeId.toString()} 
                 onValueChange={(value) => setFormData(prev => ({ ...prev, troubleTypeId: parseInt(value) }))}
                 disabled={masterDataLoading}
               >
                 <SelectTrigger className="border-gray-300 focus:ring-2 focus:ring-logistics-blue">
                   <SelectValue placeholder={masterDataLoading ? "読み込み中..." : "トラブル種類を選択"} />
                 </SelectTrigger>
                 <SelectContent>
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

            {/* 損傷の種類 */}
            <div className="space-y-3 mb-6">
              <Label htmlFor="damageType" className="text-sm font-medium text-gray-700">損傷の種類 *</Label>
              <Select 
                value={formData.damageTypeId.toString()} 
                onValueChange={(value) => setFormData(prev => ({ ...prev, damageTypeId: parseInt(value) }))}
                disabled={masterDataLoading}
              >
                <SelectTrigger className="border-gray-300 focus:ring-2 focus:ring-logistics-blue">
                  <SelectValue placeholder={masterDataLoading ? "読み込み中..." : "損傷の種類を選択"} />
                </SelectTrigger>
                <SelectContent>
                  {damageTypes.map((damageType) => (
                    <SelectItem key={damageType.id} value={damageType.id.toString()}>
                      <div className="flex items-center gap-2">
                        <span className="text-xs px-2 py-1 bg-gray-100 rounded">
                          {damageType.category}
                        </span>
                        {damageType.name}
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

                         {/* 出荷総数・不良品数 */}
             <div className="grid grid-cols-2 gap-4 mb-6">
              <div className="space-y-2">
                <Label htmlFor="totalShipments" className="text-sm font-medium text-gray-700">出荷総数</Label>
                <Input
                  id="totalShipments"
                  type="number"
                  value={formData.totalShipments}
                  onChange={(e) => setFormData(prev => ({ ...prev, totalShipments: validateNumericInput(e.target.value) }))}
                  placeholder="0"
                  className="border-gray-300 focus:ring-2 focus:ring-logistics-blue"
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="defectiveItems" className="text-sm font-medium text-gray-700">不良品数</Label>
                <Input
                  id="defectiveItems"
                  type="number"
                  value={formData.defectiveItems}
                  onChange={(e) => setFormData(prev => ({ ...prev, defectiveItems: validateNumericInput(e.target.value) }))}
                  placeholder="0"
                  className="border-gray-300 focus:ring-2 focus:ring-logistics-blue"
                />
              </div>
            </div>

            {/* 発生日 */}
            <div className="space-y-3 mb-6">
              <Label htmlFor="occurrenceDate" className="text-sm font-medium text-gray-700">発生日 *</Label>
              <div className="relative">
                <Input
                  id="occurrenceDate"
                  type="date"
                  value={formData.occurrenceDate}
                  onChange={(e) => setFormData(prev => ({ ...prev, occurrenceDate: e.target.value }))}
                  required
                  className="border-gray-300 focus:ring-2 focus:ring-logistics-blue pr-10"
                />
                <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
                  <svg className="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                  </svg>
                </div>
              </div>
            </div>

            {/* 発生場所 */}
            <div className="space-y-3 mb-6">
              <Label htmlFor="occurrenceLocation" className="text-sm font-medium text-gray-700">発生場所</Label>
              <Input
                id="occurrenceLocation"
                value={formData.occurrenceLocation}
                onChange={(e) => setFormData(prev => ({ ...prev, occurrenceLocation: e.target.value }))}
                placeholder="場所"
                className="border-gray-300 focus:ring-2 focus:ring-logistics-blue"
              />
            </div>

                         {/* 出荷元倉庫・運送会社名 */}
             <div className="grid grid-cols-2 gap-4 mb-6">
              <div className="space-y-2">
                <Label htmlFor="warehouse" className="text-sm font-medium text-gray-700">出荷元倉庫 *</Label>
                <Select 
                  value={formData.warehouseId.toString()} 
                  onValueChange={(value) => setFormData(prev => ({ ...prev, warehouseId: parseInt(value) }))}
                  disabled={masterDataLoading}
                >
                  <SelectTrigger className="border-gray-300 focus:ring-2 focus:ring-logistics-blue">
                    <SelectValue placeholder={masterDataLoading ? "読み込み中..." : "選択..."} />
                  </SelectTrigger>
                  <SelectContent>
                    {warehouses.map((warehouse) => (
                      <SelectItem key={warehouse.id} value={warehouse.id.toString()}>
                        <div className="flex items-center gap-2">
                          {warehouse.name}
                          {warehouse.location && (
                            <span className="text-xs text-gray-500">({warehouse.location})</span>
                          )}
                        </div>
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <Label htmlFor="shippingCompany" className="text-sm font-medium text-gray-700">運送会社名 *</Label>
                <Select 
                  value={formData.shippingCompanyId.toString()} 
                  onValueChange={(value) => setFormData(prev => ({ ...prev, shippingCompanyId: parseInt(value) }))}
                  disabled={masterDataLoading}
                >
                  <SelectTrigger className="border-gray-300 focus:ring-2 focus:ring-logistics-blue">
                    <SelectValue placeholder={masterDataLoading ? "読み込み中..." : "選択..."} />
                  </SelectTrigger>
                  <SelectContent>
                    {shippingCompanies.map((shippingCompany) => (
                      <SelectItem key={shippingCompany.id} value={shippingCompany.id.toString()}>
                        <div className="flex items-center gap-2">
                          {shippingCompany.name}
                          <span className="text-xs px-2 py-1 bg-gray-100 rounded">
                            {shippingCompany.companyType}
                          </span>
                        </div>
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>

            {/* 概要 */}
            <div className="space-y-3">
              <Label htmlFor="summary" className="text-sm font-medium text-gray-700">概要</Label>
              <textarea
                id="summary"
                className="w-full min-h-[80px] p-3 border border-gray-300 rounded-md focus:ring-2 focus:ring-logistics-blue focus:border-transparent"
                value={formData.summary}
                onChange={(e) => setFormData(prev => ({ ...prev, summary: e.target.value }))}
                placeholder="トラブルの簡単な概要"
              />
            </div>
          </div>
        </div>

                 {/* 右列: 原因分析・再発防止策・有効性評価 */}
         <div className="space-y-4">
           {/* 原因分析 */}
           <div className="bg-white p-4 rounded-lg shadow-sm">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">2. 原因分析</h3>
            
                         <div className="space-y-3">
               <Label htmlFor="cause" className="text-sm font-medium text-gray-700">原因</Label>
                               <textarea
                  id="cause"
                  className="w-full min-h-[80px] p-3 border border-gray-300 rounded-md focus:ring-2 focus:ring-logistics-blue focus:border-transparent"
                  value={formData.cause}
                  onChange={(e) => setFormData(prev => ({ ...prev, cause: e.target.value }))}
                  placeholder="原因を記述してください..."
                />
             </div>
          </div>

                     {/* 再発防止策 */}
           <div className="bg-white p-4 rounded-lg shadow-sm">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">3. 再発防止策</h3>
            
                         <div className="space-y-3">
               <Label htmlFor="preventionMeasures" className="text-sm font-medium text-gray-700">再発防止策</Label>
                               <textarea
                  id="preventionMeasures"
                  className="w-full min-h-[80px] p-3 border border-gray-300 rounded-md focus:ring-2 focus:ring-logistics-blue focus:border-transparent"
                  value={formData.preventionMeasures}
                  onChange={(e) => setFormData(prev => ({ ...prev, preventionMeasures: e.target.value }))}
                  placeholder="再発防止の手順をリストアップしてください..."
                />
             </div>
          </div>

                     {/* 有効性評価 */}
           <div className="bg-white p-4 rounded-lg shadow-sm">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">4. 有効性評価</h3>
            
            <div className="space-y-3 mb-4">
              <Label className="text-sm font-medium text-gray-700">有効性の確認状況</Label>
              <div className="flex gap-4">
                <label className="flex items-center gap-2">
                  <input
                    type="radio"
                    name="effectivenessStatus"
                    value="NotImplemented"
                    checked={formData.effectivenessStatus === 'NotImplemented'}
                    onChange={(e) => setFormData(prev => ({ ...prev, effectivenessStatus: e.target.value as EffectivenessStatus }))}
                    className="text-logistics-blue focus:ring-logistics-blue"
                  />
                  <span className="text-sm">未実施</span>
                </label>
                <label className="flex items-center gap-2">
                  <input
                    type="radio"
                    name="effectivenessStatus"
                    value="Implemented"
                    checked={formData.effectivenessStatus === 'Implemented'}
                    onChange={(e) => setFormData(prev => ({ ...prev, effectivenessStatus: e.target.value as EffectivenessStatus }))}
                    className="text-logistics-blue focus:ring-logistics-blue"
                  />
                  <span className="text-sm">実施</span>
                </label>
              </div>
            </div>

            <div className="space-y-3 mb-4">
              <Label htmlFor="effectivenessDate" className="text-sm font-medium text-gray-700">有効性の確認日</Label>
              <div className="relative">
                <Input
                  id="effectivenessDate"
                  type="date"
                  value={formData.effectivenessDate}
                  onChange={(e) => setFormData(prev => ({ ...prev, effectivenessDate: e.target.value }))}
                  className="border-gray-300 focus:ring-2 focus:ring-logistics-blue pr-10"
                />
                <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
                  <svg className="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                  </svg>
                </div>
              </div>
            </div>

            <div className="space-y-3">
              <Label htmlFor="effectivenessComment" className="text-sm font-medium text-gray-700">有効性の確認コメント</Label>
              <textarea
                id="effectivenessComment"
                className="w-full min-h-[80px] p-3 border border-gray-300 rounded-md focus:ring-2 focus:ring-logistics-blue focus:border-transparent"
                value={formData.effectivenessComment}
                onChange={(e) => setFormData(prev => ({ ...prev, effectivenessComment: e.target.value }))}
                placeholder="有効性の評価コメントを記述..."
              />
            </div>
          </div>

                     {/* 編集時のみ表示する項目 */}
           {incident && (
             <div className="bg-white p-4 rounded-lg shadow-sm">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">編集項目</h3>
              
              <div className="space-y-3 mb-4">
                <Label htmlFor="priority" className="text-sm font-medium text-gray-700">優先度</Label>
                <Select 
                  value={formData.priority} 
                  onValueChange={(value) => setFormData(prev => ({ ...prev, priority: value as Priority }))}
                >
                  <SelectTrigger className="border-gray-300 focus:ring-2 focus:ring-logistics-blue">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Low">低</SelectItem>
                    <SelectItem value="Medium">中</SelectItem>
                    <SelectItem value="High">高</SelectItem>
                    <SelectItem value="Critical">緊急</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div className="space-y-3">
                <Label htmlFor="resolution" className="text-sm font-medium text-gray-700">解決内容</Label>
                <textarea
                  id="resolution"
                  className="w-full min-h-[80px] p-3 border border-gray-300 rounded-md focus:ring-2 focus:ring-logistics-blue focus:border-transparent"
                  value={formData.resolution}
                  onChange={(e) => setFormData(prev => ({ ...prev, resolution: e.target.value }))}
                  placeholder="解決内容を入力"
                />
              </div>
            </div>
          )}
        </div>
      </div>

             {/* カテゴリは自動生成（隠しフィールド） */}
       <input type="hidden" value={formData.category} />

              {/* ボタン */}
       {!hideButtons && (
         <div className="flex justify-end gap-3 pt-6 border-t border-gray-200 bg-white p-4 rounded-lg shadow-sm">
           {onCancel && (
             <Button type="button" variant="outline" onClick={onCancel} className="border-gray-300">
               キャンセル
             </Button>
           )}
           <Button 
             type="submit" 
             disabled={loading}
             className={`${
               formData.hasMasterDataError 
                 ? 'bg-yellow-500 hover:bg-yellow-600 text-white' 
                 : 'bg-logistics-blue hover:bg-logistics-blue/90 text-white'
             }`}
             title={formData.hasMasterDataError ? 'マスタデータエラーがありますが、送信は可能です' : ''}
           >
             {loading ? '保存中...' : (incident ? '保存' : '作成')}
           </Button>
         </div>
       )}
    </form>
  );
}
