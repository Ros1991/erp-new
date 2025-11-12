using System.Text.Json;
using ERP.Application.Interfaces;
using ERP.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace ERP.CrossCutting.Services
{
    public interface IPermissionService
    {
        Task<bool> UserHasAccessToCompanyAsync(long userId, long companyId);
        Task<bool> UserHasPermissionAsync(long userId, long companyId, string module, string action);
        Task<RolePermissions> GetUserPermissionsAsync(long userId, long companyId);
        RolePermissions GetUserPermissions(int userId);
    }

    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PermissionService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionService(
            IUnitOfWork unitOfWork, 
            ILogger<PermissionService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> UserHasAccessToCompanyAsync(long userId, long companyId)
        {
            var hasAccess = await _unitOfWork.CompanyUserRepository.UserHasAccessToCompanyAsync(userId, companyId);
            
            _logger.LogInformation(
                "Validação de acesso: UserId={UserId}, CompanyId={CompanyId}, HasAccess={HasAccess}",
                userId, companyId, hasAccess);

            return hasAccess;
        }

        public async Task<bool> UserHasPermissionAsync(long userId, long companyId, string module, string action)
        {
            // Verificar se o usuário tem role do sistema (IsSystem)
            var role = await _unitOfWork.CompanyUserRepository.GetUserRoleInCompanyAsync(userId, companyId);
            
            if (role != null && role.IsSystem)
            {
                _logger.LogInformation("Usuário {UserId} tem role do sistema (IsSystem=true) - acesso total permitido", userId);
                return true;
            }

            var permissions = await GetUserPermissionsAsync(userId, companyId);

            if (permissions == null)
            {
                _logger.LogWarning("Permissões não encontradas para UserId={UserId}, CompanyId={CompanyId}", userId, companyId);
                return false;
            }

            // Admin tem acesso total
            if (permissions.IsAdmin)
            {
                _logger.LogInformation("Usuário {UserId} é admin - acesso permitido", userId);
                return true;
            }

            // Verifica permissões do módulo
            if (permissions.Modules.TryGetValue(module, out var modulePermissions))
            {
                var hasPermission = action.ToLower() switch
                {
                    Actions.View => modulePermissions.CanView,
                    Actions.Create => modulePermissions.CanCreate,
                    Actions.Edit => modulePermissions.CanEdit,
                    Actions.Delete => modulePermissions.CanDelete,
                    _ => false
                };

                _logger.LogInformation(
                    "Validação de permissão: UserId={UserId}, Module={Module}, Action={Action}, HasPermission={HasPermission}",
                    userId, module, action, hasPermission);

                return hasPermission;
            }

            _logger.LogWarning("Módulo {Module} não encontrado nas permissões do usuário {UserId}", module, userId);
            return false;
        }

        public async Task<RolePermissions?> GetUserPermissionsAsync(long userId, long companyId)
        {
            var companyUser = await _unitOfWork.CompanyUserRepository.GetByUserAndCompanyAsync(userId, companyId);

            if (companyUser == null)
            {
                _logger.LogWarning("Nenhum vínculo encontrado para UserId={UserId}, CompanyId={CompanyId}", userId, companyId);
                return null;
            }

            var role = companyUser.Role;
            
            if (role == null)
            {
                _logger.LogWarning("Role NULL para RoleId={RoleId}", companyUser.RoleId);
                return null;
            }

            // Se for role do sistema (IsSystem=true), retornar permissões de admin total
            if (role.IsSystem)
            {
                _logger.LogInformation("Role do sistema detectada (IsSystem=true) para UserId={UserId}, RoleId={RoleId} - retornando permissões totais", userId, role.RoleId);
                return new RolePermissions
                {
                    IsAdmin = true,
                    IsSystemRole = true,
                    AllowedEndpoints = new List<string> { "*" },
                    Modules = new Dictionary<string, ModulePermissions>()
                };
            }

            try
            {
                var permissions = JsonSerializer.Deserialize<RolePermissions>(role.Permissions, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (permissions != null)
                {
                    permissions.IsSystemRole = role.IsSystem;
                }

                _logger.LogInformation("Permissões carregadas para UserId={UserId}, RoleId={RoleId}", userId, role.RoleId);
                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desserializar permissões da Role {RoleId}", role.RoleId);
                return new RolePermissions();
            }
        }

        /// <summary>
        /// Obtém permissões do usuário usando companyId do HttpContext
        /// Método síncrono para uso em Authorization Filters
        /// </summary>
        public RolePermissions GetUserPermissions(int userId)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            long companyId = 0;
            
            // 1. Tentar pegar do HttpContext.Items (colocado pelo middleware)
            if (httpContext?.Items.TryGetValue("CompanyId", out var companyIdObj) == true)
            {
                companyId = Convert.ToInt64(companyIdObj);
            }
            // 2. Tentar pegar do header X-Company-Id
            else if (httpContext?.Request.Headers.TryGetValue("X-Company-Id", out var companyIdHeader) == true 
                     && long.TryParse(companyIdHeader.FirstOrDefault(), out long headerCompanyId))
            {
                companyId = headerCompanyId;
            }
            // 3. Tentar pegar do token (fallback)
            else
            {
                var companyIdClaim = httpContext?.User.FindFirst("companyId")?.Value;
                if (!long.TryParse(companyIdClaim, out companyId))
                {
                    _logger.LogWarning("CompanyId não encontrado (header, items ou token) para UserId={UserId}", userId);
                    return new RolePermissions();
                }
            }

            // Executa de forma síncrona (não ideal, mas necessário para IAuthorizationFilter)
            return GetUserPermissionsAsync(userId, companyId).GetAwaiter().GetResult();
        }
    }
}
