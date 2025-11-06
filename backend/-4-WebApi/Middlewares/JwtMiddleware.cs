using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ERP.WebApi.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await AttachUserToContext(context, token);
            }

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"] ?? "your-very-secure-secret-key-min-32-chars");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"] ?? "ERP.API",
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"] ?? "ERP.Client",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = long.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value);

                // Anexar informações do usuário ao contexto
                context.Items["UserId"] = userId;
                context.Items["Email"] = jwtToken.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
                context.Items["Phone"] = jwtToken.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone")?.Value;
                context.Items["Cpf"] = jwtToken.Claims.FirstOrDefault(x => x.Type == "Cpf")?.Value;

                _logger.LogInformation("Token validado com sucesso para UserId: {UserId}", userId);
            }
            catch (Exception ex)
            {
                // Token inválido - não fazer nada, deixar o [Authorize] lidar com isso
                _logger.LogWarning("Token inválido ou expirado: {Message}", ex.Message);
            }

            await Task.CompletedTask;
        }
    }
}
