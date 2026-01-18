import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '../../components/ui/Dialog';
import { Button } from '../../components/ui/Button';
import { useToast } from '../../contexts/ToastContext';
import payrollService from '../../services/payrollService';
import type { PayrollSuggestion } from '../../services/payrollService';
import { parseBackendError } from '../../utils/errorHandler';
import { ChevronLeft, ChevronRight, AlertTriangle } from 'lucide-react';

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
  const [isLoading, setIsLoading] = useState(true);
  const [suggestion, setSuggestion] = useState<PayrollSuggestion | null>(null);
  
  // Estado para mês e ano selecionados (será preenchido pela sugestão)
  const [selectedMonth, setSelectedMonth] = useState(0);
  const [selectedYear, setSelectedYear] = useState(2024);
  const [notes, setNotes] = useState('');
  const [errors, setErrors] = useState<Record<string, string>>({});

  // Buscar sugestão ao abrir o dialog
  useEffect(() => {
    if (isOpen) {
      loadSuggestion();
    }
  }, [isOpen]);

  const loadSuggestion = async () => {
    setIsLoading(true);
    try {
      const data = await payrollService.getPayrollSuggestion();
      setSuggestion(data);
      // Definir mês e ano sugeridos (mês é 1-indexed no backend, 0-indexed no frontend)
      setSelectedMonth(data.suggestedMonth - 1);
      setSelectedYear(data.suggestedYear);
    } catch (err: any) {
      // Se falhar, usar mês atual como fallback
      const now = new Date();
      setSelectedMonth(now.getMonth());
      setSelectedYear(now.getFullYear());
      setSuggestion(null);
    } finally {
      setIsLoading(false);
    }
  };

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
    setNotes('');
    setErrors({});
    setSuggestion(null);
  };

  const handleClose = () => {
    if (!isSubmitting) {
      resetForm();
      onClose();
    }
  };

  // Verificar se pode criar folha (não pode se já existir uma em aberto)
  const canCreate = !suggestion?.hasOpenPayroll;

  return (
    <Dialog open={isOpen} onOpenChange={handleClose}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Nova Folha de Pagamento</DialogTitle>
        </DialogHeader>

        {isLoading ? (
          <div className="px-6 pb-6 flex items-center justify-center py-8">
            <div className="text-gray-500">Carregando...</div>
          </div>
        ) : (
        <form onSubmit={handleSubmit} className="px-6 pb-6 space-y-4">
          {/* Alerta se já existir folha em aberto */}
          {suggestion?.hasOpenPayroll && (
            <div className="bg-amber-50 border border-amber-200 rounded-lg p-4 flex gap-3">
              <AlertTriangle className="h-5 w-5 text-amber-600 flex-shrink-0 mt-0.5" />
              <div>
                <p className="text-sm font-medium text-amber-800">Folha em aberto</p>
                <p className="text-sm text-amber-700 mt-1">
                  Já existe uma folha de pagamento em aberto para o período{' '}
                  <strong>{suggestion.openPayrollPeriod}</strong>.
                </p>
                <p className="text-sm text-amber-700 mt-1">
                  Feche ou exclua a folha existente antes de criar uma nova.
                </p>
              </div>
            </div>
          )}

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
              disabled={isSubmitting || !canCreate}
            >
              {isSubmitting ? 'Criando...' : 'Criar Folha'}
            </Button>
          </div>
        </form>
        )}
      </DialogContent>
    </Dialog>
  );
}
