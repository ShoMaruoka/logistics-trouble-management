export interface Incident {
  id: string;
  troubleType: '配送トラブル' | '商品トラブル';
  processDescription: string;
  damageType: string;
  location: string;
  date: string;
  description: string;
  cause: string;
  recurrencePreventionMeasures: string;
  shipmentVolume?: number;
  defectiveUnits?: number;
  shippingWarehouse?: string;
  shippingCompany?: string;
  effectivenessCheckStatus?: '実施' | '実施中' | '未実施';
  effectivenessCheckDate?: string;
  effectivenessCheckComment?: string;
}
