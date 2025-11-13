import { Link, useLocation } from 'react-router-dom';
import { usePermissions } from '../../contexts/PermissionContext';
import {
  Home,
  Shield,
  Users,
  UserCheck,
  Wallet,
  FileText,
  PieChart,
  DollarSign,
  MapPin,
  ShoppingCart,
  Users as UsersIcon,
  CheckSquare
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
    { icon: FileText, label: 'Contas a Pagar e Receber', path: '/account-payable-receivable', permission: 'accountPayableReceivable.canView' },
    { icon: PieChart, label: 'Centros de Custo', path: '/cost-centers', permission: 'costCenter.canView' },
    { icon: DollarSign, label: 'Empréstimos e Adiantamentos', path: '/loan-advances', permission: 'loanAdvance.canView' },
    { icon: MapPin, label: 'Locais', path: '/locations', permission: 'location.canView' },
    { icon: ShoppingCart, label: 'Ordens de Compra', path: '/purchase-orders', permission: 'purchaseOrder.canView' },
    { icon: UsersIcon, label: 'Fornecedores e Clientes', path: '/supplier-customers', permission: 'supplierCustomer.canView' },
    { icon: CheckSquare, label: 'Tarefas', path: '/tasks', permission: 'task.canView' },
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
