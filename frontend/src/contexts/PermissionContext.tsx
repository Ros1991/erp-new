import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import api from '../services/api';

interface ModulePermissions {
  canView: boolean;
  canCreate: boolean;
  canEdit: boolean;
  canDelete: boolean;
}

interface UserPermissions {
  isAdmin: boolean;
  isSystemRole: boolean;
  modules: Record<string, ModulePermissions>;
}

interface PermissionContextType {
  permissions: UserPermissions | null;
  loading: boolean;
  hasPermission: (permission: string) => boolean;
  hasAnyPermission: (...permissions: string[]) => boolean;
  hasAllPermissions: (...permissions: string[]) => boolean;
  loadPermissions: () => Promise<void>;
  clearPermissions: () => void;
}

const PermissionContext = createContext<PermissionContextType | undefined>(undefined);

export function PermissionProvider({ children }: { children: ReactNode }) {
  const [permissions, setPermissions] = useState<UserPermissions | null>(null);
  const [loading, setLoading] = useState(true);

  // Carregar permissões do localStorage ao montar
  useEffect(() => {
    const cached = localStorage.getItem('userPermissions');
    if (cached) {
      try {
        setPermissions(JSON.parse(cached));
      } catch (err) {
        console.error('Erro ao carregar permissões do cache:', err);
      }
    }
    setLoading(false);
  }, []);

  const loadPermissions = async () => {
    setLoading(true);
    try {
      // Endpoint que retorna permissões do usuário na empresa atual
      const response = await api.get('/auth/permissions');
      const userPermissions = response.data.data;
      
      setPermissions(userPermissions);
      localStorage.setItem('userPermissions', JSON.stringify(userPermissions));
    } catch (err: any) {
      console.error('Erro ao carregar permissões:', err);
      setPermissions(null);
    } finally {
      setLoading(false);
    }
  };

  const clearPermissions = () => {
    setPermissions(null);
    localStorage.removeItem('userPermissions');
  };

  /**
   * Verifica se o usuário tem uma permissão específica
   * Suporta formato: "module.permission" (ex: "role.canView")
   * Suporta hierarquia: "module.*" (todas as permissões do módulo)
   */
  const hasPermission = (permission: string): boolean => {
    // Se não tem permissões carregadas, nega acesso
    if (!permissions) {
      return false;
    }

    // IsAdmin ou IsSystem = acesso total
    if (permissions.isAdmin || permissions.isSystemRole) {
      return true;
    }

    // Parse: "role.canView" → module="role", action="canView"
    const parts = permission.split('.');
    if (parts.length !== 2) {
      console.warn(`⚠️ Formato de permissão inválido: ${permission}`);
      return false;
    }

    const [module, action] = parts;

    // Verifica se o módulo existe
    const modulePerms = permissions.modules[module];
    if (!modulePerms) {
      return false;
    }

    // Hierarquia: role.* = todas as permissões do módulo
    if (action === '*') {
      return modulePerms.canView && 
             modulePerms.canCreate && 
             modulePerms.canEdit && 
             modulePerms.canDelete;
    }

    // Verifica permissão específica
    const permKey = action as keyof ModulePermissions;
    return modulePerms[permKey] || false;
  };

  /**
   * Verifica se o usuário tem PELO MENOS UMA das permissões (OR lógico)
   */
  const hasAnyPermission = (...perms: string[]): boolean => {
    return perms.some(p => hasPermission(p));
  };

  /**
   * Verifica se o usuário tem TODAS as permissões (AND lógico)
   */
  const hasAllPermissions = (...perms: string[]): boolean => {
    return perms.every(p => hasPermission(p));
  };

  return (
    <PermissionContext.Provider
      value={{
        permissions,
        loading,
        hasPermission,
        hasAnyPermission,
        hasAllPermissions,
        loadPermissions,
        clearPermissions
      }}
    >
      {children}
    </PermissionContext.Provider>
  );
}

export function usePermissions() {
  const context = useContext(PermissionContext);
  if (context === undefined) {
    throw new Error('usePermissions deve ser usado dentro de um PermissionProvider');
  }
  return context;
}
