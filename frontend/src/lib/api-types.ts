// バックエンドAPIの型定義

export interface IncidentDto {
  id: number;
  title: string;
  description: string;
  status: 'Open' | 'InProgress' | 'Resolved' | 'Closed';
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  category: string;
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
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  reportedById: number;
}

export interface UpdateIncidentDto {
  title?: string;
  description?: string;
  category?: string;
  priority?: 'Low' | 'Medium' | 'High' | 'Critical';
  assignedToId?: number;
  resolution?: string;
}

export interface IncidentSearchDto {
  title?: string;
  status?: 'Open' | 'InProgress' | 'Resolved' | 'Closed';
  priority?: 'Low' | 'Medium' | 'High' | 'Critical';
  category?: string;
  reportedById?: number;
  assignedToId?: number;
  reportedFrom?: string;
  reportedTo?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  ascending?: boolean;
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

export interface StatisticsSummaryDto {
  totalIncidents: number;
  openIncidents: number;
  resolvedIncidents: number;
  averageResolutionTime: number;
  incidentsByPriority: Record<string, number>;
  incidentsByStatus: Record<string, number>;
  recentIncidents: IncidentDto[];
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
  page?: number;
  pageSize?: number;
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
  page?: number;
  pageSize?: number;
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
