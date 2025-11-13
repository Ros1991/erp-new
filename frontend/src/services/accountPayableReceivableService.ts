import api from './api';

export interface AccountPayableReceivable {
  accountPayableReceivableId: number;
  companyId: number;
  supplierCustomerId?: number;
  description: string;
  type: string;
  amount: number;
  dueDate: string;
  paymentDate?: string;
  isPaid: boolean;
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface AccountPayableReceivableFilters {
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

class AccountPayableReceivableService {
  async getAccountPayableReceivables(filters?: AccountPayableReceivableFilters): Promise<PagedResult<AccountPayableReceivable>> {
    const params = new URLSearchParams();
    
    if (filters?.search) params.append('Search', filters.search);
    if (filters?.page) params.append('Page', filters.page.toString());
    if (filters?.pageSize) params.append('PageSize', filters.pageSize.toString());
    if (filters?.orderBy) params.append('OrderBy', filters.orderBy);
    if (filters?.isAscending !== undefined) params.append('IsAscending', filters.isAscending.toString());

    const response = await api.get(`/account-payable-receivable/getPaged?${params.toString()}`);
    return response.data.data;
  }

  async getAllAccountPayableReceivables(): Promise<AccountPayableReceivable[]> {
    const response = await api.get('/account-payable-receivable/getAll');
    return response.data.data;
  }

  async getAccountPayableReceivableById(id: number): Promise<AccountPayableReceivable> {
    const response = await api.get(`/account-payable-receivable/${id}`);
    return response.data.data;
  }

  async createAccountPayableReceivable(data: {
    supplierCustomerId?: number;
    description: string;
    type: string;
    amount: number;
    dueDate: string;
    paymentDate?: string;
    isPaid: boolean;
  }): Promise<AccountPayableReceivable> {
    const response = await api.post('/account-payable-receivable/create', data);
    return response.data.data;
  }

  async updateAccountPayableReceivable(id: number, data: {
    supplierCustomerId?: number;
    description: string;
    type: string;
    amount: number;
    dueDate: string;
    paymentDate?: string;
    isPaid: boolean;
  }): Promise<AccountPayableReceivable> {
    const response = await api.put(`/account-payable-receivable/${id}`, data);
    return response.data.data;
  }

  async deleteAccountPayableReceivable(id: number): Promise<void> {
    await api.delete(`/account-payable-receivable/${id}`);
  }
}

export default new AccountPayableReceivableService();
