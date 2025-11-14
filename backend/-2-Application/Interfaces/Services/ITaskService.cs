using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface ITaskService
    {
        Task<List<TaskOutputDTO>> GetAllAsync(long companyId);
        Task<PagedResult<TaskOutputDTO>> GetPagedAsync(long companyId, TaskFilterDTO filters);
        Task<TaskOutputDTO> GetOneByIdAsync(long taskId);
        Task<TaskOutputDTO> CreateAsync(TaskInputDTO dto, long companyId, long currentUserId);
        Task<TaskOutputDTO> UpdateByIdAsync(long taskId, TaskInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long taskId);
    }
}
