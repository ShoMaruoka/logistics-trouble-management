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
	async getIncidents(searchParams?: IncidentSearchDto): Promise<PagedResultDto<IncidentDto>> {
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
		
		return this.request<PagedResultDto<IncidentDto>>(endpoint);
	}

	async getIncident(id: number): Promise<IncidentDto> {
		return this.request<IncidentDto>(`/api/incidents/${id}`);
	}

	async createIncident(data: CreateIncidentDto): Promise<IncidentDto> {
		return this.request<IncidentDto>('/api/incidents', {
			method: 'POST',
			body: JSON.stringify(data),
		});
	}

	async updateIncident(id: number, data: UpdateIncidentDto): Promise<IncidentDto> {
		return this.request<IncidentDto>(`/api/incidents/${id}`, {
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
	async getStatisticsSummary(year?: number): Promise<StatisticsSummaryDto> {
		const params = new URLSearchParams();
		if (year) {
			params.append('year', year.toString());
		}
		const queryString = params.toString();
		const endpoint = `/api/statistics/summary${queryString ? `?${queryString}` : ''}`;
		return this.request<StatisticsSummaryDto>(endpoint);
	}

	async getDamageTypesChart(year?: number): Promise<PieChartDataDto> {
		const params = new URLSearchParams();
		if (year) {
			params.append('year', year.toString());
		}
		const queryString = params.toString();
		const endpoint = `/api/statistics/charts/damage-types${queryString ? `?${queryString}` : ''}`;
		return this.request<PieChartDataDto>(endpoint);
	}

	async getTroubleTypesChart(year?: number): Promise<PieChartDataDto> {
		const params = new URLSearchParams();
		if (year) {
			params.append('year', year.toString());
		}
		const queryString = params.toString();
		const endpoint = `/api/statistics/charts/trouble-types${queryString ? `?${queryString}` : ''}`;
		return this.request<PieChartDataDto>(endpoint);
	}

	async getMonthlyIncidentsChart(year?: number): Promise<ChartDataDto> {
		const params = new URLSearchParams();
		if (year) {
			params.append('year', year.toString());
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

	// ヘルスチェック
	async healthCheck(): Promise<{ status: string; timestamp: string }> {
		return this.request<{ status: string; timestamp: string }>('/health');
	}
}

// 型インポート
import type {
	IncidentDto,
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
} from './api-types';

// シングルトンインスタンス
export const apiClient = new ApiClient();
export { ApiError };
