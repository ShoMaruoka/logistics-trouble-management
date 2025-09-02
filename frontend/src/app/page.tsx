"use client";

import * as React from "react";
import { useState } from "react";
import { PlusCircle } from "lucide-react";
import { useRouter } from "next/navigation";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { IncidentForm } from "@/components/incident-form";
import { Dashboard } from "@/components/dashboard";
import { IncidentManagement } from "@/components/incident-management";
import type { Incident, CreateIncidentDto, UpdateIncidentDto } from "@/lib/types";
import { Logo } from "@/components/icons";
import { 
  useCreateIncident, 
  useUpdateIncident, 
  useDeleteIncident
} from "@/lib/hooks";

export default function Home() {
  const router = useRouter();
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingIncident, setEditingIncident] = useState<Incident | null>(null);

  const { createIncident, loading: createLoading } = useCreateIncident();
  const { updateIncident, loading: updateLoading } = useUpdateIncident();
  const { deleteIncident, loading: deleteLoading } = useDeleteIncident();

  const handleCreateIncident = async (data: CreateIncidentDto) => {
    try {
      await createIncident({
        title: data.title,
        description: data.description,
        category: data.category,
        priority: data.priority,
        troubleTypeId: data.troubleTypeId,
        damageTypeId: data.damageTypeId,
        warehouseId: data.warehouseId,
        shippingCompanyId: data.shippingCompanyId,
        incidentDetails: data.incidentDetails,
        totalShipments: data.totalShipments,
        defectiveItems: data.defectiveItems,
        occurrenceDate: data.occurrenceDate,
        occurrenceLocation: data.occurrenceLocation,
        summary: data.summary,
        cause: data.cause,
        preventionMeasures: data.preventionMeasures,
        effectivenessStatus: data.effectivenessStatus,
        effectivenessDate: data.effectivenessDate || null,
        effectivenessComment: data.effectivenessComment,
        reportedById: 1 // 仮のユーザーID
      });
      setIsDialogOpen(false);
      // サーバーコンポーネントを再検証
      router.refresh();
    } catch (error) {
      console.error('物流トラブル作成エラー:', error);
    }
  };

  const handleUpdateIncident = async (data: UpdateIncidentDto) => {
    if (!editingIncident) return;
    
    try {
      await updateIncident(editingIncident.id, {
        title: data.title,
        description: data.description,
        category: data.category,
        priority: data.priority,
        troubleTypeId: data.troubleTypeId,
        damageTypeId: data.damageTypeId,
        warehouseId: data.warehouseId,
        shippingCompanyId: data.shippingCompanyId,
        effectivenessStatus: data.effectivenessStatus,
        incidentDetails: data.incidentDetails,
        totalShipments: data.totalShipments,
        defectiveItems: data.defectiveItems,
        occurrenceDate: data.occurrenceDate,
        occurrenceLocation: data.occurrenceLocation,
        summary: data.summary,
        cause: data.cause,
        preventionMeasures: data.preventionMeasures,
        effectivenessDate: data.effectivenessDate || null,
        effectivenessComment: data.effectivenessComment
      });
      setIsDialogOpen(false);
      setEditingIncident(null);
      // サーバーコンポーネントを再検証
      router.refresh();
    } catch (error) {
      console.error('物流トラブル更新エラー:', error);
    }
  };

  // フォームsubmitラッパー（型整合のため）
  const handleFormSubmit = async (data: CreateIncidentDto | UpdateIncidentDto) => {
    if (editingIncident) {
      await handleUpdateIncident(data as UpdateIncidentDto);
    } else {
      await handleCreateIncident(data as CreateIncidentDto);
    }
  };

  const handleDeleteIncident = async (incident: Incident) => {
    if (confirm('この物流トラブルを削除しますか？')) {
      try {
        await deleteIncident(incident.id);
        // サーバーコンポーネントを再検証
        router.refresh();
      } catch (error) {
        console.error('物流トラブル削除エラー:', error);
      }
    }
  };

  const handleEdit = (incident: Incident) => {
    setEditingIncident(incident);
    setIsDialogOpen(true);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="container mx-auto px-4 py-8">
        {/* ヘッダー */}
        <div className="flex justify-between items-center mb-8">
          <div className="flex items-center gap-4">
            <Logo className="h-8 w-8" />
            <h1 className="text-3xl font-bold text-gray-900">物流トラブル管理</h1>
          </div>
          <Button
            onClick={() => {
              setEditingIncident(null);
              setIsDialogOpen(true);
            }}
            className="flex items-center gap-2 bg-logistics-blue hover:bg-logistics-blue/90 text-white"
          >
            <PlusCircle className="h-5 w-5" />
            物流トラブル登録
          </Button>
        </div>

        {/* ダッシュボード */}
        <Dashboard />

        {/* インシデント管理 */}
        <IncidentManagement 
          onEdit={handleEdit}
          onDelete={handleDeleteIncident}
        />

        {/* 作成・編集ダイアログ */}
        <Dialog open={isDialogOpen} onOpenChange={(open) => {
          setIsDialogOpen(open);
          if (!open) {
            setEditingIncident(null);
          }
        }}>
          <DialogContent className="max-w-none w-[40vw] max-h-[80vh] !max-w-[40vw] !w-[40vw] flex flex-col">
            <DialogHeader>
              <DialogTitle>{editingIncident ? '物流トラブル編集' : '物流トラブル登録'}</DialogTitle>
              <DialogDescription>
                物流トラブルの{editingIncident ? '内容を更新' : '内容を入力'}してください。
              </DialogDescription>
            </DialogHeader>
            <div className="flex-1 overflow-y-auto">
              <IncidentForm
                incident={editingIncident}
                onSubmit={handleFormSubmit}
                loading={createLoading || updateLoading}
                hideButtons={true}
              />
            </div>
            <DialogFooter>
              <Button
                type="button"
                variant="outline"
                onClick={() => {
                  setIsDialogOpen(false);
                  setEditingIncident(null);
                }}
                className="border-gray-300"
              >
                キャンセル
              </Button>
              <Button
                type="button"
                onClick={() => {
                  // フォームのsubmitをトリガー
                  const form = document.querySelector('form');
                  if (form) {
                    form.dispatchEvent(new Event('submit', { bubbles: true, cancelable: true }));
                  }
                }}
                disabled={createLoading || updateLoading}
                className="bg-logistics-blue hover:bg-logistics-blue/90 text-white"
              >
                {createLoading || updateLoading ? '保存中...' : (editingIncident ? '保存' : '作成')}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>
    </div>
  );
}