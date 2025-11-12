using ERP.Application.DTOs.Employee;
using ERP.Application.DTOs.Base;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllAsync(long companyId);
        Task<PagedResult<Employee>> GetPagedAsync(long companyId, EmployeeFilterDTO filters);
        Task<Employee> GetOneByIdAsync(long employeeId);
        Task<Employee> CreateAsync(Employee entity);
        Task<Employee> UpdateByIdAsync(long employeeId, Employee entity);
        Task<bool> DeleteByIdAsync(long employeeId);
    }
}
