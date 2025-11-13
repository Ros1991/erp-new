using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class ContractBenefitDiscountMapper
    {
        public static ContractBenefitDiscountOutputDTO ToContractBenefitDiscountOutputDTO(ContractBenefitDiscount entity)
        {
            if (entity == null) return null;

            return new ContractBenefitDiscountOutputDTO
            {
                ContractBenefitDiscountId = entity.ContractBenefitDiscountId,
                ContractId = entity.ContractId,
                Description = entity.Description,
                Type = entity.Type,
                Application = entity.Application,
                Amount = entity.Amount,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<ContractBenefitDiscountOutputDTO> ToContractBenefitDiscountOutputDTOList(List<ContractBenefitDiscount> entities)
        {
            if (entities == null) return new List<ContractBenefitDiscountOutputDTO>();
            return entities.Select(ToContractBenefitDiscountOutputDTO).ToList();
        }

        public static ContractBenefitDiscount ToEntity(ContractBenefitDiscountInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new ContractBenefitDiscount(
                dto.ContractId,
                dto.Description,
                dto.Type,
                dto.Application,
                dto.Amount,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(ContractBenefitDiscount entity, ContractBenefitDiscountInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.ContractId = dto.ContractId;
            entity.Description = dto.Description;
            entity.Type = dto.Type;
            entity.Application = dto.Application;
            entity.Amount = dto.Amount;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
