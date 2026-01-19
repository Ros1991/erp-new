import { useEffect, useState, useRef } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Card, CardContent } from '../../components/ui/Card';
import { ConfirmDialog } from '../../components/ui/ConfirmDialog';
import { Protected } from '../../components/permissions/Protected';
import { useToast } from '../../contexts/ToastContext';
import { useAuth } from '../../contexts/AuthContext';
import payrollService, { type PayrollDetailed, type PayrollItem, type PayrollEmployeeDetailed } from '../../services/payrollService';
import accountService, { type Account } from '../../services/accountService';
import { parseBackendError } from '../../utils/errorHandler';
import { EditPayrollItemDialog } from './EditPayrollItemDialog';
import { WorkedUnitsDialog } from './WorkedUnitsDialog';
import { PayrollPrintReport } from './PayrollPrintReport';
import { 
  ArrowLeft,
  Calendar,
  Users,
  DollarSign,
  Lock,
  Unlock,
  FileText,
  TrendingUp,
  TrendingDown,
  ChevronDown,
  ChevronUp,
  RefreshCw,
  Clock,
  Edit2,
  Trash2,
  CheckCircle,
  Printer,
  Gift,
  Palmtree,
  ExternalLink,
  Plus
} from 'lucide-react';
import { Input } from '../../components/ui/Input';
import { Label } from '../../components/ui/Label';
import { CurrencyInput } from '../../components/ui/CurrencyInput';

