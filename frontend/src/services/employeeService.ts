import api from './api';

export interface Employee {
  employeeId: number;
  companyId: number;
  userId?: number;
  employeeIdManager?: number;
  nickname: string;
  fullName: string;
  email?: string;
  phone?: string;
  cpf?: string;
  profileImageBase64?: string;
  managerNickname?: string;
  managerFullName?: string;
  userEmail?: string;
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface EmployeeFilters {
  search?: string;
  nickname?: string;
  fullName?: string;
  email?: string;
  phone?: string;
  cpf?: string;
  employeeIdManager?: number;
  userId?: number;
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

class EmployeeService {
  async getEmployees(filters?: EmployeeFilters): Promise<PagedResult<Employee>> {
    const params = new URLSearchParams();
    
    if (filters?.search) params.append('Search', filters.search);
    if (filters?.nickname) params.append('Nickname', filters.nickname);
    if (filters?.fullName) params.append('FullName', filters.fullName);
    if (filters?.email) params.append('Email', filters.email);
    if (filters?.phone) params.append('Phone', filters.phone);
    if (filters?.cpf) params.append('Cpf', filters.cpf);
    if (filters?.employeeIdManager) params.append('EmployeeIdManager', filters.employeeIdManager.toString());
    if (filters?.userId) params.append('UserId', filters.userId.toString());
    if (filters?.page) params.append('Page', filters.page.toString());
    if (filters?.pageSize) params.append('PageSize', filters.pageSize.toString());
    if (filters?.orderBy) params.append('OrderBy', filters.orderBy);
    if (filters?.isAscending !== undefined) params.append('IsAscending', filters.isAscending.toString());

    const response = await api.get(`/employee/getPaged?${params.toString()}`);
    return response.data.data;
  }

  async getAllEmployees(): Promise<Employee[]> {
    const response = await api.get('/employee/getAll');
    return response.data.data;
  }

  async getEmployeeById(id: number): Promise<Employee> {
    const response = await api.get(`/employee/${id}`);
    return response.data.data;
  }

  async createEmployee(data: {
    userId?: number;
    employeeIdManager?: number;
    nickname: string;
    fullName: string;
    email?: string;
    phone?: string;
    cpf?: string;
    profileImageBase64?: string;
  }): Promise<Employee> {
    const response = await api.post('/employee/create', data);
    return response.data.data;
  }

  async updateEmployee(id: number, data: {
    userId?: number;
    employeeIdManager?: number;
    nickname: string;
    fullName: string;
    email?: string;
    phone?: string;
    cpf?: string;
    profileImageBase64?: string;
  }): Promise<Employee> {
    const response = await api.put(`/employee/${id}`, data);
    return response.data.data;
  }

  async deleteEmployee(id: number): Promise<void> {
    await api.delete(`/employee/${id}`);
  }

  // Helper para converter arquivo para Base64
  async imageToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        // Remove o prefixo "data:image/xxx;base64," e retorna apenas o Base64
        const base64 = reader.result as string;
        const base64Data = base64.split(',')[1];
        resolve(base64Data);
      };
      reader.onerror = error => reject(error);
    });
  }
}

export default new EmployeeService();
