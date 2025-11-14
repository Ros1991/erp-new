using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface IContractService
    {
        Task<List<ContractOutputDTO>> GetAllByEmployeeIdAsync(long employeeId);
        Task<ContractOutputDTO> GetActiveByEmployeeIdAsync(long employeeId);
        Task<ContractOutputDTO> GetOneByIdAsync(long contractId);
        Task<ContractOutputDTO> CreateAsync(ContractInputDTO dto, long currentUserId);
        Task<ContractOutputDTO> UpdateByIdAsync(long contractId, ContractInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long contractId);
    }
}
