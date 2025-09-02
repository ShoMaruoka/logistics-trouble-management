// APIクライアント

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5169';

class ApiError extends Error {
	constructor(
		public status: number,
		public message: string,
		public code: string,
		public errors?: Record<string, string[]>
	) {
		super(message);
		this.name = 'ApiError';
	}
}

class ApiClient {
	private async request<T>(
		endpoint: string,
		options: RequestInit = {}
	): Promise<T> {
		const url = `${API_BASE_URL}${endpoint}`;
		
		const config: RequestInit = {
			headers: {
				'Content-Type': 'application/json',
				...options.headers,
			},
			...options,
		};

		try {
			const response = await fetch(url, config);
			
			if (!response.ok) {
				let errorData: unknown = {};
				try {
					errorData = await response.json();
				} catch {
					// JSON解析に失敗した場合のフォールバック
					errorData = {
						message: `HTTP ${response.status}: ${response.statusText}`,
						code: 'HTTP_ERROR'
					} as { message: string; code: string };
				}

				const parsed = (errorData ?? {}) as Partial<{ message: string; code: string; errors?: Record<string, string[]> }>;

				console.error('APIエラー詳細:', {
					status: response.status,
					statusText: response.statusText,
					errorData: parsed
				});
				throw new ApiError(
					response.status,
					parsed.message || 'API request failed',
					parsed.code || 'UNKNOWN_ERROR',
					parsed.errors
				);
			}

			// 204 No Contentの場合は空のオブジェクトを返す
			if (response.status === 204) {
				return {} as T;
			}

			return (await response.json()) as T;
		} catch (error) {
			if (error instanceof ApiError) {
				throw error;
			}
			
			// ネットワークエラーなどの場合
			const message = error instanceof Error ? error.message : 'Network error';
			throw new ApiError(0, message, 'NETWORK_ERROR');
		}
	}

	// ファイルダウンロード用の特別なリクエストメソッド
	private async downloadRequest(endpoint: string): Promise<Blob> {
		const url = `${API_BASE_URL}${endpoint}`;
		
		try {
			const response = await fetch(url, {
				method: 'GET',
			});
			
			if (!response.ok) {
				throw new ApiError(
					response.status,
					`Download failed: ${response.statusText}`,
					'DOWNLOAD_ERROR'
				);
			}

			return await response.blob();
		} catch (error) {
			if (error instanceof ApiError) {
				throw error;
			}
			
			const message = error instanceof Error ? error.message : 'Download network error';
			throw new ApiError(0, message, 'NETWORK_ERROR');
		}
	}

	// インシデント関連API
	async getIncidents(searchParams?: IncidentSearchDto): Promise<PagedResultDto<Incident>> {
		const params = new URLSearchParams();
		if (searchParams) {
			(Object.entries(searchParams) as Array<[keyof IncidentSearchDto, IncidentSearchDto[keyof IncidentSearchDto]]>).forEach(([key, value]) => {
				if (value !== undefined && value !== null) {
					params.append(String(key), String(value));
				}
			});
		}
		
		const queryString = params.toString();
		const endpoint = `/api/incidents${queryString ? `?${queryString}` : ''}`;
		
		return this.request<PagedResultDto<Incident>>(endpoint);
	}

	async getIncident(id: number): Promise<Incident> {
		return this.request<Incident>(`/api/incidents/${id}`);
	}

	async createIncident(data: CreateIncidentDto): Promise<Incident> {
		// 開発環境でのみログ出力（PII保護）
		if (process.env.NODE_ENV === 'development') {
			// センシティブなフィールドをサニタイズ
			const sanitizedData = {
				...data,
				incidentDetails: data.incidentDetails ? '[REDACTED]' : undefined,
				occurrenceLocation: data.occurrenceLocation ? '[REDACTED]' : undefined,
				summary: data.summary ? '[REDACTED]' : undefined,
				cause: data.cause ? '[REDACTED]' : undefined,
				preventionMeasures: data.preventionMeasures ? '[REDACTED]' : undefined,
			};
			console.log('送信データ（サニタイズ済み）:', JSON.stringify(sanitizedData, null, 2));
		}
		return this.request<Incident>('/api/incidents', {
			method: 'POST',
			body: JSON.stringify(data),
		});
	}

