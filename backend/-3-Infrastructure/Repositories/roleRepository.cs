using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Extensions;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ErpContext _context;

        public RoleRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllAsync(long companyId)
        {
            return await _context.Set<Role>()
                .Where(r => r.CompanyId == companyId)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<PagedResult<Role>> GetPagedAsync(long companyId, RoleFilterDTO filters)
        {
            var query = _context.Set<Role>()
                .Where(r => r.CompanyId == companyId)
                .AsQueryable();

            // Filtro por nome (case insensitive)
            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                var nameLower = filters.Name.ToLower();
                query = query.Where(r => r.Name.ToLower().Contains(nameLower));
            }

            // Filtro por tipo (sistema ou customizado)
            if (filters.IsSystem.HasValue)
            {
                query = query.Where(r => r.IsSystem == filters.IsSystem.Value);
            }

            // Busca geral (Search) - case insensitive
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var searchLower = filters.Search.ToLower();
                query = query.Where(r => r.Name.ToLower().Contains(searchLower));
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
                query = query.OrderBy(r => r.Name); // Ordenação padrão por nome
            }

            // Aplicar paginação
            var items = await query
                .Skip(filters.Skip)
                .Take(filters.PageSize)
                .ToListAsync();

            return new PagedResult<Role>(items, filters.Page, filters.PageSize, total);
        }

        public async Task<Role> GetOneByIdAsync(long roleId)
        {
            return await _context.Set<Role>().FindAsync(roleId);
        }

        public async Task<Role> CreateAsync(Role entity)
        {
            await _context.Set<Role>().AddAsync(entity);
            return entity;
        }

        public async Task<Role> UpdateByIdAsync(long roleId, Role entity)
        {
            if (roleId <= 0)
                throw new ValidationException(nameof(roleId), "RoleId deve ser maior que zero.");

            var existing = await _context.Set<Role>().FindAsync(roleId);
            
            if (existing == null)
                throw new EntityNotFoundException("Role", roleId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long roleId)
        {
            if (roleId <= 0)
                throw new ValidationException(nameof(roleId), "RoleId deve ser maior que zero.");

            var existing = await _context.Set<Role>().FindAsync(roleId);
            
            if (existing == null)
                throw new EntityNotFoundException("Role", roleId);

            _context.Set<Role>().Remove(existing);
            return true;
        }
    }
}
