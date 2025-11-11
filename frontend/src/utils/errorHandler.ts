/**
 * Estrutura de erro retornada pelo backend
 */
interface BackendError {
  type?: string;
  title?: string;
  status?: number;
  errors?: Record<string, string[]>;
  traceId?: string;
  message?: string;
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

  const data: BackendError = error.response.data;

  // Erros de validação (400 Bad Request com errors)
  if (data.errors && Object.keys(data.errors).length > 0) {
    return {
      hasValidationErrors: true,
      validationErrors: data.errors,
      message: data.title || 'Erro de validação',
    };
  }

  // Erro com mensagem específica
  if (data.message) {
    return {
      hasValidationErrors: false,
      message: data.message,
    };
  }

  // Erro com title
  if (data.title) {
    return {
      hasValidationErrors: false,
      message: data.title,
    };
  }

  // Status específicos
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
