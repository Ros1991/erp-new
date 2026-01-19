using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERP.Application.DTOs;
using ERP.Application.Interfaces;

namespace ERP.Application.Services
{
    public interface IReportService
    {
        Task<FinancialSummaryDTO> GetFinancialSummaryAsync(long companyId, ReportFilterDTO filters);
        Task<List<CostCenterReportDTO>> GetCostCenterReportAsync(long companyId, ReportFilterDTO filters);
        Task<List<AccountReportDTO>> GetAccountReportAsync(long companyId, ReportFilterDTO filters);
        Task<List<SupplierCustomerReportDTO>> GetSupplierCustomerReportAsync(long companyId, SupplierCustomerReportFilterDTO filters);
        Task<List<CashFlowItemDTO>> GetCashFlowAsync(long companyId, ReportFilterDTO filters);
        Task<AccountPayableReceivableReportDTO> GetAccountPayableReceivableReportAsync(long companyId, AccountPayableReceivableReportFilterDTO filters);
        Task<FinancialForecastDTO> GetFinancialForecastAsync(long companyId, int months);
        Task<EmployeeAccountReportDTO> GetEmployeeAccountReportAsync(long companyId, EmployeeAccountReportFilterDTO filters);
    }

    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FinancialSummaryDTO> GetFinancialSummaryAsync(long companyId, ReportFilterDTO filters)
        {
            var startDate = filters.StartDate ?? DateTime.UtcNow.AddMonths(-12);
            var endDate = filters.EndDate ?? DateTime.UtcNow;

            // Query otimizada - filtra direto no banco
            var transactions = await _unitOfWork.FinancialTransactionRepository.GetByDateRangeAsync(companyId, startDate, endDate);
            
            var receitas = transactions.Where(t => t.Type == "Entrada").ToList();
            var despesas = transactions.Where(t => t.Type == "Saída").ToList();

            var totalReceitas = receitas.Sum(t => t.Amount);
            var totalDespesas = despesas.Sum(t => t.Amount);

            // Query otimizada - conta direto no banco
            var quantidadeContas = await _unitOfWork.AccountRepository.CountAsync(companyId);

            // Agrupar por mês (já filtrado do banco)
            var receitasPorMes = receitas
                .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthValueDTO
                {
                    Month = $"{g.Key.Month:D2}/{g.Key.Year}",
                    Value = g.Sum(t => t.Amount)
                })
                .ToList();

