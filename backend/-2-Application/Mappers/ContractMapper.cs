using ERP.Application.DTOs;
using ERP.Domain.Entities;
using ERP.CrossCutting.Helpers;

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
                EmployeeName = entity.Employee?.FullName,
                Type = entity.Type,
                Value = entity.Value,
                IsPayroll = entity.IsPayroll,
                HasInss = entity.HasInss,
                HasIrrf = entity.HasIrrf,
                HasFgts = entity.HasFgts,
                HasThirteenthSalary = entity.HasThirteenthSalary,
                HasVacationBonus = entity.HasVacationBonus,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                WeeklyHours = entity.WeeklyHours,
                IsActive = entity.IsActive,
                BenefitsDiscounts = entity.ContractBenefitDiscountList?.Select(b => new ContractBenefitDiscountDTO
                {
                    ContractBenefitDiscountId = b.ContractBenefitDiscountId,
                    Description = b.Description,
                    Type = b.Type,
                    Application = b.Application,
                    Amount = b.Amount,
                    Month = b.Month,
                    HasTaxes = b.HasTaxes,
                    IsProportional = b.IsProportional
                }).ToList(),
                CostCenters = entity.ContractCostCenterList?.Select(c => new ContractCostCenterDTO
                {
                    ContractCostCenterId = c.ContractCostCenterId,
                    CostCenterId = c.CostCenterId,
                    CostCenterName = c.CostCenter?.Name,
                    Percentage = c.Percentage
                }).ToList(),
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
                DateTimeHelper.ToUtc(dto.StartDate),
                dto.EndDate.HasValue ? DateTimeHelper.ToUtc(dto.EndDate.Value) : null,
                dto.WeeklyHours,
                dto.IsActive,
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
            entity.HasThirteenthSalary = dto.HasThirteenthSalary;
            entity.HasVacationBonus = dto.HasVacationBonus;
            entity.StartDate = DateTimeHelper.ToUtc(dto.StartDate);
            entity.EndDate = dto.EndDate.HasValue ? DateTimeHelper.ToUtc(dto.EndDate.Value) : null;
            entity.WeeklyHours = dto.WeeklyHours;
            entity.IsActive = dto.IsActive;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
