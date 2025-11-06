using ERP.CrossCutting.Exceptions;

namespace ERP.WebApi.Middlewares
{
    public class CompanyContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CompanyContextMiddleware> _logger;

        public CompanyContextMiddleware(RequestDelegate next, ILogger<CompanyContextMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // ✅ Endpoints que NÃO precisam de CompanyId
            var excludedPaths = new[]
            {
                "/api/auth/",
                "/api/company/",
                "/swagger",
                "/health"
            };

            var isExcluded = excludedPaths.Any(p => path.StartsWith(p));

            if (!isExcluded && context.User.Identity?.IsAuthenticated == true)
            {
                // ✅ Obtém CompanyId do header X-Company-Id
                var companyIdHeader = context.Request.Headers["X-Company-Id"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(companyIdHeader) || !long.TryParse(companyIdHeader, out var companyId))
                {
                    _logger.LogWarning("CompanyId não fornecido ou inválido no header para endpoint: {Path}", path);
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        code = 400,
                        message = "CompanyId é obrigatório. Envie via header 'X-Company-Id'.",
                        data = (object)null
                    });
                    return;
                }

                if (companyId <= 0)
                {
                    _logger.LogWarning("CompanyId inválido (deve ser > 0) para endpoint: {Path}", path);
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        code = 400,
                        message = "CompanyId deve ser maior que zero.",
                        data = (object)null
                    });
                    return;
                }

                // TODO: Validar se o usuário tem acesso a essa Company
                // var userId = context.GetUserId();
                // var hasAccess = await ValidateUserCompanyAccess(userId, companyId);
                // if (!hasAccess) { return 403; }

                // ✅ Salva CompanyId no contexto
                context.Items["CompanyId"] = companyId;
                
                _logger.LogInformation("CompanyContext definido: CompanyId={CompanyId}, Path={Path}", companyId, path);
            }

            await _next(context);
        }
    }
}
