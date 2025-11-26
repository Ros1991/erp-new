using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class PayrollItemRepository : IPayrollItemRepository
    {
        private readonly ErpContext _context;

        public PayrollItemRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<PayrollItem>> GetAllByPayrollEmployeeIdAsync(long payrollEmployeeId)
        {
            return await _context.Set<PayrollItem>()
                .Where(pi => pi.PayrollEmployeeId == payrollEmployeeId)
                .OrderBy(pi => pi.Type)
                .ThenBy(pi => pi.Description)
                .ToListAsync();
        }

        public async Task<int> GetNextInstallmentNumberAsync(long loanAdvanceId)
        {
            var maxInstallment = await _context.Set<PayrollItem>()
                .Where(pi => pi.ReferenceId == loanAdvanceId &&
                            pi.SourceType == "loan" &&
                            pi.IsActive == true)
                .MaxAsync(pi => (int?)pi.InstallmentNumber);

            return (maxInstallment ?? 0) + 1;
        }

        public async Task<PayrollItem> GetOneByIdAsync(long payrollItemId)
        {
            return await _context.Set<PayrollItem>()
                .FirstOrDefaultAsync(pi => pi.PayrollItemId == payrollItemId);
        }

        public async Task<PayrollItem> CreateAsync(PayrollItem entity)
        {
            await _context.Set<PayrollItem>().AddAsync(entity);
            return entity;
        }

        public async Task<PayrollItem> UpdateByIdAsync(long payrollItemId, PayrollItem entity)
        {
            if (payrollItemId <= 0)
                throw new ValidationException(nameof(payrollItemId), "PayrollItemId deve ser maior que zero.");

            var existing = await _context.Set<PayrollItem>().FindAsync(payrollItemId);
            
            if (existing == null)
                throw new EntityNotFoundException("PayrollItem", payrollItemId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long payrollItemId)
        {
            if (payrollItemId <= 0)
                throw new ValidationException(nameof(payrollItemId), "PayrollItemId deve ser maior que zero.");

            var existing = await _context.Set<PayrollItem>().FindAsync(payrollItemId);
            
            if (existing == null)
                throw new EntityNotFoundException("PayrollItem", payrollItemId);

            _context.Set<PayrollItem>().Remove(existing);
            return true;
        }
    }
}
