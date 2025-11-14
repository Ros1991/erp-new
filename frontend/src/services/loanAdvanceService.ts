import api from './api';

export interface LoanAdvance {
  loanAdvanceId: number;
  employeeId: number;
  employeeName?: string;
  amount: number;
  installments: number;
  discountSource: string;
  startDate: string;
  isApproved: boolean;
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface LoanAdvanceFilters {
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

class LoanAdvanceService {
  async getLoanAdvances(filters?: LoanAdvanceFilters): Promise<PagedResult<LoanAdvance>> {
    const params = new URLSearchParams();
    
    if (filters?.search) params.append('Search', filters.search);
    if (filters?.page) params.append('Page', filters.page.toString());
    if (filters?.pageSize) params.append('PageSize', filters.pageSize.toString());
    if (filters?.orderBy) params.append('OrderBy', filters.orderBy);
    if (filters?.isAscending !== undefined) params.append('IsAscending', filters.isAscending.toString());

    const response = await api.get(`/loan-advance/getPaged?${params.toString()}`);
    return response.data.data;
  }

  async getAllLoanAdvances(): Promise<LoanAdvance[]> {
    const response = await api.get('/loan-advance/getAll');
    return response.data.data;
  }

  async getLoanAdvanceById(id: number): Promise<LoanAdvance> {
    const response = await api.get(`/loan-advance/${id}`);
    return response.data.data;
  }

  async createLoanAdvance(data: {
    employeeId: number;
    amount: number;
    installments: number;
    discountSource: string;
    startDate: string;
    isApproved: boolean;
    accountId: number;
    costCenterDistributions?: Array<{
      costCenterId: number;
      percentage: number;
      amount?: number;
    }>;
  }): Promise<LoanAdvance> {
    const response = await api.post('/loan-advance/create', data);
    return response.data.data;
  }

  async updateLoanAdvance(id: number, data: {
    employeeId: number;
    amount: number;
    installments: number;
    discountSource: string;
    startDate: string;
    isApproved: boolean;
    accountId: number;
    costCenterDistributions?: Array<{
      costCenterId: number;
      percentage: number;
      amount?: number;
    }>;
  }): Promise<LoanAdvance> {
    const response = await api.put(`/loan-advance/${id}`, data);
    return response.data.data;
  }

  async deleteLoanAdvance(id: number): Promise<void> {
    await api.delete(`/loan-advance/${id}`);
  }
}

export default new LoanAdvanceService();
