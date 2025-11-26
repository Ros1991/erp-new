using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IPayrollEmployeeRepository
    {
        Task<List<PayrollEmployee>> GetAllByPayrollIdAsync(long payrollId);
        Task<PayrollEmployee> GetOneByIdAsync(long payrollEmployeeId);
        Task<PayrollEmployee> CreateAsync(PayrollEmployee entity);
        Task<PayrollEmployee> UpdateByIdAsync(long payrollEmployeeId, PayrollEmployee entity);
        Task<bool> DeleteByIdAsync(long payrollEmployeeId);
    }
}
