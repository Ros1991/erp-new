import api from './api';

export interface Account {
  accountId: number;
  companyId: number;
  name: string;
  type: string;
  initialBalance: number;
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface AccountFilters {
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
  totalCount: number;
  totalPages: number;
}

export interface AccountInput {
  name: string;
  type: string;
  initialBalance: number;
}

const accountService = {
  async getAccounts(filters: AccountFilters): Promise<PagedResult<Account>> {
    const params = new URLSearchParams();
    if (filters.search) params.append('SearchTerm', filters.search);
    if (filters.page) params.append('Page', filters.page.toString());
    if (filters.pageSize) params.append('PageSize', filters.pageSize.toString());
    if (filters.orderBy) params.append('OrderBy', filters.orderBy);
    if (filters.isAscending !== undefined) params.append('IsAscending', filters.isAscending.toString());

    const response = await api.get(`/account/getPaged?${params.toString()}`);
    return response.data.data;
  },

  async getAccountById(id: number): Promise<Account> {
    const response = await api.get(`/account/${id}`);
    return response.data.data;
  },

  async createAccount(data: AccountInput): Promise<Account> {
    const response = await api.post('/account/create', data);
    return response.data.data;
  },

  async updateAccount(id: number, data: AccountInput): Promise<Account> {
    const response = await api.put(`/account/${id}`, data);
    return response.data.data;
  },

  async deleteAccount(id: number): Promise<void> {
    await api.delete(`/account/${id}`);
  },
};

export default accountService;
