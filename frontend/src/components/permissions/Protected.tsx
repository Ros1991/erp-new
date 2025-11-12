import type { ReactNode } from 'react';
import { usePermissions } from '../../contexts/PermissionContext';

interface ProtectedProps {
  /** Permissões necessárias (OR lógico se múltiplas) */
  requires?: string | string[];
  /** Requer TODAS as permissões (AND lógico) */
  requiresAll?: string[];
  /** Componente a ser renderizado se não tiver permissão */
  fallback?: ReactNode;
  /** Filhos a serem renderizados se tiver permissão */
  children: ReactNode;
}

/**
 * Componente para controlar visibilidade baseado em permissões
 * 
 * @example
 * // Simples - mostrar apenas se tiver permissão
 * <Protected requires="role.canCreate">
 *   <button>Criar Cargo</button>
 * </Protected>
 * 
 * @example
 * // Múltiplas permissões (OR) - qualquer uma permite
 * <Protected requires={["role.canEdit", "role.canCreate"]}>
 *   <button>Editar</button>
 * </Protected>
 * 
 * @example
 * // Todas as permissões (AND)
 * <Protected requiresAll={["role.canView", "role.canEdit"]}>
 *   <button>Visualizar e Editar</button>
 * </Protected>
 * 
 * @example
 * // Com fallback
 * <Protected 
 *   requires="role.canView"
 *   fallback={<div>Sem permissão</div>}
 * >
 *   <DataTable />
 * </Protected>
 */
export function Protected({
  children,
  requires,
  requiresAll,
  fallback = null
}: ProtectedProps) {
  const { hasAnyPermission, hasAllPermissions, permissions } = usePermissions();

  // Se ainda está carregando permissões, não mostra nada
  if (!permissions) {
    return null;
  }

  // Verificar permissões
  let hasAccess = true;

  if (requiresAll && requiresAll.length > 0) {
    // AND lógico - precisa de todas
    hasAccess = hasAllPermissions(...requiresAll);
  } else if (requires) {
    // OR lógico - precisa de pelo menos uma
    const perms = Array.isArray(requires) ? requires : [requires];
    hasAccess = hasAnyPermission(...perms);
  }

  // Se não tem acesso, renderiza fallback
  if (!hasAccess) {
    return <>{fallback}</>;
  }

  // Se tem acesso, renderiza children
  return <>{children}</>;
}
