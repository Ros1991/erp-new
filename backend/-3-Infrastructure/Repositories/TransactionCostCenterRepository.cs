using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class TransactionCostCenterRepository : ITransactionCostCenterRepository
    {
        private readonly ErpContext _context;

        public TransactionCostCenterRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<TransactionCostCenter> CreateAsync(TransactionCostCenter entity)
        {
            await _context.Set<TransactionCostCenter>().AddAsync(entity);
            return entity;
        }

        public async Task<List<TransactionCostCenter>> GetByTransactionIdAsync(long financialTransactionId)
        {
            return await _context.Set<TransactionCostCenter>()
                .Where(tcc => tcc.FinancialTransactionId == financialTransactionId)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(long transactionCostCenterId)
        {
            var entity = await _context.Set<TransactionCostCenter>()
                .FirstOrDefaultAsync(tcc => tcc.TransactionCostCenterId == transactionCostCenterId);
            
            if (entity == null)
                return false;

            _context.Set<TransactionCostCenter>().Remove(entity);
            return true;
        }
    }
}
