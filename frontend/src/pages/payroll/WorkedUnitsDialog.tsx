import { useState, useEffect } from 'react';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { X, Clock, Calendar } from 'lucide-react';
import type { PayrollEmployeeDetailed } from '../../services/payrollService';

interface WorkedUnitsDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (workedUnits: number) => Promise<void>;
  employee: PayrollEmployeeDetailed | null;
  isLoading?: boolean;
}

export function WorkedUnitsDialog({
  isOpen,
  onClose,
  onSave,
  employee,
  isLoading = false
}: WorkedUnitsDialogProps) {
  const [workedUnits, setWorkedUnits] = useState('');
  const [error, setError] = useState<string | null>(null);

  const isHourly = employee?.contractType === 'Horista';
  const unitLabel = isHourly ? 'horas' : 'dias';
  const UnitIcon = isHourly ? Clock : Calendar;

  useEffect(() => {
    if (employee?.workedUnits !== undefined && employee.workedUnits !== null) {
      setWorkedUnits(employee.workedUnits.toString().replace('.', ','));
    } else {
      setWorkedUnits('');
    }
  }, [employee]);

  const formatCurrency = (valueInCents: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(valueInCents / 100);
  };

  const parseUnitsInput = (value: string): number => {
    const cleaned = value.replace(',', '.');
    const parsed = parseFloat(cleaned);
    return isNaN(parsed) ? 0 : parsed;
  };

  const handleUnitsChange = (value: string) => {
    // Permite apenas números e vírgula/ponto
    const cleaned = value.replace(/[^\d,\.]/g, '');
    setWorkedUnits(cleaned);
    setError(null);
  };

  const validate = (): boolean => {
    const units = parseUnitsInput(workedUnits);
    
    if (units <= 0) {
      setError(`Quantidade de ${unitLabel} deve ser maior que zero`);
      return false;
    }
    
    if (isHourly && units > 744) { // Max ~31 days * 24 hours
      setError('Quantidade de horas excede o máximo permitido');
      return false;
    }
    
    if (!isHourly && units > 31) {
      setError('Quantidade de dias excede o máximo permitido');
      return false;
    }
    
    setError(null);
    return true;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validate()) return;
    
    await onSave(parseUnitsInput(workedUnits));
  };

  const handleClose = () => {
    setWorkedUnits('');
    setError(null);
    onClose();
  };

  // Calcular valor estimado
  const calculateEstimatedValue = (): number => {
    if (!employee?.contractValue) return 0;
    const units = parseUnitsInput(workedUnits);
    return Math.round(employee.contractValue * units);
  };

  if (!isOpen || !employee) return null;

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
            <div className="flex items-center gap-2">
              <UnitIcon className="h-5 w-5 text-primary-600" />
              <h3 className="text-lg font-semibold text-gray-900">
                {isHourly ? 'Horas Trabalhadas' : 'Dias Trabalhados'}
              </h3>
            </div>
            <button
              onClick={handleClose}
              className="text-gray-400 hover:text-gray-600"
            >
              <X className="h-5 w-5" />
            </button>
          </div>
          
          {/* Content */}
          <form onSubmit={handleSubmit} className="p-4 space-y-4">
            {/* Employee Info */}
            <div className="bg-gray-50 rounded-lg p-3">
              <p className="font-medium text-gray-900">{employee.employeeName}</p>
              <div className="mt-1 flex items-center gap-4 text-sm text-gray-600">
                <span>
                  Tipo: <span className="font-medium">{employee.contractType}</span>
                </span>
                <span>
                  Valor/{isHourly ? 'hora' : 'dia'}: <span className="font-medium text-green-600">
                    {formatCurrency(employee.contractValue || 0)}
                  </span>
                </span>
              </div>
            </div>
            
            {/* Input */}
            <div>
              <label htmlFor="workedUnits" className="block text-sm font-medium text-gray-700 mb-1">
                Quantidade de {unitLabel}
              </label>
              <Input
                id="workedUnits"
                type="text"
                inputMode="decimal"
                value={workedUnits}
                onChange={(e) => handleUnitsChange(e.target.value)}
                placeholder={`Ex: ${isHourly ? '160' : '22'}`}
                className={error ? 'border-red-500' : ''}
              />
              {error && (
                <p className="mt-1 text-sm text-red-600">{error}</p>
              )}
            </div>
            
            {/* Preview */}
            {workedUnits && parseUnitsInput(workedUnits) > 0 && (
              <div className="bg-green-50 rounded-lg p-3 border border-green-200">
                <p className="text-sm text-green-800">
                  <span className="font-medium">Salário Base Calculado:</span>
                </p>
                <p className="text-xl font-bold text-green-700 mt-1">
                  {formatCurrency(calculateEstimatedValue())}
                </p>
                <p className="text-xs text-green-600 mt-1">
                  {formatCurrency(employee.contractValue || 0)} × {parseUnitsInput(workedUnits).toLocaleString('pt-BR')} {unitLabel}
                </p>
              </div>
            )}
            
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
