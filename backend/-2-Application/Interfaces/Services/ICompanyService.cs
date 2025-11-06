using CAU.Application.DTOs;
using CAU.Application.DTOs.Base;

namespace CAU.Application.Interfaces.Services
{
    public interface ICompanyService
    {
		Task<List<CompanyOutputDTO>> GetAllAsync();
		Task<PagedResult<CompanyOutputDTO>> GetPagedAsync(CompanyFilterDTO filters);
		Task<CompanyOutputDTO> GetOneByIdAsync(long CompanyId);
		Task<CompanyOutputDTO> CreateAsync(CompanyInputDTO dto);
		Task<CompanyOutputDTO> UpdateByIdAsync(long CompanyId, CompanyInputDTO dto);
		Task<bool> DeleteByIdAsync(long CompanyId);
    }
}
