import * as React from "react"
import { cn } from "../../lib/utils"

export interface CpfInputProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'onChange'> {
  value?: string
  onChange?: (value: string) => void
}

const CpfInput = React.forwardRef<HTMLInputElement, CpfInputProps>(
  ({ className, value, onChange, ...props }, ref) => {
    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      if (onChange) {
        // Remove tudo que não é número
        let numbers = e.target.value.replace(/\D/g, '')
        
        // Limita a 11 dígitos
        numbers = numbers.slice(0, 11)
        
        // Retorna apenas os números (sem formatação)
        onChange(numbers)
      }
    }

    // Formata o valor para exibição
    const formatValue = (val: string) => {
      if (!val) return ''
      const numbers = val.replace(/\D/g, '')
      
      let formatted = numbers
      if (numbers.length > 3) {
        formatted = numbers.slice(0, 3) + '.' + numbers.slice(3)
      }
      if (numbers.length > 6) {
        formatted = numbers.slice(0, 3) + '.' + numbers.slice(3, 6) + '.' + numbers.slice(6)
      }
      if (numbers.length > 9) {
        formatted = numbers.slice(0, 3) + '.' + numbers.slice(3, 6) + '.' + numbers.slice(6, 9) + '-' + numbers.slice(9)
      }
      
      return formatted
    }

    return (
      <input
        type="text"
        className={cn(
          "flex h-10 w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm ring-offset-white file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-gray-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50",
          className
        )}
        ref={ref}
        value={formatValue(value || '')}
        onChange={handleChange}
        placeholder="000.000.000-00"
        maxLength={14}
        {...props}
      />
    )
  }
)
CpfInput.displayName = "CpfInput"

export { CpfInput }
