using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.WebApi.Attributes;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de contratos de funcionários
/// </summary>
[Authorize]
[ApiController]
[Route("api/contract")]
[Tags("Cadastros - Contract")]
[ApiExplorerSettings(GroupName = "Cadastros - Contract")]
public class ContractController : BaseController
{
    private readonly IContractService _contractService;

    public ContractController(IContractService contractService, ILogger<ContractController> logger) : base(logger)
    {
        _contractService = contractService;
    }

    [HttpGet("employee/{employeeId}/all")]
    [RequirePermissions("employee.canView")]
    public async Task<ActionResult<BaseResponse<List<ContractOutputDTO>>>> GetAllByEmployeeIdAsync(long employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));
        return await ExecuteAsync(() => _contractService.GetAllByEmployeeIdAsync(employeeId), "Contratos listados com sucesso");
    }

    [HttpGet("employee/{employeeId}/active")]
    [RequirePermissions("employee.canView")]
    public async Task<ActionResult<BaseResponse<ContractOutputDTO>>> GetActiveByEmployeeIdAsync(long employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));
        return await ExecuteAsync(() => _contractService.GetActiveByEmployeeIdAsync(employeeId), "Contrato ativo obtido com sucesso");
    }

    [HttpGet("{contractId}")]
    [RequirePermissions("employee.canView")]
    public async Task<ActionResult<BaseResponse<ContractOutputDTO>>> GetOneByIdAsync(long contractId)
    {
        ValidateId(contractId, nameof(contractId));
        return await ExecuteAsync(() => _contractService.GetOneByIdAsync(contractId), "Contrato obtido com sucesso");
    }

    [HttpPost]
    [RequirePermissions("employee.canEdit")]
    public async Task<ActionResult<BaseResponse<ContractOutputDTO>>> CreateAsync([FromBody] ContractInputDTO dto)
    {
        var currentUserId = GetCurrentUserId();
        return await ExecuteAsync(() => _contractService.CreateAsync(dto, currentUserId), "Contrato criado com sucesso");
    }

    [HttpPut("{contractId}")]
    [RequirePermissions("employee.canEdit")]
    public async Task<ActionResult<BaseResponse<ContractOutputDTO>>> UpdateByIdAsync(long contractId, [FromBody] ContractInputDTO dto)
    {
        ValidateId(contractId, nameof(contractId));
        var currentUserId = GetCurrentUserId();
        return await ExecuteAsync(() => _contractService.UpdateByIdAsync(contractId, dto, currentUserId), "Contrato atualizado com sucesso");
    }

    [HttpDelete("{contractId}")]
    [RequirePermissions("employee.canDelete")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long contractId)
    {
        ValidateId(contractId, nameof(contractId));
        return await ExecuteAsync(() => _contractService.DeleteByIdAsync(contractId), "Contrato excluído com sucesso");
    }
}
