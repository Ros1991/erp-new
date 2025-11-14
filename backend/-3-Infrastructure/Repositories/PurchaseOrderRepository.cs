using ERP.Application.DTOs;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Extensions;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly ErpContext _context;

        public PurchaseOrderRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<PurchaseOrder>> GetAllAsync(long companyId)
        {
            return await _context.Set<PurchaseOrder>()
                .Where(a => a.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<PagedResult<PurchaseOrder>> GetPagedAsync(long companyId, PurchaseOrderFilterDTO filters)
        {
            var query = _context.Set<PurchaseOrder>()
                .Where(a => a.CompanyId == companyId)
                .AsQueryable();

            // Busca geral (Search)
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                query = query.Where(x => 
                    x.Description.Contains(filters.Search) || 
                    x.Status.Contains(filters.Search));
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

            return new PagedResult<PurchaseOrder>(items, filters.Page, filters.PageSize, total);
        }

        public async Task<PurchaseOrder> GetOneByIdAsync(long purchaseOrderId)
        {
            return await _context.Set<PurchaseOrder>().FindAsync(purchaseOrderId);
        }

        public async Task<PurchaseOrder> CreateAsync(PurchaseOrder entity)
        {
            await _context.Set<PurchaseOrder>().AddAsync(entity);
            return entity;
        }

        public async Task<PurchaseOrder> UpdateByIdAsync(long purchaseOrderId, PurchaseOrder entity)
        {
            if (purchaseOrderId <= 0)
                throw new ValidationException(nameof(purchaseOrderId), "PurchaseOrderId deve ser maior que zero.");

            var existing = await _context.Set<PurchaseOrder>().FindAsync(purchaseOrderId);
            
            if (existing == null)
                throw new EntityNotFoundException("PurchaseOrder", purchaseOrderId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long purchaseOrderId)
        {
            if (purchaseOrderId <= 0)
                throw new ValidationException(nameof(purchaseOrderId), "PurchaseOrderId deve ser maior que zero.");

            var existing = await _context.Set<PurchaseOrder>().FindAsync(purchaseOrderId);
            
            if (existing == null)
                throw new EntityNotFoundException("PurchaseOrder", purchaseOrderId);

            _context.Set<PurchaseOrder>().Remove(existing);
            return true;
        }
    }
}
