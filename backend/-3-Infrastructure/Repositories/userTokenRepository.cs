using CAU.Application.Interfaces.Repositories;
using CAU.Domain.Entities;
using CAU.Infrastructure.Data;
using CAU.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CAU.Infrastructure.Repositories
{
    public class UserTokenRepository : IUserTokenRepository
    {
        private readonly ErpContext _context;

        public UserTokenRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<UserToken>> GetAllAsync()
        {
            return await _context.Set<UserToken>().ToListAsync();
        }

        public async Task<UserToken> GetOneByIdAsync(long UserTokenId)
        {
            return await _context.Set<UserToken>().FindAsync(UserTokenId);
        }

        public async Task<UserToken> CreateAsync(UserToken entity)
        {
            await _context.Set<UserToken>().AddAsync(entity);
            return entity;
        }

        public async Task<UserToken> UpdateByIdAsync(long UserTokenId, UserToken entity)
        {
            if (UserTokenId <= 0)
                throw new ValidationException(nameof(UserTokenId), "UserTokenId deve ser maior que zero.");

            var existing = await _context.Set<UserToken>().FindAsync(UserTokenId);
            
            if (existing == null)
                throw new EntityNotFoundException("UserToken", UserTokenId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long UserTokenId)
        {
            if (UserTokenId <= 0)
                throw new ValidationException(nameof(UserTokenId), "UserTokenId deve ser maior que zero.");

            var existing = await _context.Set<UserToken>().FindAsync(UserTokenId);
            
            if (existing == null)
                throw new EntityNotFoundException("UserToken", UserTokenId);

            _context.Set<UserToken>().Remove(existing);
            return true;
        }
    }
}
