import api from './api';

export interface Company {
  companyId: number;
  name: string;
  document?: string;
  userId: number; // ID do dono da empresa
  criadoPor: number;
  atualizadoPor?: number;
  criadoEm: string;
  atualizadoEm?: string;
}

export interface CreateCompanyInput {
  name: string;
  document?: string;
}

export interface UpdateCompanyInput {
  name?: string;
  document?: string;
}

class CompanyService {
  /**
   * Lista todas as empresas do usuário autenticado
   */
  async getMyCompanies(): Promise<Company[]> {
    const response = await api.get('/company/getAll');
    return response.data.data; // BaseResponse wrapper
  }

  /**
   * Busca uma empresa por ID
   */
  async getCompanyById(id: number): Promise<Company> {
    const response = await api.get(`/company/${id}`);
    return response.data.data; // BaseResponse wrapper
  }

  /**
   * Cria uma nova empresa
   */
  async createCompany(data: CreateCompanyInput): Promise<Company> {
    const response = await api.post('/company/create', data);
    console.log(response.data.data);
    return response.data.data; // BaseResponse wrapper
  }

  /**
   * Atualiza uma empresa existente
   */
  async updateCompany(id: number, data: UpdateCompanyInput): Promise<Company> {
    const response = await api.put(`/company/${id}`, data);
    return response.data.data; // BaseResponse wrapper
  }

  /**
   * Deleta uma empresa
   */
  async deleteCompany(id: number): Promise<void> {
    await api.delete(`/company/${id}`);
  }

  /**
   * Valida CNPJ
   */
  validateCNPJ(cnpj: string): boolean {
    // Remove caracteres não numéricos
    cnpj = cnpj.replace(/\D/g, '');

    // Verifica se tem 14 dígitos
    if (cnpj.length !== 14) return false;

    // Verifica se todos os dígitos são iguais
    if (/^(\d)\1+$/.test(cnpj)) return false;

    // Validação dos dígitos verificadores
    let tamanho = cnpj.length - 2;
    let numeros = cnpj.substring(0, tamanho);
    const digitos = cnpj.substring(tamanho);
    let soma = 0;
    let pos = tamanho - 7;

    for (let i = tamanho; i >= 1; i--) {
      soma += parseInt(numeros.charAt(tamanho - i)) * pos--;
      if (pos < 2) pos = 9;
    }

    let resultado = soma % 11 < 2 ? 0 : 11 - (soma % 11);
    if (resultado !== parseInt(digitos.charAt(0))) return false;

    tamanho = tamanho + 1;
    numeros = cnpj.substring(0, tamanho);
    soma = 0;
    pos = tamanho - 7;

    for (let i = tamanho; i >= 1; i--) {
      soma += parseInt(numeros.charAt(tamanho - i)) * pos--;
      if (pos < 2) pos = 9;
    }

    resultado = soma % 11 < 2 ? 0 : 11 - (soma % 11);
    if (resultado !== parseInt(digitos.charAt(1))) return false;

    return true;
  }

  /**
   * Formata CNPJ para exibição
   */
  formatCNPJ(cnpj: string): string {
    cnpj = cnpj.replace(/\D/g, '');
    return cnpj.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5');
  }
}

export default new CompanyService();
