// 基本型定義
export type Priority = 'Low' | 'Medium' | 'High' | 'Critical';
export type IncidentStatus = 'Open' | 'InProgress' | 'Resolved' | 'Closed';
export type EffectivenessStatus = 'NotImplemented' | 'Implemented';

// 新ワークフロー関連型定義
export type WorkflowMode = 'legacy' | 'new';

// 新ワークフローステータス（6段階）
export type WorkflowStatus = 
  | 'Unclassified'           // 未分類
  | 'Pending'                // 未対応
  | 'InProgress'             // 対応中
  | 'Completed'              // 対応済
  | 'PreventionProposed'     // 再発防止策提案済
  | 'EffectivenessConfirmed' // 有効性確認済

// レガシーステータス（既存5段階）
export type LegacyIncidentStatus = 
  | 'Open'        // 未解決
  | 'InProgress'  // 対応中
  | 'Resolved'    // 解決済み
  | 'Closed'      // 完了
  | 'Cancelled'   // キャンセル

// 統合ステータス情報
export interface IncidentStatusInfo {
  mode: WorkflowMode;
  legacyStatus?: LegacyIncidentStatus;
  workflowStatus?: WorkflowStatus;
}

// マスタデータ型定義
export interface TroubleType {
  id: number;
  name: string;
  description?: string;
  color: string;
  sortOrder: number;
  isActive: boolean;
}

export interface DamageType {
  id: number;
  name: string;
  description?: string;
  category: string;
  sortOrder: number;
  isActive: boolean;
}

export interface Warehouse {
  id: number;
  name: string;
  description?: string;
  location?: string;
  contactInfo?: string;
  sortOrder: number;
  isActive: boolean;
}

export interface ShippingCompany {
  id: number;
  name: string;
  description?: string;
  companyType: string;
  contactInfo?: string;
  sortOrder: number;
  isActive: boolean;
}

// マスタデータ作成・更新用DTO
export interface CreateTroubleTypeDto {
  name: string;
  description?: string;
  color: string;
  sortOrder: number;
}

export interface UpdateTroubleTypeDto {
  name: string;
  description?: string;
  color: string;
  sortOrder: number;
  isActive: boolean;
}

export interface CreateDamageTypeDto {
  name: string;
  description?: string;
  category: string;
  sortOrder: number;
}

export interface UpdateDamageTypeDto {
  name: string;
  description?: string;
  category: string;
  sortOrder: number;
  isActive: boolean;
}

export interface CreateWarehouseDto {
  name: string;
  description?: string;
  location?: string;
  contactInfo?: string;
  sortOrder: number;
}

export interface UpdateWarehouseDto {
  name: string;
  description?: string;
  location?: string;
  contactInfo?: string;
  sortOrder: number;
  isActive: boolean;
}

export interface CreateShippingCompanyDto {
  name: string;
  description?: string;
  companyType: string;
  contactInfo?: string;
  sortOrder: number;
}

export interface UpdateShippingCompanyDto {
  name: string;
  description?: string;
  companyType: string;
  contactInfo?: string;
  sortOrder: number;
  isActive: boolean;
}

// 更新されたIncident型（マスタID対応）
export interface Incident {
  id: number;
  title: string;
  description: string;
  category: string;
  reportedById: number;
  reportedByName: string;
  assignedToId?: number;
  assignedToName?: string;
  troubleTypeId: number;
  damageTypeId: number;
  warehouseId: number;
  shippingCompanyId: number;
  effectivenessStatus: 'NotImplemented' | 'Implemented';
  effectivenessDate?: string | null; // 有効性確認日
  effectivenessComment: string; // 有効性確認コメント
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  status: 'Open' | 'InProgress' | 'Resolved' | 'Closed';
  occurrenceDate?: string | null;
  incidentDetails?: string;
  totalShipments?: number;
  defectiveItems?: number;
  occurrenceLocation?: string;
  summary?: string;
  cause?: string;
  preventionMeasures?: string;
  reportedDate: string;
  assignedDate?: string;
  resolvedDate?: string | null;
  closedDate?: string;
  resolution?: string;
  resolutionTime?: string;
  attachmentCount: number;
  isOverdue: boolean;
  createdAt: string;
  updatedAt: string;
  
  // 表示用のマスタ情報
  troubleTypeName: string;
  troubleTypeColor: string;
  damageTypeName: string;
  warehouseName: string;
  shippingCompanyName: string;
  
