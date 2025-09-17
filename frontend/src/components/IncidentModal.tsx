import React from 'react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { IncidentForm } from './incident-form';
import { Incident } from '@/lib/types';

interface IncidentModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  incident?: Incident | null;
  onSubmit: (data: any) => void;
  loading?: boolean;
  title?: string;
  description?: string;
}

export function IncidentModal({
  open,
  onOpenChange,
  incident,
  onSubmit,
  loading = false,
  title,
  description,
}: IncidentModalProps) {
  const defaultTitle = incident ? '物流トラブル編集' : '物流トラブル登録';
  const defaultDescription = incident 
    ? '物流トラブルの情報を編集してください。' 
    : '新しい物流トラブルを登録してください。';

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-none w-[40vw] max-h-[80vh] !max-w-[40vw] !w-[40vw] flex flex-col">
        <DialogHeader>
          <DialogTitle>
            {title || defaultTitle}
          </DialogTitle>
          <DialogDescription>
            {description || defaultDescription}
          </DialogDescription>
        </DialogHeader>
        <div className="flex-1 overflow-y-auto">
          <IncidentForm
            incident={incident}
            onSubmit={onSubmit}
            loading={loading}
            hideButtons={true}
          />
        </div>
        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={loading}
          >
            キャンセル
          </Button>
          <Button
            type="button"
            onClick={() => {
              console.log('IncidentModal button clicked');
              // IncidentFormの送信ボタンをプログラム的にクリック
              const form = document.getElementById('incident-form') as HTMLFormElement;
              console.log('IncidentModal form element:', form);
              if (form) {
                console.log('IncidentModal calling form.requestSubmit()');
                form.requestSubmit();
                console.log('IncidentModal form.requestSubmit() completed');
              } else {
                console.error('IncidentModal form element not found');
              }
            }}
            disabled={loading}
          >
            {loading ? '処理中...' : (incident ? '更新' : '登録')}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
