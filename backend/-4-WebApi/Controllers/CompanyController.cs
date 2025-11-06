using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.Configuration;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de company do sistema ERP
/// </summary>
/// <remarks>
/// Este controller fornece endpoints RESTful para operações CRUD em company,
/// incluindo busca com filtros por nome e validações de negócio.
/// 
/// **company** representam as funções/posições profissionais dentro do sistema,
/// sendo uma entidade fundamental para organização hierárquica e funcional.
/// 
/// </remarks>
//[SwaggerGroupOrder(200)] // Cadastros - company: Primeiro grupo Cadastro (ordem 200)
[Authorize] // ✅ Requer autenticação
[ApiController]
[Route("api/company")]
[Tags("Cadastros - company")]
[ApiExplorerSettings(GroupName = "Cadastros - company")]
public class CompanyController : BaseController
{
    private readonly ICompanyService _CompanyService;

    public CompanyController(ICompanyService CompanyService, ILogger<CompanyController> logger) : base(logger)
    {
        _CompanyService = CompanyService;
    }

    [HttpGet("/company/getAll/")]
    public async Task<ActionResult<BaseResponse<List<CompanyOutputDTO>>>> GetAllAsync()
    {
        return await ExecuteAsync(() => _CompanyService.GetAllAsync(), "Empresas listadas com sucesso");
    }

    [HttpGet("/company/getPaged/")]
    public async Task<ActionResult<BaseResponse<PagedResult<CompanyOutputDTO>>>> GetPagedAsync([FromQuery] CompanyFilterDTO filters)
    {
        return await ExecuteAsync(() => _CompanyService.GetPagedAsync(filters), "Empresas listadas com sucesso");
    }

    [HttpGet("/company/{companyId}/getOneById/")]
    public async Task<ActionResult<BaseResponse<CompanyOutputDTO>>> GetOneByIdAsync(long CompanyId)
    {
        //ValidateId(CompanyId, nameof(CompanyId));
        return await ExecuteAsync(() => _CompanyService.GetOneByIdAsync(CompanyId), "Empresa encontrada com sucesso");
    }
    
    [HttpPost("/company/create/")]
    public async Task<ActionResult<BaseResponse<CompanyOutputDTO>>> CreateAsync(CompanyInputDTO dto)
    {
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteCreateAsync(
            () => _CompanyService.CreateAsync(dto, currentUserId),
            nameof(GetOneByIdAsync),
            result => new { company_id = result.CompanyId },
            "Empresa criada com sucesso"
        );
    }
    
    [HttpPut("/company/{companyId}/updateById/")]
    public async Task<ActionResult<BaseResponse<CompanyOutputDTO>>> UpdateByIdAsync(long CompanyId, CompanyInputDTO dto)
    {
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(() => _CompanyService.UpdateByIdAsync(CompanyId, dto, currentUserId), "Empresa atualizada com sucesso");
    }
    
    [HttpDelete("/company/{companyId}/deleteById/")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long CompanyId)
    {
        //ValidateId(CompanyId, nameof(CompanyId));
        return await ExecuteBooleanAsync(
            () => _CompanyService.DeleteByIdAsync(CompanyId),
            "Empresa deletada com sucesso",
            "Empresa não encontrada ou não pôde ser deletada"
        );
    }
}
