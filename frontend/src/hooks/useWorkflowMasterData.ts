import { useState, useEffect, useCallback } from 'react';
import { useMasterData } from './useMasterData';
import type { 
  TroubleType, 
  DamageType, 
  Warehouse, 
  ShippingCompany,
  WorkflowMode
} from '@/lib/types';

interface WorkflowMasterData {
  troubleTypes: TroubleType[];
  damageTypes: DamageType[];
  warehouses: Warehouse[];
  shippingCompanies: ShippingCompany[];
  mode: WorkflowMode;
}

export function useWorkflowMasterData(mode: WorkflowMode = 'legacy') {
  const masterData = useMasterData();
  const [workflowMasterData, setWorkflowMasterData] = useState<WorkflowMasterData | null>(null);

  useEffect(() => {
    if (!masterData.loading && !masterData.error) {
      setWorkflowMasterData({
        troubleTypes: masterData.troubleTypes,
        damageTypes: masterData.damageTypes,
        warehouses: masterData.warehouses,
        shippingCompanies: masterData.shippingCompanies,
        mode
      });
    }
  }, [
    masterData.troubleTypes, 
    masterData.damageTypes, 
    masterData.warehouses, 
    masterData.shippingCompanies,
    masterData.loading,
    masterData.error,
    mode
  ]);

  const refetch = useCallback(async () => {
    await masterData.refetch();
  }, [masterData.refetch]);

  return {
    data: workflowMasterData,
    loading: masterData.loading,
    error: masterData.error,
    refetch
  };
}
