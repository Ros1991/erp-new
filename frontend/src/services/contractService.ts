import api from './api';

export interface ContractBenefitDiscount {
  contractBenefitDiscountId?: number;
  description: string;
  type: string;
  application: string;
  amount: number;
}

export interface ContractCostCenter {
  contractCostCenterId?: number;
  costCenterId: number;
  costCenterName?: string;
  percentage: number;
}

export interface Contract {
  contractId: number;
  employeeId: number;
  employeeName?: string;
  type: string;
  value: number;
  isPayroll: boolean;
  hasInss: boolean;
  hasIrrf: boolean;
  hasFgts: boolean;
  startDate: string;
  endDate?: string;
  weeklyHours?: number;
  isActive: boolean;
  benefitsDiscounts?: ContractBenefitDiscount[];
  costCenters?: ContractCostCenter[];
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface ContractInput {
  employeeId: number;
  type: string;
  value: number;
  isPayroll: boolean;
  hasInss: boolean;
  hasIrrf: boolean;
  hasFgts: boolean;
  startDate: string;
  endDate?: string;
  weeklyHours?: number;
  isActive: boolean;
  benefitsDiscounts?: ContractBenefitDiscount[];
  costCenters?: ContractCostCenter[];
}

const contractService = {
  getAllByEmployeeId: async (employeeId: number): Promise<Contract[]> => {
    const response = await api.get(`/contract/employee/${employeeId}/all`);
    return response.data.data;
  },

  getActiveByEmployeeId: async (employeeId: number): Promise<Contract | null> => {
    try {
      const response = await api.get(`/contract/employee/${employeeId}/active`);
      const contract = response.data.data;
      // Se a resposta for null ou não tiver dados válidos, retorna null
      if (!contract || !contract.contractId) {
        return null;
      }
      return contract;
    } catch (error: any) {
      // Se retornar 404 ou qualquer erro, não tem contrato ativo
      if (error.response?.status === 404) {
        return null;
      }
      throw error;
    }
  },

  getContractById: async (id: number): Promise<Contract> => {
    const response = await api.get(`/contract/${id}`);
    return response.data.data;
  },

  createContract: async (contract: ContractInput): Promise<Contract> => {
    const response = await api.post('/contract', contract);
    return response.data.data;
  },

  updateContract: async (id: number, contract: ContractInput): Promise<Contract> => {
    const response = await api.put(`/contract/${id}`, contract);
    return response.data.data;
  },

  deleteContract: async (id: number): Promise<void> => {
    await api.delete(`/contract/${id}`);
  },
};

export default contractService;
