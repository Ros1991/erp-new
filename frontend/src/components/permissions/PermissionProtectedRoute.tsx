import { Navigate } from 'react-router-dom';
import { usePermissions } from '../../contexts/PermissionContext';

interface PermissionProtectedRouteProps {
  requires?: string | string[];
  requiresAll?: string[];
  children: React.ReactNode;
}

/**
 * Componente para proteger rotas baseado em permissões
 * Se o usuário não tiver permissão, redireciona para /access-denied
 */
export function PermissionProtectedRoute({
  children,
  requires,
  requiresAll
}: PermissionProtectedRouteProps) {
  const { hasAnyPermission, hasAllPermissions, permissions } = usePermissions();

  // Aguarda permissões carregarem
  if (!permissions) {
    return (
      <div className="min-h-screen flex items-center justify-center">
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
    return <Navigate to="/access-denied" replace />;
  }

  // Se tem acesso, renderiza children
  return <>{children}</>;
}
