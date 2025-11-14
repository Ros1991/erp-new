using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.WebApi.Attributes;
using ERP.Configuration;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de locais
/// </summary>
[Authorize]
[ApiController]
[Route("api/location")]
[Tags("Cadastros - Locais")]
[ApiExplorerSettings(GroupName = "Cadastros - Locais")]
public class LocationController : BaseController
{
    private readonly ILocationService _service;

    public LocationController(ILocationService service, ILogger<LocationController> logger) : base(logger)
    {
        _service = service;
    }

    [HttpGet("getAll")]
    [RequirePermissions("location.canView")]
    public async Task<ActionResult<BaseResponse<List<LocationOutputDTO>>>> GetAllAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetAllAsync(companyId), "Locais listados com sucesso");
    }

    [HttpGet("getPaged")]
    [RequirePermissions("location.canView")]
    public async Task<ActionResult<BaseResponse<PagedResult<LocationOutputDTO>>>> GetPagedAsync([FromQuery] LocationFilterDTO filters)
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetPagedAsync(companyId, filters), "Locais listados com sucesso");
    }

    [HttpGet("{id}")]
    [RequirePermissions("location.canView")]
    public async Task<ActionResult<BaseResponse<LocationOutputDTO>>> GetOneByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteAsync(() => _service.GetOneByIdAsync(id), "Local encontrado com sucesso");
    }

    [HttpPost("create")]
    [RequirePermissions("location.canCreate")]
    public async Task<ActionResult<BaseResponse<LocationOutputDTO>>> CreateAsync(LocationInputDTO dto)
    {
        var companyId = GetCompanyId();
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteCreateAsync(
            () => _service.CreateAsync(dto, companyId, currentUserId),
            nameof(GetOneByIdAsync),
            result => new { id = result.LocationId },
            "Local criado com sucesso"
        );
    }

    [HttpPut("{id}")]
    [RequirePermissions("location.canEdit")]
    public async Task<ActionResult<BaseResponse<LocationOutputDTO>>> UpdateByIdAsync(long id, LocationInputDTO dto)
    {
        ValidateId(id, nameof(id));
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(() => _service.UpdateByIdAsync(id, dto, currentUserId), "Local atualizado com sucesso");
    }

    [HttpDelete("{id}")]
    [RequirePermissions("location.canDelete")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long id)
    {
        ValidateId(id, nameof(id));
        return await ExecuteBooleanAsync(
            () => _service.DeleteByIdAsync(id),
            "Local deletado com sucesso",
            "Local não encontrado ou não pôde ser deletado"
        );
    }
}
