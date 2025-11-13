using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class ContractCostCenterMapper
    {
        public static ContractCostCenterOutputDTO ToContractCostCenterOutputDTO(ContractCostCenter entity)
        {
            if (entity == null) return null;

            return new ContractCostCenterOutputDTO
            {
                ContractCostCenterId = entity.ContractCostCenterId,
                ContractId = entity.ContractId,
                CostCenterId = entity.CostCenterId,
                Percentage = entity.Percentage,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<ContractCostCenterOutputDTO> ToContractCostCenterOutputDTOList(List<ContractCostCenter> entities)
        {
            if (entities == null) return new List<ContractCostCenterOutputDTO>();
            return entities.Select(ToContractCostCenterOutputDTO).ToList();
        }

        public static ContractCostCenter ToEntity(ContractCostCenterInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new ContractCostCenter(
                dto.ContractId,
                dto.CostCenterId,
                dto.Percentage,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(ContractCostCenter entity, ContractCostCenterInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.ContractId = dto.ContractId;
            entity.CostCenterId = dto.CostCenterId;
            entity.Percentage = dto.Percentage;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
