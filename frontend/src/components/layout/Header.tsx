import { useState, useRef, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { ChevronDown, LogOut, Menu, RefreshCw, X } from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';

interface HeaderProps {
  sidebarOpen: boolean;
  setSidebarOpen: (open: boolean) => void;
}

export function Header({ sidebarOpen, setSidebarOpen }: HeaderProps) {
  const [userMenuOpen, setUserMenuOpen] = useState(false);
  const userMenuRef = useRef<HTMLDivElement>(null);
  const { user, selectedCompany, logout } = useAuth();

  // Fechar menu ao clicar fora
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (userMenuRef.current && !userMenuRef.current.contains(event.target as Node)) {
        setUserMenuOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  // Texto de identificação do usuário (email, telefone ou CPF)
  const getUserIdentifier = () => {
    if (user?.email) return user.email;
    if (user?.phone) return user.phone;
    if (user?.cpf) return user.cpf;
    return 'Usuário';
  };

  return (
    <header className="fixed top-0 w-full bg-white border-b border-gray-200 z-40">
      <div className="flex items-center justify-between px-6 py-4">
        <div className="flex items-center space-x-6">
          <button
            onClick={() => setSidebarOpen(!sidebarOpen)}
            className="lg:hidden text-gray-600 hover:text-gray-900"
          >
            {sidebarOpen ? <X size={24} /> : <Menu size={24} />}
          </button>
          
          {/* Logo - Pequena no mobile, completa no desktop */}
          <img src="/logo.svg" alt="MeuGestor" className="h-8 md:hidden" />
          <img src="/logo-full.svg" alt="MeuGestor" className="h-10 hidden md:block" />
          
          {/* Nome da Empresa - Visível em todas as resoluções */}
          <div className="flex items-center">
            <span className="text-lg md:text-2xl font-bold text-gray-900">
              {selectedCompany?.name || 'Nenhuma empresa selecionada'}
            </span>
          </div>
        </div>

        {/* User Menu */}
        <div className="relative" ref={userMenuRef}>
          <button
            onClick={() => setUserMenuOpen(!userMenuOpen)}
            className="flex items-center space-x-2 px-3 py-2 rounded-lg hover:bg-gray-100 transition-colors"
          >
            <div className="text-right hidden sm:block">
              <p className="text-sm font-medium text-gray-900">{user?.name}</p>
              <p className="text-xs text-gray-500">{getUserIdentifier()}</p>
            </div>
            <ChevronDown className={`h-4 w-4 text-gray-500 transition-transform ${
              userMenuOpen ? 'rotate-180' : ''
            }`} />
          </button>

          {/* Dropdown Menu */}
          {userMenuOpen && (
            <div className="absolute right-0 mt-2 w-56 bg-white rounded-lg shadow-lg border border-gray-200 py-1">
              <Link
                to="/companies"
                className="flex items-center space-x-3 px-4 py-3 hover:bg-gray-50 transition-colors"
                onClick={() => setUserMenuOpen(false)}
              >
                <RefreshCw className="h-4 w-4 text-gray-600" />
                <div>
                  <p className="text-sm font-medium text-gray-900">Trocar de Empresa</p>
                  <p className="text-xs text-gray-500">Selecionar outra empresa</p>
                </div>
              </Link>
              
              <div className="border-t border-gray-100 my-1" />
              
              <button
                onClick={() => {
                  setUserMenuOpen(false);
                  logout();
                }}
                className="flex items-center space-x-3 px-4 py-3 hover:bg-gray-50 transition-colors w-full text-left"
              >
                <LogOut className="h-4 w-4 text-red-600" />
                <div>
                  <p className="text-sm font-medium text-red-700">Sair</p>
                  <p className="text-xs text-gray-500">Encerrar sessão</p>
                </div>
              </button>
            </div>
          )}
        </div>
      </div>
    </header>
  );
}
