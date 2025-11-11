/**
 * Utilitário para gerenciar funcionalidade "Lembrar de mim"
 * 
 * NOTA DE SEGURANÇA:
 * Armazenar senha em localStorage não é a prática mais segura.
 * Em produção, considere:
 * - Usar apenas o credential e gerar um token específico de "lembrar"
 * - Implementar OAuth/SSO
 * - Usar cookies HttpOnly com tokens de sessão longos
 * - Criptografar dados antes de armazenar (embora localStorage ainda seja vulnerável a XSS)
 */

const STORAGE_KEYS = {
  CREDENTIAL: 'remember_credential',
  PASSWORD: 'remember_password',
  REMEMBER_ME: 'remember_me'
} as const;

export interface RememberMeData {
  credential: string;
  password: string;
}

/**
 * Salva as credenciais no localStorage
 */
export const saveRememberMeData = (credential: string, password: string): void => {
  localStorage.setItem(STORAGE_KEYS.CREDENTIAL, credential);
  localStorage.setItem(STORAGE_KEYS.PASSWORD, password);
  localStorage.setItem(STORAGE_KEYS.REMEMBER_ME, 'true');
};

/**
 * Carrega as credenciais salvas do localStorage
 */
export const loadRememberMeData = (): RememberMeData | null => {
  const rememberMe = localStorage.getItem(STORAGE_KEYS.REMEMBER_ME) === 'true';
  
  if (!rememberMe) {
    return null;
  }

  const credential = localStorage.getItem(STORAGE_KEYS.CREDENTIAL);
  const password = localStorage.getItem(STORAGE_KEYS.PASSWORD);

  if (credential && password) {
    return { credential, password };
  }

  return null;
};

/**
 * Verifica se o usuário marcou "Lembrar de mim"
 */
export const isRememberMeEnabled = (): boolean => {
  return localStorage.getItem(STORAGE_KEYS.REMEMBER_ME) === 'true';
};

/**
 * Limpa todos os dados de "Lembrar de mim"
 */
export const clearRememberMeData = (): void => {
  localStorage.removeItem(STORAGE_KEYS.CREDENTIAL);
  localStorage.removeItem(STORAGE_KEYS.PASSWORD);
  localStorage.removeItem(STORAGE_KEYS.REMEMBER_ME);
};
