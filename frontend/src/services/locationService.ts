import api from './api';

export interface Location {
  locationId: number;
  companyId: number;
  name: string;
  address?: string;
  latitude: number;
  longitude: number;
  radiusMeters: number;
  isActive: boolean;
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface LocationFilters {
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

class LocationService {
  async getLocations(filters?: LocationFilters): Promise<PagedResult<Location>> {
    const params = new URLSearchParams();
    
    if (filters?.search) params.append('Search', filters.search);
    if (filters?.page) params.append('Page', filters.page.toString());
    if (filters?.pageSize) params.append('PageSize', filters.pageSize.toString());
    if (filters?.orderBy) params.append('OrderBy', filters.orderBy);
    if (filters?.isAscending !== undefined) params.append('IsAscending', filters.isAscending.toString());

    const response = await api.get(`/location/getPaged?${params.toString()}`);
    return response.data.data;
  }

  async getAllLocations(): Promise<Location[]> {
    const response = await api.get('/location/getAll');
    return response.data.data;
  }

  async getLocationById(id: number): Promise<Location> {
    const response = await api.get(`/location/${id}`);
    return response.data.data;
  }

  async createLocation(data: {
    name: string;
    address?: string;
    latitude: number;
    longitude: number;
    radiusMeters: number;
    isActive: boolean;
  }): Promise<Location> {
    const response = await api.post('/location/create', data);
    return response.data.data;
  }

  async updateLocation(id: number, data: {
    name: string;
    address?: string;
    latitude: number;
    longitude: number;
    radiusMeters: number;
    isActive: boolean;
  }): Promise<Location> {
    const response = await api.put(`/location/${id}`, data);
    return response.data.data;
  }

  async deleteLocation(id: number): Promise<void> {
    await api.delete(`/location/${id}`);
  }
}

export default new LocationService();
