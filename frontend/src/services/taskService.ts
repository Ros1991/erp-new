import api from './api';

export interface Task {
  taskId: number;
  companyId: number;
  taskIdParent?: number;
  taskIdBlocking?: number;
  title: string;
  description?: string;
  priority: string;
  frequencyDays?: number;
  allowSunday: boolean;
  allowMonday: boolean;
  allowTuesday: boolean;
  allowWednesday: boolean;
  allowThursday: boolean;
  allowFriday: boolean;
  allowSaturday: boolean;
  startDate?: string;
  endDate?: string;
  overallStatus: string;
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface TaskFilters {
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

class TaskService {
  async getTasks(filters?: TaskFilters): Promise<PagedResult<Task>> {
    const params = new URLSearchParams();
    
    if (filters?.search) params.append('Search', filters.search);
    if (filters?.page) params.append('Page', filters.page.toString());
    if (filters?.pageSize) params.append('PageSize', filters.pageSize.toString());
    if (filters?.orderBy) params.append('OrderBy', filters.orderBy);
    if (filters?.isAscending !== undefined) params.append('IsAscending', filters.isAscending.toString());

    const response = await api.get(`/task/getPaged?${params.toString()}`);
    return response.data.data;
  }

  async getAllTasks(): Promise<Task[]> {
    const response = await api.get('/task/getAll');
    return response.data.data;
  }

  async getTaskById(id: number): Promise<Task> {
    const response = await api.get(`/task/${id}`);
    return response.data.data;
  }

  async createTask(data: {
    taskIdParent?: number;
    taskIdBlocking?: number;
    title: string;
    description?: string;
    priority: string;
    frequencyDays?: number;
    allowSunday: boolean;
    allowMonday: boolean;
    allowTuesday: boolean;
    allowWednesday: boolean;
    allowThursday: boolean;
    allowFriday: boolean;
    allowSaturday: boolean;
    startDate?: string;
    endDate?: string;
    overallStatus: string;
  }): Promise<Task> {
    const response = await api.post('/task/create', data);
    return response.data.data;
  }

  async updateTask(id: number, data: {
    taskIdParent?: number;
    taskIdBlocking?: number;
    title: string;
    description?: string;
    priority: string;
    frequencyDays?: number;
    allowSunday: boolean;
    allowMonday: boolean;
    allowTuesday: boolean;
    allowWednesday: boolean;
    allowThursday: boolean;
    allowFriday: boolean;
    allowSaturday: boolean;
    startDate?: string;
    endDate?: string;
    overallStatus: string;
  }): Promise<Task> {
    const response = await api.put(`/task/${id}`, data);
    return response.data.data;
  }

  async deleteTask(id: number): Promise<void> {
    await api.delete(`/task/${id}`);
  }
}

export default new TaskService();
