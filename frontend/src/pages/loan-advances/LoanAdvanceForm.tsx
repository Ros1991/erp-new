import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Card, CardContent } from '../../components/ui/Card';
import { useToast } from '../../contexts/ToastContext';
import loanAdvanceService from '../../services/loanAdvanceService';
import { ArrowLeft, Save } from 'lucide-react';

interface LoanAdvanceFormData {
  employeeId: string;
  amount: string;
  installments: string;
  discountSource: string;
  startDate: string;
  isApproved: boolean;
}

export function LoanAdvanceForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess, handleBackendError } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  
  const [formData, setFormData] = useState<LoanAdvanceFormData>({
    employeeId: '',
    amount: '0',
    installments: '1',
    discountSource: '',
    startDate: '',
    isApproved: false,
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
        amount: loanAdvance.amount.toString(),
        installments: loanAdvance.installments.toString(),
        discountSource: loanAdvance.discountSource,
        startDate: loanAdvance.startDate.split('T')[0],
        isApproved: loanAdvance.isApproved,
      });
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsLoading(false);
    }
  };

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof LoanAdvanceFormData, string>> = {};

    if (!formData.employeeId.trim()) {
      newErrors.employeeId = 'ID do empregado é obrigatório';
    } else if (isNaN(Number(formData.employeeId))) {
      newErrors.employeeId = 'ID do empregado deve ser um número válido';
    }

    if (!formData.amount.trim()) {
      newErrors.amount = 'Valor é obrigatório';
    } else if (isNaN(Number(formData.amount)) || Number(formData.amount) <= 0) {
      newErrors.amount = 'Valor deve ser um número positivo';
    }

    if (!formData.installments.trim()) {
      newErrors.installments = 'Número de parcelas é obrigatório';
    } else if (isNaN(Number(formData.installments)) || Number(formData.installments) <= 0) {
      newErrors.installments = 'Parcelas deve ser um número positivo';
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
        installments: Number(formData.installments),
        discountSource: formData.discountSource.trim(),
        startDate: new Date(formData.startDate).toISOString(),
        isApproved: formData.isApproved,
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

  const handleChange = (field: keyof LoanAdvanceFormData, value: string | boolean) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
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
                    ID do Empregado <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="employeeId"
                    type="text"
                    value={formData.employeeId}
                    onChange={(e) => handleChange('employeeId', e.target.value)}
                    placeholder="ID do empregado"
                    className={errors.employeeId ? 'border-red-500' : ''}
                  />
                  {errors.employeeId && <p className="text-sm text-red-600 mt-1">{errors.employeeId}</p>}
                </div>

                <div>
                  <label htmlFor="amount" className="block text-sm font-medium text-gray-700 mb-1">
                    Valor (R$) <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="amount"
                    type="text"
                    value={formData.amount}
                    onChange={(e) => handleChange('amount', e.target.value)}
                    placeholder="0.00"
                    className={errors.amount ? 'border-red-500' : ''}
                  />
                  {errors.amount && <p className="text-sm text-red-600 mt-1">{errors.amount}</p>}
                  <p className="text-sm text-gray-500 mt-1">Digite o valor em centavos (ex: 50000 = R$ 500,00)</p>
                </div>

                <div>
                  <label htmlFor="installments" className="block text-sm font-medium text-gray-700 mb-1">
                    Número de Parcelas <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="installments"
                    type="text"
                    value={formData.installments}
                    onChange={(e) => handleChange('installments', e.target.value)}
                    placeholder="1"
                    className={errors.installments ? 'border-red-500' : ''}
                  />
                  {errors.installments && <p className="text-sm text-red-600 mt-1">{errors.installments}</p>}
                </div>

                <div>
                  <label htmlFor="discountSource" className="block text-sm font-medium text-gray-700 mb-1">
                    Fonte de Desconto <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="discountSource"
                    type="text"
                    value={formData.discountSource}
                    onChange={(e) => handleChange('discountSource', e.target.value)}
                    placeholder="Ex: Folha de Pagamento, Vale Alimentação"
                    className={errors.discountSource ? 'border-red-500' : ''}
                  />
                  {errors.discountSource && <p className="text-sm text-red-600 mt-1">{errors.discountSource}</p>}
                </div>

                <div>
                  <label htmlFor="startDate" className="block text-sm font-medium text-gray-700 mb-1">
                    Data de Início <span className="text-red-500">*</span>
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

                <div className="flex items-center">
                  <input
                    id="isApproved"
                    type="checkbox"
                    checked={formData.isApproved}
                    onChange={(e) => handleChange('isApproved', e.target.checked)}
                    className="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
                  />
                  <label htmlFor="isApproved" className="ml-2 block text-sm text-gray-700">
                    Empréstimo aprovado
                  </label>
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
