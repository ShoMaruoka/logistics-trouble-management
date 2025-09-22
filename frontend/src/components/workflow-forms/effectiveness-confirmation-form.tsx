import * as React from "react";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Shield } from "lucide-react";

import type { IncidentWithWorkflow } from "@/lib/types";

interface EffectivenessConfirmationFormProps {
  incident: IncidentWithWorkflow;
  onSubmit: (action: string, data: any) => void;
  loading?: boolean;
}

export function EffectivenessConfirmationForm({ incident, onSubmit, loading = false }: EffectivenessConfirmationFormProps) {
  const [effectivenessStatus, setEffectivenessStatus] = useState('Implemented');
  const [effectivenessComment, setEffectivenessComment] = useState(incident.effectivenessComment || '');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (effectivenessComment.trim()) {
      onSubmit('confirm-effectiveness', { 
        incidentId: incident.id, 
        effectivenessStatus,
        effectivenessComment 
      });
    }
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <Shield className="w-5 h-5 text-green-500" />
          有効性確認
        </CardTitle>
        <CardDescription>
          提案された再発防止策の有効性を確認してください。
        </CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          {/* 提案された再発防止策の表示 */}
          {incident.preventionMeasures && (
            <div className="p-3 bg-purple-50 rounded-md">
              <Label className="font-medium">提案された再発防止策</Label>
              <p className="mt-1 text-sm text-gray-700">{incident.preventionMeasures}</p>
            </div>
          )}

          {/* 対応内容の表示（参考用） */}
          {incident.responseContent && (
            <div className="p-3 bg-blue-50 rounded-md">
              <Label className="font-medium">実施された対応</Label>
              <p className="mt-1 text-sm text-gray-700">{incident.responseContent}</p>
            </div>
          )}

          <div>
            <Label>有効性評価 *</Label>
            <Select
              value={effectivenessStatus}
              onValueChange={setEffectivenessStatus}
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="Implemented">実施</SelectItem>
                <SelectItem value="NotImplemented">未実施</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div>
            <Label htmlFor="effectivenessComment">確認コメント *</Label>
            <textarea
              id="effectivenessComment"
              className="w-full min-h-[120px] p-3 border border-gray-300 rounded-md resize-vertical"
              value={effectivenessComment}
              onChange={(e) => setEffectivenessComment(e.target.value)}
              placeholder="有効性確認の詳細コメントを入力してください&#10;&#10;例：&#10;- 再発防止策が適切に実施されていることを確認&#10;- 類似インシデントの発生状況を監視中&#10;- 関係者への周知が完了し、理解度も良好&#10;- 今後も継続的な監視が必要"
              required
            />
            <p className="text-sm text-gray-500 mt-1">
              再発防止策の実施状況と今後の監視方針を記載してください。
            </p>
          </div>

          <Button 
            type="submit"
            disabled={loading || !effectivenessComment.trim()}
            className="w-full"
          >
            {loading ? '確認中...' : '有効性を確認'}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
