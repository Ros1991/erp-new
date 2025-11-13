import * as React from "react"
import { useState, useRef, useEffect } from "react"
import { cn } from "../../lib/utils"
import { ChevronDown, Check } from "lucide-react"

export interface SelectOption {
  value: string
  label: string
}

export interface SelectProps {
  id?: string
  value?: string
  onChange?: (e: { target: { value: string } }) => void
  className?: string
  disabled?: boolean
  options?: SelectOption[]
  children?: React.ReactNode
}

const Select = React.forwardRef<HTMLDivElement, SelectProps>(
  ({ className, value, onChange, disabled, options, children, id }, ref) => {
    const [isOpen, setIsOpen] = useState(false)
    const containerRef = useRef<HTMLDivElement>(null)

    // Parse options from children if not provided as prop
    const selectOptions: SelectOption[] = options || []
    if (!options && children) {
      React.Children.forEach(children, (child) => {
        if (React.isValidElement(child) && child.type === 'option') {
          const props = child.props as { value: string; children: React.ReactNode }
          selectOptions.push({
            value: props.value,
            label: props.children as string
          })
        }
      })
    }

    const selectedOption = selectOptions.find(opt => opt.value === value)

    // Close dropdown when clicking outside
    useEffect(() => {
      const handleClickOutside = (event: MouseEvent) => {
        if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
          setIsOpen(false)
        }
      }

      if (isOpen) {
        document.addEventListener('mousedown', handleClickOutside)
      }

      return () => {
        document.removeEventListener('mousedown', handleClickOutside)
      }
    }, [isOpen])

    const handleSelect = (optionValue: string) => {
      if (onChange) {
        onChange({ target: { value: optionValue } })
      }
      setIsOpen(false)
    }

    return (
      <div ref={containerRef} className="relative">
        <button
          id={id}
          type="button"
          onClick={() => !disabled && setIsOpen(!isOpen)}
          disabled={disabled}
          className={cn(
            "flex h-10 w-full items-center justify-between rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm ring-offset-white focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 cursor-pointer text-left",
            className
          )}
        >
          <span className={selectedOption ? "text-gray-900" : "text-gray-500"}>
            {selectedOption?.label || "Selecione..."}
          </span>
          <ChevronDown 
            className={cn(
              "h-4 w-4 text-gray-500 transition-transform",
              isOpen && "transform rotate-180"
            )} 
          />
        </button>

        {isOpen && (
          <div className="absolute z-50 w-full mt-0.5 bg-white border border-gray-200 rounded-lg shadow-lg overflow-hidden">
            <div className="max-h-60 overflow-y-auto py-1">
              {selectOptions.map((option) => {
                const isSelected = option.value === value
                return (
                  <button
                    key={option.value}
                    type="button"
                    onClick={() => handleSelect(option.value)}
                    className={cn(
                      "w-full px-3 py-2 text-sm text-left flex items-center justify-between hover:bg-primary-50 transition-colors",
                      isSelected && "bg-primary-50 text-primary-600 font-medium"
                    )}
                  >
                    <span>{option.label}</span>
                    {isSelected && <Check className="h-4 w-4 text-primary-600" />}
                  </button>
                )
              })}
            </div>
          </div>
        )}
      </div>
    )
  }
)
Select.displayName = "Select"

export { Select }
