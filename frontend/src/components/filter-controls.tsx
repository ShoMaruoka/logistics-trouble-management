'use client';

import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Label } from '@/components/ui/label';
import { useFilter } from '@/contexts/FilterContext';

export function FilterControls() {
  const { selectedYear, selectedMonth, setSelectedYear, setSelectedMonth } = useFilter();
  const thisYear = new Date().getFullYear();

  return (
    <div className="flex items-center gap-3 mb-6">
      <Label>年度</Label>
      <Select value={String(selectedYear)} onValueChange={(v) => setSelectedYear(Number(v))}>
        <SelectTrigger className="w-[160px]">
          <SelectValue placeholder="年度を選択" />
        </SelectTrigger>
        <SelectContent>
          {[0,1,2,3,4].map(offset => {
            const y = thisYear - offset;
            return (
              <SelectItem key={y} value={String(y)}>{y}年</SelectItem>
            );
          })}
        </SelectContent>
      </Select>
      
      <Label>月</Label>
      <Select value={String(selectedMonth)} onValueChange={(v) => setSelectedMonth(Number(v))}>
        <SelectTrigger className="w-[160px]">
          <SelectValue placeholder="月を選択" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="0">年間表示</SelectItem>
          {[1,2,3,4,5,6,7,8,9,10,11,12].map(m => (
            <SelectItem key={m} value={String(m)}>{m}月</SelectItem>
          ))}
        </SelectContent>
      </Select>
    </div>
  );
}
