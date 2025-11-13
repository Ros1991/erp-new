import React from 'react';
import { Minus, Plus } from 'lucide-react';

interface NumberInputProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'onChange' | 'type' | 'value'> {
  value: number;
  onChange: (value: number) => void;
  min?: number;
  max?: number;
  step?: number;
}

export function NumberInput({ 
  value, 
  onChange, 
  min = 1, 
  max = 999, 
  step = 1,
  className = '',
  disabled = false,
  ...props 
}: NumberInputProps) {
  const handleIncrement = () => {
    const newValue = value + step;
    if (max === undefined || newValue <= max) {
      onChange(newValue);
    }
  };

  const handleDecrement = () => {
    const newValue = value - step;
    if (min === undefined || newValue >= min) {
      onChange(newValue);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const inputValue = e.target.value;
    
    // Permite campo vazio temporariamente
    if (inputValue === '') {
      onChange(min || 0);
      return;
    }

    // Remove caracteres não numéricos
    const numericValue = inputValue.replace(/\D/g, '');
    const parsedValue = parseInt(numericValue, 10);

    if (!isNaN(parsedValue)) {
      // Aplica limites se definidos
      let finalValue = parsedValue;
      if (min !== undefined && finalValue < min) finalValue = min;
      if (max !== undefined && finalValue > max) finalValue = max;
      onChange(finalValue);
    }
  };

  return (
    <div className="relative flex items-center">
      <button
        type="button"
        onClick={handleDecrement}
        disabled={disabled || (min !== undefined && value <= min)}
        className="absolute left-1 z-10 h-8 w-8 flex items-center justify-center text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        aria-label="Diminuir"
      >
        <Minus className="h-4 w-4" />
      </button>
      
      <input
        type="text"
        inputMode="numeric"
        value={value}
        onChange={handleInputChange}
        disabled={disabled}
        className={`w-full px-11 py-2 border border-gray-300 rounded-md text-center focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent disabled:bg-gray-50 disabled:text-gray-500 ${className}`}
        {...props}
      />
      
      <button
        type="button"
        onClick={handleIncrement}
        disabled={disabled || (max !== undefined && value >= max)}
        className="absolute right-1 z-10 h-8 w-8 flex items-center justify-center text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        aria-label="Aumentar"
      >
        <Plus className="h-4 w-4" />
      </button>
    </div>
  );
}
