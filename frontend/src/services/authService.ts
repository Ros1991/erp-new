import api from './api';

// Tipos de resposta da API (baseados nos DTOs do backend C#)
export interface AuthResponse {
  userId: number;
  email?: string;
  phone?: string;
  cpf?: string;
  token: string;
  refreshToken: string;
  expiresAt: string;
  refreshExpiresAt: string;
}

export interface RegisterData {
  email?: string;
  phone?: string;
  cpf?: string;
  password: string;
  confirmPassword: string;
}

export interface LoginData {
  credential: string; // Pode ser email, phone ou CPF
  password: string;
}

export interface ForgotPasswordData {
  credential: string; // Email, phone ou CPF
}

export interface ResetPasswordData {
  token: string;
  newPassword: string;
  confirmPassword: string;
}

// Service de Autenticação
class AuthService {
  /**
   * Faz login com credencial (email, phone ou CPF) e senha
   */
  async login(data: LoginData): Promise<AuthResponse> {
    const response = await api.post<{ data: AuthResponse }>('/auth/login', data);
    const authData = response.data.data;
    
    // Armazenar tokens no localStorage
    if (authData.token) {
      localStorage.setItem('token', authData.token);
      localStorage.setItem('refreshToken', authData.refreshToken);
      const user = {
        userId: authData.userId,
        email: authData.email,
        phone: authData.phone,
        cpf: authData.cpf
      };
      localStorage.setItem('user', JSON.stringify(user));
    }
    
    return authData;
  }

  /**
   * Registra um novo usuário
   */
  async register(data: RegisterData): Promise<AuthResponse> {
    const response = await api.post<{ data: AuthResponse }>('/auth/register', data);
    const authData = response.data.data;
    
    // Armazenar tokens no localStorage após registro
    if (authData.token) {
      localStorage.setItem('token', authData.token);
      localStorage.setItem('refreshToken', authData.refreshToken);
      const user = {
        userId: authData.userId,
        email: authData.email,
        phone: authData.phone,
        cpf: authData.cpf
      };
      localStorage.setItem('user', JSON.stringify(user));
    }
    
    return authData;
  }

  /**
   * Solicita código de recuperação de senha
   */
  async forgotPassword(data: ForgotPasswordData): Promise<{ message: string }> {
    const response = await api.post('/auth/forgot-password', data);
    return response.data;
  }

  /**
   * Redefine a senha usando o código recebido
   */
  async resetPassword(data: ResetPasswordData): Promise<{ message: string }> {
    const response = await api.post('/auth/reset-password', data);
    return response.data;
  }

  /**
   * Faz logout do usuário
   */
  async logout(): Promise<void> {
    try {
      const refreshToken = localStorage.getItem('refreshToken');
      if (refreshToken) {
        await api.post('/auth/logout', JSON.stringify(refreshToken), {
          headers: { 'Content-Type': 'application/json' }
        });
      }
    } catch (error) {
      console.error('Erro ao fazer logout:', error);
    } finally {
      // Limpar dados locais independente do resultado
      localStorage.removeItem('token');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('user');
      localStorage.removeItem('selectedCompanyId');
    }
  }

  /**
   * Renova o access token usando o refresh token
   */
  async refreshToken(): Promise<AuthResponse | null> {
    try {
      const refreshToken = localStorage.getItem('refreshToken');
      if (!refreshToken) return null;
      
      const response = await api.post<{ data: AuthResponse }>('/auth/refresh-token', JSON.stringify(refreshToken), {
        headers: { 'Content-Type': 'application/json' }
      });
      const authData = response.data.data;
      
      // Atualizar tokens
      localStorage.setItem('token', authData.token);
      localStorage.setItem('refreshToken', authData.refreshToken);
      
      return authData;
    } catch (error) {
      console.error('Erro ao renovar token:', error);
      return null;
    }
  }

  /**
   * Obtém o token armazenado
   */
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  /**
   * Obtém os dados do usuário armazenados
   */
  getUser(): { userId: number; email?: string; phone?: string; cpf?: string } | null {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      try {
        return JSON.parse(userStr);
      } catch {
        return null;
      }
    }
    return null;
  }

  /**
   * Verifica se o usuário está autenticado
   */
  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  /**
   * Atualiza os dados do usuário no localStorage
   */
  updateUser(user: { userId: number; email?: string; phone?: string; cpf?: string }): void {
    localStorage.setItem('user', JSON.stringify(user));
  }
}

export const authService = new AuthService();
export default authService;
