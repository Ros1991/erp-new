using System.Text.Json;
using ERP.Application.Interfaces;
using ERP.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ERP.CrossCutting.Services
{
    public interface IPermissionService
    {
        Task<bool> UserHasAccessToCompanyAsync(long userId, long companyId);
        Task<bool> UserHasPermissionAsync(long userId, long companyId, string module, string action);
        Task<RolePermissions> GetUserPermissionsAsync(long userId, long companyId);
    }

    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(IUnitOfWork unitOfWork, ILogger<PermissionService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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
                    Actions.Export => modulePermissions.CanExport,
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

        public async Task<RolePermissions> GetUserPermissionsAsync(long userId, long companyId)
        {
            var role = await _unitOfWork.CompanyUserRepository.GetUserRoleInCompanyAsync(userId, companyId);

            if (role == null)
            {
                _logger.LogWarning("Role não encontrada para UserId={UserId}, CompanyId={CompanyId}", userId, companyId);
                return null;
            }

            try
            {
                var permissions = JsonSerializer.Deserialize<RolePermissions>(role.Permissions, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                _logger.LogInformation("Permissões carregadas para UserId={UserId}, RoleId={RoleId}", userId, role.RoleId);
                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desserializar permissões da Role {RoleId}", role.RoleId);
                return new RolePermissions();
            }
        }
    }
}
