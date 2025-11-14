using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ERP.CrossCutting.Services;
using System.Security.Claims;
using ERP.Application.DTOs;

namespace ERP.WebApi.Attributes
{
    /// <summary>
    /// Atributo para validar permissões específicas em endpoints
    /// Suporta múltiplas permissões (OR lógico) e hierarquia (role.*)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RequirePermissionsAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;
        private const string CacheKey = "UserPermissions";

        /// <summary>
        /// Define as permissões necessárias (OR lógico)
        /// </summary>
        /// <param name="permissions">Lista de permissões no formato "module.permission" (ex: "role.canView", "user.canEdit")</param>
        public RequirePermissionsAttribute(params string[] permissions)
        {
            _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Se não está autenticado, deixa o [Authorize] padrão lidar
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                return;
            }

            var permissionService = context.HttpContext.RequestServices
                .GetRequiredService<IPermissionService>();

            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<RequirePermissionsAttribute>>();

            var userId = GetUserId(context.HttpContext);
            if (userId == null)
            {
                logger.LogWarning("UserId não encontrado no token JWT");
                context.Result = new ForbidResult();
                return;
            }

            try
            {
                // Cache de permissões na request para evitar múltiplas consultas
                var userPermissions = GetCachedPermissions(context.HttpContext, permissionService, userId.Value);

                // IsAdmin ou IsSystem = bypass total
                if (userPermissions.IsAdmin || userPermissions.IsSystemRole)
                {
                    logger.LogInformation("Acesso permitido: Usuário {UserId} é Admin/System", userId);
                    return;
                }

                // Valida se tem pelo menos UMA das permissões (OR)
                bool hasPermission = _permissions.Any(permission =>
                    HasPermission(userPermissions, permission, logger, userId.Value));

                if (!hasPermission)
                {
                    logger.LogWarning(
                        "Acesso negado: Usuário {UserId} não possui nenhuma das permissões requeridas: {Permissions}",
                        userId, string.Join(", ", _permissions));

                    context.Result = new ObjectResult(new BaseResponse<object>(
                        403,
                        "Você não tem permissão para acessar este recurso"))
                    {
                        StatusCode = 403
                    };
                }
                else
                {
                    logger.LogDebug("Acesso permitido: Usuário {UserId} possui permissão", userId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao validar permissões para usuário {UserId}", userId);
                context.Result = new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Obtém permissões do cache ou busca do serviço
        /// </summary>
        private (bool IsAdmin, bool IsSystemRole, Dictionary<string, Dictionary<string, bool>> Modules) 
            GetCachedPermissions(HttpContext context, IPermissionService service, int userId)
        {
            // Tenta buscar do cache da request
            if (context.Items.TryGetValue(CacheKey, out var cached))
            {
                return ((bool, bool, Dictionary<string, Dictionary<string, bool>>))cached;
            }

            // Busca do serviço e cacheia
            var permissions = service.GetUserPermissions(userId);
            
            var permissionDict = new Dictionary<string, Dictionary<string, bool>>(StringComparer.OrdinalIgnoreCase);
            foreach (var module in permissions.Modules)
            {
                var modulePerms = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
                {
                    ["canView"] = module.Value.CanView,
                    ["canCreate"] = module.Value.CanCreate,
                    ["canEdit"] = module.Value.CanEdit,
                    ["canDelete"] = module.Value.CanDelete
                };
                permissionDict[module.Key] = modulePerms;
            }

            var cacheData = (permissions.IsAdmin, permissions.IsSystemRole, permissionDict);
            context.Items[CacheKey] = cacheData;

            return cacheData;
        }

        /// <summary>
        /// Verifica se o usuário tem uma permissão específica
        /// Suporta hierarquia: "role.*" = todas as permissões do módulo role
        /// </summary>
        private bool HasPermission(
            (bool IsAdmin, bool IsSystemRole, Dictionary<string, Dictionary<string, bool>> Modules) userPerms,
            string requiredPermission,
            ILogger logger,
            int userId)
        {
            // Parse: "role.canView" → module="role", permission="canView"
            var parts = requiredPermission.Split('.', 2);
            if (parts.Length != 2)
            {
                logger.LogWarning("Formato de permissão inválido: {Permission}", requiredPermission);
                return false;
            }

            string module = parts[0];
            string permission = parts[1];

            // Verifica se o módulo existe
            if (!userPerms.Modules.TryGetValue(module, out var modulePermissions))
            {
                logger.LogDebug("Módulo {Module} não encontrado nas permissões do usuário {UserId}", module, userId);
                return false;
            }

            // Hierarquia: role.* = todas as permissões do módulo
            if (permission == "*")
            {
                bool hasAll = modulePermissions.Values.All(p => p);
                logger.LogDebug(
                    "Verificação de hierarquia {Module}.* para usuário {UserId}: {HasAll}",
                    module, userId, hasAll);
                return hasAll;
            }

            // Verifica permissão específica
            if (modulePermissions.TryGetValue(permission, out bool hasPermission))
            {
                logger.LogDebug(
                    "Permissão {Permission} para usuário {UserId}: {HasPermission}",
                    requiredPermission, userId, hasPermission);
                return hasPermission;
            }

            logger.LogDebug(
                "Permissão {Permission} não encontrada no módulo {Module} para usuário {UserId}",
                permission, module, userId);
            return false;
        }

        /// <summary>
        /// Extrai UserId do token JWT
        /// </summary>
        private int? GetUserId(HttpContext context)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? context.User.FindFirst("userId")?.Value
                           ?? context.User.FindFirst("sub")?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }
    }

    /// <summary>
    /// Extensão do RolePermissions para incluir flag IsSystemRole
    /// </summary>
    public static class RolePermissionsExtensions
    {
        public static bool IsSystemRole { get; set; } = false;
    }
}
