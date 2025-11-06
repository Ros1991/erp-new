using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
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
                .ToListAsync();
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
