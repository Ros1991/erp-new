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
                userId,        // CriadoPor
                userId,        // AtualizadoPor
                now,           // CriadoEm
                now            // AtualizadoEm
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
    }
}
