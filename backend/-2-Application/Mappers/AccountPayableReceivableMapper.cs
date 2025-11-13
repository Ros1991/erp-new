using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class AccountPayableReceivableMapper
    {
        public static AccountPayableReceivableOutputDTO ToAccountPayableReceivableOutputDTO(AccountPayableReceivable entity)
        {
            if (entity == null) return null;

            return new AccountPayableReceivableOutputDTO
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
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<AccountPayableReceivableOutputDTO> ToAccountPayableReceivableOutputDTOList(List<AccountPayableReceivable> entities)
        {
            if (entities == null) return new List<AccountPayableReceivableOutputDTO>();
            return entities.Select(ToAccountPayableReceivableOutputDTO).ToList();
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
                dto.DueDate,
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
            entity.DueDate = dto.DueDate;
            entity.IsPaid = dto.IsPaid;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
            // ✅ CriadoPor e CriadoEm NÃO são alterados
        }
    }
}
