import api from './api';

export interface Payroll {
  payrollId: number;
  companyId: number;
  periodStartDate: string;
  periodEndDate: string;
  totalGrossPay: number;
  totalDeductions: number;
  totalNetPay: number;
  totalInss: number;
  totalFgts: number;
  thirteenthPercentage?: number;
  thirteenthTaxOption?: string;
  isClosed: boolean;
  closedAt?: string;
  closedBy?: number;
  closedByName?: string;
  notes?: string;
  snapshot?: string;
  employeeCount: number;
  isLastPayroll: boolean;
  criadoPor: number;
  criadoPorNome: string;
  atualizadoPor?: number;
  atualizadoPorNome?: string;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface PayrollFilters {
  periodStartDate?: string;
  periodEndDate?: string;
  isClosed?: boolean;
  search?: string;
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
  totalCount: number;
  totalPages: number;
}

export interface PayrollItem {
  payrollItemId: number;
  payrollEmployeeId: number;
  description: string;
  type: string; // "Credit" ou "Debit"
  category: string;
  amount: number; // em centavos
  referenceId?: number;
  calculationBasis?: number;
  calculationDetails?: string;
}

export interface PayrollEmployeeDetailed {
  payrollEmployeeId: number;
  payrollId: number;
  employeeId: number;
  employeeName: string;
  employeeDocument?: string;
  isOnVacation: boolean;
  vacationDays?: number;
  vacationAdvanceAmount?: number; // em centavos
  vacationAdvancePaid?: boolean;
  vacationStartDate?: string;
  vacationEndDate?: string;
  vacationNotes?: string;
  totalGrossPay: number; // em centavos
  totalDeductions: number; // em centavos
  totalNetPay: number; // em centavos
  contractId?: number;
  contractType?: string; // "Mensalista", "Horista", "Diarista"
  contractValue?: number; // Valor base do contrato em centavos
  workedUnits?: number; // Horas ou dias trabalhados (para horistas/diaristas)
  hasFgts?: boolean; // Se o contrato tem FGTS
  items: PayrollItem[];
}

export interface UpdatePayrollItemData {
  description: string;
  amount: number; // em centavos
}

export interface UpdateWorkedUnitsData {
  workedUnits: number;
}

export interface PayrollItemInputData {
  payrollEmployeeId: number;
  description: string;
  type: 'Provento' | 'Desconto';
  category?: string;
  amount: number; // em centavos
}

export interface PayrollDetailed extends Payroll {
  employees: PayrollEmployeeDetailed[];
}

export interface PayrollSuggestion {
  suggestedMonth: number;
  suggestedYear: number;
  hasOpenPayroll: boolean;
  openPayrollId?: number;
  openPayrollPeriod?: string;
}

class PayrollService {
  async getPayrollSuggestion(): Promise<PayrollSuggestion> {
    const response = await api.get('/payroll/suggestion');
    return response.data.data;
  }

  async getAllPayrolls(): Promise<Payroll[]> {
    const response = await api.get('/payroll/getAll');
    return response.data.data;
  }

  async getPayrolls(filters?: PayrollFilters): Promise<PagedResult<Payroll>> {
    const params = new URLSearchParams();
    
    if (filters?.periodStartDate) params.append('periodStartDate', filters.periodStartDate);
    if (filters?.periodEndDate) params.append('periodEndDate', filters.periodEndDate);
    if (filters?.isClosed !== undefined) params.append('isClosed', filters.isClosed.toString());
    if (filters?.search) params.append('search', filters.search);
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());
    if (filters?.orderBy) params.append('orderBy', filters.orderBy);
    if (filters?.orderDirection) params.append('orderDirection', filters.orderDirection);

    const response = await api.get(`/payroll/getPaged?${params.toString()}`);
    return response.data.data;
  }

  async getPayrollById(id: number): Promise<Payroll> {
    const response = await api.get(`/payroll/${id}`);
    return response.data.data;
  }

  async getPayrollDetails(id: number): Promise<PayrollDetailed> {
    const response = await api.get(`/payroll/${id}/details`);
    return response.data.data;
  }

  async createPayroll(data: { 
    periodStartDate: string; 
    periodEndDate: string; 
    notes?: string;
  }): Promise<Payroll> {
    const response = await api.post('/payroll/create', data);
    return response.data.data;
  }

  async updatePayroll(id: number, data: { 
    periodStartDate: string; 
    periodEndDate: string; 
    notes?: string;
  }): Promise<Payroll> {
    const response = await api.put(`/payroll/${id}`, data);
    return response.data.data;
  }

  async deletePayroll(id: number): Promise<void> {
    await api.delete(`/payroll/${id}`);
  }

  async recalculatePayroll(id: number): Promise<PayrollDetailed> {
    const response = await api.post(`/payroll/${id}/recalculate`);
    return response.data.data;
  }

  async updatePayrollItem(payrollItemId: number, data: UpdatePayrollItemData): Promise<PayrollItem> {
    const response = await api.put(`/payroll/item/${payrollItemId}`, data);
    return response.data.data;
  }

  async updateWorkedUnits(payrollEmployeeId: number, data: UpdateWorkedUnitsData): Promise<PayrollEmployeeDetailed> {
    const response = await api.put(`/payroll/employee/${payrollEmployeeId}/worked-units`, data);
    return response.data.data;
  }

