using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.Services;
using ERP.WebApi.Controllers.Base;
using ERP.WebApi.Attributes;

namespace ERP.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService, ILogger<ReportController> logger) : base(logger)
        {
            _reportService = reportService;
        }

        [HttpGet("financial-summary")]
        [RequirePermissions("report.canView")]
        public async Task<ActionResult<BaseResponse<FinancialSummaryDTO>>> GetFinancialSummary([FromQuery] ReportFilterDTO filters)
        {
            var companyId = GetCompanyId();
            return await ExecuteAsync(
                () => _reportService.GetFinancialSummaryAsync(companyId, filters),
                "Resumo financeiro obtido com sucesso"
            );
        }

        [HttpGet("cost-center")]
        [RequirePermissions("report.canView")]
        public async Task<ActionResult<BaseResponse<List<CostCenterReportDTO>>>> GetCostCenterReport([FromQuery] ReportFilterDTO filters)
        {
            var companyId = GetCompanyId();
            return await ExecuteAsync(
                () => _reportService.GetCostCenterReportAsync(companyId, filters),
                "Relatório por centro de custo obtido com sucesso"
            );
        }

        [HttpGet("account")]
        [RequirePermissions("report.canView")]
        public async Task<ActionResult<BaseResponse<List<AccountReportDTO>>>> GetAccountReport([FromQuery] ReportFilterDTO filters)
        {
            var companyId = GetCompanyId();
            return await ExecuteAsync(
                () => _reportService.GetAccountReportAsync(companyId, filters),
                "Relatório por conta obtido com sucesso"
            );
        }

        [HttpGet("supplier-customer")]
        [RequirePermissions("report.canView")]
        public async Task<ActionResult<BaseResponse<List<SupplierCustomerReportDTO>>>> GetSupplierCustomerReport([FromQuery] SupplierCustomerReportFilterDTO filters)
        {
            var companyId = GetCompanyId();
            return await ExecuteAsync(
                () => _reportService.GetSupplierCustomerReportAsync(companyId, filters),
                "Relatório por fornecedor/cliente obtido com sucesso"
            );
        }

        [HttpGet("cash-flow")]
        [RequirePermissions("report.canView")]
        public async Task<ActionResult<BaseResponse<List<CashFlowItemDTO>>>> GetCashFlow([FromQuery] ReportFilterDTO filters)
        {
            var companyId = GetCompanyId();
            return await ExecuteAsync(
                () => _reportService.GetCashFlowAsync(companyId, filters),
                "Fluxo de caixa obtido com sucesso"
            );
        }

        [HttpGet("accounts-payable-receivable")]
        [RequirePermissions("report.canView")]
        public async Task<ActionResult<BaseResponse<AccountPayableReceivableReportDTO>>> GetAccountPayableReceivableReport([FromQuery] AccountPayableReceivableReportFilterDTO filters)
        {
            var companyId = GetCompanyId();
            return await ExecuteAsync(
                () => _reportService.GetAccountPayableReceivableReportAsync(companyId, filters),
                "Relatório de contas a pagar/receber obtido com sucesso"
            );
        }

        [HttpGet("financial-forecast")]
        [RequirePermissions("report.canView")]
        public async Task<ActionResult<BaseResponse<FinancialForecastDTO>>> GetFinancialForecast([FromQuery] int months = 6)
        {
            var companyId = GetCompanyId();
            return await ExecuteAsync(
                () => _reportService.GetFinancialForecastAsync(companyId, months),
                "Previsão financeira obtida com sucesso"
            );
        }

        [HttpGet("employee-account")]
        [RequirePermissions("report.canView")]
        public async Task<ActionResult<BaseResponse<EmployeeAccountReportDTO>>> GetEmployeeAccountReport([FromQuery] EmployeeAccountReportFilterDTO filters)
        {
            var companyId = GetCompanyId();
            return await ExecuteAsync(
                () => _reportService.GetEmployeeAccountReportAsync(companyId, filters),
                "Conta corrente do funcionário obtida com sucesso"
            );
        }
    }
}
