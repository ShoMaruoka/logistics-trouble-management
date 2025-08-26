// 列挙型の値を日本語表示名に変換するヘルパー関数

export const getTroubleTypeName = (value: string): string => {
  switch (value) {
    case 'ProductTrouble':
      return '商品トラブル';
    case 'DeliveryTrouble':
      return '配送トラブル';
    case 'SystemTrouble':
      return 'システムトラブル';
    case 'EquipmentTrouble':
      return '設備トラブル';
    case 'QualityTrouble':
      return '品質トラブル';
    case 'LogisticsTrouble':
      return '物流トラブル';
    case 'ManagementTrouble':
      return '管理トラブル';
    case 'SafetyTrouble':
      return '安全トラブル';
    case 'EnvironmentalTrouble':
      return '環境トラブル';
    case 'OtherTrouble':
      return 'その他のトラブル';
    default:
      return '不明';
  }
};

export const getDamageTypeName = (value: string): string => {
  switch (value) {
    case 'None':
      return '0';
    case 'WrongShipment':
      return '誤出荷';
    case 'EarlyOrLateArrival':
      return '早着・延着';
    case 'Lost':
      return '紛失';
    case 'WrongDelivery':
      return '誤配送';
    case 'DamageOrContamination':
      return '破損・汚損';
    case 'OtherDeliveryMistake':
      return 'その他の配送ミス';
    case 'OtherProductAccident':
      return 'その他の商品事故';
    default:
      return '0';
  }
};

export const getWarehouseName = (value: string): string => {
  switch (value) {
    case 'None':
      return '0';
    case 'WarehouseA':
      return 'A倉庫';
    case 'WarehouseB':
      return 'B倉庫';
    case 'WarehouseC':
      return 'C倉庫';
    default:
      return '0';
  }
};

export const getShippingCompanyName = (value: string): string => {
  switch (value) {
    case 'None':
      return '0';
    case 'InHouse':
      return '庫内';
    case 'Charter':
      return 'チャーター';
    case 'ATransport':
      return 'A運輸';
    case 'BExpress':
      return 'B急便';
    default:
      return '0';
  }
};

export const getEffectivenessStatusName = (value: string): string => {
  switch (value) {
    case 'NotImplemented':
      return '未実施';
    case 'Implemented':
      return '実施';
    default:
      return '未実施';
  }
};

export const getTroubleTypeColor = (value: string): string => {
  switch (value) {
    case 'ProductTrouble': // 商品トラブル
      return 'bg-red-100 text-red-800';
    case 'DeliveryTrouble': // 配送トラブル
    case 'SystemTrouble': // システムトラブル
    case 'EquipmentTrouble': // 設備トラブル
    case 'QualityTrouble': // 品質トラブル
    case 'LogisticsTrouble': // 物流トラブル
    case 'ManagementTrouble': // 管理トラブル
    case 'SafetyTrouble': // 安全トラブル
    case 'EnvironmentalTrouble': // 環境トラブル
    case 'OtherTrouble': // その他のトラブル
      return 'bg-orange-100 text-orange-800';
    default:
      return 'bg-gray-100 text-gray-800';
  }
};

export const getEffectivenessStatusColor = (value: string): string => {
  switch (value) {
    case 'NotImplemented': // 未実施
      return 'bg-gray-100 text-gray-800';
    case 'Implemented': // 実施
      return 'bg-green-100 text-green-800';
    default:
      return 'bg-gray-100 text-gray-800';
  }
};
