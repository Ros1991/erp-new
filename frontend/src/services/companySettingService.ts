import api from './api';

export interface DefaultCostCenterDistribution {
  defaultCostCenterDistributionId?: number;
  costCenterId: number;
  costCenterName?: string;
  percentage: number;
}

export interface CompanySetting {
  companySettingId?: number;
  companyId?: number;
  employeeIdGeneralManager?: number;
  generalManagerName?: string;
  timeToleranceMinutes: number;
  payrollDay: number;
  payrollClosingDay: number;
  vacationDaysPerYear: number;
  minHoursForLunchBreak: number;
  maxOvertimeHoursPerMonth: number;
  allowWeekendWork: boolean;
  requireJustificationAfterHours: number;
  weeklyHoursDefault: number;
  defaultCostCenterDistributions?: DefaultCostCenterDistribution[];
  criadoPor?: number;
  atualizadoPor?: number;
  criadoEm?: string;
  atualizadoEm?: string;
}

export interface CompanySettingInput {
  employeeIdGeneralManager?: number;
  timeToleranceMinutes: number;
  payrollDay: number;
  payrollClosingDay: number;
  vacationDaysPerYear: number;
  minHoursForLunchBreak: number;
  maxOvertimeHoursPerMonth: number;
  allowWeekendWork: boolean;
  requireJustificationAfterHours: number;
  weeklyHoursDefault: number;
  defaultCostCenterDistributions?: { costCenterId: number; percentage: number }[];
}

const companySettingService = {
  async getSettings(): Promise<CompanySetting> {
    const response = await api.get('/company-setting');
    return response.data.data;
  },

  async saveSettings(data: CompanySettingInput): Promise<CompanySetting> {
    const response = await api.put('/company-setting', data);
    return response.data.data;
  },

  async getDefaultDistributions(): Promise<DefaultCostCenterDistribution[]> {
    const response = await api.get('/company-setting/default-distributions');
    return response.data.data;
  }
};

export default companySettingService;
