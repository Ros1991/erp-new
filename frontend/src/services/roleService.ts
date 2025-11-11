import api from './api';

export interface Role {
  roleId: number;
  companyId: number;
  name: string;
  permissions: string; // JSON string
  isSystem: boolean;
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface RoleFilters {
  name?: string;
  isSystem?: boolean;
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
  totalPages: number;
}

class RoleService {
  async getRoles(filters?: RoleFilters): Promise<PagedResult<Role>> {
    const params = new URLSearchParams();
    
    if (filters?.name) params.append('name', filters.name);
    if (filters?.isSystem !== undefined) params.append('isSystem', filters.isSystem.toString());
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());
    if (filters?.orderBy) params.append('orderBy', filters.orderBy);
    if (filters?.orderDirection) params.append('orderDirection', filters.orderDirection);

    const response = await api.get(`/role/getPaged?${params.toString()}`);
    return response.data.data;
  }

  async getRoleById(id: number): Promise<Role> {
    const response = await api.get(`/role/${id}`);
    return response.data.data;
  }

  async createRole(data: { name: string; permissions: string }): Promise<Role> {
    const response = await api.post('/role/create', data);
    return response.data.data;
  }

  async updateRole(id: number, data: { name: string; permissions: string }): Promise<Role> {
    const response = await api.put(`/role/${id}`, data);
    return response.data.data;
  }

  async deleteRole(id: number): Promise<void> {
    await api.delete(`/role/${id}`);
  }
}

export default new RoleService();
