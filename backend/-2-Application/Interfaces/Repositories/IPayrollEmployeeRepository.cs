using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IPayrollEmployeeRepository
    {
        Task<List<PayrollEmployee>> GetAllByPayrollIdAsync(long payrollId);
        Task<PayrollEmployee> GetOneByIdAsync(long payrollEmployeeId);
        Task<PayrollEmployee> GetByPayrollAndContractAsync(long payrollId, long contractId);
        Task<PayrollEmployee?> GetPreviousPayrollEmployeeAsync(long employeeId, long currentPayrollId);
        Task<PayrollEmployee> CreateAsync(PayrollEmployee entity);
        Task<PayrollEmployee> UpdateByIdAsync(long payrollEmployeeId, PayrollEmployee entity);
        Task<bool> DeleteByIdAsync(long payrollEmployeeId);
    }
}
