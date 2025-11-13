import * as React from "react"
import { cn } from "../../lib/utils"

export interface PhoneInputProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'onChange'> {
  value?: string
  onChange?: (value: string) => void
}

const PhoneInput = React.forwardRef<HTMLInputElement, PhoneInputProps>(
  ({ className, value, onChange, ...props }, ref) => {
    const formatPhone = (value: string): string => {
      // Remove tudo exceto números
      const numbers = value.replace(/\D/g, '')
      
      // Aplica máscara baseado na quantidade de dígitos
      if (numbers.length <= 10) {
        // Telefone fixo: (00) 0000-0000
        return numbers
          .replace(/^(\d{2})(\d)/, '($1) $2')
          .replace(/(\d{4})(\d)/, '$1-$2')
      } else {
        // Celular: (00) 00000-0000
        return numbers
          .replace(/^(\d{2})(\d)/, '($1) $2')
          .replace(/(\d{5})(\d)/, '$1-$2')
          .substring(0, 15) // Limita tamanho
      }
    }

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      const formatted = formatPhone(e.target.value)
      
      if (onChange) {
        // Retorna apenas os números
        onChange(formatted.replace(/\D/g, ''))
      }
    }

    return (
      <input
        type="text"
        className={cn(
          "flex h-10 w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm ring-offset-white file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-gray-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50",
          className
        )}
        ref={ref}
        value={formatPhone(value || '')}
        onChange={handleChange}
        placeholder="(00) 00000-0000"
        {...props}
      />
    )
  }
)
PhoneInput.displayName = "PhoneInput"

export { PhoneInput }
