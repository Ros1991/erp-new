import * as React from "react"
import { cn } from "../../lib/utils"

export interface DocumentInputProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'onChange'> {
  value?: string
  onChange?: (value: string) => void
}

const DocumentInput = React.forwardRef<HTMLInputElement, DocumentInputProps>(
  ({ className, value, onChange, ...props }, ref) => {
    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      if (onChange) {
        // Permite digitar qualquer coisa
        onChange(e.target.value)
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
        value={value || ''}
        onChange={handleChange}
        placeholder="CPF ou CNPJ"
        {...props}
      />
    )
  }
)
DocumentInput.displayName = "DocumentInput"

export { DocumentInput }

// Função helper para sanitizar documento (usar no submit)
export const sanitizeDocument = (document: string): string => {
  return document.replace(/\D/g, '')
}
