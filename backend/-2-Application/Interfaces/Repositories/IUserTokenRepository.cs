using CAU.Domain.Entities;

namespace CAU.Application.Interfaces.Repositories
{
    public interface IUserTokenRepository
    {
        Task<List<UserToken>> GetAllAsync();
        Task<UserToken> GetOneByIdAsync(long UserTokenId);
        Task<UserToken> CreateAsync(UserToken entity);
        Task<UserToken> UpdateByIdAsync(long UserTokenId, UserToken entity);
        Task<bool> DeleteByIdAsync(long UserTokenId);
    }
}
