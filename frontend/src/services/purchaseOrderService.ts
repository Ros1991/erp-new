import api from './api';

export interface PurchaseOrder {
  purchaseOrderId: number;
  companyId: number;
  userIdRequester: number;
  requesterName?: string;
  userIdApprover?: number;
  approverName?: string;
  description: string;
  totalAmount: number;
  status: string;
  processedMessage?: string;
  processedAt?: string;
  accountId?: number;
  accountName?: string;
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface CostCenterDistribution {
  costCenterId: number;
  percentage: number;
}

export interface ProcessPurchaseOrderInput {
  status: 'Aprovado' | 'Rejeitado';
  processedMessage?: string;
  processedAt: string;
  transactionDescription?: string;
  accountId?: number;
  costCenterDistributions?: CostCenterDistribution[];
}

export interface PurchaseOrderFilters {
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

class PurchaseOrderService {
  async getPurchaseOrders(filters?: PurchaseOrderFilters): Promise<PagedResult<PurchaseOrder>> {
    const params = new URLSearchParams();
    
    if (filters?.search) params.append('Search', filters.search);
    if (filters?.page) params.append('Page', filters.page.toString());
    if (filters?.pageSize) params.append('PageSize', filters.pageSize.toString());
    if (filters?.orderBy) params.append('OrderBy', filters.orderBy);
    if (filters?.isAscending !== undefined) params.append('IsAscending', filters.isAscending.toString());

    const response = await api.get(`/purchase-order/getPaged?${params.toString()}`);
    return response.data.data;
  }

  async getAllPurchaseOrders(): Promise<PurchaseOrder[]> {
    const response = await api.get('/purchase-order/getAll');
    return response.data.data;
  }

  async getPurchaseOrderById(id: number): Promise<PurchaseOrder> {
    const response = await api.get(`/purchase-order/${id}`);
    return response.data.data;
  }

  async createPurchaseOrder(data: {
    description: string;
  }): Promise<PurchaseOrder> {
    const response = await api.post('/purchase-order/create', data);
    return response.data.data;
  }

  async updatePurchaseOrder(id: number, data: {
    description: string;
  }): Promise<PurchaseOrder> {
    const response = await api.put(`/purchase-order/${id}`, data);
    return response.data.data;
  }

  async deletePurchaseOrder(id: number): Promise<void> {
    await api.delete(`/purchase-order/${id}`);
  }

  async processPurchaseOrder(id: number, data: ProcessPurchaseOrderInput): Promise<PurchaseOrder> {
    const response = await api.post(`/purchase-order/${id}/process`, data);
    return response.data.data;
  }
}

export default new PurchaseOrderService();
