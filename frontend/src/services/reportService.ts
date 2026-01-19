import api from './api';

export interface FinancialSummary {
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
  receitasPorMes: { month: string; value: number }[];
  despesasPorMes: { month: string; value: number }[];
}

export interface CostCenterReport {
  costCenterId: number;
  costCenterName: string;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
  percentualDespesas: number;
}

export interface AccountReport {
  accountId: number;
  accountName: string;
  accountType: string;
  saldoInicial: number;
  totalEntradas: number;
  totalSaidas: number;
  saldoFinal: number;
}

export interface SupplierCustomerReport {
  supplierCustomerId: number;
  supplierCustomerName: string;
  totalPago: number;
  totalRecebido: number;
  quantidadeTransacoes: number;
}

export interface CashFlowItem {
  date: string;
  entradas: number;
  saidas: number;
  saldo: number;
  saldoAcumulado: number;
}

export interface AccountPayableReceivableReport {
  vencidas: { quantidade: number; valor: number };
  vencendoHoje: { quantidade: number; valor: number };
  vencendo7Dias: { quantidade: number; valor: number };
  vencendo30Dias: { quantidade: number; valor: number };
  aVencer: { quantidade: number; valor: number };
  items: {
    id: number;
    description: string;
    supplierCustomerName: string;
    amount: number;
    dueDate: string;
    type: string;
    isPaid: boolean;
    daysOverdue: number;
  }[];
}

export interface FinancialForecast {
  saldoAtual: number;
  totalAPagar: number;
  totalAReceber: number;
  saldoProjetado: number;
  meses: {
    month: string;
    year: number;
    aPagar: number;
    aReceber: number;
    saldo: number;
    saldoAcumulado: number;
  }[];
  contasAPagar: {
    id: number;
    description: string;
    supplierCustomerName: string;
    amount: number;
    dueDate: string;
    type: string;
  }[];
  contasAReceber: {
    id: number;
    description: string;
    supplierCustomerName: string;
    amount: number;
    dueDate: string;
    type: string;
  }[];
}

export interface ReportFilters {
  startDate?: string;
  endDate?: string;
  costCenterId?: number;
  accountId?: number;
  supplierCustomerId?: number;
  type?: 'Pagar' | 'Receber' | 'Todos';
}

class ReportService {
  async getFinancialSummary(filters: ReportFilters): Promise<FinancialSummary> {
    const params = new URLSearchParams();
    if (filters.startDate) params.append('StartDate', filters.startDate);
    if (filters.endDate) params.append('EndDate', filters.endDate);
    
    const response = await api.get(`/report/financial-summary?${params.toString()}`);
    return response.data.data;
  }

  async getCostCenterReport(filters: ReportFilters): Promise<CostCenterReport[]> {
    const params = new URLSearchParams();
    if (filters.startDate) params.append('StartDate', filters.startDate);
    if (filters.endDate) params.append('EndDate', filters.endDate);
    
    const response = await api.get(`/report/cost-center?${params.toString()}`);
    return response.data.data;
  }

  async getAccountReport(filters: ReportFilters): Promise<AccountReport[]> {
    const params = new URLSearchParams();
    if (filters.startDate) params.append('StartDate', filters.startDate);
    if (filters.endDate) params.append('EndDate', filters.endDate);
    
    const response = await api.get(`/report/account?${params.toString()}`);
    return response.data.data;
  }

  async getSupplierCustomerReport(filters: ReportFilters): Promise<SupplierCustomerReport[]> {
    const params = new URLSearchParams();
    if (filters.startDate) params.append('StartDate', filters.startDate);
    if (filters.endDate) params.append('EndDate', filters.endDate);
    if (filters.type && filters.type !== 'Todos') params.append('Type', filters.type);
    
    const response = await api.get(`/report/supplier-customer?${params.toString()}`);
    return response.data.data;
  }

  async getCashFlow(filters: ReportFilters): Promise<CashFlowItem[]> {
    const params = new URLSearchParams();
    if (filters.startDate) params.append('StartDate', filters.startDate);
    if (filters.endDate) params.append('EndDate', filters.endDate);
    if (filters.accountId) params.append('AccountId', filters.accountId.toString());
    
    const response = await api.get(`/report/cash-flow?${params.toString()}`);
    return response.data.data;
  }

  async getAccountPayableReceivableReport(filters: ReportFilters): Promise<AccountPayableReceivableReport> {
    const params = new URLSearchParams();
    if (filters.type && filters.type !== 'Todos') params.append('Type', filters.type);
    
    const response = await api.get(`/report/accounts-payable-receivable?${params.toString()}`);
    return response.data.data;
  }

  async getFinancialForecast(months: number = 6): Promise<FinancialForecast> {
    const response = await api.get(`/report/financial-forecast?months=${months}`);
    return response.data.data;
  }
}

export default new ReportService();
