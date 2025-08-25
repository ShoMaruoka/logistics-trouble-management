import * as React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

import type { 
  IncidentDto, 
  CreateIncidentDto, 
  UpdateIncidentDto, 
  Priority,
  TroubleType,
  DamageType,
  Warehouse,
  ShippingCompany,
  EffectivenessStatus 
} from "@/lib/api-types";

interface IncidentFormProps {
  incident?: IncidentDto | null;
  onSubmit: (data: CreateIncidentDto | UpdateIncidentDto) => void;
  onCancel?: () => void;
  loading?: boolean;
  hideButtons?: boolean;
}

export function IncidentForm({ incident, onSubmit, onCancel, loading = false, hideButtons = false }: IncidentFormProps) {
  const [formData, setFormData] = React.useState({
    title: '',
    description: '',
    category: '', // This will be auto-generated
    priority: 'Medium' as Priority,
    troubleType: 'ProductTrouble' as TroubleType,
    damageType: 'DamageOrContamination' as DamageType,
    warehouse: 'WarehouseA' as Warehouse,
    shippingCompany: 'InHouse' as ShippingCompany,
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
  });

  React.useEffect(() => {
    if (incident) {
      setFormData({
        title: incident.title || '',
        description: incident.description || '',
        category: incident.category || '',
        priority: incident.priority || 'Medium',
        troubleType: incident.troubleType || 'ProductTrouble',
        damageType: incident.damageType || 'DamageOrContamination',
        warehouse: incident.warehouse || 'WarehouseA',
        shippingCompany: incident.shippingCompany || 'InHouse',
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
      });
    } else {
      // 新規作成時はフォームをリセット
      setFormData({
        title: '',
        description: '',
        category: '',
        priority: 'Medium',
        troubleType: 'ProductTrouble',
        damageType: 'DamageOrContamination',
        warehouse: 'WarehouseA',
        shippingCompany: 'InHouse',
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
      });
    }
  }, [incident]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    // カテゴリを自動生成（トラブル種類 + 損傷種類）
    const troubleTypeLabel = formData.troubleType === 'ProductTrouble' ? '商品トラブル' : '配送トラブル';
    const damageTypeLabels = {
      'WrongShipment': '誤出荷',
      'EarlyOrLateArrival': '早着・延着',
      'Lost': '紛失',
      'WrongDelivery': '誤配送',
      'DamageOrContamination': '破損・汚損',
      'OtherDeliveryMistake': 'その他の配送ミス',
      'OtherProductAccident': 'その他の商品事故'
    };
    const damageTypeLabel = damageTypeLabels[formData.damageType];
    const autoCategory = `${troubleTypeLabel} - ${damageTypeLabel}`;
    
    onSubmit({
      ...formData,
      category: autoCategory,
      occurrenceDate: formData.occurrenceDate,
      effectivenessDate: formData.effectivenessDate || null
    });
  };



  return (
         <form onSubmit={handleSubmit} className="space-y-4 bg-gray-100 p-4 rounded-lg w-full">
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
                 value={formData.troubleType} 
                 onValueChange={(value) => setFormData(prev => ({ ...prev, troubleType: value as TroubleType }))}
               >
                 <SelectTrigger className="border-gray-300 focus:ring-2 focus:ring-logistics-blue">
                   <SelectValue placeholder="トラブル種類を選択" />
                 </SelectTrigger>
                 <SelectContent>
                   <SelectItem value="ProductTrouble">商品トラブル</SelectItem>
                   <SelectItem value="DeliveryTrouble">配送トラブル</SelectItem>
                 </SelectContent>
               </Select>
             </div>

            {/* 損傷の種類 */}
            <div className="space-y-3 mb-6">
              <Label htmlFor="damageType" className="text-sm font-medium text-gray-700">損傷の種類 *</Label>
              <Select 
                value={formData.damageType} 
                onValueChange={(value) => setFormData(prev => ({ ...prev, damageType: value as DamageType }))}
              >
                <SelectTrigger className="border-gray-300 focus:ring-2 focus:ring-logistics-blue">
                  <SelectValue placeholder="まずトラブル種類を選択" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="WrongShipment">誤出荷</SelectItem>
                  <SelectItem value="EarlyOrLateArrival">早着・延着</SelectItem>
                  <SelectItem value="Lost">紛失</SelectItem>
                  <SelectItem value="WrongDelivery">誤配送</SelectItem>
                  <SelectItem value="DamageOrContamination">破損・汚損</SelectItem>
                  <SelectItem value="OtherDeliveryMistake">その他の配送ミス</SelectItem>
                  <SelectItem value="OtherProductAccident">その他の商品事故</SelectItem>
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
                  onChange={(e) => setFormData(prev => ({ ...prev, totalShipments: parseInt(e.target.value) || 0 }))}
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
                  onChange={(e) => setFormData(prev => ({ ...prev, defectiveItems: parseInt(e.target.value) || 0 }))}
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
                  value={formData.warehouse} 
                  onValueChange={(value) => setFormData(prev => ({ ...prev, warehouse: value as Warehouse }))}
                >
                  <SelectTrigger className="border-gray-300 focus:ring-2 focus:ring-logistics-blue">
                    <SelectValue placeholder="選択..." />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="WarehouseA">A倉庫</SelectItem>
                    <SelectItem value="WarehouseB">B倉庫</SelectItem>
                    <SelectItem value="WarehouseC">C倉庫</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <Label htmlFor="shippingCompany" className="text-sm font-medium text-gray-700">運送会社名 *</Label>
                <Select 
                  value={formData.shippingCompany} 
                  onValueChange={(value) => setFormData(prev => ({ ...prev, shippingCompany: value as ShippingCompany }))}
                >
                  <SelectTrigger className="border-gray-300 focus:ring-2 focus:ring-logistics-blue">
                    <SelectValue placeholder="選択..." />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="InHouse">庫内</SelectItem>
                    <SelectItem value="Charter">チャーター</SelectItem>
                    <SelectItem value="ATransport">A運輸</SelectItem>
                    <SelectItem value="BExpress">B急便</SelectItem>
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
             className="bg-logistics-blue hover:bg-logistics-blue/90 text-white"
           >
             {loading ? '保存中...' : (incident ? '保存' : '作成')}
           </Button>
         </div>
       )}
    </form>
  );
}
