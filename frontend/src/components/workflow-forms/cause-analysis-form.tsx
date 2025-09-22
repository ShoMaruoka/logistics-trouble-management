import * as React from "react";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Play } from "lucide-react";

import type { IncidentWithWorkflow } from "@/lib/types";

interface CauseAnalysisFormProps {
  incident: IncidentWithWorkflow;
  onSubmit: (action: string, data: any) => void;
  loading?: boolean;
}

export function CauseAnalysisForm({ incident, onSubmit, loading = false }: CauseAnalysisFormProps) {
  const [cause, setCause] = useState(incident.cause || '');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (cause.trim()) {
      onSubmit('analyze-cause', { incidentId: incident.id, cause });
    }
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <Play className="w-5 h-5 text-orange-500" />
          原因分析
        </CardTitle>
        <CardDescription>
          インシデントの原因を分析して入力してください。
        </CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <Label htmlFor="cause">原因 *</Label>
            <textarea
              id="cause"
              className="w-full min-h-[120px] p-3 border border-gray-300 rounded-md resize-vertical"
              value={cause}
              onChange={(e) => setCause(e.target.value)}
              placeholder="インシデントの原因を詳しく入力してください&#10;&#10;例：&#10;- 配送ルートの変更による積み込み手順の変更&#10;- 新人スタッフの作業手順理解不足&#10;- システムの設定ミスによる自動処理の誤動作"
              required
            />
            <p className="text-sm text-gray-500 mt-1">
              原因を詳細に記述することで、効果的な再発防止策の立案に役立ちます。
            </p>
          </div>

          <Button 
            type="submit"
            disabled={loading || !cause.trim()}
            className="w-full"
          >
            {loading ? '入力中...' : '原因を入力'}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