export function PayrollDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess } = useToast();
  const { selectedCompany } = useAuth();
  const printRef = useRef<HTMLDivElement>(null);
  const [payroll, setPayroll] = useState<PayrollDetailed | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [expandedEmployees, setExpandedEmployees] = useState<Set<number>>(new Set());
  const [recalculateDialogOpen, setRecalculateDialogOpen] = useState(false);
  const [isRecalculating, setIsRecalculating] = useState(false);
  
  // Estados para edição de item
  const [editItemDialogOpen, setEditItemDialogOpen] = useState(false);
  const [selectedItem, setSelectedItem] = useState<PayrollItem | null>(null);
  const [isUpdatingItem, setIsUpdatingItem] = useState(false);
  
  // Estados para horas/dias trabalhados
  const [workedUnitsDialogOpen, setWorkedUnitsDialogOpen] = useState(false);
  const [selectedEmployee, setSelectedEmployee] = useState<PayrollEmployeeDetailed | null>(null);
  const [isUpdatingWorkedUnits, setIsUpdatingWorkedUnits] = useState(false);
  
  // Estados para recalcular funcionário específico
  const [recalculateEmployeeDialogOpen, setRecalculateEmployeeDialogOpen] = useState(false);
  const [employeeToRecalculate, setEmployeeToRecalculate] = useState<PayrollEmployeeDetailed | null>(null);
  const [isRecalculatingEmployee, setIsRecalculatingEmployee] = useState(false);

  // Estados para ações da folha
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [payDialogOpen, setPayDialogOpen] = useState(false);
  const [isPaying, setIsPaying] = useState(false);
  const [paymentDate, setPaymentDate] = useState('');
  const [inssAmount, setInssAmount] = useState(0);
  const [fgtsAmount, setFgtsAmount] = useState(0);
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [selectedAccountId, setSelectedAccountId] = useState<number | null>(null);
  const [isReopening, setIsReopening] = useState(false);
  const [reopenDialogOpen, setReopenDialogOpen] = useState(false);
  const [thirteenthDialogOpen, setThirteenthDialogOpen] = useState(false);
  const [thirteenthPercentage, setThirteenthPercentage] = useState(100);
  const [thirteenthTaxOption, setThirteenthTaxOption] = useState<'none' | 'proportional' | 'full'>('none');
  const [isGenerating13th, setIsGenerating13th] = useState(false);

  // Estados para ações do funcionário
  const [vacationDialogOpen, setVacationDialogOpen] = useState(false);
  const [vacationEmployee, setVacationEmployee] = useState<PayrollEmployeeDetailed | null>(null);
  const [vacationStartDate, setVacationStartDate] = useState('');
  const [vacationDays, setVacationDays] = useState(30);
  const [vacationIncludeTaxes, setVacationIncludeTaxes] = useState(true);
  const [vacationAdvanceNextMonth, setVacationAdvanceNextMonth] = useState(true);
  const [vacationNotes, setVacationNotes] = useState('');
  const [isAddingVacation, setIsAddingVacation] = useState(false);
  const [addItemDialogOpen, setAddItemDialogOpen] = useState(false);
  const [addItemEmployee, setAddItemEmployee] = useState<PayrollEmployeeDetailed | null>(null);
  const [newItemType, setNewItemType] = useState<'Provento' | 'Desconto'>('Provento');
  const [newItemDescription, setNewItemDescription] = useState('');
  const [newItemAmount, setNewItemAmount] = useState(0);
  const [isAddingItem, setIsAddingItem] = useState(false);

  useEffect(() => {
    loadPayrollDetails();
    loadAccounts();
  }, [id]);

  const loadAccounts = async () => {
    try {
      const result = await accountService.getAccounts({ pageSize: 100 });
      setAccounts(result.items);
      if (result.items.length > 0) {
        setSelectedAccountId(result.items[0].accountId);
      }
    } catch (error) {
      console.error('Erro ao carregar contas:', error);
    }
  };

  const handleRecalculate = async () => {
    if (!id) return;
    
    setIsRecalculating(true);
    try {
      const data = await payrollService.recalculatePayroll(parseInt(id));
      setPayroll(data);
      setExpandedEmployees(new Set()); // Funcionários fechados por padrão
      showSuccess('Folha de pagamento recalculada com sucesso!');
      setRecalculateDialogOpen(false);
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsRecalculating(false);
    }
  };

  const loadPayrollDetails = async () => {
    if (!id) return;
    
    setIsLoading(true);
    try {
      const data = await payrollService.getPayrollDetails(parseInt(id));
      setPayroll(data);
      // Manter funcionários fechados por padrão
      setExpandedEmployees(new Set());
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
      navigate('/payroll');
    } finally {
      setIsLoading(false);
    }
  };

  const toggleEmployee = (employeeId: number) => {
    setExpandedEmployees(prev => {
      const newSet = new Set(prev);
      if (newSet.has(employeeId)) {
        newSet.delete(employeeId);
      } else {
        newSet.add(employeeId);
      }
      return newSet;
    });
  };

  // Handler para abrir edição de item
  const handleEditItem = (item: PayrollItem) => {
    if (payroll?.isClosed) return;
    setSelectedItem(item);
    setEditItemDialogOpen(true);
  };

  // Handler para salvar edição de item
  const handleSaveItem = async (data: { description: string; amount: number }) => {
    if (!selectedItem || !payroll) return;
    
    setIsUpdatingItem(true);
    try {
      const updatedItem = await payrollService.updatePayrollItem(selectedItem.payrollItemId, data);
      // Atualizar estado local com dados retornados da API
      const updatedPayroll = payrollService.updatePayrollItemLocally(payroll, updatedItem);
      setPayroll(updatedPayroll);
      showSuccess('Item atualizado com sucesso!');
      setEditItemDialogOpen(false);
      setSelectedItem(null);
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsUpdatingItem(false);
    }
  };

  // Handler para abrir dialog de horas/dias trabalhados
  const handleEditWorkedUnits = (employee: PayrollEmployeeDetailed) => {
    if (payroll?.isClosed) return;
    setSelectedEmployee(employee);
    setWorkedUnitsDialogOpen(true);
  };

  // Handler para salvar horas/dias trabalhados
  const handleSaveWorkedUnits = async (workedUnits: number) => {
    if (!selectedEmployee || !payroll) return;
    
    setIsUpdatingWorkedUnits(true);
    try {
      const updatedEmployee = await payrollService.updateWorkedUnits(selectedEmployee.payrollEmployeeId, { workedUnits });
      // Atualizar estado local com dados retornados da API
      const updatedPayroll = payrollService.updateWorkedUnitsLocally(payroll, updatedEmployee);
      setPayroll(updatedPayroll);
      showSuccess('Horas/dias trabalhados atualizados com sucesso!');
      setWorkedUnitsDialogOpen(false);
      setSelectedEmployee(null);
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsUpdatingWorkedUnits(false);
    }
  };

  // Handler para abrir dialog de recalcular funcionário
  const handleRecalculateEmployeeClick = (employee: PayrollEmployeeDetailed) => {
    setEmployeeToRecalculate(employee);
    setRecalculateEmployeeDialogOpen(true);
  };

  // Handler para recalcular funcionário específico
  const handleRecalculateEmployee = async () => {
    if (!employeeToRecalculate || !payroll) return;
    
    setIsRecalculatingEmployee(true);
    try {
      const updatedEmployee = await payrollService.recalculateEmployee(employeeToRecalculate.payrollEmployeeId);
      // Atualizar estado local com dados retornados da API (usa employeeId pois payrollEmployeeId muda)
      const updatedPayroll = payrollService.replaceEmployeeLocally(payroll, updatedEmployee);
      setPayroll(updatedPayroll);
      showSuccess(`Funcionário ${employeeToRecalculate.employeeName} recalculado com sucesso!`);
      setRecalculateEmployeeDialogOpen(false);
      setEmployeeToRecalculate(null);
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsRecalculatingEmployee(false);
    }
  };

  // Handler para apagar folha
  const handleDeletePayroll = async () => {
    if (!payroll) return;
    
    setIsDeleting(true);
    try {
      await payrollService.deletePayroll(payroll.payrollId);
      showSuccess('Folha de pagamento excluída com sucesso!');
      setDeleteDialogOpen(false);
      navigate('/payroll');
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsDeleting(false);
    }
  };

  // Handler para abrir dialog de pagar folha
  const handleOpenPayDialog = () => {
    if (!payroll) return;
    
    // Calcular data default: dia 5 do mês seguinte ao período
    const periodEnd = new Date(payroll.periodEndDate);
    const payDate = new Date(periodEnd.getFullYear(), periodEnd.getMonth() + 1, 5);
    const formattedDate = payDate.toISOString().split('T')[0];
    setPaymentDate(formattedDate);
    
    // Usar valores salvos no backend
    setInssAmount(payroll.totalInss);
    setFgtsAmount(payroll.totalFgts);
    
    setPayDialogOpen(true);
  };

  // Handler para pagar folha
  const handlePayPayroll = async () => {
    if (!payroll || !selectedAccountId || !paymentDate) return;
    
    setIsPaying(true);
    try {
      const updatedPayroll = await payrollService.closePayroll(payroll.payrollId, {
        accountId: selectedAccountId,
        paymentDate,
        inssAmount,
        fgtsAmount
      });
      setPayroll(updatedPayroll);
      showSuccess('Folha de pagamento fechada com sucesso!');
      setPayDialogOpen(false);
    } catch (error) {
      const { message } = parseBackendError(error);
      showError(message);
    } finally {
      setIsPaying(false);
    }
  };

  // Handler para reabrir folha
  const handleReopenPayroll = async () => {
    if (!payroll) return;
    
    setIsReopening(true);
    try {
      const updatedPayroll = await payrollService.reopenPayroll(payroll.payrollId);
      setPayroll(updatedPayroll);
      showSuccess('Folha de pagamento reaberta com sucesso!');
      setReopenDialogOpen(false);
    } catch (error) {
      const { message } = parseBackendError(error);
      showError(message);
    } finally {
      setIsReopening(false);
    }
  };

  // Handler para imprimir folha
  const handlePrint = () => {
    window.print();
  };

  // Handler para gerar/atualizar 13º
  const handleGenerate13th = async () => {
    if (!payroll) return;
    setIsGenerating13th(true);
    try {
      const updatedPayroll = await payrollService.applyThirteenthSalary(payroll.payrollId, {
        percentage: thirteenthPercentage,
        taxOption: thirteenthTaxOption
      });
      setPayroll(updatedPayroll);
      showSuccess(`13º salário ${payroll.thirteenthPercentage ? 'atualizado' : 'aplicado'} com sucesso!`);
      setThirteenthDialogOpen(false);
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsGenerating13th(false);
    }
  };

  // Handler para remover 13º
  const handleRemove13th = async () => {
    if (!payroll) return;
    setIsGenerating13th(true);
    try {
      const updatedPayroll = await payrollService.removeThirteenthSalary(payroll.payrollId);
      setPayroll(updatedPayroll);
      showSuccess('13º salário removido com sucesso!');
      setThirteenthDialogOpen(false);
      setThirteenthPercentage(100);
      setThirteenthTaxOption('none');
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsGenerating13th(false);
    }
  };

  // Handler para abrir dialog de férias
  const handleAddVacation = (employee: PayrollEmployeeDetailed) => {
    setVacationEmployee(employee);
    // Preencher com valores atuais se já tem férias
    if (employee.isOnVacation && employee.vacationStartDate) {
      setVacationStartDate(employee.vacationStartDate.split('T')[0]);
      setVacationDays(employee.vacationDays || 30);
      setVacationNotes(employee.vacationNotes || '');
    } else {
      // Auto-preencher data início com data inicial da folha
      setVacationStartDate(payroll?.periodStartDate?.split('T')[0] || '');
      setVacationDays(30);
      // Auto-preencher observação com período aquisitivo baseado no ano da folha
      const payrollYear = payroll?.periodStartDate ? new Date(payroll.periodStartDate).getFullYear() : new Date().getFullYear();
      setVacationNotes(`Período aquisitivo ${payrollYear - 1}/${payrollYear}`);
    }
    setVacationIncludeTaxes(true);
    setVacationAdvanceNextMonth(true);
    setVacationDialogOpen(true);
  };

  // Handler para salvar férias
  const handleSaveVacation = async () => {
    if (!payroll || !vacationEmployee) return;
    setIsAddingVacation(true);
    try {
      const updatedPayroll = await payrollService.applyVacation(payroll.payrollId, {
        payrollEmployeeId: vacationEmployee.payrollEmployeeId,
        vacationDays: vacationDays,
        vacationStartDate: vacationStartDate,
        includeTaxes: vacationIncludeTaxes,
        advanceNextMonth: vacationAdvanceNextMonth,
        notes: vacationNotes || undefined
      });
      setPayroll(updatedPayroll);
      showSuccess(`Férias ${vacationEmployee.isOnVacation ? 'atualizadas' : 'aplicadas'} com sucesso!`);
      setVacationDialogOpen(false);
      setVacationEmployee(null);
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsAddingVacation(false);
    }
  };

  // Handler para remover férias
  const handleRemoveVacation = async () => {
    if (!payroll || !vacationEmployee) return;
    setIsAddingVacation(true);
    try {
      const updatedPayroll = await payrollService.removeVacation(payroll.payrollId, vacationEmployee.payrollEmployeeId);
      setPayroll(updatedPayroll);
      showSuccess('Férias removidas com sucesso!');
      setVacationDialogOpen(false);
      setVacationEmployee(null);
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsAddingVacation(false);
    }
  };

  // Handler para abrir contrato em nova aba
  const handleOpenContract = (employee: PayrollEmployeeDetailed) => {
    window.open(`/employees/${employee.employeeId}/contracts`, '_blank');
  };

  // Handler para abrir dialog de adicionar item
  const handleAddItem = (employee: PayrollEmployeeDetailed) => {
    setAddItemEmployee(employee);
    setNewItemType('Provento');
    setNewItemDescription('');
    setNewItemAmount(0);
    setAddItemDialogOpen(true);
  };

  // Handler para salvar novo item
  const handleSaveNewItem = async () => {
    if (!addItemEmployee || !payroll) return;
    
    setIsAddingItem(true);
    try {
      const updatedEmployee = await payrollService.addPayrollItem({
        payrollEmployeeId: addItemEmployee.payrollEmployeeId,
        description: newItemDescription,
        type: newItemType,
        category: 'Manual',
        amount: newItemAmount
      });
      
      // Atualizar estado local
      const updatedPayroll = payrollService.replaceEmployeeLocally(payroll, updatedEmployee);
      setPayroll(updatedPayroll);
      
      showSuccess(`${newItemType === 'Provento' ? 'Crédito' : 'Débito'} adicionado com sucesso!`);
      setAddItemDialogOpen(false);
      setAddItemEmployee(null);
      setNewItemDescription('');
      setNewItemAmount(0);
    } catch (err: any) {
      const { message } = parseBackendError(err);
      showError(message);
    } finally {
      setIsAddingItem(false);
    }
  };

  const formatCurrency = (valueInCents: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(valueInCents / 100);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('pt-BR');
  };

  const formatPeriod = (startDate: string, endDate: string) => {
    const start = new Date(startDate);
    const end = new Date(endDate);
    const startMonth = start.toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' });
    const endMonth = end.toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' });
    
    if (startMonth === endMonth) {
      return startMonth.charAt(0).toUpperCase() + startMonth.slice(1);
    }
    return `${startMonth} - ${endMonth}`;
  };

  if (isLoading) {
    return (
      <MainLayout>
        <div className="flex justify-center items-center min-h-[400px]">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>
      </MainLayout>
    );
  }

  if (!payroll) {
    return (
      <MainLayout>
        <div className="text-center py-12">
          <p className="text-gray-500">Folha de pagamento não encontrada</p>
        </div>
      </MainLayout>
    );
  }

  return (
    <>
    <MainLayout>
      {/* Header */}
      <div className="mb-6">
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center gap-3">
            <Button
              variant="outline"
              onClick={() => navigate('/payroll')}
              className="flex items-center gap-2"
            >
              <ArrowLeft className="h-4 w-4" />
              <span className="hidden sm:inline">Voltar</span>
            </Button>
            <div>
              <h1 className="text-2xl sm:text-3xl font-bold text-gray-900">
                {formatPeriod(payroll.periodStartDate, payroll.periodEndDate)}
              </h1>
              <p className="text-sm text-gray-600 mt-1">
                {formatDate(payroll.periodStartDate)} - {formatDate(payroll.periodEndDate)}
              </p>
            </div>
          </div>
          <div className="flex items-center gap-2 flex-wrap justify-end">
            {/* Botões apenas para folha aberta */}
            {!payroll.isClosed && (
              <>
                <Protected requires="payroll.canEdit">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setRecalculateDialogOpen(true)}
                    className="flex items-center gap-1"
                  >
                    <RefreshCw className="h-4 w-4" />
                    <span className="hidden sm:inline">Recalcular</span>
                  </Button>
                </Protected>
                <Protected requires="payroll.canEdit">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={handleOpenPayDialog}
                    className="flex items-center gap-1 text-green-700 border-green-300 hover:bg-green-50"
                  >
                    <CheckCircle className="h-4 w-4" />
                    <span className="hidden sm:inline">Pagar</span>
                  </Button>
                </Protected>
                <Protected requires="payroll.canDelete">
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setDeleteDialogOpen(true)}
                    className="flex items-center gap-1 text-red-700 border-red-300 hover:bg-red-50"
                  >
                    <Trash2 className="h-4 w-4" />
                    <span className="hidden sm:inline">Excluir</span>
                  </Button>
                </Protected>
              </>
            )}
            {/* Botão para reabrir folha fechada */}
            {payroll.isClosed && (
              <Protected requires="payroll.canEdit">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setReopenDialogOpen(true)}
                  className="flex items-center gap-1 text-orange-700 border-orange-300 hover:bg-orange-50"
                >
                  <Unlock className="h-4 w-4" />
                  <span className="hidden sm:inline">Reabrir</span>
                </Button>
              </Protected>
            )}
            {/* Botões sempre visíveis */}
            <Button
              variant="outline"
              size="sm"
              onClick={handlePrint}
              className="flex items-center gap-1"
            >
              <Printer className="h-4 w-4" />
              <span className="hidden sm:inline">Imprimir</span>
            </Button>
            {!payroll.isClosed && (
              <Protected requires="payroll.canEdit">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => {
                    // Se já tem 13º aplicado, preencher com valores atuais
                    if (payroll.thirteenthPercentage) {
                      setThirteenthPercentage(payroll.thirteenthPercentage);
                      setThirteenthTaxOption((payroll.thirteenthTaxOption || 'none') as 'none' | 'proportional' | 'full');
                    } else {
                      setThirteenthPercentage(100);
                      setThirteenthTaxOption('none');
                    }
                    setThirteenthDialogOpen(true);
                  }}
                  className={`flex items-center gap-1 ${payroll.thirteenthPercentage ? 'text-green-700 border-green-300 hover:bg-green-50' : 'text-purple-700 border-purple-300 hover:bg-purple-50'}`}
                >
                  <Gift className="h-4 w-4" />
                  <span className="hidden sm:inline">{payroll.thirteenthPercentage ? `13º (${payroll.thirteenthPercentage}%)` : 'Gerar 13º'}</span>
                </Button>
              </Protected>
            )}
            {/* Badge de status */}
            {payroll.isClosed ? (
              <span className="inline-flex items-center px-3 py-1.5 rounded-full text-sm font-medium bg-gray-100 text-gray-800">
                <Lock className="h-4 w-4 mr-1.5" />
                Fechada
              </span>
            ) : (
              <span className="inline-flex items-center px-3 py-1.5 rounded-full text-sm font-medium bg-green-100 text-green-800">
                <Unlock className="h-4 w-4 mr-1.5" />
                Aberta
              </span>
            )}
          </div>
        </div>

        {/* Summary Cards */}
        {(() => {
          // Calcular total de benefícios (Proventos que não são Salário Base)
          const totalBenefits = payroll.employees.reduce((total, emp) => {
            return total + emp.items
              .filter(item => item.type === 'Provento' && item.category !== 'Salario')
              .reduce((sum, item) => sum + item.amount, 0);
          }, 0);

          return (
            <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-7 gap-3 mb-6">
              <Card>
                <CardContent className="p-3">
                  <div className="flex items-center gap-2 mb-1">
                    <Users className="h-4 w-4 text-blue-600" />
                    <p className="text-xs text-gray-600">Empregados</p>
                  </div>
                  <p className="text-xl font-bold text-gray-900">{payroll.employeeCount}</p>
                </CardContent>
              </Card>

              <Card>
                <CardContent className="p-3">
                  <div className="flex items-center gap-2 mb-1">
                    <TrendingUp className="h-4 w-4 text-green-600" />
                    <p className="text-xs text-gray-600">Total Salários</p>
                  </div>
                  <p className="text-lg font-bold text-green-600">{formatCurrency(payroll.totalGrossPay - totalBenefits)}</p>
                </CardContent>
              </Card>

              <Card>
                <CardContent className="p-3">
                  <div className="flex items-center gap-2 mb-1">
                    <TrendingUp className="h-4 w-4 text-blue-600" />
                    <p className="text-xs text-gray-600">Total Benefícios</p>
                  </div>
                  <p className="text-lg font-bold text-green-600">{formatCurrency(totalBenefits)}</p>
                </CardContent>
              </Card>

              <Card>
                <CardContent className="p-3">
                  <div className="flex items-center gap-2 mb-1">
                    <TrendingDown className="h-4 w-4 text-red-600" />
                    <p className="text-xs text-gray-600">Total Deduções</p>
                  </div>
                  <p className="text-lg font-bold text-red-600">{formatCurrency(payroll.totalDeductions)}</p>
                </CardContent>
              </Card>

              <Card>
                <CardContent className="p-3">
                  <div className="flex items-center gap-2 mb-1">
                    <DollarSign className="h-4 w-4 text-green-600" />
                    <p className="text-xs text-gray-600">Total Líquido</p>
                  </div>
                  <p className="text-lg font-bold text-green-600">{formatCurrency(payroll.totalNetPay)}</p>
                </CardContent>
              </Card>

              <Card>
                <CardContent className="p-3">
                  <div className="flex items-center gap-2 mb-1">
                    <FileText className="h-4 w-4 text-orange-600" />
                    <p className="text-xs text-gray-600">INSS a Recolher</p>
                  </div>
                  <p className="text-lg font-bold text-orange-600">{formatCurrency(payroll.totalInss)}</p>
                </CardContent>
              </Card>

              <Card>
                <CardContent className="p-3">
                  <div className="flex items-center gap-2 mb-1">
                    <FileText className="h-4 w-4 text-purple-600" />
                    <p className="text-xs text-gray-600">FGTS a Recolher</p>
                  </div>
                  <p className="text-lg font-bold text-purple-600">{formatCurrency(payroll.totalFgts)}</p>
                </CardContent>
              </Card>
            </div>
          );
        })()}

        {/* 13º Salário Badge */}
        {payroll.thirteenthPercentage && (
          <div className="mb-6 p-3 bg-gradient-to-r from-purple-50 to-pink-50 border border-purple-200 rounded-lg flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-purple-100 rounded-full">
                <Gift className="h-5 w-5 text-purple-600" />
              </div>
              <div>
                <p className="text-sm font-semibold text-purple-900">13º Salário Aplicado</p>
                <p className="text-xs text-purple-700">
                  {payroll.thirteenthPercentage}% • Impostos: {payroll.thirteenthTaxOption === 'none' ? 'Não' : payroll.thirteenthTaxOption === 'proportional' ? 'Proporcionais' : 'Totais'}
                </p>
              </div>
            </div>
            <Protected requires="payroll.canEdit">
              <Button
                variant="outline"
                size="sm"
                onClick={() => {
                  setThirteenthPercentage(payroll.thirteenthPercentage!);
                  setThirteenthTaxOption((payroll.thirteenthTaxOption || 'none') as 'none' | 'proportional' | 'full');
                  setThirteenthDialogOpen(true);
                }}
                className="text-purple-700 border-purple-300 hover:bg-purple-100"
              >
                Alterar
              </Button>
            </Protected>
          </div>
        )}

        {/* Notes */}
        {payroll.notes && (
          <Card className="mb-6">
            <CardContent className="p-4">
              <div className="flex items-start gap-3">
                <FileText className="h-5 w-5 text-gray-400 flex-shrink-0 mt-0.5" />
                <div>
                  <p className="text-sm font-medium text-gray-700 mb-1">Observações</p>
                  <p className="text-sm text-gray-600">{payroll.notes}</p>
                </div>
              </div>
            </CardContent>
          </Card>
        )}
      </div>

      {/* Employee List */}
      <div className="space-y-4">
        <h2 className="text-xl font-bold text-gray-900 mb-4">Empregados ({payroll.employees.length})</h2>
        
        {payroll.employees.map((employee) => {
          const isExpanded = expandedEmployees.has(employee.employeeId);
          const credits = employee.items.filter(item => item.type === 'Provento');
          const debits = employee.items.filter(item => item.type === 'Desconto');
          // Calcular benefícios do empregado (proventos que não são salário base)
          const employeeBenefits = employee.items
            .filter(item => item.type === 'Provento' && item.category !== 'Salario')
            .reduce((sum, item) => sum + item.amount, 0);
          
          return (
            <Card key={employee.employeeId} className="overflow-hidden">
              <CardContent className="p-0">
                {/* Employee Header - Clickable */}
                <button
                  onClick={() => toggleEmployee(employee.employeeId)}
                  className="w-full p-4 hover:bg-gray-50 transition-colors text-left"
                >
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3 flex-1 min-w-0">
                      <div className="flex-shrink-0">
                        {isExpanded ? (
                          <ChevronDown className="h-5 w-5 text-gray-400" />
                        ) : (
                          <ChevronUp className="h-5 w-5 text-gray-400" />
                        )}
                      </div>
                      <div className="flex-1 min-w-0">
                        <h3 className="font-semibold text-gray-900 truncate">{employee.employeeName}</h3>
                        {employee.employeeDocument && (
                          <p className="text-sm text-gray-500">{employee.employeeDocument}</p>
                        )}
                        {employee.isOnVacation && (
                          <span className="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-blue-100 text-blue-800 mt-1">
                            <Calendar className="h-3 w-3 mr-1" />
                            Em férias ({employee.vacationDays} dias)
                          </span>
                        )}
                      </div>
                    </div>
                    
                    {/* Totals - Desktop */}
                    <div className="hidden lg:flex items-center gap-6 flex-shrink-0">
                      <div className="text-right">
                        <p className="text-xs text-gray-500">Salário</p>
                        <p className="text-sm font-medium text-gray-900">{formatCurrency(employee.totalGrossPay - employeeBenefits)}</p>
                      </div>
                      <div className="text-right">
                        <p className="text-xs text-gray-500">Benefícios</p>
                        <p className="text-sm font-medium text-blue-600">{formatCurrency(employeeBenefits)}</p>
                      </div>
                      <div className="text-right">
                        <p className="text-xs text-gray-500">Deduções</p>
                        <p className="text-sm font-medium text-red-600">{formatCurrency(employee.totalDeductions)}</p>
                      </div>
                      <div className="text-right">
                        <p className="text-xs text-gray-500">Líquido</p>
                        <p className="text-sm font-semibold text-green-600">{formatCurrency(employee.totalNetPay)}</p>
                      </div>
                    </div>
                    
                    {/* Totals - Tablet (sem benefícios) */}
                    <div className="hidden sm:flex lg:hidden items-center gap-6 flex-shrink-0">
                      <div className="text-right">
                        <p className="text-xs text-gray-500">Salário</p>
                        <p className="text-sm font-medium text-gray-900">{formatCurrency(employee.totalGrossPay - employeeBenefits)}</p>
                      </div>
                      <div className="text-right">
                        <p className="text-xs text-gray-500">Deduções</p>
                        <p className="text-sm font-medium text-red-600">{formatCurrency(employee.totalDeductions)}</p>
                      </div>
                      <div className="text-right">
                        <p className="text-xs text-gray-500">Líquido</p>
                        <p className="text-sm font-semibold text-green-600">{formatCurrency(employee.totalNetPay)}</p>
                      </div>
                    </div>
                    
                    {/* Mobile - Show only Net */}
                    <div className="sm:hidden flex-shrink-0 text-right">
                      <p className="text-xs text-gray-500">Líquido</p>
                      <p className="text-lg font-semibold text-green-600">{formatCurrency(employee.totalNetPay)}</p>
                    </div>
                  </div>
                </button>

                {/* Employee Details - Expandable */}
                {isExpanded && (
                  <div className="border-t border-gray-200 bg-gray-50 p-4">
                    {/* Ações do funcionário */}
                    {!payroll.isClosed && (
                      <div className="flex flex-wrap gap-2 mb-4">
                        {/* Botão de Horas/Dias para Horistas e Diaristas */}
                        {(employee.contractType === 'Horista' || employee.contractType === 'Diarista') && (
                          <Protected requires="payroll.canEdit">
                            <div className="flex-1 min-w-[200px] p-3 bg-blue-50 rounded-lg border border-blue-200">
                              <div className="flex items-center justify-between">
                                <div className="flex items-center gap-2">
                                  <Clock className="h-4 w-4 text-blue-600" />
                                  <div>
                                    <p className="text-sm font-medium text-blue-900">
                                      {employee.contractType === 'Horista' ? 'Horas Trabalhadas' : 'Dias Trabalhados'}
                                    </p>
                                    <p className="text-xs text-blue-600">
                                      {employee.workedUnits 
                                        ? `${employee.workedUnits} ${employee.contractType === 'Horista' ? 'horas' : 'dias'} × ${formatCurrency(employee.contractValue || 0)}`
                                        : 'Clique para informar'}
                                    </p>
                                  </div>
                                </div>
                                <Button
                                  variant="outline"
                                  size="sm"
                                  onClick={(e) => {
                                    e.stopPropagation();
                                    handleEditWorkedUnits(employee);
                                  }}
                                  className="text-blue-700 border-blue-300 hover:bg-blue-100"
                                >
                                  <Edit2 className="h-3 w-3 mr-1" />
                                  {employee.workedUnits ? 'Alterar' : 'Informar'}
                                </Button>
                              </div>
                            </div>
                          </Protected>
                        )}

                        {/* Botão de Recalcular Funcionário */}
                        <Protected requires="payroll.canEdit">
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={(e) => {
                              e.stopPropagation();
                              handleRecalculateEmployeeClick(employee);
                            }}
                            className="text-orange-700 border-orange-300 hover:bg-orange-100"
                          >
                            <RefreshCw className="h-3 w-3 mr-1" />
                            Recalcular
                          </Button>
                        </Protected>

                        {/* Botão de Férias */}
                        <Protected requires="payroll.canEdit">
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={(e) => {
                              e.stopPropagation();
                              handleAddVacation(employee);
                            }}
                            className="text-cyan-700 border-cyan-300 hover:bg-cyan-100"
                          >
                            <Palmtree className="h-3 w-3 mr-1" />
                            Férias
                          </Button>
                        </Protected>

                        {/* Botão de Adicionar Débito/Crédito */}
                        <Protected requires="payroll.canEdit">
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={(e) => {
                              e.stopPropagation();
                              handleAddItem(employee);
                            }}
                            className="text-indigo-700 border-indigo-300 hover:bg-indigo-100"
                          >
                            <Plus className="h-3 w-3 mr-1" />
                            Adicionar
                          </Button>
                        </Protected>

                        {/* Botão de Atalho para Contratos */}
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={(e) => {
                            e.stopPropagation();
                            handleOpenContract(employee);
                          }}
                          className="text-gray-700 border-gray-300 hover:bg-gray-100"
                        >
                          <ExternalLink className="h-3 w-3 mr-1" />
                          Contratos
                        </Button>
                      </div>
                    )}

                    <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                      {/* Credits */}
                      <div>
                        <h4 className="font-medium text-gray-900 mb-3 flex items-center gap-2">
                          <TrendingUp className="h-4 w-4 text-green-600" />
                          Créditos ({credits.length})
                        </h4>
                        <div className="space-y-2">
                          {credits.length === 0 ? (
                            <p className="text-sm text-gray-500 italic">Nenhum crédito</p>
                          ) : (
                            credits.map((item) => (
                              <div 
                                key={item.payrollItemId} 
                                className={`flex justify-between items-start bg-white p-3 rounded-lg ${
                                  !payroll.isClosed ? 'cursor-pointer hover:bg-green-50 transition-colors' : ''
                                }`}
                                onClick={() => !payroll.isClosed && handleEditItem(item)}
                              >
                                <div className="flex-1">
                                  <p className="text-sm font-medium text-gray-900">{item.description}</p>
                                  <p className="text-xs text-gray-500">{item.category}</p>
                                </div>
                                <div className="flex items-center gap-2">
                                  <p className="text-sm font-semibold text-green-600">
                                    {formatCurrency(item.amount)}
                                  </p>
                                  {!payroll.isClosed && (
                                    <Edit2 className="h-3 w-3 text-gray-400" />
                                  )}
                                </div>
                              </div>
                            ))
                          )}
                        </div>
                      </div>

                      {/* Debits */}
                      <div>
                        <h4 className="font-medium text-gray-900 mb-3 flex items-center gap-2">
                          <TrendingDown className="h-4 w-4 text-red-600" />
                          Débitos ({debits.length})
                        </h4>
                        <div className="space-y-2">
                          {debits.length === 0 ? (
                            <p className="text-sm text-gray-500 italic">Nenhum débito</p>
                          ) : (
                            debits.map((item) => (
                              <div 
                                key={item.payrollItemId} 
                                className={`flex justify-between items-start bg-white p-3 rounded-lg ${
                                  !payroll.isClosed ? 'cursor-pointer hover:bg-red-50 transition-colors' : ''
                                }`}
                                onClick={() => !payroll.isClosed && handleEditItem(item)}
                              >
                                <div className="flex-1">
                                  <p className="text-sm font-medium text-gray-900">{item.description}</p>
                                  <p className="text-xs text-gray-500">{item.category}</p>
                                </div>
                                <div className="flex items-center gap-2">
                                  <p className="text-sm font-semibold text-red-600">
                                    {formatCurrency(item.amount)}
                                  </p>
                                  {!payroll.isClosed && (
                                    <Edit2 className="h-3 w-3 text-gray-400" />
                                  )}
                                </div>
                              </div>
                            ))
                          )}
                        </div>
                      </div>
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Recalculate Confirmation Dialog */}
      <ConfirmDialog
        isOpen={recalculateDialogOpen}
        onClose={() => setRecalculateDialogOpen(false)}
        onConfirm={handleRecalculate}
        title="Recalcular Folha de Pagamento"
        description="Tem certeza que deseja recalcular esta folha de pagamento? Todos os empregados e seus itens serão removidos e recriados com base nos contratos ativos atuais. Esta ação não pode ser desfeita."
        confirmText="Recalcular"
        cancelText="Cancelar"
        variant="danger"
        isLoading={isRecalculating}
      />

      {/* Edit Payroll Item Dialog */}
      <EditPayrollItemDialog
        isOpen={editItemDialogOpen}
        onClose={() => {
          setEditItemDialogOpen(false);
          setSelectedItem(null);
        }}
        onSave={handleSaveItem}
        item={selectedItem}
        isLoading={isUpdatingItem}
      />

      {/* Worked Units Dialog */}
      <WorkedUnitsDialog
        isOpen={workedUnitsDialogOpen}
        onClose={() => {
          setWorkedUnitsDialogOpen(false);
          setSelectedEmployee(null);
        }}
        onSave={handleSaveWorkedUnits}
        employee={selectedEmployee}
        isLoading={isUpdatingWorkedUnits}
      />

      {/* Recalculate Employee Confirmation Dialog */}
      <ConfirmDialog
        isOpen={recalculateEmployeeDialogOpen}
        onClose={() => {
          setRecalculateEmployeeDialogOpen(false);
          setEmployeeToRecalculate(null);
        }}
        onConfirm={handleRecalculateEmployee}
        title="Recalcular Funcionário"
        description={`Tem certeza que deseja recalcular ${employeeToRecalculate?.employeeName || 'este funcionário'}? Todos os itens (proventos e descontos) serão removidos e recriados com base no contrato atual. Esta ação não pode ser desfeita.`}
        confirmText="Recalcular"
        cancelText="Cancelar"
        variant="danger"
        isLoading={isRecalculatingEmployee}
      />

      {/* Delete Payroll Confirmation Dialog */}
      <ConfirmDialog
        isOpen={deleteDialogOpen}
        onClose={() => setDeleteDialogOpen(false)}
        onConfirm={handleDeletePayroll}
        title="Excluir Folha de Pagamento"
        description="Tem certeza que deseja excluir esta folha de pagamento? Todos os dados de empregados e itens serão permanentemente removidos. Esta ação não pode ser desfeita."
        confirmText="Excluir"
        cancelText="Cancelar"
        variant="danger"
        isLoading={isDeleting}
      />

      {/* Pay Payroll Dialog */}
      {payDialogOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg shadow-xl p-6 w-full max-w-md mx-4">
            <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center gap-2">
              <CheckCircle className="h-5 w-5 text-green-600" />
              Pagar Folha de Pagamento
            </h2>
            
            <p className="text-sm text-gray-600 mb-4">
              Após o pagamento, a folha será fechada e não poderá mais ser editada.
            </p>
            
            <div className="space-y-4">
              <div>
                <Label htmlFor="accountId">Conta Corrente</Label>
                <select
                  id="accountId"
                  value={selectedAccountId || ''}
                  onChange={(e) => setSelectedAccountId(Number(e.target.value))}
                  className="mt-1 w-full rounded-md border border-gray-300 bg-white px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                >
                  <option value="">Selecione uma conta...</option>
                  {accounts.map((account) => (
                    <option key={account.accountId} value={account.accountId}>
                      {account.name}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <Label htmlFor="paymentDate">Data do Pagamento</Label>
                <Input
                  id="paymentDate"
                  type="date"
                  value={paymentDate}
                  onChange={(e) => setPaymentDate(e.target.value)}
                  className="mt-1"
                />
              </div>

              <div>
                <Label htmlFor="inssAmount">Valor do Boleto INSS</Label>
                <CurrencyInput
                  id="inssAmount"
                  value={inssAmount}
                  onChange={(value) => setInssAmount(parseInt(value) || 0)}
                  className="mt-1"
                />
                <p className="text-xs text-gray-500 mt-1">Soma dos descontos de INSS dos funcionários</p>
              </div>

              <div>
                <Label htmlFor="fgtsAmount">Valor do Boleto FGTS</Label>
                <CurrencyInput
                  id="fgtsAmount"
                  value={fgtsAmount}
                  onChange={(value) => setFgtsAmount(parseInt(value) || 0)}
                  className="mt-1"
                />
                <p className="text-xs text-gray-500 mt-1">8% sobre a remuneração bruta</p>
              </div>
            </div>

            <div className="flex justify-end gap-3 mt-6">
              <Button
                variant="outline"
                onClick={() => setPayDialogOpen(false)}
              >
                Cancelar
              </Button>
              <Button
                onClick={handlePayPayroll}
                disabled={isPaying || !paymentDate || !selectedAccountId}
                className="bg-green-600 hover:bg-green-700"
              >
                {isPaying ? 'Processando...' : 'Confirmar Pagamento'}
              </Button>
            </div>
          </div>
        </div>
      )}

      {/* Generate 13th Salary Dialog */}
      {thirteenthDialogOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg shadow-xl p-6 w-full max-w-md mx-4">
            <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center gap-2">
              <Gift className="h-5 w-5 text-purple-600" />
              {payroll.thirteenthPercentage ? 'Alterar 13º Salário' : 'Gerar 13º Salário'}
            </h2>

            {payroll.thirteenthPercentage && (
              <div className="mb-4 p-3 bg-green-50 border border-green-200 rounded-lg">
                <p className="text-sm text-green-800">
                  13º já aplicado: <strong>{payroll.thirteenthPercentage}%</strong> com impostos: <strong>{payroll.thirteenthTaxOption === 'none' ? 'Não' : payroll.thirteenthTaxOption === 'proportional' ? 'Proporcionais' : 'Totais'}</strong>
                </p>
              </div>
            )}
            
            <div className="space-y-4">
              <div>
                <Label htmlFor="thirteenthPercentage">Porcentagem do 13º</Label>
                <div className="flex items-center gap-2 mt-1">
                  <Input
                    id="thirteenthPercentage"
                    type="number"
                    min="0"
                    max="100"
                    value={thirteenthPercentage}
                    onChange={(e) => setThirteenthPercentage(Number(e.target.value))}
                    className="flex-1"
                  />
                  <span className="text-gray-500">%</span>
                </div>
              </div>

              <div>
                <Label htmlFor="thirteenthTaxOption">Opção de Impostos</Label>
                <select
                  id="thirteenthTaxOption"
                  value={thirteenthTaxOption}
                  onChange={(e) => setThirteenthTaxOption(e.target.value as 'none' | 'proportional' | 'full')}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-primary-500 focus:ring-primary-500 sm:text-sm p-2 border"
                >
                  <option value="none">Não Lançar Impostos</option>
                  <option value="proportional">Impostos Proporcionais</option>
                  <option value="full">Impostos Totais</option>
                </select>
              </div>
            </div>

            <div className="flex justify-between mt-6">
              {payroll.thirteenthPercentage ? (
                <Button
                  variant="outline"
                  onClick={handleRemove13th}
                  disabled={isGenerating13th}
                  className="text-red-600 border-red-300 hover:bg-red-50"
                >
                  {isGenerating13th ? 'Removendo...' : 'Remover 13º'}
                </Button>
              ) : (
                <div />
              )}
              <div className="flex gap-3">
                <Button
                  variant="outline"
                  onClick={() => {
                    setThirteenthDialogOpen(false);
                    setThirteenthPercentage(100);
                    setThirteenthTaxOption('none');
                  }}
                >
                  Cancelar
                </Button>
                <Button
                  onClick={handleGenerate13th}
                  disabled={isGenerating13th}
                  className="bg-purple-600 hover:bg-purple-700"
                >
                  {isGenerating13th ? 'Salvando...' : payroll.thirteenthPercentage ? 'Atualizar 13º' : 'Gerar 13º'}
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Vacation Dialog */}
      {vacationDialogOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg shadow-xl p-6 w-full max-w-md mx-4 max-h-[90vh] overflow-y-auto">
            <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center gap-2">
              <Palmtree className="h-5 w-5 text-cyan-600" />
              {vacationEmployee?.isOnVacation ? 'Alterar' : 'Adicionar'} Férias - {vacationEmployee?.employeeName}
            </h2>

            {vacationEmployee?.isOnVacation && (
              <div className="mb-4 p-3 bg-cyan-50 border border-cyan-200 rounded-lg">
                <p className="text-sm text-cyan-700">
                  Este funcionário já possui férias aplicadas. Você pode alterar ou remover.
                </p>
              </div>
            )}
            
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="vacationStartDate">Data de Início</Label>
                  <Input
                    id="vacationStartDate"
                    type="date"
                    value={vacationStartDate}
                    onChange={(e) => setVacationStartDate(e.target.value)}
                    className="mt-1"
                  />
                </div>

                <div>
                  <Label htmlFor="vacationDays">Quantidade de Dias</Label>
                  <Input
                    id="vacationDays"
                    type="number"
                    min="1"
                    max="30"
                    value={vacationDays}
                    onChange={(e) => setVacationDays(Number(e.target.value))}
                    className="mt-1"
                  />
                </div>
              </div>

              <p className="text-xs text-gray-500">
                O 1/3 constitucional será calculado proporcionalmente aos dias.
              </p>

              <div className="space-y-3">
                <div className="flex items-center gap-2">
                  <input
                    type="checkbox"
                    id="vacationIncludeTaxes"
                    checked={vacationIncludeTaxes}
                    onChange={(e) => setVacationIncludeTaxes(e.target.checked)}
                    className="h-4 w-4 text-cyan-600 rounded border-gray-300"
                  />
                  <Label htmlFor="vacationIncludeTaxes" className="cursor-pointer">
                    Incluir INSS sobre férias
                  </Label>
                </div>

                <div className="flex items-center gap-2">
                  <input
                    type="checkbox"
                    id="vacationAdvanceNextMonth"
                    checked={vacationAdvanceNextMonth}
                    onChange={(e) => setVacationAdvanceNextMonth(e.target.checked)}
                    className="h-4 w-4 text-cyan-600 rounded border-gray-300"
                  />
                  <Label htmlFor="vacationAdvanceNextMonth" className="cursor-pointer">
                    Adiantar salário do próximo mês
                  </Label>
                </div>
                <p className="text-xs text-gray-500">
                  Inclui salário, benefícios, descontos, impostos e parcelas de empréstimos do próximo mês.
                </p>
              </div>

              <div>
                <Label htmlFor="vacationNotes">Observações (opcional)</Label>
                <Input
                  id="vacationNotes"
                  type="text"
                  placeholder="Ex: Período aquisitivo 2024/2025"
                  value={vacationNotes}
                  onChange={(e) => setVacationNotes(e.target.value)}
                  className="mt-1"
                />
              </div>
            </div>

            <div className="flex justify-between gap-3 mt-6">
              <div>
                {vacationEmployee?.isOnVacation && (
                  <Button
                    variant="outline"
                    onClick={handleRemoveVacation}
                    disabled={isAddingVacation}
                    className="text-red-600 border-red-300 hover:bg-red-50"
                  >
                    {isAddingVacation ? 'Removendo...' : 'Remover Férias'}
                  </Button>
                )}
              </div>
              <div className="flex gap-3">
                <Button
                  variant="outline"
                  onClick={() => {
                    setVacationDialogOpen(false);
                    setVacationEmployee(null);
                  }}
                >
                  Cancelar
                </Button>
                <Button
                  onClick={handleSaveVacation}
                  disabled={isAddingVacation || !vacationStartDate}
                  className="bg-cyan-600 hover:bg-cyan-700"
                >
                  {isAddingVacation ? 'Salvando...' : vacationEmployee?.isOnVacation ? 'Alterar Férias' : 'Aplicar Férias'}
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Add Item Dialog */}
      {addItemDialogOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg shadow-xl p-6 w-full max-w-md mx-4">
            <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center gap-2">
              <Plus className="h-5 w-5 text-indigo-600" />
              Adicionar Item - {addItemEmployee?.employeeName}
            </h2>
            
            <div className="space-y-4">
              <div>
                <Label htmlFor="newItemType">Tipo</Label>
                <select
                  id="newItemType"
                  value={newItemType}
                  onChange={(e) => setNewItemType(e.target.value as 'Provento' | 'Desconto')}
                  className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-primary-500 focus:ring-primary-500 sm:text-sm p-2 border"
                >
                  <option value="Provento">Crédito (Provento)</option>
                  <option value="Desconto">Débito (Desconto)</option>
                </select>
              </div>

              <div>
                <Label htmlFor="newItemDescription">Descrição</Label>
                <Input
                  id="newItemDescription"
                  type="text"
                  placeholder="Ex: Bônus de produtividade"
                  value={newItemDescription}
                  onChange={(e) => setNewItemDescription(e.target.value)}
                  className="mt-1"
                />
              </div>

              <div>
                <Label htmlFor="newItemAmount">Valor</Label>
                <CurrencyInput
                  id="newItemAmount"
                  value={newItemAmount}
                  onChange={(value) => setNewItemAmount(parseInt(value) || 0)}
                  className="mt-1"
                />
              </div>
            </div>

            <div className="flex justify-end gap-3 mt-6">
              <Button
                variant="outline"
                onClick={() => {
                  setAddItemDialogOpen(false);
                  setAddItemEmployee(null);
                }}
              >
                Cancelar
              </Button>
              <Button
                onClick={handleSaveNewItem}
                disabled={isAddingItem || !newItemDescription || newItemAmount <= 0}
                className="bg-indigo-600 hover:bg-indigo-700"
              >
                {isAddingItem ? 'Salvando...' : 'Adicionar Item'}
              </Button>
            </div>
          </div>
        </div>
      )}

    {/* Reopen Payroll Confirmation Dialog */}
      <ConfirmDialog
        isOpen={reopenDialogOpen}
        onClose={() => setReopenDialogOpen(false)}
        onConfirm={handleReopenPayroll}
        title="Reabrir Folha de Pagamento"
        description="Tem certeza que deseja reabrir esta folha? Todas as transações financeiras geradas serão revertidas, incluindo lançamentos por funcionário, INSS/FGTS e abatimentos de empréstimos."
        confirmText="Reabrir"
        cancelText="Cancelar"
        variant="danger"
        isLoading={isReopening}
      />

    </MainLayout>

    {/* Componente de Impressão - fora do MainLayout para evitar página extra */}
    {payroll && (
      <PayrollPrintReport
        ref={printRef}
        payroll={payroll}
        companyName={selectedCompany?.name || 'Empresa'}
      />
    )}
    </>
  );
}
