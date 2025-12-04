import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Plus, CheckCircle2, XCircle, FileText, ChevronDown, ChevronUp } from 'lucide-react';
import { MainLayout } from '../../components/layout';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { useToast } from '../../contexts/ToastContext';
import contractService, { type Contract } from '../../services/contractService';
import employeeService from '../../services/employeeService';
import { Protected } from '../../components/permissions/Protected';
import { getApplicationTypeLabel, migrateApplicationTypeValue } from '../../constants/applicationType';

export function EmployeeContracts() {
  const { employeeId } = useParams<{ employeeId: string }>();
  const navigate = useNavigate();
  const { handleBackendError } = useToast();
  const [isLoading, setIsLoading] = useState(true);
  const [activeContract, setActiveContract] = useState<Contract | null>(null);
  const [contractHistory, setContractHistory] = useState<Contract[]>([]);
  const [showHistory, setShowHistory] = useState(false);
  const [employeeName, setEmployeeName] = useState('');

  useEffect(() => {
    if (employeeId) {
      loadEmployee();
      loadActiveContract();
    }
  }, [employeeId]);

  const loadEmployee = async () => {
    try {
      const employee = await employeeService.getEmployeeById(Number(employeeId));
      setEmployeeName(employee.fullName || employee.nickname);
    } catch (err: any) {
      handleBackendError(err);
    }
  };

  const loadActiveContract = async () => {
    setIsLoading(true);
    try {
      const active = await contractService.getActiveByEmployeeId(Number(employeeId));
      // Só define o contrato se tiver dados válidos
      if (active && active.contractId && active.type) {
        setActiveContract(active);
      } else {
        setActiveContract(null);
      }
    } catch (err: any) {
      // Se retornar 404 ou erro, não tem contrato ativo
      setActiveContract(null);
    } finally {
      setIsLoading(false);
    }
  };

  const loadAllContracts = async () => {
    try {
      const contracts = await contractService.getAllByEmployeeId(Number(employeeId));
      setContractHistory(contracts.filter(c => !c.isActive));
      setShowHistory(true);
    } catch (err: any) {
      handleBackendError(err);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value / 100); // Converter de centavos para reais
  };

  const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString('pt-BR');
  };

  const getMonthName = (month: number) => {
    const months = ['', 'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'];
    return months[month] || '';
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
      </div>
    );
  }

  return (
    <MainLayout>
      <div className="max-w-7xl">
      {/* Header */}
      <Button
        variant="outline"
        onClick={() => navigate('/employees')}
        className="mb-4"
      >
        <ArrowLeft className="h-4 w-4 mr-2" />
        Voltar
      </Button>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-3xl font-bold">Contratos</h1>
          <p className="text-muted-foreground">{employeeName}</p>
        </div>
        <Protected requires="employee.canEdit">
          <Button onClick={() => navigate(`/employees/${employeeId}/contracts/new`)}>
            <Plus className="h-4 w-4 mr-2" />
            Novo Contrato
          </Button>
        </Protected>
      </div>

      {/* Empty State - Nenhum Contrato */}
      {!activeContract && !showHistory ? (
        <Card className="border-dashed border-2">
          <CardContent className="py-16 text-center">
            <div className="max-w-md mx-auto">
              <div className="mb-6 flex justify-center">
                <div className="rounded-full bg-primary/10 p-6">
                  <FileText className="h-16 w-16 text-primary" />
                </div>
              </div>
              <h2 className="text-2xl font-bold mb-3">Nenhum contrato cadastrado</h2>
              <p className="text-muted-foreground mb-6">
                Este funcionário ainda não possui nenhum contrato. Crie o primeiro contrato para registrar
                informações sobre tipo, valor, benefícios e centros de custo.
              </p>
              <Protected requires="employee.canEdit">
                <Button size="lg" onClick={() => navigate(`/employees/${employeeId}/contracts/new`)}>
                  <Plus className="h-5 w-5 mr-2" />
                  Criar Primeiro Contrato
                </Button>
              </Protected>
            </div>
          </CardContent>
        </Card>
      ) : activeContract ? (
        <Card className="mb-6 border-primary shadow-lg">
          <CardHeader className="bg-primary/5">
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="flex items-center gap-2">
                  <CheckCircle2 className="h-5 w-5 text-green-600" />
                  Contrato Ativo
                </CardTitle>
                <CardDescription>
                  {formatDate(activeContract.startDate)} {activeContract.endDate && `- ${formatDate(activeContract.endDate)}`}
                </CardDescription>
              </div>
              <Protected requires="employee.canEdit">
                <Button
                  variant="outline"
                  onClick={() => navigate(`/employees/${employeeId}/contracts/${activeContract.contractId}`)}
                >
                  Editar
                </Button>
              </Protected>
            </div>
          </CardHeader>
          <CardContent className="pt-6">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
              <div>
                <p className="text-sm text-muted-foreground mb-1">Tipo</p>
                <p className="font-semibold">{activeContract.type}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground mb-1">Valor</p>
                <p className="font-semibold text-lg">{formatCurrency(activeContract.value)}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground mb-1">Horas Semanais</p>
                <p className="font-semibold">{activeContract.weeklyHours || 'N/A'}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground mb-1">Características</p>
                <div className="flex flex-wrap gap-2">
                  {activeContract.isPayroll && (
                    <span className="px-2 py-1 bg-blue-100 text-blue-800 text-xs rounded">
                      Folha
                    </span>
                  )}
                  {activeContract.hasInss && (
                    <span className="px-2 py-1 bg-purple-100 text-purple-800 text-xs rounded">
                      INSS
                    </span>
                  )}
                  {activeContract.hasIrrf && (
                    <span className="px-2 py-1 bg-orange-100 text-orange-800 text-xs rounded">
                      IRRF
                    </span>
                  )}
                  {activeContract.hasFgts && (
                    <span className="px-2 py-1 bg-green-100 text-green-800 text-xs rounded">
                      FGTS
                    </span>
                  )}
                  {activeContract.hasThirteenthSalary && (
                    <span className="px-2 py-1 bg-teal-100 text-teal-800 text-xs rounded">
                      13º Salário
                    </span>
                  )}
                  {activeContract.hasVacationBonus && (
                    <span className="px-2 py-1 bg-cyan-100 text-cyan-800 text-xs rounded">
                      Adicional Férias
                    </span>
                  )}
                </div>
              </div>
            </div>

            {/* Benefícios e Descontos */}
            {activeContract.benefitsDiscounts && activeContract.benefitsDiscounts.length > 0 && (
              <div className="mb-6">
                <h3 className="font-semibold mb-3">Benefícios e Descontos</h3>
                <div className="space-y-2">
                  {activeContract.benefitsDiscounts.map((item, idx) => (
                    <div key={idx} className="flex items-center justify-between p-3 bg-muted rounded">
                      <div className="flex-1">
                        <p className="font-medium">{item.description}</p>
                        <p className="text-sm text-muted-foreground">
                          {item.type} • {getApplicationTypeLabel(migrateApplicationTypeValue(item.application))}
                          {item.month && ` • ${getMonthName(item.month)}`}
                        </p>
                        <div className="flex flex-wrap gap-1 mt-1">
                          {item.hasTaxes && (
                            <span className="px-1.5 py-0.5 bg-amber-100 text-amber-700 text-xs rounded">
                              Incide impostos
                            </span>
                          )}
                          {item.isProportional !== false && (
                            <span className="px-1.5 py-0.5 bg-blue-100 text-blue-700 text-xs rounded">
                              Proporcional
                            </span>
                          )}
                          {item.isProportional === false && (
                            <span className="px-1.5 py-0.5 bg-gray-100 text-gray-600 text-xs rounded">
                              Valor fixo
                            </span>
                          )}
                        </div>
                      </div>
                      <p className={`font-semibold ${item.type === 'Benefício' ? 'text-green-600' : 'text-red-600'}`}>
                        {item.type === 'Desconto' && '-'}
                        {formatCurrency(item.amount)}
                      </p>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* Centros de Custo */}
            {activeContract.costCenters && activeContract.costCenters.length > 0 && (
              <div>
                <h3 className="font-semibold mb-3">Distribuição por Centro de Custo</h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                  {activeContract.costCenters.map((cc, idx) => (
                    <div key={idx} className="flex items-center justify-between p-3 bg-muted rounded">
                      <span className="font-medium">{cc.costCenterName}</span>
                      <span className="text-primary font-semibold">{cc.percentage.toFixed(2)}%</span>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </CardContent>
        </Card>
      ) : (
        <Card className="mb-6">
          <CardContent className="py-12 text-center">
            <XCircle className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
            <p className="text-muted-foreground mb-4">Nenhum contrato ativo no momento</p>
            <p className="text-sm text-muted-foreground mb-4">Todos os contratos estão encerrados. Veja o histórico abaixo.</p>
          </CardContent>
        </Card>
      )}

      {/* Botão Toggle Histórico */}
      {activeContract && (
        <div className="text-center">
          <Button 
            variant="outline" 
            onClick={() => {
              if (!showHistory) {
                loadAllContracts();
              } else {
                setShowHistory(false);
              }
            }}
          >
            {showHistory ? (
              <>
                <ChevronUp className="h-4 w-4 mr-2" />
                Ocultar Contratos Anteriores
              </>
            ) : (
              <>
                <ChevronDown className="h-4 w-4 mr-2" />
                Ver Contratos Anteriores
              </>
            )}
          </Button>
        </div>
      )}

      {/* Histórico de Contratos */}
      {showHistory && (
        <div>
          <h2 className="text-2xl font-bold mb-4">Contratos Anteriores</h2>
          {contractHistory.length === 0 ? (
            <Card>
              <CardContent className="py-8 text-center text-muted-foreground">
                Nenhum contrato anterior encontrado
              </CardContent>
            </Card>
          ) : (
            <div className="space-y-4">
              {contractHistory.map((contract) => (
                <Card key={contract.contractId}>
                  <CardContent className="p-4">
                    <div className="flex items-center justify-between">
                      <div className="flex-1">
                        <div className="flex items-center gap-3 mb-2">
                          <h3 className="font-semibold">{contract.type}</h3>
                          <span className="text-sm text-muted-foreground">
                            {formatDate(contract.startDate)} - {contract.endDate ? formatDate(contract.endDate) : 'Presente'}
                          </span>
                        </div>
                        <p className="text-lg font-semibold text-primary">{formatCurrency(contract.value)}</p>
                      </div>
                      <Protected requires="employee.canView">
                        <Button
                          variant="ghost"
                          onClick={() => navigate(`/employees/${employeeId}/contracts/${contract.contractId}`)}
                        >
                          Ver Detalhes
                        </Button>
                      </Protected>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
    </MainLayout>
  );
}
