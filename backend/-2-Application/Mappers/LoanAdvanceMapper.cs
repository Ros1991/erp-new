using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class LoanAdvanceMapper
    {
        public static LoanAdvanceOutputDTO ToLoanAdvanceOutputDTO(LoanAdvance entity, Domain.Entities.FinancialTransaction transaction = null)
        {
            if (entity == null) return null;

            var dto = new LoanAdvanceOutputDTO
            {
                LoanAdvanceId = entity.LoanAdvanceId,
                EmployeeId = entity.EmployeeId,
                EmployeeName = entity.Employee?.Nickname,
                Amount = entity.Amount,
                Installments = entity.Installments,
                DiscountSource = entity.DiscountSource,
                StartDate = entity.StartDate,
                Description = entity.Description,
                IsApproved = entity.IsApproved,
                InstallmentsPaid = entity.InstallmentsPaid,
                RemainingAmount = entity.RemainingAmount,
                IsFullyPaid = entity.IsFullyPaid,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };

            // Se a transação foi incluída, popula AccountId e CostCenters
            if (transaction != null)
            {
                dto.AccountId = transaction.AccountId;
                dto.AccountName = transaction.Account?.Name;
                
                if (transaction.TransactionCostCenterList != null && transaction.TransactionCostCenterList.Any())
                {
                    dto.CostCenterDistributions = transaction.TransactionCostCenterList
                        .Select(tcc => new CostCenterDistributionDTO
                        {
                            CostCenterId = tcc.CostCenterId,
                            CostCenterName = tcc.CostCenter?.Name,
                            Amount = tcc.Amount,
                            Percentage = tcc.Percentage
                        })
                        .ToList();
                }
            }

            return dto;
        }

        public static List<LoanAdvanceOutputDTO> ToLoanAdvanceOutputDTOList(List<LoanAdvance> entities)
        {
            if (entities == null) return new List<LoanAdvanceOutputDTO>();
            return entities.Select(e => ToLoanAdvanceOutputDTO(e, null)).ToList();
        }

        public static LoanAdvance ToEntity(LoanAdvanceInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            var entity = new LoanAdvance(
                dto.EmployeeId,
                dto.Amount,
                dto.Installments,
                dto.DiscountSource,
                DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc),
                dto.IsApproved,
                userId,        // CriadoPor
                null,          // AtualizadoPor
                now,           // CriadoEm
                null           // AtualizadoEm
            );
            
            entity.Description = dto.Description;
            return entity;
        }

        public static void UpdateEntity(LoanAdvance entity, LoanAdvanceInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.EmployeeId = dto.EmployeeId;
            entity.Amount = dto.Amount;
            entity.Installments = dto.Installments;
            entity.DiscountSource = dto.DiscountSource;
            entity.StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);
            entity.Description = dto.Description;
            entity.IsApproved = dto.IsApproved;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
            // ✅ CriadoPor e CriadoEm NÃO são alterados
        }
    }
}
