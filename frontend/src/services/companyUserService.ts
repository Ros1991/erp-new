import api from './api';

export interface CompanyUser {
  companyUserId: number;
  companyId: number;
  userId: number;
  roleId?: number;
  userEmail?: string;
  userPhone?: string;
  userCpf?: string;
  roleName?: string;
  roleIsSystem?: boolean;
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface CompanyUserFilters {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

class CompanyUserService {
  /**
   * Obter todos os usuários da empresa
   */
  async getAll(): Promise<CompanyUser[]> {
    const response = await api.get('/companyuser/getAll');
    return response.data.data;
  }

  /**
   * Obter usuários paginados com filtros
   */
  async getPaged(filters: CompanyUserFilters = {}): Promise<PagedResult<CompanyUser>> {
    const params = new URLSearchParams();
    
    if (filters.page) params.append('page', filters.page.toString());
    if (filters.pageSize) params.append('pageSize', filters.pageSize.toString());
    if (filters.searchTerm) params.append('searchTerm', filters.searchTerm);
    if (filters.sortBy) params.append('sortBy', filters.sortBy);
    if (filters.sortDirection) params.append('sortDirection', filters.sortDirection);

    const response = await api.get(`/companyuser/getPaged?${params.toString()}`);
    return response.data.data;
  }

  /**
   * Obter um usuário por ID (companyUserId)
   */
  async getById(id: number): Promise<CompanyUser> {
    const response = await api.get(`/companyuser/${id}`);
    return response.data.data;
  }

  /**
   * Adicionar usuário à empresa
   */
  async create(data: Partial<CompanyUser>): Promise<CompanyUser> {
    const response = await api.post('/companyuser/create', data);
    return response.data.data;
  }

  /**
   * Atualizar cargo do usuário na empresa
   */
  async update(id: number, data: Partial<CompanyUser>): Promise<CompanyUser> {
    const response = await api.put(`/companyuser/${id}`, data);
    return response.data.data;
  }

  /**
   * Remover usuário da empresa
   */
  async delete(id: number): Promise<boolean> {
    const response = await api.delete(`/companyuser/${id}`);
    return response.data.data;
  }
}

export default new CompanyUserService();
