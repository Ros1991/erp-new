using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.WebApi.Attributes;
using ERP.Configuration;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de empréstimos e adiantamentos
/// </summary>
[Authorize]
[ApiController]
[Route("api/loan-advance")]
[Tags("RH - Empréstimos/Adiantamentos")]
[ApiExplorerSettings(GroupName = "RH - Empréstimos/Adiantamentos")]
public class LoanAdvanceController : BaseController
{
    private readonly ILoanAdvanceService _service;

    public LoanAdvanceController(ILoanAdvanceService service, ILogger<LoanAdvanceController> logger) : base(logger)
    {
        _service = service;
    }

    [HttpGet("getAll")]
    [RequirePermissions("loanAdvance.canView")]
    public async Task<ActionResult<BaseResponse<List<LoanAdvanceOutputDTO>>>> GetAllAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetAllAsync(companyId), "Empréstimos/Adiantamentos listados com sucesso");
    }

    [HttpGet("getPaged")]
    [RequirePermissions("loanAdvance.canView")]
    public async Task<ActionResult<BaseResponse<PagedResult<LoanAdvanceOutputDTO>>>> GetPagedAsync([FromQuery] LoanAdvanceFilterDTO filters)
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetPagedAsync(companyId, filters), "Empréstimos/Adiantamentos listados com sucesso");
    }

    [HttpGet("{id}")]
    [RequirePermissions("loanAdvance.canView")]
    public async Task<ActionResult<BaseResponse<LoanAdvanceOutputDTO>>> GetOneByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteAsync(() => _service.GetOneByIdAsync(id), "Empréstimo/Adiantamento encontrado com sucesso");
    }

    [HttpPost("create")]
    [RequirePermissions("loanAdvance.canCreate")]
    public async Task<ActionResult<BaseResponse<LoanAdvanceOutputDTO>>> CreateAsync(LoanAdvanceInputDTO dto)
    {
        var companyId = GetCompanyId();
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteCreateAsync(
            () => _service.CreateAsync(dto, companyId, currentUserId),
            nameof(GetOneByIdAsync),
            result => new { id = result.LoanAdvanceId },
            "Empréstimo/Adiantamento criado com sucesso"
        );
    }

    [HttpPut("{id}")]
    [RequirePermissions("loanAdvance.canEdit")]
    public async Task<ActionResult<BaseResponse<LoanAdvanceOutputDTO>>> UpdateByIdAsync(long id, LoanAdvanceInputDTO dto)
    {
        ValidateId(id, nameof(id));
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(() => _service.UpdateByIdAsync(id, dto, currentUserId), "Empréstimo/Adiantamento atualizado com sucesso");
    }

    [HttpDelete("{id}")]
    [RequirePermissions("loanAdvance.canDelete")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteBooleanAsync(
            () => _service.DeleteByIdAsync(id),
            "Empréstimo/Adiantamento deletado com sucesso",
            "Empréstimo/Adiantamento não encontrado ou não pôde ser deletado"
        );
    }
}
