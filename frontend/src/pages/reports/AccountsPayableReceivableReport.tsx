import { useState, useEffect, useCallback } from 'react';
import { MainLayout } from '../../components/layout';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Select } from '../../components/ui/Select';
import { 
  PieChart, Pie, Cell, ResponsiveContainer, BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend
} from 'recharts';
import { RefreshCw, AlertTriangle, Clock, Calendar, CheckCircle, AlertCircle } from 'lucide-react';
import reportService from '../../services/reportService';
import type { AccountPayableReceivableReport as ReportType } from '../../services/reportService';

const COLORS = {
  vencidas: '#EF4444',
  hoje: '#F59E0B',
  seteDias: '#3B82F6',
  trintaDias: '#8B5CF6',
  aVencer: '#10B981'
};

export function AccountsPayableReceivableReport() {
  const [isLoading, setIsLoading] = useState(true);
  const [data, setData] = useState<ReportType | null>(null);
  const [type, setType] = useState<'Pagar' | 'Receber' | 'Todos'>('Todos');

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
      const result = await reportService.getAccountPayableReceivableReport({ type });
      setData(result);
    } catch (error) {
      console.error('Erro ao carregar relatório:', error);
      // Mock data
      setData({
        vencidas: { quantidade: 5, valor: 150000 },
        vencendoHoje: { quantidade: 2, valor: 45000 },
        vencendo7Dias: { quantidade: 8, valor: 230000 },
        vencendo30Dias: { quantidade: 12, valor: 480000 },
        aVencer: { quantidade: 25, valor: 950000 },
        items: [
          { id: 1, description: 'Aluguel Janeiro', supplierCustomerName: 'Imobiliária ABC', amount: 50000, dueDate: '2026-01-10', type: 'Pagar', isPaid: false, daysOverdue: 8 },
          { id: 2, description: 'Fornecedor XYZ', supplierCustomerName: 'XYZ Ltda', amount: 35000, dueDate: '2026-01-12', type: 'Pagar', isPaid: false, daysOverdue: 6 },
          { id: 3, description: 'Energia Elétrica', supplierCustomerName: 'CEMIG', amount: 25000, dueDate: '2026-01-15', type: 'Pagar', isPaid: false, daysOverdue: 3 },
          { id: 4, description: 'Internet', supplierCustomerName: 'Provedor Net', amount: 15000, dueDate: '2026-01-18', type: 'Pagar', isPaid: false, daysOverdue: 0 },
          { id: 5, description: 'Venda Cliente A', supplierCustomerName: 'Cliente A', amount: 80000, dueDate: '2026-01-20', type: 'Receber', isPaid: false, daysOverdue: -2 },
          { id: 6, description: 'Serviço Consultoria', supplierCustomerName: 'Cliente B', amount: 120000, dueDate: '2026-01-25', type: 'Receber', isPaid: false, daysOverdue: -7 },
          { id: 7, description: 'Produto Premium', supplierCustomerName: 'Cliente C', amount: 200000, dueDate: '2026-02-01', type: 'Receber', isPaid: false, daysOverdue: -14 },
          { id: 8, description: 'Manutenção', supplierCustomerName: 'Fornecedor Tech', amount: 45000, dueDate: '2026-02-05', type: 'Pagar', isPaid: false, daysOverdue: -18 },
        ]
      });
    } finally {
      setIsLoading(false);
    }
  }, [type]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const pieData = data ? [
    { name: 'Vencidas', value: data.vencidas.valor / 100, color: COLORS.vencidas },
    { name: 'Vencendo Hoje', value: data.vencendoHoje.valor / 100, color: COLORS.hoje },
    { name: 'Próximos 7 dias', value: data.vencendo7Dias.valor / 100, color: COLORS.seteDias },
    { name: 'Próximos 30 dias', value: data.vencendo30Dias.valor / 100, color: COLORS.trintaDias },
    { name: 'A Vencer', value: data.aVencer.valor / 100, color: COLORS.aVencer },
  ].filter(item => item.value > 0) : [];

  const barData = data ? [
    { name: 'Vencidas', quantidade: data.vencidas.quantidade, valor: data.vencidas.valor / 100 },
    { name: 'Hoje', quantidade: data.vencendoHoje.quantidade, valor: data.vencendoHoje.valor / 100 },
    { name: '7 dias', quantidade: data.vencendo7Dias.quantidade, valor: data.vencendo7Dias.valor / 100 },
    { name: '30 dias', quantidade: data.vencendo30Dias.quantidade, valor: data.vencendo30Dias.valor / 100 },
    { name: 'A Vencer', quantidade: data.aVencer.quantidade, valor: data.aVencer.valor / 100 },
  ] : [];

  const totalValor = data 
    ? data.vencidas.valor + data.vencendoHoje.valor + data.vencendo7Dias.valor + data.vencendo30Dias.valor + data.aVencer.valor
    : 0;

  const totalQuantidade = data 
    ? data.vencidas.quantidade + data.vencendoHoje.quantidade + data.vencendo7Dias.quantidade + data.vencendo30Dias.quantidade + data.aVencer.quantidade
    : 0;

  const getStatusBadge = (daysOverdue: number) => {
    if (daysOverdue > 0) {
      return (
        <span className="px-2 py-1 text-xs font-medium rounded-full bg-red-100 text-red-800">
          {daysOverdue} dias em atraso
        </span>
      );
    } else if (daysOverdue === 0) {
      return (
        <span className="px-2 py-1 text-xs font-medium rounded-full bg-amber-100 text-amber-800">
          Vence hoje
        </span>
      );
    } else {
      return (
        <span className="px-2 py-1 text-xs font-medium rounded-full bg-green-100 text-green-800">
          Vence em {Math.abs(daysOverdue)} dias
        </span>
      );
    }
  };

  return (
    <MainLayout>
      <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Contas a Pagar/Receber</h1>
          <p className="text-gray-600">Análise de vencimentos e pendências</p>
        </div>
        
        <div className="flex items-center gap-3">
          <Select
            value={type}
            onChange={(e) => setType(e.target.value as 'Pagar' | 'Receber' | 'Todos')}
            className="w-40"
          >
            <option value="Todos">Todos</option>
            <option value="Pagar">A Pagar</option>
            <option value="Receber">A Receber</option>
          </Select>
          <Button onClick={loadData} disabled={isLoading}>
            <RefreshCw className={`h-4 w-4 mr-2 ${isLoading ? 'animate-spin' : ''}`} />
            Atualizar
          </Button>
        </div>
      </div>

      {/* KPIs de Status */}
      <div className="grid grid-cols-2 md:grid-cols-5 gap-4">
        <Card className="border-l-4 border-l-red-500">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <AlertTriangle className="h-8 w-8 text-red-500" />
              <div>
                <p className="text-xs text-gray-500">Vencidas</p>
                <p className="text-lg font-bold text-red-600">{data?.vencidas.quantidade || 0}</p>
                <p className="text-sm text-red-600">{formatCurrency(data?.vencidas.valor || 0)}</p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-l-4 border-l-amber-500">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <Clock className="h-8 w-8 text-amber-500" />
              <div>
                <p className="text-xs text-gray-500">Vence Hoje</p>
                <p className="text-lg font-bold text-amber-600">{data?.vencendoHoje.quantidade || 0}</p>
                <p className="text-sm text-amber-600">{formatCurrency(data?.vencendoHoje.valor || 0)}</p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-l-4 border-l-blue-500">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <Calendar className="h-8 w-8 text-blue-500" />
              <div>
                <p className="text-xs text-gray-500">Próx. 7 dias</p>
                <p className="text-lg font-bold text-blue-600">{data?.vencendo7Dias.quantidade || 0}</p>
                <p className="text-sm text-blue-600">{formatCurrency(data?.vencendo7Dias.valor || 0)}</p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-l-4 border-l-purple-500">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <AlertCircle className="h-8 w-8 text-purple-500" />
              <div>
                <p className="text-xs text-gray-500">Próx. 30 dias</p>
                <p className="text-lg font-bold text-purple-600">{data?.vencendo30Dias.quantidade || 0}</p>
                <p className="text-sm text-purple-600">{formatCurrency(data?.vencendo30Dias.valor || 0)}</p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-l-4 border-l-green-500">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <CheckCircle className="h-8 w-8 text-green-500" />
              <div>
                <p className="text-xs text-gray-500">A Vencer</p>
                <p className="text-lg font-bold text-green-600">{data?.aVencer.quantidade || 0}</p>
                <p className="text-sm text-green-600">{formatCurrency(data?.aVencer.valor || 0)}</p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Resumo Total */}
      <Card className="bg-gradient-to-r from-blue-500 to-blue-600 text-white">
        <CardContent className="p-6">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between">
            <div>
              <p className="text-blue-100">Total em Aberto</p>
              <p className="text-3xl font-bold">{formatCurrency(totalValor)}</p>
            </div>
            <div className="mt-4 md:mt-0 text-right">
              <p className="text-blue-100">Quantidade</p>
              <p className="text-3xl font-bold">{totalQuantidade}</p>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Gráficos */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Pizza */}
        <Card>
          <CardHeader>
            <CardTitle>Distribuição por Status</CardTitle>
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
                    {pieData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={entry.color} />
                    ))}
                  </Pie>
                  <Tooltip formatter={(value) => formatCurrency(Number(value) * 100)} />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>

        {/* Barras */}
        <Card>
          <CardHeader>
            <CardTitle>Quantidade e Valor por Status</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-80">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={barData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="name" />
                  <YAxis yAxisId="left" orientation="left" stroke="#3B82F6" />
                  <YAxis yAxisId="right" orientation="right" stroke="#10B981" tickFormatter={(v) => `R$${(v/1000).toFixed(0)}k`} />
                  <Tooltip formatter={(value, name) => 
                    name === 'valor' ? formatCurrency(Number(value) * 100) : value
                  } />
                  <Legend />
                  <Bar yAxisId="left" dataKey="quantidade" name="Quantidade" fill="#3B82F6" radius={[4, 4, 0, 0]} />
                  <Bar yAxisId="right" dataKey="valor" name="Valor" fill="#10B981" radius={[4, 4, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Tabela de Itens */}
      <Card>
        <CardHeader>
          <CardTitle>Contas Pendentes</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Descrição</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Fornecedor/Cliente</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Tipo</th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Valor</th>
                  <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Vencimento</th>
                  <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {data?.items.map((item) => (
                  <tr key={item.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className="font-medium text-gray-900">{item.description}</span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-gray-600">
                      {item.supplierCustomerName}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-2 py-1 text-xs font-medium rounded-full ${
                        item.type === 'Pagar' 
                          ? 'bg-red-100 text-red-800' 
                          : 'bg-green-100 text-green-800'
                      }`}>
                        {item.type === 'Pagar' ? 'A Pagar' : 'A Receber'}
                      </span>
                    </td>
                    <td className={`px-6 py-4 whitespace-nowrap text-right font-bold ${
                      item.type === 'Pagar' ? 'text-red-600' : 'text-green-600'
                    }`}>
                      {formatCurrency(item.amount)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-center text-gray-600">
                      {formatDate(item.dueDate)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-center">
                      {getStatusBadge(item.daysOverdue)}
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
