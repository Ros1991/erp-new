using ERP.Application.DTOs;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Extensions;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class CostCenterRepository : ICostCenterRepository
    {
        private readonly ErpContext _context;

        public CostCenterRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<CostCenter>> GetAllAsync(long companyId)
        {
            return await _context.Set<CostCenter>()
                .Where(a => a.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<PagedResult<CostCenter>> GetPagedAsync(long companyId, CostCenterFilterDTO filters)
        {
            var query = _context.Set<CostCenter>()
                .Where(a => a.CompanyId == companyId)
                .AsQueryable();

            // Busca geral (Search)
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                query = query.Where(x => 
                    x.Name.Contains(filters.Search) || 
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

            return new PagedResult<CostCenter>(items, filters.Page, filters.PageSize, total);
        }

        public async Task<CostCenter> GetOneByIdAsync(long costCenterId)
        {
            return await _context.Set<CostCenter>().FindAsync(costCenterId);
        }

        public async Task<CostCenter> CreateAsync(CostCenter entity)
        {
            await _context.Set<CostCenter>().AddAsync(entity);
            return entity;
        }

        public async Task<CostCenter> UpdateByIdAsync(long costCenterId, CostCenter entity)
        {
            if (costCenterId <= 0)
                throw new ValidationException(nameof(costCenterId), "CostCenterId deve ser maior que zero.");

            var existing = await _context.Set<CostCenter>().FindAsync(costCenterId);
            
            if (existing == null)
                throw new EntityNotFoundException("CostCenter", costCenterId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long costCenterId)
        {
            if (costCenterId <= 0)
                throw new ValidationException(nameof(costCenterId), "CostCenterId deve ser maior que zero.");

            var existing = await _context.Set<CostCenter>().FindAsync(costCenterId);
            
            if (existing == null)
                throw new EntityNotFoundException("CostCenter", costCenterId);

            _context.Set<CostCenter>().Remove(existing);
            return true;
        }
    }
}