  async recalculateEmployee(payrollEmployeeId: number): Promise<PayrollEmployeeDetailed> {
    const response = await api.post(`/payroll/employee/${payrollEmployeeId}/recalculate`);
    return response.data.data;
  }

  async addPayrollItem(data: PayrollItemInputData): Promise<PayrollEmployeeDetailed> {
    const response = await api.post('/payroll/item', data);
    return response.data.data;
  }

  async applyThirteenthSalary(payrollId: number, data: { percentage: number; taxOption: string }): Promise<PayrollDetailed> {
    const response = await api.post(`/payroll/${payrollId}/thirteenth-salary`, data);
    return response.data.data;
  }

  async removeThirteenthSalary(payrollId: number): Promise<PayrollDetailed> {
    const response = await api.delete(`/payroll/${payrollId}/thirteenth-salary`);
    return response.data.data;
  }

  async applyVacation(payrollId: number, data: {
    payrollEmployeeId: number;
    vacationDays: number;
    vacationStartDate: string;
    includeTaxes: boolean;
    advanceNextMonth: boolean;
    notes?: string;
  }): Promise<PayrollDetailed> {
    const response = await api.post(`/payroll/${payrollId}/vacation`, data);
    return response.data.data;
  }

  async removeVacation(payrollId: number, payrollEmployeeId: number): Promise<PayrollDetailed> {
    const response = await api.delete(`/payroll/${payrollId}/vacation/${payrollEmployeeId}`);
    return response.data.data;
  }

  // Helpers para atualização local do estado
  updatePayrollItemLocally(payroll: PayrollDetailed, updatedItem: PayrollItem): PayrollDetailed {
    // Atualizar itens e recalcular totais do funcionário e da folha
    const newEmployees = payroll.employees.map(emp => {
      const hasItem = emp.items.some(item => item.payrollItemId === updatedItem.payrollItemId);
      if (!hasItem) return emp;
      
      // Atualizar itens do funcionário
      const newItems = emp.items.map(item =>
        item.payrollItemId === updatedItem.payrollItemId ? updatedItem : item
      );
      
      // Recalcular totais do funcionário
      const totalGrossPay = newItems
        .filter(item => item.type === 'Provento')
        .reduce((sum, item) => sum + item.amount, 0);
      const totalDeductions = newItems
        .filter(item => item.type === 'Desconto')
        .reduce((sum, item) => sum + item.amount, 0);
      
      return {
        ...emp,
        items: newItems,
        totalGrossPay,
        totalDeductions,
        totalNetPay: totalGrossPay - totalDeductions
      };
    });
    
    return {
      ...payroll,
      employees: newEmployees,
      // Recalcular totais da folha
      totalGrossPay: newEmployees.reduce((sum, emp) => sum + emp.totalGrossPay, 0),
      totalDeductions: newEmployees.reduce((sum, emp) => sum + emp.totalDeductions, 0),
      totalNetPay: newEmployees.reduce((sum, emp) => sum + emp.totalNetPay, 0)
    };
  }

  updateWorkedUnitsLocally(payroll: PayrollDetailed, updatedEmployee: PayrollEmployeeDetailed): PayrollDetailed {
    return {
      ...payroll,
      employees: payroll.employees.map(emp =>
        emp.payrollEmployeeId === updatedEmployee.payrollEmployeeId ? updatedEmployee : emp
      ),
      // Atualizar totais da folha
      totalGrossPay: payroll.employees.reduce((sum, emp) => 
        sum + (emp.payrollEmployeeId === updatedEmployee.payrollEmployeeId ? updatedEmployee.totalGrossPay : emp.totalGrossPay), 0),
      totalDeductions: payroll.employees.reduce((sum, emp) => 
        sum + (emp.payrollEmployeeId === updatedEmployee.payrollEmployeeId ? updatedEmployee.totalDeductions : emp.totalDeductions), 0),
      totalNetPay: payroll.employees.reduce((sum, emp) => 
        sum + (emp.payrollEmployeeId === updatedEmployee.payrollEmployeeId ? updatedEmployee.totalNetPay : emp.totalNetPay), 0)
    };
  }

  // Helper para recalcular funcionário (usa employeeId pois o payrollEmployeeId muda)
  replaceEmployeeLocally(payroll: PayrollDetailed, updatedEmployee: PayrollEmployeeDetailed): PayrollDetailed {
    const newEmployees = payroll.employees.map(emp =>
      emp.employeeId === updatedEmployee.employeeId ? updatedEmployee : emp
    );
    
    return {
      ...payroll,
      employees: newEmployees,
      // Recalcular totais da folha
      totalGrossPay: newEmployees.reduce((sum, emp) => sum + emp.totalGrossPay, 0),
      totalDeductions: newEmployees.reduce((sum, emp) => sum + emp.totalDeductions, 0),
      totalNetPay: newEmployees.reduce((sum, emp) => sum + emp.totalNetPay, 0)
    };
  }

  async closePayroll(payrollId: number, data: { accountId: number; paymentDate: string; inssAmount: number; fgtsAmount: number }): Promise<PayrollDetailed> {
    const response = await api.post(`/payroll/${payrollId}/close`, data);
    return response.data.data;
  }

  async reopenPayroll(payrollId: number): Promise<PayrollDetailed> {
    const response = await api.post(`/payroll/${payrollId}/reopen`);
    return response.data.data;
  }
}

export default new PayrollService();
