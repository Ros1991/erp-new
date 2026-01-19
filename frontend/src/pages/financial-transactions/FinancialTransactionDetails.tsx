import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { MainLayout } from '../../components/layout';
import { Button } from '../../components/ui/Button';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { useToast } from '../../contexts/ToastContext';
import financialTransactionService, { type FinancialTransaction } from '../../services/financialTransactionService';
import { 
  ArrowLeft,
  ArrowRightLeft,
  Calendar,
  Wallet,
  Building2,
  FileText,
  ExternalLink,
  PieChart,
  Info
} from 'lucide-react';

export function FinancialTransactionDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { handleBackendError } = useToast();
  const [transaction, setTransaction] = useState<FinancialTransaction | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadTransaction();
  }, [id]);

  const loadTransaction = async () => {
    if (!id) return;
    
    setIsLoading(true);
    try {
      const data = await financialTransactionService.getFinancialTransactionById(parseInt(id));
      setTransaction(data);
    } catch (err: any) {
      handleBackendError(err);
      navigate('/financial-transactions');
    } finally {
      setIsLoading(false);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value / 100);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('pt-BR');
  };

  const formatDateTime = (dateString: string) => {
    return new Date(dateString).toLocaleString('pt-BR');
  };

  const getOriginInfo = () => {
    if (!transaction) return null;

    // Empréstimo/Adiantamento (verificar primeiro pois tem ID específico)
    if (transaction.loanAdvanceId) {
      return {
        label: 'Empréstimo/Adiantamento',
        description: 'Esta transação foi gerada a partir de um empréstimo ou adiantamento',
        link: `/loan-advances/${transaction.loanAdvanceId}/edit`,
        icon: FileText
      };
    }

    if (transaction.accountPayableReceivableId) {
      return {
        label: 'Conta a Pagar/Receber',
        description: 'Esta transação foi gerada a partir de uma conta a pagar ou receber',
        link: `/account-payable-receivable/${transaction.accountPayableReceivableId}/edit`,
        icon: FileText
      };
    }

    if (transaction.purchaseOrderId) {
      return {
        label: 'Ordem de Compra',
        description: 'Esta transação foi gerada a partir de uma ordem de compra',
        link: `/purchase-orders/${transaction.purchaseOrderId}`,
        icon: FileText
      };
    }

    // Folha de pagamento
    if (transaction.payrollId) {
      return {
        label: 'Folha de Pagamento',
        description: 'Esta transação foi gerada pelo fechamento de uma folha de pagamento',
        link: `/payroll/${transaction.payrollId}`,
        icon: FileText
      };
    }

    // Detectar origem pela descrição (fallback para transações antigas sem payrollId)
    if (transaction.description.includes('Folha') || 
        transaction.description.includes('INSS') || 
        transaction.description.includes('FGTS')) {
      return {
        label: 'Folha de Pagamento',
        description: 'Esta transação foi gerada pelo fechamento de uma folha de pagamento. Acesse a lista de folhas para encontrar o período correspondente.',
        link: '/payroll',
        icon: FileText
      };
    }

    return null;
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

  if (!transaction) {
    return (
      <MainLayout>
        <div className="text-center py-12">
          <p className="text-gray-500">Transação não encontrada</p>
        </div>
      </MainLayout>
    );
  }

  const originInfo = getOriginInfo();
  const isOutgoing = transaction.type === 'Débito' || transaction.type === 'Saída';

  return (
    <MainLayout>
      {/* Header */}
      <div className="mb-6">
        <div className="flex items-center gap-3 mb-4">
          <Button
            variant="outline"
            onClick={() => navigate('/financial-transactions')}
            className="flex items-center gap-2"
          >
            <ArrowLeft className="h-4 w-4" />
            <span className="hidden sm:inline">Voltar</span>
          </Button>
          <div>
            <h1 className="text-2xl sm:text-3xl font-bold text-gray-900">
              Detalhes da Transação
            </h1>
            <p className="text-sm text-gray-600 mt-1">
              Visualização somente leitura
            </p>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="space-y-6">
        {/* Transaction Summary Card */}
        <Card>
          <CardContent className="p-6">
            <div className="flex items-start gap-4">
              <div className={`h-14 w-14 rounded-full flex items-center justify-center ${
                isOutgoing ? 'bg-red-100' : 'bg-green-100'
              }`}>
                <ArrowRightLeft className={`h-7 w-7 ${
                  isOutgoing ? 'text-red-600' : 'text-green-600'
                }`} />
              </div>
              <div className="flex-1">
                <h2 className="text-xl font-semibold text-gray-900">{transaction.description}</h2>
                <div className="flex flex-wrap items-center gap-3 mt-2">
                  <span className={`inline-flex px-3 py-1 text-sm font-medium rounded-full ${
                    isOutgoing ? 'bg-red-100 text-red-800' : 'bg-green-100 text-green-800'
                  }`}>
                    {transaction.type}
                  </span>
                  <span className={`text-2xl font-bold ${
                    isOutgoing ? 'text-red-600' : 'text-green-600'
                  }`}>
                    {formatCurrency(transaction.amount)}
                  </span>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Origin Info Card */}
        {originInfo && (
          <Card className="border-blue-200 bg-blue-50">
            <CardHeader className="pb-2">
              <CardTitle className="text-base flex items-center gap-2 text-blue-900">
                <Info className="h-5 w-5" />
                Origem da Transação
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-0">
              <div className="flex items-center justify-between">
                <div>
                  <p className="font-medium text-blue-900">{originInfo.label}</p>
                  <p className="text-sm text-blue-700">{originInfo.description}</p>
                </div>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => navigate(originInfo.link)}
                  className="flex items-center gap-1 border-blue-300 text-blue-700 hover:bg-blue-100"
                >
                  <ExternalLink className="h-4 w-4" />
                  <span className="hidden sm:inline">Ver Origem</span>
                </Button>
              </div>
            </CardContent>
          </Card>
        )}

        {/* Details Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {/* Transaction Details */}
          <Card>
            <CardHeader>
              <CardTitle className="text-base flex items-center gap-2">
                <Calendar className="h-5 w-5 text-gray-500" />
                Informações da Transação
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="flex justify-between items-center py-2 border-b border-gray-100">
                <span className="text-sm text-gray-600">Data da Transação</span>
                <span className="text-sm font-medium text-gray-900">{formatDate(transaction.transactionDate)}</span>
              </div>
              <div className="flex justify-between items-center py-2 border-b border-gray-100">
                <span className="text-sm text-gray-600">Tipo</span>
                <span className={`text-sm font-medium ${isOutgoing ? 'text-red-600' : 'text-green-600'}`}>
                  {transaction.type}
                </span>
              </div>
              <div className="flex justify-between items-center py-2 border-b border-gray-100">
                <span className="text-sm text-gray-600">Valor</span>
                <span className={`text-sm font-bold ${isOutgoing ? 'text-red-600' : 'text-green-600'}`}>
                  {formatCurrency(transaction.amount)}
                </span>
              </div>
              <div className="flex justify-between items-center py-2">
                <span className="text-sm text-gray-600">Criado em</span>
                <span className="text-sm text-gray-900">{formatDateTime(transaction.criadoEm)}</span>
              </div>
            </CardContent>
          </Card>

          {/* Account & Supplier Info */}
          <Card>
            <CardHeader>
              <CardTitle className="text-base flex items-center gap-2">
                <Wallet className="h-5 w-5 text-gray-500" />
                Conta e Relacionamentos
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              {transaction.accountName && (
                <div className="flex justify-between items-center py-2 border-b border-gray-100">
                  <span className="text-sm text-gray-600">Conta Corrente</span>
                  <span className="text-sm font-medium text-gray-900">{transaction.accountName}</span>
                </div>
              )}
              {transaction.supplierCustomerName && (
                <div className="flex justify-between items-start py-2 border-b border-gray-100">
                  <span className="text-sm text-gray-600">Fornecedor/Cliente</span>
                  <div className="text-right">
                    <span className="text-sm font-medium text-gray-900">{transaction.supplierCustomerName}</span>
                    {transaction.supplierCustomerId && (
                      <Button
                        variant="link"
                        size="sm"
                        className="h-auto p-0 text-xs text-primary-600"
                        onClick={() => navigate(`/supplier-customers/${transaction.supplierCustomerId}/edit`)}
                      >
                        Ver cadastro
                      </Button>
                    )}
                  </div>
                </div>
              )}
              {!transaction.accountName && !transaction.supplierCustomerName && (
                <div className="text-sm text-gray-500 italic py-4 text-center">
                  Nenhum relacionamento adicional
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        {/* Cost Center Distribution */}
        {transaction.costCenterDistributions && transaction.costCenterDistributions.length > 0 && (
          <Card>
            <CardHeader>
              <CardTitle className="text-base flex items-center gap-2">
                <PieChart className="h-5 w-5 text-gray-500" />
                Rateio por Centro de Custo
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                {transaction.costCenterDistributions.map((cc, index) => (
                  <div 
                    key={cc.costCenterId || index} 
                    className="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
                  >
                    <div className="flex items-center gap-3">
                      <Building2 className="h-5 w-5 text-gray-400" />
                      <div>
                        <p className="font-medium text-gray-900">{cc.costCenterName || `Centro de Custo #${cc.costCenterId}`}</p>
                        <p className="text-sm text-gray-500">{cc.percentage.toFixed(2)}% do valor</p>
                      </div>
                    </div>
                    <span className={`font-medium ${isOutgoing ? 'text-red-600' : 'text-green-600'}`}>
                      {formatCurrency(cc.amount)}
                    </span>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        )}
      </div>
    </MainLayout>
  );
}
