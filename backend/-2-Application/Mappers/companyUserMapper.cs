using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class CompanyUserMapper
    {
        public static CompanyUserOutputDTO ToCompanyUserOutputDTO(CompanyUser entity)
        {
            if (entity == null) return null;

            return new CompanyUserOutputDTO
            {
                CompanyUserId = entity.CompanyUserId,
                CompanyId = entity.CompanyId,
                UserId = entity.UserId,
                RoleId = entity.RoleId,
                UserEmail = entity.User?.Email,
                RoleName = entity.Role?.Name,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<CompanyUserOutputDTO> ToCompanyUserOutputDTOList(IEnumerable<CompanyUser> entities)
        {
            return entities?.Select(ToCompanyUserOutputDTO).ToList() ?? new List<CompanyUserOutputDTO>();
        }

        public static CompanyUser ToEntity(CompanyUserInputDTO dto, long companyId, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new CompanyUser(
                companyId,
                dto.UserId,
                dto.RoleId,
                userId,        // CriadoPor
                userId,        // AtualizadoPor
                now,           // CriadoEm
                now            // AtualizadoEm
            );
        }

        public static void UpdateEntity(CompanyUser entity, CompanyUserInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.UserId = dto.UserId;
            entity.RoleId = dto.RoleId;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
