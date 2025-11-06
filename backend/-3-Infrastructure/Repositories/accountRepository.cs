using CAU.Application.DTOs;
using CAU.Application.DTOs.Base;
using CAU.Application.Interfaces.Repositories;
using CAU.Domain.Entities;
using CAU.Infrastructure.Data;
using CAU.Infrastructure.Extensions;
using CAU.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CAU.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ErpContext _context;

        public AccountRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _context.Set<Account>()
                .ToListAsync();
        }

        public async Task<PagedResult<Account>> GetPagedAsync(AccountFilterDTO filters)
        {
            var query = _context.Set<Account>().AsQueryable();

            // Busca geral (Search)
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                query = query.Where(x => 
                    x.Name.Contains(filters.Search) || 
                    x.Type.Contains(filters.Search));
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

            return new PagedResult<Account>(items, filters.Page, filters.PageSize, total);
        }

        public async Task<Account> GetOneByIdAsync(long AccountId)
        {
            return await _context.Set<Account>().FindAsync(AccountId);
        }

        public async Task<Account> CreateAsync(Account entity)
        {
            await _context.Set<Account>().AddAsync(entity);
            return entity;
        }

        public async Task<Account> UpdateByIdAsync(long AccountId, Account entity)
        {
            if (AccountId <= 0)
                throw new ValidationException(nameof(AccountId), "AccountId deve ser maior que zero.");

            var existing = await _context.Set<Account>().FindAsync(AccountId);
            
            if (existing == null)
                throw new EntityNotFoundException("Account", AccountId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long AccountId)
        {
            if (AccountId <= 0)
                throw new ValidationException(nameof(AccountId), "AccountId deve ser maior que zero.");

            var existing = await _context.Set<Account>().FindAsync(AccountId);
            
            if (existing == null)
                throw new EntityNotFoundException("Account", AccountId);

            _context.Set<Account>().Remove(existing);
            return true;
        }
    }
}
