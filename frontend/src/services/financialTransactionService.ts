import api from './api';

export interface FinancialTransaction {
  financialTransactionId: number;
  companyId: number;
  accountId: number | null;
  accountName?: string;
  purchaseOrderId?: number;
  accountPayableReceivableId?: number;
  supplierCustomerId?: number;
  supplierCustomerName?: string;
  loanAdvanceId?: number;
  payrollId?: number;
  description: string;
  type: string;
  amount: number;
  transactionDate: string;
  costCenterDistributions?: CostCenterDistribution[];
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface FinancialTransactionFilters {
  search?: string;
  page?: number;
  pageSize?: number;
  orderBy?: string;
  isAscending?: boolean;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  totalCount: number;
  totalPages: number;
}

export interface CostCenterDistribution {
  costCenterId: number;
  costCenterName?: string;
  percentage: number;
  amount: number;
}

export interface FinancialTransactionInput {
  accountId: number | null;
  purchaseOrderId?: number;
  accountPayableReceivableId?: number;
  supplierCustomerId?: number;
  description: string;
  type: string;
  amount: number;
  transactionDate: string;
  costCenterDistributions?: CostCenterDistribution[];
}

const financialTransactionService = {
  async getFinancialTransactions(filters: FinancialTransactionFilters): Promise<PagedResult<FinancialTransaction>> {
    const params = new URLSearchParams();
    if (filters.search) params.append('Search', filters.search);
    if (filters.page) params.append('Page', filters.page.toString());
    if (filters.pageSize) params.append('PageSize', filters.pageSize.toString());
    if (filters.orderBy) params.append('OrderBy', filters.orderBy);
    if (filters.isAscending !== undefined) params.append('OrderDirection', filters.isAscending ? 'asc' : 'desc');

    const response = await api.get(`/financial-transaction/getPaged?${params.toString()}`);
    return response.data.data;
  },

  async getFinancialTransactionById(id: number): Promise<FinancialTransaction> {
    const response = await api.get(`/financial-transaction/${id}`);
    return response.data.data;
  },

  async createFinancialTransaction(data: FinancialTransactionInput): Promise<FinancialTransaction> {
    const response = await api.post('/financial-transaction/create', data);
    return response.data.data;
  },

  async updateFinancialTransaction(id: number, data: FinancialTransactionInput): Promise<FinancialTransaction> {
    const response = await api.put(`/financial-transaction/${id}`, data);
    return response.data.data;
  },

  async deleteFinancialTransaction(id: number): Promise<void> {
    await api.delete(`/financial-transaction/${id}`);
  },
};

export default financialTransactionService;
