using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs.Employee;
using ERP.Application.DTOs.Base;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.WebApi.Attributes;
using ERP.Configuration;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de empregados
/// </summary>
/// <remarks>
/// Este controller fornece endpoints RESTful para operações CRUD em empregados,
/// incluindo gerenciamento de dados pessoais, hierarquia e imagem de perfil.
/// </remarks>
[Authorize]
[ApiController]
[Route("api/employee")]
[Tags("Cadastros - Employee")]
[ApiExplorerSettings(GroupName = "Cadastros - Employee")]
public class EmployeeController : BaseController
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger) : base(logger)
    {
        _employeeService = employeeService;
    }

    [HttpGet("getAll")]
    [RequirePermissions("employee.canView")]
    public async Task<ActionResult<BaseResponse<List<EmployeeOutputDTO>>>> GetAllAsync()
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _employeeService.GetAllAsync(companyId), "Empregados listados com sucesso");
    }

    [HttpGet("getPaged")]
    [RequirePermissions("employee.canView")]
    public async Task<ActionResult<BaseResponse<PagedResult<EmployeeOutputDTO>>>> GetPagedAsync([FromQuery] EmployeeFilterDTO filters)
    {
        var companyId = GetCompanyId();
        return await ExecuteAsync(() => _employeeService.GetPagedAsync(companyId, filters), "Empregados listados com sucesso");
    }

    [HttpGet("{employeeId}")]
    [RequirePermissions("employee.canView")]
    public async Task<ActionResult<BaseResponse<EmployeeOutputDTO>>> GetOneByIdAsync(long employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));
        return await ExecuteAsync(() => _employeeService.GetOneByIdAsync(employeeId), "Empregado encontrado com sucesso");
    }

    [HttpPost("create")]
    [RequirePermissions("employee.canCreate")]
    public async Task<ActionResult<BaseResponse<EmployeeOutputDTO>>> CreateAsync(EmployeeInputDTO dto)
    {
        var companyId = GetCompanyId();
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteCreateAsync(
            () => _employeeService.CreateAsync(dto, companyId, currentUserId),
            nameof(GetOneByIdAsync),
            result => new { employeeId = result.EmployeeId },
            "Empregado criado com sucesso"
        );
    }

    [HttpPut("{employeeId}")]
    [RequirePermissions("employee.canEdit")]
    public async Task<ActionResult<BaseResponse<EmployeeOutputDTO>>> UpdateByIdAsync(long employeeId, EmployeeInputDTO dto)
    {
        ValidateId(employeeId, nameof(employeeId));
        var currentUserId = GetCurrentUserId();
        return await ValidateAndExecuteAsync(() => _employeeService.UpdateByIdAsync(employeeId, dto, currentUserId), "Empregado atualizado com sucesso");
    }

    [HttpDelete("{employeeId}")]
    [RequirePermissions("employee.canDelete")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));
        return await ExecuteBooleanAsync(
            () => _employeeService.DeleteByIdAsync(employeeId),
            "Empregado deletado com sucesso",
            "Empregado não encontrado ou não pôde ser deletado"
        );
    }
}
