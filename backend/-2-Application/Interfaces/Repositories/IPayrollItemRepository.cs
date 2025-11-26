using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IPayrollItemRepository
    {
        Task<List<PayrollItem>> GetAllByPayrollEmployeeIdAsync(long payrollEmployeeId);
        Task<int> GetNextInstallmentNumberAsync(long loanAdvanceId);
        Task<PayrollItem> GetOneByIdAsync(long payrollItemId);
        Task<PayrollItem> CreateAsync(PayrollItem entity);
        Task<PayrollItem> UpdateByIdAsync(long payrollItemId, PayrollItem entity);
        Task<bool> DeleteByIdAsync(long payrollItemId);
    }
}
