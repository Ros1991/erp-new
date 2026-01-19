import { useState, useEffect, useCallback } from 'react';
import { MainLayout } from '../../components/layout';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { 
  AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
  BarChart, Bar
} from 'recharts';
import { RefreshCw, TrendingUp, TrendingDown, Wallet, Calendar, ArrowDownCircle, ArrowUpCircle } from 'lucide-react';
import reportService from '../../services/reportService';
import type { FinancialForecast } from '../../services/reportService';

export function FinancialForecastReport() {
  const [isLoading, setIsLoading] = useState(true);
  const [data, setData] = useState<FinancialForecast | null>(null);
  const [months, setMonths] = useState(6);

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value / 100);
  };

  const formatDate = (dateStr: string) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('pt-BR');
  };

  const loadData = useCallback(async () => {
    setIsLoading(true);
    try {
      const result = await reportService.getFinancialForecast(months);
      setData(result);
    } catch (error) {
      console.error('Erro ao carregar relatório:', error);
      // Mock data
      const mockMeses = [];
      let saldoAcumulado = 1500000;
      const monthNames = ['jan', 'fev', 'mar', 'abr', 'mai', 'jun', 'jul', 'ago', 'set', 'out', 'nov', 'dez'];
      const currentMonth = new Date().getMonth();
      const currentYear = new Date().getFullYear();
      
      for (let i = 0; i < months; i++) {
        const monthIndex = (currentMonth + i) % 12;
        const year = currentYear + Math.floor((currentMonth + i) / 12);
        const aPagar = Math.floor(Math.random() * 300000) + 100000;
        const aReceber = Math.floor(Math.random() * 400000) + 150000;
        const saldo = aReceber - aPagar;
        saldoAcumulado += saldo;
        
        mockMeses.push({
          month: monthNames[monthIndex],
          year,
          aPagar,
          aReceber,
          saldo,
          saldoAcumulado
        });
      }

      setData({
        saldoAtual: 1500000,
        totalAPagar: mockMeses.reduce((sum, m) => sum + m.aPagar, 0),
        totalAReceber: mockMeses.reduce((sum, m) => sum + m.aReceber, 0),
        saldoProjetado: mockMeses[mockMeses.length - 1].saldoAcumulado,
        meses: mockMeses,
        contasAPagar: [
          { id: 1, description: 'Aluguel', supplierCustomerName: 'Imobiliária XYZ', amount: 250000, dueDate: new Date().toISOString(), type: 'Pagar' },
          { id: 2, description: 'Energia Elétrica', supplierCustomerName: 'CEMIG', amount: 85000, dueDate: new Date().toISOString(), type: 'Pagar' },
          { id: 3, description: 'Internet', supplierCustomerName: 'Vivo', amount: 25000, dueDate: new Date().toISOString(), type: 'Pagar' },
        ],
        contasAReceber: [
          { id: 4, description: 'Serviço de Consultoria', supplierCustomerName: 'Cliente ABC', amount: 500000, dueDate: new Date().toISOString(), type: 'Receber' },
          { id: 5, description: 'Venda de Produtos', supplierCustomerName: 'Cliente DEF', amount: 300000, dueDate: new Date().toISOString(), type: 'Receber' },
        ]
      });
    } finally {
      setIsLoading(false);
    }
  }, [months]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const chartData = data?.meses.map(item => ({
    name: `${item.month}/${item.year}`,
    aPagar: item.aPagar / 100,
    aReceber: item.aReceber / 100,
    saldo: item.saldo / 100,
    saldoAcumulado: item.saldoAcumulado / 100
  })) || [];

  return (
    <MainLayout>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Previsão Financeira</h1>
            <p className="text-gray-600">Projeção de receitas e despesas futuras</p>
          </div>
          
          <div className="flex flex-wrap items-center gap-3">
            <div className="flex items-center gap-2">
              <Calendar className="h-4 w-4 text-gray-500" />
              <select
                value={months}
                onChange={(e) => setMonths(Number(e.target.value))}
                className="px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value={3}>3 meses</option>
                <option value={6}>6 meses</option>
                <option value={12}>12 meses</option>
              </select>
            </div>
            
            <Button 
              onClick={loadData} 
              disabled={isLoading}
              className="flex items-center gap-2"
            >
              <RefreshCw className={`h-4 w-4 ${isLoading ? 'animate-spin' : ''}`} />
              Atualizar
            </Button>
          </div>
        </div>

        {isLoading ? (
          <div className="flex items-center justify-center h-64">
            <div className="text-gray-500">Carregando...</div>
          </div>
        ) : data ? (
          <>
            {/* Cards de Resumo */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
              <Card>
                <CardContent className="p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm font-medium text-gray-600">Saldo Atual</p>
                      <p className={`text-2xl font-bold ${data.saldoAtual >= 0 ? 'text-blue-600' : 'text-red-600'}`}>
                        {formatCurrency(data.saldoAtual)}
                      </p>
                    </div>
                    <Wallet className="h-8 w-8 text-blue-500" />
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardContent className="p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm font-medium text-gray-600">Total a Pagar</p>
                      <p className="text-2xl font-bold text-red-600">
                        {formatCurrency(data.totalAPagar)}
                      </p>
                    </div>
                    <ArrowDownCircle className="h-8 w-8 text-red-500" />
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardContent className="p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm font-medium text-gray-600">Total a Receber</p>
                      <p className="text-2xl font-bold text-green-600">
                        {formatCurrency(data.totalAReceber)}
                      </p>
                    </div>
                    <ArrowUpCircle className="h-8 w-8 text-green-500" />
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardContent className="p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm font-medium text-gray-600">Saldo Projetado</p>
                      <p className={`text-2xl font-bold ${data.saldoProjetado >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                        {formatCurrency(data.saldoProjetado)}
                      </p>
                    </div>
                    {data.saldoProjetado >= data.saldoAtual ? (
                      <TrendingUp className="h-8 w-8 text-green-500" />
                    ) : (
                      <TrendingDown className="h-8 w-8 text-red-500" />
                    )}
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Gráfico de Projeção Mensal */}
            <Card>
              <CardHeader>
                <CardTitle>Projeção Mensal</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="h-80">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={chartData}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="name" />
                      <YAxis tickFormatter={(value) => `R$ ${value.toLocaleString('pt-BR')}`} />
                      <Tooltip 
                        formatter={(value) => value !== undefined ? [`R$ ${value.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`, ''] : ['', '']}
                        labelFormatter={(label) => `Período: ${label}`}
                      />
                      <Legend />
                      <Bar dataKey="aReceber" name="A Receber" fill="#10b981" />
                      <Bar dataKey="aPagar" name="A Pagar" fill="#ef4444" />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </CardContent>
            </Card>

            {/* Gráfico de Saldo Acumulado */}
            <Card>
              <CardHeader>
                <CardTitle>Evolução do Saldo Projetado</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="h-80">
                  <ResponsiveContainer width="100%" height="100%">
                    <AreaChart data={chartData}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="name" />
                      <YAxis tickFormatter={(value) => `R$ ${value.toLocaleString('pt-BR')}`} />
                      <Tooltip 
                        formatter={(value) => value !== undefined ? [`R$ ${value.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`, ''] : ['', '']}
                      />
                      <Legend />
                      <Area 
                        type="monotone" 
                        dataKey="saldoAcumulado" 
                        name="Saldo Projetado" 
                        stroke="#3b82f6" 
                        fill="#93c5fd" 
                      />
                    </AreaChart>
                  </ResponsiveContainer>
                </div>
              </CardContent>
            </Card>

            {/* Tabelas de Contas */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Contas a Pagar */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <ArrowDownCircle className="h-5 w-5 text-red-500" />
                    Contas a Pagar ({data.contasAPagar.length})
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="overflow-x-auto">
                    <table className="min-w-full divide-y divide-gray-200">
                      <thead className="bg-gray-50">
                        <tr>
                          <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Descrição</th>
                          <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Fornecedor</th>
                          <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Vencimento</th>
                          <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Valor</th>
                        </tr>
                      </thead>
                      <tbody className="bg-white divide-y divide-gray-200">
                        {data.contasAPagar.slice(0, 10).map((conta) => (
                          <tr key={conta.id} className="hover:bg-gray-50">
                            <td className="px-4 py-3 text-sm text-gray-900">{conta.description}</td>
                            <td className="px-4 py-3 text-sm text-gray-600">{conta.supplierCustomerName}</td>
                            <td className="px-4 py-3 text-sm text-gray-600">{formatDate(conta.dueDate)}</td>
                            <td className="px-4 py-3 text-sm text-red-600 text-right font-medium">
                              {formatCurrency(conta.amount)}
                            </td>
                          </tr>
                        ))}
                        {data.contasAPagar.length === 0 && (
                          <tr>
                            <td colSpan={4} className="px-4 py-8 text-center text-gray-500">
                              Nenhuma conta a pagar pendente
                            </td>
                          </tr>
                        )}
                      </tbody>
                    </table>
                    {data.contasAPagar.length > 10 && (
                      <p className="mt-2 text-sm text-gray-500 text-center">
                        Mostrando 10 de {data.contasAPagar.length} contas
                      </p>
                    )}
                  </div>
                </CardContent>
              </Card>

              {/* Contas a Receber */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <ArrowUpCircle className="h-5 w-5 text-green-500" />
                    Contas a Receber ({data.contasAReceber.length})
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="overflow-x-auto">
                    <table className="min-w-full divide-y divide-gray-200">
                      <thead className="bg-gray-50">
                        <tr>
                          <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Descrição</th>
                          <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Cliente</th>
                          <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Vencimento</th>
                          <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Valor</th>
                        </tr>
                      </thead>
                      <tbody className="bg-white divide-y divide-gray-200">
                        {data.contasAReceber.slice(0, 10).map((conta) => (
                          <tr key={conta.id} className="hover:bg-gray-50">
                            <td className="px-4 py-3 text-sm text-gray-900">{conta.description}</td>
                            <td className="px-4 py-3 text-sm text-gray-600">{conta.supplierCustomerName}</td>
                            <td className="px-4 py-3 text-sm text-gray-600">{formatDate(conta.dueDate)}</td>
                            <td className="px-4 py-3 text-sm text-green-600 text-right font-medium">
                              {formatCurrency(conta.amount)}
                            </td>
                          </tr>
                        ))}
                        {data.contasAReceber.length === 0 && (
                          <tr>
                            <td colSpan={4} className="px-4 py-8 text-center text-gray-500">
                              Nenhuma conta a receber pendente
                            </td>
                          </tr>
                        )}
                      </tbody>
                    </table>
                    {data.contasAReceber.length > 10 && (
                      <p className="mt-2 text-sm text-gray-500 text-center">
                        Mostrando 10 de {data.contasAReceber.length} contas
                      </p>
                    )}
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Tabela de Projeção Detalhada */}
            <Card>
              <CardHeader>
                <CardTitle>Projeção Detalhada por Mês</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="overflow-x-auto">
                  <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                      <tr>
                        <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Período</th>
                        <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">A Receber</th>
                        <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">A Pagar</th>
                        <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Saldo do Mês</th>
                        <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Saldo Acumulado</th>
                      </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                      {data.meses.map((mes, index) => (
                        <tr key={index} className="hover:bg-gray-50">
                          <td className="px-4 py-3 text-sm font-medium text-gray-900">
                            {mes.month}/{mes.year}
                          </td>
                          <td className="px-4 py-3 text-sm text-green-600 text-right">
                            {formatCurrency(mes.aReceber)}
                          </td>
                          <td className="px-4 py-3 text-sm text-red-600 text-right">
                            {formatCurrency(mes.aPagar)}
                          </td>
                          <td className={`px-4 py-3 text-sm text-right font-medium ${mes.saldo >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                            {formatCurrency(mes.saldo)}
                          </td>
                          <td className={`px-4 py-3 text-sm text-right font-bold ${mes.saldoAcumulado >= 0 ? 'text-blue-600' : 'text-red-600'}`}>
                            {formatCurrency(mes.saldoAcumulado)}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </CardContent>
            </Card>
          </>
        ) : (
          <div className="flex items-center justify-center h-64">
            <div className="text-gray-500">Erro ao carregar dados</div>
          </div>
        )}
      </div>
    </MainLayout>
  );
}
