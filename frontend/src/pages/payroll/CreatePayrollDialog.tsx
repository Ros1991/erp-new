import { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '../../components/ui/Dialog';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { useToast } from '../../contexts/ToastContext';
import payrollService from '../../services/payrollService';
import { parseBackendError } from '../../utils/errorHandler';

interface CreatePayrollDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export function CreatePayrollDialog({ isOpen, onClose, onSuccess }: CreatePayrollDialogProps) {
  const { showError, showSuccess } = useToast();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formData, setFormData] = useState({
    periodStartDate: '',
    periodEndDate: '',
    notes: ''
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleChange = (field: string, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: '' }));
    }
  };

  const validate = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.periodStartDate) {
      newErrors.periodStartDate = 'Data de início é obrigatória';
    }

    if (!formData.periodEndDate) {
      newErrors.periodEndDate = 'Data de fim é obrigatória';
    }

    if (formData.periodStartDate && formData.periodEndDate) {
      const startDate = new Date(formData.periodStartDate);
      const endDate = new Date(formData.periodEndDate);
      
      if (endDate <= startDate) {
        newErrors.periodEndDate = 'Data de fim deve ser posterior à data de início';
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) return;

    setIsSubmitting(true);
    try {
      await payrollService.createPayroll({
        periodStartDate: formData.periodStartDate,
        periodEndDate: formData.periodEndDate,
        notes: formData.notes || undefined
      });

      showSuccess('Folha de pagamento criada com sucesso!');
      setFormData({ periodStartDate: '', periodEndDate: '', notes: '' });
      setErrors({});
      onSuccess();
      onClose();
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    if (!isSubmitting) {
      setFormData({ periodStartDate: '', periodEndDate: '', notes: '' });
      setErrors({});
      onClose();
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={handleClose}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Nova Folha de Pagamento</DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="px-6 pb-6 space-y-4">
          <div>
            <label htmlFor="periodStartDate" className="block text-sm font-medium text-gray-700 mb-1">
              Data de Início do Período *
            </label>
            <Input
              id="periodStartDate"
              type="date"
              value={formData.periodStartDate}
              onChange={(e) => handleChange('periodStartDate', e.target.value)}
              className={errors.periodStartDate ? 'border-red-500' : ''}
              disabled={isSubmitting}
            />
            {errors.periodStartDate && (
              <p className="text-sm text-red-500 mt-1">{errors.periodStartDate}</p>
            )}
          </div>

          <div>
            <label htmlFor="periodEndDate" className="block text-sm font-medium text-gray-700 mb-1">
              Data de Fim do Período *
            </label>
            <Input
              id="periodEndDate"
              type="date"
              value={formData.periodEndDate}
              onChange={(e) => handleChange('periodEndDate', e.target.value)}
              className={errors.periodEndDate ? 'border-red-500' : ''}
              disabled={isSubmitting}
            />
            {errors.periodEndDate && (
              <p className="text-sm text-red-500 mt-1">{errors.periodEndDate}</p>
            )}
          </div>

          <div>
            <label htmlFor="notes" className="block text-sm font-medium text-gray-700 mb-1">
              Observações
            </label>
            <textarea
              id="notes"
              value={formData.notes}
              onChange={(e) => handleChange('notes', e.target.value)}
              rows={3}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500"
              placeholder="Observações sobre esta folha..."
              disabled={isSubmitting}
            />
          </div>

          <div className="text-sm text-gray-600 bg-blue-50 p-3 rounded-md">
            <p className="font-medium mb-1">ℹ️ Criação Automática</p>
            <p>A folha será criada automaticamente com todos os contratos ativos, incluindo:</p>
            <ul className="list-disc list-inside mt-1 space-y-1">
              <li>Salários base</li>
              <li>Benefícios e descontos</li>
              <li>Parcelas de empréstimos pendentes</li>
            </ul>
          </div>

          <div className="flex justify-end space-x-3 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={handleClose}
              disabled={isSubmitting}
            >
              Cancelar
            </Button>
            <Button
              type="submit"
              disabled={isSubmitting}
            >
              {isSubmitting ? 'Criando...' : 'Criar Folha'}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