            var despesasPorMes = despesas
                .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthValueDTO
                {
                    Month = $"{g.Key.Month:D2}/{g.Key.Year}",
                    Value = g.Sum(t => t.Amount)
                })
                .ToList();

            return new FinancialSummaryDTO
            {
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas,
                Saldo = totalReceitas - totalDespesas,
                QuantidadeContas = quantidadeContas,
                ReceitasPorMes = receitasPorMes,
                DespesasPorMes = despesasPorMes
            };
        }

        public async Task<List<CostCenterReportDTO>> GetCostCenterReportAsync(long companyId, ReportFilterDTO filters)
        {
            var startDate = filters.StartDate ?? DateTime.UtcNow.AddMonths(-12);
            var endDate = filters.EndDate ?? DateTime.UtcNow;

            var costCenters = await _unitOfWork.CostCenterRepository.GetAllAsync(companyId);
            
            // Query otimizada - filtra por data e inclui centros de custo direto no banco
            var filteredTransactions = await _unitOfWork.FinancialTransactionRepository.GetByDateRangeWithCostCentersAsync(companyId, startDate, endDate);

            var result = new List<CostCenterReportDTO>();
            long totalGeral = 0;

            foreach (var cc in costCenters)
            {
                // Buscar transações que têm este centro de custo na lista
                var ccTransactions = filteredTransactions
                    .Where(t => t.TransactionCostCenterList != null && 
                                t.TransactionCostCenterList.Any(tcc => tcc.CostCenterId == cc.CostCenterId))
                    .ToList();

                // Calcular valores proporcionais ao percentual de cada centro de custo
                long receitas = 0;
                long despesas = 0;

                foreach (var t in ccTransactions)
                {
                    var tcc = t.TransactionCostCenterList?.FirstOrDefault(x => x.CostCenterId == cc.CostCenterId);
                    if (tcc != null)
                    {
                        var valorProporcional = (long)(t.Amount * tcc.Percentage / 100);
                        if (t.Type == "Entrada")
                            receitas += valorProporcional;
                        else
                            despesas += valorProporcional;
                    }
                }

                totalGeral += despesas;

                result.Add(new CostCenterReportDTO
                {
                    CostCenterId = cc.CostCenterId,
                    CostCenterName = cc.Name,
                    TotalReceitas = receitas,
                    TotalDespesas = despesas,
                    Saldo = receitas - despesas
                });
            }

            // Calcular percentual
            foreach (var item in result)
            {
                item.Percentual = totalGeral > 0 
                    ? Math.Round((decimal)item.TotalDespesas / totalGeral * 100, 2) 
                    : 0;
            }

            return result.OrderByDescending(r => r.TotalDespesas).ToList();
        }

        public async Task<List<AccountReportDTO>> GetAccountReportAsync(long companyId, ReportFilterDTO filters)
        {
            var startDate = filters.StartDate ?? DateTime.UtcNow.AddMonths(-12);
            var endDate = filters.EndDate ?? DateTime.UtcNow;

            var accounts = await _unitOfWork.AccountRepository.GetAllAsync(companyId);
            
            // Query otimizada - filtra por data direto no banco
            var transactions = await _unitOfWork.FinancialTransactionRepository.GetByDateRangeAsync(companyId, startDate, endDate);

            var result = new List<AccountReportDTO>();

            foreach (var account in accounts)
            {
                var accountTransactions = transactions
                    .Where(t => t.AccountId == account.AccountId)
                    .ToList();

                var entradas = accountTransactions.Where(t => t.Type == "Entrada").Sum(t => t.Amount);
                var saidas = accountTransactions.Where(t => t.Type == "Saída").Sum(t => t.Amount);

                result.Add(new AccountReportDTO
                {
                    AccountId = account.AccountId,
                    AccountName = account.Name,
                    AccountType = account.Type,
                    SaldoInicial = account.InitialBalance,
                    TotalEntradas = entradas,
                    TotalSaidas = saidas,
                    SaldoFinal = account.InitialBalance + entradas - saidas
                });
            }

            return result.OrderByDescending(r => r.SaldoFinal).ToList();
        }

        public async Task<List<SupplierCustomerReportDTO>> GetSupplierCustomerReportAsync(long companyId, SupplierCustomerReportFilterDTO filters)
        {
            var startDate = filters.StartDate ?? DateTime.UtcNow.AddMonths(-12);
            var endDate = filters.EndDate ?? DateTime.UtcNow;

            var supplierCustomers = await _unitOfWork.SupplierCustomerRepository.GetAllAsync(companyId);
            
            // Query otimizada - filtra por data, isPaid e tipo direto no banco
            var filtered = await _unitOfWork.AccountPayableReceivableRepository.GetPaidByDateRangeAsync(companyId, startDate, endDate, filters.Type);

            var result = new List<SupplierCustomerReportDTO>();

            foreach (var sc in supplierCustomers)
            {
                var scItems = filtered.Where(apr => apr.SupplierCustomerId == sc.SupplierCustomerId).ToList();
                
                if (scItems.Count == 0) continue;

                var pago = scItems.Where(apr => apr.Type == "Pagar").Sum(apr => apr.Amount);
                var recebido = scItems.Where(apr => apr.Type == "Receber").Sum(apr => apr.Amount);

                result.Add(new SupplierCustomerReportDTO
                {
                    SupplierCustomerId = sc.SupplierCustomerId,
                    SupplierCustomerName = sc.Name,
                    TotalPago = pago,
                    TotalRecebido = recebido,
                    QuantidadeTransacoes = scItems.Count
                });
            }

            return result.OrderByDescending(r => r.TotalPago + r.TotalRecebido).ToList();
        }

        public async Task<List<CashFlowItemDTO>> GetCashFlowAsync(long companyId, ReportFilterDTO filters)
        {
            var startDate = filters.StartDate ?? DateTime.UtcNow.AddDays(-30);
            var endDate = filters.EndDate ?? DateTime.UtcNow;

            // Query otimizada - soma saldo inicial direto no banco
            var saldoInicial = await _unitOfWork.AccountRepository.GetTotalInitialBalanceAsync(companyId);

            // Query otimizada - soma transações anteriores direto no banco
            var (entradasAnteriores, saidasAnteriores) = await _unitOfWork.FinancialTransactionRepository.GetSumBeforeDateAsync(companyId, startDate);
            saldoInicial += entradasAnteriores - saidasAnteriores;

            // Query otimizada - filtra por data direto no banco
            var filteredTransactions = await _unitOfWork.FinancialTransactionRepository.GetByDateRangeAsync(companyId, startDate, endDate);

            var result = new List<CashFlowItemDTO>();
            var saldoAcumulado = saldoInicial;

            // Agrupar por dia (já filtrado do banco)
            var dias = filteredTransactions
                .GroupBy(t => t.TransactionDate.Date)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var dia in dias)
            {
                var entradas = dia.Where(t => t.Type == "Entrada").Sum(t => t.Amount);
                var saidas = dia.Where(t => t.Type == "Saída").Sum(t => t.Amount);
                var saldo = entradas - saidas;
                saldoAcumulado += saldo;

                result.Add(new CashFlowItemDTO
                {
                    Date = dia.Key,
                    Entradas = entradas,
                    Saidas = saidas,
                    Saldo = saldo,
                    SaldoAcumulado = saldoAcumulado
                });
            }

            return result;
        }

        public async Task<AccountPayableReceivableReportDTO> GetAccountPayableReceivableReportAsync(long companyId, AccountPayableReceivableReportFilterDTO filters)
        {
            var today = DateTime.UtcNow.Date;

            // Query otimizada - filtra pendentes e tipo direto no banco
            var pendentes = await _unitOfWork.AccountPayableReceivableRepository.GetPendingAsync(companyId, filters.Type);

            var vencidas = pendentes.Where(apr => apr.DueDate.Date < today).ToList();
            var vencendoHoje = pendentes.Where(apr => apr.DueDate.Date == today).ToList();
            var vencendo7Dias = pendentes.Where(apr => apr.DueDate.Date > today && apr.DueDate.Date <= today.AddDays(7)).ToList();
            var vencendo30Dias = pendentes.Where(apr => apr.DueDate.Date > today.AddDays(7) && apr.DueDate.Date <= today.AddDays(30)).ToList();
            var aVencer = pendentes.Where(apr => apr.DueDate.Date > today.AddDays(30)).ToList();

            var items = pendentes
                .Take(50)
                .Select(apr => new AccountPayableReceivableItemDTO
                {
                    Id = apr.AccountPayableReceivableId,
                    Description = apr.Description,
                    SupplierCustomerName = apr.SupplierCustomer?.Name ?? "N/A",
                    Amount = apr.Amount,
                    DueDate = apr.DueDate,
                    Type = apr.Type,
                    IsPaid = apr.IsPaid,
                    DaysOverdue = (today - apr.DueDate.Date).Days
                })
                .ToList();

            return new AccountPayableReceivableReportDTO
            {
                Vencidas = new StatusSummaryDTO { Quantidade = vencidas.Count, Valor = vencidas.Sum(v => v.Amount) },
                VencendoHoje = new StatusSummaryDTO { Quantidade = vencendoHoje.Count, Valor = vencendoHoje.Sum(v => v.Amount) },
                Vencendo7Dias = new StatusSummaryDTO { Quantidade = vencendo7Dias.Count, Valor = vencendo7Dias.Sum(v => v.Amount) },
                Vencendo30Dias = new StatusSummaryDTO { Quantidade = vencendo30Dias.Count, Valor = vencendo30Dias.Sum(v => v.Amount) },
                AVencer = new StatusSummaryDTO { Quantidade = aVencer.Count, Valor = aVencer.Sum(v => v.Amount) },
                Items = items
            };
        }

        public async Task<FinancialForecastDTO> GetFinancialForecastAsync(long companyId, int months)
        {
            var today = DateTime.UtcNow.Date;
            var endDate = today.AddMonths(months);

            // Query otimizada - soma saldo inicial direto no banco
            var saldoInicial = await _unitOfWork.AccountRepository.GetTotalInitialBalanceAsync(companyId);

            // Query otimizada - soma transações até hoje direto no banco
            var (entradasPassadas, saidasPassadas) = await _unitOfWork.FinancialTransactionRepository.GetSumUpToDateAsync(companyId, today);
            var saldoAtual = saldoInicial + entradasPassadas - saidasPassadas;

            // Query otimizada - busca pendentes direto no banco (já incluindo SupplierCustomer)
            var pendentes = await _unitOfWork.AccountPayableReceivableRepository.GetPendingAsync(companyId);
            
            // Filtrar por data de vencimento até o fim do período
            var pendentesNoPeriodo = pendentes.Where(apr => apr.DueDate.Date <= endDate).ToList();
            
            var contasAPagar = pendentesNoPeriodo
                .Where(apr => apr.Type == "Pagar")
                .Select(apr => new ForecastItemDTO
                {
                    Id = apr.AccountPayableReceivableId,
                    Description = apr.Description,
                    SupplierCustomerName = apr.SupplierCustomer?.Name ?? "N/A",
                    Amount = apr.Amount,
                    DueDate = apr.DueDate,
                    Type = apr.Type
                })
                .ToList();

            var contasAReceber = pendentesNoPeriodo
                .Where(apr => apr.Type == "Receber")
                .Select(apr => new ForecastItemDTO
                {
                    Id = apr.AccountPayableReceivableId,
                    Description = apr.Description,
                    SupplierCustomerName = apr.SupplierCustomer?.Name ?? "N/A",
                    Amount = apr.Amount,
                    DueDate = apr.DueDate,
                    Type = apr.Type
                })
                .ToList();

            var totalAPagar = contasAPagar.Sum(c => c.Amount);
            var totalAReceber = contasAReceber.Sum(c => c.Amount);
            var saldoProjetado = saldoAtual - totalAPagar + totalAReceber;

            // Agrupar por mês
            var meses = new List<MonthForecastDTO>();
            var saldoAcumulado = saldoAtual;

            for (int i = 0; i < months; i++)
            {
                var mesInicio = today.AddMonths(i);
                var mesFim = today.AddMonths(i + 1).AddDays(-1);

                // Se estamos no mês atual, começar de hoje
                if (i == 0)
                {
                    mesInicio = today;
                }

                var aPagarMes = pendentes
                    .Where(apr => apr.Type == "Pagar" && apr.DueDate.Date >= mesInicio && apr.DueDate.Date <= mesFim)
                    .Sum(apr => apr.Amount);

                var aReceberMes = pendentes
                    .Where(apr => apr.Type == "Receber" && apr.DueDate.Date >= mesInicio && apr.DueDate.Date <= mesFim)
                    .Sum(apr => apr.Amount);

                var saldoMes = aReceberMes - aPagarMes;
                saldoAcumulado += saldoMes;

                var mesData = new DateTime(today.Year, today.Month, 1).AddMonths(i);
                meses.Add(new MonthForecastDTO
                {
                    Month = mesData.ToString("MMM", new System.Globalization.CultureInfo("pt-BR")),
                    Year = mesData.Year,
                    APagar = aPagarMes,
                    AReceber = aReceberMes,
                    Saldo = saldoMes,
                    SaldoAcumulado = saldoAcumulado
                });
            }

            return new FinancialForecastDTO
            {
                SaldoAtual = saldoAtual,
                TotalAPagar = totalAPagar,
                TotalAReceber = totalAReceber,
                SaldoProjetado = saldoProjetado,
                Meses = meses,
                ContasAPagar = contasAPagar,
                ContasAReceber = contasAReceber
            };
        }

        public async Task<EmployeeAccountReportDTO> GetEmployeeAccountReportAsync(long companyId, EmployeeAccountReportFilterDTO filters)
        {
            var startDate = filters.StartDate ?? DateTime.UtcNow.AddMonths(-12);
            var endDate = filters.EndDate ?? DateTime.UtcNow;

            // Buscar funcionário
            var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(filters.EmployeeId);
            if (employee == null || employee.CompanyId != companyId)
            {
                return new EmployeeAccountReportDTO();
            }

            // Buscar todos os dados do funcionário
            var loans = await _unitOfWork.LoanAdvanceRepository.GetByEmployeeIdAsync(filters.EmployeeId);
            var payrollEmployees = await _unitOfWork.PayrollEmployeeRepository.GetByEmployeeIdAsync(filters.EmployeeId);

            // ========================================
            // 1. CALCULAR SALDO ANTERIOR AO PERÍODO
            // ========================================
            long saldoAnterior = 0;

            // Empréstimos anteriores ao período
            foreach (var loan in loans.Where(l => l.LoanDate < startDate))
            {
                saldoAnterior -= loan.Amount; // Negativo - funcionário recebeu dinheiro
            }

            // Folhas anteriores ao período
            foreach (var pe in payrollEmployees)
            {
                var payroll = await _unitOfWork.PayrollRepository.GetOneByIdAsync(pe.PayrollId);
                if (payroll == null || !payroll.IsClosed || payroll.CompanyId != companyId)
                    continue;

                var paymentDate = payroll.PaymentDate ?? payroll.PeriodEndDate;
                if (paymentDate >= startDate)
                    continue; // Só considerar anteriores ao período

                var payrollItems = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(pe.PayrollEmployeeId);
                
                foreach (var item in payrollItems)
                {
                    if (item.SourceType == "loan" || item.SourceType == "thirteenth_loan" || item.SourceType == "vacation_loan")
                        continue;

                    if (item.Type == "Provento")
                        saldoAnterior += item.Amount;
                    else if (item.Type == "Desconto")
                        saldoAnterior -= item.Amount;
                }

                saldoAnterior -= pe.TotalNetPay; // Líquido pago
            }

            // ========================================
            // 2. ITENS DO PERÍODO
            // ========================================
            var items = new List<EmployeeAccountItemDTO>();

            // Adicionar saldo anterior como primeiro item (se houver)
            if (saldoAnterior != 0)
            {
                items.Add(new EmployeeAccountItemDTO
                {
                    Date = startDate.AddDays(-1),
                    Description = "Saldo Anterior",
                    Value = saldoAnterior,
                    Type = saldoAnterior >= 0 ? "Credito" : "Debito",
                    Source = "Folha"
                });
            }

            // Empréstimos do período
            foreach (var loan in loans.Where(l => l.LoanDate >= startDate && l.LoanDate <= endDate))
            {
                // Montar descrição:
                // - Se tem descrição e mais de 1 parcela: "Descrição (X parcelas)"
                // - Se tem descrição e 1 parcela: "Descrição"
                // - Se não tem descrição e mais de 1 parcela: "Empréstimo em X parcelas"
                // - Se não tem descrição e 1 parcela: "Empréstimo"
                string loanDescription;
                if (!string.IsNullOrWhiteSpace(loan.Description))
                {
                    loanDescription = loan.Installments > 1 
                        ? $"{loan.Description} ({loan.Installments} parcelas)" 
                        : loan.Description;
                }
                else
                {
                    loanDescription = loan.Installments > 1 
                        ? $"Empréstimo em {loan.Installments} parcelas" 
                        : "Empréstimo";
                }
                
                items.Add(new EmployeeAccountItemDTO
                {
                    Date = loan.LoanDate,
                    Description = loanDescription,
                    Value = -loan.Amount,
                    Type = "Debito",
                    Source = "Emprestimo"
                });
            }

            // Folhas do período
            foreach (var pe in payrollEmployees)
            {
                var payroll = await _unitOfWork.PayrollRepository.GetOneByIdAsync(pe.PayrollId);
                if (payroll == null || !payroll.IsClosed || payroll.CompanyId != companyId)
                    continue;

                var paymentDate = payroll.PaymentDate ?? payroll.PeriodEndDate;
                if (paymentDate < startDate || paymentDate > endDate)
                    continue;

                var payrollItems = await _unitOfWork.PayrollItemRepository.GetAllByPayrollEmployeeIdAsync(pe.PayrollEmployeeId);
                
                foreach (var item in payrollItems)
                {
                    if (item.SourceType == "loan" || item.SourceType == "thirteenth_loan" || item.SourceType == "vacation_loan")
                        continue;

                    if (item.Type == "Provento")
                    {
                        items.Add(new EmployeeAccountItemDTO
                        {
                            Date = paymentDate,
                            Description = item.Description,
                            Value = item.Amount,
                            Type = "Credito",
                            Source = "Folha"
                        });
                    }
                    else if (item.Type == "Desconto")
                    {
                        items.Add(new EmployeeAccountItemDTO
                        {
                            Date = paymentDate,
                            Description = item.Description,
                            Value = -item.Amount,
                            Type = "Debito",
                            Source = "Folha"
                        });
                    }
                }

                items.Add(new EmployeeAccountItemDTO
                {
                    Date = paymentDate,
                    Description = $"Total Líquido Pago - {payroll.PeriodStartDate:MM/yyyy}",
                    Value = -pe.TotalNetPay,
                    Type = "Debito",
                    Source = "Folha"
                });
            }

            // Ordenar por data, colocando empréstimos por último na mesma data
            // Isso garante que adiantamentos automáticos apareçam após os itens da folha
            items = items
                .OrderBy(i => i.Date)
                .ThenBy(i => i.Source == "Emprestimo" ? 1 : 0)
                .ThenBy(i => i.Description)
                .ToList();

            // Calcular saldo acumulado
            long saldo = 0;
            foreach (var item in items)
            {
                saldo += item.Value;
                item.Balance = saldo;
            }

            return new EmployeeAccountReportDTO
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.FullName,
                EmployeeNickname = employee.Nickname ?? "",
                SaldoFinal = saldo,
                Items = items
            };
        }
    }
}
