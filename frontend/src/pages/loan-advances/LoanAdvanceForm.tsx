import { useEffect, useState, useMemo } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Card, CardContent } from '../../components/ui/Card';
import { CurrencyInput } from '../../components/ui/CurrencyInput';
import { NumberInput } from '../../components/ui/NumberInput';
import { Select } from '../../components/ui/Select';
import { EntityPicker, type EntityPickerItem } from '../../components/ui/EntityPicker';
import { useToast } from '../../contexts/ToastContext';
import loanAdvanceService from '../../services/loanAdvanceService';
import employeeService from '../../services/employeeService';
import { ArrowLeft, Save } from 'lucide-react';

interface LoanAdvanceFormData {
  employeeId: string;
  employeeName: string;
  amount: string;
  installments: number;
  discountSource: string;
  startDate: string;
}

export function LoanAdvanceForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess, handleBackendError } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  
  const [formData, setFormData] = useState<LoanAdvanceFormData>({
    employeeId: '',
    employeeName: '',
    amount: '0',
    installments: 1,
    discountSource: 'Todos',
    startDate: new Date().toISOString().split('T')[0], // Data de hoje
  });

  const [errors, setErrors] = useState<Partial<Record<keyof LoanAdvanceFormData, string>>>({});

  const isEditing = !!id;

  useEffect(() => {
    if (isEditing) {
      loadLoanAdvance();
    }
  }, [id]);

  const loadLoanAdvance = async () => {
    setIsLoading(true);
    try {
      const loanAdvance = await loanAdvanceService.getLoanAdvanceById(Number(id));
      setFormData({
        employeeId: loanAdvance.employeeId.toString(),
        employeeName: loanAdvance.employeeName || '',
        amount: loanAdvance.amount.toString(),
        installments: Number(loanAdvance.installments),
        discountSource: loanAdvance.discountSource,
        startDate: loanAdvance.startDate.split('T')[0],
      });
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsLoading(false);
    }
  };

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof LoanAdvanceFormData, string>> = {};

    if (!formData.employeeId) {
      newErrors.employeeId = 'Empregado é obrigatório';
    }

    if (!formData.amount || Number(formData.amount) <= 0) {
      newErrors.amount = 'Valor é obrigatório e deve ser maior que zero';
    }

    if (formData.installments <= 0) {
      newErrors.installments = 'Número de parcelas deve ser maior que zero';
    }

    if (!formData.discountSource.trim()) {
      newErrors.discountSource = 'Fonte de desconto é obrigatória';
    }

    if (!formData.startDate) {
      newErrors.startDate = 'Data de início é obrigatória';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) {
      showError('Por favor, corrija os erros no formulário');
      return;
    }

    setIsSaving(true);
    try {
      const loanAdvanceData = {
        employeeId: Number(formData.employeeId),
        amount: Number(formData.amount),
        installments: formData.installments,
        discountSource: formData.discountSource.trim(),
        startDate: new Date(formData.startDate).toISOString(),
        isApproved: true, // Sempre aprovado
      };

      if (isEditing) {
        await loanAdvanceService.updateLoanAdvance(Number(id), loanAdvanceData);
        showSuccess('Empréstimo e Adiantamento atualizado com sucesso!');
      } else {
        await loanAdvanceService.createLoanAdvance(loanAdvanceData);
        showSuccess('Empréstimo e Adiantamento criado com sucesso!');
      }

      navigate('/loan-advances');
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsSaving(false);
    }
  };

  const handleChange = (field: keyof LoanAdvanceFormData, value: string | number) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  const handleEmployeeChange = (item: EntityPickerItem | null) => {
    setFormData(prev => ({
      ...prev,
      employeeId: item ? item.id.toString() : '',
      employeeName: item ? item.displayText : ''
    }));
    if (errors.employeeId) {
      setErrors(prev => ({ ...prev, employeeId: undefined }));
    }
  };

  const handleSearchEmployee = async (searchTerm: string, page: number) => {
    try {
      const result = await employeeService.getEmployees({
        search: searchTerm,
        page: page,
        pageSize: 10,
      });

      return {
        items: result.items.map(item => ({
          id: item.employeeId,
          displayText: item.fullName,
          secondaryText: item.email || item.phone || undefined
        })),
        totalPages: result.totalPages,
        totalCount: result.totalCount
      };
    } catch (error) {
      console.error('Erro ao buscar empregados:', error);
      return {
        items: [],
        totalPages: 1,
        totalCount: 0
      };
    }
  };

  // Calcular valor da parcela
  const installmentValue = useMemo(() => {
    const amount = Number(formData.amount) || 0;
    const installments = formData.installments || 1;
    return amount / installments;
  }, [formData.amount, formData.installments]);

  const formatCurrency = (value: number): string => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value / 100);
  };

  if (isLoading) {
    return (
      <MainLayout>
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>
      </MainLayout>
    );
  }

  return (
    <MainLayout>
      <div className="max-w-2xl mx-auto">
        <div className="mb-6">
          <Button
            variant="ghost"
            onClick={() => navigate('/loan-advances')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <h1 className="text-3xl font-bold text-gray-900">
            {isEditing ? 'Editar Empréstimo e Adiantamento' : 'Novo Empréstimo e Adiantamento'}
          </h1>
          <p className="text-gray-600 mt-1">
            {isEditing ? 'Atualize as informações do empréstimo e adiantamento' : 'Preencha as informações para criar um novo empréstimo e adiantamento'}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <Card>
            <CardContent className="p-6">
              <div className="space-y-4">
                <div>
                  <label htmlFor="employeeId" className="block text-sm font-medium text-gray-700 mb-1">
                    Empregado <span className="text-red-500">*</span>
                  </label>
                  <EntityPicker
                    value={formData.employeeId ? Number(formData.employeeId) : null}
                    selectedLabel={formData.employeeName}
                    onChange={handleEmployeeChange}
                    onSearch={handleSearchEmployee}
                    placeholder="Selecione um empregado"
                    label="Selecionar Empregado"
                    className={errors.employeeId ? 'border-red-500' : ''}
                  />
                  {errors.employeeId && <p className="text-sm text-red-600 mt-1">{errors.employeeId}</p>}
                </div>

                <div>
                  <label htmlFor="amount" className="block text-sm font-medium text-gray-700 mb-1">
                    Valor <span className="text-red-500">*</span>
                  </label>
                  <CurrencyInput
                    id="amount"
                    value={formData.amount}
                    onChange={(value) => handleChange('amount', value)}
                    className={errors.amount ? 'border-red-500' : ''}
                  />
                  {errors.amount && <p className="text-sm text-red-600 mt-1">{errors.amount}</p>}
                </div>

                <div>
                  <label htmlFor="installments" className="block text-sm font-medium text-gray-700 mb-1">
                    Número de Parcelas <span className="text-red-500">*</span>
                  </label>
                  <NumberInput
                    value={formData.installments}
                    onChange={(value) => handleChange('installments', value)}
                    min={1}
                    max={999}
                    className={errors.installments ? 'border-red-500' : ''}
                  />
                  {errors.installments && <p className="text-sm text-red-600 mt-1">{errors.installments}</p>}
                </div>

                {/* Label indicativa do valor da parcela */}
                {formData.amount && Number(formData.amount) > 0 && formData.installments > 0 && (
                  <div className="bg-blue-50 border border-blue-200 rounded-lg p-3">
                    <p className="text-sm font-medium text-blue-900 text-center">
                      {formData.installments}x de {formatCurrency(installmentValue)}
                    </p>
                  </div>
                )}

                <div>
                  <label htmlFor="discountSource" className="block text-sm font-medium text-gray-700 mb-1">
                    Fonte de Desconto <span className="text-red-500">*</span>
                  </label>
                  <Select
                    id="discountSource"
                    value={formData.discountSource}
                    onChange={(e) => handleChange('discountSource', e.target.value)}
                    className={errors.discountSource ? 'border-red-500' : ''}
                  >
                    <option value="Todos">Todos</option>
                    <option value="Mensal">Mensal</option>
                    <option value="Férias">Férias</option>
                    <option value="Décimo Terceiro">Décimo Terceiro</option>
                  </Select>
                  {errors.discountSource && <p className="text-sm text-red-600 mt-1">{errors.discountSource}</p>}
                </div>

                <div>
                  <label htmlFor="startDate" className="block text-sm font-medium text-gray-700 mb-1">
                    Data de Início da Cobrança <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="startDate"
                    type="date"
                    value={formData.startDate}
                    onChange={(e) => handleChange('startDate', e.target.value)}
                    className={errors.startDate ? 'border-red-500' : ''}
                  />
                  {errors.startDate && <p className="text-sm text-red-600 mt-1">{errors.startDate}</p>}
                </div>
              </div>
            </CardContent>
          </Card>

          <div className="flex gap-3 justify-end">
            <Button type="submit" disabled={isSaving} className="flex-1 sm:flex-none">
              {isSaving ? (
                <>
                  <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                  Salvando...
                </>
              ) : (
                <>
                  <Save className="h-4 w-4 mr-2" />
                  {isEditing ? 'Atualizar' : 'Criar'} Empréstimo e Adiantamento
                </>
              )}
            </Button>
            <Button
              type="button"
              variant="outline"
              onClick={() => navigate('/loan-advances')}
              disabled={isSaving}
              className="flex-1 sm:flex-none"
            >
              Cancelar
            </Button>
          </div>
        </form>
      </div>
    </MainLayout>
  );
}
