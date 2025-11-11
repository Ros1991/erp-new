using System;

namespace ERP.CrossCutting.Helpers
{
    /// <summary>
    /// Helper para garantir uso consistente de UTC em toda aplicação
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Retorna a data/hora atual em UTC
        /// Use SEMPRE este método ao invés de DateTime.Now
        /// </summary>
        public static DateTime UtcNow => DateTime.UtcNow;

        /// <summary>
        /// Converte DateTime para UTC se necessário
        /// </summary>
        public static DateTime ToUtc(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;
            
            if (dateTime.Kind == DateTimeKind.Local)
                return dateTime.ToUniversalTime();
            
            // Se Unspecified, assume que já está em UTC
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        /// <summary>
        /// Converte UTC para timezone específico
        /// Usar apenas para exibição no frontend
        /// </summary>
        public static DateTime ConvertFromUtc(DateTime utcDateTime, string timeZoneId)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
                utcDateTime = ToUtc(utcDateTime);

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
        }

        /// <summary>
        /// Converte UTC para timezone de Brasília (America/Sao_Paulo)
        /// </summary>
        public static DateTime ConvertToSaoPauloTime(DateTime utcDateTime)
        {
            return ConvertFromUtc(utcDateTime, "E. South America Standard Time");
        }

        /// <summary>
        /// Verifica se data já expirou (comparação em UTC)
        /// </summary>
        public static bool IsExpired(DateTime? expirationDate)
        {
            if (!expirationDate.HasValue)
                return true;

            var utcExpiration = ToUtc(expirationDate.Value);
            return utcExpiration < UtcNow;
        }

        /// <summary>
        /// Verifica se data ainda é válida (comparação em UTC)
        /// </summary>
        public static bool IsValid(DateTime? expirationDate)
        {
            return !IsExpired(expirationDate);
        }
    }
}
