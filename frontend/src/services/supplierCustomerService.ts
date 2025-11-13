import api from './api';

export interface SupplierCustomer {
  supplierCustomerId: number;
  companyId: number;
  name: string;
  document?: string;
  email?: string;
  phone?: string;
  address?: string;
  isActive: boolean;
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface SupplierCustomerFilters {
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

class SupplierCustomerService {
  async getSupplierCustomers(filters?: SupplierCustomerFilters): Promise<PagedResult<SupplierCustomer>> {
    const params = new URLSearchParams();
    
    if (filters?.search) params.append('Search', filters.search);
    if (filters?.page) params.append('Page', filters.page.toString());
    if (filters?.pageSize) params.append('PageSize', filters.pageSize.toString());
    if (filters?.orderBy) params.append('OrderBy', filters.orderBy);
    if (filters?.isAscending !== undefined) params.append('IsAscending', filters.isAscending.toString());

    const response = await api.get(`/supplier-customer/getPaged?${params.toString()}`);
    return response.data.data;
  }

  async getAllSupplierCustomers(): Promise<SupplierCustomer[]> {
    const response = await api.get('/supplier-customer/getAll');
    return response.data.data;
  }

  async getSupplierCustomerById(id: number): Promise<SupplierCustomer> {
    const response = await api.get(`/supplier-customer/${id}`);
    return response.data.data;
  }

  async createSupplierCustomer(data: {
    name: string;
    document?: string;
    email?: string;
    phone?: string;
    address?: string;
    isActive: boolean;
  }): Promise<SupplierCustomer> {
    const response = await api.post('/supplier-customer/create', data);
    return response.data.data;
  }

  async updateSupplierCustomer(id: number, data: {
    name: string;
    document?: string;
    email?: string;
    phone?: string;
    address?: string;
    isActive: boolean;
  }): Promise<SupplierCustomer> {
    const response = await api.put(`/supplier-customer/${id}`, data);
    return response.data.data;
  }

  async deleteSupplierCustomer(id: number): Promise<void> {
    await api.delete(`/supplier-customer/${id}`);
  }
}

export default new SupplierCustomerService();
