import * as React from "react";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { CheckCircle } from "lucide-react";

import type { IncidentWithWorkflow } from "@/lib/types";

interface ResponseCompletionFormProps {
  incident: IncidentWithWorkflow;
  onSubmit: (action: string, data: any) => void;
  loading?: boolean;
}

export function ResponseCompletionForm({ incident, onSubmit, loading = false }: ResponseCompletionFormProps) {
  const [responseContent, setResponseContent] = useState(incident.responseContent || '');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (responseContent.trim()) {
      onSubmit('complete-response', { incidentId: incident.id, responseContent });
    }
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <CheckCircle className="w-5 h-5 text-blue-500" />
          対応完了
        </CardTitle>
        <CardDescription>
          実施した対応内容を入力して対応を完了してください。
        </CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          {/* 原因の表示（参考用） */}
          {incident.cause && (
            <div className="p-3 bg-gray-50 rounded-md">
              <Label className="font-medium">分析された原因</Label>
              <p className="mt-1 text-sm text-gray-700">{incident.cause}</p>
            </div>
          )}

          <div>
            <Label htmlFor="responseContent">対応内容 *</Label>
            <textarea
              id="responseContent"
              className="w-full min-h-[120px] p-3 border border-gray-300 rounded-md resize-vertical"
              value={responseContent}
              onChange={(e) => setResponseContent(e.target.value)}
              placeholder="実施した対応内容を詳しく入力してください&#10;&#10;例：&#10;- 配送ルートの見直しと手順書の更新&#10;- 新人スタッフへの追加研修の実施&#10;- システム設定の修正と動作確認&#10;- 関係者への情報共有と注意喚起"
              required
            />
            <p className="text-sm text-gray-500 mt-1">
              実施した対応内容を具体的に記録してください。
            </p>
          </div>

          <Button 
            type="submit"
            disabled={loading || !responseContent.trim()}
            className="w-full"
          >
            {loading ? '完了中...' : '対応を完了'}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
