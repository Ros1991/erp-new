import type { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { usePermissions } from '../../contexts/PermissionContext';

interface ProtectedRouteProps {
  /** Permissões necessárias (OR lógico se múltiplas) */
  requires?: string | string[];
  /** Requer TODAS as permissões (AND lógico) */
  requiresAll?: string[];
  /** Rota para redirecionar se não tiver permissão */
  redirectTo?: string;
  /** Filhos a serem renderizados se tiver permissão */
  children: ReactNode;
}

/**
 * Componente para proteger rotas inteiras baseado em permissões
 * Redireciona para página de acesso negado se não tiver permissão
 * 
 * @example
 * // No routes/index.tsx
 * <Route
 *   path="/roles"
 *   element={
 *     <ProtectedRoute requires="role.canView">
 *       <Roles />
 *     </ProtectedRoute>
 *   }
 * />
 * 
 * @example
 * // Múltiplas permissões (OR)
 * <Route
 *   path="/roles/new"
 *   element={
 *     <ProtectedRoute requires={["role.canCreate", "role.canEdit"]}>
 *       <RoleForm />
 *     </ProtectedRoute>
 *   }
 * />
 * 
 * @example
 * // Todas as permissões (AND)
 * <Route
 *   path="/admin"
 *   element={
 *     <ProtectedRoute requiresAll={["role.*", "user.*"]}>
 *       <AdminPanel />
 *     </ProtectedRoute>
 *   }
 * />
 */
export function ProtectedRoute({ 
  requires, 
  requiresAll, 
  redirectTo = '/access-denied',
  children 
}: ProtectedRouteProps) {
  const { hasAnyPermission, hasAllPermissions, loading } = usePermissions();

  // Aguardar carregamento das permissões
  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
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

  // Se não tem acesso, redireciona
  if (!hasAccess) {
    return <Navigate to={redirectTo} replace />;
  }

  // Se tem acesso, renderiza a rota
  return <>{children}</>;
}
