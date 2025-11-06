using CAU.Application.DTOs;
using CAU.Application.DTOs.Base;
using CAU.Domain.Entities;

namespace CAU.Application.Interfaces.Repositories
{
	public interface IUserRepository
	{
		Task<List<User>> GetAllAsync();
		Task<PagedResult<User>> GetPagedAsync(UserFilterDTO filters);
		Task<User> GetOneByIdAsync(long UserId);
		Task<User> CreateAsync(User entity);
		Task<User> UpdateByIdAsync(long UserId, User entity);
		Task<bool> DeleteByIdAsync(long UserId);
	}
}
