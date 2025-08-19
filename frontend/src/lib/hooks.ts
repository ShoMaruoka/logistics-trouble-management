// React Hooks for API calls

import { useState, useEffect, useCallback } from 'react';
import { apiClient, ApiError } from './api-client';
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

// 共通の状態管理
interface ApiState<T> {
	data: T | null;
	loading: boolean;
	error: string | null;
}

// インシデント関連Hooks
export function useIncidents(searchParams?: IncidentSearchDto) {
	const [state, setState] = useState<ApiState<PagedResultDto<IncidentDto>>>({
		data: null,
		loading: true,
		error: null,
	});

	const fetchIncidents = useCallback(async () => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const data = await apiClient.getIncidents(searchParams);
			setState({ data, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : 'インシデントの取得に失敗しました';
			setState({ data: null, loading: false, error: message });
		}
	}, [searchParams]);

	useEffect(() => {
		fetchIncidents();
	}, [fetchIncidents]);

	return { ...state, refetch: fetchIncidents };
}

export function useIncident(id: number) {
	const [state, setState] = useState<ApiState<IncidentDto>>({
		data: null,
		loading: true,
		error: null,
	});

	const fetchIncident = useCallback(async () => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const data = await apiClient.getIncident(id);
			setState({ data, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : 'インシデントの取得に失敗しました';
			setState({ data: null, loading: false, error: message });
		}
	}, [id]);

	useEffect(() => {
		if (id) {
			fetchIncident();
		}
	}, [fetchIncident]);

	return { ...state, refetch: fetchIncident };
}

export function useCreateIncident() {
	const [state, setState] = useState<ApiState<IncidentDto>>({
		data: null,
		loading: false,
		error: null,
	});

	const createIncident = useCallback(async (data: CreateIncidentDto) => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const result = await apiClient.createIncident(data);
			setState({ data: result, loading: false, error: null });
			return result;
		} catch (error) {
			const message = error instanceof ApiError ? error.message : 'インシデントの作成に失敗しました';
			setState({ data: null, loading: false, error: message });
			throw error;
		}
	}, []);

	return { ...state, createIncident };
}

export function useUpdateIncident() {
	const [state, setState] = useState<ApiState<IncidentDto>>({
		data: null,
		loading: false,
		error: null,
	});

	const updateIncident = useCallback(async (id: number, data: UpdateIncidentDto) => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const result = await apiClient.updateIncident(id, data);
			setState({ data: result, loading: false, error: null });
			return result;
		} catch (error) {
			const message = error instanceof ApiError ? error.message : 'インシデントの更新に失敗しました';
			setState({ data: null, loading: false, error: message });
			throw error;
		}
	}, []);

	return { ...state, updateIncident };
}

export function useDeleteIncident() {
	const [state, setState] = useState<ApiState<void>>({
		data: null,
		loading: false,
		error: null,
	});

	const deleteIncident = useCallback(async (id: number) => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			await apiClient.deleteIncident(id);
			setState({ data: null, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : 'インシデントの削除に失敗しました';
			setState({ data: null, loading: false, error: message });
			throw error;
		}
	}, []);

	return { ...state, deleteIncident };
}

// 統計関連Hooks
export function useStatisticsSummary(year?: number) {
	const [state, setState] = useState<ApiState<StatisticsSummaryDto>>({
		data: null,
		loading: true,
		error: null,
	});

	const fetchSummary = useCallback(async () => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const data = await apiClient.getStatisticsSummary(year);
			setState({ data, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : '統計情報の取得に失敗しました';
			setState({ data: null, loading: false, error: message });
		}
	}, [year]);

	useEffect(() => {
		fetchSummary();
	}, [fetchSummary]);

	return { ...state, refetch: fetchSummary };
}

export function useDamageTypesChart(year?: number) {
	const [state, setState] = useState<ApiState<PieChartDataDto>>({
		data: null,
		loading: true,
		error: null,
	});

	const fetchChart = useCallback(async () => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const data = await apiClient.getDamageTypesChart(year); // Pass year here
			setState({ data, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : '損傷種類チャートの取得に失敗しました';
			setState({ data: null, loading: false, error: message });
		}
	}, [year]); // Added year to dependency array

	useEffect(() => {
		fetchChart();
	}, [fetchChart]);

	return { ...state, refetch: fetchChart };
}

