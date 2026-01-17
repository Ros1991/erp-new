using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class ContractBenefitDiscountRepository : IContractBenefitDiscountRepository
    {
        private readonly ErpContext _context;

        public ContractBenefitDiscountRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<ContractBenefitDiscount?> GetOneByIdAsync(long id)
        {
            return await _context.Set<ContractBenefitDiscount>()
                .FirstOrDefaultAsync(cbd => cbd.ContractBenefitDiscountId == id);
        }
    }
}
