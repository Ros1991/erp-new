using ERP.Application.DTOs;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Extensions;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class SupplierCustomerRepository : ISupplierCustomerRepository
    {
        private readonly ErpContext _context;

        public SupplierCustomerRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<SupplierCustomer>> GetAllAsync(long companyId)
        {
            return await _context.Set<SupplierCustomer>()
                .Where(a => a.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<PagedResult<SupplierCustomer>> GetPagedAsync(long companyId, SupplierCustomerFilterDTO filters)
        {
            var query = _context.Set<SupplierCustomer>()
                .Where(a => a.CompanyId == companyId)
                .AsQueryable();

            // Busca geral (Search)
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                query = query.Where(x => 
                    x.Name.Contains(filters.Search) || 
                    (x.Document != null && x.Document.Contains(filters.Search)) ||
                    (x.Email != null && x.Email.Contains(filters.Search)));
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

            return new PagedResult<SupplierCustomer>(items, filters.Page, filters.PageSize, total);
        }

        public async Task<SupplierCustomer> GetOneByIdAsync(long supplierCustomerId)
        {
            return await _context.Set<SupplierCustomer>().FindAsync(supplierCustomerId);
        }

        public async Task<SupplierCustomer> CreateAsync(SupplierCustomer entity)
        {
            await _context.Set<SupplierCustomer>().AddAsync(entity);
            return entity;
        }

        public async Task<SupplierCustomer> UpdateByIdAsync(long supplierCustomerId, SupplierCustomer entity)
        {
            if (supplierCustomerId <= 0)
                throw new ValidationException(nameof(supplierCustomerId), "SupplierCustomerId deve ser maior que zero.");

            var existing = await _context.Set<SupplierCustomer>().FindAsync(supplierCustomerId);
            
            if (existing == null)
                throw new EntityNotFoundException("SupplierCustomer", supplierCustomerId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long supplierCustomerId)
        {
            if (supplierCustomerId <= 0)
                throw new ValidationException(nameof(supplierCustomerId), "SupplierCustomerId deve ser maior que zero.");

            var existing = await _context.Set<SupplierCustomer>().FindAsync(supplierCustomerId);
            
            if (existing == null)
                throw new EntityNotFoundException("SupplierCustomer", supplierCustomerId);

            _context.Set<SupplierCustomer>().Remove(existing);
            return true;
        }
    }
}
