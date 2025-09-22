import { useState, useEffect } from 'react';
import { apiClient } from '@/lib/api-client';
import type { WorkflowStatisticsDto } from '@/lib/types';

interface UseWorkflowStatisticsOptions {
  enabled?: boolean;
  refetchInterval?: number;
}

export function useWorkflowStatistics(options: UseWorkflowStatisticsOptions = {}) {
  const { enabled = true, refetchInterval } = options;
  
  const [data, setData] = useState<WorkflowStatisticsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchWorkflowStatistics = async () => {
    if (!enabled) return;
    
    try {
      setLoading(true);
      const result = await apiClient.getWorkflowStatistics();
      setData(result);
      setError(null);
    } catch (err) {
      console.error('ワークフロー統計取得エラー:', err);
      setError(err instanceof Error ? err.message : 'ワークフロー統計の取得に失敗しました');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchWorkflowStatistics();

    // 定期更新の設定
    if (refetchInterval && refetchInterval > 0) {
      const interval = setInterval(fetchWorkflowStatistics, refetchInterval);
      return () => clearInterval(interval);
    }
  }, [enabled, refetchInterval]);

  const refetch = () => {
    fetchWorkflowStatistics();
  };

  return {
    data,
    loading,
    error,
    refetch
  };
}
