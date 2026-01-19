import { useState, useEffect, useCallback } from 'react';
import { MainLayout } from '../../components/layout';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { 
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer
} from 'recharts';
import { Calendar, RefreshCw, Wallet, TrendingUp, TrendingDown, ArrowUpCircle, ArrowDownCircle } from 'lucide-react';
import reportService from '../../services/reportService';
import type { AccountReport as AccountReportType } from '../../services/reportService';

const COLORS = ['#3B82F6', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6'];

export function AccountReport() {
  const [isLoading, setIsLoading] = useState(true);
  const [data, setData] = useState<AccountReportType[]>([]);
  
  const today = new Date();
  const firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
  
  const [startDate, setStartDate] = useState(firstDayOfMonth.toISOString().split('T')[0]);
  const [endDate, setEndDate] = useState(today.toISOString().split('T')[0]);

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value / 100);
  };

  const loadData = useCallback(async () => {
    setIsLoading(true);
    try {
      const result = await reportService.getAccountReport({ startDate, endDate });
      setData(result);
    } catch (error) {
      console.error('Erro ao carregar relatório:', error);
      // Mock data
      setData([
        { accountId: 1, accountName: 'Banco do Brasil', accountType: 'Banco', saldoInicial: 1000000, totalEntradas: 500000, totalSaidas: 300000, saldoFinal: 1200000 },
        { accountId: 2, accountName: 'Itaú', accountType: 'Banco', saldoInicial: 500000, totalEntradas: 300000, totalSaidas: 150000, saldoFinal: 650000 },
        { accountId: 3, accountName: 'Caixa', accountType: 'Banco', saldoInicial: 200000, totalEntradas: 100000, totalSaidas: 80000, saldoFinal: 220000 },
        { accountId: 4, accountName: 'Sócio João', accountType: 'Sócio', saldoInicial: 0, totalEntradas: 50000, totalSaidas: 100000, saldoFinal: -50000 },
      ]);
    } finally {
      setIsLoading(false);
    }
  }, [startDate, endDate]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const chartData = data.map(item => ({
    name: item.accountName,
    entradas: item.totalEntradas / 100,
    saidas: item.totalSaidas / 100,
    saldo: item.saldoFinal / 100
  }));

  const totalSaldoFinal = data.reduce((sum, item) => sum + item.saldoFinal, 0);
  const totalEntradas = data.reduce((sum, item) => sum + item.totalEntradas, 0);
  const totalSaidas = data.reduce((sum, item) => sum + item.totalSaidas, 0);

  return (
    <MainLayout>
      <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Relatório por Conta Corrente</h1>
          <p className="text-gray-600">Movimentação financeira por conta</p>
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
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Contas</p>
                <p className="text-2xl font-bold text-blue-600">{data.length}</p>
              </div>
              <div className="p-3 bg-blue-100 rounded-full">
                <Wallet className="h-6 w-6 text-blue-600" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Total Entradas</p>
                <p className="text-2xl font-bold text-green-600">{formatCurrency(totalEntradas)}</p>
              </div>
              <div className="p-3 bg-green-100 rounded-full">
                <ArrowUpCircle className="h-6 w-6 text-green-600" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Total Saídas</p>
                <p className="text-2xl font-bold text-red-600">{formatCurrency(totalSaidas)}</p>
              </div>
              <div className="p-3 bg-red-100 rounded-full">
                <ArrowDownCircle className="h-6 w-6 text-red-600" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Saldo Total</p>
                <p className={`text-2xl font-bold ${totalSaldoFinal >= 0 ? 'text-blue-600' : 'text-red-600'}`}>
                  {formatCurrency(totalSaldoFinal)}
                </p>
              </div>
              <div className={`p-3 rounded-full ${totalSaldoFinal >= 0 ? 'bg-blue-100' : 'bg-red-100'}`}>
                {totalSaldoFinal >= 0 
                  ? <TrendingUp className="h-6 w-6 text-blue-600" />
                  : <TrendingDown className="h-6 w-6 text-red-600" />
                }
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Gráfico */}
      <Card>
        <CardHeader>
          <CardTitle>Movimentação por Conta</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={chartData} layout="vertical">
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis type="number" tickFormatter={(value) => `R$ ${(value / 1000).toFixed(0)}k`} />
                <YAxis dataKey="name" type="category" width={120} />
                <Tooltip formatter={(value) => formatCurrency(Number(value) * 100)} />
                <Legend />
                <Bar dataKey="entradas" name="Entradas" fill="#10B981" radius={[0, 4, 4, 0]} />
                <Bar dataKey="saidas" name="Saídas" fill="#EF4444" radius={[0, 4, 4, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </CardContent>
      </Card>

      {/* Tabela Detalhada */}
      <Card>
        <CardHeader>
          <CardTitle>Detalhamento por Conta</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Conta</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Tipo</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Saldo Inicial</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Entradas</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Saídas</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Saldo Final</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {data.map((item, index) => (
                  <tr key={item.accountId} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        <div 
                          className="w-3 h-3 rounded-full mr-3" 
                          style={{ backgroundColor: COLORS[index % COLORS.length] }}
                        />
                        <span className="font-medium text-gray-900">{item.accountName}</span>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-2 py-1 text-xs font-medium rounded-full ${
                        item.accountType === 'Banco' 
                          ? 'bg-blue-100 text-blue-800' 
                          : item.accountType === 'Sócio'
                          ? 'bg-purple-100 text-purple-800'
                          : 'bg-gray-100 text-gray-800'
                      }`}>
                        {item.accountType}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-gray-600">
                      {formatCurrency(item.saldoInicial)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-green-600 font-medium">
                      {formatCurrency(item.totalEntradas)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-red-600 font-medium">
                      {formatCurrency(item.totalSaidas)}
                    </td>
                    <td className={`px-6 py-4 whitespace-nowrap text-right font-bold ${item.saldoFinal >= 0 ? 'text-blue-600' : 'text-red-600'}`}>
                      {formatCurrency(item.saldoFinal)}
                    </td>
                  </tr>
                ))}
              </tbody>
              <tfoot className="bg-gray-100">
                <tr>
                  <td className="px-6 py-4 font-bold text-gray-900" colSpan={2}>Total</td>
                  <td className="px-6 py-4 text-right text-gray-600">-</td>
                  <td className="px-6 py-4 text-right font-bold text-green-600">{formatCurrency(totalEntradas)}</td>
                  <td className="px-6 py-4 text-right font-bold text-red-600">{formatCurrency(totalSaidas)}</td>
                  <td className={`px-6 py-4 text-right font-bold ${totalSaldoFinal >= 0 ? 'text-blue-600' : 'text-red-600'}`}>
                    {formatCurrency(totalSaldoFinal)}
                  </td>
                </tr>
              </tfoot>
            </table>
          </div>
        </CardContent>
      </Card>
      </div>
    </MainLayout>
  );
}
