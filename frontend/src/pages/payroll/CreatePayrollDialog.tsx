import { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '../../components/ui/Dialog';
import { Button } from '../../components/ui/Button';
import { useToast } from '../../contexts/ToastContext';
import payrollService from '../../services/payrollService';
import { parseBackendError } from '../../utils/errorHandler';
import { ChevronLeft, ChevronRight } from 'lucide-react';

interface CreatePayrollDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

const MONTHS = [
  'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
  'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
];

export function CreatePayrollDialog({ isOpen, onClose, onSuccess }: CreatePayrollDialogProps) {
  const { showError, showSuccess } = useToast();
  const [isSubmitting, setIsSubmitting] = useState(false);
  
  // Estado para mês e ano selecionados (default: mês atual)
  const currentDate = new Date();
  const [selectedMonth, setSelectedMonth] = useState(currentDate.getMonth());
  const [selectedYear, setSelectedYear] = useState(currentDate.getFullYear());
  const [notes, setNotes] = useState('');
  const [errors, setErrors] = useState<Record<string, string>>({});

  // Calcular primeiro e último dia do mês
  const getFirstDayOfMonth = (year: number, month: number) => {
    return new Date(year, month, 1).toISOString().split('T')[0];
  };

  const getLastDayOfMonth = (year: number, month: number) => {
    return new Date(year, month + 1, 0).toISOString().split('T')[0];
  };

  const handlePrevYear = () => setSelectedYear(prev => prev - 1);
  const handleNextYear = () => setSelectedYear(prev => prev + 1);

  const validate = () => {
    const newErrors: Record<string, string> = {};
    // Validação básica - mês e ano são sempre válidos pelo seletor
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) return;

    setIsSubmitting(true);
    try {
      const periodStartDate = getFirstDayOfMonth(selectedYear, selectedMonth);
      const periodEndDate = getLastDayOfMonth(selectedYear, selectedMonth);

      await payrollService.createPayroll({
        periodStartDate,
        periodEndDate,
        notes: notes || undefined
      });

      showSuccess('Folha de pagamento criada com sucesso!');
      resetForm();
      onSuccess();
      onClose();
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsSubmitting(false);
    }
  };

  const resetForm = () => {
    const now = new Date();
    setSelectedMonth(now.getMonth());
    setSelectedYear(now.getFullYear());
    setNotes('');
    setErrors({});
  };

  const handleClose = () => {
    if (!isSubmitting) {
      resetForm();
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
          {/* Seletor de Ano */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Período de Referência *
            </label>
            <div className="flex items-center justify-center gap-4 mb-3">
              <button
                type="button"
                onClick={handlePrevYear}
                disabled={isSubmitting}
                className="p-2 rounded-full hover:bg-gray-100 disabled:opacity-50"
              >
                <ChevronLeft className="h-5 w-5" />
              </button>
              <span className="text-xl font-bold min-w-[80px] text-center">{selectedYear}</span>
              <button
                type="button"
                onClick={handleNextYear}
                disabled={isSubmitting}
                className="p-2 rounded-full hover:bg-gray-100 disabled:opacity-50"
              >
                <ChevronRight className="h-5 w-5" />
              </button>
            </div>

            {/* Grid de Meses */}
            <div className="grid grid-cols-4 gap-2">
              {MONTHS.map((month, index) => (
                <button
                  key={month}
                  type="button"
                  onClick={() => setSelectedMonth(index)}
                  disabled={isSubmitting}
                  className={`py-2 px-1 text-sm rounded-md transition-colors ${
                    selectedMonth === index
                      ? 'bg-primary-600 text-white font-medium'
                      : 'bg-gray-100 hover:bg-gray-200 text-gray-700'
                  } disabled:opacity-50`}
                >
                  {month.substring(0, 3)}
                </button>
              ))}
            </div>

            {/* Preview do período */}
            <p className="text-center text-sm text-gray-500 mt-3">
              Período: {new Date(selectedYear, selectedMonth, 1).toLocaleDateString('pt-BR')} até{' '}
              {new Date(selectedYear, selectedMonth + 1, 0).toLocaleDateString('pt-BR')}
            </p>

            {errors.period && (
              <p className="text-sm text-red-500 mt-1 text-center">{errors.period}</p>
            )}
          </div>

          <div>
            <label htmlFor="notes" className="block text-sm font-medium text-gray-700 mb-1">
              Observações
            </label>
            <textarea
              id="notes"
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
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
