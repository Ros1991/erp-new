import React, { createContext, useContext, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import authService from '../services/authService';
import companyService from '../services/companyService';

interface User {
  userId: number;
  name?: string;
  email?: string;
  phone?: string;
  cpf?: string;
}

interface Company {
  id: number;
  name: string;
  cnpj?: string;
  userId: number; // ID do dono da empresa
  isActive: boolean;
  createdAt: string;
}

interface AuthContextType {
  user: User | null;
  selectedCompany: Company | null;
  companies: Company[];
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (credential: string, password: string) => Promise<void>;
  logout: () => void;
  selectCompany: (company: Company) => void;
  loadCompanies: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [selectedCompany, setSelectedCompany] = useState<Company | null>(null);
  const [companies, setCompanies] = useState<Company[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    checkAuth();
  }, []);

  const checkAuth = async () => {
    const token = authService.getToken();
    const companyId = localStorage.getItem('selectedCompanyId');
    
    if (token) {
      try {
        // Obter dados do usuário do localStorage
        const user = authService.getUser();
        
        if (user) {
          setUser(user);
          
          if (companyId) {
            const company = JSON.parse(localStorage.getItem('selectedCompany') || '{}');
            if (company.id) {
              setSelectedCompany(company);
            }
          }
        } else {
          // Sem dados de usuário, limpar
          localStorage.clear();
        }
      } catch (error) {
        console.error('Auth check failed:', error);
        localStorage.clear();
      }
    }
    setIsLoading(false);
  };

  const login = async (credential: string, password: string) => {
    try {
      // Fazer login usando o authService
      const response = await authService.login({ credential, password });
      
      // Extrair dados do usuário da resposta
      const user = {
        userId: response.userId,
        email: response.email,
        phone: response.phone,
        cpf: response.cpf
      };
      
      setUser(user);
      
      // Carregar empresas do usuário
      await loadCompanies();
      
      navigate('/companies');
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  };

  const logout = async () => {
    try {
      await authService.logout();
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      setUser(null);
      setSelectedCompany(null);
      setCompanies([]);
      navigate('/login');
    }
  };

  const selectCompany = (company: Company) => {
    setSelectedCompany(company);
    localStorage.setItem('selectedCompanyId', company.id.toString());
    localStorage.setItem('selectedCompany', JSON.stringify(company));
    api.defaults.headers.common['X-Company-Id'] = company.id.toString();
    navigate('/dashboard');
  };

  const loadCompanies = async () => {
    try {
      // Buscar empresas do usuário via API
      const companiesData = await companyService.getMyCompanies();
      
      // Mapear para o formato esperado pelo frontend
      const mappedCompanies: Company[] = companiesData.map((company: any) => ({
        id: company.companyId,
        name: company.name,
        cnpj: company.document,
        userId: company.userId, // ID do dono da empresa
        isActive: true, // Assumir ativo por padrão (ajustar se backend tiver campo isActive)
        createdAt: company.criadoEm
      }));
      
      setCompanies(mappedCompanies);
    } catch (error) {
      console.error('Load companies failed:', error);
      setCompanies([]);
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        selectedCompany,
        companies,
        isAuthenticated: !!user,
        isLoading,
        login,
        logout,
        selectCompany,
        loadCompanies
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
