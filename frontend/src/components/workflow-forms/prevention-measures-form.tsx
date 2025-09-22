import * as React from "react";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { FileText } from "lucide-react";

import type { IncidentWithWorkflow } from "@/lib/types";

interface PreventionMeasuresFormProps {
  incident: IncidentWithWorkflow;
  onSubmit: (action: string, data: any) => void;
  loading?: boolean;
}

export function PreventionMeasuresForm({ incident, onSubmit, loading = false }: PreventionMeasuresFormProps) {
  const [preventionMeasures, setPreventionMeasures] = useState(incident.preventionMeasures || '');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (preventionMeasures.trim()) {
      onSubmit('propose-prevention', { incidentId: incident.id, preventionMeasures });
    }
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <FileText className="w-5 h-5 text-purple-500" />
          再発防止策提案
        </CardTitle>
        <CardDescription>
          今後同様のインシデントを防ぐための対策を提案してください。
        </CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          {/* 原因と対応内容の表示（参考用） */}
          <div className="space-y-3">
            {incident.cause && (
              <div className="p-3 bg-gray-50 rounded-md">
                <Label className="font-medium">原因</Label>
                <p className="mt-1 text-sm text-gray-700">{incident.cause}</p>
              </div>
            )}
            
            {incident.responseContent && (
              <div className="p-3 bg-blue-50 rounded-md">
                <Label className="font-medium">実施した対応</Label>
                <p className="mt-1 text-sm text-gray-700">{incident.responseContent}</p>
              </div>
            )}
          </div>

          <div>
            <Label htmlFor="preventionMeasures">再発防止策 *</Label>
            <textarea
              id="preventionMeasures"
              className="w-full min-h-[120px] p-3 border border-gray-300 rounded-md resize-vertical"
              value={preventionMeasures}
              onChange={(e) => setPreventionMeasures(e.target.value)}
              placeholder="再発防止策を詳しく入力してください&#10;&#10;例：&#10;- 作業手順書の改訂と周知徹底&#10;- 定期的な研修プログラムの実施&#10;- システムチェック機能の強化&#10;- 品質管理体制の見直し&#10;- 関係部署との連携強化"
              required
            />
            <p className="text-sm text-gray-500 mt-1">
              具体的で実行可能な防止策を提案してください。
            </p>
          </div>

          <Button 
            type="submit"
            disabled={loading || !preventionMeasures.trim()}
            className="w-full"
          >
            {loading ? '提案中...' : '再発防止策を提案'}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
