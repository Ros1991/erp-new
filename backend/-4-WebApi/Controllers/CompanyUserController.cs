using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.WebApi.Attributes;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de usuários da empresa (CompanyUsers)
/// </summary>
/// <remarks>
/// Este controller fornece endpoints RESTful para operações CRUD em vínculos de usuários com empresas,
/// incluindo atribuição de cargos/roles.
/// </remarks>
[Authorize]
[ApiController]
[Route("api/companyuser")]
[Tags("Cadastros - CompanyUser")]
[ApiExplorerSettings(GroupName = "Cadastros - CompanyUser")]
public class CompanyUserController : BaseController
{
    private readonly ICompanyUserService _companyUserService;

    public CompanyUserController(ICompanyUserService companyUserService, ILogger<CompanyUserController> logger) : base(logger)
    {
        _companyUserService = companyUserService;
    }

    /// <summary>
    /// Obter todos os usuários da empresa atual
    /// </summary>
    [HttpGet("getAll")]
    [RequirePermissions("user.canView")]
    public async Task<ActionResult<BaseResponse<List<CompanyUserOutputDTO>>>> GetAllAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(
            () => _companyUserService.GetAllByCompanyAsync(companyId), 
            "Usuários listados com sucesso"
        );
    }

    /// <summary>
    /// Obter usuários da empresa com paginação e filtros
    /// </summary>
    [HttpGet("getPaged")]
    [RequirePermissions("user.canView")]
    public async Task<ActionResult<BaseResponse<PagedResult<CompanyUserOutputDTO>>>> GetPagedAsync([FromQuery] CompanyUserFilterDTO filters)
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(
            () => _companyUserService.GetPagedAsync(companyId, filters), 
            "Usuários listados com sucesso"
        );
    }

    /// <summary>
    /// Obter um usuário específico da empresa
    /// </summary>
    [HttpGet("{companyUserId}")]
    [RequirePermissions("user.canView")]
    public async Task<ActionResult<BaseResponse<CompanyUserOutputDTO>>> GetOneByIdAsync(long companyUserId)
    {
        ValidateId(companyUserId, nameof(companyUserId));
        return await ExecuteAsync(
            () => _companyUserService.GetOneByIdAsync(companyUserId), 
            "Usuário encontrado com sucesso"
        );
    }

    /// <summary>
    /// Adicionar usuário à empresa
    /// </summary>
    [HttpPost("create")]
    [RequirePermissions("user.canCreate")]
    public async Task<ActionResult<BaseResponse<CompanyUserOutputDTO>>> CreateAsync(CompanyUserInputDTO dto)
    {
        var companyId = GetCompanyId();
        var currentUserId = GetCurrentUserId();
        
        return await ValidateAndExecuteCreateAsync(
            () => _companyUserService.AddUserToCompanyAsync(dto, companyId, currentUserId),
            nameof(GetOneByIdAsync),
            result => new { companyUserId = result.CompanyUserId },
            "Usuário adicionado à empresa com sucesso"
        );
    }

    /// <summary>
    /// Atualizar cargo do usuário na empresa
    /// </summary>
    [HttpPut("{companyUserId}")]
    [RequirePermissions("user.canEdit")]
    public async Task<ActionResult<BaseResponse<CompanyUserOutputDTO>>> UpdateByIdAsync(
        long companyUserId, 
        CompanyUserInputDTO dto)
    {
        ValidateId(companyUserId, nameof(companyUserId));
        var currentUserId = GetCurrentUserId();
        
        return await ValidateAndExecuteAsync(
            () => _companyUserService.UpdateUserRoleAsync(companyUserId, dto, currentUserId), 
            "Cargo do usuário atualizado com sucesso"
        );
    }

    /// <summary>
    /// Remover usuário da empresa
    /// </summary>
    [HttpDelete("{companyUserId}")]
    [RequirePermissions("user.canDelete")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long companyUserId)
    {
        ValidateId(companyUserId, nameof(companyUserId));
        
        return await ExecuteBooleanAsync(
            () => _companyUserService.RemoveUserFromCompanyAsync(companyUserId),
            "Usuário removido da empresa com sucesso",
            "Usuário não encontrado ou não pôde ser removido"
        );
    }
}
