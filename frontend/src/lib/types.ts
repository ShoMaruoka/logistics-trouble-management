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
  id: string;
  title: string;
  description: string;
  category: string;
  reportedById: number;
  troubleTypeId: number;
  damageTypeId: number;
  warehouseId: number;
  shippingCompanyId: number;
  effectivenessStatus: 'NotImplemented' | 'InProgress' | 'Completed';
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  status: 'Open' | 'InProgress' | 'Resolved' | 'Closed';
  occurrenceDate: string;
  incidentDetails?: string;
  totalShipments?: number;
  defectiveItems?: number;
  occurrenceLocation?: string;
  summary?: string;
  cause?: string;
  preventionMeasures?: string;
  reportedDate: string;
  assignedToId?: number;
  assignedDate?: string;
  resolvedDate?: string;
  closedDate?: string;
  resolution?: string;
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

// 既存の型定義（後方互換性のため）
export interface StatisticsSummaryDto {
  totalIncidents: number;
  openCount: number;
  resolvedCount: number;
  averageResolutionTime: number;
}

export interface ChartDataDto {
  label: string;
  value: number;
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
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface IncidentSearchDto {
  searchTerm?: string;
  status?: string;
  priority?: string;
  troubleTypeId?: number;
  damageTypeId?: number;
  warehouseId?: number;
  shippingCompanyId?: number;
  startDate?: string;
  endDate?: string;
  pageNumber: number;
  pageSize: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}
