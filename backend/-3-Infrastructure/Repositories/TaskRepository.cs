using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Application.Interfaces.Repositories;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Extensions;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;
using Task = ERP.Domain.Entities.Task;

namespace ERP.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ErpContext _context;

        public TaskRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<Task>> GetAllAsync(long companyId)
        {
            return await _context.Set<Task>()
                .Where(a => a.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<PagedResult<Task>> GetPagedAsync(long companyId, TaskFilterDTO filters)
        {
            var query = _context.Set<Task>()
                .Where(a => a.CompanyId == companyId)
                .AsQueryable();

            // Busca geral (Search)
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                query = query.Where(x => 
                    x.Title.Contains(filters.Search) || 
                    (x.Description != null && x.Description.Contains(filters.Search)));
            }

            // Contar total antes da paginação
            var total = await query.CountAsync();

            // ✅ Aplicar ordenação dinâmica
            if (!string.IsNullOrWhiteSpace(filters.OrderBy))
            {
                query = query.OrderByProperty(filters.OrderBy, filters.IsAscending);
            }
            else
            {
                query = query.OrderByDescending(x => x.CriadoEm); // Ordenação padrão
            }

            // Aplicar paginação
            var items = await query
                .Skip(filters.Skip)
                .Take(filters.PageSize)
                .ToListAsync();

            return new PagedResult<Task>(items, filters.Page, filters.PageSize, total);
        }

        public async Task<Task> GetOneByIdAsync(long taskId)
        {
            return await _context.Set<Task>().FindAsync(taskId);
        }

        public async Task<Task> CreateAsync(Task entity)
        {
            await _context.Set<Task>().AddAsync(entity);
            return entity;
        }

        public async Task<Task> UpdateByIdAsync(long taskId, Task entity)
        {
            if (taskId <= 0)
                throw new ValidationException(nameof(taskId), "TaskId deve ser maior que zero.");

            var existing = await _context.Set<Task>().FindAsync(taskId);
            
            if (existing == null)
                throw new EntityNotFoundException("Task", taskId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long taskId)
        {
            if (taskId <= 0)
                throw new ValidationException(nameof(taskId), "TaskId deve ser maior que zero.");

            var existing = await _context.Set<Task>().FindAsync(taskId);
            
            if (existing == null)
                throw new EntityNotFoundException("Task", taskId);

            _context.Set<Task>().Remove(existing);
            return true;
        }
    }
}
