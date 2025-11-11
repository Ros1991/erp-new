using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.Configuration;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de cargos/roles
/// </summary>
/// <remarks>
/// Este controller fornece endpoints RESTful para operações CRUD em cargos,
/// incluindo gerenciamento de permissões e roles da empresa.
/// </remarks>
[Authorize]
[ApiController]
[Route("api/role")]
[Tags("Cadastros - Role")]
[ApiExplorerSettings(GroupName = "Cadastros - Role")]
public class RoleController : BaseController
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService, ILogger<RoleController> logger) : base(logger)
    {
        _roleService = roleService;
    }

    [HttpGet("getAll")]
    public async Task<ActionResult<BaseResponse<List<RoleOutputDTO>>>> GetAllAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _roleService.GetAllAsync(companyId), "Cargos listados com sucesso");
    }

    [HttpGet("getPaged")]
    public async Task<ActionResult<BaseResponse<PagedResult<RoleOutputDTO>>>> GetPagedAsync([FromQuery] RoleFilterDTO filters)
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _roleService.GetPagedAsync(companyId, filters), "Cargos listados com sucesso");
    }

    [HttpGet("{roleId}")]
    public async Task<ActionResult<BaseResponse<RoleOutputDTO>>> GetOneByIdAsync(long roleId)
    {
        ValidateId(roleId, nameof(roleId));
        return await ExecuteAsync(() => _roleService.GetOneByIdAsync(roleId), "Cargo encontrado com sucesso");
    }

    [HttpPost("create")]
    public async Task<ActionResult<BaseResponse<RoleOutputDTO>>> CreateAsync(RoleInputDTO dto)
    {
        var companyId = GetCompanyId();
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteCreateAsync(
            () => _roleService.CreateAsync(dto, companyId, currentUserId),
            nameof(GetOneByIdAsync),
            result => new { roleId = result.RoleId },
            "Cargo criado com sucesso"
        );
    }

    [HttpPut("{roleId}")]
    public async Task<ActionResult<BaseResponse<RoleOutputDTO>>> UpdateByIdAsync(long roleId, RoleInputDTO dto)
    {
        ValidateId(roleId, nameof(roleId));
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(() => _roleService.UpdateByIdAsync(roleId, dto, currentUserId), "Cargo atualizado com sucesso");
    }

    [HttpDelete("{roleId}")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long roleId)
    {
        ValidateId(roleId, nameof(roleId));
        return await ExecuteBooleanAsync(
            () => _roleService.DeleteByIdAsync(roleId),
            "Cargo deletado com sucesso",
            "Cargo não encontrado ou não pôde ser deletado"
        );
    }
}
