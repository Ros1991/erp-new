using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.WebApi.Attributes;
using ERP.Configuration;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de centros de custo
/// </summary>
[Authorize]
[ApiController]
[Route("api/cost-center")]
[Tags("Financeiro - Centros de Custo")]
[ApiExplorerSettings(GroupName = "Financeiro - Centros de Custo")]
public class CostCenterController : BaseController
{
    private readonly ICostCenterService _service;

    public CostCenterController(ICostCenterService service, ILogger<CostCenterController> logger) : base(logger)
    {
        _service = service;
    }

    [HttpGet("getAll")]
    [RequirePermissions("costCenter.canView")]
    public async Task<ActionResult<BaseResponse<List<CostCenterOutputDTO>>>> GetAllAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetAllAsync(companyId), "Centros de custo listados com sucesso");
    }

    [HttpGet("getPaged")]
    [RequirePermissions("costCenter.canView")]
    public async Task<ActionResult<BaseResponse<PagedResult<CostCenterOutputDTO>>>> GetPagedAsync([FromQuery] CostCenterFilterDTO filters)
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetPagedAsync(companyId, filters), "Centros de custo listados com sucesso");
    }

    [HttpGet("{id}")]
    [RequirePermissions("costCenter.canView")]
    public async Task<ActionResult<BaseResponse<CostCenterOutputDTO>>> GetOneByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteAsync(() => _service.GetOneByIdAsync(id), "Centro de custo encontrado com sucesso");
    }

    [HttpPost("create")]
    [RequirePermissions("costCenter.canCreate")]
    public async Task<ActionResult<BaseResponse<CostCenterOutputDTO>>> CreateAsync(CostCenterInputDTO dto)
    {
        var companyId = GetCompanyId();
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteCreateAsync(
            () => _service.CreateAsync(dto, companyId, currentUserId),
            nameof(GetOneByIdAsync),
            result => new { id = result.CostCenterId },
            "Centro de custo criado com sucesso"
        );
    }

    [HttpPut("{id}")]
    [RequirePermissions("costCenter.canEdit")]
    public async Task<ActionResult<BaseResponse<CostCenterOutputDTO>>> UpdateByIdAsync(long id, CostCenterInputDTO dto)
    {
        ValidateId(id, nameof(id));
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(() => _service.UpdateByIdAsync(id, dto, currentUserId), "Centro de custo atualizado com sucesso");
    }

    [HttpDelete("{id}")]
    [RequirePermissions("costCenter.canDelete")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteBooleanAsync(
            () => _service.DeleteByIdAsync(id),
            "Centro de custo deletado com sucesso",
            "Centro de custo não encontrado ou não pôde ser deletado"
        );
    }
}
