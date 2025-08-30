// バックエンドAPIの型定義

export type Priority = 'Low' | 'Medium' | 'High' | 'Critical';
export type IncidentStatus = 'Open' | 'InProgress' | 'Resolved' | 'Closed';

// 物流特化型定義（文字列ベース）
export type TroubleType = 'ProductTrouble' | 'DeliveryTrouble' | 'SystemTrouble' | 'EquipmentTrouble' | 'QualityTrouble' | 'LogisticsTrouble' | 'ManagementTrouble' | 'SafetyTrouble' | 'EnvironmentalTrouble' | 'OtherTrouble';
export type DamageType = 'None' | 'WrongShipment' | 'EarlyOrLateArrival' | 'Lost' | 'WrongDelivery' | 'DamageOrContamination' | 'OtherDeliveryMistake' | 'OtherProductAccident';
export type Warehouse = 'None' | 'WarehouseA' | 'WarehouseB' | 'WarehouseC';
export type ShippingCompany = 'None' | 'InHouse' | 'Charter' | 'ATransport' | 'BExpress';
export type EffectivenessStatus = 'NotImplemented' | 'Implemented';

export interface IncidentDto {
  id: number;
  title: string;
  description: string;
  status: IncidentStatus;
  priority: Priority;
  category: string;
  
  // 物流特化項目
  troubleType: TroubleType;
  damageType: DamageType;
  warehouse: Warehouse;
  shippingCompany: ShippingCompany;
  effectivenessStatus: EffectivenessStatus;
  
  // 新規追加項目（提供サイト対応）
  incidentDetails: string; // 発生経緯
  totalShipments: number; // 出荷総数
  defectiveItems: number; // 不良品数
  occurrenceDate: string; // 発生日
  occurrenceLocation: string; // 発生場所
  summary: string; // 概要
  cause: string; // 原因
  preventionMeasures: string; // 再発防止策
  effectivenessDate?: string | null; // 有効性確認日
  effectivenessComment: string; // 有効性確認コメント
  
  reportedById: number;
  reportedByName: string;
  assignedToId?: number;
  assignedToName?: string;
  reportedDate: string;
  resolvedDate?: string;
  resolution?: string;
  createdAt: string;
  updatedAt: string;
  attachmentCount: number;
  isOverdue: boolean;
  resolutionTime?: string;
}

export interface CreateIncidentDto {
  title: string;
  description: string;
  category: string;
  priority: Priority;
  
  // 物流特化項目
  troubleType: TroubleType;
  damageType: DamageType;
  warehouse: Warehouse;
  shippingCompany: ShippingCompany;
  
  // 新規追加項目（提供サイト対応）
  incidentDetails: string; // 発生経緯
  totalShipments: number; // 出荷総数
  defectiveItems: number; // 不良品数
  occurrenceDate: string; // 発生日
  occurrenceLocation: string; // 発生場所
  summary: string; // 概要
  cause: string; // 原因
  preventionMeasures: string; // 再発防止策
  effectivenessStatus: EffectivenessStatus; // 有効性評価
  effectivenessDate?: string | null; // 有効性確認日
  effectivenessComment: string; // 有効性確認コメント
  
  reportedById: number;
}

export interface UpdateIncidentDto {
  title?: string;
  description?: string;
  category?: string;
  priority?: Priority;
  
  // 物流特化項目
  troubleType?: TroubleType;
  damageType?: DamageType;
  warehouse?: Warehouse;
  shippingCompany?: ShippingCompany;
  effectivenessStatus?: EffectivenessStatus;
  
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
  status?: IncidentStatus;
  priority?: Priority;
  category?: string;
  reportedById?: number;
  assignedToId?: number;
  fromDate?: string;
  toDate?: string;
  isOverdue?: boolean;
  page: number;
  pageSize: number;
  sortBy?: string;
  ascending?: boolean;
}

export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

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

export interface ChartDataDto {
  chartType: string;
  title: string;
  series: ChartSeriesDto[];
  labels: string[];
}

export interface ChartSeriesDto {
  name: string;
  data: number[];
}

export interface PieChartDataDto {
  title: string;
  items: PieChartItemDto[];
}

export interface PieChartItemDto {
  label: string;
  value: number;
  color: string;
}

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
  description: string;
  measuredById: number;
}

export interface UpdateEffectivenessDto {
  effectivenessType: string;
  beforeValue: number;
  afterValue: number;
  description: string;
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

export interface ErrorResponse {
  traceId: string;
  message: string;
  code: string;
  errors?: Record<string, string[]>;
}
