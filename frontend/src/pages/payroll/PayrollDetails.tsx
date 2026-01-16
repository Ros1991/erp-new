import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Card, CardContent } from '../../components/ui/Card';
import { ConfirmDialog } from '../../components/ui/ConfirmDialog';
import { Protected } from '../../components/permissions/Protected';
import { useToast } from '../../contexts/ToastContext';
import payrollService, { type PayrollDetailed, type PayrollItem, type PayrollEmployeeDetailed } from '../../services/payrollService';
import { parseBackendError } from '../../utils/errorHandler';
import { EditPayrollItemDialog } from './EditPayrollItemDialog';
import { WorkedUnitsDialog } from './WorkedUnitsDialog';
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
  Edit2
} from 'lucide-react';

export function PayrollDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { showError, showSuccess } = useToast();
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

  useEffect(() => {
    loadPayrollDetails();
  }, [id]);

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
          <div className="flex items-center gap-3">
            {!payroll.isClosed && (
              <Protected requires="payroll.canEdit">
                <Button
                  variant="outline"
                  onClick={() => setRecalculateDialogOpen(true)}
                  className="flex items-center gap-2"
                >
                  <RefreshCw className="h-4 w-4" />
                  <span className="hidden sm:inline">Recalcular</span>
                </Button>
              </Protected>
            )}
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
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4 mb-6">
              <Card>
                <CardContent className="p-4">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm text-gray-600">Empregados</p>
                      <p className="text-2xl font-bold text-gray-900">{payroll.employeeCount}</p>
                    </div>
                    <Users className="h-8 w-8 text-blue-600" />
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardContent className="p-4">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm text-gray-600">Total Salários</p>
                      <p className="text-xl font-bold text-gray-900">{formatCurrency(payroll.totalGrossPay - totalBenefits)}</p>
                    </div>
                    <TrendingUp className="h-8 w-8 text-green-600" />
                  </div>
                </CardContent>
              </Card>

              {/* Card de Benefícios */}
              <Card>
                <CardContent className="p-4">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm text-gray-600">Total Benefícios</p>
                      <p className="text-xl font-bold text-blue-600">{formatCurrency(totalBenefits)}</p>
                    </div>
                    <TrendingUp className="h-8 w-8 text-blue-600" />
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardContent className="p-4">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm text-gray-600">Total Deduções</p>
                      <p className="text-xl font-bold text-red-600">{formatCurrency(payroll.totalDeductions)}</p>
                    </div>
                    <TrendingDown className="h-8 w-8 text-red-600" />
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardContent className="p-4">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm text-gray-600">Total Líquido</p>
                      <p className="text-xl font-bold text-green-600">{formatCurrency(payroll.totalNetPay)}</p>
                    </div>
                    <DollarSign className="h-8 w-8 text-green-600" />
                  </div>
                </CardContent>
              </Card>
            </div>
          );
        })()}

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
    </MainLayout>
  );
}
