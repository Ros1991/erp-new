namespace ERP.Application.DTOs
{
    public class PayrollDetailedOutputDTO
    {
        public long PayrollId { get; set; }
        public long CompanyId { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public long TotalGrossPay { get; set; }
        public long TotalDeductions { get; set; }
        public long TotalNetPay { get; set; }
        public long TotalInss { get; set; }
        public long TotalFgts { get; set; }
        public int? ThirteenthPercentage { get; set; }
        public string? ThirteenthTaxOption { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedAt { get; set; }
        public long? ClosedBy { get; set; }
        public string? ClosedByName { get; set; }
        public string? Notes { get; set; }
        public string? Snapshot { get; set; }
        public int EmployeeCount { get; set; }
        public bool IsLastPayroll { get; set; }
        public long CriadoPor { get; set; }
        public string? CriadoPorNome { get; set; }
        public long? AtualizadoPor { get; set; }
        public string? AtualizadoPorNome { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
        
        // Empregados com seus itens
        public List<PayrollEmployeeDetailedDTO> Employees { get; set; } = new();
    }

    public class PayrollEmployeeDetailedDTO
    {
        public long PayrollEmployeeId { get; set; }
        public long PayrollId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string? EmployeeDocument { get; set; }
        public bool IsOnVacation { get; set; }
        public long? VacationDays { get; set; }
        public long? VacationAdvanceAmount { get; set; }
        public long TotalGrossPay { get; set; }
        public long TotalDeductions { get; set; }
        public long TotalNetPay { get; set; }
        
        // Informações do contrato
        public long? ContractId { get; set; }
        public string? ContractType { get; set; } // "Mensalista", "Horista", "Diarista"
        public long? ContractValue { get; set; } // Valor base do contrato em centavos
        public decimal? WorkedUnits { get; set; } // Horas ou dias trabalhados (para horistas/diaristas)
        public bool HasFgts { get; set; } // Se o contrato tem FGTS
        
        // Itens da folha (débitos e créditos)
        public List<PayrollItemDetailedDTO> Items { get; set; } = new();
    }

    public class PayrollItemDetailedDTO
    {
        public long PayrollItemId { get; set; }
        public long PayrollEmployeeId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // "Credit" ou "Debit"
        public string Category { get; set; }
        public long Amount { get; set; }
        public long? ReferenceId { get; set; }
        public long? CalculationBasis { get; set; }
        public string? CalculationDetails { get; set; }
    }
}
