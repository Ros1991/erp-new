using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class PayrollEmployeeRepository : IPayrollEmployeeRepository
    {
        private readonly ErpContext _context;

        public PayrollEmployeeRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<PayrollEmployee>> GetAllByPayrollIdAsync(long payrollId)
        {
            return await _context.Set<PayrollEmployee>()
                .Include(pe => pe.Employee)
                .Include(pe => pe.Contract)
                .Include(pe => pe.PayrollItemList)
                .Where(pe => pe.PayrollId == payrollId)
                .OrderBy(pe => pe.Employee.Nickname)
                .ToListAsync();
        }

        public async Task<PayrollEmployee> GetOneByIdAsync(long payrollEmployeeId)
        {
            return await _context.Set<PayrollEmployee>()
                .Include(pe => pe.Employee)
                .Include(pe => pe.Contract)
                .Include(pe => pe.PayrollItemList)
                .FirstOrDefaultAsync(pe => pe.PayrollEmployeeId == payrollEmployeeId);
        }

        public async Task<PayrollEmployee> GetByPayrollAndContractAsync(long payrollId, long contractId)
        {
            return await _context.Set<PayrollEmployee>()
                .Include(pe => pe.Employee)
                .Include(pe => pe.Contract)
                .Include(pe => pe.PayrollItemList)
                .FirstOrDefaultAsync(pe => pe.PayrollId == payrollId && pe.ContractId == contractId);
        }

        public async Task<PayrollEmployee?> GetPreviousPayrollEmployeeAsync(long employeeId, long currentPayrollId)
        {
            // Buscar a folha atual para pegar a data de referência
            var currentPayroll = await _context.Set<Payroll>().FindAsync(currentPayrollId);
            if (currentPayroll == null) return null;

            // Buscar a folha anterior mais recente para este empregado
            return await _context.Set<PayrollEmployee>()
                .Include(pe => pe.Payroll)
                .Where(pe => pe.EmployeeId == employeeId && 
                             pe.PayrollId != currentPayrollId &&
                             pe.Payroll.PeriodEndDate < currentPayroll.PeriodStartDate)
                .OrderByDescending(pe => pe.Payroll.PeriodEndDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<PayrollEmployee>> GetByEmployeeIdAsync(long employeeId)
        {
            return await _context.Set<PayrollEmployee>()
                .Include(pe => pe.Payroll)
                .Include(pe => pe.PayrollItemList)
                .Where(pe => pe.EmployeeId == employeeId)
                .OrderBy(pe => pe.Payroll.PeriodStartDate)
                .ToListAsync();
        }

        public async Task<PayrollEmployee> CreateAsync(PayrollEmployee entity)
        {
            await _context.Set<PayrollEmployee>().AddAsync(entity);
            return entity;
        }

        public async Task<PayrollEmployee> UpdateByIdAsync(long payrollEmployeeId, PayrollEmployee entity)
        {
            if (payrollEmployeeId <= 0)
                throw new ValidationException(nameof(payrollEmployeeId), "PayrollEmployeeId deve ser maior que zero.");

            var existing = await _context.Set<PayrollEmployee>().FindAsync(payrollEmployeeId);
            
            if (existing == null)
                throw new EntityNotFoundException("PayrollEmployee", payrollEmployeeId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long payrollEmployeeId)
        {
            if (payrollEmployeeId <= 0)
                throw new ValidationException(nameof(payrollEmployeeId), "PayrollEmployeeId deve ser maior que zero.");

            var existing = await _context.Set<PayrollEmployee>().FindAsync(payrollEmployeeId);
            
            if (existing == null)
                throw new EntityNotFoundException("PayrollEmployee", payrollEmployeeId);

            _context.Set<PayrollEmployee>().Remove(existing);
            return true;
        }
    }
}
