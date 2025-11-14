using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class FinancialTransactionMapper
    {
        public static FinancialTransactionOutputDTO ToFinancialTransactionOutputDTO(FinancialTransaction entity)
        {
            if (entity == null) return null;

            return new FinancialTransactionOutputDTO
            {
                FinancialTransactionId = entity.FinancialTransactionId,
                CompanyId = entity.CompanyId,
                AccountId = entity.AccountId,
                AccountName = entity.Account?.Name,
                PurchaseOrderId = entity.PurchaseOrderId,
                AccountPayableReceivableId = entity.AccountPayableReceivableId,
                SupplierCustomerId = entity.SupplierCustomerId,
                SupplierCustomerName = entity.SupplierCustomer?.Name,
                LoanAdvanceId = entity.LoanAdvanceId,
                Description = entity.Description,
                Type = entity.Type,
                Amount = entity.Amount,
                TransactionDate = entity.TransactionDate,
                CostCenterDistributions = entity.TransactionCostCenterList?.Select(tcc => new CostCenterDistributionDTO
                {
                    CostCenterId = tcc.CostCenterId,
                    CostCenterName = tcc.CostCenter?.Name,
                    Percentage = tcc.Percentage,
                    Amount = tcc.Amount
                }).ToList(),
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<FinancialTransactionOutputDTO> ToFinancialTransactionOutputDTOList(List<FinancialTransaction> entities)
        {
            if (entities == null) return new List<FinancialTransactionOutputDTO>();
            return entities.Select(ToFinancialTransactionOutputDTO).ToList();
        }

        public static FinancialTransaction ToEntity(FinancialTransactionInputDTO dto, long companyId, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new FinancialTransaction(
                companyId,
                dto.AccountId,
                dto.PurchaseOrderId,
                dto.AccountPayableReceivableId,
                dto.SupplierCustomerId,
                null, // LoanAdvanceId - não vem do DTO, apenas de empréstimos
                dto.Description,
                dto.Type,
                dto.Amount,
                dto.TransactionDate,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(FinancialTransaction entity, FinancialTransactionInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.AccountId = dto.AccountId;
            entity.PurchaseOrderId = dto.PurchaseOrderId;
            entity.AccountPayableReceivableId = dto.AccountPayableReceivableId;
            entity.SupplierCustomerId = dto.SupplierCustomerId;
            entity.Description = dto.Description;
            entity.Type = dto.Type;
            entity.Amount = dto.Amount;
            entity.TransactionDate = dto.TransactionDate;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
