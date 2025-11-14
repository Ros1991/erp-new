using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.WebApi.Attributes;
using ERP.Configuration;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de usuários do sistema
/// </summary>
/// <remarks>
/// Este controller fornece endpoints RESTful para operações CRUD em usuários,
/// incluindo autenticação e gerenciamento de perfis.
/// </remarks>
//[SwaggerGroupOrder(200)] // Cadastros - user: Primeiro grupo Cadastro (ordem 200)
[Authorize] // ✅ Requer autenticação
[ApiController]
[Route("api/user")]
[Tags("Cadastros - user")]
[ApiExplorerSettings(GroupName = "Cadastros - user")]
public class UserController : BaseController
{
    private readonly IUserService _UserService;

    public UserController(IUserService UserService, ILogger<UserController> logger) : base(logger)
    {
        _UserService = UserService;
    }

    [HttpGet("getAll")]
    [RequirePermissions("user.canView")]
    public async Task<ActionResult<BaseResponse<List<UserOutputDTO>>>> GetAllAsync()
    {
        return await ExecuteAsync(() => _UserService.GetAllAsync(), "Usuários listados com sucesso");
    }

    [HttpGet("getPaged")]
    [RequirePermissions("user.canView")]
    public async Task<ActionResult<BaseResponse<PagedResult<UserOutputDTO>>>> GetPagedAsync([FromQuery] UserFilterDTO filters)
    {
        return await ExecuteAsync(() => _UserService.GetPagedAsync(filters), "Usuários listados com sucesso");
    }

    [HttpGet("{userId}")]
    [RequirePermissions("user.canView")]
    public async Task<ActionResult<BaseResponse<UserOutputDTO>>> GetOneByIdAsync(long UserId)
    {
        //ValidateId(UserId, nameof(UserId));
        return await ExecuteAsync(() => _UserService.GetOneByIdAsync(UserId), "Usuário encontrado com sucesso");
    }

    [HttpPost("create")]
    [RequirePermissions("user.canCreate")]
    public async Task<ActionResult<BaseResponse<UserOutputDTO>>> CreateAsync(UserInputDTO dto)
    {
        return await ValidateAndExecuteCreateAsync(
            () => _UserService.CreateAsync(dto),
            nameof(GetOneByIdAsync),
            result => new { UserId = result.UserId },
            "Usuário criado com sucesso"
        );
    }
    
    [HttpPut("{userId}")]
    [RequirePermissions("user.canEdit")]
    public async Task<ActionResult<BaseResponse<UserOutputDTO>>> UpdateByIdAsync(long UserId, UserInputDTO dto)
    {
        //ValidateId(UserId, nameof(UserId));
        return await ValidateAndExecuteAsync(() => _UserService.UpdateByIdAsync(UserId, dto), "Usuário atualizado com sucesso");
    }
    
    [HttpDelete("{userId}")]
    [RequirePermissions("user.canDelete")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteByIdAsync(long UserId)
    {
        //ValidateId(UserId, nameof(UserId));
        return await ExecuteBooleanAsync(
            () => _UserService.DeleteByIdAsync(UserId),
            "Usuário deletado com sucesso",
            "Usuário não encontrado ou não pôde ser deletado"
        );
    }
}
