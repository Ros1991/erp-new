using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface ITransactionCostCenterRepository
    {
        Task<TransactionCostCenter> CreateAsync(TransactionCostCenter entity);
        Task<List<TransactionCostCenter>> GetByTransactionIdAsync(long financialTransactionId);
        Task<bool> DeleteAsync(long transactionCostCenterId);
    }
}
