using CAU.Application.DTOs;
using CAU.Application.DTOs.Base;

namespace CAU.Application.Interfaces.Services
{
    public interface IUserService
    {
		Task<List<UserOutputDTO>> GetAllAsync();
		Task<PagedResult<UserOutputDTO>> GetPagedAsync(UserFilterDTO filters);
		Task<UserOutputDTO> GetOneByIdAsync(long UserId);
		Task<UserOutputDTO> CreateAsync(UserInputDTO dto);
		Task<UserOutputDTO> UpdateByIdAsync(long UserId, UserInputDTO dto);
		Task<bool> DeleteByIdAsync(long UserId);
    }
}
