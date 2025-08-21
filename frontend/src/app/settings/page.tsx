'use client';

import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';

export default function SettingsPage() {
  return (
    <div className="container mx-auto p-6">
      <div className="mb-6">
        <h1 className="text-3xl font-bold">設定</h1>
        <p className="text-gray-600">システムの設定を管理します</p>
      </div>

      <div className="grid gap-6">
        {/* 一般設定 */}
        <Card>
          <CardHeader>
            <CardTitle>一般設定</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="company-name">会社名</Label>
                <Input id="company-name" defaultValue="物流トラブル管理システム" />
              </div>
              <div className="space-y-2">
                <Label htmlFor="timezone">タイムゾーン</Label>
                <Select defaultValue="jst">
                  <SelectTrigger>
                    <SelectValue placeholder="タイムゾーンを選択" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="jst">JST (UTC+9)</SelectItem>
                    <SelectItem value="utc">UTC</SelectItem>
                    <SelectItem value="pst">PST (UTC-8)</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="language">言語</Label>
                <Select defaultValue="ja">
                  <SelectTrigger>
                    <SelectValue placeholder="言語を選択" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="ja">日本語</SelectItem>
                    <SelectItem value="en">English</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <Label htmlFor="date-format">日付形式</Label>
                <Select defaultValue="yyyy-mm-dd">
                  <SelectTrigger>
                    <SelectValue placeholder="日付形式を選択" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="yyyy-mm-dd">YYYY-MM-DD</SelectItem>
                    <SelectItem value="dd-mm-yyyy">DD-MM-YYYY</SelectItem>
                    <SelectItem value="mm-dd-yyyy">MM-DD-YYYY</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* 通知設定 */}
        <Card>
          <CardHeader>
            <CardTitle>通知設定</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>新規インシデント通知</Label>
                <p className="text-sm text-muted-foreground">
                  新しいインシデントが作成された際に通知を受け取ります
                </p>
              </div>
              <Select defaultValue="enabled">
                <SelectTrigger className="w-24">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="enabled">有効</SelectItem>
                  <SelectItem value="disabled">無効</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>インシデント更新通知</Label>
                <p className="text-sm text-muted-foreground">
                  インシデントが更新された際に通知を受け取ります
                </p>
              </div>
              <Select defaultValue="enabled">
                <SelectTrigger className="w-24">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="enabled">有効</SelectItem>
                  <SelectItem value="disabled">無効</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>期限切れ警告</Label>
                <p className="text-sm text-muted-foreground">
                  インシデントの期限が近づいた際に警告を受け取ります
                </p>
              </div>
              <Select defaultValue="disabled">
                <SelectTrigger className="w-24">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="enabled">有効</SelectItem>
                  <SelectItem value="disabled">無効</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>メール通知</Label>
                <p className="text-sm text-muted-foreground">
                  重要な通知をメールで受け取ります
                </p>
              </div>
              <Select defaultValue="disabled">
                <SelectTrigger className="w-24">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="enabled">有効</SelectItem>
                  <SelectItem value="disabled">無効</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </CardContent>
        </Card>

        {/* データ管理 */}
        <Card>
          <CardHeader>
            <CardTitle>データ管理</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="backup-frequency">バックアップ頻度</Label>
                <Select defaultValue="daily">
                  <SelectTrigger>
                    <SelectValue placeholder="バックアップ頻度を選択" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="daily">毎日</SelectItem>
                    <SelectItem value="weekly">毎週</SelectItem>
                    <SelectItem value="monthly">毎月</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <Label htmlFor="retention-period">データ保持期間</Label>
                <Select defaultValue="2-years">
                  <SelectTrigger>
                    <SelectValue placeholder="保持期間を選択" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="1-year">1年</SelectItem>
                    <SelectItem value="2-years">2年</SelectItem>
                    <SelectItem value="5-years">5年</SelectItem>
                    <SelectItem value="indefinite">無期限</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
            <div className="flex gap-2">
              <Button variant="outline">データエクスポート</Button>
              <Button variant="outline">データインポート</Button>
              <Button variant="destructive">データクリア</Button>
            </div>
          </CardContent>
        </Card>

        {/* セキュリティ設定 */}
        <Card>
          <CardHeader>
            <CardTitle>セキュリティ設定</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>二要素認証</Label>
                <p className="text-sm text-muted-foreground">
                  セキュリティを向上させるために二要素認証を有効にします
                </p>
              </div>
              <Select defaultValue="disabled">
                <SelectTrigger className="w-24">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="enabled">有効</SelectItem>
                  <SelectItem value="disabled">無効</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="flex items-center justify-between">
              <div className="space-y-0.5">
                <Label>セッションタイムアウト</Label>
                <p className="text-sm text-muted-foreground">
                  一定時間操作がない場合に自動的にログアウトします
                </p>
              </div>
              <Select defaultValue="enabled">
                <SelectTrigger className="w-24">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="enabled">有効</SelectItem>
                  <SelectItem value="disabled">無効</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="session-timeout">セッションタイムアウト時間</Label>
                <Select defaultValue="30">
                  <SelectTrigger>
                    <SelectValue placeholder="時間を選択" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="15">15分</SelectItem>
                    <SelectItem value="30">30分</SelectItem>
                    <SelectItem value="60">1時間</SelectItem>
                    <SelectItem value="120">2時間</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* 保存ボタン */}
        <div className="flex justify-end">
          <Button>設定を保存</Button>
        </div>
      </div>
    </div>
  );
}
