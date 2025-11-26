using ERP.Application.DTOs;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly ErpContext _context;

        public ContractRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<Contract>> GetAllByEmployeeIdAsync(long employeeId)
        {
            return await _context.Set<Contract>()
                .Include(c => c.Employee)
                .Include(c => c.ContractBenefitDiscountList)
                .Include(c => c.ContractCostCenterList)
                    .ThenInclude(ccc => ccc.CostCenter)
                .Where(c => c.EmployeeId == employeeId)
                .OrderByDescending(c => c.IsActive)
                .ThenByDescending(c => c.StartDate)
                .ToListAsync();
        }

        public async Task<Contract> GetActiveByEmployeeIdAsync(long employeeId)
        {
            return await _context.Set<Contract>()
                .Include(c => c.Employee)
                .Include(c => c.ContractBenefitDiscountList)
                .Include(c => c.ContractCostCenterList)
                    .ThenInclude(ccc => ccc.CostCenter)
                .Where(c => c.EmployeeId == employeeId && c.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Contract>> GetActivePayrollContractsByCompanyAsync(long companyId)
        {
            return await _context.Set<Contract>()
                .Include(c => c.Employee)
                .Include(c => c.ContractBenefitDiscountList)
                .Include(c => c.ContractCostCenterList)
                    .ThenInclude(ccc => ccc.CostCenter)
                .Where(c => c.Employee.CompanyId == companyId &&
                           c.IsActive == true &&
                           c.IsPayroll == true)
                .OrderBy(c => c.Employee.Nickname)
                .ToListAsync();
        }

        public async Task<Contract> GetOneByIdAsync(long contractId)
        {
            return await _context.Set<Contract>()
                .Include(c => c.Employee)
                .Include(c => c.ContractBenefitDiscountList)
                .Include(c => c.ContractCostCenterList)
                    .ThenInclude(ccc => ccc.CostCenter)
                .FirstOrDefaultAsync(c => c.ContractId == contractId);
        }

        public async Task<Contract> CreateAsync(Contract entity)
        {
            await _context.Set<Contract>().AddAsync(entity);
            return entity;
        }

        public async Task<Contract> UpdateByIdAsync(long contractId, Contract entity)
        {
            var existingEntity = await _context.Set<Contract>().FindAsync(contractId);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            }
            return existingEntity;
        }

        public async Task<bool> DeleteByIdAsync(long contractId)
        {
            var entity = await _context.Set<Contract>().FindAsync(contractId);
            if (entity != null)
            {
                _context.Set<Contract>().Remove(entity);
                return true;
            }
            return false;
        }

        public async Task<bool> DeactivateOtherContractsAsync(long employeeId, long currentContractId)
        {
            var contracts = await _context.Set<Contract>()
                .Where(c => c.EmployeeId == employeeId && c.ContractId != currentContractId && c.IsActive)
                .ToListAsync();

            foreach (var contract in contracts)
            {
                contract.IsActive = false;
                contract.EndDate = DateTime.UtcNow.Date;
            }

            return contracts.Any();
        }
    }
}
