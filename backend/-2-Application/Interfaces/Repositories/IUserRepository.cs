using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
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
