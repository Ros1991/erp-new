using ERP.Application.DTOs;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Extensions;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ErpContext _context;

        public UserRepository(ErpContext context)
        {
            _context = context;
        }

	    public async Task<List<User>> GetAllAsync()
        {
            return await _context.Set<User>()
            .ToListAsync();
        }

        public async Task<PagedResult<User>> GetPagedAsync(UserFilterDTO filters)
        {
            var query = _context.Set<User>().AsQueryable();

            // Busca por email, telefone ou CPF
            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                var searchLower = filters.SearchTerm.ToLower();
                // Apenas dígitos (para phone/cpf)
                var onlyDigits = System.Text.RegularExpressions.Regex.Replace(searchLower, @"[^\d]", "");
                
                query = query.Where(x => 
                    // Email: busca normal com ToLower
                    (x.Email != null && x.Email.ToLower().Contains(searchLower)) ||
                    // Phone: busca APENAS se tiver dígitos no termo
                    (!string.IsNullOrEmpty(onlyDigits) && x.Phone != null && x.Phone.Contains(onlyDigits)) ||
                    // CPF: busca APENAS se tiver dígitos no termo
                    (!string.IsNullOrEmpty(onlyDigits) && x.Cpf != null && x.Cpf.Contains(onlyDigits))
                );
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
                query = query.OrderByDescending(x => x.UserId); // Ordenação padrão
            }

            // Aplicar paginação
            var items = await query
                .Skip(filters.Skip)
                .Take(filters.PageSize)
                .ToListAsync();

            return new PagedResult<User>(items, filters.Page, filters.PageSize, total);
        }

	    public async Task<User> GetOneByIdAsync(long UserId)
        {
            return await _context.Set<User>().FindAsync(UserId);
        }

	    public async Task<User> CreateAsync(User entity) 
        {
            await _context.Set<User>().AddAsync(entity);
            return entity;
        }

	    public async Task<User> UpdateByIdAsync(long UserId, User entity)
        {
            if (UserId <= 0)
                throw new ValidationException(nameof(UserId), "UserId deve ser maior que zero.");

            var existing = await _context.Set<User>().FindAsync(UserId);
            
            if (existing == null)
                throw new EntityNotFoundException("User", UserId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }
        
	    public async Task<bool> DeleteByIdAsync(long UserId)
        {
            if (UserId <= 0)
                throw new ValidationException(nameof(UserId), "UserId deve ser maior que zero.");

            var existing = await _context.Set<User>().FindAsync(UserId);
            
            if (existing == null)
                throw new EntityNotFoundException("User", UserId);

            _context.Set<User>().Remove(existing);
            return true;
        }
    }
}
