import { Link, useLocation } from 'react-router-dom';
import { usePermissions } from '../../contexts/PermissionContext';
import {
  Home,
  Shield,
  Users,
  UserCheck,
  Wallet
} from 'lucide-react';

interface SidebarProps {
  sidebarOpen: boolean;
}

interface MenuItem {
  icon: any;
  label: string;
  path: string;
  permission?: string; // Permissão necessária para ver o item (opcional)
}

export function Sidebar({ sidebarOpen }: SidebarProps) {
  const location = useLocation();
  const { hasPermission } = usePermissions();
  
  const menuItems: MenuItem[] = [
    { icon: Home, label: 'Dashboard', path: '/dashboard' }, // Sem permissão = sempre visível
    { icon: Shield, label: 'Cargos', path: '/roles', permission: 'role.canView' },
    { icon: Users, label: 'Usuários', path: '/users', permission: 'user.canView' },
    { icon: UserCheck, label: 'Empregados', path: '/employees', permission: 'employee.canView' },
    { icon: Wallet, label: 'Conta Correntes', path: '/accounts', permission: 'account.canView' },
  ];
  
  // Filtrar itens baseado nas permissões
  const visibleItems = menuItems.filter(item => {
    // Se não tem permissão definida, sempre mostra (ex: Dashboard)
    if (!item.permission) return true;
    // Verifica se tem a permissão necessária
    return hasPermission(item.permission);
  });

  return (
    <aside
      className={`fixed left-0 top-16 h-[calc(100vh-4rem)] bg-white border-r border-gray-200 transition-transform z-30 ${
        sidebarOpen ? 'translate-x-0' : '-translate-x-full'
      } lg:translate-x-0 lg:w-64 w-64`}
    >
      <nav className="p-4 space-y-1">
        {visibleItems.map((item) => {
          const isActive = location.pathname === item.path;
          
          return (
            <Link
              key={item.path}
              to={item.path}
              className={`flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors ${
                isActive
                  ? 'bg-primary-50 text-primary-600'
                  : 'text-gray-700 hover:bg-gray-100'
              }`}
            >
              <item.icon className="h-5 w-5" />
              <span className="font-medium">{item.label}</span>
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}
