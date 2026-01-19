import { useState, useEffect } from 'react';
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
  FolderOpen,
  ChevronDown,
  ChevronRight,
  ArrowRightLeft,
  Receipt,
  Building2,
  BarChart3,
  TrendingUp,
  Landmark,
  HandCoins,
  Activity,
  ClipboardList,
  ShoppingCart
} from 'lucide-react';

interface SidebarProps {
  sidebarOpen: boolean;
}

interface MenuItem {
  icon: any;
  label: string;
  path?: string;
  permission?: string;
  children?: MenuItem[];
}

export function Sidebar({ sidebarOpen }: SidebarProps) {
  const location = useLocation();
  const { hasPermission } = usePermissions();
  const [expandedMenus, setExpandedMenus] = useState<string[]>(['Cadastros', 'Relatórios']);

  // Manter menus expandidos quando um item filho está ativo
  useEffect(() => {
    const currentPath = location.pathname;
    
    // Verificar se a rota atual pertence a algum menu
    if (currentPath.startsWith('/reports/')) {
      setExpandedMenus(prev => prev.includes('Relatórios') ? prev : [...prev, 'Relatórios']);
    }
    if (['/roles', '/users', '/employees', '/accounts', '/supplier-customers', '/cost-centers', 
         '/account-payable-receivable', '/loan-advances', '/financial-transactions', '/payroll', '/purchase-orders']
        .some(path => currentPath.startsWith(path))) {
      setExpandedMenus(prev => prev.includes('Cadastros') ? prev : [...prev, 'Cadastros']);
    }
  }, [location.pathname]);

  const toggleMenu = (label: string) => {
    setExpandedMenus(prev => 
      prev.includes(label) 
        ? prev.filter(item => item !== label)
        : [...prev, label]
    );
  };
  
  const menuItems: MenuItem[] = [
    { icon: Home, label: 'Dashboard', path: '/dashboard' },
    {
      icon: FolderOpen,
      label: 'Cadastros',
      children: [
        { icon: Shield, label: 'Cargos', path: '/roles', permission: 'role.canView' },
        { icon: Users, label: 'Usuários', path: '/users', permission: 'user.canView' },
        { icon: UserCheck, label: 'Empregados', path: '/employees', permission: 'employee.canView' },
        { icon: Wallet, label: 'Contas Correntes', path: '/accounts', permission: 'account.canView' },
        { icon: Building2, label: 'Fornecedores e Clientes', path: '/supplier-customers', permission: 'supplierCustomer.canView' },
        { icon: PieChart, label: 'Centros de Custo', path: '/cost-centers', permission: 'costCenter.canView' },
        { icon: FileText, label: 'Contas a Pagar e Receber', path: '/account-payable-receivable', permission: 'accountPayableReceivable.canView' },
        { icon: DollarSign, label: 'Empréstimos e Adiantamentos', path: '/loan-advances', permission: 'loanAdvance.canView' },
        { icon: ShoppingCart, label: 'Ordens de Compra', path: '/purchase-orders', permission: 'purchaseOrder.canView' },
        { icon: ArrowRightLeft, label: 'Transações Financeiras', path: '/financial-transactions', permission: 'financialTransaction.canView' },
        { icon: Receipt, label: 'Folha de Pagamento', path: '/payroll', permission: 'payroll.canView' },
      ]
    },
    {
      icon: BarChart3,
      label: 'Relatórios',
      children: [
        { icon: TrendingUp, label: 'Dashboard Financeiro', path: '/reports/financial-dashboard', permission: 'report.canView' },
        { icon: PieChart, label: 'Por Centro de Custo', path: '/reports/cost-center', permission: 'report.canView' },
        { icon: Landmark, label: 'Por Conta Corrente', path: '/reports/account', permission: 'report.canView' },
        { icon: HandCoins, label: 'Por Fornecedor/Cliente', path: '/reports/supplier-customer', permission: 'report.canView' },
        { icon: Activity, label: 'Fluxo de Caixa', path: '/reports/cash-flow', permission: 'report.canView' },
        { icon: ClipboardList, label: 'Contas a Pagar/Receber', path: '/reports/accounts-payable-receivable', permission: 'report.canView' },
        { icon: BarChart3, label: 'Previsão Financeira', path: '/reports/financial-forecast', permission: 'report.canView' },
        { icon: Users, label: 'Conta Corrente Funcionário', path: '/reports/employee-account', permission: 'report.canView' },
      ]
    },
  ];
  
  // Filtra menus recursivamente baseado em permissões
  // Parent só é mostrado se tiver pelo menos 1 child visível
  const filterMenuItems = (items: MenuItem[]): MenuItem[] => {
    return items
      .map(item => {
        // Se tem children (ex: "Cadastros"), filtra recursivamente
        if (item.children) {
          const visibleChildren = filterMenuItems(item.children);
          // Parent só aparece se tiver pelo menos 1 child visível
          if (visibleChildren.length > 0) {
            return { ...item, children: visibleChildren };
          }
          // Se não tem nenhum child visível, não mostra o parent
          return null;
        }
        // Items sem permissão = sempre visível (ex: Dashboard)
        if (!item.permission) return item;
        // Verifica se tem a permissão necessária
        return hasPermission(item.permission) ? item : null;
      })
      .filter(Boolean) as MenuItem[];
  };

  const visibleItems = filterMenuItems(menuItems);

  return (
    <aside
      className={`fixed left-0 top-16 h-[calc(100vh-4rem)] bg-white border-r border-gray-200 transition-transform z-30 ${
        sidebarOpen ? 'translate-x-0' : '-translate-x-full'
      } lg:translate-x-0 lg:w-72 w-72 overflow-y-auto`}
    >
      <nav className="p-4 space-y-1">
        {visibleItems.map((item) => {
          if (item.children) {
            const isExpanded = expandedMenus.includes(item.label);
            const hasActiveChild = item.children.some(child => child.path === location.pathname);
            
            return (
              <div key={item.label}>
                <button
                  onClick={() => toggleMenu(item.label)}
                  className={`w-full flex items-center justify-between px-4 py-3 rounded-lg transition-colors ${
                    hasActiveChild
                      ? 'bg-primary-50 text-primary-600'
                      : 'text-gray-700 hover:bg-gray-100'
                  }`}
                >
                  <div className="flex items-center space-x-3">
                    <item.icon className="h-5 w-5" />
                    <span className="font-medium">{item.label}</span>
                  </div>
                  {isExpanded ? (
                    <ChevronDown className="h-4 w-4" />
                  ) : (
                    <ChevronRight className="h-4 w-4" />
                  )}
                </button>
                {isExpanded && (
                  <div className="ml-4 mt-1 space-y-1">
                    {item.children.map((child) => {
                      const isActive = location.pathname === child.path;
                      return (
                        <Link
                          key={child.path}
                          to={child.path!}
                          className={`flex items-center space-x-3 px-4 py-2 rounded-lg transition-colors text-sm ${
                            isActive
                              ? 'bg-primary-50 text-primary-600'
                              : 'text-gray-600 hover:bg-gray-50'
                          }`}
                        >
                          <child.icon className="h-4 w-4" />
                          <span>{child.label}</span>
                        </Link>
                      );
                    })}
                  </div>
                )}
              </div>
            );
          }
          
          const isActive = location.pathname === item.path;
          return (
            <Link
              key={item.path}
              to={item.path!}
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
