import { useState, useEffect } from 'react';
import { apiClient } from '@/lib/api-client';
import type { 
  TroubleType, 
  DamageType, 
  Warehouse, 
  ShippingCompany 
} from '@/lib/types';

export function useMasterData() {
  const [troubleTypes, setTroubleTypes] = useState<TroubleType[]>([]);
  const [damageTypes, setDamageTypes] = useState<DamageType[]>([]);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [shippingCompanies, setShippingCompanies] = useState<ShippingCompany[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchMasterData = async () => {
      try {
        setLoading(true);
        setError(null);

        const [
          troubleTypesData,
          damageTypesData,
          warehousesData,
          shippingCompaniesData
        ] = await Promise.all([
          apiClient.getActiveTroubleTypes(),
          apiClient.getActiveDamageTypes(),
          apiClient.getActiveWarehouses(),
          apiClient.getActiveShippingCompanies()
        ]);

        setTroubleTypes(troubleTypesData);
        setDamageTypes(damageTypesData);
        setWarehouses(warehousesData);
        setShippingCompanies(shippingCompaniesData);
      } catch (err) {
        setError('マスタデータの取得に失敗しました');
        console.error('マスタデータ取得エラー:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchMasterData();
  }, []);

  return {
    troubleTypes,
    damageTypes,
    warehouses,
    shippingCompanies,
    loading,
    error
  };
}
