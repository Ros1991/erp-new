import api from './api';

export interface User {
  userId: number;
  email?: string;
  phone?: string;
  cpf?: string;
}

export interface UserFilters {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
}

export interface PagedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
}

const userService = {
  // Buscar usuários com paginação
  async getPaged(filters: UserFilters = {}): Promise<PagedResponse<User>> {
    const params = new URLSearchParams();
    if (filters.page) params.append('page', filters.page.toString());
    if (filters.pageSize) params.append('pageSize', filters.pageSize.toString());
    if (filters.searchTerm) params.append('searchTerm', filters.searchTerm);

    const response = await api.get(`/user/getPaged?${params.toString()}`);
    return response.data.data;
  },

  // Criar novo usuário (sem vincular à empresa ainda)
  async create(data: { email?: string; phone?: string; cpf?: string; password: string }): Promise<User> {
    const response = await api.post('/user/create', {
      email: data.email,
      phone: data.phone,
      cpf: data.cpf,
      password: data.password // Backend espera password
    });
    return response.data.data;
  },

  // Buscar usuário por ID
  async getById(userId: number): Promise<User> {
    const response = await api.get(`/user/${userId}`);
    return response.data.data;
  }
};

export default userService;
