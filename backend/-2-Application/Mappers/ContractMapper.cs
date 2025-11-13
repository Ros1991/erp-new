using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class ContractMapper
    {
        public static ContractOutputDTO ToContractOutputDTO(Contract entity)
        {
            if (entity == null) return null;

            return new ContractOutputDTO
            {
                ContractId = entity.ContractId,
                EmployeeId = entity.EmployeeId,
                Type = entity.Type,
                Value = entity.Value,
                IsPayroll = entity.IsPayroll,
                HasInss = entity.HasInss,
                HasIrrf = entity.HasIrrf,
                HasFgts = entity.HasFgts,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                WeeklyHours = entity.WeeklyHours,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<ContractOutputDTO> ToContractOutputDTOList(List<Contract> entities)
        {
            if (entities == null) return new List<ContractOutputDTO>();
            return entities.Select(ToContractOutputDTO).ToList();
        }

        public static Contract ToEntity(ContractInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new Contract(
                dto.EmployeeId,
                dto.Type,
                dto.Value,
                dto.IsPayroll,
                dto.HasInss,
                dto.HasIrrf,
                dto.HasFgts,
                dto.StartDate,
                dto.EndDate,
                dto.WeeklyHours,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(Contract entity, ContractInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.EmployeeId = dto.EmployeeId;
            entity.Type = dto.Type;
            entity.Value = dto.Value;
            entity.IsPayroll = dto.IsPayroll;
            entity.HasInss = dto.HasInss;
            entity.HasIrrf = dto.HasIrrf;
            entity.HasFgts = dto.HasFgts;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.WeeklyHours = dto.WeeklyHours;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
