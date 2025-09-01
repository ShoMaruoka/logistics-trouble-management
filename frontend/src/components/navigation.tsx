'use client';

import * as React from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { cn } from "@/lib/utils";
import { 
  Home, 
  AlertTriangle, 
  BarChart3, 
  FileText, 
  TrendingUp,
  Settings,
  Database
} from "lucide-react";

const navigation = [
  { name: 'ダッシュボード', href: '/', icon: Home },
  { name: 'トラブル管理', href: '/incidents', icon: AlertTriangle },
  { name: '統計・分析', href: '/statistics', icon: BarChart3 },
  { name: 'ファイル管理', href: '/attachments', icon: FileText },
  { name: '効果測定', href: '/effectiveness', icon: TrendingUp },
  { name: 'マスタ管理', href: '/master', icon: Database },
  { name: '設定', href: '/settings', icon: Settings },
];

export function Navigation() {
  const pathname = usePathname();

  return (
    <nav className="bg-white shadow-sm border-b">
      <div className="container mx-auto px-6">
        <div className="flex items-center justify-between h-16">
          {/* ロゴ */}
          <div className="flex items-center">
            <Link href="/" className="flex items-center space-x-2">
              <AlertTriangle className="h-8 w-8 text-blue-600" />
              <span className="text-xl font-bold text-gray-900">
                物流トラブル管理
              </span>
            </Link>
          </div>

          {/* ナビゲーションリンク */}
          <div className="hidden md:flex items-center space-x-8">
            {navigation.map((item) => {
              const isActive = pathname === item.href;
              return (
                <Link
                  key={item.name}
                  href={item.href}
                  className={cn(
                    "flex items-center space-x-2 px-3 py-2 rounded-md text-sm font-medium transition-colors",
                    isActive
                      ? "bg-blue-100 text-blue-700"
                      : "text-gray-600 hover:text-gray-900 hover:bg-gray-100"
                  )}
                >
                  <item.icon className="h-4 w-4" />
                  <span>{item.name}</span>
                </Link>
              );
            })}
          </div>

          {/* モバイルメニューボタン */}
          <div className="md:hidden">
            <button className="text-gray-600 hover:text-gray-900">
              <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
}
