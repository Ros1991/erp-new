using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.WebApi.Attributes;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de folhas de pagamento
/// </summary>
/// <remarks>
/// Este controller fornece endpoints RESTful para operações de folhas de pagamento,
/// incluindo criação automática, listagem e gerenciamento de períodos.
/// </remarks>
[Authorize]
[ApiController]
[Route("api/payroll")]
[Tags("Folha de Pagamento")]
[ApiExplorerSettings(GroupName = "Folha de Pagamento")]
public class PayrollController : BaseController
{
    private readonly IPayrollService _payrollService;

    public PayrollController(IPayrollService payrollService, ILogger<PayrollController> logger) : base(logger)
    {
        _payrollService = payrollService;
    }

    [HttpGet("getAll")]
    [RequirePermissions("payroll.canView")]
    public async Task<ActionResult<BaseResponse<List<PayrollOutputDTO>>>> GetAllAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _payrollService.GetAllAsync(companyId), "Folhas de pagamento listadas com sucesso");
    }

    [HttpGet("getPaged")]
    [RequirePermissions("payroll.canView")]
    public async Task<ActionResult<BaseResponse<PagedResult<PayrollOutputDTO>>>> GetPagedAsync([FromQuery] PayrollFilterDTO filters)
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _payrollService.GetPagedAsync(companyId, filters), "Folhas de pagamento listadas com sucesso");
    }

    [HttpGet("{payrollId}")]
    [RequirePermissions("payroll.canView")]
    public async Task<ActionResult<BaseResponse<PayrollOutputDTO>>> GetOneByIdAsync(long payrollId)
    {
        ValidateId(payrollId, nameof(payrollId));
        return await ExecuteAsync(() => _payrollService.GetOneByIdAsync(payrollId), "Folha de pagamento encontrada com sucesso");
    }

    [HttpGet("{payrollId}/details")]
    [RequirePermissions("payroll.canView")]
    public async Task<ActionResult<BaseResponse<PayrollDetailedOutputDTO>>> GetDetailedByIdAsync(long payrollId)
    {
        ValidateId(payrollId, nameof(payrollId));
        return await ExecuteAsync(() => _payrollService.GetDetailedByIdAsync(payrollId), "Detalhes da folha de pagamento carregados com sucesso");
    }

    [HttpPost("create")]
    [RequirePermissions("payroll.canCreate")]
    public async Task<ActionResult<BaseResponse<PayrollOutputDTO>>> CreateAsync(PayrollInputDTO dto)
    {
        var companyId = GetCompanyId();
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteCreateAsync(
            () => _payrollService.CreatePayrollAsync(companyId, currentUserId, dto),
            nameof(GetOneByIdAsync),
            result => new { payrollId = result.PayrollId },
            "Folha de pagamento criada com sucesso"
        );
    }

    [HttpPut("{payrollId}")]
    [RequirePermissions("payroll.canEdit")]
    public async Task<ActionResult<BaseResponse<PayrollOutputDTO>>> UpdateByIdAsync(long payrollId, PayrollInputDTO dto)
    {
        ValidateId(payrollId, nameof(payrollId));
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(() => _payrollService.UpdateByIdAsync(payrollId, dto, currentUserId), "Folha de pagamento atualizada com sucesso");
    }

    [HttpDelete("{payrollId}")]
    [RequirePermissions("payroll.canDelete")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long payrollId)
    {
        ValidateId(payrollId, nameof(payrollId));
        return await ExecuteBooleanAsync(
            () => _payrollService.DeleteByIdAsync(payrollId),
            "Folha de pagamento deletada com sucesso",
            "Folha de pagamento não encontrada ou não pôde ser deletada"
        );
    }

    [HttpPost("{payrollId}/recalculate")]
    [RequirePermissions("payroll.canEdit")]
    public async Task<ActionResult<BaseResponse<PayrollDetailedOutputDTO>>> RecalculateAsync(long payrollId)
    {
        ValidateId(payrollId, nameof(payrollId));
        var currentUserId = GetCurrentUserId();
        return await ExecuteAsync(
            () => _payrollService.RecalculatePayrollAsync(payrollId, currentUserId),
            "Folha de pagamento recalculada com sucesso"
        );
    }

    [HttpPost("employee/{payrollEmployeeId}/recalculate")]
    [RequirePermissions("payroll.canEdit")]
    public async Task<ActionResult<BaseResponse<PayrollEmployeeDetailedDTO>>> RecalculateEmployeeAsync(long payrollEmployeeId)
    {
        ValidateId(payrollEmployeeId, nameof(payrollEmployeeId));
        var currentUserId = GetCurrentUserId();
        return await ExecuteAsync(
            () => _payrollService.RecalculatePayrollEmployeeAsync(payrollEmployeeId, currentUserId),
            "Funcionário recalculado com sucesso"
        );
    }

    [HttpPut("item/{payrollItemId}")]
    [RequirePermissions("payroll.canEdit")]
    public async Task<ActionResult<BaseResponse<PayrollItemDetailedDTO>>> UpdatePayrollItemAsync(long payrollItemId, UpdatePayrollItemDTO dto)
    {
        ValidateId(payrollItemId, nameof(payrollItemId));
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(
            () => _payrollService.UpdatePayrollItemAsync(payrollItemId, dto, currentUserId),
            "Item da folha de pagamento atualizado com sucesso"
        );
    }

    [HttpPut("employee/{payrollEmployeeId}/worked-units")]
    [RequirePermissions("payroll.canEdit")]
    public async Task<ActionResult<BaseResponse<PayrollEmployeeDetailedDTO>>> UpdateWorkedUnitsAsync(long payrollEmployeeId, UpdateWorkedUnitsDTO dto)
    {
        ValidateId(payrollEmployeeId, nameof(payrollEmployeeId));
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(
            () => _payrollService.UpdateWorkedUnitsAsync(payrollEmployeeId, dto, currentUserId),
            "Horas/dias trabalhados atualizados com sucesso"
        );
    }

    [HttpPost("item")]
    [RequirePermissions("payroll.canEdit")]
    public async Task<ActionResult<BaseResponse<PayrollEmployeeDetailedDTO>>> AddPayrollItemAsync(PayrollItemInputDTO dto)
    {
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(
            () => _payrollService.AddPayrollItemAsync(dto, currentUserId),
            "Item adicionado com sucesso"
        );
    }
}
