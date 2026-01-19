using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.WebApi.Attributes;
using System.Collections.Generic;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de configurações da empresa
/// </summary>
[Authorize]
[ApiController]
[Route("api/company-setting")]
[Tags("Empresa - Configurações")]
[ApiExplorerSettings(GroupName = "Empresa - Configurações")]
public class CompanySettingController : BaseController
{
    private readonly ICompanySettingService _service;

    public CompanySettingController(ICompanySettingService service, ILogger<CompanySettingController> logger) : base(logger)
    {
        _service = service;
    }

    [HttpGet]
    [RequirePermissions("companySettings.canView")]
    public async Task<ActionResult<BaseResponse<CompanySettingOutputDTO>>> GetAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetByCompanyIdAsync(companyId), "Configurações carregadas com sucesso");
    }

    [HttpPut]
    [RequirePermissions("companySettings.canEdit")]
    public async Task<ActionResult<BaseResponse<CompanySettingOutputDTO>>> SaveAsync(CompanySettingInputDTO dto)
    {
        var companyId = GetCompanyId();
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(() => _service.SaveAsync(dto, companyId, currentUserId), "Configurações salvas com sucesso");
    }

    [HttpGet("default-distributions")]
    [RequirePermissions("companySettings.canView")]
    public async Task<ActionResult<BaseResponse<List<DefaultCostCenterDistributionDTO>>>> GetDefaultDistributionsAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _service.GetDefaultDistributionsAsync(companyId), "Rateio padrão carregado com sucesso");
    }
}
