import { Link, useLocation } from 'react-router-dom';
import {
  Home,
  Shield,
  Users,
  Wallet
} from 'lucide-react';

interface SidebarProps {
  sidebarOpen: boolean;
}

export function Sidebar({ sidebarOpen }: SidebarProps) {
  const location = useLocation();
  
  const menuItems = [
    { icon: Home, label: 'Dashboard', path: '/dashboard' },
    { icon: Shield, label: 'Cargos', path: '/roles' },
    { icon: Users, label: 'Usuários', path: '/users' },
    { icon: Wallet, label: 'Conta Correntes', path: '/accounts' },
  ];

  return (
    <aside
      className={`fixed left-0 top-16 h-[calc(100vh-4rem)] bg-white border-r border-gray-200 transition-transform z-30 ${
        sidebarOpen ? 'translate-x-0' : '-translate-x-full'
      } lg:translate-x-0 lg:w-64 w-64`}
    >
      <nav className="p-4 space-y-1">
        {menuItems.map((item) => {
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
