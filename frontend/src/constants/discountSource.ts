/**
 * Códigos curtos para fonte de desconto de empréstimos/adiantamentos
 * Máximo 10 caracteres para caber no varchar(10) do banco
 */

export const DiscountSourceCode = {
  ALL: 'TODOS',
  SALARY: 'SALARIO',
  THIRTEENTH_SALARY: '13SAL',
  VACATION: 'FERIAS',
  ANNUAL: 'ANUAL',           // Mantido para compatibilidade mas não exibido
  BONUS: 'BONUS',            // Mantido para compatibilidade mas não exibido
  COMMISSION: 'COMISSAO',    // Mantido para compatibilidade mas não exibido
} as const;

export type DiscountSourceCodeType = typeof DiscountSourceCode[keyof typeof DiscountSourceCode];

export const DISCOUNT_SOURCE_OPTIONS = [
  { value: DiscountSourceCode.ALL, label: 'Todos' },
  { value: DiscountSourceCode.SALARY, label: 'Salário' },
  { value: DiscountSourceCode.THIRTEENTH_SALARY, label: 'Décimo Terceiro' },
  { value: DiscountSourceCode.VACATION, label: 'Férias' },
  // ANNUAL, BONUS e COMMISSION não são exibidos na combo mas são mantidos para compatibilidade
];

/**
 * Retorna a descrição amigável do código
 */
export function getDiscountSourceLabel(code: string): string {
  const option = DISCOUNT_SOURCE_OPTIONS.find(opt => opt.value === code);
  return option?.label || code;
}

/**
 * Converte valores antigos para novos códigos (para compatibilidade)
 */
export function migrateDiscountSourceValue(oldValue: string): string {
  const normalized = oldValue.toLowerCase().trim();
  
  if (normalized.includes('todos') || normalized.includes('tudo')) {
    return DiscountSourceCode.ALL;
  }
  if (normalized.includes('salário') || normalized.includes('salario') || normalized === 'mensal') {
    return DiscountSourceCode.SALARY;
  }
  if (normalized.includes('décimo') || normalized.includes('decimo') || normalized.includes('13')) {
    return DiscountSourceCode.THIRTEENTH_SALARY;
  }
  if (normalized.includes('férias') || normalized.includes('ferias')) {
    return DiscountSourceCode.VACATION;
  }
  if (normalized.includes('anual')) {
    return DiscountSourceCode.ANNUAL;
  }
  if (normalized.includes('bônus') || normalized.includes('bonus')) {
    return DiscountSourceCode.BONUS;
  }
  if (normalized.includes('comissão') || normalized.includes('comissao')) {
    return DiscountSourceCode.COMMISSION;
  }
  
  // Se já for um código válido, retorna
  if (Object.values(DiscountSourceCode).includes(oldValue as DiscountSourceCodeType)) {
    return oldValue;
  }
  
  // Default
  return DiscountSourceCode.ALL;
}
