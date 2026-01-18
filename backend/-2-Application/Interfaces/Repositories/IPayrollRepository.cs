using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IPayrollRepository
    {
        Task<List<Payroll>> GetAllAsync(long companyId);
        Task<PagedResult<Payroll>> GetPagedAsync(long companyId, PayrollFilterDTO filters);
        Task<Payroll> GetOneByIdAsync(long payrollId);
        Task<Payroll> GetOneByIdWithIncludesAsync(long payrollId);
        Task<Payroll> GetByCompanyAndPeriodAsync(long companyId, DateTime startDate, DateTime endDate);
        Task<Payroll> GetLastPayrollAsync(long companyId);
        Task<Payroll?> GetLastClosedPayrollByCompanyAsync(long companyId);
        Task<Payroll?> GetOpenPayrollByCompanyAsync(long companyId);
        Task<int> GetEmployeeCountAsync(long payrollId);
        Task<Payroll> CreateAsync(Payroll entity);
        Task<Payroll> UpdateByIdAsync(long payrollId, Payroll entity);
        Task<bool> DeleteByIdAsync(long payrollId);
    }
}
