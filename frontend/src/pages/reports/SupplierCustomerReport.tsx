import { useState, useEffect, useCallback } from 'react';
import { MainLayout } from '../../components/layout';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Select } from '../../components/ui/Select';
import { 
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
  PieChart, Pie, Cell
} from 'recharts';
import { Calendar, RefreshCw, Users, Receipt, TrendingUp, TrendingDown } from 'lucide-react';
import reportService from '../../services/reportService';
import type { SupplierCustomerReport as SupplierCustomerReportType } from '../../services/reportService';

const COLORS = ['#3B82F6', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6', '#EC4899', '#06B6D4', '#84CC16'];

export function SupplierCustomerReport() {
  const [isLoading, setIsLoading] = useState(true);
  const [data, setData] = useState<SupplierCustomerReportType[]>([]);
  const [type, setType] = useState<'Pagar' | 'Receber' | 'Todos'>('Todos');
  
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
      const result = await reportService.getSupplierCustomerReport({ startDate, endDate, type });
      setData(result);
    } catch (error) {
      console.error('Erro ao carregar relatório:', error);
      // Mock data
      setData([
        { supplierCustomerId: 1, supplierCustomerName: 'Fornecedor ABC Ltda', totalPago: 250000, totalRecebido: 0, quantidadeTransacoes: 15 },
        { supplierCustomerId: 2, supplierCustomerName: 'Empresa XYZ S.A.', totalPago: 180000, totalRecebido: 350000, quantidadeTransacoes: 22 },
        { supplierCustomerId: 3, supplierCustomerName: 'Distribuidora 123', totalPago: 120000, totalRecebido: 0, quantidadeTransacoes: 8 },
        { supplierCustomerId: 4, supplierCustomerName: 'Cliente Premium', totalPago: 0, totalRecebido: 500000, quantidadeTransacoes: 12 },
        { supplierCustomerId: 5, supplierCustomerName: 'Tech Solutions', totalPago: 95000, totalRecebido: 0, quantidadeTransacoes: 5 },
        { supplierCustomerId: 6, supplierCustomerName: 'Mercado Central', totalPago: 75000, totalRecebido: 200000, quantidadeTransacoes: 30 },
      ]);
    } finally {
      setIsLoading(false);
    }
  }, [startDate, endDate, type]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const chartData = data
    .sort((a, b) => (b.totalPago + b.totalRecebido) - (a.totalPago + a.totalRecebido))
    .slice(0, 10)
    .map(item => ({
      name: item.supplierCustomerName.length > 20 
        ? item.supplierCustomerName.substring(0, 20) + '...' 
        : item.supplierCustomerName,
      pago: item.totalPago / 100,
      recebido: item.totalRecebido / 100
    }));

  const pieDataPago = data
    .filter(item => item.totalPago > 0)
    .sort((a, b) => b.totalPago - a.totalPago)
    .slice(0, 5)
    .map(item => ({
      name: item.supplierCustomerName.length > 15 
        ? item.supplierCustomerName.substring(0, 15) + '...' 
        : item.supplierCustomerName,
      value: item.totalPago / 100
    }));

  const totalPago = data.reduce((sum, item) => sum + item.totalPago, 0);
  const totalRecebido = data.reduce((sum, item) => sum + item.totalRecebido, 0);
  const totalTransacoes = data.reduce((sum, item) => sum + item.quantidadeTransacoes, 0);

  return (
    <MainLayout>
      <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Relatório por Fornecedor/Cliente</h1>
          <p className="text-gray-600">Análise de pagamentos e recebimentos por parceiro</p>
        </div>
        
        <div className="flex flex-wrap items-center gap-3">
          <Select
            value={type}
            onChange={(e) => setType(e.target.value as 'Pagar' | 'Receber' | 'Todos')}
            className="w-32"
          >
            <option value="Todos">Todos</option>
            <option value="Pagar">Fornecedores</option>
            <option value="Receber">Clientes</option>
          </Select>
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
                <p className="text-sm font-medium text-gray-500">Parceiros</p>
                <p className="text-2xl font-bold text-blue-600">{data.length}</p>
              </div>
              <div className="p-3 bg-blue-100 rounded-full">
                <Users className="h-6 w-6 text-blue-600" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Total Pago</p>
                <p className="text-2xl font-bold text-red-600">{formatCurrency(totalPago)}</p>
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
                <p className="text-sm font-medium text-gray-500">Total Recebido</p>
                <p className="text-2xl font-bold text-green-600">{formatCurrency(totalRecebido)}</p>
              </div>
              <div className="p-3 bg-green-100 rounded-full">
                <TrendingUp className="h-6 w-6 text-green-600" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Transações</p>
                <p className="text-2xl font-bold text-purple-600">{totalTransacoes}</p>
              </div>
              <div className="p-3 bg-purple-100 rounded-full">
                <Receipt className="h-6 w-6 text-purple-600" />
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Gráficos */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Gráfico de Barras - Top 10 */}
        <Card>
          <CardHeader>
            <CardTitle>Top 10 Parceiros por Movimentação</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-80">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={chartData} layout="vertical">
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis type="number" tickFormatter={(value) => `R$ ${(value / 1000).toFixed(0)}k`} />
                  <YAxis dataKey="name" type="category" width={100} tick={{ fontSize: 11 }} />
                  <Tooltip formatter={(value) => formatCurrency(Number(value) * 100)} />
                  <Legend />
                  <Bar dataKey="pago" name="Pago" fill="#EF4444" radius={[0, 4, 4, 0]} />
                  <Bar dataKey="recebido" name="Recebido" fill="#10B981" radius={[0, 4, 4, 0]} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>

        {/* Gráfico de Pizza - Maiores Fornecedores */}
        <Card>
          <CardHeader>
            <CardTitle>Top 5 Fornecedores (Pagamentos)</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-80">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={pieDataPago}
                    cx="50%"
                    cy="50%"
                    labelLine={false}
                    label={({ name, percent }) => `${name}: ${((percent || 0) * 100).toFixed(0)}%`}
                    outerRadius={100}
                    fill="#8884d8"
                    dataKey="value"
                  >
                    {pieDataPago.map((_, index) => (
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
          <CardTitle>Detalhamento por Fornecedor/Cliente</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nome</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Total Pago</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Total Recebido</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Saldo</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Transações</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {data.map((item, index) => {
                  const saldo = item.totalRecebido - item.totalPago;
                  return (
                    <tr key={item.supplierCustomerId} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center">
                          <div 
                            className="w-3 h-3 rounded-full mr-3" 
                            style={{ backgroundColor: COLORS[index % COLORS.length] }}
                          />
                          <span className="font-medium text-gray-900">{item.supplierCustomerName}</span>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-right text-red-600 font-medium">
                        {formatCurrency(item.totalPago)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-right text-green-600 font-medium">
                        {formatCurrency(item.totalRecebido)}
                      </td>
                      <td className={`px-6 py-4 whitespace-nowrap text-right font-bold ${saldo >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                        {formatCurrency(saldo)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-right text-gray-600">
                        {item.quantidadeTransacoes}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
              <tfoot className="bg-gray-100">
                <tr>
                  <td className="px-6 py-4 font-bold text-gray-900">Total</td>
                  <td className="px-6 py-4 text-right font-bold text-red-600">{formatCurrency(totalPago)}</td>
                  <td className="px-6 py-4 text-right font-bold text-green-600">{formatCurrency(totalRecebido)}</td>
                  <td className={`px-6 py-4 text-right font-bold ${totalRecebido - totalPago >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                    {formatCurrency(totalRecebido - totalPago)}
                  </td>
                  <td className="px-6 py-4 text-right font-bold text-gray-600">{totalTransacoes}</td>
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
