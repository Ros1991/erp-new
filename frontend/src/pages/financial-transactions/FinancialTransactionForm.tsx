import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Select } from '../../components/ui/Select';
import { CurrencyInput } from '../../components/ui/CurrencyInput';
import { EntityPicker, type EntityPickerItem } from '../../components/ui/EntityPicker';
import { Card, CardContent } from '../../components/ui/Card';
import { useToast } from '../../contexts/ToastContext';
import financialTransactionService from '../../services/financialTransactionService';
import accountService from '../../services/accountService';
import supplierCustomerService from '../../services/supplierCustomerService';
import { ArrowLeft, Save } from 'lucide-react';

interface FinancialTransactionFormData {
  accountId: string;
  accountName: string;
  supplierCustomerId: string;
  supplierCustomerName: string;
  description: string;
  type: string;
  amount: string;
  transactionDate: string;
}

export function FinancialTransactionForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess, handleBackendError } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  
  const [formData, setFormData] = useState<FinancialTransactionFormData>({
    accountId: '',
    accountName: '',
    supplierCustomerId: '',
    supplierCustomerName: '',
    description: '',
    type: 'Entrada',
    amount: '0',
    transactionDate: new Date().toISOString().split('T')[0],
  });

  const [errors, setErrors] = useState<Partial<Record<keyof FinancialTransactionFormData, string>>>({});

  const isEditing = !!id;

  useEffect(() => {
    if (isEditing) {
      loadFinancialTransaction();
    }
  }, [id]);

  const loadFinancialTransaction = async () => {
    setIsLoading(true);
    try {
      const transaction = await financialTransactionService.getFinancialTransactionById(Number(id));
      setFormData({
        accountId: transaction.accountId.toString(),
        accountName: transaction.accountName || '',
        supplierCustomerId: transaction.supplierCustomerId?.toString() || '',
        supplierCustomerName: transaction.supplierCustomerName || '',
        description: transaction.description,
        type: transaction.type,
        amount: transaction.amount.toString(),
        transactionDate: transaction.transactionDate.split('T')[0],
      });
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsLoading(false);
    }
  };

  const validate = (): boolean => {
    const newErrors: Partial<Record<keyof FinancialTransactionFormData, string>> = {};

    if (!formData.accountId) {
      newErrors.accountId = 'Conta é obrigatória';
    }

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

    if (!formData.transactionDate) {
      newErrors.transactionDate = 'Data da transação é obrigatória';
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
      const transactionData = {
        accountId: Number(formData.accountId),
        supplierCustomerId: formData.supplierCustomerId ? Number(formData.supplierCustomerId) : undefined,
        description: formData.description.trim(),
        type: formData.type.trim(),
        amount: Number(formData.amount),
        transactionDate: new Date(formData.transactionDate).toISOString(),
      };

      if (isEditing) {
        await financialTransactionService.updateFinancialTransaction(Number(id), transactionData);
        showSuccess('Transação atualizada com sucesso!');
      } else {
        await financialTransactionService.createFinancialTransaction(transactionData);
        showSuccess('Transação criada com sucesso!');
      }

      navigate('/financial-transactions');
    } catch (err: any) {
      handleBackendError(err);
    } finally {
      setIsSaving(false);
    }
  };

  const handleSearchAccount = async (searchTerm: string, page: number) => {
    try {
      const result = await accountService.getAccounts({
        search: searchTerm,
        page: page,
        pageSize: 10,
      });

      return {
        items: result.items.map(item => ({
          id: item.accountId,
          displayText: item.name,
          secondaryText: item.type || undefined
        })),
        totalPages: result.totalPages,
        totalCount: result.totalCount
      };
    } catch (error) {
      console.error('Erro ao buscar contas:', error);
      return {
        items: [],
        totalPages: 1,
        totalCount: 0
      };
    }
  };

  const handleAccountChange = (item: EntityPickerItem | null) => {
    setFormData(prev => ({
      ...prev,
      accountId: item ? item.id.toString() : '',
      accountName: item ? item.displayText : ''
    }));
    if (errors.accountId) {
      setErrors(prev => ({ ...prev, accountId: undefined }));
    }
  };

  const handleSearchSupplierCustomer = async (searchTerm: string, page: number) => {
    try {
      const result = await supplierCustomerService.getSupplierCustomers({
        search: searchTerm,
        page: page,
        pageSize: 10,
      });

      return {
        items: result.items.map(item => ({
          id: item.supplierCustomerId,
          displayText: item.name,
          secondaryText: item.document || item.email || undefined
        })),
        totalPages: result.totalPages,
        totalCount: result.totalCount
      };
    } catch (error) {
      console.error('Erro ao buscar fornecedores/clientes:', error);
      return {
        items: [],
        totalPages: 1,
        totalCount: 0
      };
    }
  };

  const handleSupplierCustomerChange = (item: EntityPickerItem | null) => {
    setFormData(prev => ({
      ...prev,
      supplierCustomerId: item ? item.id.toString() : '',
      supplierCustomerName: item ? item.displayText : ''
    }));
  };

  const handleChange = (field: keyof FinancialTransactionFormData, value: string) => {
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
            onClick={() => navigate('/financial-transactions')}
            className="mb-4"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Button>
          <h1 className="text-3xl font-bold text-gray-900">
            {isEditing ? 'Editar Transação Financeira' : 'Nova Transação Financeira'}
          </h1>
          <p className="text-gray-600 mt-1">
            {isEditing ? 'Atualize as informações da transação' : 'Preencha as informações para criar uma nova transação'}
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <Card>
            <CardContent className="p-6">
              <div className="space-y-4">
                <div>
                  <label htmlFor="accountId" className="block text-sm font-medium text-gray-700 mb-1">
                    Conta Corrente <span className="text-red-500">*</span>
                  </label>
                  <EntityPicker
                    value={formData.accountId ? Number(formData.accountId) : null}
                    selectedLabel={formData.accountName}
                    onChange={handleAccountChange}
                    onSearch={handleSearchAccount}
                    placeholder="Selecione uma conta"
                    label="Selecionar Conta Corrente"
                    className={errors.accountId ? 'border-red-500' : ''}
                  />
                  {errors.accountId && <p className="text-sm text-red-600 mt-1">{errors.accountId}</p>}
                </div>

                <div>
                  <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1">
                    Descrição <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="description"
                    type="text"
                    value={formData.description}
                    onChange={(e) => handleChange('description', e.target.value)}
                    placeholder="Ex: Pagamento de fornecedor, Recebimento de cliente"
                    className={errors.description ? 'border-red-500' : ''}
                  />
                  {errors.description && <p className="text-sm text-red-600 mt-1">{errors.description}</p>}
                </div>

                <div>
                  <label htmlFor="type" className="block text-sm font-medium text-gray-700 mb-1">
                    Tipo <span className="text-red-500">*</span>
                  </label>
                  <Select
                    id="type"
                    value={formData.type}
                    onChange={(e) => handleChange('type', e.target.value)}
                    className={errors.type ? 'border-red-500' : ''}
                  >
                    <option value="Entrada">Entrada</option>
                    <option value="Saída">Saída</option>
                  </Select>
                  {errors.type && <p className="text-sm text-red-600 mt-1">{errors.type}</p>}
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
                  <label htmlFor="transactionDate" className="block text-sm font-medium text-gray-700 mb-1">
                    Data da Transação <span className="text-red-500">*</span>
                  </label>
                  <Input
                    id="transactionDate"
                    type="date"
                    value={formData.transactionDate}
                    onChange={(e) => handleChange('transactionDate', e.target.value)}
                    className={errors.transactionDate ? 'border-red-500' : ''}
                  />
                  {errors.transactionDate && <p className="text-sm text-red-600 mt-1">{errors.transactionDate}</p>}
                </div>

                <div>
                  <label htmlFor="supplierCustomerId" className="block text-sm font-medium text-gray-700 mb-1">
                    Fornecedor ou Cliente (opcional)
                  </label>
                  <EntityPicker
                    value={formData.supplierCustomerId ? Number(formData.supplierCustomerId) : null}
                    selectedLabel={formData.supplierCustomerName}
                    onChange={handleSupplierCustomerChange}
                    onSearch={handleSearchSupplierCustomer}
                    placeholder="Selecione um fornecedor ou cliente"
                    label="Selecionar Fornecedor ou Cliente"
                  />
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
                      {isEditing ? 'Atualizar' : 'Criar'} Transação
                    </>
                  )}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => navigate('/financial-transactions')}
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
