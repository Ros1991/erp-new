using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.Domain.Entities;
using System.Threading.Tasks;

namespace ERP.Application.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PayrollService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PayrollOutputDTO>> GetAllAsync(long companyId)
        {
            var entities = await _unitOfWork.PayrollRepository.GetAllAsync(companyId);
            return PayrollMapper.ToPayrollOutputDTOList(entities);
        }

        public async Task<PagedResult<PayrollOutputDTO>> GetPagedAsync(long companyId, PayrollFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.PayrollRepository.GetPagedAsync(companyId, filters);
            
            // Buscar a última folha para marcar no resultado
            var lastPayroll = await _unitOfWork.PayrollRepository.GetLastPayrollAsync(companyId);
            var lastPayrollId = lastPayroll?.PayrollId ?? 0;

            // Mapear para DTO com informações extras
            var dtoItems = new List<PayrollOutputDTO>();
            foreach (var entity in pagedEntities.Items)
            {
                var employeeCount = await _unitOfWork.PayrollRepository.GetEmployeeCountAsync(entity.PayrollId);
                var isLastPayroll = entity.PayrollId == lastPayrollId;
                
                var dto = PayrollMapper.ToPayrollOutputDTO(entity, employeeCount, isLastPayroll);
                dtoItems.Add(dto);
            }
            
            return new PagedResult<PayrollOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<PayrollOutputDTO> GetOneByIdAsync(long payrollId)
        {
            var entity = await _unitOfWork.PayrollRepository.GetOneByIdWithIncludesAsync(payrollId);
            if (entity == null)
            {
                throw new EntityNotFoundException("Payroll", payrollId);
            }

            var employeeCount = await _unitOfWork.PayrollRepository.GetEmployeeCountAsync(payrollId);
            var lastPayroll = await _unitOfWork.PayrollRepository.GetLastPayrollAsync(entity.CompanyId);
            var isLastPayroll = entity.PayrollId == lastPayroll?.PayrollId;

            return PayrollMapper.ToPayrollOutputDTO(entity, employeeCount, isLastPayroll);
        }

        public async Task<PayrollOutputDTO> CreatePayrollAsync(long companyId, long currentUserId, PayrollInputDTO dto)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // Validar se já existe folha para o período
            var periodStart = DateTime.SpecifyKind(dto.PeriodStartDate, DateTimeKind.Utc);
            var periodEnd = DateTime.SpecifyKind(dto.PeriodEndDate, DateTimeKind.Utc);
            var existingPayroll = await _unitOfWork.PayrollRepository
                .GetByCompanyAndPeriodAsync(companyId, periodStart, periodEnd);
            
            if (existingPayroll != null)
            {
                throw new ValidationException("Payroll", "Já existe uma folha de pagamento para este período.");
            }

            // Criar a folha (sem transação ainda, pois vamos criar muitos registros)
            var payroll = PayrollMapper.ToEntity(dto, companyId, currentUserId);
            payroll.Notes = dto.Notes;
            
            var createdPayroll = await _unitOfWork.PayrollRepository.CreateAsync(payroll);
            await _unitOfWork.SaveChangesAsync();

            // Buscar contratos ativos elegíveis para folha
            var activeContracts = await _unitOfWork.ContractRepository
                .GetActivePayrollContractsByCompanyAsync(companyId);

            if (!activeContracts.Any())
            {
                throw new ValidationException("Payroll", "Não há contratos ativos elegíveis para folha de pagamento.");
            }

            // Para cada contrato, criar PayrollEmployee e seus itens
            foreach (var contract in activeContracts)
            {
                await CreatePayrollEmployeeWithItemsAsync(createdPayroll, contract, currentUserId);
            }

            // Recalcular totais da folha
            await RecalculatePayrollTotalsAsync(createdPayroll.PayrollId);

            // Buscar folha completa com includes para retornar
            var result = await GetOneByIdAsync(createdPayroll.PayrollId);
            return result;
        }

        public async Task<PayrollOutputDTO> UpdateByIdAsync(long payrollId, PayrollInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var existingEntity = await _unitOfWork.PayrollRepository.GetOneByIdAsync(payrollId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("Payroll", payrollId);
            }

            // Validar se a folha está fechada (não pode editar)
            if (existingEntity.IsClosed)
            {
                throw new ValidationException("Payroll", "Não é possível editar uma folha de pagamento fechada.");
            }

            PayrollMapper.UpdateEntity(existingEntity, dto, currentUserId);
            
            await _unitOfWork.SaveChangesAsync();
            return await GetOneByIdAsync(payrollId);
        }

        public async Task<bool> DeleteByIdAsync(long payrollId)
        {
            var existingEntity = await _unitOfWork.PayrollRepository.GetOneByIdAsync(payrollId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("Payroll", payrollId);
            }

            // Validar se é a última folha (só pode deletar a última)
            var lastPayroll = await _unitOfWork.PayrollRepository.GetLastPayrollAsync(existingEntity.CompanyId);
            if (lastPayroll == null || existingEntity.PayrollId != lastPayroll.PayrollId)
            {
                throw new ValidationException("Payroll", "Só é possível excluir a última folha de pagamento criada.");
            }

            // Validar se a folha está fechada (não pode deletar)
            if (existingEntity.IsClosed)
            {
                throw new ValidationException("Payroll", "Não é possível excluir uma folha de pagamento fechada.");
            }

            var result = await _unitOfWork.PayrollRepository.DeleteByIdAsync(payrollId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        // ========== MÉTODOS PRIVADOS ==========

        private async System.Threading.Tasks.Task CreatePayrollEmployeeWithItemsAsync(Payroll payroll, Contract contract, long userId)
        {
            var now = DateTime.UtcNow;

            // 1. Criar PayrollEmployee
            var payrollEmployee = new PayrollEmployee
            {
                PayrollId = payroll.PayrollId,
                EmployeeId = contract.EmployeeId,
                ContractId = contract.ContractId,
                BaseSalary = contract.Value,
                IsOnVacation = false,
                TotalGrossPay = 0,
                TotalDeductions = 0,
                TotalNetPay = 0,
                CriadoPor = userId,
                CriadoEm = now
            };

            var createdPayrollEmployee = await _unitOfWork.PayrollEmployeeRepository.CreateAsync(payrollEmployee);
            await _unitOfWork.SaveChangesAsync();

            var items = new List<PayrollItem>();

            // 2. Criar item de salário base
            items.Add(new PayrollItem
            {
                PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                Description = "Salário Base",
                Type = "Provento",
                Category = "Salario",
                Amount = Convert.ToInt64(contract.Value),
                SourceType = "contract_benefit",
                ReferenceId = contract.ContractId,
                IsManual = false,
                IsActive = true,
                CriadoPor = userId,
                CriadoEm = now
            });

            // 3. Adicionar benefícios do contrato
            var benefits = contract.ContractBenefitDiscountList?
                .Where(bd => bd.Type == "Provento")
                .ToList() ?? new List<ContractBenefitDiscount>();

            foreach (var benefit in benefits)
            {
                items.Add(new PayrollItem
                {
                    PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                    Description = benefit.Description,
                    Type = "Provento",
                    Category = "Beneficio",
                    Amount = Convert.ToInt64(benefit.Amount),
                    SourceType = "contract_benefit",
                    ReferenceId = benefit.ContractBenefitDiscountId,
                    IsManual = false,
                    IsActive = true,
                    CriadoPor = userId,
                    CriadoEm = now
                });
            }

            // 4. Adicionar descontos do contrato
            var discounts = contract.ContractBenefitDiscountList?
                .Where(bd => bd.Type == "Desconto")
                .ToList() ?? new List<ContractBenefitDiscount>();

            foreach (var discount in discounts)
            {
                items.Add(new PayrollItem
                {
                    PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                    Description = discount.Description,
                    Type = "Desconto",
                    Category = "Desconto",
                    Amount = Convert.ToInt64(discount.Amount),
                    SourceType = "contract_discount",
                    ReferenceId = discount.ContractBenefitDiscountId,
                    IsManual = false,
                    IsActive = true,
                    CriadoPor = userId,
                    CriadoEm = now
                });
            }

            // 5. Buscar e adicionar empréstimos pendentes
            var pendingLoans = await _unitOfWork.LoanAdvanceRepository
                .GetPendingLoansByEmployeeAsync(contract.EmployeeId, payroll.PeriodEndDate);

            foreach (var loan in pendingLoans)
            {
                // Calcular próxima parcela a ser cobrada
                var nextInstallment = await _unitOfWork.PayrollItemRepository
                    .GetNextInstallmentNumberAsync(loan.LoanAdvanceId);

                // Verificar se ainda há parcelas a cobrar
                if (nextInstallment <= loan.Installments)
                {
                    var installmentAmount = loan.Amount / loan.Installments;

                    items.Add(new PayrollItem
                    {
                        PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                        Description = $"Empréstimo #{loan.LoanAdvanceId} - Parcela {nextInstallment}/{loan.Installments}",
                        Type = "Desconto",
                        Category = "Emprestimo",
                        Amount = Convert.ToInt64(installmentAmount),
                        SourceType = "loan",
                        ReferenceId = loan.LoanAdvanceId,
                        InstallmentNumber = nextInstallment,
                        InstallmentTotal = (int)loan.Installments,
                        IsManual = false,
                        IsActive = true,
                        CriadoPor = userId,
                        CriadoEm = now
                    });
                }
            }

            // 6. Salvar todos os itens
            foreach (var item in items)
            {
                await _unitOfWork.PayrollItemRepository.CreateAsync(item);
            }
            await _unitOfWork.SaveChangesAsync();

            // 7. Calcular totais do PayrollEmployee
            await RecalculatePayrollEmployeeTotalsAsync(createdPayrollEmployee.PayrollEmployeeId);
        }

        private async System.Threading.Tasks.Task RecalculatePayrollEmployeeTotalsAsync(long payrollEmployeeId)
        {
            var payrollEmployee = await _unitOfWork.PayrollEmployeeRepository.GetOneByIdAsync(payrollEmployeeId);
            if (payrollEmployee == null) return;

            // Buscar todos os itens ativos do empregado
            var items = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(payrollEmployeeId);
            var activeItems = items.Where(i => i.IsActive).ToList();

            // Calcular totais
            var totalGrossPay = activeItems
                .Where(i => i.Type == "Provento")
                .Sum(i => i.Amount);

            var totalDeductions = activeItems
                .Where(i => i.Type == "Desconto")
                .Sum(i => i.Amount);

            var totalNetPay = totalGrossPay - totalDeductions;

            // Atualizar PayrollEmployee
            payrollEmployee.TotalGrossPay = Convert.ToInt64(totalGrossPay);
            payrollEmployee.TotalDeductions = Convert.ToInt64(totalDeductions);
            payrollEmployee.TotalNetPay = Convert.ToInt64(totalNetPay);

            await _unitOfWork.SaveChangesAsync();
        }

        private async System.Threading.Tasks.Task RecalculatePayrollTotalsAsync(long payrollId)
        {
            var payroll = await _unitOfWork.PayrollRepository.GetOneByIdAsync(payrollId);
            if (payroll == null) return;

            // Buscar todos os empregados da folha
            var employees = await _unitOfWork.PayrollEmployeeRepository.GetAllByPayrollIdAsync(payrollId);

            // Somar totais de todos os empregados
            var totalGrossPay = employees.Sum(e => e.TotalGrossPay);
            var totalDeductions = employees.Sum(e => e.TotalDeductions);
            var totalNetPay = employees.Sum(e => e.TotalNetPay);

            // Atualizar Payroll
            payroll.TotalGrossPay = totalGrossPay;
            payroll.TotalDeductions = totalDeductions;
            payroll.TotalNetPay = totalNetPay;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
