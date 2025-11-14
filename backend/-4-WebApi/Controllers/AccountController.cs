using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.WebApi.Attributes;
using ERP.Configuration;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de contas financeiras
/// </summary>
/// <remarks>
/// Este controller fornece endpoints RESTful para operações CRUD em contas,
/// incluindo gerenciamento de contas bancárias e financeiras da empresa.
/// </remarks>
[Authorize]
[ApiController]
[Route("api/account")]
[Tags("Cadastros - Account")]
[ApiExplorerSettings(GroupName = "Cadastros - Account")]
public class AccountController : BaseController
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService, ILogger<AccountController> logger) : base(logger)
    {
        _accountService = accountService;
    }

    [HttpGet("getAll")]
    [RequirePermissions("account.canView")]
    public async Task<ActionResult<BaseResponse<List<AccountOutputDTO>>>> GetAllAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _accountService.GetAllAsync(companyId), "Contas listadas com sucesso");
    }

    [HttpGet("getPaged")]
    [RequirePermissions("account.canView")]
    public async Task<ActionResult<BaseResponse<PagedResult<AccountOutputDTO>>>> GetPagedAsync([FromQuery] AccountFilterDTO filters)
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _accountService.GetPagedAsync(companyId, filters), "Contas listadas com sucesso");
    }

    [HttpGet("{accountId}")]
    [RequirePermissions("account.canView")]
    public async Task<ActionResult<BaseResponse<AccountOutputDTO>>> GetOneByIdAsync(long accountId)
    {
        ValidateId(accountId, nameof(accountId));
        return await ExecuteAsync(() => _accountService.GetOneByIdAsync(accountId), "Conta encontrada com sucesso");
    }

    [HttpPost("create")]
    [RequirePermissions("account.canCreate")]
    public async Task<ActionResult<BaseResponse<AccountOutputDTO>>> CreateAsync(AccountInputDTO dto)
    {
        var companyId = GetCompanyId();
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteCreateAsync(
            () => _accountService.CreateAsync(dto, companyId, currentUserId),
            nameof(GetOneByIdAsync),
            result => new { accountId = result.AccountId },
            "Conta criada com sucesso"
        );
    }

    [HttpPut("{accountId}")]
    [RequirePermissions("account.canEdit")]
    public async Task<ActionResult<BaseResponse<AccountOutputDTO>>> UpdateByIdAsync(long accountId, AccountInputDTO dto)
    {
        ValidateId(accountId, nameof(accountId));
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(() => _accountService.UpdateByIdAsync(accountId, dto, currentUserId), "Conta atualizada com sucesso");
    }

    [HttpDelete("{accountId}")]
    [RequirePermissions("account.canDelete")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long accountId)
    {
        ValidateId(accountId, nameof(accountId));
        return await ExecuteBooleanAsync(
            () => _accountService.DeleteByIdAsync(accountId),
            "Conta deletada com sucesso",
            "Conta não encontrada ou não pôde ser deletada"
        );
    }
}
