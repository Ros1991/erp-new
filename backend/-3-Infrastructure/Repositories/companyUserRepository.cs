using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class CompanyUserRepository : ICompanyUserRepository
    {
        private readonly ErpContext _context;

        public CompanyUserRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyUser>> GetAllAsync(long companyId)
        {
            return await _context.Set<CompanyUser>()
                .Include(cu => cu.User)
                .Include(cu => cu.Role)
                .Where(cu => cu.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<PagedResult<CompanyUser>> GetPagedAsync(long companyId, CompanyUserFilterDTO filters)
        {
            // ✅ Query no banco, NÃO em memória
            var query = _context.Set<CompanyUser>()
                .Include(cu => cu.User)
                .Include(cu => cu.Role)
                .Where(cu => cu.CompanyId == companyId)
                .AsQueryable();

            // ✅ Filtro por termo de busca NO BANCO
            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                var searchLower = filters.SearchTerm.ToLower();
                // Phone e CPF são salvos sem formatação, então remove caracteres especiais do termo
                var cleanSearch = System.Text.RegularExpressions.Regex.Replace(searchLower, @"[^\d]", "");
                
                query = query.Where(cu => 
                    // Email: busca normal com ToLower
                    (cu.User.Email != null && cu.User.Email.ToLower().Contains(searchLower)) ||
                    // Phone: sempre busca sem formatação (banco não tem formatação)
                    (cu.User.Phone != null && cu.User.Phone.Contains(cleanSearch)) ||
                    // CPF: sempre busca sem formatação (banco não tem formatação)
                    (cu.User.Cpf != null && cu.User.Cpf.Contains(cleanSearch)) ||
                    // Cargo: busca normal com ToLower
                    (cu.Role != null && cu.Role.Name.ToLower().Contains(searchLower))
                );
            }

            // ✅ Contar total NO BANCO antes da paginação
            var total = await query.CountAsync();

            // ✅ Ordenação NO BANCO
            query = filters.SortDirection?.ToLower() == "desc"
                ? query.OrderByDescending(cu => cu.User.Email)
                : query.OrderBy(cu => cu.User.Email);

            // ✅ Paginação NO BANCO (Skip/Take em IQueryable)
            var items = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return new PagedResult<CompanyUser>(items, filters.Page, filters.PageSize, total);
        }

        public async Task<CompanyUser> GetByUserAndCompanyAsync(long userId, long companyId)
        {
            return await _context.Set<CompanyUser>()
                .Include(cu => cu.Role)
                .FirstOrDefaultAsync(cu => cu.UserId == userId && cu.CompanyId == companyId);
        }

        public async Task<CompanyUser> GetOneByIdAsync(long companyUserId)
        {
            return await _context.Set<CompanyUser>()
                .Include(cu => cu.User)
                .Include(cu => cu.Role)
                .Include(cu => cu.Company)
                .FirstOrDefaultAsync(cu => cu.CompanyUserId == companyUserId);
        }

        public async Task<Role> GetUserRoleInCompanyAsync(long userId, long companyId)
        {
            var companyUser = await _context.Set<CompanyUser>()
                .Include(cu => cu.Role)
                .FirstOrDefaultAsync(cu => cu.UserId == userId && cu.CompanyId == companyId);

            return companyUser?.Role;
        }

        public async Task<bool> UserHasAccessToCompanyAsync(long userId, long companyId)
        {
            return await _context.Set<CompanyUser>()
                .AnyAsync(cu => cu.UserId == userId && cu.CompanyId == companyId);
        }

        public async Task<CompanyUser> CreateAsync(CompanyUser entity)
        {
            await _context.Set<CompanyUser>().AddAsync(entity);
            return entity;
        }

        public async Task<CompanyUser> UpdateByIdAsync(long companyUserId, CompanyUser entity)
        {
            if (companyUserId <= 0)
                throw new ValidationException(nameof(companyUserId), "CompanyUserId deve ser maior que zero.");

            var existing = await _context.Set<CompanyUser>().FindAsync(companyUserId);
            
            if (existing == null)
                throw new EntityNotFoundException("CompanyUser", companyUserId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long companyUserId)
        {
            if (companyUserId <= 0)
                throw new ValidationException(nameof(companyUserId), "CompanyUserId deve ser maior que zero.");

            var existing = await _context.Set<CompanyUser>().FindAsync(companyUserId);
            
            if (existing == null)
                throw new EntityNotFoundException("CompanyUser", companyUserId);

            _context.Set<CompanyUser>().Remove(existing);
            return true;
        }
    }
}
