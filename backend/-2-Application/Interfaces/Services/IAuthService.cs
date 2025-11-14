using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO dto);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO dto);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDTO dto);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDTO dto);
        Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken);
    }
}
