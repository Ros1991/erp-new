import { useState, useEffect, useCallback } from 'react';
import { MainLayout } from '../../components/layout';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { 
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
  PieChart, Pie, Cell, Area, AreaChart
} from 'recharts';
import { 
  TrendingUp, TrendingDown, Calendar, 
  ArrowUpCircle, ArrowDownCircle, Wallet, RefreshCw
} from 'lucide-react';
import reportService from '../../services/reportService';
import type { FinancialSummary } from '../../services/reportService';

const COLORS = ['#10B981', '#EF4444', '#3B82F6', '#F59E0B', '#8B5CF6', '#EC4899'];

export function FinancialDashboard() {
  const [isLoading, setIsLoading] = useState(true);
  const [summary, setSummary] = useState<FinancialSummary | null>(null);
  
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
      const data = await reportService.getFinancialSummary({
        startDate,
        endDate
      });
      setSummary(data);
    } catch (error) {
      console.error('Erro ao carregar resumo financeiro:', error);
      // Mock data para demonstração
      setSummary({
        totalReceitas: 15000000,
        totalDespesas: 12000000,
        saldo: 3000000,
        receitasPorMes: [
          { month: 'Jan', value: 1200000 },
          { month: 'Fev', value: 1350000 },
          { month: 'Mar', value: 1100000 },
          { month: 'Abr', value: 1500000 },
          { month: 'Mai', value: 1400000 },
          { month: 'Jun', value: 1600000 },
          { month: 'Jul', value: 1450000 },
          { month: 'Ago', value: 1300000 },
          { month: 'Set', value: 1550000 },
          { month: 'Out', value: 1650000 },
          { month: 'Nov', value: 1400000 },
          { month: 'Dez', value: 0 },
        ],
        despesasPorMes: [
          { month: 'Jan', value: 950000 },
          { month: 'Fev', value: 1100000 },
          { month: 'Mar', value: 900000 },
          { month: 'Abr', value: 1200000 },
          { month: 'Mai', value: 1150000 },
          { month: 'Jun', value: 1300000 },
          { month: 'Jul', value: 1100000 },
          { month: 'Ago', value: 1050000 },
          { month: 'Set', value: 1250000 },
          { month: 'Out', value: 1350000 },
          { month: 'Nov', value: 1150000 },
          { month: 'Dez', value: 0 },
        ]
      });
    } finally {
      setIsLoading(false);
    }
  }, [startDate, endDate]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const chartData = summary ? summary.receitasPorMes.map((r, i) => ({
    month: r.month,
    receitas: r.value / 100,
    despesas: summary.despesasPorMes[i]?.value / 100 || 0,
    saldo: (r.value - (summary.despesasPorMes[i]?.value || 0)) / 100
  })) : [];

  const pieData = summary ? [
    { name: 'Receitas', value: summary.totalReceitas / 100 },
    { name: 'Despesas', value: summary.totalDespesas / 100 }
  ] : [];

  return (
    <MainLayout>
      <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Dashboard Financeiro</h1>
          <p className="text-gray-600">Visão geral das finanças da empresa</p>
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
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Total Receitas</p>
                <p className="text-2xl font-bold text-green-600">
                  {summary ? formatCurrency(summary.totalReceitas) : '-'}
                </p>
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
                <p className="text-sm font-medium text-gray-500">Total Despesas</p>
                <p className="text-2xl font-bold text-red-600">
                  {summary ? formatCurrency(summary.totalDespesas) : '-'}
                </p>
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
                <p className="text-sm font-medium text-gray-500">Saldo</p>
                <p className={`text-2xl font-bold ${summary && summary.saldo >= 0 ? 'text-blue-600' : 'text-red-600'}`}>
                  {summary ? formatCurrency(summary.saldo) : '-'}
                </p>
              </div>
              <div className={`p-3 rounded-full ${summary && summary.saldo >= 0 ? 'bg-blue-100' : 'bg-red-100'}`}>
                <Wallet className={`h-6 w-6 ${summary && summary.saldo >= 0 ? 'text-blue-600' : 'text-red-600'}`} />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Margem</p>
                <p className={`text-2xl font-bold ${summary && summary.saldo >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                  {summary && summary.totalReceitas > 0 
                    ? `${((summary.saldo / summary.totalReceitas) * 100).toFixed(1)}%`
                    : '-'}
                </p>
              </div>
              <div className={`p-3 rounded-full ${summary && summary.saldo >= 0 ? 'bg-green-100' : 'bg-red-100'}`}>
                {summary && summary.saldo >= 0 
                  ? <TrendingUp className="h-6 w-6 text-green-600" />
                  : <TrendingDown className="h-6 w-6 text-red-600" />
                }
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Gráficos */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Gráfico de Barras - Receitas vs Despesas por Mês */}
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle>Receitas vs Despesas por Mês</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-80">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="month" />
                  <YAxis tickFormatter={(value) => `R$ ${(value / 1000).toFixed(0)}k`} />
                  <Tooltip 
                    formatter={(value) => formatCurrency(Number(value) * 100)}
                    labelStyle={{ color: '#374151' }}
                  />
                  <Legend />
                  <Bar dataKey="receitas" name="Receitas" fill="#10B981" radius={[4, 4, 0, 0]} />
                  <Bar dataKey="despesas" name="Despesas" fill="#EF4444" radius={[4, 4, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>

        {/* Gráfico de Pizza */}
        <Card>
          <CardHeader>
            <CardTitle>Distribuição</CardTitle>
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

      {/* Gráfico de Linha - Evolução do Saldo */}
      <Card>
        <CardHeader>
          <CardTitle>Evolução do Saldo Mensal</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="h-80">
            <ResponsiveContainer width="100%" height="100%">
              <AreaChart data={chartData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="month" />
                <YAxis tickFormatter={(value) => `R$ ${(value / 1000).toFixed(0)}k`} />
                <Tooltip formatter={(value) => formatCurrency(Number(value) * 100)} />
                <Legend />
                <Area 
                  type="monotone" 
                  dataKey="saldo" 
                  name="Saldo"
                  stroke="#3B82F6" 
                  fill="#93C5FD" 
                  fillOpacity={0.6}
                />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        </CardContent>
      </Card>
      </div>
    </MainLayout>
  );
}
