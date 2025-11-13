import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Card, CardContent } from '../../components/ui/Card';
import { useToast } from '../../contexts/ToastContext';
import accountPayableReceivableService from '../../services/accountPayableReceivableService';
import { ArrowLeft, Save } from 'lucide-react';

interface AccountPayableReceivableFormData {
  supplierCustomerId: string;
  description: string;
  type: string;
  amount: string;
  dueDate: string;
  isPaid: boolean;
}

export function AccountPayableReceivableForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess, handleBackendError } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  
  const [formData, setFormData] = useState<AccountPayableReceivableFormData>({
    supplierCustomerId: '',
    description: '',
    type: '',
    amount: '0',
    dueDate: '',
    isPaid: false,
  });

  const [errors, setErrors] = useState<Partial<Record<keyof AccountPayableReceivableFormData, string>>>({});

  const isEditing = !!id;

  useEffect(() => {
    if (isEditing) {
      loadAccountPayableReceivable();
    }
  }, [id]);

  const loadAccountPayableReceivable = async () => {
    setIsLoading(true);
    try {
      const account = await accountPayableReceivableService.getAccountPayableReceivableById(Number(id));
      setFormData({
        supplierCustomerId: account.supplierCustomerId?.toString() || '',
        description: account.description,
        type: account.type,
        amount: account.amount.toString(),
        dueDate: account.dueDate.split('T')[0],
        isPaid: account.isPaid,
      });
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsLoading(false);
    }
  };

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof AccountPayableReceivableFormData, string>> = {};

    if (!formData.description.trim()) {
      newErrors.description = 'Descrição é obrigatória';
    }

    if (!formData.type.trim()) {
      newErrors.type = 'Tipo é obrigatório';
    }

    if (!formData.amount.trim()) {
      newErrors.amount = 'Valor é obrigatório';
    } else if (isNaN(Number(formData.amount))) {
      newErrors.amount = 'Valor deve ser um número válido';
    }

    if (!formData.dueDate) {
      newErrors.dueDate = 'Data de vencimento é obrigatória';
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
      const accountData = {
        supplierCustomerId: formData.supplierCustomerId ? Number(formData.supplierCustomerId) : undefined,
        description: formData.description.trim(),
        type: formData.type.trim(),
        amount: Number(formData.amount),
        dueDate: new Date(formData.dueDate).toISOString(),
        isPaid: formData.isPaid,
      };

      if (isEditing) {
        await accountPayableReceivableService.updateAccountPayableReceivable(Number(id), accountData);
        showSuccess('Conta atualizada com sucesso!');
      } else {
        await accountPayableReceivableService.createAccountPayableReceivable(accountData);
        showSuccess('Conta criada com sucesso!');
      }

      navigate('/account-payable-receivable');
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsSaving(false);
    }
  };

  const handleChange = (field: keyof AccountPayableReceivableFormData, value: string | boolean) => {
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
            onClick={() => navigate('/account-payable-receivable')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <h1 className="text-3xl font-bold text-gray-900">
            {isEditing ? 'Editar Conta a Pagar e Receber' : 'Nova Conta a Pagar e Receber'}
          </h1>
          <p className="text-gray-600 mt-1">
            {isEditing ? 'Atualize as informações da conta' : 'Preencha as informações para criar uma nova conta'}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <Card>
            <CardContent className="p-6">
              <div className="space-y-4">
                <div>
                  <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1">
                    Descrição <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="description"
                    type="text"
                    value={formData.description}
                    onChange={(e) => handleChange('description', e.target.value)}
                    placeholder="Ex: Aluguel, Fornecedor XYZ"
                    className={errors.description ? 'border-red-500' : ''}
                  />
                  {errors.description && <p className="text-sm text-red-600 mt-1">{errors.description}</p>}
                </div>

                <div>
                  <label htmlFor="type" className="block text-sm font-medium text-gray-700 mb-1">
                    Tipo <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="type"
                    type="text"
                    value={formData.type}
                    onChange={(e) => handleChange('type', e.target.value)}
                    placeholder="Ex: Pagar, Receber"
                    className={errors.type ? 'border-red-500' : ''}
                  />
                  {errors.type && <p className="text-sm text-red-600 mt-1">{errors.type}</p>}
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
                  <p className="text-sm text-gray-500 mt-1">Digite o valor em centavos (ex: 10000 = R$ 100,00)</p>
                </div>

                <div>
                  <label htmlFor="dueDate" className="block text-sm font-medium text-gray-700 mb-1">
                    Data de Vencimento <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="dueDate"
                    type="date"
                    value={formData.dueDate}
                    onChange={(e) => handleChange('dueDate', e.target.value)}
                    className={errors.dueDate ? 'border-red-500' : ''}
                  />
                  {errors.dueDate && <p className="text-sm text-red-600 mt-1">{errors.dueDate}</p>}
                </div>

                <div>
                  <label htmlFor="supplierCustomerId" className="block text-sm font-medium text-gray-700 mb-1">
                    ID do Fornecedor e Cliente (opcional)
                  </label>
                  <Input
                    id="supplierCustomerId"
                    type="text"
                    value={formData.supplierCustomerId}
                    onChange={(e) => handleChange('supplierCustomerId', e.target.value)}
                    placeholder="ID do fornecedor e cliente"
                  />
                </div>

                <div className="flex items-center">
                  <input
                    id="isPaid"
                    type="checkbox"
                    checked={formData.isPaid}
                    onChange={(e) => handleChange('isPaid', e.target.checked)}
                    className="h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 rounded"
                  />
                  <label htmlFor="isPaid" className="ml-2 block text-sm text-gray-700">
                    Conta paga
                  </label>
                </div>
              </div>

            </CardContent>
          </Card>

          <div className="flex gap-3">
                <Button type="submit" disabled={isSaving} className="flex-1 sm:flex-none">
                  {isSaving ? (
                    <>
                      <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                      Salvando...
                    </>
                  ) : (
                    <>
                      <Save className="h-4 w-4 mr-2" />
                      {isEditing ? 'Atualizar' : 'Criar'} Conta
                    </>
                  )}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => navigate('/account-payable-receivable')}
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
