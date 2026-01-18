using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class AccountPayableReceivableMapper
    {
        public static AccountPayableReceivableOutputDTO ToAccountPayableReceivableOutputDTO(AccountPayableReceivable entity, FinancialTransaction? transaction = null)
        {
            if (entity == null) return null;

            var dto = new AccountPayableReceivableOutputDTO
            {
                AccountPayableReceivableId = entity.AccountPayableReceivableId,
                CompanyId = entity.CompanyId,
                SupplierCustomerId = entity.SupplierCustomerId,
                SupplierCustomerName = entity.SupplierCustomer?.Name,
                Description = entity.Description,
                Type = entity.Type,
                Amount = entity.Amount,
                DueDate = entity.DueDate,
                IsPaid = entity.IsPaid,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm,
                CostCenterDistributions = new List<CostCenterDistributionDTO>()
            };

            // Pegar distribuições da tabela própria (sempre disponíveis)
            if (entity.CostCenterDistributions != null && entity.CostCenterDistributions.Any())
            {
                dto.CostCenterDistributions = entity.CostCenterDistributions
                    .Select(d => new CostCenterDistributionDTO
                    {
                        CostCenterId = d.CostCenterId,
                        CostCenterName = d.CostCenter?.Name,
                        Percentage = d.Percentage,
                        Amount = d.Amount
                    })
                    .ToList();
            }

            // Adicionar dados da transação financeira se existir (quando isPaid=true)
            if (transaction != null)
            {
                dto.AccountId = transaction.AccountId;
                dto.AccountName = transaction.Account?.Name;
            }

            return dto;
        }

        public static List<AccountPayableReceivableOutputDTO> ToAccountPayableReceivableOutputDTOList(List<AccountPayableReceivable> entities)
        {
            if (entities == null) return new List<AccountPayableReceivableOutputDTO>();
            return entities.Select(e => ToAccountPayableReceivableOutputDTO(e, null)).ToList();
        }

        public static AccountPayableReceivable ToEntity(AccountPayableReceivableInputDTO dto, long companyId, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new AccountPayableReceivable(
                companyId,
                dto.SupplierCustomerId,
                dto.Description,
                dto.Type,
                dto.Amount,
                DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc),
                dto.IsPaid,
                userId,        // CriadoPor
                null,          // AtualizadoPor
                now,           // CriadoEm
                null           // AtualizadoEm
            );
        }

        public static void UpdateEntity(AccountPayableReceivable entity, AccountPayableReceivableInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.SupplierCustomerId = dto.SupplierCustomerId;
            entity.Description = dto.Description;
            entity.Type = dto.Type;
            entity.Amount = dto.Amount;
            entity.DueDate = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc);
            entity.IsPaid = dto.IsPaid;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
            // ✅ CriadoPor e CriadoEm NÃO são alterados
        }
    }
}
