'use client';

import React, { createContext, useContext, useState, ReactNode } from 'react';

interface FilterContextType {
  selectedYear: number;
  selectedMonth: number;
  setSelectedYear: (year: number) => void;
  setSelectedMonth: (month: number) => void;
}

const FilterContext = createContext<FilterContextType | undefined>(undefined);

export function FilterProvider({ children }: { children: ReactNode }) {
  const thisYear = new Date().getFullYear();
  const [selectedYear, setSelectedYear] = useState<number>(thisYear);
  const [selectedMonth, setSelectedMonth] = useState<number>(0); // 0 = 年間表示

  return (
    <FilterContext.Provider value={{
      selectedYear,
      selectedMonth,
      setSelectedYear,
      setSelectedMonth,
    }}>
      {children}
    </FilterContext.Provider>
  );
}

export function useFilter() {
  const context = useContext(FilterContext);
  if (context === undefined) {
    throw new Error('useFilter must be used within a FilterProvider');
  }
  return context;
}
