using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Application.DTOs.Auth;
using ERP.Application.DTOs.Base;
using ERP.Application.Interfaces.Services;
using ERP.WebApi.Controllers.Base;
using ERP.CrossCutting.Services;
using ERP.Domain.Models;

namespace ERP.WebApi.Controllers;

/// <summary>
/// Controller para autenticação e gerenciamento de usuários
/// </summary>
[ApiController]
[Route("api/auth")]
[Tags("Autenticação")]
[ApiExplorerSettings(GroupName = "Autenticação")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IPermissionService _permissionService;

    public AuthController(
        IAuthService authService, 
        IPermissionService permissionService,
        ILogger<AuthController> logger) : base(logger)
    {
        _authService = authService;
        _permissionService = permissionService;
    }

    /// <summary>
    /// Registrar novo usuário
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<AuthResponseDTO>>> RegisterAsync(RegisterRequestDTO dto)
    {
        return await ValidateAndExecuteAsync(
            () => _authService.RegisterAsync(dto),
            "Usuário registrado com sucesso"
        );
    }

    /// <summary>
    /// Login de usuário
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<AuthResponseDTO>>> LoginAsync(LoginRequestDTO dto)
    {
        return await ValidateAndExecuteAsync(
            () => _authService.LoginAsync(dto),
            "Login realizado com sucesso"
        );
    }

    /// <summary>
    /// Logout de usuário
    /// </summary>
    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<bool>>> LogoutAsync([FromBody] string refreshToken)
    {
        return await ExecuteBooleanAsync(
            () => _authService.LogoutAsync(refreshToken),
            "Logout realizado com sucesso",
            "Falha ao realizar logout"
        );
    }

    /// <summary>
    /// Solicitar reset de senha
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<bool>>> ForgotPasswordAsync(ForgotPasswordRequestDTO dto)
    {
        return await ExecuteBooleanAsync(
            () => _authService.ForgotPasswordAsync(dto),
            "Email de recuperação enviado com sucesso. Verifique seu email.",
            "Falha ao enviar email de recuperação"
        );
    }

    /// <summary>
    /// Resetar senha com token
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<bool>>> ResetPasswordAsync(ResetPasswordRequestDTO dto)
    {
        return await ExecuteBooleanAsync(
            () => _authService.ResetPasswordAsync(dto),
            "Senha alterada com sucesso",
            "Falha ao alterar senha"
        );
    }

    /// <summary>
    /// Renovar access token usando refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<BaseResponse<AuthResponseDTO>>> RefreshTokenAsync([FromBody] string refreshToken)
    {
        return await ValidateAndExecuteAsync(
            () => _authService.RefreshTokenAsync(refreshToken),
            "Token renovado com sucesso"
        );
    }

    /// <summary>
    /// Obter permissões do usuário na empresa atual
    /// </summary>
    [HttpGet("permissions")]
    [Authorize]
    public async Task<ActionResult<BaseResponse<RolePermissions>>> GetUserPermissionsAsync()
    {
        var userId = GetCurrentUserId();
        var companyId = GetCompanyId();
        
        var permissions = await _permissionService.GetUserPermissionsAsync(userId, companyId);
        
        return SuccessResponse(permissions, "Permissões obtidas com sucesso");
    }
}
