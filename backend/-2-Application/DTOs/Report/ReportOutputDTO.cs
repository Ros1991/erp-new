using System;
using System.Collections.Generic;

namespace ERP.Application.DTOs
{
    // Dashboard Financeiro
    public class FinancialSummaryDTO
    {
        public long TotalReceitas { get; set; }
        public long TotalDespesas { get; set; }
        public long Saldo { get; set; }
        public int QuantidadeContas { get; set; }
        public List<MonthValueDTO> ReceitasPorMes { get; set; } = new();
        public List<MonthValueDTO> DespesasPorMes { get; set; } = new();
    }

    public class MonthValueDTO
    {
        public string Month { get; set; } = string.Empty;
        public long Value { get; set; }
    }

    // Relatório por Centro de Custo
    public class CostCenterReportDTO
    {
        public long CostCenterId { get; set; }
        public string CostCenterName { get; set; } = string.Empty;
        public long TotalReceitas { get; set; }
        public long TotalDespesas { get; set; }
        public long Saldo { get; set; }
        public decimal Percentual { get; set; }
    }

    // Relatório por Conta Corrente
    public class AccountReportDTO
    {
        public long AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public long SaldoInicial { get; set; }
        public long TotalEntradas { get; set; }
        public long TotalSaidas { get; set; }
        public long SaldoFinal { get; set; }
    }

    // Relatório por Fornecedor/Cliente
    public class SupplierCustomerReportDTO
    {
        public long SupplierCustomerId { get; set; }
        public string SupplierCustomerName { get; set; } = string.Empty;
        public long TotalPago { get; set; }
        public long TotalRecebido { get; set; }
        public int QuantidadeTransacoes { get; set; }
    }

    // Fluxo de Caixa
    public class CashFlowItemDTO
    {
        public DateTime Date { get; set; }
        public long Entradas { get; set; }
        public long Saidas { get; set; }
        public long Saldo { get; set; }
        public long SaldoAcumulado { get; set; }
    }

    // Contas a Pagar/Receber
    public class AccountPayableReceivableReportDTO
    {
        public StatusSummaryDTO Vencidas { get; set; } = new();
        public StatusSummaryDTO VencendoHoje { get; set; } = new();
        public StatusSummaryDTO Vencendo7Dias { get; set; } = new();
        public StatusSummaryDTO Vencendo30Dias { get; set; } = new();
        public StatusSummaryDTO AVencer { get; set; } = new();
        public List<AccountPayableReceivableItemDTO> Items { get; set; } = new();
    }

    public class StatusSummaryDTO
    {
        public int Quantidade { get; set; }
        public long Valor { get; set; }
    }

    public class AccountPayableReceivableItemDTO
    {
        public long Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string SupplierCustomerName { get; set; } = string.Empty;
        public long Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public int DaysOverdue { get; set; }
    }

    // Previsão Financeira
    public class FinancialForecastDTO
    {
        public long SaldoAtual { get; set; }
        public long TotalAPagar { get; set; }
        public long TotalAReceber { get; set; }
        public long SaldoProjetado { get; set; }
        public List<MonthForecastDTO> Meses { get; set; } = new();
        public List<ForecastItemDTO> ContasAPagar { get; set; } = new();
        public List<ForecastItemDTO> ContasAReceber { get; set; } = new();
    }

    public class MonthForecastDTO
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public long APagar { get; set; }
        public long AReceber { get; set; }
        public long Saldo { get; set; }
        public long SaldoAcumulado { get; set; }
    }

    public class ForecastItemDTO
    {
        public long Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string SupplierCustomerName { get; set; } = string.Empty;
        public long Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
