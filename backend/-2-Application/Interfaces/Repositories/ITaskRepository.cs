using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using Task = ERP.Domain.Entities.Task;

namespace ERP.Application.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        Task<List<Task>> GetAllAsync(long companyId);
        Task<PagedResult<Task>> GetPagedAsync(long companyId, TaskFilterDTO filters);
        Task<Task> GetOneByIdAsync(long taskId);
        Task<Task> CreateAsync(Task entity);
        Task<Task> UpdateByIdAsync(long taskId, Task entity);
        Task<bool> DeleteByIdAsync(long taskId);
    }
}