	async updateIncident(id: number, data: UpdateIncidentDto): Promise<Incident> {
		return this.request<Incident>(`/api/incidents/${id}`, {
			method: 'PUT',
			body: JSON.stringify(data),
		});
	}

	async deleteIncident(id: number): Promise<void> {
		return this.request<void>(`/api/incidents/${id}`, {
			method: 'DELETE',
		});
	}

	// 統計関連API
	async getStatisticsSummary(year?: number, month?: number): Promise<StatisticsSummaryDto> {
		const params = new URLSearchParams();
		if (year) {
			params.append('year', year.toString());
		}
		if (month) {
			params.append('month', month.toString());
		}
		const queryString = params.toString();
		const endpoint = `/api/statistics/summary${queryString ? `?${queryString}` : ''}`;
		return this.request<StatisticsSummaryDto>(endpoint);
	}

	async getDamageTypesChart(year?: number, month?: number): Promise<PieChartDataDto> {
		const params = new URLSearchParams();
		if (year) {
			params.append('year', year.toString());
		}
		if (month) {
			params.append('month', month.toString());
		}
		const queryString = params.toString();
		const endpoint = `/api/statistics/charts/damage-types${queryString ? `?${queryString}` : ''}`;
		return this.request<PieChartDataDto>(endpoint);
	}

	async getTroubleTypesChart(year?: number, month?: number): Promise<PieChartDataDto> {
		const params = new URLSearchParams();
		if (year) {
			params.append('year', year.toString());
		}
		if (month) {
			params.append('month', month.toString());
		}
		const queryString = params.toString();
		const endpoint = `/api/statistics/charts/trouble-types${queryString ? `?${queryString}` : ''}`;
		return this.request<PieChartDataDto>(endpoint);
	}

	async getMonthlyIncidentsChart(year?: number, month?: number): Promise<ChartDataDto> {
		const params = new URLSearchParams();
		if (year) {
			params.append('year', year.toString());
		}
		if (month) {
			params.append('month', month.toString());
		}
		const queryString = params.toString();
		const endpoint = `/api/statistics/charts/monthly-incidents${queryString ? `?${queryString}` : ''}`;
		return this.request<ChartDataDto>(endpoint);
	}

	// ファイル管理API
	async getAttachments(searchParams?: AttachmentSearchDto): Promise<PagedResultDto<AttachmentDto>> {
		const params = new URLSearchParams();
		if (searchParams) {
			(Object.entries(searchParams) as Array<[keyof AttachmentSearchDto, AttachmentSearchDto[keyof AttachmentSearchDto]]>).forEach(([key, value]) => {
				if (value !== undefined && value !== null) {
					params.append(String(key), String(value));
				}
			});
		}
		
		const queryString = params.toString();
		const endpoint = `/api/attachments${queryString ? `?${queryString}` : ''}`;
		
		return this.request<PagedResultDto<AttachmentDto>>(endpoint);
	}

	async getAttachmentsByIncident(incidentId: number): Promise<AttachmentDto[]> {
		return this.request<AttachmentDto[]>(`/api/attachments/incident/${incidentId}`);
	}

	async createAttachment(data: CreateAttachmentDto): Promise<AttachmentDto> {
		return this.request<AttachmentDto>('/api/attachments', {
			method: 'POST',
			body: JSON.stringify(data),
		});
	}

	async deleteAttachment(id: number): Promise<void> {
		return this.request<void>(`/api/attachments/${id}`, {
			method: 'DELETE',
		});
	}

	// ファイルダウンロードAPI
	async downloadAttachment(id: number): Promise<Blob> {
		return this.downloadRequest(`/api/attachments/${id}/download`);
	}

