using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.Application.DTOs.Base;

namespace ERP.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TaskOutputDTO>> GetAllAsync(long companyId)
        {
            var entities = await _unitOfWork.TaskRepository.GetAllAsync(companyId);
            return TaskMapper.ToTaskOutputDTOList(entities);
        }

        public async Task<PagedResult<TaskOutputDTO>> GetPagedAsync(long companyId, TaskFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.TaskRepository.GetPagedAsync(companyId, filters);
            var dtoItems = TaskMapper.ToTaskOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<TaskOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<TaskOutputDTO> GetOneByIdAsync(long taskId)
        {
            var entity = await _unitOfWork.TaskRepository.GetOneByIdAsync(taskId);
            if (entity == null)
            {
                throw new EntityNotFoundException("Task", taskId);
            }
            return TaskMapper.ToTaskOutputDTO(entity);
        }

        public async Task<TaskOutputDTO> CreateAsync(TaskInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var entity = TaskMapper.ToEntity(dto, companyId, currentUserId);
            
            var createdEntity = await _unitOfWork.TaskRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return TaskMapper.ToTaskOutputDTO(createdEntity);
        }

        public async Task<TaskOutputDTO> UpdateByIdAsync(long taskId, TaskInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var existingEntity = await _unitOfWork.TaskRepository.GetOneByIdAsync(taskId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("Task", taskId);
            }

            TaskMapper.UpdateEntity(existingEntity, dto, currentUserId);
            
            await _unitOfWork.SaveChangesAsync();
            return TaskMapper.ToTaskOutputDTO(existingEntity);
        }

        public async Task<bool> DeleteByIdAsync(long taskId)
        {
            var result = await _unitOfWork.TaskRepository.DeleteByIdAsync(taskId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
