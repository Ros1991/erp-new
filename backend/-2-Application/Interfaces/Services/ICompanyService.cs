using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface ICompanyService
    {
		Task<List<CompanyOutputDTO>> GetAllAsync(long userId);
		Task<PagedResult<CompanyOutputDTO>> GetPagedAsync(CompanyFilterDTO filters, long userId);
		Task<CompanyOutputDTO> GetOneByIdAsync(long CompanyId);
		Task<CompanyOutputDTO> CreateAsync(CompanyInputDTO dto, long currentUserId);
		Task<CompanyOutputDTO> UpdateByIdAsync(long CompanyId, CompanyInputDTO dto, long currentUserId);
		Task<bool> DeleteByIdAsync(long CompanyId, long currentUserId);
    }
}
