/**
 * Utilitários de formatação para campos de formulário
 */

/**
 * Remove todos os caracteres não numéricos de uma string
 */
export function removeNonNumeric(value: string): string {
  return value.replace(/\D/g, '');
}

/**
 * Formata um CPF (000.000.000-00)
 */
export function formatCpf(value: string): string {
  const numbers = removeNonNumeric(value);
  
  if (numbers.length <= 11) {
    return numbers
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
  }
  
  return value;
}

/**
 * Formata um telefone brasileiro ((00) 00000-0000)
 */
export function formatPhone(value: string): string {
  const numbers = removeNonNumeric(value);
  
  if (numbers.length <= 11) {
    return numbers
      .replace(/(\d{2})(\d)/, '($1) $2')
      .replace(/(\d{5})(\d)/, '$1-$2');
  }
  
  return value;
}

/**
 * Remove formatação de telefone ou CPF antes de enviar ao backend
 * Se for email (contém @), mantém como está
 */
export function cleanCredential(credential: string): string {
  if (credential.includes('@')) {
    return credential; // É email, mantém como está
  }
  return removeNonNumeric(credential); // É telefone ou CPF, remove formatação
}

/**
 * Valida se um CPF tem 11 dígitos (após remover formatação)
 */
export function isValidCpfLength(cpf: string): boolean {
  const numbers = removeNonNumeric(cpf);
  return numbers.length === 11;
}

/**
 * Valida se um telefone tem 10 ou 11 dígitos (após remover formatação)
 */
export function isValidPhoneLength(phone: string): boolean {
  const numbers = removeNonNumeric(phone);
  return numbers.length === 10 || numbers.length === 11;
}
