import api from './api';

export interface Payroll {
  payrollId: number;
  companyId: number;
  periodStartDate: string;
  periodEndDate: string;
  totalGrossPay: number;
  totalDeductions: number;
  totalNetPay: number;
  isClosed: boolean;
  closedAt?: string;
  closedBy?: number;
  closedByName?: string;
  notes?: string;
  snapshot?: string;
  employeeCount: number;
  isLastPayroll: boolean;
  criadoPor: number;
  criadoPorNome: string;
  atualizadoPor?: number;
  atualizadoPorNome?: string;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface PayrollFilters {
  periodStartDate?: string;
  periodEndDate?: string;
  isClosed?: boolean;
  search?: string;
  page?: number;
  pageSize?: number;
  orderBy?: string;
  orderDirection?: 'asc' | 'desc';
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  totalCount: number;
  totalPages: number;
}

class PayrollService {
  async getAllPayrolls(): Promise<Payroll[]> {
    const response = await api.get('/payroll/getAll');
    return response.data.data;
  }

  async getPayrolls(filters?: PayrollFilters): Promise<PagedResult<Payroll>> {
    const params = new URLSearchParams();
    
    if (filters?.periodStartDate) params.append('periodStartDate', filters.periodStartDate);
    if (filters?.periodEndDate) params.append('periodEndDate', filters.periodEndDate);
    if (filters?.isClosed !== undefined) params.append('isClosed', filters.isClosed.toString());
    if (filters?.search) params.append('search', filters.search);
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());
    if (filters?.orderBy) params.append('orderBy', filters.orderBy);
    if (filters?.orderDirection) params.append('orderDirection', filters.orderDirection);

    const response = await api.get(`/payroll/getPaged?${params.toString()}`);
    return response.data.data;
  }

  async getPayrollById(id: number): Promise<Payroll> {
    const response = await api.get(`/payroll/${id}`);
    return response.data.data;
  }

  async createPayroll(data: { 
    periodStartDate: string; 
    periodEndDate: string; 
    notes?: string;
  }): Promise<Payroll> {
    const response = await api.post('/payroll/create', data);
    return response.data.data;
  }

  async updatePayroll(id: number, data: { 
    periodStartDate: string; 
    periodEndDate: string; 
    notes?: string;
  }): Promise<Payroll> {
    const response = await api.put(`/payroll/${id}`, data);
    return response.data.data;
  }

  async deletePayroll(id: number): Promise<void> {
    await api.delete(`/payroll/${id}`);
  }
}

export default new PayrollService();