  // 既存のプロパティ（後方互換性のため）
  troubleType?: string;
  damageType?: string;
  location?: string;
  date?: string;
  processDescription?: string;
  recurrencePreventionMeasures?: string;
  shipmentVolume?: number;
  defectiveUnits?: number;
  shippingWarehouse?: string;
  shippingCompany?: string;
  effectivenessCheckStatus?: '実施' | '実施中' | '未実施';
  effectivenessCheckDate?: string;
  effectivenessCheckComment?: string;
}

// 新ワークフロー対応版Incident型
export interface IncidentWithWorkflow extends Incident {
  // ワークフロー関連プロパティ
  workflowStatus?: WorkflowStatus;
  workflowMode: WorkflowMode;
  
  // 新ワークフロー日付フィールド
  dueDate?: string;
  responseStartDate?: string;
  causeAnalysisDate?: string;
  completionDate?: string;
  preventionProposalDate?: string;
  effectivenessConfirmationDate?: string;
  
  // 新作業内容フィールド
  responseContent?: string;
  
  // ワークフローデータ（段階的に追加される情報）
  workflowData?: IncidentWorkflowData;
}

// ワークフローデータ
export interface IncidentWorkflowData {
  // 分類段階で追加される情報
  totalShipments?: number;
  defectiveItems?: number;
  occurrenceLocation?: string;
  priority?: Priority;
  dueDate?: string;
  
  // ワークフロー進捗日付
  responseStartDate?: string;
  causeAnalysisDate?: string;
  completionDate?: string;
  preventionProposalDate?: string;
  effectivenessConfirmationDate?: string;
  
  // 作業内容
  cause?: string;
  responseContent?: string;
  preventionMeasures?: string;
  effectivenessStatus?: string;
  effectivenessComment?: string;
}

// ワークフロー関連DTO型定義
export interface WorkflowActionResultDto {
  success: boolean;
  message: string;
  newStatus?: WorkflowStatus;
  availableActions: string[];
}

export interface ClassifyIncidentDto {
  incidentId: number;
  troubleTypeId: number;
  damageTypeId: number;
  warehouseId: number;
  shippingCompanyId: number;
  totalShipments: number;
  defectiveItems: number;
  priority: Priority;
  dueDate: string;
}

export interface StartResponseDto {
  incidentId: number;
}

export interface AnalyzeCauseDto {
  incidentId: number;
  cause: string;
}

export interface CompleteResponseDto {
  incidentId: number;
  responseContent: string;
}

export interface ProposePreventionDto {
  incidentId: number;
  preventionMeasures: string;
}

export interface ConfirmEffectivenessDto {
  incidentId: number;
  effectivenessStatus: string;
  effectivenessComment: string;
}

export interface WorkflowStatisticsDto {
  unclassifiedCount: number;
  pendingCount: number;
  inProgressCount: number;
  completedCount: number;
  preventionProposedCount: number;
  effectivenessConfirmedCount: number;
  totalWithWorkflow: number;
  totalLegacyMode: number;
}

// 既存の型定義（後方互換性のため）
export interface StatisticsSummaryDto {
  totalIncidents: number;
  openCount: number;
  inProgressCount: number;
  resolvedCount: number;
  closedCount: number;
  criticalCount: number;
  highCount: number;
  mediumCount: number;
  lowCount: number;
  averageResolutionTime: string; // TimeSpan is serialized as string
  ppm: number;
}

export interface ChartSeriesDto {
  name: string;
  data: number[];
}

export interface ChartDataDto {
  chartType: string;
  title: string;
  series: ChartSeriesDto[];
  labels: string[];
}

export interface PieChartDataDto {
  items: PieChartItemDto[];
}

export interface PieChartItemDto {
  label: string;
  value: number;
  color: string;
}

export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface CreateIncidentDto {
  title: string;
  description: string;
  category: string;
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  
  // 物流特化項目（マスタIDベース）
  troubleTypeId: number;
  damageTypeId: number;
  warehouseId: number;
  shippingCompanyId: number;
  
  // 新規追加項目（提供サイト対応）
  incidentDetails: string; // 発生経緯
  totalShipments: number; // 出荷総数
  defectiveItems: number; // 不良品数
  occurrenceDate: string; // 発生日
  occurrenceLocation: string; // 発生場所
  summary: string; // 概要
  cause: string; // 原因
  preventionMeasures: string; // 再発防止策
  effectivenessStatus: 'NotImplemented' | 'Implemented'; // 有効性評価
  effectivenessDate?: string | null; // 有効性確認日
  effectivenessComment: string; // 有効性確認コメント
  
  reportedById: number;
}

