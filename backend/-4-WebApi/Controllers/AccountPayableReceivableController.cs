using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.WebApi.Attributes;
using ERP.Configuration;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de contas a pagar/receber
/// </summary>
[Authorize]
[ApiController]
[Route("api/account-payable-receivable")]
[Tags("Financeiro - Contas a Pagar/Receber")]
[ApiExplorerSettings(GroupName = "Financeiro - Contas a Pagar/Receber")]
public class AccountPayableReceivableController : BaseController
{
    private readonly IAccountPayableReceivableService _service;

    public AccountPayableReceivableController(IAccountPayableReceivableService service, ILogger<AccountPayableReceivableController> logger) : base(logger)
    {
        _service = service;
    }

    [HttpGet("getAll")]
    [RequirePermissions("accountPayableReceivable.canView")]
    public async Task<ActionResult<BaseResponse<List<AccountPayableReceivableOutputDTO>>>> GetAllAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetAllAsync(companyId), "Contas a pagar e receber listadas com sucesso");
    }

    [HttpGet("getPaged")]
    [RequirePermissions("accountPayableReceivable.canView")]
    public async Task<ActionResult<BaseResponse<PagedResult<AccountPayableReceivableOutputDTO>>>> GetPagedAsync([FromQuery] AccountPayableReceivableFilterDTO filters)
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetPagedAsync(companyId, filters), "Contas a pagar e receber listadas com sucesso");
    }

    [HttpGet("{id}")]
    [RequirePermissions("accountPayableReceivable.canView")]
    public async Task<ActionResult<BaseResponse<AccountPayableReceivableOutputDTO>>> GetOneByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteAsync(() => _service.GetOneByIdAsync(id), "Conta a pagar e receber encontrada com sucesso");
    }

    [HttpPost("create")]
    [RequirePermissions("accountPayableReceivable.canCreate")]
    public async Task<ActionResult<BaseResponse<AccountPayableReceivableOutputDTO>>> CreateAsync(AccountPayableReceivableInputDTO dto)
    {
        var companyId = GetCompanyId();
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteCreateAsync(
            () => _service.CreateAsync(dto, companyId, currentUserId),
            nameof(GetOneByIdAsync),
            result => new { id = result.AccountPayableReceivableId },
            "Conta a pagar e receber criada com sucesso"
        );
    }

    [HttpPut("{id}")]
    [RequirePermissions("accountPayableReceivable.canEdit")]
    public async Task<ActionResult<BaseResponse<AccountPayableReceivableOutputDTO>>> UpdateByIdAsync(long id, AccountPayableReceivableInputDTO dto)
    {
        ValidateId(id, nameof(id));
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(() => _service.UpdateByIdAsync(id, dto, currentUserId), "Conta a pagar e receber atualizada com sucesso");
    }

    [HttpDelete("{id}")]
    [RequirePermissions("accountPayableReceivable.canDelete")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteBooleanAsync(
            () => _service.DeleteByIdAsync(id),
            "Conta a pagar e receber deletada com sucesso",
            "Conta a pagar e receber não encontrada ou não pôde ser deletada"
        );
    }
}
