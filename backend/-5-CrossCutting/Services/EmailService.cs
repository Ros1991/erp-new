using Microsoft.Extensions.Logging;

namespace ERP.CrossCutting.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string resetToken);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            // TODO: Implementar envio de email real (SendGrid, SMTP, etc)
            
            _logger.LogInformation(
                "===== EMAIL DE RESET DE SENHA =====\n" +
                "Para: {Email}\n" +
                "Token de Reset: {ResetToken}\n" +
                "Link: /auth/reset-password?token={ResetToken}\n" +
                "Expira em: 1 hora\n" +
                "====================================",
                email, resetToken, resetToken);

            await Task.CompletedTask;
        }
    }
}
