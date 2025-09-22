import { useState } from 'react';
import { apiClient } from '@/lib/api-client';
import type { 
  WorkflowActionResultDto,
  ClassifyIncidentDto,
  AnalyzeCauseDto,
  CompleteResponseDto,
  ProposePreventionDto,
  ConfirmEffectivenessDto
} from '@/lib/types';

export function useWorkflowActions() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const executeAction = async <T>(
    action: () => Promise<T>,
    successMessage?: string
  ): Promise<T | null> => {
    try {
      setLoading(true);
      setError(null);
      const result = await action();
      
      if (successMessage) {
        // 成功メッセージの表示（実際のアプリではtoast等を使用）
        console.log(successMessage);
      }
      
      return result;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'ワークフロー操作に失敗しました';
      setError(errorMessage);
      console.error('ワークフロー操作エラー:', err);
      return null;
    } finally {
      setLoading(false);
    }
  };

  const enableWorkflow = async (incidentId: number): Promise<WorkflowActionResultDto | null> => {
    return executeAction(
      () => apiClient.enableWorkflow(incidentId),
      'ワークフローが有効化されました'
    );
  };

  const classifyIncident = async (
    incidentId: number, 
    data: Omit<ClassifyIncidentDto, 'incidentId'>
  ): Promise<WorkflowActionResultDto | null> => {
    return executeAction(
      () => apiClient.classifyIncident(incidentId, data),
      'インシデントが分類されました'
    );
  };

  const startResponse = async (incidentId: number): Promise<WorkflowActionResultDto | null> => {
    return executeAction(
      () => apiClient.startResponse(incidentId),
      '対応を開始しました'
    );
  };

  const analyzeCause = async (incidentId: number, cause: string): Promise<WorkflowActionResultDto | null> => {
    return executeAction(
      () => apiClient.analyzeCause(incidentId, cause),
      '原因が入力されました'
    );
  };

  const completeResponse = async (incidentId: number, responseContent: string): Promise<WorkflowActionResultDto | null> => {
    return executeAction(
      () => apiClient.completeResponse(incidentId, responseContent),
      '対応が完了しました'
    );
  };

  const proposePrevention = async (incidentId: number, preventionMeasures: string): Promise<WorkflowActionResultDto | null> => {
    return executeAction(
      () => apiClient.proposePrevention(incidentId, preventionMeasures),
      '再発防止策が提案されました'
    );
  };

  const confirmEffectiveness = async (
    incidentId: number, 
    effectivenessStatus: string, 
    effectivenessComment: string
  ): Promise<WorkflowActionResultDto | null> => {
    return executeAction(
      () => apiClient.confirmEffectiveness(incidentId, effectivenessStatus, effectivenessComment),
      '有効性が確認されました'
    );
  };

  return {
    loading,
    error,
    enableWorkflow,
    classifyIncident,
    startResponse,
    analyzeCause,
    completeResponse,
    proposePrevention,
    confirmEffectiveness,
    clearError: () => setError(null)
  };
}
