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
