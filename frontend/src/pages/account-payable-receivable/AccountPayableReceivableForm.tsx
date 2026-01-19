import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Select } from '../../components/ui/Select';
import { CurrencyInput } from '../../components/ui/CurrencyInput';
import { EntityPicker, type EntityPickerItem } from '../../components/ui/EntityPicker';
import { CostCenterDistribution, type CostCenterDistributionItem } from '../../components/ui/CostCenterDistribution';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { useToast } from '../../contexts/ToastContext';
import accountPayableReceivableService from '../../services/accountPayableReceivableService';
import supplierCustomerService from '../../services/supplierCustomerService';
import accountService from '../../services/accountService';
import costCenterService from '../../services/costCenterService';
import { ArrowLeft, Save } from 'lucide-react';

interface AccountPayableReceivableFormData {
  supplierCustomerId: string;
  supplierCustomerName: string;
  accountId: string;
  accountName: string;
  description: string;
  type: string;
  amount: string;
  dueDate: string;
  isPaid: boolean;
  paymentDate: string;
}

export function AccountPayableReceivableForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess, handleBackendError } = useToast();
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  
  const [formData, setFormData] = useState<AccountPayableReceivableFormData>({
    supplierCustomerId: '',
    supplierCustomerName: '',
    accountId: '',
    accountName: '',
    description: '',
    type: 'Pagar',
    amount: '0',
    dueDate: '',
    isPaid: false,
    paymentDate: '',
  });

  const [costCenters, setCostCenters] = useState<CostCenterDistributionItem[]>([]);
  const [availableCostCenters, setAvailableCostCenters] = useState<any[]>([]);
  const [errors, setErrors] = useState<Partial<Record<keyof AccountPayableReceivableFormData, string>>>({});

  const isEditing = !!id;

  useEffect(() => {
    if (isEditing) {
      loadAccountPayableReceivable();
    }
  }, [id]);

  useEffect(() => {
    loadAccountsAndCostCenters();
  }, []);

  const loadAccountsAndCostCenters = async () => {
    try {
      const [accountsData, costCentersData] = await Promise.all([
        accountService.getAccounts({ page: 1, pageSize: 100 }),
        costCenterService.getCostCenters({ page: 1, pageSize: 100 })
      ]);
      
      setAvailableCostCenters(costCentersData.items);
      
      // Auto-selecionar se houver apenas 1 conta e não estiver editando
      if (accountsData.items.length === 1 && !isEditing) {
        setFormData(prev => ({
          ...prev,
          accountId: accountsData.items[0].accountId.toString(),
          accountName: accountsData.items[0].name
        }));
      }
      
      // Auto-selecionar se houver apenas 1 centro de custo e não estiver editando
      if (costCentersData.items.length === 1 && !isEditing) {
        setCostCenters([{
          costCenterId: costCentersData.items[0].costCenterId.toString(),
          costCenterName: costCentersData.items[0].name,
          percentage: 100,
          amount: 0
        }]);
      }
    } catch (err) {
      console.error('Erro ao carregar opções:', err);
    }
  };

  const loadAccountPayableReceivable = async () => {
    setIsLoading(true);
    try {
      const account = await accountPayableReceivableService.getAccountPayableReceivableById(Number(id));
      setFormData({
        supplierCustomerId: account.supplierCustomerId?.toString() || '',
        supplierCustomerName: account.supplierCustomerName || '',
        accountId: account.accountId?.toString() || '',
        accountName: account.accountName || '',
        description: account.description,
        type: account.type,
        amount: account.amount.toString(),
        dueDate: account.dueDate.split('T')[0],
        isPaid: account.isPaid,
        paymentDate: '',
      });
      
      // Carregar cost centers se houver
      if (account.costCenterDistributions && account.costCenterDistributions.length > 0) {
        setCostCenters(account.costCenterDistributions.map((cc: any) => ({
          costCenterId: cc.costCenterId.toString(),
          costCenterName: cc.costCenterName || '',
          amount: cc.amount,
          percentage: cc.percentage
        })));
      }
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

    // Conta corrente é obrigatória quando marcado como pago
    if (formData.isPaid && !formData.accountId) {
      newErrors.accountId = 'Conta corrente é obrigatória quando a conta está marcada como paga';
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

    // Validar centros de custo se houver
    if (costCenters.length > 0) {
      const totalPercentage = costCenters.reduce((sum, c) => sum + c.percentage, 0);
      
      if (Math.abs(totalPercentage - 100) > 0.01) {
        showError('A soma das porcentagens dos centros de custo deve ser 100%');
        return;
      }

      const hasEmptyCostCenter = costCenters.some((c) => !c.costCenterId);
      if (hasEmptyCostCenter) {
        showError('Selecione um centro de custo para todas as distribuições');
        return;
      }
    }

    setIsSaving(true);
    try {
      const accountData = {
        supplierCustomerId: formData.supplierCustomerId ? Number(formData.supplierCustomerId) : undefined,
        accountId: formData.accountId ? Number(formData.accountId) : undefined,
        description: formData.description.trim(),
        type: formData.type.trim(),
        amount: Number(formData.amount),
        dueDate: new Date(formData.dueDate).toISOString(),
        isPaid: formData.isPaid,
        paymentDate: formData.isPaid && formData.paymentDate ? new Date(formData.paymentDate).toISOString() : undefined,
        costCenterDistributions:
          costCenters.length > 0
            ? costCenters.map((c) => ({
                costCenterId: Number(c.costCenterId),
                percentage: c.percentage,
                amount: c.amount,
              }))
            : undefined,
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

  const handleAccountChange = (item: EntityPickerItem | null) => {
    setFormData(prev => ({
      ...prev,
      accountId: item ? item.id.toString() : '',
      accountName: item ? item.displayText : ''
    }));
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
                  <Select
                    id="type"
                    value={formData.type}
                    onChange={(e) => handleChange('type', e.target.value)}
                    className={errors.type ? 'border-red-500' : ''}
                  >
                    <option value="Pagar">Pagar</option>
                    <option value="Receber">Receber</option>
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

                {formData.isPaid && (
                  <>
                    <div>
                      <label htmlFor="paymentDate" className="block text-sm font-medium text-gray-700 mb-1">
                        Data do Pagamento
                      </label>
                      <Input
                        id="paymentDate"
                        type="date"
                        value={formData.paymentDate}
                        onChange={(e) => handleChange('paymentDate', e.target.value)}
                      />
                      <p className="text-xs text-gray-500 mt-1">Se não informada, será usada a data de vencimento</p>
                    </div>
                    <div>
                      <label htmlFor="accountId" className="block text-sm font-medium text-gray-700 mb-1">
                        Conta Corrente <span className="text-red-500">*</span>
                      </label>
                      <EntityPicker
                        value={formData.accountId ? Number(formData.accountId) : null}
                        selectedLabel={formData.accountName}
                        onChange={handleAccountChange}
                        onSearch={handleSearchAccount}
                        placeholder="Selecione uma conta corrente"
                        label="Selecionar Conta Corrente"
                      />
                      {errors.accountId && <p className="text-sm text-red-600 mt-1">{errors.accountId}</p>}
                    </div>
                  </>
                )}
              </div>

            </CardContent>
          </Card>

          {/* Rateio de Centros de Custo - só exibe se tiver mais de 1 */}
          {availableCostCenters.length > 1 && (
            <Card>
              <CardHeader>
                <CardTitle>Distribuição por Centro de Custo</CardTitle>
              </CardHeader>
              <CardContent className="p-6">
                <CostCenterDistribution
                  distributions={costCenters}
                  onChange={setCostCenters}
                  totalAmount={Number(formData.amount) || 0}
                />
              </CardContent>
            </Card>
          )}

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
