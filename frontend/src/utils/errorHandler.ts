/**
 * Estrutura de resposta do backend (BaseResponse)
 */
interface BackendResponse {
  data?: any;
  code: number;
  message: string;
  errors?: Record<string, string[]>;
}

/**
 * Parseia erros de validação do backend e retorna um objeto com os erros
 */
export function parseBackendError(error: any): { 
  hasValidationErrors: boolean;
  validationErrors?: Record<string, string[]>;
  message: string;
} {
  // Erro de rede ou sem resposta
  if (!error.response) {
    return {
      hasValidationErrors: false,
      message: 'Erro de conexão. Verifique sua internet e tente novamente.',
    };
  }

  const response: BackendResponse = error.response.data;

  // Erros de validação (errors na raiz do BaseResponse)
  if (response.errors && Object.keys(response.errors).length > 0) {
    return {
      hasValidationErrors: true,
      validationErrors: response.errors,
      message: response.message || 'Erro de validação',
    };
  }

  // Erro com mensagem específica
  if (response.message) {
    return {
      hasValidationErrors: false,
      message: response.message,
    };
  }

  // Status específicos (fallback)
  switch (error.response.status) {
    case 400:
      return {
        hasValidationErrors: false,
        message: 'Dados inválidos. Verifique as informações e tente novamente.',
      };
    case 401:
      return {
        hasValidationErrors: false,
        message: 'Não autorizado. Verifique suas credenciais.',
      };
    case 403:
      return {
        hasValidationErrors: false,
        message: 'Acesso negado.',
      };
    case 404:
      return {
        hasValidationErrors: false,
        message: 'Recurso não encontrado.',
      };
    case 500:
      return {
        hasValidationErrors: false,
        message: 'Erro interno do servidor. Tente novamente mais tarde.',
      };
    default:
      return {
        hasValidationErrors: false,
        message: 'Ocorreu um erro inesperado. Tente novamente.',
      };
  }
}

/**
 * Extrai a primeira mensagem de erro de validação
 */
export function getFirstValidationError(errors: Record<string, string[]>): string {
  const firstField = Object.keys(errors)[0];
  const firstError = errors[firstField][0];
  return firstError;
}
