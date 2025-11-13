import api from './api';

export interface CostCenter {
  costCenterId: number;
  companyId: number;
  name: string;
  description?: string;
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface CostCenterFilters {
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

class CostCenterService {
  async getCostCenters(filters?: CostCenterFilters): Promise<PagedResult<CostCenter>> {
    const params = new URLSearchParams();
    
    if (filters?.search) params.append('Search', filters.search);
    if (filters?.page) params.append('Page', filters.page.toString());
    if (filters?.pageSize) params.append('PageSize', filters.pageSize.toString());
    if (filters?.orderBy) params.append('OrderBy', filters.orderBy);
    if (filters?.isAscending !== undefined) params.append('IsAscending', filters.isAscending.toString());

    const response = await api.get(`/cost-center/getPaged?${params.toString()}`);
    return response.data.data;
  }

  async getAllCostCenters(): Promise<CostCenter[]> {
    const response = await api.get('/cost-center/getAll');
    return response.data.data;
  }

  async getCostCenterById(id: number): Promise<CostCenter> {
    const response = await api.get(`/cost-center/${id}`);
    return response.data.data;
  }

  async createCostCenter(data: {
    name: string;
    description?: string;
  }): Promise<CostCenter> {
    const response = await api.post('/cost-center/create', data);
    return response.data.data;
  }

  async updateCostCenter(id: number, data: {
    name: string;
    description?: string;
  }): Promise<CostCenter> {
    const response = await api.put(`/cost-center/${id}`, data);
    return response.data.data;
  }

  async deleteCostCenter(id: number): Promise<void> {
    await api.delete(`/cost-center/${id}`);
  }
}

export default new CostCenterService();
