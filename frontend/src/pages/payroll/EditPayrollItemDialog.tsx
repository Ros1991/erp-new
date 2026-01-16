import { useState, useEffect } from 'react';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { CurrencyInput } from '../../components/ui/CurrencyInput';
import { X } from 'lucide-react';
import type { PayrollItem } from '../../services/payrollService';

interface EditPayrollItemDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (data: { description: string; amount: number }) => Promise<void>;
  item: PayrollItem | null;
  isLoading?: boolean;
}

export function EditPayrollItemDialog({
  isOpen,
  onClose,
  onSave,
  item,
  isLoading = false
}: EditPayrollItemDialogProps) {
  const [description, setDescription] = useState('');
  const [amount, setAmount] = useState<number>(0);
  const [errors, setErrors] = useState<{ description?: string; amount?: string }>({});

  useEffect(() => {
    if (item) {
      setDescription(item.description);
      setAmount(item.amount);
    }
  }, [item]);

  const validate = (): boolean => {
    const newErrors: { description?: string; amount?: string } = {};
    
    if (!description.trim()) {
      newErrors.description = 'Descrição é obrigatória';
    }
    
    if (amount <= 0) {
      newErrors.amount = 'Valor deve ser maior que zero';
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validate()) return;
    
    await onSave({
      description: description.trim(),
      amount: amount
    });
  };

  const handleClose = () => {
    setDescription('');
    setAmount(0);
    setErrors({});
    onClose();
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex min-h-screen items-center justify-center p-4">
        {/* Backdrop */}
        <div 
          className="fixed inset-0 bg-black bg-opacity-50 transition-opacity"
          onClick={handleClose}
        />
        
        {/* Dialog */}
        <div className="relative bg-white rounded-lg shadow-xl max-w-md w-full">
          {/* Header */}
          <div className="flex items-center justify-between p-4 border-b">
            <h3 className="text-lg font-semibold text-gray-900">
              Editar Item da Folha
            </h3>
            <button
              onClick={handleClose}
              className="text-gray-400 hover:text-gray-600"
            >
              <X className="h-5 w-5" />
            </button>
          </div>
          
          {/* Content */}
          <form onSubmit={handleSubmit} className="p-4 space-y-4">
            {item && (
              <div className="text-sm text-gray-500 mb-2">
                <span className={`inline-flex items-center px-2 py-0.5 rounded text-xs font-medium ${
                  item.type === 'Provento' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                }`}>
                  {item.type}
                </span>
                <span className="ml-2">{item.category}</span>
              </div>
            )}
            
            <div>
              <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1">
                Descrição
              </label>
              <Input
                id="description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Descrição do item"
                className={errors.description ? 'border-red-500' : ''}
              />
              {errors.description && (
                <p className="mt-1 text-sm text-red-600">{errors.description}</p>
              )}
            </div>
            
            <div>
              <label htmlFor="amount" className="block text-sm font-medium text-gray-700 mb-1">
                Valor
              </label>
              <CurrencyInput
                id="amount"
                value={amount}
                onChange={(value) => setAmount(parseInt(value) || 0)}
                className={errors.amount ? 'border-red-500' : ''}
              />
              {errors.amount && (
                <p className="mt-1 text-sm text-red-600">{errors.amount}</p>
              )}
            </div>
            
            {/* Footer */}
            <div className="flex justify-end gap-3 pt-4">
              <Button
                type="button"
                variant="outline"
                onClick={handleClose}
                disabled={isLoading}
              >
                Cancelar
              </Button>
              <Button
                type="submit"
                disabled={isLoading}
              >
                {isLoading ? 'Salvando...' : 'Salvar'}
              </Button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
