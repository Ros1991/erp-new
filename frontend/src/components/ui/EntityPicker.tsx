import * as React from "react"
import { useState, useEffect } from "react"
import { cn } from "../../lib/utils"
import { Search, X, ChevronLeft, ChevronRight } from "lucide-react"
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "./Dialog"
import { Input } from "./Input"
import { Button } from "./Button"

export interface EntityPickerItem {
  id: number
  displayText: string
  secondaryText?: string
}

export interface EntityPickerProps {
  value?: number | null
  selectedLabel?: string
  onChange?: (item: EntityPickerItem | null) => void
  onSearch: (searchTerm: string, page: number) => Promise<{
    items: EntityPickerItem[]
    totalPages: number
    totalCount: number
  }>
  placeholder?: string
  label?: string
  className?: string
  disabled?: boolean
}

const EntityPicker = React.forwardRef<HTMLDivElement, EntityPickerProps>(
  ({ 
    value, 
    selectedLabel,
    onChange, 
    onSearch, 
    placeholder = "Selecione...", 
    label = "Selecionar",
    className,
    disabled 
  }, ref) => {
    const [isOpen, setIsOpen] = useState(false)
    const [searchTerm, setSearchTerm] = useState('')
    const [items, setItems] = useState<EntityPickerItem[]>([])
    const [currentPage, setCurrentPage] = useState(1)
    const [totalPages, setTotalPages] = useState(1)
    const [totalCount, setTotalCount] = useState(0)
    const [isLoading, setIsLoading] = useState(false)

    const loadItems = async (search: string, page: number) => {
      setIsLoading(true)
      try {
        const result = await onSearch(search, page)
        setItems(result.items)
        setTotalPages(result.totalPages)
        setTotalCount(result.totalCount)
        setCurrentPage(page)
      } catch (error) {
        console.error('Erro ao carregar itens:', error)
        setItems([])
      } finally {
        setIsLoading(false)
      }
    }

    // Busca automática ao digitar (debounce)
    useEffect(() => {
      if (!isOpen) return

      const timer = setTimeout(() => {
        loadItems(searchTerm, 1)
      }, 500) // 500ms de debounce

      return () => clearTimeout(timer)
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [searchTerm, isOpen])

    const handleOpen = () => {
      setIsOpen(true)
      setSearchTerm('')
    }

    const handleSelect = (item: EntityPickerItem) => {
      if (onChange) {
        onChange(item)
      }
      setIsOpen(false)
    }

    const handleClear = (e: React.MouseEvent) => {
      e.stopPropagation()
      if (onChange) {
        onChange(null)
      }
    }

    const handlePageChange = (page: number) => {
      loadItems(searchTerm, page)
    }


    return (
      <>
        <div ref={ref} className={cn("relative", className)}>
          <button
            type="button"
            onClick={handleOpen}
            disabled={disabled}
            className={cn(
              "flex h-10 w-full items-center justify-between rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm ring-offset-white focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 cursor-pointer text-left",
              !selectedLabel && "text-gray-500"
            )}
          >
            <span className="truncate">
              {selectedLabel || placeholder}
            </span>
            <div className="flex items-center gap-1">
              {value && (
                <X 
                  className="h-4 w-4 text-gray-400 hover:text-gray-600"
                  onClick={handleClear}
                />
              )}
              <Search className="h-4 w-4 text-gray-500" />
            </div>
          </button>
        </div>

        <Dialog
          open={isOpen}
          onOpenChange={setIsOpen}
        >
          <DialogContent className="relative">
            <button
              onClick={() => setIsOpen(false)}
              className="absolute right-4 top-4 rounded-sm opacity-70 ring-offset-white transition-opacity hover:opacity-100 focus:outline-none focus:ring-2 focus:ring-gray-400 focus:ring-offset-2 z-10"
            >
              <X className="h-4 w-4" />
              <span className="sr-only">Fechar</span>
            </button>
            <DialogHeader>
              <DialogTitle>{label}</DialogTitle>
            </DialogHeader>
            <div className="px-6 pb-6 space-y-4">
            {/* Campo de busca */}
            <div>
              <Input
                type="text"
                placeholder="Buscar..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                autoFocus
              />
            </div>

            {/* Lista de resultados */}
            <div className="border rounded-lg max-h-96 overflow-y-auto">
              {isLoading ? (
                <div className="p-8 text-center text-gray-500">
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600 mx-auto mb-2"></div>
                  Carregando...
                </div>
              ) : items.length === 0 ? (
                <div className="p-8 text-center text-gray-500">
                  Nenhum resultado encontrado
                </div>
              ) : (
                <div className="divide-y">
                  {items.map((item) => (
                    <button
                      key={item.id}
                      type="button"
                      onClick={() => handleSelect(item)}
                      className={cn(
                        "w-full px-4 py-3 text-left hover:bg-gray-50 transition-colors",
                        value === item.id && "bg-primary-50"
                      )}
                    >
                      <div className="font-medium text-gray-900">{item.displayText}</div>
                      {item.secondaryText && (
                        <div className="text-sm text-gray-500">{item.secondaryText}</div>
                      )}
                    </button>
                  ))}
                </div>
              )}
            </div>

            {/* Paginação */}
            {totalPages > 1 && (
              <div className="flex items-center justify-between pt-4 border-t">
                <div className="text-sm text-gray-600">
                  Página {currentPage} de {totalPages} • Total: {totalCount}
                </div>
                <div className="flex gap-2">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => handlePageChange(currentPage - 1)}
                    disabled={currentPage === 1 || isLoading}
                  >
                    <ChevronLeft className="h-4 w-4" />
                  </Button>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => handlePageChange(currentPage + 1)}
                    disabled={currentPage === totalPages || isLoading}
                  >
                    <ChevronRight className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            )}
            </div>
          </DialogContent>
        </Dialog>
      </>
    )
  }
)
EntityPicker.displayName = "EntityPicker"

export { EntityPicker }
