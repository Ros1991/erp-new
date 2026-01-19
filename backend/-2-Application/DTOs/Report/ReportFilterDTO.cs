using System;

namespace ERP.Application.DTOs
{
    public class ReportFilterDTO
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class SupplierCustomerReportFilterDTO : ReportFilterDTO
    {
        public string? Type { get; set; } // Pagar, Receber, Todos
    }

    public class AccountPayableReceivableReportFilterDTO
    {
        public string? Type { get; set; } // Pagar, Receber, Todos
    }

    public class EmployeeAccountReportFilterDTO : ReportFilterDTO
    {
        public long EmployeeId { get; set; }
    }
}
