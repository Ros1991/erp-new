namespace ERP.WebApi.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Obtém o ID do usuário autenticado do contexto HTTP
        /// </summary>
        public static long GetUserId(this HttpContext context)
        {
            if (context.Items.TryGetValue("UserId", out var userId) && userId is long id)
            {
                return id;
            }

            // Fallback: tentar obter do ClaimsPrincipal
            var userIdClaim = context.User.FindFirst("UserId")?.Value;
            if (long.TryParse(userIdClaim, out var userIdFromClaim))
            {
                return userIdFromClaim;
            }

            return 0;
        }

        /// <summary>
        /// Obtém o email do usuário autenticado do contexto HTTP
        /// </summary>
        public static string? GetUserEmail(this HttpContext context)
        {
            if (context.Items.TryGetValue("Email", out var email) && email is string emailStr)
            {
                return emailStr;
            }

            return context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Obtém o telefone do usuário autenticado do contexto HTTP
        /// </summary>
        public static string? GetUserPhone(this HttpContext context)
        {
            if (context.Items.TryGetValue("Phone", out var phone) && phone is string phoneStr)
            {
                return phoneStr;
            }

            return context.User.FindFirst(System.Security.Claims.ClaimTypes.MobilePhone)?.Value;
        }

        /// <summary>
        /// Obtém o CPF do usuário autenticado do contexto HTTP
        /// </summary>
        public static string? GetUserCpf(this HttpContext context)
        {
            if (context.Items.TryGetValue("Cpf", out var cpf) && cpf is string cpfStr)
            {
                return cpfStr;
            }

            return context.User.FindFirst("Cpf")?.Value;
        }

        /// <summary>
        /// Verifica se o usuário está autenticado
        /// </summary>
        public static bool IsAuthenticated(this HttpContext context)
        {
            return context.GetUserId() > 0 || context.User.Identity?.IsAuthenticated == true;
        }

        /// <summary>
        /// Obtém o CompanyId do contexto da requisição (multi-tenant)
        /// </summary>
        public static long GetCompanyId(this HttpContext context)
        {
            if (context.Items.TryGetValue("CompanyId", out var companyId) && companyId is long id)
            {
                return id;
            }

            return 0;
        }

        /// <summary>
        /// Define o CompanyId no contexto da requisição
        /// </summary>
        public static void SetCompanyId(this HttpContext context, long companyId)
        {
            context.Items["CompanyId"] = companyId;
        }

        /// <summary>
        /// Verifica se existe um CompanyId no contexto
        /// </summary>
        public static bool HasCompanyContext(this HttpContext context)
        {
            return context.GetCompanyId() > 0;
        }

        /// <summary>
        /// Verifica se o usuário tem uma permissão específica (usa cache do RequirePermissionsAttribute)
        /// </summary>
        public static bool HasPermission(this HttpContext context, string permission)
        {
            const string CacheKey = "UserPermissions";
            
            if (!context.Items.TryGetValue(CacheKey, out var cached))
            {
                return false;
            }

            var (isAdmin, isSystemRole, modules) = ((bool, bool, Dictionary<string, Dictionary<string, bool>>))cached;

            // Admin/System tem todas as permissões
            if (isAdmin || isSystemRole)
            {
                return true;
            }

            // Parse: "module.permission"
            var parts = permission.Split('.', 2);
            if (parts.Length != 2)
            {
                return false;
            }

            string moduleName = parts[0];
            string permissionName = parts[1];

            if (!modules.TryGetValue(moduleName, out var modulePermissions))
            {
                return false;
            }

            return modulePermissions.TryGetValue(permissionName, out bool hasPermission) && hasPermission;
        }
    }
}
