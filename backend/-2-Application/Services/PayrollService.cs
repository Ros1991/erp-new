using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.Domain.Entities;
using ERP.Domain.Enums;
using Task = System.Threading.Tasks.Task;

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

        public async Task<PayrollDetailedOutputDTO> GetDetailedByIdAsync(long payrollId)
        {
            var entity = await _unitOfWork.PayrollRepository.GetOneByIdWithIncludesAsync(payrollId);
            if (entity == null)
            {
                throw new EntityNotFoundException("Payroll", payrollId);
            }

            var employeeCount = await _unitOfWork.PayrollRepository.GetEmployeeCountAsync(payrollId);
            var lastPayroll = await _unitOfWork.PayrollRepository.GetLastPayrollAsync(entity.CompanyId);
            var isLastPayroll = entity.PayrollId == lastPayroll?.PayrollId;

            // Buscar empregados da folha com seus itens
            var payrollEmployees = await _unitOfWork.PayrollEmployeeRepository.GetAllByPayrollIdAsync(payrollId);
            
            var employeeDetailsList = new List<PayrollEmployeeDetailedDTO>();
            
            foreach (var payrollEmployee in payrollEmployees)
            {
                // Buscar dados do empregado
                var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(payrollEmployee.EmployeeId);
                
                // Buscar dados do contrato
                Contract? contract = null;
                if (payrollEmployee.ContractId.HasValue)
                {
                    contract = await _unitOfWork.ContractRepository.GetOneByIdAsync(payrollEmployee.ContractId.Value);
                }
                
                // Buscar itens do empregado na folha
                var payrollItems = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(payrollEmployee.PayrollEmployeeId);
                
                var itemsList = payrollItems.Select(item => new PayrollItemDetailedDTO
                {
                    PayrollItemId = item.PayrollItemId,
                    PayrollEmployeeId = item.PayrollEmployeeId,
                    Description = item.Description,
                    Type = item.Type,
                    Category = item.Category,
                    Amount = item.Amount,
                    ReferenceId = item.ReferenceId,
                    CalculationBasis = item.CalculationBasis,
                    CalculationDetails = item.CalculationDetails
                }).ToList();
                
                employeeDetailsList.Add(new PayrollEmployeeDetailedDTO
                {
                    PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                    PayrollId = payrollEmployee.PayrollId,
                    EmployeeId = payrollEmployee.EmployeeId,
                    EmployeeName = employee?.Nickname ?? employee?.FullName ?? "Desconhecido",
                    EmployeeDocument = employee?.Cpf,
                    IsOnVacation = payrollEmployee.IsOnVacation,
                    VacationDays = payrollEmployee.VacationDays,
                    VacationAdvanceAmount = payrollEmployee.VacationAdvanceAmount,
                    VacationAdvancePaid = payrollEmployee.VacationAdvancePaid,
                    VacationStartDate = payrollEmployee.VacationStartDate,
                    VacationEndDate = payrollEmployee.VacationEndDate,
                    VacationNotes = payrollEmployee.VacationNotes,
                    TotalGrossPay = payrollEmployee.TotalGrossPay,
                    TotalDeductions = payrollEmployee.TotalDeductions,
                    TotalNetPay = payrollEmployee.TotalNetPay,
                    ContractId = payrollEmployee.ContractId,
                    ContractType = contract?.Type,
                    ContractValue = contract?.Value,
                    WorkedUnits = payrollEmployee.WorkedUnits,
                    HasFgts = contract?.HasFgts ?? false,
                    Items = itemsList
                });
            }

            return new PayrollDetailedOutputDTO
            {
                PayrollId = entity.PayrollId,
                CompanyId = entity.CompanyId,
                PeriodStartDate = entity.PeriodStartDate,
                PeriodEndDate = entity.PeriodEndDate,
                TotalGrossPay = entity.TotalGrossPay,
                TotalDeductions = entity.TotalDeductions,
                TotalNetPay = entity.TotalNetPay,
                TotalInss = entity.TotalInss,
                TotalFgts = entity.TotalFgts,
                ThirteenthPercentage = entity.ThirteenthPercentage,
                ThirteenthTaxOption = entity.ThirteenthTaxOption,
                IsClosed = entity.IsClosed,
                ClosedAt = entity.ClosedAt,
                ClosedBy = entity.ClosedBy,
                ClosedByName = entity.ClosedByUser?.Email,
                Notes = entity.Notes,
                Snapshot = entity.Snapshot,
                EmployeeCount = employeeCount,
                IsLastPayroll = isLastPayroll,
                CriadoPor = entity.CriadoPor,
                CriadoPorNome = "",
                AtualizadoPor = entity.AtualizadoPor,
                AtualizadoPorNome = null,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm,
                Employees = employeeDetailsList
            };
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

            // Buscar contratos ativos elegíveis para folha (considerando período de validade)
            var activeContracts = await _unitOfWork.ContractRepository
                .GetActivePayrollContractsByCompanyAsync(companyId, periodStart, periodEnd);

            if (!activeContracts.Any())
            {
                throw new ValidationException("Payroll", "Não há contratos ativos elegíveis para folha de pagamento neste período.");
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

            // Validar se a folha está fechada (não pode deletar)
            if (existingEntity.IsClosed)
            {
                throw new ValidationException("Payroll", "Não é possível excluir uma folha de pagamento fechada.");
            }

            // Deletar registros filhos primeiro
            var payrollEmployees = await _unitOfWork.PayrollEmployeeRepository.GetAllByPayrollIdAsync(payrollId);
            foreach (var pe in payrollEmployees)
            {
                // Deletar itens do funcionário
                var items = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(pe.PayrollEmployeeId);
                foreach (var item in items)
                {
                    await _unitOfWork.PayrollItemRepository.DeleteByIdAsync(item.PayrollItemId);
                }
                // Deletar o empregado da folha
                await _unitOfWork.PayrollEmployeeRepository.DeleteByIdAsync(pe.PayrollEmployeeId);
            }

            // Deletar a folha
            var result = await _unitOfWork.PayrollRepository.DeleteByIdAsync(payrollId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<PayrollDetailedOutputDTO> RecalculatePayrollAsync(long payrollId, long currentUserId)
        {
            var payroll = await _unitOfWork.PayrollRepository.GetOneByIdAsync(payrollId);
            if (payroll == null)
            {
                throw new EntityNotFoundException("Payroll", payrollId);
            }

            // Validar se a folha está fechada
            if (payroll.IsClosed)
            {
                throw new ValidationException("Payroll", "Não é possível recalcular uma folha de pagamento fechada.");
            }

            // 1. Deletar todos os PayrollEmployees existentes (cascade delete remove os PayrollItems)
            var existingEmployees = await _unitOfWork.PayrollEmployeeRepository.GetAllByPayrollIdAsync(payrollId);
            foreach (var pe in existingEmployees)
            {
                // Deletar itens primeiro
                var items = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(pe.PayrollEmployeeId);
                foreach (var item in items)
                {
                    await _unitOfWork.PayrollItemRepository.DeleteByIdAsync(item.PayrollItemId);
                }
                // Deletar o empregado da folha
                await _unitOfWork.PayrollEmployeeRepository.DeleteByIdAsync(pe.PayrollEmployeeId);
            }
            await _unitOfWork.SaveChangesAsync();

            // 2. Buscar contratos ativos elegíveis para folha (considerando período de validade)
            var activeContracts = await _unitOfWork.ContractRepository
                .GetActivePayrollContractsByCompanyAsync(payroll.CompanyId, payroll.PeriodStartDate, payroll.PeriodEndDate);

            if (!activeContracts.Any())
            {
                // Zerar totais da folha se não houver contratos
                payroll.TotalGrossPay = 0;
                payroll.TotalDeductions = 0;
                payroll.TotalNetPay = 0;
                payroll.AtualizadoPor = currentUserId;
                payroll.AtualizadoEm = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
                
                return await GetDetailedByIdAsync(payrollId);
            }

            // 3. Para cada contrato, criar PayrollEmployee e seus itens
            foreach (var contract in activeContracts)
            {
                await CreatePayrollEmployeeWithItemsAsync(payroll, contract, currentUserId);
            }

            // 4. Recalcular totais da folha
            await RecalculatePayrollTotalsAsync(payrollId);

            // 5. Atualizar informações de modificação
            payroll.AtualizadoPor = currentUserId;
            payroll.AtualizadoEm = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            // 6. Retornar folha detalhada
            return await GetDetailedByIdAsync(payrollId);
        }

        public async Task<PayrollEmployeeDetailedDTO> RecalculatePayrollEmployeeAsync(long payrollEmployeeId, long currentUserId)
        {
            // 1. Buscar o PayrollEmployee existente
            var payrollEmployee = await _unitOfWork.PayrollEmployeeRepository.GetOneByIdAsync(payrollEmployeeId);
            if (payrollEmployee == null)
            {
                throw new EntityNotFoundException("PayrollEmployee", payrollEmployeeId);
            }

            // 2. Buscar a folha de pagamento
            var payroll = await _unitOfWork.PayrollRepository.GetOneByIdAsync(payrollEmployee.PayrollId);
            if (payroll == null)
            {
                throw new EntityNotFoundException("Payroll", payrollEmployee.PayrollId);
            }

            // 3. Validar se a folha está fechada
            if (payroll.IsClosed)
            {
                throw new ValidationException("Payroll", "Não é possível recalcular uma folha de pagamento fechada.");
            }

            // 4. Validar e buscar o contrato do funcionário
            if (!payrollEmployee.ContractId.HasValue)
            {
                throw new ValidationException("PayrollEmployee", "Funcionário não possui contrato vinculado.");
            }

            var contract = await _unitOfWork.ContractRepository.GetOneByIdAsync(payrollEmployee.ContractId.Value);
            if (contract == null)
            {
                throw new EntityNotFoundException("Contract", payrollEmployee.ContractId.Value);
            }

            // 5. Deletar itens existentes do funcionário
            var items = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(payrollEmployeeId);
            foreach (var item in items)
            {
                await _unitOfWork.PayrollItemRepository.DeleteByIdAsync(item.PayrollItemId);
            }

            // 6. Deletar o PayrollEmployee atual
            await _unitOfWork.PayrollEmployeeRepository.DeleteByIdAsync(payrollEmployeeId);
            await _unitOfWork.SaveChangesAsync();

            // 7. Recriar o PayrollEmployee com seus itens
            await CreatePayrollEmployeeWithItemsAsync(payroll, contract, currentUserId);

            // 8. Recalcular totais da folha
            await RecalculatePayrollTotalsAsync(payroll.PayrollId);

            // 9. Atualizar informações de modificação da folha
            payroll.AtualizadoPor = currentUserId;
            payroll.AtualizadoEm = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            // 10. Buscar o novo PayrollEmployee criado para retornar
            var newPayrollEmployee = await _unitOfWork.PayrollEmployeeRepository
                .GetByPayrollAndContractAsync(payroll.PayrollId, contract.ContractId);
            
            if (newPayrollEmployee == null)
            {
                throw new EntityNotFoundException("PayrollEmployee", "recém-criado");
            }

            // 11. Buscar dados complementares para o DTO
            var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(newPayrollEmployee.EmployeeId);
            var payrollItems = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(newPayrollEmployee.PayrollEmployeeId);

            var itemsList = payrollItems.Select(item => new PayrollItemDetailedDTO
            {
                PayrollItemId = item.PayrollItemId,
                PayrollEmployeeId = item.PayrollEmployeeId,
                Description = item.Description,
                Type = item.Type,
                Category = item.Category,
                Amount = item.Amount,
                ReferenceId = item.ReferenceId,
                CalculationBasis = item.CalculationBasis,
                CalculationDetails = item.CalculationDetails
            }).ToList();

            return new PayrollEmployeeDetailedDTO
            {
                PayrollEmployeeId = newPayrollEmployee.PayrollEmployeeId,
                PayrollId = newPayrollEmployee.PayrollId,
                EmployeeId = newPayrollEmployee.EmployeeId,
                EmployeeName = employee?.Nickname ?? employee?.FullName ?? "Desconhecido",
                EmployeeDocument = employee?.Cpf,
                IsOnVacation = newPayrollEmployee.IsOnVacation,
                VacationDays = newPayrollEmployee.VacationDays,
                VacationAdvanceAmount = newPayrollEmployee.VacationAdvanceAmount,
                VacationAdvancePaid = newPayrollEmployee.VacationAdvancePaid,
                VacationStartDate = newPayrollEmployee.VacationStartDate,
                VacationEndDate = newPayrollEmployee.VacationEndDate,
                VacationNotes = newPayrollEmployee.VacationNotes,
                TotalGrossPay = newPayrollEmployee.TotalGrossPay,
                TotalDeductions = newPayrollEmployee.TotalDeductions,
                TotalNetPay = newPayrollEmployee.TotalNetPay,
                ContractId = newPayrollEmployee.ContractId,
                ContractType = contract?.Type,
                ContractValue = contract?.Value,
                WorkedUnits = newPayrollEmployee.WorkedUnits,
                HasFgts = contract?.HasFgts ?? false,
                Items = itemsList
            };
        }

        // ========== MÉTODOS PRIVADOS ==========

        /// <summary>
        /// Calcula o número de dias úteis (segunda a sexta) em um período
        /// </summary>
        private int CalculateBusinessDays(DateTime startDate, DateTime endDate)
        {
            int businessDays = 0;
            var current = startDate;
            
            while (current <= endDate)
            {
                if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
                {
                    businessDays++;
                }
                current = current.AddDays(1);
            }
            
            return businessDays;
        }

        private async System.Threading.Tasks.Task CreatePayrollEmployeeWithItemsAsync(Payroll payroll, Contract contract, long userId)
        {
            var now = DateTime.UtcNow;

            // Calcular dias úteis do período da folha
            var businessDays = CalculateBusinessDays(payroll.PeriodStartDate, payroll.PeriodEndDate);
            
            // Para Horista: 8 horas por dia útil. Para Diarista: 1 dia por dia útil
            decimal? workedUnits = null;
            if (contract.Type == "Horista")
            {
                workedUnits = businessDays * 8; // 8 horas por dia útil
                Console.WriteLine($"[PAYROLL DEBUG] Horista: {businessDays} dias úteis x 8 horas = {workedUnits} horas");
            }
            else if (contract.Type == "Diarista")
            {
                workedUnits = businessDays; // 1 diária por dia útil
                Console.WriteLine($"[PAYROLL DEBUG] Diarista: {businessDays} dias úteis = {workedUnits} dias");
            }

            // 1. Criar PayrollEmployee
            var payrollEmployee = new PayrollEmployee
            {
                PayrollId = payroll.PayrollId,
                EmployeeId = contract.EmployeeId,
                ContractId = contract.ContractId,
                BaseSalary = contract.Value,
                WorkedUnits = workedUnits,
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

            // Função para calcular a proporção baseada na data de início do contrato
            // Se o contrato começou dentro do período da folha, retorna a proporção (0.0 a 1.0)
            // Se começou antes do período, retorna 1.0 (100%)
            decimal CalculateProportionalFactor(DateTime contractStartDate, DateTime periodStart, DateTime periodEnd)
            {
                // Se o contrato começou antes ou no início do período, é 100%
                if (contractStartDate <= periodStart)
                {
                    return 1.0m;
                }
                
                // Se o contrato começou depois do fim do período, é 0%
                if (contractStartDate > periodEnd)
                {
                    return 0.0m;
                }
                
                // Contrato começou dentro do período - calcular proporção
                var totalDays = (periodEnd - periodStart).Days + 1; // +1 para incluir o último dia
                var workedDays = (periodEnd - contractStartDate).Days + 1; // +1 para incluir o dia de início
                
                return (decimal)workedDays / totalDays;
            }

            // Calcular fator de proporcionalidade para este contrato (para benefícios mensais)
            var proportionalFactor = CalculateProportionalFactor(
                contract.StartDate, 
                payroll.PeriodStartDate, 
                payroll.PeriodEndDate
            );
            
            Console.WriteLine($"[PAYROLL DEBUG] ProportionalFactor (mensal): {proportionalFactor:P2} (Contract Start: {contract.StartDate:d}, Period: {payroll.PeriodStartDate:d} - {payroll.PeriodEndDate:d})");

            // Função para calcular proporção ANUAL baseada em meses trabalhados
            // Se trabalhou 12+ meses, recebe 100%. Se trabalhou 6 meses, recebe 50% (6/12)
            decimal CalculateAnnualProportionalFactor(DateTime contractStartDate, DateTime referenceDate)
            {
                // Calcular meses completos de trabalho
                var totalMonths = ((referenceDate.Year - contractStartDate.Year) * 12) + (referenceDate.Month - contractStartDate.Month);
                
                // Ajustar se ainda não completou o mês
                if (referenceDate.Day < contractStartDate.Day)
                {
                    totalMonths--;
                }
                
                // Se trabalhou 12+ meses, é 100%
                if (totalMonths >= 12)
                {
                    return 1.0m;
                }
                
                // Se ainda não trabalhou nem 1 mês completo
                if (totalMonths <= 0)
                {
                    // Calcular proporção de dias do primeiro mês
                    var daysInMonth = DateTime.DaysInMonth(contractStartDate.Year, contractStartDate.Month);
                    var workedDays = (referenceDate - contractStartDate).Days + 1;
                    return Math.Max(0, (decimal)workedDays / daysInMonth / 12);
                }
                
                // Proporcional aos meses trabalhados (1 a 11 meses)
                return (decimal)totalMonths / 12;
            }
            
            // Calcular fator de proporcionalidade anual (para benefícios anuais)
            var annualProportionalFactor = CalculateAnnualProportionalFactor(
                contract.StartDate,
                payroll.PeriodEndDate
            );
            
            Console.WriteLine($"[PAYROLL DEBUG] ProportionalFactor (anual): {annualProportionalFactor:P2} (Contract Start: {contract.StartDate:d}, Reference: {payroll.PeriodEndDate:d})");

            // 2. Criar item de salário base
            // Para Horista/Diarista: valor do contrato × unidades trabalhadas
            // Para Mensalista: valor do contrato (proporcional se iniciou no período)
            var salaryAmount = contract.Value;
            var salaryDescription = "Salário Base";
            
            if (contract.Type == "Horista" && workedUnits.HasValue)
            {
                salaryAmount = (long)Math.Round(contract.Value * workedUnits.Value);
                salaryDescription = $"Salário Base ({workedUnits.Value:0.##} horas)";
                Console.WriteLine($"[PAYROLL DEBUG] Salário Horista: {contract.Value} x {workedUnits.Value} horas = {salaryAmount}");
            }
            else if (contract.Type == "Diarista" && workedUnits.HasValue)
            {
                salaryAmount = (long)Math.Round(contract.Value * workedUnits.Value);
                salaryDescription = $"Salário Base ({workedUnits.Value:0.##} dias)";
                Console.WriteLine($"[PAYROLL DEBUG] Salário Diarista: {contract.Value} x {workedUnits.Value} dias = {salaryAmount}");
            }
            else if (proportionalFactor < 1.0m)
            {
                salaryAmount = (long)Math.Round(contract.Value * proportionalFactor);
                salaryDescription = $"Salário Base (proporcional {proportionalFactor:P0})";
                Console.WriteLine($"[PAYROLL DEBUG] Salário proporcional: Original: {contract.Value} -> Proporcional: {salaryAmount}");
            }
            
            items.Add(new PayrollItem
            {
                PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                Description = salaryDescription,
                Type = "Provento",
                Category = "Salario",
                Amount = Convert.ToInt64(salaryAmount),
                SourceType = "contract_benefit",
                ReferenceId = contract.ContractId,
                IsManual = false,
                IsActive = true,
                CriadoPor = userId,
                CriadoEm = now
            });

            // 2.5. Verificar se o empregado teve férias na folha anterior e descontar o adiantamento
            var previousPayrollEmployee = await _unitOfWork.PayrollEmployeeRepository
                .GetPreviousPayrollEmployeeAsync(contract.EmployeeId, payroll.PayrollId);
            
            if (previousPayrollEmployee != null && previousPayrollEmployee.VacationAdvancePaid && previousPayrollEmployee.VacationAdvanceAmount.HasValue && previousPayrollEmployee.VacationAdvanceAmount.Value > 0)
            {
                // Descontar o adiantamento de férias da folha anterior
                var advanceAmount = previousPayrollEmployee.VacationAdvanceAmount.Value;
                var vacationDays = previousPayrollEmployee.VacationDays ?? 30;
                
                // Calcular proporção: se tirou 30 dias de férias, desconta 100% do adiantamento
                // Se tirou 15 dias, desconta 50% do adiantamento (proporcional aos dias de férias)
                var advanceProportionalFactor = vacationDays / 30.0;
                var discountAmount = (long)Math.Round(advanceAmount * advanceProportionalFactor);
                
                Console.WriteLine($"[PAYROLL DEBUG] Desconto de adiantamento de férias: Valor original: {advanceAmount}, Dias férias: {vacationDays}, Proporção: {advanceProportionalFactor:P0}, Desconto: {discountAmount}");
                
                items.Add(new PayrollItem
                {
                    PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                    Description = $"Desconto Adiantamento Férias ({vacationDays} dias)",
                    Type = "Desconto",
                    Category = "Desconto Adiantamento",
                    Amount = discountAmount,
                    SourceType = "vacation_advance_deduction",
                    IsManual = false,
                    IsActive = true,
                    CriadoPor = userId,
                    CriadoEm = now
                });
                
                // Descontar INSS adiantado nas férias (como PROVENTO para compensar)
                if (previousPayrollEmployee.VacationAdvanceInss.HasValue && previousPayrollEmployee.VacationAdvanceInss.Value > 0)
                {
                    var inssDiscount = previousPayrollEmployee.VacationAdvanceInss.Value;
                    Console.WriteLine($"[PAYROLL DEBUG] Desconto INSS adiantado férias: R$ {inssDiscount / 100.0m:N2}");
                    
                    items.Add(new PayrollItem
                    {
                        PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                        Description = $"Dedução INSS Adiantamento Férias",
                        Type = "Provento",
                        Category = "Dedução Imposto Adiantado",
                        Amount = inssDiscount,
                        SourceType = "vacation_advance_tax_deduction",
                        IsManual = false,
                        IsActive = true,
                        CriadoPor = userId,
                        CriadoEm = now
                    });
                }
                
                // Descontar IRRF adiantado nas férias (como PROVENTO para compensar)
                if (previousPayrollEmployee.VacationAdvanceIrrf.HasValue && previousPayrollEmployee.VacationAdvanceIrrf.Value > 0)
                {
                    var irrfDiscount = previousPayrollEmployee.VacationAdvanceIrrf.Value;
                    Console.WriteLine($"[PAYROLL DEBUG] Desconto IRRF adiantado férias: R$ {irrfDiscount / 100.0m:N2}");
                    
                    items.Add(new PayrollItem
                    {
                        PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                        Description = $"Dedução IRRF Adiantamento Férias",
                        Type = "Provento",
                        Category = "Dedução Imposto Adiantado",
                        Amount = irrfDiscount,
                        SourceType = "vacation_advance_tax_deduction",
                        IsManual = false,
                        IsActive = true,
                        CriadoPor = userId,
                        CriadoEm = now
                    });
                }
            }

            // 3. Adicionar benefícios do contrato (apenas os que se aplicam ao salário)
            // Função para verificar se a Application se aplica ao salário
            bool IsSalaryApplication(string application)
            {
                if (string.IsNullOrEmpty(application)) return true; // Se vazio, assume que se aplica
                var normalized = application.ToLower().Trim();
                return normalized == "salario" || normalized == "todos" || normalized == "mensal" || 
                       normalized == "salário" || normalized == "all";
            }
            
            // Função para verificar se é benefício (case-insensitive)
            bool IsBenefit(string type)
            {
                if (string.IsNullOrEmpty(type)) return false;
                var normalized = type.ToLower().Trim();
                return normalized == "benefício" || normalized == "beneficio" || normalized == "provento";
            }
            
            // Função para verificar se é desconto (case-insensitive)
            bool IsDiscount(string type)
            {
                if (string.IsNullOrEmpty(type)) return false;
                var normalized = type.ToLower().Trim();
                return normalized == "desconto";
            }

            // LOG TEMPORÁRIO - verificar dados
            Console.WriteLine($"[PAYROLL DEBUG] Contrato {contract.ContractId} - Employee {contract.EmployeeId}");
            Console.WriteLine($"[PAYROLL DEBUG] Total BenefitsDiscounts: {contract.ContractBenefitDiscountList?.Count ?? 0}");
            if (contract.ContractBenefitDiscountList != null)
            {
                foreach (var bd in contract.ContractBenefitDiscountList)
                {
                    Console.WriteLine($"[PAYROLL DEBUG] - Type='{bd.Type}' | Application='{bd.Application}' | Desc='{bd.Description}' | Amount={bd.Amount}");
                    Console.WriteLine($"[PAYROLL DEBUG]   IsBenefit={IsBenefit(bd.Type)} | IsSalaryApp={IsSalaryApplication(bd.Application)}");
                }
            }
            
            // Verificar se benefícios/descontos já foram adiantados nas férias do mês anterior
            var skipBenefitsDiscounts = previousPayrollEmployee != null && previousPayrollEmployee.VacationAdvanceBenefits;
            
            if (skipBenefitsDiscounts)
            {
                Console.WriteLine($"[PAYROLL DEBUG] Benefícios/descontos já foram adiantados nas férias do mês anterior - pulando lançamento");
            }
            
            var benefits = contract.ContractBenefitDiscountList?
                .Where(bd => IsBenefit(bd.Type) && IsSalaryApplication(bd.Application))
                .ToList() ?? new List<ContractBenefitDiscount>();
            
            Console.WriteLine($"[PAYROLL DEBUG] Benefits filtrados: {benefits.Count}");

            foreach (var benefit in benefits.Where(_ => !skipBenefitsDiscounts))
            {
                // Calcular valor: se IsProportional, aplicar fator apropriado
                var benefitAmount = benefit.Amount;
                var description = benefit.Description;
                
                if (benefit.IsProportional)
                {
                    // Verificar se é benefício ANUAL
                    var isAnnual = !string.IsNullOrEmpty(benefit.Application) && 
                                   benefit.Application.ToLower().Trim() == "anual";
                    
                    if (isAnnual)
                    {
                        // Benefício anual: proporção baseada em meses trabalhados (até 12 meses)
                        if (annualProportionalFactor < 1.0m)
                        {
                            benefitAmount = (long)Math.Round(benefit.Amount * annualProportionalFactor);
                            var monthsWorked = (int)Math.Round(annualProportionalFactor * 12);
                            description = $"{benefit.Description} (proporcional {monthsWorked}/12 meses)";
                            Console.WriteLine($"[PAYROLL DEBUG] Benefício ANUAL proporcional: {benefit.Description} - Original: {benefit.Amount} -> Proporcional: {benefitAmount} ({annualProportionalFactor:P0})");
                        }
                    }
                    else
                    {
                        // Benefício mensal: proporção baseada em dias no período
                        if (proportionalFactor < 1.0m)
                        {
                            benefitAmount = (long)Math.Round(benefit.Amount * proportionalFactor);
                            description = $"{benefit.Description} (proporcional {proportionalFactor:P0})";
                            Console.WriteLine($"[PAYROLL DEBUG] Benefício MENSAL proporcional: {benefit.Description} - Original: {benefit.Amount} -> Proporcional: {benefitAmount}");
                        }
                    }
                }
                
                items.Add(new PayrollItem
                {
                    PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                    Description = description,
                    Type = "Provento",
                    Category = "Benefício",
                    Amount = Convert.ToInt64(benefitAmount),
                    SourceType = "contract_benefit",
                    ReferenceId = benefit.ContractBenefitDiscountId,
                    IsManual = false,
                    IsActive = true,
                    CriadoPor = userId,
                    CriadoEm = now
                });
            }

            // 4. Adicionar descontos do contrato (apenas os que se aplicam ao salário)
            // Se benefícios/descontos já foram adiantados, pular
            var discounts = contract.ContractBenefitDiscountList?
                .Where(bd => IsDiscount(bd.Type) && IsSalaryApplication(bd.Application))
                .ToList() ?? new List<ContractBenefitDiscount>();

            foreach (var discount in discounts.Where(_ => !skipBenefitsDiscounts))
            {
                // Calcular valor: se IsProportional, aplicar fator apropriado
                var discountAmount = discount.Amount;
                var discountDescription = discount.Description;
                
                if (discount.IsProportional)
                {
                    // Verificar se é desconto ANUAL
                    var isAnnual = !string.IsNullOrEmpty(discount.Application) && 
                                   discount.Application.ToLower().Trim() == "anual";
                    
                    if (isAnnual)
                    {
                        // Desconto anual: proporção baseada em meses trabalhados (até 12 meses)
                        if (annualProportionalFactor < 1.0m)
                        {
                            discountAmount = (long)Math.Round(discount.Amount * annualProportionalFactor);
                            var monthsWorked = (int)Math.Round(annualProportionalFactor * 12);
                            discountDescription = $"{discount.Description} (proporcional {monthsWorked}/12 meses)";
                            Console.WriteLine($"[PAYROLL DEBUG] Desconto ANUAL proporcional: {discount.Description} - Original: {discount.Amount} -> Proporcional: {discountAmount} ({annualProportionalFactor:P0})");
                        }
                    }
                    else
                    {
                        // Desconto mensal: proporção baseada em dias no período
                        if (proportionalFactor < 1.0m)
                        {
                            discountAmount = (long)Math.Round(discount.Amount * proportionalFactor);
                            discountDescription = $"{discount.Description} (proporcional {proportionalFactor:P0})";
                            Console.WriteLine($"[PAYROLL DEBUG] Desconto MENSAL proporcional: {discount.Description} - Original: {discount.Amount} -> Proporcional: {discountAmount}");
                        }
                    }
                }
                
                items.Add(new PayrollItem
                {
                    PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                    Description = discountDescription,
                    Type = "Desconto",
                    Category = "Desconto",
                    Amount = Convert.ToInt64(discountAmount),
                    SourceType = "contract_discount",
                    ReferenceId = discount.ContractBenefitDiscountId,
                    IsManual = false,
                    IsActive = true,
                    CriadoPor = userId,
                    CriadoEm = now
                });
            }

            // 5. Buscar e adicionar empréstimos pendentes
            // Filtrar apenas empréstimos que devem ser descontados na folha normal
            // Excluir "13SAL" e "FERIAS" - esses só são descontados quando há 13º ou férias
            var pendingLoans = await _unitOfWork.LoanAdvanceRepository
                .GetPendingLoansByEmployeeAsync(contract.EmployeeId, payroll.PeriodEndDate);
            
            var salaryLoans = pendingLoans.Where(l => 
                l.DiscountSource != DiscountSourceType.ThirteenthSalary && 
                l.DiscountSource != DiscountSourceType.Vacation
            ).ToList();

            foreach (var loan in salaryLoans)
            {
                // Calcular próxima parcela a ser cobrada
                var nextInstallment = await _unitOfWork.PayrollItemRepository
                    .GetNextInstallmentNumberAsync(loan.LoanAdvanceId);

                // Verificar se ainda há parcelas a cobrar
                if (nextInstallment <= loan.Installments)
                {
                    var installmentAmount = loan.Amount / loan.Installments;

                    var loanDescription = !string.IsNullOrWhiteSpace(loan.Description) 
                        ? loan.Description 
                        : $"Empréstimo #{loan.LoanAdvanceId}";

                    // Se for parcela única, não mostrar "Parcela 1/1"
                    var itemDescription = loan.Installments == 1 
                        ? loanDescription 
                        : $"{loanDescription} - Parcela {nextInstallment}/{loan.Installments}";

                    // Categoria: Adiantamento (1 parcela) ou Emprestimo (múltiplas)
                    var itemCategory = loan.Installments == 1 ? "Adiantamento" : "Emprestimo";

                    items.Add(new PayrollItem
                    {
                        PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                        Description = itemDescription,
                        Type = "Desconto",
                        Category = itemCategory,
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

            // 6. Calcular e adicionar impostos (INSS, IRRF, FGTS)
            // Calcular base de cálculo (salário + benefícios que incidem impostos)
            var taxableIncome = salaryAmount; // Salário base (já proporcional se aplicável)
            
            // Adicionar benefícios que têm incidência de impostos
            foreach (var benefit in benefits.Where(b => b.HasTaxes))
            {
                // Recalcular o valor do benefício (considerando proporcionalidade)
                var benefitValue = benefit.Amount;
                if (benefit.IsProportional)
                {
                    var isAnnual = !string.IsNullOrEmpty(benefit.Application) && 
                                   benefit.Application.ToLower().Trim() == "anual";
                    if (isAnnual && annualProportionalFactor < 1.0m)
                    {
                        benefitValue = (long)Math.Round(benefit.Amount * annualProportionalFactor);
                    }
                    else if (!isAnnual && proportionalFactor < 1.0m)
                    {
                        benefitValue = (long)Math.Round(benefit.Amount * proportionalFactor);
                    }
                }
                taxableIncome += benefitValue;
            }
            
            Console.WriteLine($"[PAYROLL DEBUG] Base de cálculo para impostos: {taxableIncome} centavos (R$ {taxableIncome / 100.0m:N2})");

            // ===== INSS (Tabela 2024 - valores em centavos) =====
            // Faixas progressivas
            long CalculateINSS(long grossIncome)
            {
                // Converter para reais para cálculo
                decimal income = grossIncome / 100.0m;
                decimal inss = 0;
                
                // Faixa 1: Até R$ 1.412,00 → 7,5%
                if (income <= 1412.00m)
                {
                    inss = income * 0.075m;
                }
                // Faixa 2: De R$ 1.412,01 até R$ 2.666,68 → 9%
                else if (income <= 2666.68m)
                {
                    inss = (1412.00m * 0.075m) + ((income - 1412.00m) * 0.09m);
                }
                // Faixa 3: De R$ 2.666,69 até R$ 4.000,03 → 12%
                else if (income <= 4000.03m)
                {
                    inss = (1412.00m * 0.075m) + ((2666.68m - 1412.00m) * 0.09m) + ((income - 2666.68m) * 0.12m);
                }
                // Faixa 4: De R$ 4.000,04 até R$ 7.786,02 → 14%
                else if (income <= 7786.02m)
                {
                    inss = (1412.00m * 0.075m) + ((2666.68m - 1412.00m) * 0.09m) + ((4000.03m - 2666.68m) * 0.12m) + ((income - 4000.03m) * 0.14m);
                }
                // Acima do teto: contribuição máxima
                else
                {
                    inss = (1412.00m * 0.075m) + ((2666.68m - 1412.00m) * 0.09m) + ((4000.03m - 2666.68m) * 0.12m) + ((7786.02m - 4000.03m) * 0.14m);
                }
                
                // Converter de volta para centavos
                return (long)Math.Round(inss * 100);
            }

            // ===== IRRF (Tabela 2024 - valores em centavos) =====
            // Calculado sobre (Salário - INSS - Dependentes)
            long CalculateIRRF(long grossIncome, long inssValue)
            {
                // Converter para reais
                decimal income = grossIncome / 100.0m;
                decimal inss = inssValue / 100.0m;
                
                // Base de cálculo = Renda - INSS (sem considerar dependentes por enquanto)
                decimal baseCalculo = income - inss;
                
                decimal irrf = 0;
                decimal deduction = 0;
                
                // Faixa 1: Até R$ 2.259,20 → Isento
                if (baseCalculo <= 2259.20m)
                {
                    irrf = 0;
                }
                // Faixa 2: De R$ 2.259,21 até R$ 2.826,65 → 7,5%
                else if (baseCalculo <= 2826.65m)
                {
                    irrf = (baseCalculo * 0.075m) - 169.44m;
                }
                // Faixa 3: De R$ 2.826,66 até R$ 3.751,05 → 15%
                else if (baseCalculo <= 3751.05m)
                {
                    irrf = (baseCalculo * 0.15m) - 381.44m;
                }
                // Faixa 4: De R$ 3.751,06 até R$ 4.664,68 → 22,5%
                else if (baseCalculo <= 4664.68m)
                {
                    irrf = (baseCalculo * 0.225m) - 662.77m;
                }
                // Faixa 5: Acima de R$ 4.664,68 → 27,5%
                else
                {
                    irrf = (baseCalculo * 0.275m) - 896.00m;
                }
                
                // IRRF não pode ser negativo
                if (irrf < 0) irrf = 0;
                
                // Converter de volta para centavos
                return (long)Math.Round(irrf * 100);
            }

            // ===== FGTS (8% sobre salário bruto - pago pelo empregador) =====
            long CalculateFGTS(long grossIncome)
            {
                return (long)Math.Round(grossIncome * 0.08m);
            }

            // Calcular e adicionar INSS se contrato tiver
            if (contract.HasInss)
            {
                var inssValue = CalculateINSS(taxableIncome);
                if (inssValue > 0)
                {
                    items.Add(new PayrollItem
                    {
                        PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                        Description = "INSS",
                        Type = "Desconto",
                        Category = "Imposto",
                        Amount = inssValue,
                        SourceType = "tax",
                        CalculationBasis = taxableIncome,
                        IsManual = false,
                        IsActive = true,
                        CriadoPor = userId,
                        CriadoEm = now
                    });
                    Console.WriteLine($"[PAYROLL DEBUG] INSS calculado: R$ {inssValue / 100.0m:N2}");
                    
                    // Calcular IRRF se contrato tiver (usa INSS como dedução)
                    if (contract.HasIrrf)
                    {
                        var irrfValue = CalculateIRRF(taxableIncome, inssValue);
                        if (irrfValue > 0)
                        {
                            items.Add(new PayrollItem
                            {
                                PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                                Description = "IRRF",
                                Type = "Desconto",
                                Category = "Imposto",
                                Amount = irrfValue,
                                SourceType = "tax",
                                CalculationBasis = taxableIncome - inssValue, // Base de cálculo do IRRF
                                IsManual = false,
                                IsActive = true,
                                CriadoPor = userId,
                                CriadoEm = now
                            });
                            Console.WriteLine($"[PAYROLL DEBUG] IRRF calculado: R$ {irrfValue / 100.0m:N2}");
                        }
                        else
                        {
                            Console.WriteLine($"[PAYROLL DEBUG] IRRF: Isento (base R$ {(taxableIncome - inssValue) / 100.0m:N2})");
                        }
                    }
                }
            }
            else if (contract.HasIrrf)
            {
                // IRRF sem INSS (caso raro, mas possível)
                var irrfValue = CalculateIRRF(taxableIncome, 0);
                if (irrfValue > 0)
                {
                    items.Add(new PayrollItem
                    {
                        PayrollEmployeeId = createdPayrollEmployee.PayrollEmployeeId,
                        Description = "IRRF",
                        Type = "Desconto",
                        Category = "Imposto",
                        Amount = irrfValue,
                        SourceType = "tax",
                        CalculationBasis = taxableIncome,
                        IsManual = false,
                        IsActive = true,
                        CriadoPor = userId,
                        CriadoEm = now
                    });
                    Console.WriteLine($"[PAYROLL DEBUG] IRRF calculado (sem INSS): R$ {irrfValue / 100.0m:N2}");
                }
            }

            // FGTS: apenas log para referência (não cria item no banco - é encargo do empregador)
            if (contract.HasFgts)
            {
                var fgtsValue = CalculateFGTS(taxableIncome);
                Console.WriteLine($"[PAYROLL DEBUG] FGTS (encargo empregador, não salvo): R$ {fgtsValue / 100.0m:N2}");
            }

            // 7. Salvar todos os itens
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

            // Calcular INSS e FGTS
            long totalInss = 0;
            long totalFgts = 0;

            foreach (var emp in employees)
            {
                // INSS - buscar itens com descrição "INSS"
                var items = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(emp.PayrollEmployeeId);
                totalInss += items.Where(i => i.Type == "Desconto" && i.Description.ToUpper() == "INSS").Sum(i => i.Amount);

                // FGTS - calcular 8% do salário + benefícios com impostos (se o contrato tem FGTS)
                if (emp.ContractId.HasValue)
                {
                    var contract = await _unitOfWork.ContractRepository.GetOneByIdAsync(emp.ContractId.Value);
                    if (contract != null && contract.HasFgts)
                    {
                        // Base: salário (categoria Salario)
                        var salarioBase = items
                            .Where(i => i.Type == "Provento" && i.Category == "Salario")
                            .Sum(i => i.Amount);

                        // Adicionar benefícios que têm incidência de impostos
                        long beneficiosComImpostos = 0;
                        var benefitItems = items.Where(i => i.SourceType == "contract_benefit" && i.ReferenceId.HasValue);
                        foreach (var benefitItem in benefitItems)
                        {
                            var benefit = await _unitOfWork.ContractBenefitDiscountRepository.GetOneByIdAsync(benefitItem.ReferenceId.Value);
                            if (benefit != null && benefit.HasTaxes)
                            {
                                beneficiosComImpostos += benefitItem.Amount;
                            }
                        }

                        // FGTS = 8% do (salário + benefícios com impostos)
                        totalFgts += (long)Math.Round((salarioBase + beneficiosComImpostos) * 0.08m);
                    }
                }
            }

            // Atualizar Payroll
            payroll.TotalGrossPay = totalGrossPay;
            payroll.TotalDeductions = totalDeductions;
            payroll.TotalNetPay = totalNetPay;
            payroll.TotalInss = totalInss;
            payroll.TotalFgts = totalFgts;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PayrollItemDetailedDTO> UpdatePayrollItemAsync(long payrollItemId, UpdatePayrollItemDTO dto, long currentUserId)
        {
            var item = await _unitOfWork.PayrollItemRepository.GetOneByIdAsync(payrollItemId);
            if (item == null)
            {
                throw new EntityNotFoundException("PayrollItem", payrollItemId);
            }

            // Buscar o PayrollEmployee para verificar se a folha está fechada
            var payrollEmployee = await _unitOfWork.PayrollEmployeeRepository.GetOneByIdAsync(item.PayrollEmployeeId);
            if (payrollEmployee == null)
            {
                throw new EntityNotFoundException("PayrollEmployee", item.PayrollEmployeeId);
            }

            var payroll = await _unitOfWork.PayrollRepository.GetOneByIdAsync(payrollEmployee.PayrollId);
            if (payroll == null)
            {
                throw new EntityNotFoundException("Payroll", payrollEmployee.PayrollId);
            }

            if (payroll.IsClosed)
            {
                throw new ValidationException("Payroll", "Não é possível editar itens de uma folha de pagamento fechada.");
            }

            // Atualizar item
            item.Description = dto.Description;
            item.Amount = dto.Amount;
            item.IsManual = true; // Marca como editado manualmente
            item.AtualizadoPor = currentUserId;
            item.AtualizadoEm = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            // Recalcular totais
            await RecalculatePayrollEmployeeTotalsAsync(payrollEmployee.PayrollEmployeeId);
            await RecalculatePayrollTotalsAsync(payroll.PayrollId);

            return new PayrollItemDetailedDTO
            {
                PayrollItemId = item.PayrollItemId,
                PayrollEmployeeId = item.PayrollEmployeeId,
                Description = item.Description,
                Type = item.Type,
                Category = item.Category,
                Amount = item.Amount,
                ReferenceId = item.ReferenceId,
                CalculationBasis = item.CalculationBasis,
                CalculationDetails = item.CalculationDetails
            };
        }

        public async Task<PayrollEmployeeDetailedDTO> UpdateWorkedUnitsAsync(long payrollEmployeeId, UpdateWorkedUnitsDTO dto, long currentUserId)
        {
            var payrollEmployee = await _unitOfWork.PayrollEmployeeRepository.GetOneByIdAsync(payrollEmployeeId);
            if (payrollEmployee == null)
            {
                throw new EntityNotFoundException("PayrollEmployee", payrollEmployeeId);
            }

            var payroll = await _unitOfWork.PayrollRepository.GetOneByIdAsync(payrollEmployee.PayrollId);
            if (payroll == null)
            {
                throw new EntityNotFoundException("Payroll", payrollEmployee.PayrollId);
            }

            if (payroll.IsClosed)
            {
                throw new ValidationException("Payroll", "Não é possível editar uma folha de pagamento fechada.");
            }

            // Buscar contrato para obter o valor base
            Contract? contract = null;
            if (payrollEmployee.ContractId.HasValue)
            {
                contract = await _unitOfWork.ContractRepository.GetOneByIdAsync(payrollEmployee.ContractId.Value);
            }

            if (contract == null)
            {
                throw new ValidationException("Contract", "Contrato não encontrado para este empregado.");
            }

            // Verificar se é horista ou diarista
            if (contract.Type != "Horista" && contract.Type != "Diarista")
            {
                throw new ValidationException("Contract", "Esta função é apenas para contratos de horistas ou diaristas.");
            }

            // Atualizar unidades trabalhadas
            payrollEmployee.WorkedUnits = dto.WorkedUnits;
            payrollEmployee.AtualizadoPor = currentUserId;
            payrollEmployee.AtualizadoEm = DateTime.UtcNow;

            // Calcular novo valor do salário base
            var newBaseSalary = (long)(contract.Value * dto.WorkedUnits);

            // Buscar e atualizar o item de Salário Base
            var salaryItems = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(payrollEmployeeId);
            var salaryItem = salaryItems.FirstOrDefault(i => i.Category == "Salario" && i.Type == "Provento");
            
            if (salaryItem != null)
            {
                salaryItem.Amount = newBaseSalary;
                salaryItem.Description = contract.Type == "Horista" 
                    ? $"Salário Base ({dto.WorkedUnits:0.##} horas)" 
                    : $"Salário Base ({dto.WorkedUnits:0.##} dias)";
                salaryItem.IsManual = true;
                salaryItem.AtualizadoPor = currentUserId;
                salaryItem.AtualizadoEm = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync();

            // Recalcular totais
            await RecalculatePayrollEmployeeTotalsAsync(payrollEmployeeId);
            await RecalculatePayrollTotalsAsync(payroll.PayrollId);

            // Buscar dados do empregado para retorno
            var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(payrollEmployee.EmployeeId);
            var payrollItems = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(payrollEmployeeId);
            
            // Recarregar payrollEmployee após recálculo
            payrollEmployee = await _unitOfWork.PayrollEmployeeRepository.GetOneByIdAsync(payrollEmployeeId);

            var itemsList = payrollItems.Select(item => new PayrollItemDetailedDTO
            {
                PayrollItemId = item.PayrollItemId,
                PayrollEmployeeId = item.PayrollEmployeeId,
                Description = item.Description,
                Type = item.Type,
                Category = item.Category,
                Amount = item.Amount,
                ReferenceId = item.ReferenceId,
                CalculationBasis = item.CalculationBasis,
                CalculationDetails = item.CalculationDetails
            }).ToList();

            return new PayrollEmployeeDetailedDTO
            {
                PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                PayrollId = payrollEmployee.PayrollId,
                EmployeeId = payrollEmployee.EmployeeId,
                EmployeeName = employee?.Nickname ?? employee?.FullName ?? "Desconhecido",
                EmployeeDocument = employee?.Cpf,
                IsOnVacation = payrollEmployee.IsOnVacation,
                VacationDays = payrollEmployee.VacationDays,
                VacationAdvanceAmount = payrollEmployee.VacationAdvanceAmount,
                VacationAdvancePaid = payrollEmployee.VacationAdvancePaid,
                VacationStartDate = payrollEmployee.VacationStartDate,
                VacationEndDate = payrollEmployee.VacationEndDate,
                VacationNotes = payrollEmployee.VacationNotes,
                TotalGrossPay = payrollEmployee.TotalGrossPay,
                TotalDeductions = payrollEmployee.TotalDeductions,
                TotalNetPay = payrollEmployee.TotalNetPay,
                ContractId = payrollEmployee.ContractId,
                ContractType = contract?.Type,
                ContractValue = contract?.Value,
                WorkedUnits = payrollEmployee.WorkedUnits,
                HasFgts = contract?.HasFgts ?? false,
                Items = itemsList
            };
        }

        public async Task<PayrollEmployeeDetailedDTO> AddPayrollItemAsync(PayrollItemInputDTO dto, long currentUserId)
        {
            // Buscar PayrollEmployee
            var payrollEmployee = await _unitOfWork.PayrollEmployeeRepository.GetOneByIdAsync(dto.PayrollEmployeeId);
            if (payrollEmployee == null)
                throw new EntityNotFoundException("PayrollEmployee", dto.PayrollEmployeeId);

            // Buscar Payroll e verificar se está aberta
            var payroll = await _unitOfWork.PayrollRepository.GetOneByIdAsync(payrollEmployee.PayrollId);
            if (payroll == null)
                throw new EntityNotFoundException("Payroll", payrollEmployee.PayrollId);

            if (payroll.IsClosed)
                throw new ValidationException("Payroll", "Não é possível adicionar itens em uma folha fechada.");

            // Validar dados do item
            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new ValidationException("Description", "A descrição do item é obrigatória.");

            if (dto.Amount <= 0)
                throw new ValidationException("Amount", "O valor do item deve ser maior que zero.");

            if (dto.Type != "Provento" && dto.Type != "Desconto")
                throw new ValidationException("Type", "O tipo do item deve ser 'Provento' ou 'Desconto'.");

            // Criar novo item
            var newItem = new PayrollItem
            {
                PayrollEmployeeId = dto.PayrollEmployeeId,
                Description = dto.Description,
                Type = dto.Type,
                Category = dto.Category ?? "Manual",
                Amount = dto.Amount,
                IsManual = true,
                IsActive = true,
                SourceType = "manual",
                CriadoPor = currentUserId,
                CriadoEm = DateTime.UtcNow
            };

            await _unitOfWork.PayrollItemRepository.CreateAsync(newItem);
            await _unitOfWork.SaveChangesAsync();

            // Recalcular totais
            await RecalculatePayrollEmployeeTotalsAsync(dto.PayrollEmployeeId);
            await RecalculatePayrollTotalsAsync(payroll.PayrollId);

            // Buscar dados do empregado para retorno
            var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(payrollEmployee.EmployeeId);
            var contract = payrollEmployee.ContractId.HasValue 
                ? await _unitOfWork.ContractRepository.GetOneByIdAsync(payrollEmployee.ContractId.Value) 
                : null;
            var payrollItems = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(dto.PayrollEmployeeId);
            
            // Recarregar payrollEmployee após recálculo
            payrollEmployee = await _unitOfWork.PayrollEmployeeRepository.GetOneByIdAsync(dto.PayrollEmployeeId);

            var itemsList = payrollItems.Select(item => new PayrollItemDetailedDTO
            {
                PayrollItemId = item.PayrollItemId,
                PayrollEmployeeId = item.PayrollEmployeeId,
                Description = item.Description,
                Type = item.Type,
                Category = item.Category,
                Amount = item.Amount,
                ReferenceId = item.ReferenceId,
                CalculationBasis = item.CalculationBasis,
                CalculationDetails = item.CalculationDetails
            }).ToList();

            return new PayrollEmployeeDetailedDTO
            {
                PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                PayrollId = payrollEmployee.PayrollId,
                EmployeeId = payrollEmployee.EmployeeId,
                EmployeeName = employee?.Nickname ?? employee?.FullName ?? "Desconhecido",
                EmployeeDocument = employee?.Cpf,
                IsOnVacation = payrollEmployee.IsOnVacation,
                VacationDays = payrollEmployee.VacationDays,
                VacationAdvanceAmount = payrollEmployee.VacationAdvanceAmount,
                VacationAdvancePaid = payrollEmployee.VacationAdvancePaid,
                VacationStartDate = payrollEmployee.VacationStartDate,
                VacationEndDate = payrollEmployee.VacationEndDate,
                VacationNotes = payrollEmployee.VacationNotes,
                TotalGrossPay = payrollEmployee.TotalGrossPay,
                TotalDeductions = payrollEmployee.TotalDeductions,
                TotalNetPay = payrollEmployee.TotalNetPay,
                ContractId = payrollEmployee.ContractId,
                ContractType = contract?.Type,
                ContractValue = contract?.Value,
                WorkedUnits = payrollEmployee.WorkedUnits,
                HasFgts = contract?.HasFgts ?? false,
                Items = itemsList
            };
        }

        public async Task<PayrollDetailedOutputDTO> ApplyThirteenthSalaryAsync(long payrollId, ThirteenthSalaryInputDTO dto, long currentUserId)
        {
            var payroll = await _unitOfWork.PayrollRepository.GetOneByIdAsync(payrollId);
            if (payroll == null)
            {
                throw new EntityNotFoundException("Payroll", payrollId);
            }

            if (payroll.IsClosed)
            {
                throw new ValidationException("Payroll", "Não é possível aplicar 13º em uma folha de pagamento fechada.");
            }

            if (dto.Percentage < 0 || dto.Percentage > 100)
            {
                throw new ValidationException("Payroll", "A porcentagem do 13º deve estar entre 0 e 100%.");
            }

            // Se já tem 13º aplicado, remover primeiro
            if (payroll.ThirteenthPercentage.HasValue)
            {
                await RemoveThirteenthSalaryItemsAsync(payrollId);
            }

            // Buscar todos os empregados da folha
            var payrollEmployees = await _unitOfWork.PayrollEmployeeRepository.GetAllByPayrollIdAsync(payrollId);

            foreach (var payrollEmployee in payrollEmployees)
            {
                // Buscar contrato do empregado
                if (!payrollEmployee.ContractId.HasValue) continue;
                
                var contract = await _unitOfWork.ContractRepository.GetOneByIdAsync(payrollEmployee.ContractId.Value);
                if (contract == null || !contract.HasThirteenthSalary) continue;

                // Calcular valor do 13º baseado no salário bruto (proporcionalmente à %)
                var salaryItems = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(payrollEmployee.PayrollEmployeeId);
                var baseSalary = salaryItems
                    .Where(i => i.Type == "Provento" && i.Category == "Salario")
                    .Sum(i => i.Amount);

                var thirteenthValue = (long)Math.Round(baseSalary * (dto.Percentage / 100.0));

                // Criar item de 13º Salário
                var thirteenthItem = new PayrollItem
                {
                    PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                    Description = $"13º Salário ({dto.Percentage}%)",
                    Type = "Provento",
                    Category = "13º Salário",
                    Amount = thirteenthValue,
                    SourceType = "thirteenth_salary",
                    IsManual = false,
                    IsActive = true,
                    CriadoPor = currentUserId,
                    CriadoEm = DateTime.UtcNow
                };
                await _unitOfWork.PayrollItemRepository.CreateAsync(thirteenthItem);

                // Buscar benefícios/descontos do contrato que se aplicam ao 13º
                var contractBenefitsDiscounts = contract.ContractBenefitDiscountList?
                    .Where(bd => bd.Application == "13º Salário" || bd.Application == "Todos")
                    .ToList() ?? new List<ContractBenefitDiscount>();

                foreach (var cbd in contractBenefitsDiscounts)
                {
                    // Calcular valor proporcional à % do 13º
                    var itemValue = (long)Math.Round(cbd.Amount * (dto.Percentage / 100.0));
                    
                    var itemType = cbd.Type.ToLower().Contains("desconto") ? "Desconto" : "Provento";
                    var itemCategory = itemType == "Provento" ? "Benefício 13º" : "Desconto 13º";

                    var benefitItem = new PayrollItem
                    {
                        PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                        Description = $"{cbd.Description} (13º)",
                        Type = itemType,
                        Category = itemCategory,
                        Amount = itemValue,
                        SourceType = "thirteenth_benefit",
                        ReferenceId = cbd.ContractBenefitDiscountId,
                        IsManual = false,
                        IsActive = true,
                        CriadoPor = currentUserId,
                        CriadoEm = DateTime.UtcNow
                    };
                    await _unitOfWork.PayrollItemRepository.CreateAsync(benefitItem);
                }

                // Buscar empréstimos que descontam no 13º ou em Todos
                var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(payrollEmployee.EmployeeId);
                if (employee != null)
                {
                    var loans = await _unitOfWork.LoanAdvanceRepository.GetPendingLoansByEmployeeAsync(employee.EmployeeId);
                    var thirteenthLoans = loans.Where(l => 
                        l.IsApproved && 
                        !l.IsFullyPaid && 
                        (l.DiscountSource == DiscountSourceType.ThirteenthSalary || l.DiscountSource == DiscountSourceType.All)
                    ).ToList();

                    foreach (var loan in thirteenthLoans)
                    {
                        var installmentValue = (long)Math.Round((decimal)loan.Amount / loan.Installments);
                        // Proporcional à % do 13º
                        var proportionalValue = (long)Math.Round(installmentValue * (dto.Percentage / 100.0));

                        var loanItem = new PayrollItem
                        {
                            PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                            Description = $"{loan.Description ?? "Empréstimo"} (13º)",
                            Type = "Desconto",
                            Category = "Empréstimo 13º",
                            Amount = proportionalValue,
                            SourceType = "thirteenth_loan",
                            ReferenceId = loan.LoanAdvanceId,
                            IsManual = false,
                            IsActive = true,
                            CriadoPor = currentUserId,
                            CriadoEm = DateTime.UtcNow
                        };
                        await _unitOfWork.PayrollItemRepository.CreateAsync(loanItem);
                    }
                }

                // Aplicar impostos se necessário
                if (dto.TaxOption != "none")
                {
                    var taxBase = thirteenthValue;
                    
                    // Se for proporcional, usa só o valor do 13º; se for full, usa valor cheio
                    if (dto.TaxOption == "full")
                    {
                        taxBase = baseSalary; // Valor cheio do salário
                    }

                    // INSS sobre 13º (se contrato tem INSS)
                    if (contract.HasInss)
                    {
                        var inssValue = CalculateINSS(taxBase);
                        if (inssValue > 0)
                        {
                            var inssItem = new PayrollItem
                            {
                                PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                                Description = "INSS 13º",
                                Type = "Desconto",
                                Category = "Imposto 13º",
                                Amount = inssValue,
                                SourceType = "thirteenth_tax",
                                CalculationBasis = taxBase,
                                IsManual = false,
                                IsActive = true,
                                CriadoPor = currentUserId,
                                CriadoEm = DateTime.UtcNow
                            };
                            await _unitOfWork.PayrollItemRepository.CreateAsync(inssItem);
                        }
                    }
                }
            }

            // Salvar configuração do 13º na folha
            payroll.ThirteenthPercentage = dto.Percentage;
            payroll.ThirteenthTaxOption = dto.TaxOption;
            payroll.AtualizadoPor = currentUserId;
            payroll.AtualizadoEm = DateTime.UtcNow;

            // Salvar todas as criações primeiro
            await _unitOfWork.SaveChangesAsync();

            // Depois recalcular totais de cada empregado
            foreach (var payrollEmployee in payrollEmployees)
            {
                if (!payrollEmployee.ContractId.HasValue) continue;
                var contract = await _unitOfWork.ContractRepository.GetOneByIdAsync(payrollEmployee.ContractId.Value);
                if (contract == null || !contract.HasThirteenthSalary) continue;
                
                await RecalculatePayrollEmployeeTotalsAsync(payrollEmployee.PayrollEmployeeId);
            }

            // Recalcular totais da folha
            await RecalculatePayrollTotalsAsync(payrollId);

            return await GetDetailedByIdAsync(payrollId);
        }

        public async Task<PayrollDetailedOutputDTO> RemoveThirteenthSalaryAsync(long payrollId, long currentUserId)
        {
            var payroll = await _unitOfWork.PayrollRepository.GetOneByIdAsync(payrollId);
            if (payroll == null)
            {
                throw new EntityNotFoundException("Payroll", payrollId);
            }

            if (payroll.IsClosed)
            {
                throw new ValidationException("Payroll", "Não é possível remover 13º de uma folha de pagamento fechada.");
            }

            if (!payroll.ThirteenthPercentage.HasValue)
            {
                throw new ValidationException("Payroll", "Esta folha não possui 13º salário aplicado.");
            }

            await RemoveThirteenthSalaryItemsAsync(payrollId);

            // Limpar configuração do 13º na folha
            payroll.ThirteenthPercentage = null;
            payroll.ThirteenthTaxOption = null;
            payroll.AtualizadoPor = currentUserId;
            payroll.AtualizadoEm = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            // Recalcular totais da folha
            await RecalculatePayrollTotalsAsync(payrollId);

            return await GetDetailedByIdAsync(payrollId);
        }

        private async Task RemoveThirteenthSalaryItemsAsync(long payrollId)
        {
            var payrollEmployees = await _unitOfWork.PayrollEmployeeRepository.GetAllByPayrollIdAsync(payrollId);

            // Primeiro deletar todos os itens de 13º
            foreach (var payrollEmployee in payrollEmployees)
            {
                var items = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(payrollEmployee.PayrollEmployeeId);
                
                // Remover itens relacionados ao 13º
                var thirteenthItems = items.Where(i => 
                    i.SourceType == "thirteenth_salary" ||
                    i.SourceType == "thirteenth_benefit" ||
                    i.SourceType == "thirteenth_loan" ||
                    i.SourceType == "thirteenth_tax" ||
                    i.Category == "13º Salário" ||
                    i.Category == "Benefício 13º" ||
                    i.Category == "Desconto 13º" ||
                    i.Category == "Empréstimo 13º" ||
                    i.Category == "Imposto 13º"
                ).ToList();

                foreach (var item in thirteenthItems)
                {
                    await _unitOfWork.PayrollItemRepository.DeleteByIdAsync(item.PayrollItemId);
                }
            }

            // Salvar deleções primeiro
            await _unitOfWork.SaveChangesAsync();

            // Depois recalcular totais de cada empregado
            foreach (var payrollEmployee in payrollEmployees)
            {
                await RecalculatePayrollEmployeeTotalsAsync(payrollEmployee.PayrollEmployeeId);
            }
        }

        private long CalculateINSS(long baseSalary)
        {
            // Tabela INSS 2024 (valores em centavos)
            decimal salarioDecimal = baseSalary / 100m;
            decimal inss = 0;

            if (salarioDecimal <= 1412.00m)
            {
                inss = salarioDecimal * 0.075m;
            }
            else if (salarioDecimal <= 2666.68m)
            {
                inss = 105.90m + (salarioDecimal - 1412.00m) * 0.09m;
            }
            else if (salarioDecimal <= 4000.03m)
            {
                inss = 218.82m + (salarioDecimal - 2666.68m) * 0.12m;
            }
            else if (salarioDecimal <= 7786.02m)
            {
                inss = 378.82m + (salarioDecimal - 4000.03m) * 0.14m;
            }
            else
            {
                inss = 908.86m; // Teto
            }

            return (long)Math.Round(inss * 100); // Converter para centavos
        }

        private long CalculateIRRF(long grossIncome, long inssValue)
        {
            // Converter para reais
            decimal income = grossIncome / 100.0m;
            decimal inss = inssValue / 100.0m;
            
            // Base de cálculo = Renda - INSS (sem considerar dependentes por enquanto)
            decimal baseCalculo = income - inss;
            
            decimal irrf = 0;
            
            // Faixa 1: Até R$ 2.259,20 → Isento
            if (baseCalculo <= 2259.20m)
            {
                irrf = 0;
            }
            // Faixa 2: De R$ 2.259,21 até R$ 2.826,65 → 7,5%
            else if (baseCalculo <= 2826.65m)
            {
                irrf = (baseCalculo * 0.075m) - 169.44m;
            }
            // Faixa 3: De R$ 2.826,66 até R$ 3.751,05 → 15%
            else if (baseCalculo <= 3751.05m)
            {
                irrf = (baseCalculo * 0.15m) - 381.44m;
            }
            // Faixa 4: De R$ 3.751,06 até R$ 4.664,68 → 22,5%
            else if (baseCalculo <= 4664.68m)
            {
                irrf = (baseCalculo * 0.225m) - 662.77m;
            }
            // Faixa 5: Acima de R$ 4.664,68 → 27,5%
            else
            {
                irrf = (baseCalculo * 0.275m) - 896.00m;
            }
            
            // IRRF não pode ser negativo
            if (irrf < 0) irrf = 0;
            
            // Converter de volta para centavos
            return (long)Math.Round(irrf * 100);
        }

        public async Task<PayrollDetailedOutputDTO> ApplyVacationAsync(long payrollId, VacationInputDTO dto, long currentUserId)
        {
            var payroll = await _unitOfWork.PayrollRepository.GetOneByIdAsync(payrollId);
            if (payroll == null)
            {
                throw new EntityNotFoundException("Payroll", payrollId);
            }

            if (payroll.IsClosed)
            {
                throw new ValidationException("Payroll", "Não é possível aplicar férias em uma folha de pagamento fechada.");
            }

            if (dto.VacationDays < 1 || dto.VacationDays > 30)
            {
                throw new ValidationException("Payroll", "Os dias de férias devem estar entre 1 e 30.");
            }

            var payrollEmployee = await _unitOfWork.PayrollEmployeeRepository.GetOneByIdAsync(dto.PayrollEmployeeId);
            if (payrollEmployee == null)
            {
                throw new EntityNotFoundException("PayrollEmployee", dto.PayrollEmployeeId);
            }

            // Se já tem férias aplicadas, remover primeiro
            if (payrollEmployee.IsOnVacation)
            {
                await RemoveVacationItemsAsync(payrollEmployee.PayrollEmployeeId);
            }

            // Buscar contrato do empregado
            if (!payrollEmployee.ContractId.HasValue)
            {
                throw new ValidationException("Payroll", "Funcionário sem contrato ativo.");
            }

            var contract = await _unitOfWork.ContractRepository.GetOneByIdAsync(payrollEmployee.ContractId.Value);
            if (contract == null)
            {
                throw new ValidationException("Payroll", "Contrato não encontrado.");
            }

            // Buscar salário base do empregado
            var items = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(payrollEmployee.PayrollEmployeeId);
            var baseSalary = items
                .Where(i => i.Type == "Provento" && i.Category == "Salario" && i.IsActive)
                .Sum(i => i.Amount);

            // Calcular valor proporcional aos dias de férias
            var dailyRate = baseSalary / 30.0;
            var proportionalValue = (long)Math.Round(dailyRate * dto.VacationDays);

            // Calcular 1/3 constitucional proporcional aos dias de férias
            // 1/3 = (Salário / 3) * (dias / 30)
            var oneThirdBonus = (long)Math.Round(proportionalValue / 3.0);

            // Criar item de 1/3 de Férias (ÚNICO item de férias - o salário base já está sendo pago normalmente)
            var oneThirdItem = new PayrollItem
            {
                PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                Description = $"1/3 Constitucional de Férias ({dto.VacationDays} dias)",
                Type = "Provento",
                Category = "Férias",
                Amount = oneThirdBonus,
                SourceType = "vacation_bonus",
                IsManual = false,
                IsActive = true,
                CriadoPor = currentUserId,
                CriadoEm = DateTime.UtcNow
            };
            await _unitOfWork.PayrollItemRepository.CreateAsync(oneThirdItem);

            // Total para base de cálculo de INSS férias = apenas o 1/3
            var totalVacationValue = oneThirdBonus;

            // Buscar benefícios/descontos do contrato que se aplicam a Férias
            var contractBenefitsDiscounts = contract.ContractBenefitDiscountList?
                .Where(bd => bd.Application == "Férias" || bd.Application == "Todos")
                .ToList() ?? new List<ContractBenefitDiscount>();

            foreach (var cbd in contractBenefitsDiscounts)
            {
                var benefitItem = new PayrollItem
                {
                    PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                    Description = $"{cbd.Description} (Férias)",
                    Type = cbd.Type,
                    Category = cbd.Type == "Provento" ? "Benefício Férias" : "Desconto Férias",
                    Amount = cbd.Amount,
                    SourceType = "vacation_benefit",
                    IsManual = false,
                    IsActive = true,
                    CriadoPor = currentUserId,
                    CriadoEm = DateTime.UtcNow
                };
                await _unitOfWork.PayrollItemRepository.CreateAsync(benefitItem);
            }

            // Buscar empréstimos que descontam APENAS em Férias (não incluir "Todos" - esses são apenas para salário e 13º)
            var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(payrollEmployee.EmployeeId);
            if (employee != null)
            {
                var loans = await _unitOfWork.LoanAdvanceRepository.GetPendingLoansByEmployeeAsync(employee.EmployeeId);
                var vacationLoans = loans.Where(l => 
                    l.IsApproved && 
                    !l.IsFullyPaid && 
                    l.DiscountSource == DiscountSourceType.Vacation
                ).ToList();

                foreach (var loan in vacationLoans)
                {
                    var remainingInstallments = loan.Installments - loan.InstallmentsPaid;
                    if (remainingInstallments > 0)
                    {
                        var installmentValue = (long)Math.Round((decimal)loan.Amount / loan.Installments);
                        
                        var loanItem = new PayrollItem
                        {
                            PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                            Description = $"{loan.Description ?? "Empréstimo"} - Parcela Férias",
                            Type = "Desconto",
                            Category = "Empréstimo Férias",
                            Amount = installmentValue,
                            SourceType = "vacation_loan",
                            ReferenceId = loan.LoanAdvanceId,
                            IsManual = false,
                            IsActive = true,
                            CriadoPor = currentUserId,
                            CriadoEm = DateTime.UtcNow
                        };
                        await _unitOfWork.PayrollItemRepository.CreateAsync(loanItem);
                    }
                }
            }

            // Aplicar INSS se configurado para incluir impostos
            if (dto.IncludeTaxes && contract.HasInss)
            {
                var inssValue = CalculateINSS(totalVacationValue);
                if (inssValue > 0)
                {
                    var inssItem = new PayrollItem
                    {
                        PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                        Description = "INSS Férias",
                        Type = "Desconto",
                        Category = "Imposto Férias",
                        Amount = inssValue,
                        SourceType = "vacation_tax",
                        CalculationBasis = totalVacationValue,
                        IsManual = false,
                        IsActive = true,
                        CriadoPor = currentUserId,
                        CriadoEm = DateTime.UtcNow
                    };
                    await _unitOfWork.PayrollItemRepository.CreateAsync(inssItem);
                }
            }

            // Adiantar salário do próximo mês se configurado (proporcional aos dias de férias)
            if (dto.AdvanceNextMonth)
            {
                // Adiantamento proporcional aos dias de férias
                var advanceAmount = proportionalValue;
                
                var advanceSalaryItem = new PayrollItem
                {
                    PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                    Description = $"Adiantamento Salário ({dto.VacationDays} dias)",
                    Type = "Provento",
                    Category = "Adiantamento Férias",
                    Amount = advanceAmount,
                    SourceType = "vacation_advance_salary",
                    IsManual = false,
                    IsActive = true,
                    CriadoPor = currentUserId,
                    CriadoEm = DateTime.UtcNow
                };
                await _unitOfWork.PayrollItemRepository.CreateAsync(advanceSalaryItem);

                // INSS e IRRF do adiantamento (proporcional)
                long advanceInssValue = 0;
                long advanceIrrfValue = 0;
                
                if (dto.IncludeTaxes && contract.HasInss)
                {
                    advanceInssValue = CalculateINSS(advanceAmount);
                    if (advanceInssValue > 0)
                    {
                        var advanceInssItem = new PayrollItem
                        {
                            PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                            Description = $"INSS Adiantamento ({dto.VacationDays} dias)",
                            Type = "Desconto",
                            Category = "Imposto Férias",
                            Amount = advanceInssValue,
                            SourceType = "vacation_advance_tax",
                            CalculationBasis = advanceAmount,
                            IsManual = false,
                            IsActive = true,
                            CriadoPor = currentUserId,
                            CriadoEm = DateTime.UtcNow
                        };
                        await _unitOfWork.PayrollItemRepository.CreateAsync(advanceInssItem);
                    }
                    
                    // IRRF do adiantamento (calculado sobre base - INSS)
                    if (contract.HasIrrf)
                    {
                        advanceIrrfValue = CalculateIRRF(advanceAmount, advanceInssValue);
                        if (advanceIrrfValue > 0)
                        {
                            var advanceIrrfItem = new PayrollItem
                            {
                                PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                                Description = $"IRRF Adiantamento ({dto.VacationDays} dias)",
                                Type = "Desconto",
                                Category = "Imposto Férias",
                                Amount = advanceIrrfValue,
                                SourceType = "vacation_advance_tax",
                                CalculationBasis = advanceAmount - advanceInssValue,
                                IsManual = false,
                                IsActive = true,
                                CriadoPor = currentUserId,
                                CriadoEm = DateTime.UtcNow
                            };
                            await _unitOfWork.PayrollItemRepository.CreateAsync(advanceIrrfItem);
                        }
                    }
                }
                
                // Armazenar valores de impostos adiantados para descontar na folha seguinte
                payrollEmployee.VacationAdvanceInss = advanceInssValue;
                payrollEmployee.VacationAdvanceIrrf = advanceIrrfValue;
                payrollEmployee.VacationAdvanceBenefits = true; // Marcar que benefícios/descontos foram adiantados

                // Benefícios e descontos mensais adiantados (integralmente)
                // Usar mesma lógica do processamento normal - case-insensitive e múltiplos formatos
                bool IsSalaryApplicationForVacation(string application)
                {
                    if (string.IsNullOrEmpty(application)) return true;
                    var normalized = application.ToLower().Trim();
                    return normalized == "salario" || normalized == "todos" || normalized == "mensal" || 
                           normalized == "salário" || normalized == "all";
                }
                
                var monthlyBenefitsDiscounts = contract.ContractBenefitDiscountList?
                    .Where(bd => IsSalaryApplicationForVacation(bd.Application))
                    .ToList() ?? new List<ContractBenefitDiscount>();
                
                Console.WriteLine($"[VACATION DEBUG] Benefícios/Descontos para adiantar: {monthlyBenefitsDiscounts.Count}");
                foreach (var bd in monthlyBenefitsDiscounts)
                {
                    Console.WriteLine($"[VACATION DEBUG] - {bd.Description} | Type={bd.Type} | App={bd.Application} | Amount={bd.Amount}");
                }

                foreach (var cbd in monthlyBenefitsDiscounts)
                {
                    // Verificar tipo de forma case-insensitive
                    var typeNormalized = (cbd.Type ?? "").ToLower().Trim();
                    var isBenefit = typeNormalized == "benefício" || typeNormalized == "beneficio" || typeNormalized == "provento";
                    
                    var advanceBenefitItem = new PayrollItem
                    {
                        PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                        Description = $"{cbd.Description} (Adiantamento)",
                        Type = isBenefit ? "Provento" : "Desconto",
                        Category = isBenefit ? "Adiantamento Férias" : "Desconto Adiantamento",
                        Amount = cbd.Amount,
                        SourceType = "vacation_advance_benefit",
                        IsManual = false,
                        IsActive = true,
                        CriadoPor = currentUserId,
                        CriadoEm = DateTime.UtcNow
                    };
                    await _unitOfWork.PayrollItemRepository.CreateAsync(advanceBenefitItem);
                }

                // Empréstimos mensais adiantados (verificar se ainda tem parcelas)
                // Inclui "Mensal" e "Todos" pois estamos adiantando o salário do próximo mês
                if (employee != null)
                {
                    var monthlyLoans = await _unitOfWork.LoanAdvanceRepository.GetPendingLoansByEmployeeAsync(employee.EmployeeId);
                    var activeMonthlyLoans = monthlyLoans.Where(l => 
                        l.IsApproved && 
                        !l.IsFullyPaid && 
                        (l.DiscountSource == DiscountSourceType.Salary || l.DiscountSource == DiscountSourceType.All)
                    ).ToList();

                    foreach (var loan in activeMonthlyLoans)
                    {
                        // Verificar quantas parcelas já foram pagas e quantas ainda restam
                        var remainingInstallments = loan.Installments - loan.InstallmentsPaid;
                        
                        // Se ainda tem mais de 1 parcela restante, pode adiantar
                        // (1 parcela já está sendo descontada no mês atual)
                        if (remainingInstallments > 1)
                        {
                            var installmentValue = (long)Math.Round((decimal)loan.Amount / loan.Installments);
                            
                            var advanceLoanItem = new PayrollItem
                            {
                                PayrollEmployeeId = payrollEmployee.PayrollEmployeeId,
                                Description = $"{loan.Description ?? "Empréstimo"} - Parcela Adiantada (Férias)",
                                Type = "Desconto",
                                Category = "Empréstimo Adiantamento",
                                Amount = installmentValue,
                                SourceType = "vacation_advance_loan",
                                ReferenceId = loan.LoanAdvanceId,
                                IsManual = false,
                                IsActive = true,
                                CriadoPor = currentUserId,
                                CriadoEm = DateTime.UtcNow
                            };
                            await _unitOfWork.PayrollItemRepository.CreateAsync(advanceLoanItem);
                        }
                    }
                }
            }

            // Atualizar PayrollEmployee com informações de férias
            // Calcular data final automaticamente: data início + dias - 1
            var vacationEndDate = dto.VacationStartDate.AddDays(dto.VacationDays - 1);
            
            payrollEmployee.IsOnVacation = true;
            payrollEmployee.VacationDays = dto.VacationDays;
            payrollEmployee.VacationStartDate = dto.VacationStartDate;
            payrollEmployee.VacationEndDate = vacationEndDate;
            payrollEmployee.VacationAdvanceAmount = dto.AdvanceNextMonth ? baseSalary : 0;
            payrollEmployee.VacationAdvancePaid = dto.AdvanceNextMonth;
            payrollEmployee.VacationNotes = dto.Notes;
            payrollEmployee.AtualizadoPor = currentUserId;
            payrollEmployee.AtualizadoEm = DateTime.UtcNow;

            // Salvar todas as criações primeiro
            await _unitOfWork.SaveChangesAsync();

            // Depois recalcular totais
            await RecalculatePayrollEmployeeTotalsAsync(payrollEmployee.PayrollEmployeeId);
            await RecalculatePayrollTotalsAsync(payrollId);

            return await GetDetailedByIdAsync(payrollId);
        }

        public async Task<PayrollDetailedOutputDTO> RemoveVacationAsync(long payrollId, long payrollEmployeeId, long currentUserId)
        {
            var payroll = await _unitOfWork.PayrollRepository.GetOneByIdAsync(payrollId);
            if (payroll == null)
            {
                throw new EntityNotFoundException("Payroll", payrollId);
            }

            if (payroll.IsClosed)
            {
                throw new ValidationException("Payroll", "Não é possível remover férias de uma folha de pagamento fechada.");
            }

            var payrollEmployee = await _unitOfWork.PayrollEmployeeRepository.GetOneByIdAsync(payrollEmployeeId);
            if (payrollEmployee == null)
            {
                throw new EntityNotFoundException("PayrollEmployee", payrollEmployeeId);
            }

            await RemoveVacationItemsAsync(payrollEmployeeId);

            // Limpar informações de férias do PayrollEmployee
            payrollEmployee.IsOnVacation = false;
            payrollEmployee.VacationDays = null;
            payrollEmployee.VacationStartDate = null;
            payrollEmployee.VacationEndDate = null;
            payrollEmployee.VacationAdvanceAmount = null;
            payrollEmployee.VacationAdvancePaid = false;
            payrollEmployee.VacationNotes = null;
            payrollEmployee.AtualizadoPor = currentUserId;
            payrollEmployee.AtualizadoEm = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            // Recalcular totais
            await RecalculatePayrollTotalsAsync(payrollId);

            return await GetDetailedByIdAsync(payrollId);
        }

        private async Task RemoveVacationItemsAsync(long payrollEmployeeId)
        {
            var items = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(payrollEmployeeId);
            
            // Remover itens relacionados a férias
            var vacationItems = items.Where(i => 
                i.SourceType == "vacation" ||
                i.SourceType == "vacation_bonus" ||
                i.SourceType == "vacation_benefit" ||
                i.SourceType == "vacation_loan" ||
                i.SourceType == "vacation_tax" ||
                i.SourceType == "vacation_advance_salary" ||
                i.SourceType == "vacation_advance_tax" ||
                i.SourceType == "vacation_advance_benefit" ||
                i.SourceType == "vacation_advance_loan" ||
                i.Category == "Férias" ||
                i.Category == "Benefício Férias" ||
                i.Category == "Desconto Férias" ||
                i.Category == "Empréstimo Férias" ||
                i.Category == "Imposto Férias" ||
                i.Category == "Adiantamento Férias" ||
                i.Category == "Desconto Adiantamento" ||
                i.Category == "Empréstimo Adiantamento"
            ).ToList();

            foreach (var item in vacationItems)
            {
                await _unitOfWork.PayrollItemRepository.DeleteByIdAsync(item.PayrollItemId);
            }

            // Salvar deleções primeiro
            await _unitOfWork.SaveChangesAsync();

            // Depois recalcular totais
            await RecalculatePayrollEmployeeTotalsAsync(payrollEmployeeId);
        }

        public async Task<PayrollSuggestionDTO> GetPayrollSuggestionAsync(long companyId)
        {
            var now = DateTime.UtcNow;
            
            // Buscar a última folha fechada da empresa (ordenada por data de término do período)
            var lastClosedPayroll = await _unitOfWork.PayrollRepository
                .GetLastClosedPayrollByCompanyAsync(companyId);
            
            // Verificar se existe folha em aberto
            var openPayroll = await _unitOfWork.PayrollRepository
                .GetOpenPayrollByCompanyAsync(companyId);
            
            int suggestedMonth;
            int suggestedYear;
            
            if (lastClosedPayroll != null)
            {
                // Sugerir o próximo mês após a última folha fechada
                var lastPeriodEnd = lastClosedPayroll.PeriodEndDate;
                var nextMonth = lastPeriodEnd.AddMonths(1);
                suggestedMonth = nextMonth.Month;
                suggestedYear = nextMonth.Year;
            }
            else
            {
                // Se não tiver nenhuma folha, sugerir o mês atual
                suggestedMonth = now.Month;
                suggestedYear = now.Year;
            }
            
            return new PayrollSuggestionDTO
            {
                SuggestedMonth = suggestedMonth,
                SuggestedYear = suggestedYear,
                HasOpenPayroll = openPayroll != null,
                OpenPayrollId = openPayroll?.PayrollId,
                OpenPayrollPeriod = openPayroll != null 
                    ? $"{openPayroll.PeriodStartDate:dd/MM/yyyy} - {openPayroll.PeriodEndDate:dd/MM/yyyy}"
                    : null
            };
        }

        public async Task<PayrollDetailedOutputDTO> ClosePayrollAsync(long payrollId, ClosePayrollInputDTO dto, long currentUserId)
        {
            var payroll = await _unitOfWork.PayrollRepository.GetOneByIdWithIncludesAsync(payrollId);
            if (payroll == null)
                throw new BusinessRuleException("ClosePayroll", "Folha de pagamento não encontrada");

            if (payroll.IsClosed)
                throw new BusinessRuleException("ClosePayroll", "Folha de pagamento já está fechada");

            var account = await _unitOfWork.AccountRepository.GetOneByIdAsync(dto.AccountId);
            if (account == null)
                throw new BusinessRuleException("ClosePayroll", "Conta corrente não encontrada");

            var now = DateTime.UtcNow;
            var generatedTransactionIds = new List<long>();
            var generatedLoanAdvanceIds = new List<long>();
            var paidLoanInstallments = new List<(long LoanAdvanceId, decimal InstallmentsPaidBefore)>();

            // Calcular rateio médio para INSS/FGTS baseado nos centros de custo de todos os funcionários
            var costCenterTotals = new Dictionary<long, decimal>();
            decimal totalGrossPayAll = 0;

            foreach (var pe in payroll.PayrollEmployeeList)
            {
                if (pe.Contract?.ContractCostCenterList != null)
                {
                    foreach (var ccc in pe.Contract.ContractCostCenterList)
                    {
                        var amount = pe.TotalGrossPay * (ccc.Percentage / 100m);
                        if (!costCenterTotals.ContainsKey(ccc.CostCenterId))
                            costCenterTotals[ccc.CostCenterId] = 0;
                        costCenterTotals[ccc.CostCenterId] += amount;
                        totalGrossPayAll += amount;
                    }
                }
                else
                {
                    totalGrossPayAll += pe.TotalGrossPay;
                }
            }

            // Calcular percentuais médios para rateio de INSS/FGTS
            var averageCostCenterPercentages = new Dictionary<long, decimal>();
            if (totalGrossPayAll > 0)
            {
                foreach (var kvp in costCenterTotals)
                {
                    averageCostCenterPercentages[kvp.Key] = (kvp.Value / totalGrossPayAll) * 100;
                }
            }

            // 1. Criar lançamentos financeiros por funcionário
            foreach (var pe in payroll.PayrollEmployeeList)
            {
                var employee = pe.Employee;
                var netPay = pe.TotalNetPay;

                // Se líquido negativo, criar empréstimo para próximo mês
                if (netPay < 0)
                {
                    var loanAdvance = new LoanAdvance
                    {
                        EmployeeId = pe.EmployeeId,
                        Amount = Math.Abs(netPay),
                        Installments = 1,
                        DiscountSource = "Folha",
                        StartDate = payroll.PeriodEndDate.AddMonths(1),
                        LoanDate = dto.PaymentDate, // Data do empréstimo = data de pagamento da folha
                        Description = $"Adiantamento gerado automaticamente - Folha {payroll.PeriodStartDate:MM/yyyy}",
                        IsApproved = true,
                        InstallmentsPaid = 0,
                        RemainingAmount = Math.Abs(netPay),
                        IsFullyPaid = false,
                        CriadoPor = currentUserId,
                        CriadoEm = now
                    };
                    await _unitOfWork.LoanAdvanceRepository.CreateAsync(loanAdvance);
                    await _unitOfWork.SaveChangesAsync();
                    generatedLoanAdvanceIds.Add(loanAdvance.LoanAdvanceId);
                    netPay = 0; // Não gerar transação de saída se negativo
                }

                if (netPay > 0)
                {
                    // Criar transação financeira para o funcionário
                    var transaction = new FinancialTransaction
                    {
                        CompanyId = payroll.CompanyId,
                        AccountId = dto.AccountId,
                        Description = $"Pagamento Folha {payroll.PeriodStartDate:MM/yyyy} - {employee.Nickname}",
                        Type = "Saída",
                        Amount = netPay,
                        TransactionDate = dto.PaymentDate,
                        CriadoPor = currentUserId,
                        CriadoEm = now
                    };
                    await _unitOfWork.FinancialTransactionRepository.CreateAsync(transaction);
                    await _unitOfWork.SaveChangesAsync();
                    generatedTransactionIds.Add(transaction.FinancialTransactionId);

                    // Adicionar rateio por centro de custo do contrato
                    if (pe.Contract?.ContractCostCenterList != null && pe.Contract.ContractCostCenterList.Any())
                    {
                        foreach (var ccc in pe.Contract.ContractCostCenterList)
                        {
                            var tcc = new TransactionCostCenter
                            {
                                FinancialTransactionId = transaction.FinancialTransactionId,
                                CostCenterId = ccc.CostCenterId,
                                Percentage = ccc.Percentage,
                                Amount = (long)(netPay * (ccc.Percentage / 100m)),
                                CriadoPor = currentUserId,
                                CriadoEm = now
                            };
                            await _unitOfWork.TransactionCostCenterRepository.CreateAsync(tcc);
                        }
                    }
                }

                // 2. Abater parcelas de empréstimos do funcionário
                // SourceTypes de empréstimo: "loan" (normal), "thirteenth_loan" (13º), "vacation_loan" (férias)
                var loanItems = pe.PayrollItemList
                    .Where(pi => pi.Type == "Desconto" && 
                           (pi.SourceType == "loan" || pi.SourceType == "thirteenth_loan" || pi.SourceType == "vacation_loan") && 
                           pi.ReferenceId.HasValue)
                    .ToList();

                foreach (var loanItem in loanItems)
                {
                    var loan = await _unitOfWork.LoanAdvanceRepository.GetOneByIdAsync(loanItem.ReferenceId!.Value);
                    if (loan != null && !loan.IsFullyPaid)
                    {
                        paidLoanInstallments.Add((loan.LoanAdvanceId, loan.InstallmentsPaid));
                        
                        // Calcular valor original da parcela
                        var originalInstallmentValue = loan.Amount / loan.Installments;
                        
                        // Calcular fração da parcela paga (suporta 13º parcial, ex: 50% = 0.5 parcela)
                        decimal installmentFraction = originalInstallmentValue > 0 
                            ? (decimal)loanItem.Amount / originalInstallmentValue 
                            : 1m;
                        
                        loan.InstallmentsPaid += installmentFraction;
                        loan.RemainingAmount -= loanItem.Amount;
                        if (loan.RemainingAmount < 0) loan.RemainingAmount = 0;
                        
                        // Marcar como quitado APENAS se todas as parcelas foram pagas
                        loan.IsFullyPaid = loan.InstallmentsPaid >= loan.Installments;
                        loan.AtualizadoPor = currentUserId;
                        loan.AtualizadoEm = now;
                        await _unitOfWork.LoanAdvanceRepository.UpdateByIdAsync(loan.LoanAdvanceId, loan);
                    }
                }
            }

            // 3. Criar transação para INSS
            if (dto.InssAmount > 0)
            {
                var inssTransaction = new FinancialTransaction
                {
                    CompanyId = payroll.CompanyId,
                    AccountId = dto.AccountId,
                    Description = $"INSS Folha {payroll.PeriodStartDate:MM/yyyy}",
                    Type = "Saída",
                    Amount = dto.InssAmount,
                    TransactionDate = dto.PaymentDate,
                    CriadoPor = currentUserId,
                    CriadoEm = now
                };
                await _unitOfWork.FinancialTransactionRepository.CreateAsync(inssTransaction);
                await _unitOfWork.SaveChangesAsync();
                generatedTransactionIds.Add(inssTransaction.FinancialTransactionId);

                // Rateio médio para INSS
                foreach (var kvp in averageCostCenterPercentages)
                {
                    var tcc = new TransactionCostCenter
                    {
                        FinancialTransactionId = inssTransaction.FinancialTransactionId,
                        CostCenterId = kvp.Key,
                        Percentage = kvp.Value,
                        Amount = (long)(dto.InssAmount * (kvp.Value / 100m)),
                        CriadoPor = currentUserId,
                        CriadoEm = now
                    };
                    await _unitOfWork.TransactionCostCenterRepository.CreateAsync(tcc);
                }
            }

            // 4. Criar transação para FGTS
            if (dto.FgtsAmount > 0)
            {
                var fgtsTransaction = new FinancialTransaction
                {
                    CompanyId = payroll.CompanyId,
                    AccountId = dto.AccountId,
                    Description = $"FGTS Folha {payroll.PeriodStartDate:MM/yyyy}",
                    Type = "Saída",
                    Amount = dto.FgtsAmount,
                    TransactionDate = dto.PaymentDate,
                    CriadoPor = currentUserId,
                    CriadoEm = now
                };
                await _unitOfWork.FinancialTransactionRepository.CreateAsync(fgtsTransaction);
                await _unitOfWork.SaveChangesAsync();
                generatedTransactionIds.Add(fgtsTransaction.FinancialTransactionId);

                // Rateio médio para FGTS
                foreach (var kvp in averageCostCenterPercentages)
                {
                    var tcc = new TransactionCostCenter
                    {
                        FinancialTransactionId = fgtsTransaction.FinancialTransactionId,
                        CostCenterId = kvp.Key,
                        Percentage = kvp.Value,
                        Amount = (long)(dto.FgtsAmount * (kvp.Value / 100m)),
                        CriadoPor = currentUserId,
                        CriadoEm = now
                    };
                    await _unitOfWork.TransactionCostCenterRepository.CreateAsync(tcc);
                }
            }

            // 5. Atualizar folha como fechada
            payroll.IsClosed = true;
            payroll.ClosedAt = now;
            payroll.ClosedBy = currentUserId;
            payroll.PaymentDate = dto.PaymentDate;
            payroll.AccountId = dto.AccountId;
            payroll.InssAmount = dto.InssAmount;
            payroll.FgtsAmount = dto.FgtsAmount;
            payroll.GeneratedTransactionIds = string.Join(",", generatedTransactionIds);
            payroll.GeneratedLoanAdvanceIds = string.Join(",", generatedLoanAdvanceIds);
            payroll.AtualizadoPor = currentUserId;
            payroll.AtualizadoEm = now;

            await _unitOfWork.PayrollRepository.UpdateByIdAsync(payroll.PayrollId, payroll);
            await _unitOfWork.SaveChangesAsync();

            return await GetDetailedByIdAsync(payrollId);
        }

        public async Task<PayrollDetailedOutputDTO> ReopenPayrollAsync(long payrollId, long currentUserId)
        {
            var payroll = await _unitOfWork.PayrollRepository.GetOneByIdWithIncludesAsync(payrollId);
            if (payroll == null)
                throw new BusinessRuleException("ClosePayroll", "Folha de pagamento não encontrada");

            if (!payroll.IsClosed)
                throw new BusinessRuleException("ReopenPayroll", "Folha de pagamento já está aberta");

            var now = DateTime.UtcNow;

            // 1. Reverter transações financeiras geradas
            if (!string.IsNullOrEmpty(payroll.GeneratedTransactionIds))
            {
                var transactionIds = payroll.GeneratedTransactionIds.Split(',')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(long.Parse)
                    .ToList();

                foreach (var transactionId in transactionIds)
                {
                    // Remover rateios primeiro
                    var costCenters = await _unitOfWork.TransactionCostCenterRepository.GetByTransactionIdAsync(transactionId);
                    foreach (var cc in costCenters)
                    {
                        await _unitOfWork.TransactionCostCenterRepository.DeleteAsync(cc.TransactionCostCenterId);
                    }
                    // Remover transação
                    await _unitOfWork.FinancialTransactionRepository.DeleteByIdAsync(transactionId);
                }
            }

            // 2. Reverter empréstimos gerados automaticamente
            if (!string.IsNullOrEmpty(payroll.GeneratedLoanAdvanceIds))
            {
                var loanIds = payroll.GeneratedLoanAdvanceIds.Split(',')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(long.Parse)
                    .ToList();

                foreach (var loanId in loanIds)
                {
                    await _unitOfWork.LoanAdvanceRepository.DeleteByIdAsync(loanId);
                }
            }

            // 3. Reverter parcelas de empréstimos abatidas (decrementar InstallmentsPaid)
            foreach (var pe in payroll.PayrollEmployeeList)
            {
                // SourceTypes de empréstimo: "loan" (normal), "thirteenth_loan" (13º), "vacation_loan" (férias)
                var loanItems = pe.PayrollItemList
                    .Where(pi => pi.Type == "Desconto" && 
                           (pi.SourceType == "loan" || pi.SourceType == "thirteenth_loan" || pi.SourceType == "vacation_loan") && 
                           pi.ReferenceId.HasValue)
                    .ToList();

                foreach (var loanItem in loanItems)
                {
                    var loan = await _unitOfWork.LoanAdvanceRepository.GetOneByIdAsync(loanItem.ReferenceId!.Value);
                    if (loan != null)
                    {
                        // Calcular fração da parcela a reverter (mesma lógica do fechamento)
                        var originalInstallmentValue = loan.Amount / loan.Installments;
                        decimal installmentFraction = originalInstallmentValue > 0 
                            ? (decimal)loanItem.Amount / originalInstallmentValue 
                            : 1m;
                        
                        loan.InstallmentsPaid = Math.Max(0, loan.InstallmentsPaid - installmentFraction);
                        loan.RemainingAmount += loanItem.Amount;
                        loan.IsFullyPaid = false;
                        loan.AtualizadoPor = currentUserId;
                        loan.AtualizadoEm = now;
                        await _unitOfWork.LoanAdvanceRepository.UpdateByIdAsync(loan.LoanAdvanceId, loan);
                    }
                }
            }

            // 4. Reabrir a folha
            payroll.IsClosed = false;
            payroll.ClosedAt = null;
            payroll.ClosedBy = null;
            payroll.PaymentDate = null;
            payroll.AccountId = null;
            payroll.InssAmount = null;
            payroll.FgtsAmount = null;
            payroll.GeneratedTransactionIds = null;
            payroll.GeneratedLoanAdvanceIds = null;
            payroll.AtualizadoPor = currentUserId;
            payroll.AtualizadoEm = now;

            await _unitOfWork.PayrollRepository.UpdateByIdAsync(payroll.PayrollId, payroll);
            await _unitOfWork.SaveChangesAsync();

            return await GetDetailedByIdAsync(payrollId);
        }
    }
}
