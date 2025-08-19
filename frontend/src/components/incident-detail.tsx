import * as React from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Edit, Calendar, User, FileText, Clock, CheckCircle, XCircle } from "lucide-react";
import type { IncidentDto } from "@/lib/api-types";

interface IncidentDetailProps {
  incident: IncidentDto;
  onEdit: () => void;
  onClose: () => void;
}

export function IncidentDetail({ incident, onEdit, onClose }: IncidentDetailProps) {
  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Open':
        return 'bg-blue-100 text-blue-800';
      case 'InProgress':
        return 'bg-yellow-100 text-yellow-800';
      case 'Resolved':
        return 'bg-green-100 text-green-800';
      case 'Closed':
        return 'bg-gray-100 text-gray-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'Critical':
        return 'bg-red-100 text-red-800';
      case 'High':
        return 'bg-orange-100 text-orange-800';
      case 'Medium':
        return 'bg-yellow-100 text-yellow-800';
      case 'Low':
        return 'bg-green-100 text-green-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleString('ja-JP', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg max-w-4xl w-full max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          {/* ヘッダー */}
          <div className="flex justify-between items-start mb-6">
            <div>
              <h2 className="text-2xl font-bold text-gray-900 mb-2">{incident.title}</h2>
              <div className="flex gap-2">
                <Badge className={getStatusColor(incident.status)}>
                  {incident.status === 'Open' && '未対応'}
                  {incident.status === 'InProgress' && '対応中'}
                  {incident.status === 'Resolved' && '解決済み'}
                  {incident.status === 'Closed' && '完了'}
                </Badge>
                <Badge className={getPriorityColor(incident.priority)}>
                  {incident.priority === 'Critical' && '緊急'}
                  {incident.priority === 'High' && '高'}
                  {incident.priority === 'Medium' && '中'}
                  {incident.priority === 'Low' && '低'}
                </Badge>
              </div>
            </div>
            <div className="flex gap-2">
              <Button onClick={onEdit} className="flex items-center gap-2">
                <Edit className="h-4 w-4" />
                編集
              </Button>
              <Button variant="outline" onClick={onClose}>
                閉じる
              </Button>
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
            {/* メインコンテンツ */}
            <div className="lg:col-span-2 space-y-6">
              {/* 詳細 */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <FileText className="h-5 w-5" />
                    詳細
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-gray-700 whitespace-pre-wrap">{incident.description}</p>
                </CardContent>
              </Card>

              {/* 解決内容 */}
              {incident.resolution && (
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <CheckCircle className="h-5 w-5 text-green-600" />
                      解決内容
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <p className="text-gray-700 whitespace-pre-wrap">{incident.resolution}</p>
                  </CardContent>
                </Card>
              )}
            </div>

            {/* サイドバー */}
            <div className="space-y-6">
              {/* 基本情報 */}
              <Card>
                <CardHeader>
                  <CardTitle>基本情報</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="flex items-center gap-2">
                    <Calendar className="h-4 w-4 text-gray-500" />
                    <span className="text-sm text-gray-600">報告日</span>
                    <span className="text-sm font-medium">{formatDate(incident.reportedDate)}</span>
                  </div>

                  {incident.resolvedDate && (
                    <div className="flex items-center gap-2">
                      <CheckCircle className="h-4 w-4 text-green-500" />
                      <span className="text-sm text-gray-600">解決日</span>
                      <span className="text-sm font-medium">{formatDate(incident.resolvedDate)}</span>
                    </div>
                  )}

                  <div className="flex items-center gap-2">
                    <User className="h-4 w-4 text-gray-500" />
                    <span className="text-sm text-gray-600">報告者</span>
                    <span className="text-sm font-medium">{incident.reportedByName}</span>
                  </div>

                  {incident.assignedToName && (
                    <div className="flex items-center gap-2">
                      <User className="h-4 w-4 text-gray-500" />
                      <span className="text-sm text-gray-600">担当者</span>
                      <span className="text-sm font-medium">{incident.assignedToName}</span>
                    </div>
                  )}

                  <div className="flex items-center gap-2">
                    <span className="text-sm text-gray-600">カテゴリ</span>
                    <span className="text-sm font-medium">{incident.category}</span>
                  </div>
                </CardContent>
              </Card>

              {/* 統計情報 */}
              <Card>
                <CardHeader>
                  <CardTitle>統計情報</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="flex items-center gap-2">
                    <Clock className="h-4 w-4 text-gray-500" />
                    <span className="text-sm text-gray-600">解決時間</span>
                    <span className="text-sm font-medium">
                      {incident.resolutionTime || '未解決'}
                    </span>
                  </div>

                  <div className="flex items-center gap-2">
                    <FileText className="h-4 w-4 text-gray-500" />
                    <span className="text-sm text-gray-600">添付ファイル</span>
                    <span className="text-sm font-medium">{incident.attachmentCount}件</span>
                  </div>

                  {incident.isOverdue && (
                    <div className="flex items-center gap-2 text-red-600">
                      <XCircle className="h-4 w-4" />
                      <span className="text-sm font-medium">期限超過</span>
                    </div>
                  )}
                </CardContent>
              </Card>

              {/* タイムスタンプ */}
              <Card>
                <CardHeader>
                  <CardTitle>タイムスタンプ</CardTitle>
                </CardHeader>
                <CardContent className="space-y-2">
                  <div className="text-sm">
                    <span className="text-gray-600">作成日時:</span>
                    <div className="font-medium">{formatDate(incident.createdAt)}</div>
                  </div>
                  <div className="text-sm">
                    <span className="text-gray-600">更新日時:</span>
                    <div className="font-medium">{formatDate(incident.updatedAt)}</div>
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
