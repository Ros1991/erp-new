using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IContractBenefitDiscountRepository
    {
        Task<ContractBenefitDiscount?> GetOneByIdAsync(long id);
    }
}
