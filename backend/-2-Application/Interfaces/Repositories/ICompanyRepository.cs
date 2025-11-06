using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
	public interface ICompanyRepository
	{
		Task<List<Company>> GetAllAsync();
		Task<PagedResult<Company>> GetPagedAsync(CompanyFilterDTO filters);
		Task<Company> GetOneByIdAsync(long CompanyId);
		Task<Company> CreateAsync(Company entity);
		Task<Company> UpdateByIdAsync(long CompanyId, Company entity);
		Task<bool> DeleteByIdAsync(long CompanyId);
	}
}
