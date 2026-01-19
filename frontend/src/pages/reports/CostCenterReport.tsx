import { useState, useEffect, useCallback } from 'react';
import { MainLayout } from '../../components/layout';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { 
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
  PieChart, Pie, Cell
} from 'recharts';
import { Calendar, RefreshCw, Building2, TrendingUp, TrendingDown } from 'lucide-react';
import reportService from '../../services/reportService';
import type { CostCenterReport as CostCenterReportType } from '../../services/reportService';

const COLORS = ['#3B82F6', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6', '#EC4899', '#06B6D4', '#84CC16'];

export function CostCenterReport() {
  const [isLoading, setIsLoading] = useState(true);
  const [data, setData] = useState<CostCenterReportType[]>([]);
  
  const today = new Date();
  const twelveMonthsAgo = new Date(today.getFullYear(), today.getMonth() - 11, 1);
  
  const [startDate, setStartDate] = useState(twelveMonthsAgo.toISOString().split('T')[0]);
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
      const result = await reportService.getCostCenterReport({ startDate, endDate });
      setData(result);
    } catch (error) {
      console.error('Erro ao carregar relatório:', error);
      // Mock data
      setData([
        { costCenterId: 1, costCenterName: 'Administrativo', totalReceitas: 500000, totalDespesas: 350000, saldo: 150000, percentualDespesas: 25 },
        { costCenterId: 2, costCenterName: 'Comercial', totalReceitas: 800000, totalDespesas: 450000, saldo: 350000, percentualDespesas: 32 },
        { costCenterId: 3, costCenterName: 'Operacional', totalReceitas: 300000, totalDespesas: 280000, saldo: 20000, percentualDespesas: 20 },
        { costCenterId: 4, costCenterName: 'TI', totalReceitas: 200000, totalDespesas: 180000, saldo: 20000, percentualDespesas: 13 },
        { costCenterId: 5, costCenterName: 'RH', totalReceitas: 100000, totalDespesas: 140000, saldo: -40000, percentualDespesas: 10 },
      ]);
    } finally {
      setIsLoading(false);
    }
  }, [startDate, endDate]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const chartData = data.map(item => ({
    name: item.costCenterName,
    receitas: item.totalReceitas / 100,
    despesas: item.totalDespesas / 100,
    saldo: item.saldo / 100
  }));

  const pieData = data.map(item => ({
    name: item.costCenterName,
    value: item.totalDespesas / 100
  }));

  const totalDespesas = data.reduce((sum, item) => sum + item.totalDespesas, 0);
  const totalReceitas = data.reduce((sum, item) => sum + item.totalReceitas, 0);

  return (
    <MainLayout>
      <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Relatório por Centro de Custo</h1>
          <p className="text-gray-600">Análise financeira por centro de custo</p>
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
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Centros de Custo</p>
                <p className="text-2xl font-bold text-blue-600">{data.length}</p>
              </div>
              <div className="p-3 bg-blue-100 rounded-full">
                <Building2 className="h-6 w-6 text-blue-600" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Total Despesas</p>
                <p className="text-2xl font-bold text-red-600">{formatCurrency(totalDespesas)}</p>
              </div>
              <div className="p-3 bg-red-100 rounded-full">
                <TrendingDown className="h-6 w-6 text-red-600" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Total Receitas</p>
                <p className="text-2xl font-bold text-green-600">{formatCurrency(totalReceitas)}</p>
              </div>
              <div className="p-3 bg-green-100 rounded-full">
                <TrendingUp className="h-6 w-6 text-green-600" />
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Gráficos */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Gráfico de Barras */}
        <Card>
          <CardHeader>
            <CardTitle>Receitas vs Despesas por Centro de Custo</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-80">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={chartData} layout="vertical">
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis type="number" tickFormatter={(value) => `R$ ${(value / 1000).toFixed(0)}k`} />
                  <YAxis dataKey="name" type="category" width={100} />
                  <Tooltip formatter={(value) => formatCurrency(Number(value) * 100)} />
                  <Legend />
                  <Bar dataKey="receitas" name="Receitas" fill="#10B981" radius={[0, 4, 4, 0]} />
                  <Bar dataKey="despesas" name="Despesas" fill="#EF4444" radius={[0, 4, 4, 0]} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>

        {/* Gráfico de Pizza */}
        <Card>
          <CardHeader>
            <CardTitle>Distribuição de Despesas por Centro de Custo</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-80">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={pieData}
                    cx="50%"
                    cy="50%"
                    labelLine={false}
                    label={({ name, percent }) => `${name}: ${((percent || 0) * 100).toFixed(0)}%`}
                    outerRadius={100}
                    fill="#8884d8"
                    dataKey="value"
                  >
                    {pieData.map((_, index) => (
                      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip formatter={(value) => formatCurrency(Number(value) * 100)} />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Tabela Detalhada */}
      <Card>
        <CardHeader>
          <CardTitle>Detalhamento por Centro de Custo</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Centro de Custo</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Receitas</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Despesas</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Saldo</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">% Despesas</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {data.map((item) => (
                  <tr key={item.costCenterId} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        <Building2 className="h-5 w-5 text-gray-400 mr-2" />
                        <span className="font-medium text-gray-900">{item.costCenterName}</span>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-green-600 font-medium">
                      {formatCurrency(item.totalReceitas)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-red-600 font-medium">
                      {formatCurrency(item.totalDespesas)}
                    </td>
                    <td className={`px-6 py-4 whitespace-nowrap text-right font-bold ${item.saldo >= 0 ? 'text-blue-600' : 'text-red-600'}`}>
                      {formatCurrency(item.saldo)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right">
                      <div className="flex items-center justify-end">
                        <div className="w-16 bg-gray-200 rounded-full h-2 mr-2">
                          <div 
                            className="bg-blue-600 h-2 rounded-full" 
                            style={{ width: `${Math.min(item.percentualDespesas ?? 0, 100)}%` }}
                          />
                        </div>
                        <span className="text-gray-600">{(item.percentualDespesas ?? 0).toFixed(1)}%</span>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
              <tfoot className="bg-gray-100">
                <tr>
                  <td className="px-6 py-4 font-bold text-gray-900">Total</td>
                  <td className="px-6 py-4 text-right font-bold text-green-600">{formatCurrency(totalReceitas)}</td>
                  <td className="px-6 py-4 text-right font-bold text-red-600">{formatCurrency(totalDespesas)}</td>
                  <td className={`px-6 py-4 text-right font-bold ${totalReceitas - totalDespesas >= 0 ? 'text-blue-600' : 'text-red-600'}`}>
                    {formatCurrency(totalReceitas - totalDespesas)}
                  </td>
                  <td className="px-6 py-4 text-right font-bold text-gray-600">100%</td>
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
