using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IContractRepository
    {
        Task<List<Contract>> GetAllByEmployeeIdAsync(long employeeId);
        Task<Contract> GetActiveByEmployeeIdAsync(long employeeId);
        Task<List<Contract>> GetActivePayrollContractsByCompanyAsync(long companyId, DateTime periodStartDate, DateTime periodEndDate);
        Task<Contract> GetOneByIdAsync(long contractId);
        Task<Contract> CreateAsync(Contract entity);
        Task<Contract> UpdateByIdAsync(long contractId, Contract entity);
        Task<bool> DeleteByIdAsync(long contractId);
        Task<bool> DeactivateOtherContractsAsync(long employeeId, long currentContractId);
    }
}
