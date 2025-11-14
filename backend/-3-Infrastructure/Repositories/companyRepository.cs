using ERP.Application.DTOs;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Extensions;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ErpContext _context;

        public CompanyRepository(ErpContext context)
        {
            _context = context;
        }

	    public async Task<List<Company>> GetAllAsync(long userId)
        {
            // Retorna apenas empresas não deletadas onde o usuário está associado via CompanyUser
            return await _context.Set<Company>()
                .Where(c => c.DeletadoEm == null && 
                    _context.Set<CompanyUser>()
                        .Any(cu => cu.CompanyId == c.CompanyId && cu.UserId == userId))
                .OrderByDescending(c => c.CriadoEm)
                .ToListAsync();
         }

        public async Task<PagedResult<Company>> GetPagedAsync(CompanyFilterDTO filters, long userId)
        {
            // Filtrar apenas empresas não deletadas onde o usuário está associado via CompanyUser
            var query = _context.Set<Company>()
                .Where(c => c.DeletadoEm == null &&
                    _context.Set<CompanyUser>()
                        .Any(cu => cu.CompanyId == c.CompanyId && cu.UserId == userId))
                .AsQueryable();

            // Aplicar filtros
            //if (!string.IsNullOrWhiteSpace(filters.Name))
            //    query = query.Where(x => x.Name.Contains(filters.Name));

            //if (!string.IsNullOrWhiteSpace(filters.Document))
            //    query = query.Where(x => x.Document.Contains(filters.Document));


            // Busca geral (Search)
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                query = query.Where(x => 
                    x.Name.Contains(filters.Search) || 
                    x.Document.Contains(filters.Search));
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

            return new PagedResult<Company>(items, filters.Page, filters.PageSize, total);
        }

	    public async Task<Company> GetOneByIdAsync(long CompanyId)
        {
            return await _context.Set<Company>().FindAsync(CompanyId);
        }

	    public async Task<Company> CreateAsync(Company entity)
        {
            await _context.Set<Company>().AddAsync(entity);
            return entity;
         }

	    public async Task<Company> UpdateByIdAsync(long CompanyId, Company entity)
        {
            if (CompanyId <= 0)
                throw new ValidationException(nameof(CompanyId), "CompanyId deve ser maior que zero.");

            var existing = await _context.Set<Company>().FindAsync(CompanyId);
            
            if (existing == null)
                throw new EntityNotFoundException("Company", CompanyId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

	    public async Task<bool> DeleteByIdAsync(long CompanyId, long currentUserId)
        {
            if (CompanyId <= 0)
                throw new ValidationException(nameof(CompanyId), "CompanyId deve ser maior que zero.");

            var existing = await _context.Set<Company>().FindAsync(CompanyId);
            
            if (existing == null || existing.DeletadoEm != null)
                throw new EntityNotFoundException("Company", CompanyId);

            // Soft delete: apenas marcar como deletado
            existing.DeletadoEm = DateTime.UtcNow;
            existing.DeletadoPor = currentUserId;
            
            return true;
        }
    }
}