export function useTroubleTypesChart(year?: number) {
	const [state, setState] = useState<ApiState<PieChartDataDto>>({
		data: null,
		loading: true,
		error: null,
	});

	const fetchChart = useCallback(async () => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const data = await apiClient.getTroubleTypesChart(year); // Pass year here
			setState({ data, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : 'トラブル種類チャートの取得に失敗しました';
			setState({ data: null, loading: false, error: message });
		}
	}, [year]); // Added year to dependency array

	useEffect(() => {
		fetchChart();
	}, [fetchChart]);

	return { ...state, refetch: fetchChart };
}

export function useMonthlyIncidentsChart(year?: number) {
	const [state, setState] = useState<ApiState<ChartDataDto>>({
		data: null,
		loading: true,
		error: null,
	});

	const fetchChart = useCallback(async () => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const data = await apiClient.getMonthlyIncidentsChart(year); // Pass year here
			setState({ data, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : '月間インシデントチャートの取得に失敗しました';
			setState({ data: null, loading: false, error: message });
		}
	}, [year]); // Added year to dependency array

	useEffect(() => {
		fetchChart();
	}, [fetchChart]);

	return { ...state, refetch: fetchChart };
}

// ファイル管理関連Hooks
export function useAttachments(searchParams?: AttachmentSearchDto) {
	const [state, setState] = useState<ApiState<PagedResultDto<AttachmentDto>>>({
		data: null,
		loading: true,
		error: null,
	});

	const fetchAttachments = useCallback(async () => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const data = await apiClient.getAttachments(searchParams);
			setState({ data, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : '添付ファイルの取得に失敗しました';
			setState({ data: null, loading: false, error: message });
		}
	}, [searchParams]);

	useEffect(() => {
		fetchAttachments();
	}, [fetchAttachments]);

	return { ...state, refetch: fetchAttachments };
}

export function useAttachmentsByIncident(incidentId: number) {
	const [state, setState] = useState<ApiState<AttachmentDto[]>>({
		data: null,
		loading: true,
		error: null,
	});

	const fetchAttachments = useCallback(async () => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const data = await apiClient.getAttachmentsByIncident(incidentId);
			setState({ data, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : 'インシデントの添付ファイル取得に失敗しました';
			setState({ data: null, loading: false, error: message });
		}
	}, [incidentId]);

	useEffect(() => {
		if (incidentId) {
			fetchAttachments();
		}
	}, [fetchAttachments]);

	return { ...state, refetch: fetchAttachments };
}

export function useCreateAttachment() {
	const [state, setState] = useState<ApiState<AttachmentDto>>({
		data: null,
		loading: false,
		error: null,
	});

	const createAttachment = useCallback(async (data: CreateAttachmentDto) => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const result = await apiClient.createAttachment(data);
			setState({ data: result, loading: false, error: null });
			return result;
		} catch (error) {
			const message = error instanceof ApiError ? error.message : '添付ファイルの作成に失敗しました';
			setState({ data: null, loading: false, error: message });
			throw error;
		}
	}, []);

	return { ...state, createAttachment };
}

export function useDeleteAttachment() {
	const [state, setState] = useState<ApiState<void>>({
		data: null,
		loading: false,
		error: null,
	});

	const deleteAttachment = useCallback(async (id: number) => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			await apiClient.deleteAttachment(id);
			setState({ data: null, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : '添付ファイルの削除に失敗しました';
			setState({ data: null, loading: false, error: message });
			throw error;
		}
	}, []);

	return { ...state, deleteAttachment };
}

export function useDownloadAttachment() {
	const [state, setState] = useState<ApiState<Blob>>({
		data: null,
		loading: false,
		error: null,
	});

	const downloadAttachment = useCallback(async (id: number, fileName: string) => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const blob = await apiClient.downloadAttachment(id);
			
			// ブラウザでダウンロード
			const url = window.URL.createObjectURL(blob);
			const a = document.createElement('a');
			a.href = url;
			a.download = fileName;
			document.body.appendChild(a);
			a.click();
			window.URL.revokeObjectURL(url);
			document.body.removeChild(a);
			
			setState({ data: blob, loading: false, error: null });
			return blob;
		} catch (error) {
			const message = error instanceof ApiError ? error.message : 'ファイルのダウンロードに失敗しました';
			setState({ data: null, loading: false, error: message });
			throw error;
		}
	}, []);

	return { ...state, downloadAttachment };
}

// 効果測定関連Hooks
export function useEffectiveness(searchParams?: EffectivenessSearchDto) {
	const [state, setState] = useState<ApiState<PagedResultDto<EffectivenessDto>>>({
		data: null,
		loading: true,
		error: null,
	});

	const fetchEffectiveness = useCallback(async () => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const data = await apiClient.getEffectiveness(searchParams);
			setState({ data, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : '効果測定の取得に失敗しました';
			setState({ data: null, loading: false, error: message });
		}
	}, [searchParams]);

	useEffect(() => {
		fetchEffectiveness();
	}, [fetchEffectiveness]);

	return { ...state, refetch: fetchEffectiveness };
}

