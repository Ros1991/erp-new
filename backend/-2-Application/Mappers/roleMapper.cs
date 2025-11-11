using System.Text.Json;
using ERP.Application.DTOs;
using ERP.Domain.Entities;
using ERP.Domain.Models;

namespace ERP.Application.Mappers
{
    public static class RoleMapper
    {
        public static RoleOutputDTO ToRoleOutputDTO(Role entity)
        {
            if (entity == null) return null;

            return new RoleOutputDTO
            {
                RoleId = entity.RoleId,
                CompanyId = entity.CompanyId,
                Name = entity.Name,
                Permissions = DeserializePermissions(entity.Permissions),
                IsSystem = entity.IsSystem,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<RoleOutputDTO> ToRoleOutputDTOList(IEnumerable<Role> entities)
        {
            return entities?.Select(ToRoleOutputDTO).ToList() ?? new List<RoleOutputDTO>();
        }

        public static Role ToEntity(RoleInputDTO dto, long companyId, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new Role(
                companyId,
                dto.Name,
                SerializePermissions(dto.Permissions),
                false,         // IsSystem (false por padrão)
                userId,        // CriadoPor
                null,          // AtualizadoPor
                now,           // CriadoEm
                null           // AtualizadoEm
            );
        }

        public static void UpdateEntity(Role entity, RoleInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.Name = dto.Name;
            entity.Permissions = SerializePermissions(dto.Permissions);
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }

        /// <summary>
        /// Serializa permissões para JSON
        /// </summary>
        private static string SerializePermissions(RolePermissions permissions)
        {
            if (permissions == null) return "{}";
            return JsonSerializer.Serialize(permissions, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
        }

        /// <summary>
        /// Deserializa permissões de JSON
        /// </summary>
        private static RolePermissions DeserializePermissions(string permissionsJson)
        {
            if (string.IsNullOrWhiteSpace(permissionsJson)) return new RolePermissions();
            
            try
            {
                return JsonSerializer.Deserialize<RolePermissions>(permissionsJson, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                }) ?? new RolePermissions();
            }
            catch
            {
                return new RolePermissions();
            }
        }

        /// <summary>
        /// Cria role de Owner (Dono) com permissões totais para uma empresa
        /// </summary>
        public static Role CreateOwnerRole(long companyId, long userId)
        {
            var now = DateTime.UtcNow;
            var ownerPermissions = CreateOwnerPermissions();

            return new Role(
                companyId,
                "Dono",
                SerializePermissions(ownerPermissions),
                true,          // IsSystem = true (não pode ser editada/deletada)
                userId,        // CriadoPor
                null,          // AtualizadoPor
                now,           // CriadoEm
                null           // AtualizadoEm
            );
        }

        /// <summary>
        /// Cria permissões de Owner (acesso total)
        /// </summary>
        private static RolePermissions CreateOwnerPermissions()
        {
            return new RolePermissions
            {
                IsAdmin = true,
                AllowedEndpoints = new List<string> { "*" }, // Todos os endpoints
                Modules = new Dictionary<string, ModulePermissions>
                {
                    { Modules.Company, new ModulePermissions { CanView = true, CanCreate = true, CanEdit = true, CanDelete = true, CanExport = true } },
                    { Modules.Account, new ModulePermissions { CanView = true, CanCreate = true, CanEdit = true, CanDelete = true, CanExport = true } },
                    { Modules.User, new ModulePermissions { CanView = true, CanCreate = true, CanEdit = true, CanDelete = true, CanExport = true } },
                    { Modules.Role, new ModulePermissions { CanView = true, CanCreate = true, CanEdit = true, CanDelete = true, CanExport = true } },
                    { Modules.Product, new ModulePermissions { CanView = true, CanCreate = true, CanEdit = true, CanDelete = true, CanExport = true } },
                    { Modules.Order, new ModulePermissions { CanView = true, CanCreate = true, CanEdit = true, CanDelete = true, CanExport = true } },
                    { Modules.Financial, new ModulePermissions { CanView = true, CanCreate = true, CanEdit = true, CanDelete = true, CanExport = true } },
                    { Modules.Report, new ModulePermissions { CanView = true, CanCreate = true, CanEdit = true, CanDelete = true, CanExport = true } }
                }
            };
        }
    }
}
