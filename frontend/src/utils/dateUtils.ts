/**
 * Utilitários para manipulação de datas com suporte a timezone
 * 
 * REGRA GERAL:
 * - Backend sempre envia/recebe datas em UTC (ISO 8601)
 * - Frontend converte para timezone local apenas para exibição
 * - Ao enviar para backend, sempre converter para UTC
 */

/**
 * Converte string UTC do backend para Date local
 * Backend retorna: "2025-11-11T17:00:00Z"
 * JavaScript converte automaticamente para local
 */
export function parseUTCDate(utcString: string | null | undefined): Date | null {
  if (!utcString) return null;
  return new Date(utcString);
}

/**
 * Formata data/hora para exibição local (pt-BR, America/Sao_Paulo)
 * Exemplo: "11/11/2025, 14:30"
 */
export function formatLocalDateTime(date: Date | string | null | undefined): string {
  if (!date) return '-';
  const d = typeof date === 'string' ? parseUTCDate(date) : date;
  if (!d) return '-';
  
  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'short',
    timeStyle: 'short',
    timeZone: 'America/Sao_Paulo'
  }).format(d);
}

/**
 * Formata apenas a data (sem hora) para exibição local
 * Exemplo: "11/11/2025"
 */
export function formatLocalDate(date: Date | string | null | undefined): string {
  if (!date) return '-';
  const d = typeof date === 'string' ? parseUTCDate(date) : date;
  if (!d) return '-';
  
  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'short',
    timeZone: 'America/Sao_Paulo'
  }).format(d);
}

/**
 * Formata apenas a hora para exibição local
 * Exemplo: "14:30"
 */
export function formatLocalTime(date: Date | string | null | undefined): string {
  if (!date) return '-';
  const d = typeof date === 'string' ? parseUTCDate(date) : date;
  if (!d) return '-';
  
  return new Intl.DateTimeFormat('pt-BR', {
    timeStyle: 'short',
    timeZone: 'America/Sao_Paulo'
  }).format(d);
}

/**
 * Formata data/hora completa (formato longo)
 * Exemplo: "11 de novembro de 2025, 14:30"
 */
export function formatLocalDateTimeLong(date: Date | string | null | undefined): string {
  if (!date) return '-';
  const d = typeof date === 'string' ? parseUTCDate(date) : date;
  if (!d) return '-';
  
  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'long',
    timeStyle: 'short',
    timeZone: 'America/Sao_Paulo'
  }).format(d);
}

/**
 * Converte Date local para ISO UTC string (para enviar ao backend)
 * Input: Date do usuário (ex: selecionado em um DatePicker)
 * Output: "2025-11-11T17:00:00.000Z" (UTC)
 */
export function toUTCString(date: Date | null | undefined): string | null {
  if (!date) return null;
  return date.toISOString();
}

/**
 * Verifica se data já expirou (comparação em tempo real)
 */
export function isExpired(expirationDate: Date | string | null | undefined): boolean {
  if (!expirationDate) return true;
  const d = typeof expirationDate === 'string' ? parseUTCDate(expirationDate) : expirationDate;
  if (!d) return true;
  return d < new Date();
}

/**
 * Verifica se data ainda é válida
 */
export function isValid(expirationDate: Date | string | null | undefined): boolean {
  return !isExpired(expirationDate);
}

/**
 * Calcula diferença em tempo legível (ex: "2 horas", "5 minutos")
 */
export function getTimeUntil(targetDate: Date | string | null | undefined): string {
  if (!targetDate) return 'Data inválida';
  const d = typeof targetDate === 'string' ? parseUTCDate(targetDate) : targetDate;
  if (!d) return 'Data inválida';
  
  const now = new Date();
  const diffMs = d.getTime() - now.getTime();
  
  if (diffMs < 0) return 'Expirado';
  
  const diffMinutes = Math.floor(diffMs / 60000);
  const diffHours = Math.floor(diffMinutes / 60);
  const diffDays = Math.floor(diffHours / 24);
  
  if (diffDays > 0) return `${diffDays} dia${diffDays > 1 ? 's' : ''}`;
  if (diffHours > 0) return `${diffHours} hora${diffHours > 1 ? 's' : ''}`;
  if (diffMinutes > 0) return `${diffMinutes} minuto${diffMinutes > 1 ? 's' : ''}`;
  return 'Menos de 1 minuto';
}

