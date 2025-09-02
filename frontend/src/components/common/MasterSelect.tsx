'use client';

import React from 'react';
import { ChevronDownIcon } from '@heroicons/react/24/outline';

interface MasterSelectProps {
  label: string;
  value: number | string;
  onChange: (value: number | string) => void;
  options: Array<{ id: number; name: string; [key: string]: any }>;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  error?: string;
  className?: string;
}

export const MasterSelect: React.FC<MasterSelectProps> = ({
  label,
  value,
  onChange,
  options,
  placeholder = '選択してください',
  required = false,
  disabled = false,
  error,
  className = '',
}) => {
  const handleChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const selectedValue = e.target.value;
    onChange(selectedValue === '' ? '' : Number(selectedValue));
  };

  return (
    <div className={`w-full ${className}`}>
      <label className="block text-sm font-medium text-gray-700 mb-1">
        {label}
        {required && <span className="text-red-500 ml-1">*</span>}
      </label>
      <div className="relative">
        <select
          value={value || ''}
          onChange={handleChange}
          disabled={disabled}
          className={`
            w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500
            ${disabled ? 'bg-gray-100 cursor-not-allowed' : 'bg-white'}
            ${error ? 'border-red-300 focus:ring-red-500 focus:border-red-500' : 'border-gray-300'}
          `}
        >
          <option value="">{placeholder}</option>
          {options.map((option) => (
            <option key={option.id} value={option.id}>
              {option.name}
            </option>
          ))}
        </select>
        <ChevronDownIcon className="absolute right-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400 pointer-events-none" />
      </div>
      {error && (
        <p className="mt-1 text-sm text-red-600">{error}</p>
      )}
    </div>
  );
};

export default MasterSelect;