	// 効果測定API
	async getEffectiveness(searchParams?: EffectivenessSearchDto): Promise<PagedResultDto<EffectivenessDto>> {
		const params = new URLSearchParams();
		if (searchParams) {
			(Object.entries(searchParams) as Array<[keyof EffectivenessSearchDto, EffectivenessSearchDto[keyof EffectivenessSearchDto]]>).forEach(([key, value]) => {
				if (value !== undefined && value !== null) {
					params.append(String(key), String(value));
				}
			});
		}
		
		const queryString = params.toString();
		const endpoint = `/api/effectiveness${queryString ? `?${queryString}` : ''}`;
		
		return this.request<PagedResultDto<EffectivenessDto>>(endpoint);
	}

	async getEffectivenessById(id: number): Promise<EffectivenessDto> {
		return this.request<EffectivenessDto>(`/api/effectiveness/${id}`);
	}

	async getEffectivenessByIncident(incidentId: number): Promise<EffectivenessDto[]> {
		return this.request<EffectivenessDto[]>(`/api/effectiveness/incident/${incidentId}`);
	}

	async getEffectivenessSummary(): Promise<EffectivenessSummaryDto> {
		return this.request<EffectivenessSummaryDto>('/api/effectiveness/summary');
	}

	async createEffectiveness(data: CreateEffectivenessDto): Promise<EffectivenessDto> {
		return this.request<EffectivenessDto>('/api/effectiveness', {
			method: 'POST',
			body: JSON.stringify(data),
		});
	}

	async updateEffectiveness(id: number, data: UpdateEffectivenessDto): Promise<EffectivenessDto> {
		return this.request<EffectivenessDto>(`/api/effectiveness/${id}`, {
			method: 'PUT',
			body: JSON.stringify(data),
		});
	}

	async deleteEffectiveness(id: number): Promise<void> {
		return this.request<void>(`/api/effectiveness/${id}`, {
			method: 'DELETE',
		});
	}

	// マスタデータ関連API
	// トラブル種類マスタ
	async getTroubleTypes(): Promise<TroubleType[]> {
		return this.request<TroubleType[]>('/api/troubletypes');
	}

	async getActiveTroubleTypes(): Promise<TroubleType[]> {
		return this.request<TroubleType[]>('/api/troubletypes/active');
	}

	async getTroubleType(id: number): Promise<TroubleType> {
		return this.request<TroubleType>(`/api/troubletypes/${id}`);
	}

	async createTroubleType(data: CreateTroubleTypeDto): Promise<TroubleType> {
		return this.request<TroubleType>('/api/troubletypes', {
			method: 'POST',
			body: JSON.stringify(data),
		});
	}

	async updateTroubleType(id: number, data: UpdateTroubleTypeDto): Promise<void> {
		return this.request<void>(`/api/troubletypes/${id}`, {
			method: 'PUT',
			body: JSON.stringify(data),
		});
	}

	async deleteTroubleType(id: number): Promise<void> {
		return this.request<void>(`/api/troubletypes/${id}`, {
			method: 'DELETE',
		});
	}

	// 損傷種類マスタ
	async getDamageTypes(): Promise<DamageType[]> {
		return this.request<DamageType[]>('/api/damagetypes');
	}

	async getActiveDamageTypes(): Promise<DamageType[]> {
		return this.request<DamageType[]>('/api/damagetypes/active');
	}

	async getDamageTypesByCategory(category: string): Promise<DamageType[]> {
		return this.request<DamageType[]>(`/api/damagetypes/category/${category}`);
	}

	async getDamageType(id: number): Promise<DamageType> {
		return this.request<DamageType>(`/api/damagetypes/${id}`);
	}

	async createDamageType(data: CreateDamageTypeDto): Promise<DamageType> {
		return this.request<DamageType>('/api/damagetypes', {
			method: 'POST',
			body: JSON.stringify(data),
		});
	}

