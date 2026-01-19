import { useState, useEffect, useCallback } from 'react';
import { MainLayout } from '../../components/layout';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { 
  AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
  ComposedChart, Line, Bar
} from 'recharts';
import { Calendar, RefreshCw, TrendingUp, TrendingDown, ArrowUpCircle, ArrowDownCircle, Activity } from 'lucide-react';
import reportService from '../../services/reportService';
import type { CashFlowItem } from '../../services/reportService';

export function CashFlowReport() {
  const [isLoading, setIsLoading] = useState(true);
  const [data, setData] = useState<CashFlowItem[]>([]);
  
  const today = new Date();
  const thirtyDaysAgo = new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000);
  
  const [startDate, setStartDate] = useState(thirtyDaysAgo.toISOString().split('T')[0]);
  const [endDate, setEndDate] = useState(today.toISOString().split('T')[0]);

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value / 100);
  };

  const formatDate = (dateStr: string) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' });
  };

  const loadData = useCallback(async () => {
    setIsLoading(true);
    try {
      const result = await reportService.getCashFlow({ startDate, endDate });
      setData(result);
    } catch (error) {
      console.error('Erro ao carregar relatório:', error);
      // Mock data - 30 dias
      const mockData: CashFlowItem[] = [];
      let saldoAcumulado = 1000000; // Saldo inicial de R$ 10.000
      
      for (let i = 30; i >= 0; i--) {
        const date = new Date(today.getTime() - i * 24 * 60 * 60 * 1000);
        const entradas = Math.floor(Math.random() * 50000) + 10000;
        const saidas = Math.floor(Math.random() * 40000) + 5000;
        const saldo = entradas - saidas;
        saldoAcumulado += saldo;
        
        mockData.push({
          date: date.toISOString().split('T')[0],
          entradas,
          saidas,
          saldo,
          saldoAcumulado
        });
      }
      setData(mockData);
    } finally {
      setIsLoading(false);
    }
  }, [startDate, endDate]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const chartData = data.map(item => ({
    date: formatDate(item.date),
    entradas: item.entradas / 100,
    saidas: item.saidas / 100,
    saldo: item.saldo / 100,
    saldoAcumulado: item.saldoAcumulado / 100
  }));

  const totalEntradas = data.reduce((sum, item) => sum + item.entradas, 0);
  const totalSaidas = data.reduce((sum, item) => sum + item.saidas, 0);
  const saldoPeriodo = totalEntradas - totalSaidas;
  const saldoFinal = data.length > 0 ? data[data.length - 1].saldoAcumulado : 0;

  const diasPositivos = data.filter(item => item.saldo > 0).length;
  const diasNegativos = data.filter(item => item.saldo < 0).length;

  return (
    <MainLayout>
      <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Fluxo de Caixa</h1>
          <p className="text-gray-600">Movimentação financeira diária</p>
        </div>
        
        <div className="flex flex-wrap items-center gap-3">
          <div className="flex items-center gap-2">
            <Calendar className="h-4 w-4 text-gray-500" />
            <Input
              type="date"
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
              className="w-40"
            />
            <span className="text-gray-500">até</span>
            <Input
              type="date"
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
              className="w-40"
            />
          </div>
          <Button onClick={loadData} disabled={isLoading}>
            <RefreshCw className={`h-4 w-4 mr-2 ${isLoading ? 'animate-spin' : ''}`} />
            Atualizar
          </Button>
        </div>
      </div>

      {/* KPIs */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4">
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Total Entradas</p>
                <p className="text-xl font-bold text-green-600">{formatCurrency(totalEntradas)}</p>
              </div>
              <div className="p-3 bg-green-100 rounded-full">
                <ArrowUpCircle className="h-5 w-5 text-green-600" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Total Saídas</p>
                <p className="text-xl font-bold text-red-600">{formatCurrency(totalSaidas)}</p>
              </div>
              <div className="p-3 bg-red-100 rounded-full">
                <ArrowDownCircle className="h-5 w-5 text-red-600" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Saldo Período</p>
                <p className={`text-xl font-bold ${saldoPeriodo >= 0 ? 'text-blue-600' : 'text-red-600'}`}>
                  {formatCurrency(saldoPeriodo)}
                </p>
              </div>
              <div className={`p-3 rounded-full ${saldoPeriodo >= 0 ? 'bg-blue-100' : 'bg-red-100'}`}>
                {saldoPeriodo >= 0 
                  ? <TrendingUp className="h-5 w-5 text-blue-600" />
                  : <TrendingDown className="h-5 w-5 text-red-600" />
                }
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Saldo Final</p>
                <p className={`text-xl font-bold ${saldoFinal >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                  {formatCurrency(saldoFinal)}
                </p>
              </div>
              <div className={`p-3 rounded-full ${saldoFinal >= 0 ? 'bg-green-100' : 'bg-red-100'}`}>
                <Activity className="h-5 w-5 text-green-600" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div>
              <p className="text-sm font-medium text-gray-500">Dias +/-</p>
              <div className="flex items-center gap-2 mt-1">
                <span className="text-lg font-bold text-green-600">{diasPositivos}</span>
                <span className="text-gray-400">/</span>
                <span className="text-lg font-bold text-red-600">{diasNegativos}</span>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Gráfico Principal - Fluxo de Caixa */}
      <Card>
        <CardHeader>
          <CardTitle>Evolução do Saldo Acumulado</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <AreaChart data={chartData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" tick={{ fontSize: 11 }} />
                <YAxis tickFormatter={(value) => `R$ ${(value / 1000).toFixed(0)}k`} />
                <Tooltip formatter={(value) => formatCurrency(Number(value) * 100)} />
                <Legend />
                <Area 
                  type="monotone" 
                  dataKey="saldoAcumulado" 
                  name="Saldo Acumulado"
                  stroke="#3B82F6" 
                  fill="#93C5FD" 
                  fillOpacity={0.6}
                />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </CardContent>
      </Card>

      {/* Gráfico Entradas vs Saídas */}
      <Card>
        <CardHeader>
          <CardTitle>Entradas vs Saídas por Dia</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <ComposedChart data={chartData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="date" tick={{ fontSize: 11 }} />
                <YAxis tickFormatter={(value) => `R$ ${(value / 1000).toFixed(0)}k`} />
                <Tooltip formatter={(value) => formatCurrency(Number(value) * 100)} />
                <Legend />
                <Bar dataKey="entradas" name="Entradas" fill="#10B981" radius={[4, 4, 0, 0]} />
                <Bar dataKey="saidas" name="Saídas" fill="#EF4444" radius={[4, 4, 0, 0]} />
                <Line 
                  type="monotone" 
                  dataKey="saldo" 
                  name="Saldo Diário" 
                  stroke="#3B82F6" 
                  strokeWidth={2}
                  dot={false}
                />
              </ComposedChart>
            </ResponsiveContainer>
          </div>
        </CardContent>
      </Card>

      {/* Tabela Detalhada */}
      <Card>
        <CardHeader>
          <CardTitle>Movimentação Diária</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto max-h-96">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50 sticky top-0">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Data</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Entradas</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Saídas</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Saldo Dia</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Saldo Acumulado</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {data.map((item) => (
                  <tr key={item.date} className="hover:bg-gray-50">
                    <td className="px-6 py-3 whitespace-nowrap text-sm font-medium text-gray-900">
                      {formatDate(item.date)}
                    </td>
                    <td className="px-6 py-3 whitespace-nowrap text-right text-sm text-green-600">
                      {formatCurrency(item.entradas)}
                    </td>
                    <td className="px-6 py-3 whitespace-nowrap text-right text-sm text-red-600">
                      {formatCurrency(item.saidas)}
                    </td>
                    <td className={`px-6 py-3 whitespace-nowrap text-right text-sm font-medium ${item.saldo >= 0 ? 'text-blue-600' : 'text-red-600'}`}>
                      {formatCurrency(item.saldo)}
                    </td>
                    <td className={`px-6 py-3 whitespace-nowrap text-right text-sm font-bold ${item.saldoAcumulado >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                      {formatCurrency(item.saldoAcumulado)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>
      </div>
    </MainLayout>
  );
}