export function useEffectivenessById(id: number) {
	const [state, setState] = useState<ApiState<EffectivenessDto>>({
		data: null,
		loading: true,
		error: null,
	});

	const fetchEffectiveness = useCallback(async () => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const data = await apiClient.getEffectivenessById(id);
			setState({ data, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : '効果測定の取得に失敗しました';
			setState({ data: null, loading: false, error: message });
		}
	}, [id]);

	useEffect(() => {
		if (id) {
			fetchEffectiveness();
		}
	}, [fetchEffectiveness]);

	return { ...state, refetch: fetchEffectiveness };
}

export function useEffectivenessByIncident(incidentId: number) {
	const [state, setState] = useState<ApiState<EffectivenessDto[]>>({
		data: null,
		loading: true,
		error: null,
	});

	const fetchEffectiveness = useCallback(async () => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const data = await apiClient.getEffectivenessByIncident(incidentId);
			setState({ data, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : 'インシデントの効果測定取得に失敗しました';
			setState({ data: null, loading: false, error: message });
		}
	}, [incidentId]);

	useEffect(() => {
		if (incidentId) {
			fetchEffectiveness();
		}
	}, [fetchEffectiveness]);

	return { ...state, refetch: fetchEffectiveness };
}

export function useEffectivenessSummary() {
	const [state, setState] = useState<ApiState<EffectivenessSummaryDto>>({
		data: null,
		loading: true,
		error: null,
	});

	const fetchSummary = useCallback(async () => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const data = await apiClient.getEffectivenessSummary();
			setState({ data, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : '効果測定サマリーの取得に失敗しました';
			setState({ data: null, loading: false, error: message });
		}
	}, []);

	useEffect(() => {
		fetchSummary();
	}, [fetchSummary]);

	return { ...state, refetch: fetchSummary };
}

export function useCreateEffectiveness() {
	const [state, setState] = useState<ApiState<EffectivenessDto>>({
		data: null,
		loading: false,
		error: null,
	});

	const createEffectiveness = useCallback(async (data: CreateEffectivenessDto) => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const result = await apiClient.createEffectiveness(data);
			setState({ data: result, loading: false, error: null });
			return result;
		} catch (error) {
			const message = error instanceof ApiError ? error.message : '効果測定の作成に失敗しました';
			setState({ data: null, loading: false, error: message });
			throw error;
		}
	}, []);

	return { ...state, createEffectiveness };
}

export function useUpdateEffectiveness() {
	const [state, setState] = useState<ApiState<EffectivenessDto>>({
		data: null,
		loading: false,
		error: null,
	});

	const updateEffectiveness = useCallback(async (id: number, data: UpdateEffectivenessDto) => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const result = await apiClient.updateEffectiveness(id, data);
			setState({ data: result, loading: false, error: null });
			return result;
		} catch (error) {
			const message = error instanceof ApiError ? error.message : '効果測定の更新に失敗しました';
			setState({ data: null, loading: false, error: message });
			throw error;
		}
	}, []);

	return { ...state, updateEffectiveness };
}

export function useDeleteEffectiveness() {
	const [state, setState] = useState<ApiState<void>>({
		data: null,
		loading: false,
		error: null,
	});

	const deleteEffectiveness = useCallback(async (id: number) => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			await apiClient.deleteEffectiveness(id);
			setState({ data: null, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : '効果測定の削除に失敗しました';
			setState({ data: null, loading: false, error: message });
			throw error;
		}
	}, []);

	return { ...state, deleteEffectiveness };
}

// ヘルスチェック
export function useHealthCheck() {
	const [state, setState] = useState<ApiState<{ status: string; timestamp: string }>>({
		data: null,
		loading: true,
		error: null,
	});

	const checkHealth = useCallback(async () => {
		try {
			setState(prev => ({ ...prev, loading: true, error: null }));
			const data = await apiClient.healthCheck();
			setState({ data, loading: false, error: null });
		} catch (error) {
			const message = error instanceof ApiError ? error.message : 'ヘルスチェックに失敗しました';
			setState({ data: null, loading: false, error: message });
		}
	}, []);

	useEffect(() => {
		checkHealth();
	}, [checkHealth]);

	return { ...state, refetch: checkHealth };
}
