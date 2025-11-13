import * as React from "react"
import { cn } from "../../lib/utils"

export interface CurrencyInputProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'onChange'> {
  value?: string | number
  onChange?: (value: string) => void
}

const CurrencyInput = React.forwardRef<HTMLInputElement, CurrencyInputProps>(
  ({ className, value, onChange, ...props }, ref) => {
    const [displayValue, setDisplayValue] = React.useState('')
    const inputRef = React.useRef<HTMLInputElement>(null)
    const [cursorPosition, setCursorPosition] = React.useState<number | null>(null)

    // Merge refs
    React.useImperativeHandle(ref, () => inputRef.current as HTMLInputElement)

    // Formata o valor inicial quando o componente monta ou value muda
    React.useEffect(() => {
      if (value !== undefined) {
        const numValue = typeof value === 'string' ? parseFloat(value) || 0 : value
        setDisplayValue(formatCurrency(numValue))
      }
    }, [value])

    // Reposiciona o cursor após a formatação
    React.useEffect(() => {
      if (cursorPosition !== null && inputRef.current) {
        inputRef.current.setSelectionRange(cursorPosition, cursorPosition)
        setCursorPosition(null)
      }
    }, [displayValue, cursorPosition])

    const formatCurrency = (value: number): string => {
      // Converte centavos para reais
      const valueInReais = value / 100
      return valueInReais.toLocaleString('pt-BR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      })
    }

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      const input = e.target
      const oldValue = displayValue
      const newValue = input.value
      
      // Posição do cursor antes da mudança
      const cursorPos = input.selectionStart || 0
      
      // Remove tudo exceto números
      const numbers = newValue.replace(/\D/g, '')
      
      if (numbers === '') {
        setDisplayValue('0,00')
        if (onChange) onChange('0')
        setCursorPosition(1) // Posiciona após o primeiro 0
        return
      }

      // Converte para número (em centavos)
      const numValue = parseInt(numbers, 10)
      
      // Formata o novo valor
      const formatted = formatCurrency(numValue)
      
      // Calcula nova posição do cursor
      // Conta quantos caracteres não-numéricos existem antes da posição do cursor
      const oldNonDigits = (oldValue.substring(0, cursorPos).match(/\D/g) || []).length
      const newNonDigits = (formatted.substring(0, cursorPos).match(/\D/g) || []).length
      const diff = newNonDigits - oldNonDigits
      
      let newCursorPos = cursorPos + diff
      
      // Garante que o cursor não fique antes do início ou depois do fim
      newCursorPos = Math.max(0, Math.min(newCursorPos, formatted.length))
      
      // Atualiza display
      setDisplayValue(formatted)
      setCursorPosition(newCursorPos)
      
      // Chama onChange com o valor em centavos
      if (onChange) {
        onChange(numValue.toString())
      }
    }

    return (
      <div className="relative">
        <span className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500 text-sm pointer-events-none">
          R$
        </span>
        <input
          type="text"
          className={cn(
            "flex h-10 w-full rounded-lg border border-gray-300 bg-white pl-10 pr-3 py-2 text-sm ring-offset-white file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-gray-500 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50",
            className
          )}
          ref={inputRef}
          value={displayValue}
          onChange={handleChange}
          placeholder="0,00"
          {...props}
        />
      </div>
    )
  }
)
CurrencyInput.displayName = "CurrencyInput"

export { CurrencyInput }
