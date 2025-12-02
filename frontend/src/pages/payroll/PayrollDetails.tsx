import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Card, CardContent } from '../../components/ui/Card';
import { ConfirmDialog } from '../../components/ui/ConfirmDialog';
import { Protected } from '../../components/permissions/Protected';
import { useToast } from '../../contexts/ToastContext';
import payrollService, { type PayrollDetailed } from '../../services/payrollService';
import { parseBackendError } from '../../utils/errorHandler';
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
  RefreshCw
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

  useEffect(() => {
    loadPayrollDetails();
  }, [id]);

  const handleRecalculate = async () => {
    if (!id) return;
    
    setIsRecalculating(true);
    try {
      const data = await payrollService.recalculatePayroll(parseInt(id));
      setPayroll(data);
      setExpandedEmployees(new Set(data.employees.map(e => e.employeeId)));
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
      // Expandir todos os empregados por padrão
      setExpandedEmployees(new Set(data.employees.map(e => e.employeeId)));
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
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
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
                  <p className="text-sm text-gray-600">Total Bruto</p>
                  <p className="text-xl font-bold text-gray-900">{formatCurrency(payroll.totalGrossPay)}</p>
                </div>
                <TrendingUp className="h-8 w-8 text-green-600" />
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
                    
                    {/* Totals */}
                    <div className="hidden sm:flex items-center gap-6 flex-shrink-0">
                      <div className="text-right">
                        <p className="text-xs text-gray-500">Bruto</p>
                        <p className="text-sm font-medium text-gray-900">{formatCurrency(employee.totalGrossPay)}</p>
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
                              <div key={item.payrollItemId} className="flex justify-between items-start bg-white p-3 rounded-lg">
                                <div className="flex-1">
                                  <p className="text-sm font-medium text-gray-900">{item.description}</p>
                                  <p className="text-xs text-gray-500">{item.category}</p>
                                </div>
                                <p className="text-sm font-semibold text-green-600 ml-2">
                                  {formatCurrency(item.amount)}
                                </p>
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
                              <div key={item.payrollItemId} className="flex justify-between items-start bg-white p-3 rounded-lg">
                                <div className="flex-1">
                                  <p className="text-sm font-medium text-gray-900">{item.description}</p>
                                  <p className="text-xs text-gray-500">{item.category}</p>
                                </div>
                                <p className="text-sm font-semibold text-red-600 ml-2">
                                  {formatCurrency(item.amount)}
                                </p>
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
    </MainLayout>
  );
}
