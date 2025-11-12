import { usePermissions } from '../contexts/PermissionContext';

/**
 * Hook para verificar permissões de forma simples
 * 
 * @example
 * const canCreate = usePermission('role.canCreate');
 * 
 * @example
 * const canEditOrCreate = usePermission(['role.canEdit', 'role.canCreate']);
 * 
 * @returns boolean indicando se o usuário tem a(s) permissão(ões)
 */
export function usePermission(permission: string | string[]): boolean {
  const { hasPermission, hasAnyPermission } = usePermissions();

  if (Array.isArray(permission)) {
    return hasAnyPermission(...permission);
  }

  return hasPermission(permission);
}
