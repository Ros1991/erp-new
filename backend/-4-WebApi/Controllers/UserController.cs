using Microsoft.AspNetCore.Mvc;
using CAU.Application.DTOs;
using CAU.Application.DTOs.Base;
using CAU.Application.Interfaces.Services;
using CAU.WebApi.Controllers.Base;
using CAU.Configuration;

namespace CAU.WebApi.Controllers;

/// <summary>
/// Controller para gerenciamento de usuários do sistema
/// </summary>
/// <remarks>
/// Este controller fornece endpoints RESTful para operações CRUD em usuários,
/// incluindo autenticação e gerenciamento de perfis.
/// </remarks>
//[SwaggerGroupOrder(200)] // Cadastros - user: Primeiro grupo Cadastro (ordem 200)
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

    [HttpGet("/user/getAll/")]
    public async Task<ActionResult<BaseResponse<List<UserOutputDTO>>>> GetAllAsync()
    {
        return await ExecuteAsync(() => _UserService.GetAllAsync(), "Usuários listados com sucesso");
    }

    [HttpGet("/user/getPaged/")]
    public async Task<ActionResult<BaseResponse<PagedResult<UserOutputDTO>>>> GetPagedAsync([FromQuery] UserFilterDTO filters)
    {
        return await ExecuteAsync(() => _UserService.GetPagedAsync(filters), "Usuários listados com sucesso");
    }

    [HttpGet("/user/{userId}/getOneById/")]
    public async Task<ActionResult<BaseResponse<UserOutputDTO>>> GetOneByIdAsync(long UserId)
    {
        //ValidateId(UserId, nameof(UserId));
        return await ExecuteAsync(() => _UserService.GetOneByIdAsync(UserId), "Usuário encontrado com sucesso");
    }

    [HttpPost("/user/create/")]
    public async Task<ActionResult<BaseResponse<UserOutputDTO>>> CreateAsync(UserInputDTO dto)
    {
        return await ValidateAndExecuteCreateAsync(
            () => _UserService.CreateAsync(dto),
            nameof(GetOneByIdAsync),
            result => new { user_id = result.UserId },
            "Usuário criado com sucesso"
        );
    }
    
    [HttpPut("/user/{userId}/updateById/")]
    public async Task<ActionResult<BaseResponse<UserOutputDTO>>> UpdateByIdAsync(long UserId, UserInputDTO dto)
    {
        //ValidateId(UserId, nameof(UserId));
        return await ValidateAndExecuteAsync(() => _UserService.UpdateByIdAsync(UserId, dto), "Usuário atualizado com sucesso");
    }
    
    [HttpDelete("/user/{userId}/deleteById/")]
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
