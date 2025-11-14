using ERP.Application.DTOs;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Extensions;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ErpContext _context;

        public LocationRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<Location>> GetAllAsync(long companyId)
        {
            return await _context.Set<Location>()
                .Where(a => a.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<PagedResult<Location>> GetPagedAsync(long companyId, LocationFilterDTO filters)
        {
            var query = _context.Set<Location>()
                .Where(a => a.CompanyId == companyId)
                .AsQueryable();

            // Busca geral (Search)
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                query = query.Where(x => 
                    x.Name.Contains(filters.Search) || 
                    (x.Address != null && x.Address.Contains(filters.Search)));
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

            return new PagedResult<Location>(items, filters.Page, filters.PageSize, total);
        }

        public async Task<Location> GetOneByIdAsync(long locationId)
        {
            return await _context.Set<Location>().FindAsync(locationId);
        }

        public async Task<Location> CreateAsync(Location entity)
        {
            await _context.Set<Location>().AddAsync(entity);
            return entity;
        }

        public async Task<Location> UpdateByIdAsync(long locationId, Location entity)
        {
            if (locationId <= 0)
                throw new ValidationException(nameof(locationId), "LocationId deve ser maior que zero.");

            var existing = await _context.Set<Location>().FindAsync(locationId);
            
            if (existing == null)
                throw new EntityNotFoundException("Location", locationId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long locationId)
        {
            if (locationId <= 0)
                throw new ValidationException(nameof(locationId), "LocationId deve ser maior que zero.");

            var existing = await _context.Set<Location>().FindAsync(locationId);
            
            if (existing == null)
                throw new EntityNotFoundException("Location", locationId);

            _context.Set<Location>().Remove(existing);
            return true;
        }
    }
}