	async updateDamageType(id: number, data: UpdateDamageTypeDto): Promise<void> {
		return this.request<void>(`/api/damagetypes/${id}`, {
			method: 'PUT',
			body: JSON.stringify(data),
		});
	}

	async deleteDamageType(id: number): Promise<void> {
		return this.request<void>(`/api/damagetypes/${id}`, {
			method: 'DELETE',
		});
	}

	// 出荷元倉庫マスタ
	async getWarehouses(): Promise<Warehouse[]> {
		return this.request<Warehouse[]>('/api/warehouses');
	}

	async getActiveWarehouses(): Promise<Warehouse[]> {
		return this.request<Warehouse[]>('/api/warehouses/active');
	}

	async getWarehouse(id: number): Promise<Warehouse> {
		return this.request<Warehouse>(`/api/warehouses/${id}`);
	}

	async createWarehouse(data: CreateWarehouseDto): Promise<Warehouse> {
		return this.request<Warehouse>('/api/warehouses', {
			method: 'POST',
			body: JSON.stringify(data),
		});
	}

	async updateWarehouse(id: number, data: UpdateWarehouseDto): Promise<void> {
		return this.request<void>(`/api/warehouses/${id}`, {
			method: 'PUT',
			body: JSON.stringify(data),
		});
	}

	async deleteWarehouse(id: number): Promise<void> {
		return this.request<void>(`/api/warehouses/${id}`, {
			method: 'DELETE',
		});
	}

	// 運送会社マスタ
	async getShippingCompanies(): Promise<ShippingCompany[]> {
		return this.request<ShippingCompany[]>('/api/shippingcompanies');
	}

	async getActiveShippingCompanies(): Promise<ShippingCompany[]> {
		return this.request<ShippingCompany[]>('/api/shippingcompanies/active');
	}

	async getShippingCompaniesByCompanyType(companyType: string): Promise<ShippingCompany[]> {
		return this.request<ShippingCompany[]>(`/api/shippingcompanies/company-type/${companyType}`);
	}

	async getShippingCompany(id: number): Promise<ShippingCompany> {
		return this.request<ShippingCompany>(`/api/shippingcompanies/${id}`);
	}

	async createShippingCompany(data: CreateShippingCompanyDto): Promise<ShippingCompany> {
		return this.request<ShippingCompany>('/api/shippingcompanies', {
			method: 'POST',
			body: JSON.stringify(data),
		});
	}

	async updateShippingCompany(id: number, data: UpdateShippingCompanyDto): Promise<void> {
		return this.request<void>(`/api/shippingcompanies/${id}`, {
			method: 'PUT',
			body: JSON.stringify(data),
		});
	}

	async deleteShippingCompany(id: number): Promise<void> {
		return this.request<void>(`/api/shippingcompanies/${id}`, {
			method: 'DELETE',
		});
	}

	// ヘルスチェック
	async healthCheck(): Promise<{ status: string; timestamp: string }> {
		return this.request<{ status: string; timestamp: string }>('/health');
	}
}

// 型インポート
import type {
	Incident,
	CreateIncidentDto,
	UpdateIncidentDto,
	IncidentSearchDto,
	PagedResultDto,
	StatisticsSummaryDto,
	ChartDataDto,
	PieChartDataDto,
	AttachmentDto,
	CreateAttachmentDto,
	AttachmentSearchDto,
	EffectivenessDto,
	CreateEffectivenessDto,
	UpdateEffectivenessDto,
	EffectivenessSearchDto,
	EffectivenessSummaryDto,
	// マスタデータ型
	TroubleType,
	DamageType,
	Warehouse,
	ShippingCompany,
	CreateTroubleTypeDto,
	UpdateTroubleTypeDto,
	CreateDamageTypeDto,
	UpdateDamageTypeDto,
	CreateWarehouseDto,
	UpdateWarehouseDto,
	CreateShippingCompanyDto,
	UpdateShippingCompanyDto,
} from './types';

// シングルトンインスタンス
export const apiClient = new ApiClient();
export { ApiError };