export interface UpdateIncidentDto {
  title?: string;
  description?: string;
  category?: string;
  priority?: 'Low' | 'Medium' | 'High' | 'Critical';
  
  // 物流特化項目（マスタIDベース）
  troubleTypeId?: number;
  damageTypeId?: number;
  warehouseId?: number;
  shippingCompanyId?: number;
  effectivenessStatus?: 'NotImplemented' | 'Implemented';
  
  // 新規追加項目（提供サイト対応）
  incidentDetails?: string; // 発生経緯
  totalShipments?: number; // 出荷総数
  defectiveItems?: number; // 不良品数
  occurrenceDate?: string; // 発生日
  occurrenceLocation?: string; // 発生場所
  summary?: string; // 概要
  cause?: string; // 原因
  preventionMeasures?: string; // 再発防止策
  effectivenessDate?: string | null; // 有効性確認日
  effectivenessComment?: string; // 有効性確認コメント
  
  assignedToId?: number;
  resolution?: string;
}

export interface IncidentSearchDto {
  searchTerm?: string;
  status?: string;
  priority?: string;
  category?: string;
  troubleTypeId?: number;
  damageTypeId?: number;
  warehouseId?: number;
  shippingCompanyId?: number;
  fromDate?: string;
  toDate?: string;
  page: number;
  pageSize: number;
  sortBy?: string;
  ascending?: boolean;
}

export interface UpdateIncidentDto {
  title?: string;
  description?: string;
  category?: string;
  reportedById?: number;
  troubleTypeId?: number;
  damageTypeId?: number;
  warehouseId?: number;
  shippingCompanyId?: number;
  effectivenessStatus?: 'NotImplemented' | 'Implemented';
  effectivenessDate?: string | null;
  effectivenessComment?: string;
  priority?: 'Low' | 'Medium' | 'High' | 'Critical';
  status?: 'Open' | 'InProgress' | 'Resolved' | 'Closed';
  occurrenceDate?: string;
  incidentDetails?: string;
  totalShipments?: number;
  defectiveItems?: number;
  occurrenceLocation?: string;
  summary?: string;
  cause?: string;
  preventionMeasures?: string;
  reportedDate?: string;
  assignedToId?: number;
  assignedDate?: string;
  resolvedDate?: string;
  closedDate?: string;
  resolution?: string;
}

// 添付ファイル関連の型定義
export interface AttachmentDto {
  id: number;
  fileName: string;
  contentType: string;
  fileSize: number;
  uploadedAt: string;
  incidentId: number;
  incidentTitle: string;
  uploadedBy: string;
}

export interface CreateAttachmentDto {
  incidentId: number;
  fileName: string;
  contentType: string;
  fileSize: number;
  uploadedById: number;
}

export interface AttachmentSearchDto {
  incidentId?: number;
  fileName?: string;
  contentType?: string;
  uploadedById?: number;
  uploadedFrom?: string;
  uploadedTo?: string;
  page: number;
  pageSize: number;
  sortBy?: string;
  ascending?: boolean;
}

// 効果測定関連の型定義
export interface EffectivenessDto {
  id: number;
  incidentId: number;
  incidentTitle: string;
  effectivenessType: string;
  beforeValue: number;
  afterValue: number;
  improvementRate: number;
  description: string;
  measuredAt: string;
  measuredBy: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateEffectivenessDto {
  incidentId: number;
  effectivenessType: string;
  beforeValue: number;
  afterValue: number;
  improvementRate: number;
  description: string;
  measuredById: number;
}

export interface UpdateEffectivenessDto {
  effectivenessType?: string;
  beforeValue?: number;
  afterValue?: number;
  improvementRate?: number;
  description?: string;
}

export interface EffectivenessSearchDto {
  incidentId?: number;
  effectivenessType?: string;
  measuredById?: number;
  measuredFrom?: string;
  measuredTo?: string;
  minImprovementRate?: number;
  maxImprovementRate?: number;
  page: number;
  pageSize: number;
  sortBy?: string;
  ascending?: boolean;
}

export interface EffectivenessSummaryDto {
  totalMeasurements: number;
  averageImprovementRate: number;
  maxImprovementRate: number;
  minImprovementRate: number;
  effectivenessTypeCounts: Record<string, number>;
  recentMeasurements: EffectivenessDto[];
}
