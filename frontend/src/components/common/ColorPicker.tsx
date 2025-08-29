'use client';

import React, { useState } from 'react';

interface ColorPickerProps {
  label: string;
  value: string;
  onChange: (color: string) => void;
  required?: boolean;
  error?: string;
  className?: string;
}

const PRESET_COLORS = [
  '#EF4444', '#F97316', '#F59E0B', '#EAB308', '#84CC16', '#22C55E',
  '#10B981', '#06B6D4', '#3B82F6', '#6366F1', '#8B5CF6', '#A855F7',
  '#EC4899', '#F43F5E', '#6B7280', '#374151', '#1F2937', '#111827'
];

export const ColorPicker: React.FC<ColorPickerProps> = ({
  label,
  value,
  onChange,
  required = false,
  error,
  className = '',
}) => {
  const [isOpen, setIsOpen] = useState(false);

  const handleColorSelect = (color: string) => {
    onChange(color);
    setIsOpen(false);
  };

  const handleCustomColorChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    onChange(e.target.value);
  };

  return (
    <div className={`w-full ${className}`}>
      <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
        {label}
        {required && <span className="text-red-500 ml-1">*</span>}
      </label>
      <div className="relative">
        <div className="flex items-center space-x-2">
          <div
            className="w-10 h-10 rounded border-2 border-gray-300 cursor-pointer"
            style={{ backgroundColor: value }}
            onClick={() => setIsOpen(!isOpen)}
          />
          <input
            type="text"
            value={value}
            onChange={handleCustomColorChange}
            placeholder="#000000"
            className="flex-1 px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-800 dark:border-gray-600 dark:text-white"
          />
        </div>
        
        {isOpen && (
          <div className="absolute z-10 mt-2 p-3 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-md shadow-lg">
            <div className="grid grid-cols-6 gap-2 mb-3">
              {PRESET_COLORS.map((color) => (
                <button
                  key={color}
                  type="button"
                  className="w-8 h-8 rounded border-2 border-gray-300 hover:border-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                  style={{ backgroundColor: color }}
                  onClick={() => handleColorSelect(color)}
                  title={color}
                />
              ))}
            </div>
            <div className="text-xs text-gray-500 dark:text-gray-400">
              プリセットカラーをクリックするか、HEX値を直接入力してください
            </div>
          </div>
        )}
      </div>
      {error && (
        <p className="mt-1 text-sm text-red-600 dark:text-red-400">{error}</p>
      )}
    </div>
  );
};

export default ColorPicker;
