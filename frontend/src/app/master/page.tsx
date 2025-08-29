'use client';

import React, { useState } from 'react';
import { CogIcon, ExclamationTriangleIcon, TruckIcon, BuildingOfficeIcon } from '@heroicons/react/24/outline';
import TroubleTypeManagement from '@/components/master/TroubleTypeManagement';
import DamageTypeManagement from '@/components/master/DamageTypeManagement';
import WarehouseManagement from '@/components/master/WarehouseManagement';
import ShippingCompanyManagement from '@/components/master/ShippingCompanyManagement';

type MasterTab = 'troubleTypes' | 'damageTypes' | 'warehouses' | 'shippingCompanies';

const tabs = [
  {
    id: 'troubleTypes' as MasterTab,
    name: 'トラブル種類',
    icon: ExclamationTriangleIcon,
    component: TroubleTypeManagement
  },
  {
    id: 'damageTypes' as MasterTab,
    name: '損傷種類',
    icon: CogIcon,
    component: DamageTypeManagement
  },
  {
    id: 'warehouses' as MasterTab,
    name: '出荷元倉庫',
    icon: BuildingOfficeIcon,
    component: WarehouseManagement
  },
  {
    id: 'shippingCompanies' as MasterTab,
    name: '運送会社',
    icon: TruckIcon,
    component: ShippingCompanyManagement
  }
];

export default function MasterManagementPage() {
  const [activeTab, setActiveTab] = useState<MasterTab>('troubleTypes');

  const ActiveComponent = tabs.find(tab => tab.id === activeTab)?.component || TroubleTypeManagement;

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
            マスタ管理
          </h1>
          <p className="mt-2 text-gray-600 dark:text-gray-400">
            システムで使用する各種マスタデータの管理を行います
          </p>
        </div>

        {/* タブナビゲーション */}
        <div className="border-b border-gray-200 dark:border-gray-700 mb-8">
          <nav className="-mb-px flex space-x-8">
            {tabs.map((tab) => {
              const Icon = tab.icon;
              return (
                <button
                  key={tab.id}
                  onClick={() => setActiveTab(tab.id)}
                  className={`
                    flex items-center space-x-2 py-2 px-1 border-b-2 font-medium text-sm
                    ${activeTab === tab.id
                      ? 'border-blue-500 text-blue-600 dark:text-blue-400'
                      : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 dark:text-gray-400 dark:hover:text-gray-300'
                    }
                  `}
                >
                  <Icon className="h-5 w-5" />
                  <span>{tab.name}</span>
                </button>
              );
            })}
          </nav>
        </div>

        {/* アクティブなタブのコンテンツ */}
        <div className="bg-white dark:bg-gray-800 rounded-lg shadow">
          <ActiveComponent />
        </div>
      </div>
    </div>
  );
}
