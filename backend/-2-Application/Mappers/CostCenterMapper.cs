using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class CostCenterMapper
    {
        public static CostCenterOutputDTO ToCostCenterOutputDTO(CostCenter entity)
        {
            if (entity == null) return null;

            return new CostCenterOutputDTO
            {
                CostCenterId = entity.CostCenterId,
                CompanyId = entity.CompanyId,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<CostCenterOutputDTO> ToCostCenterOutputDTOList(List<CostCenter> entities)
        {
            if (entities == null) return new List<CostCenterOutputDTO>();
            return entities.Select(ToCostCenterOutputDTO).ToList();
        }

        public static CostCenter ToEntity(CostCenterInputDTO dto, long companyId, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new CostCenter(
                companyId,
                dto.Name,
                dto.Description,
                dto.IsActive,
                userId,        // CriadoPor
                null,          // AtualizadoPor
                now,           // CriadoEm
                null           // AtualizadoEm
            );
        }

        public static void UpdateEntity(CostCenter entity, CostCenterInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
            // ✅ CriadoPor e CriadoEm NÃO são alterados
        }
    }
}
