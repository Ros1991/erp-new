import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:8148/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para adicionar o token automaticamente
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    const companyId = localStorage.getItem('selectedCompanyId');
    if (companyId) {
      config.headers['X-Company-Id'] = companyId;
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor para lidar com erros de autenticação e acesso
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Erro 401 - Não autenticado
    if (error.response?.status === 401) {
      localStorage.clear();
      window.location.href = '/login';
      return Promise.reject(error);
    }
    
    // Erro 403 - Sem acesso à empresa
    if (error.response?.status === 403) {
      // Remove apenas o companyId, mantém o token
      localStorage.removeItem('selectedCompanyId');
      localStorage.removeItem('selectedCompanyName');
      window.location.href = '/select-company';
      return Promise.reject(error);
    }
    
    // Mensagem específica do backend indicando falta de acesso
    const message = error.response?.data?.message || '';
    if (
      message.toLowerCase().includes('não tem acesso') ||
      message.toLowerCase().includes('sem acesso') ||
      message.toLowerCase().includes('acesso negado')
    ) {
      localStorage.removeItem('selectedCompanyId');
      localStorage.removeItem('selectedCompanyName');
      window.location.href = '/select-company';
      return Promise.reject(error);
    }
    
    return Promise.reject(error);
  }
);

export default api;
