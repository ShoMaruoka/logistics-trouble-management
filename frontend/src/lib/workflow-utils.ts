import type { 
  WorkflowStatus, 
  IncidentStatus, 
  WorkflowMode,
  IncidentWithWorkflow,
  Incident
} from './types';

/**
 * ワークフローステータスの表示ラベルを取得
 */
export function getWorkflowStatusLabel(status: WorkflowStatus): string {
  switch (status) {
    case 'Unclassified': return '未分類';
    case 'Pending': return '未対応';
    case 'InProgress': return '対応中';
    case 'Completed': return '対応済';
    case 'PreventionProposed': return '再発防止策提案済';
    case 'EffectivenessConfirmed': return '有効性確認済';
    default: return '不明';
  }
}

/**
 * ワークフローステータスの色を取得
 */
export function getWorkflowStatusColor(status: WorkflowStatus): string {
  switch (status) {
    case 'Unclassified': return 'bg-gray-500';
    case 'Pending': return 'bg-red-500';
    case 'InProgress': return 'bg-orange-500';
    case 'Completed': return 'bg-blue-500';
    case 'PreventionProposed': return 'bg-purple-500';
    case 'EffectivenessConfirmed': return 'bg-green-500';
    default: return 'bg-gray-500';
  }
}

/**
 * ワークフローステータスのテキスト色を取得
 */
export function getWorkflowStatusTextColor(status: WorkflowStatus): string {
  switch (status) {
    case 'Unclassified': return 'text-gray-500';
    case 'Pending': return 'text-red-500';
    case 'InProgress': return 'text-orange-500';
    case 'Completed': return 'text-blue-500';
    case 'PreventionProposed': return 'text-purple-500';
    case 'EffectivenessConfirmed': return 'text-green-500';
    default: return 'text-gray-500';
  }
}

/**
 * レガシーステータスから新ワークフローステータスへのマッピング
 */
export function mapLegacyToWorkflowStatus(legacyStatus: IncidentStatus, category?: string): WorkflowStatus {
  switch (legacyStatus) {
    case 'Open':
      return category ? 'Pending' : 'Unclassified';
    case 'InProgress':
      return 'InProgress';
    case 'Resolved':
      return 'Completed';
    case 'Closed':
      return 'EffectivenessConfirmed';
    default:
      return 'Unclassified';
  }
}

/**
 * 通常のIncidentをIncidentWithWorkflowに変換
 */
export function convertToWorkflowIncident(
  incident: Incident, 
  mode: WorkflowMode = 'legacy'
): IncidentWithWorkflow {
  const workflowIncident: IncidentWithWorkflow = {
    ...incident,
    workflowMode: mode,
  };

  // 新ワークフローモードの場合、ワークフローステータスをマッピング
  if (mode === 'new') {
    workflowIncident.workflowStatus = mapLegacyToWorkflowStatus(incident.status, incident.category);
  }

  return workflowIncident;
}

/**
 * ワークフローの進捗率を計算
 */
export function calculateWorkflowProgress(status?: WorkflowStatus): number {
  if (!status) return 0;
  
  switch (status) {
    case 'Unclassified': return 10;
    case 'Pending': return 25;
    case 'InProgress': return 50;
    case 'Completed': return 75;
    case 'PreventionProposed': return 90;
    case 'EffectivenessConfirmed': return 100;
    default: return 0;
  }
}

/**
 * ユーザーロールに基づいて利用可能なワークフローアクションを取得
 */
export function getAvailableWorkflowActions(
  status?: WorkflowStatus, 
  userRole?: string
): string[] {
  const actions: string[] = [];
  
  if (!status) return ['enable-workflow'];

  switch (status) {
    case 'Unclassified':
      if (userRole === 'Incident Manager' || userRole === 'Admin')
        actions.push('classify');
      break;
      
    case 'Pending':
      if (userRole === 'Warehouse Staff' || userRole === 'Admin')
        actions.push('start-response');
      break;
      
    case 'InProgress':
      if (userRole === 'Warehouse Staff' || userRole === 'Admin') {
        actions.push('analyze-cause', 'complete-response');
      }
      break;
      
    case 'Completed':
      if (userRole === 'Warehouse Staff' || userRole === 'Admin')
        actions.push('propose-prevention');
      break;
      
    case 'PreventionProposed':
      if (userRole === 'Incident Manager' || userRole === 'Admin')
        actions.push('confirm-effectiveness');
      break;
      
    case 'EffectivenessConfirmed':
      // 完了状態 - アクションなし
      break;
  }
  
  return actions;
}

/**
 * ワークフローの次のステップを取得
 */
export function getNextWorkflowStep(status?: WorkflowStatus): string {
  switch (status) {
    case 'Unclassified': return 'インシデント管理者による分類が必要です';
    case 'Pending': return '倉庫担当者による対応開始が必要です';
    case 'InProgress': return '原因分析と対応完了が必要です';
    case 'Completed': return '再発防止策の提案が必要です';
    case 'PreventionProposed': return 'インシデント管理者による有効性確認が必要です';
    case 'EffectivenessConfirmed': return 'ワークフローが完了しています';
    default: return 'ワークフローを有効化してください';
  }
}

/**
 * ワークフローステータスに基づく期限の緊急度を判定
 */
export function getWorkflowUrgency(
  status?: WorkflowStatus, 
  dueDate?: string,
  responseStartDate?: string
): 'low' | 'medium' | 'high' | 'critical' {
  if (!dueDate) return 'low';
  
  const due = new Date(dueDate);
  const now = new Date();
  const diffDays = Math.ceil((due.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));
  
  // 期限切れ
  if (diffDays < 0) return 'critical';
  
  // 緊急（1日以内）
  if (diffDays <= 1) return 'high';
  
  // 注意（3日以内）
  if (diffDays <= 3) return 'medium';
  
  // 通常
  return 'low';
}

/**
 * ワークフローの完了率を計算
 */
export function calculateWorkflowCompletionRate(incidents: IncidentWithWorkflow[]): number {
  const workflowIncidents = incidents.filter(i => i.workflowStatus);
  if (workflowIncidents.length === 0) return 0;
  
  const completedIncidents = workflowIncidents.filter(i => 
    i.workflowStatus === 'EffectivenessConfirmed'
  );
  
  return Math.round((completedIncidents.length / workflowIncidents.length) * 100);
}
