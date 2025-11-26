/**
 * Códigos curtos para tipo de aplicação de benefícios/descontos
 * Máximo 10 caracteres para caber no varchar(10) do banco
 */

export const ApplicationTypeCode = {
  SALARY: 'SALARIO',
  THIRTEENTH_SALARY: '13SAL',
  VACATION: 'FERIAS',
  ANNUAL: 'ANUAL',
  ALL: 'TODOS',
  BONUS: 'BONUS',           // Mantido para compatibilidade mas não exibido
  COMMISSION: 'COMISSAO',   // Mantido para compatibilidade mas não exibido
} as const;

export type ApplicationTypeCodeType = typeof ApplicationTypeCode[keyof typeof ApplicationTypeCode];

export const APPLICATION_TYPE_OPTIONS = [
  { value: ApplicationTypeCode.ALL, label: 'Todos os Pagamentos' },
  { value: ApplicationTypeCode.SALARY, label: 'Salário' },
  { value: ApplicationTypeCode.THIRTEENTH_SALARY, label: 'Décimo Terceiro' },
  { value: ApplicationTypeCode.VACATION, label: 'Férias' },
  { value: ApplicationTypeCode.ANNUAL, label: 'Anual' },
  // BONUS e COMMISSION não são exibidos na combo mas são mantidos para compatibilidade
];

/**
 * Retorna a descrição amigável do código
 */
export function getApplicationTypeLabel(code: string): string {
  const option = APPLICATION_TYPE_OPTIONS.find(opt => opt.value === code);
  return option?.label || code;
}

/**
 * Converte valores antigos para novos códigos (para compatibilidade)
 */
export function migrateApplicationTypeValue(oldValue: string): string {
  const normalized = oldValue.toLowerCase().trim();
  
  if (normalized.includes('anual')) {
    return ApplicationTypeCode.ANNUAL;
  }
  if (normalized.includes('todos') || normalized === 'tudo' || normalized === 'all') {
    return ApplicationTypeCode.ALL;
  }
  if (normalized.includes('salário') || normalized.includes('salario') || normalized === 'mensal') {
    return ApplicationTypeCode.SALARY;
  }
  if (normalized.includes('décimo') || normalized.includes('decimo') || normalized.includes('13')) {
    return ApplicationTypeCode.THIRTEENTH_SALARY;
  }
  if (normalized.includes('férias') || normalized.includes('ferias')) {
    return ApplicationTypeCode.VACATION;
  }
  if (normalized.includes('bônus') || normalized.includes('bonus')) {
    return ApplicationTypeCode.BONUS;
  }
  if (normalized.includes('comissão') || normalized.includes('comissao')) {
    return ApplicationTypeCode.COMMISSION;
  }
  
  // Se já for um código válido, retorna
  if (Object.values(ApplicationTypeCode).includes(oldValue as ApplicationTypeCodeType)) {
    return oldValue;
  }
  
  // Default
  return ApplicationTypeCode.ALL;
}