/**
 * Formata data relativa (ex: "há 2 horas", "em 3 dias")
 */
export function formatRelativeTime(date: Date | string | null | undefined): string {
  if (!date) return '-';
  const d = typeof date === 'string' ? parseUTCDate(date) : date;
  if (!d) return '-';
  
  const now = new Date();
  const diffMs = d.getTime() - now.getTime();
  const isPast = diffMs < 0;
  const absDiffMs = Math.abs(diffMs);
  
  const seconds = Math.floor(absDiffMs / 1000);
  const minutes = Math.floor(seconds / 60);
  const hours = Math.floor(minutes / 60);
  const days = Math.floor(hours / 24);
  const months = Math.floor(days / 30);
  const years = Math.floor(days / 365);
  
  let result = '';
  
  if (years > 0) {
    result = `${years} ano${years > 1 ? 's' : ''}`;
  } else if (months > 0) {
    result = `${months} ${months > 1 ? 'meses' : 'mês'}`;
  } else if (days > 0) {
    result = `${days} dia${days > 1 ? 's' : ''}`;
  } else if (hours > 0) {
    result = `${hours} hora${hours > 1 ? 's' : ''}`;
  } else if (minutes > 0) {
    result = `${minutes} minuto${minutes > 1 ? 's' : ''}`;
  } else {
    result = 'agora mesmo';
    return result;
  }
  
  return isPast ? `há ${result}` : `em ${result}`;
}

/**
 * Adiciona dias a uma data
 */
export function addDays(date: Date, days: number): Date {
  const result = new Date(date);
  result.setDate(result.getDate() + days);
  return result;
}

/**
 * Adiciona horas a uma data
 */
export function addHours(date: Date, hours: number): Date {
  const result = new Date(date);
  result.setHours(result.getHours() + hours);
  return result;
}

/**
 * Obtém o início do dia (00:00:00) em timezone local
 */
export function startOfDay(date: Date): Date {
  const result = new Date(date);
  result.setHours(0, 0, 0, 0);
  return result;
}

/**
 * Obtém o fim do dia (23:59:59) em timezone local
 */
export function endOfDay(date: Date): Date {
  const result = new Date(date);
  result.setHours(23, 59, 59, 999);
  return result;
}

/**
 * Verifica se duas datas são do mesmo dia
 */
export function isSameDay(date1: Date | string, date2: Date | string): boolean {
  const d1 = typeof date1 === 'string' ? parseUTCDate(date1) : date1;
  const d2 = typeof date2 === 'string' ? parseUTCDate(date2) : date2;
  
  if (!d1 || !d2) return false;
  
  return (
    d1.getFullYear() === d2.getFullYear() &&
    d1.getMonth() === d2.getMonth() &&
    d1.getDate() === d2.getDate()
  );
}

/**
 * Valida se string é uma data válida
 */
export function isValidDate(dateString: string): boolean {
  const date = new Date(dateString);
  return date instanceof Date && !isNaN(date.getTime());
}

/**
 * Formata duração em segundos para formato legível
 * Exemplo: 3665 → "1h 1m 5s"
 */
export function formatDuration(seconds: number): string {
  const hours = Math.floor(seconds / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  const secs = seconds % 60;
  
  const parts: string[] = [];
  if (hours > 0) parts.push(`${hours}h`);
  if (minutes > 0) parts.push(`${minutes}m`);
  if (secs > 0 || parts.length === 0) parts.push(`${secs}s`);
  
  return parts.join(' ');
}
