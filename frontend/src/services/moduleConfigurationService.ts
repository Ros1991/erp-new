import api from './api';

export interface EndpointRoute {
  method: string;
  path: string;
}

export interface PermissionConfig {
  key: string;
  name: string;
  description: string;
  allowedEndpoints?: string[];
  allowedRoutes?: EndpointRoute[];
  order: number;
  color?: string;
}

export interface ModuleConfig {
  key: string;
  name: string;
  description: string;
  icon?: string;
  order: number;
  isActive: boolean;
  permissions: PermissionConfig[];
}

export interface SystemModulesConfig {
  modules: ModuleConfig[];
}

class ModuleConfigurationService {
  private cachedConfig: SystemModulesConfig | null = null;

  /**
   * Obtém a configuração completa de módulos do sistema
   * Faz cache para evitar múltiplas chamadas
   */
  async getConfiguration(forceRefresh = false): Promise<SystemModulesConfig> {
    if (this.cachedConfig && !forceRefresh) {
      return this.cachedConfig;
    }

    const response = await api.get('/moduleconfiguration');
    this.cachedConfig = response.data.data;
    return response.data.data;
  }

  /**
   * Obtém apenas os módulos ativos
   */
  async getActiveModules(): Promise<ModuleConfig[]> {
    const response = await api.get('/moduleconfiguration/active');
    return response.data.data;
  }

  /**
   * Obtém um módulo específico por chave
   */
  async getModule(moduleKey: string): Promise<ModuleConfig> {
    const response = await api.get(`/moduleconfiguration/${moduleKey}`);
    return response.data.data;
  }

  /**
   * Obtém os endpoints permitidos para uma permissão específica
   */
  async getAllowedEndpoints(moduleKey: string, permissionKey: string): Promise<string[]> {
    const response = await api.get(`/moduleconfiguration/${moduleKey}/permissions/${permissionKey}/endpoints`);
    return response.data.data;
  }

  /**
   * Limpa o cache (útil quando a configuração é atualizada)
   */
  clearCache() {
    this.cachedConfig = null;
  }
}

export default new ModuleConfigurationService();
