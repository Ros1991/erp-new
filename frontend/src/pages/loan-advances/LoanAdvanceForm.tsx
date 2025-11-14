import { useEffect, useState, useMemo } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { CurrencyInput } from '../../components/ui/CurrencyInput';
import { NumberInput } from '../../components/ui/NumberInput';
import { Select } from '../../components/ui/Select';
import { EntityPicker, type EntityPickerItem } from '../../components/ui/EntityPicker';
import { CostCenterDistribution, type CostCenterDistributionItem } from '../../components/ui/CostCenterDistribution';
import { useToast } from '../../contexts/ToastContext';
import { useAutoSelect } from '../../hooks/useAutoSelect';
import loanAdvanceService from '../../services/loanAdvanceService';
import employeeService from '../../services/employeeService';
import accountService from '../../services/accountService';
import contractService from '../../services/contractService';
import costCenterService from '../../services/costCenterService';
import { toUTCString } from '../../utils/dateUtils';
import { ArrowLeft, Save, Info } from 'lucide-react';

interface LoanAdvanceFormData {
  employeeId: string;
  employeeName: string;
  accountId: string;
  accountName: string;
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
    accountId: '',
    accountName: '',
    amount: '0',
    installments: 1,
    discountSource: 'Todos',
    startDate: new Date().toISOString().split('T')[0], // Data de hoje
  });

  const [costCenters, setCostCenters] = useState<CostCenterDistributionItem[]>([]);
  const [errors, setErrors] = useState<Partial<Record<keyof LoanAdvanceFormData, string>>>({});
  
  // Estados para auto-seleção
  const [availableAccounts, setAvailableAccounts] = useState<any[]>([]);
  const [availableCostCenters, setAvailableCostCenters] = useState<any[]>([]);

  const isEditing = !!id;

  // Auto-select hooks
  const accountAutoSelect = useAutoSelect(
    availableAccounts.length,
    'conta',
    formData.accountId && availableAccounts.length > 0
      ? availableAccounts.find(a => a.accountId.toString() === formData.accountId) || null
      : null
  );

  const costCenterAutoSelect = useAutoSelect(
    availableCostCenters.length,
    'centro de custo',
    null
  );

  // Carregar contas e centros de custo ao montar
  useEffect(() => {
    loadAccountsAndCostCenters();
  }, []);

  useEffect(() => {
    if (isEditing) {
      loadLoanAdvance();
    }
  }, [id]);

  const loadAccountsAndCostCenters = async () => {
    try {
      const [accountsData, costCentersData] = await Promise.all([
        accountService.getAccounts({ page: 1, pageSize: 100 }),
        costCenterService.getCostCenters({ page: 1, pageSize: 100 })
      ]);
      
      setAvailableAccounts(accountsData.items);
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

  const loadLoanAdvance = async () => {
    setIsLoading(true);
    try {
      const loanAdvance = await loanAdvanceService.getLoanAdvanceById(Number(id));
      setFormData({
        employeeId: loanAdvance.employeeId.toString(),
        employeeName: loanAdvance.employeeName || '',
        accountId: '', // Não retornado pelo backend na edição
        accountName: '',
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

    // accountId agora é opcional
    // if (!formData.accountId) {
    //   newErrors.accountId = 'Conta é obrigatória';
    // }

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
      const loanAdvanceData = {
        employeeId: Number(formData.employeeId),
        amount: Number(formData.amount),
        installments: formData.installments,
        discountSource: formData.discountSource.trim(),
        startDate: toUTCString(new Date(formData.startDate))!,
        isApproved: true, // Sempre aprovado
        accountId: formData.accountId ? Number(formData.accountId) : null, // null se não houver conta
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

  const handleEmployeeChange = async (item: EntityPickerItem | null) => {
    setFormData(prev => ({
      ...prev,
      employeeId: item ? item.id.toString() : '',
      employeeName: item ? item.displayText : ''
    }));
    if (errors.employeeId) {
      setErrors(prev => ({ ...prev, employeeId: undefined }));
    }

    // Buscar contrato ativo do funcionário para carregar centros de custo automaticamente
    if (item) {
      try {
        const contract = await contractService.getActiveByEmployeeId(item.id);
        if (contract && contract.costCenters && contract.costCenters.length > 0) {
          const costCenterItems: CostCenterDistributionItem[] = contract.costCenters.map(cc => ({
            costCenterId: cc.costCenterId.toString(),
            costCenterName: cc.costCenterName || '',
            percentage: cc.percentage,
            amount: 0, // Será calculado automaticamente pelo componente
          }));
          setCostCenters(costCenterItems);
        } else {
          setCostCenters([]);
        }
      } catch (error) {
        console.error('Erro ao buscar contrato ativo:', error);
        setCostCenters([]);
      }
    } else {
      setCostCenters([]);
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
          displayText: item.nickname,
          secondaryText: item.fullName || undefined
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
            <CardHeader>
              <CardTitle>Dados do Empréstimo/Adiantamento</CardTitle>
            </CardHeader>
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
                  {formData.employeeId && costCenters.length > 0 && (
                    <p className="text-xs text-green-600 mt-1">
                      ✓ Centros de custo carregados automaticamente do contrato ativo
                    </p>
                  )}
                </div>

                {/* Conta - Mostrar apenas se houver 2+ contas */}
                {accountAutoSelect.shouldShow && (
                  <div>
                    <label htmlFor="accountId" className="block text-sm font-medium text-gray-700 mb-1">
                      Conta
                    </label>
                    <EntityPicker
                      value={formData.accountId ? Number(formData.accountId) : null}
                      selectedLabel={formData.accountName}
                      onChange={handleAccountChange}
                      onSearch={handleSearchAccount}
                      placeholder="Selecione a conta de saída"
                      label="Selecionar Conta"
                      className={errors.accountId ? 'border-red-500' : ''}
                    />
                    {errors.accountId && <p className="text-sm text-red-600 mt-1">{errors.accountId}</p>}
                  </div>
                )}

                {/* Mensagem informativa sobre conta - apenas quando auto-selecionada */}
                {accountAutoSelect.message && accountAutoSelect.autoSelected && (
                  <div className="bg-blue-50 border border-blue-200 rounded-lg p-3 flex items-start gap-2">
                    <Info className="h-4 w-4 text-blue-600 mt-0.5 flex-shrink-0" />
                    <p className="text-sm text-blue-900">{accountAutoSelect.message}</p>
                  </div>
                )}

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

          {/* Centros de Custo - Mostrar apenas se houver centros disponíveis */}
          {availableCostCenters.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle>Centros de Custo</CardTitle>
              </CardHeader>
              <CardContent className="p-6">
                {/* Mensagem informativa sobre centros de custo - apenas quando auto-selecionado */}
                {costCenterAutoSelect.message && costCenters.length === 1 && costCenterAutoSelect.autoSelected && (
                  <div className="bg-blue-50 border border-blue-200 rounded-lg p-3 flex items-start gap-2 mb-4">
                    <Info className="h-4 w-4 text-blue-600 mt-0.5 flex-shrink-0" />
                    <p className="text-sm text-blue-900">{costCenterAutoSelect.message}</p>
                  </div>
                )}
                
                <CostCenterDistribution
                  totalAmount={Number(formData.amount) / 100} // Converter de centavos para reais
                  distributions={costCenters}
                  onChange={setCostCenters}
                  readonly={availableCostCenters.length === 1}
                />
                {costCenters.length === 0 && (
                  <div className="text-sm text-muted-foreground text-center py-4">
                    {formData.employeeId 
                      ? 'Nenhum centro de custo carregado. Você pode adicionar manualmente ou o funcionário não possui contrato ativo.'
                      : 'Selecione um funcionário para carregar automaticamente os centros de custo do contrato ativo.'}
                  </div>
                )}
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
