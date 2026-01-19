import { useState, useEffect, useCallback } from 'react';
import { MainLayout } from '../../components/layout';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Calendar, RefreshCw, User, TrendingUp, TrendingDown, Wallet } from 'lucide-react';
import reportService from '../../services/reportService';
import employeeService from '../../services/employeeService';
import type { Employee } from '../../services/employeeService';
import type { EmployeeAccountReport as EmployeeAccountReportType, EmployeeAccountItem } from '../../services/reportService';

export function EmployeeAccountReport() {
  const [isLoading, setIsLoading] = useState(false);
  const [data, setData] = useState<EmployeeAccountReportType | null>(null);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [selectedEmployeeId, setSelectedEmployeeId] = useState<number | null>(null);
  
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

  const formatDate = (dateStr: string) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('pt-BR');
  };

  const loadEmployees = useCallback(async () => {
    try {
      const result = await employeeService.getEmployees({
        page: 1,
        pageSize: 1000,
        isAscending: true,
        orderBy: 'nickname'
      });
      setEmployees(result.items);
    } catch (error) {
      console.error('Erro ao carregar funcionários:', error);
    }
  }, []);

  const loadData = useCallback(async () => {
    if (!selectedEmployeeId) return;
    
    setIsLoading(true);
    try {
      const result = await reportService.getEmployeeAccountReport({
        employeeId: selectedEmployeeId,
        startDate,
        endDate
      });
      setData(result);
    } catch (error) {
      console.error('Erro ao carregar relatório:', error);
    } finally {
      setIsLoading(false);
    }
  }, [selectedEmployeeId, startDate, endDate]);

  useEffect(() => {
    loadEmployees();
  }, [loadEmployees]);

  useEffect(() => {
    if (selectedEmployeeId) {
      loadData();
    }
  }, [selectedEmployeeId, loadData]);

  const getValueColor = (item: EmployeeAccountItem) => {
    return item.value >= 0 ? 'text-green-600' : 'text-red-600';
  };

  const getBalanceColor = (balance: number) => {
    return balance >= 0 ? 'text-green-600' : 'text-red-600';
  };

  const formatValue = (value: number) => {
    return formatCurrency(Math.abs(value));
  };

  return (
    <MainLayout>
      <div className="space-y-6">
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Conta Corrente do Funcionário</h1>
            <p className="text-gray-500 mt-1">Extrato de empréstimos e folhas de pagamento</p>
          </div>
          <Button onClick={loadData} disabled={isLoading || !selectedEmployeeId}>
            <RefreshCw className={`h-4 w-4 mr-2 ${isLoading ? 'animate-spin' : ''}`} />
            Atualizar
          </Button>
        </div>

        {/* Filtros */}
        <Card>
          <CardContent className="p-4">
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  <User className="h-4 w-4 inline mr-1" />
                  Funcionário
                </label>
                <select
                  className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                  value={selectedEmployeeId || ''}
                  onChange={(e) => setSelectedEmployeeId(e.target.value ? Number(e.target.value) : null)}
                >
                  <option value="">Selecione um funcionário</option>
                  {employees.map((emp) => (
                    <option key={emp.employeeId} value={emp.employeeId}>
                      {emp.nickname || emp.fullName}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  <Calendar className="h-4 w-4 inline mr-1" />
                  Data Inicial
                </label>
                <Input
                  type="date"
                  value={startDate}
                  onChange={(e) => setStartDate(e.target.value)}
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  <Calendar className="h-4 w-4 inline mr-1" />
                  Data Final
                </label>
                <Input
                  type="date"
                  value={endDate}
                  onChange={(e) => setEndDate(e.target.value)}
                />
              </div>
              <div className="flex items-end">
                <Button onClick={loadData} disabled={isLoading || !selectedEmployeeId} className="w-full">
                  Buscar
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>

        {!selectedEmployeeId && (
          <Card>
            <CardContent className="p-8 text-center">
              <User className="h-12 w-12 mx-auto text-gray-400 mb-4" />
              <p className="text-gray-500">Selecione um funcionário para visualizar o extrato</p>
            </CardContent>
          </Card>
        )}

        {data && data.employeeId > 0 && (
          <>
            {/* Resumo */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <Card>
                <CardContent className="p-4">
                  <div className="flex items-center gap-3">
                    <div className="p-2 bg-blue-100 rounded-lg">
                      <User className="h-5 w-5 text-blue-600" />
                    </div>
                    <div>
                      <p className="text-sm text-gray-500">Funcionário</p>
                      <p className="text-lg font-semibold">{data.employeeNickname || data.employeeName}</p>
                    </div>
                  </div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-4">
                  <div className="flex items-center gap-3">
                    <div className="p-2 bg-gray-100 rounded-lg">
                      <Wallet className="h-5 w-5 text-gray-600" />
                    </div>
                    <div>
                      <p className="text-sm text-gray-500">Movimentações</p>
                      <p className="text-lg font-semibold">{data.items.length}</p>
                    </div>
                  </div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-4">
                  <div className="flex items-center gap-3">
                    <div className={`p-2 rounded-lg ${data.saldoFinal >= 0 ? 'bg-green-100' : 'bg-red-100'}`}>
                      {data.saldoFinal >= 0 ? (
                        <TrendingUp className="h-5 w-5 text-green-600" />
                      ) : (
                        <TrendingDown className="h-5 w-5 text-red-600" />
                      )}
                    </div>
                    <div>
                      <p className="text-sm text-gray-500">Saldo Final</p>
                      <p className={`text-lg font-semibold ${getBalanceColor(data.saldoFinal)}`}>
                        {formatCurrency(data.saldoFinal)}
                      </p>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Tabela de Extrato */}
            <Card>
              <CardHeader>
                <CardTitle>Extrato</CardTitle>
              </CardHeader>
              <CardContent>
                {isLoading ? (
                  <div className="flex justify-center items-center py-8">
                    <RefreshCw className="h-8 w-8 animate-spin text-gray-400" />
                  </div>
                ) : data.items.length === 0 ? (
                  <div className="text-center py-8 text-gray-500">
                    Nenhuma movimentação encontrada no período
                  </div>
                ) : (
                  <div className="overflow-x-auto">
                    <table className="w-full">
                      <thead>
                        <tr className="bg-yellow-400">
                          <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900">Data</th>
                          <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900">Descrição</th>
                          <th className="px-4 py-3 text-right text-sm font-semibold text-gray-900">Valor</th>
                          <th className="px-4 py-3 text-right text-sm font-semibold text-gray-900">Saldo</th>
                        </tr>
                      </thead>
                      <tbody>
                        {data.items.map((item, index) => (
                          <tr 
                            key={index} 
                            className={index % 2 === 0 ? 'bg-white' : 'bg-gray-50'}
                          >
                            <td className="px-4 py-3 text-sm text-gray-900">
                              {formatDate(item.date)}
                            </td>
                            <td className="px-4 py-3 text-sm text-gray-900">
                              {item.description}
                            </td>
                            <td className={`px-4 py-3 text-sm text-right font-medium ${getValueColor(item)}`}>
                              {formatValue(item.value)}
                            </td>
                            <td className={`px-4 py-3 text-sm text-right font-medium ${getBalanceColor(item.balance)}`}>
                              {formatCurrency(item.balance)}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                      <tfoot>
                        <tr className="bg-gray-100 font-semibold">
                          <td colSpan={2} className="px-4 py-3 text-sm text-gray-900">
                            Saldo Final
                          </td>
                          <td className="px-4 py-3 text-sm text-right"></td>
                          <td className={`px-4 py-3 text-sm text-right ${getBalanceColor(data.saldoFinal)}`}>
                            {formatCurrency(data.saldoFinal)}
                          </td>
                        </tr>
                      </tfoot>
                    </table>
                  </div>
                )}
              </CardContent>
            </Card>
          </>
        )}
      </div>
    </MainLayout>
  );
}
