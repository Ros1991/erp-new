using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.CrossCutting.Exceptions;
using ERP.CrossCutting.Services;
using ERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHashService _passwordHashService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHashService passwordHashService,
            ITokenService tokenService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _passwordHashService = passwordHashService;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO dto)
        {
            // Validar que pelo menos um campo de contato está preenchido
            if (string.IsNullOrWhiteSpace(dto.Email) && 
                string.IsNullOrWhiteSpace(dto.Phone) && 
                string.IsNullOrWhiteSpace(dto.Cpf))
            {
                throw new ValidationException(
                    "ContactInfo",
                    "Pelo menos um dos campos Email, Phone ou CPF deve ser preenchido.");
            }

            // Verificar se usuário já existe
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            var existingUser = users.FirstOrDefault(u =>
                (!string.IsNullOrWhiteSpace(dto.Email) && u.Email == dto.Email) ||
                (!string.IsNullOrWhiteSpace(dto.Phone) && u.Phone == dto.Phone) ||
                (!string.IsNullOrWhiteSpace(dto.Cpf) && u.Cpf == dto.Cpf));

            if (existingUser != null)
            {
                throw new ValidationException("User", "Usuário já existe com essas credenciais.");
            }

            // Hash da senha
            var passwordHash = _passwordHashService.HashPassword(dto.Password);

            // Criar usuário
            var user = new User(
                dto.Email,
                dto.Phone,
                dto.Cpf,
                passwordHash,
                null,
                null
            );

            await _unitOfWork.UserRepository.CreateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Gerar tokens
            var accessToken = _tokenService.GenerateAccessToken(user.UserId, user.Email, user.Phone, user.Cpf);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Salvar refresh token
            var userToken = new UserToken
            {
                UserId = user.UserId,
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2),
                RefreshExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _unitOfWork.UserTokenRepository.CreateAsync(userToken);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                Phone = user.Phone,
                Cpf = user.Cpf,
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2),
                RefreshExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO dto)
        {
            // Buscar usuário por credential (email, phone ou cpf)
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            var user = users.FirstOrDefault(u =>
                (u.Email != null && u.Email == dto.Credential) ||
                (u.Phone != null && u.Phone == dto.Credential) ||
                (u.Cpf != null && u.Cpf == dto.Credential));

            if (user == null)
            {
                throw new ValidationException("Credentials", "Credenciais inválidas.");
            }

            // Verificar senha
            if (!_passwordHashService.VerifyPassword(dto.Password, user.PasswordHash))
            {
                throw new ValidationException("Credentials", "Credenciais inválidas.");
            }

            // Gerar tokens
            var accessToken = _tokenService.GenerateAccessToken(user.UserId, user.Email, user.Phone, user.Cpf);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Salvar refresh token
            var userToken = new UserToken
            {
                UserId = user.UserId,
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2),
                RefreshExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _unitOfWork.UserTokenRepository.CreateAsync(userToken);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                Phone = user.Phone,
                Cpf = user.Cpf,
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2),
                RefreshExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var userTokens = await _unitOfWork.UserTokenRepository.GetAllAsync();
            var token = userTokens.FirstOrDefault(t => t.RefreshToken == refreshToken);

            if (token == null)
            {
                return false;
            }

            token.IsRevoked = true;
            await _unitOfWork.UserTokenRepository.UpdateByIdAsync(token.UserTokenId, token);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDTO dto)
        {
            // Buscar usuário
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            var user = users.FirstOrDefault(u =>
                (u.Email != null && u.Email == dto.Credential) ||
                (u.Phone != null && u.Phone == dto.Credential) ||
                (u.Cpf != null && u.Cpf == dto.Credential));

            if (user == null)
            {
                // Não revelar se usuário existe ou não (segurança)
                return true;
            }

            // Gerar token de reset
            var resetToken = Guid.NewGuid().ToString("N");
            user.ResetToken = resetToken;
            user.ResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);

            await _unitOfWork.UserRepository.UpdateByIdAsync(user.UserId, user);
            await _unitOfWork.SaveChangesAsync();

            // Enviar email (por enquanto só log)
            var emailToSend = user.Email ?? user.Phone ?? user.Cpf ?? "unknown";
            await _emailService.SendPasswordResetEmailAsync(emailToSend, resetToken);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDTO dto)
        {
            // Buscar usuário pelo token
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.ResetToken == dto.Token);

            if (user == null)
            {
                throw new ValidationException("Token", "Token inválido.");
            }

            // Verificar se token expirou
            if (!user.ResetTokenExpiresAt.HasValue || user.ResetTokenExpiresAt.Value < DateTime.UtcNow)
            {
                throw new ValidationException("Token", "Token expirado.");
            }

            // Hash da nova senha
            user.PasswordHash = _passwordHashService.HashPassword(dto.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiresAt = null;

            await _unitOfWork.UserRepository.UpdateByIdAsync(user.UserId, user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            var userTokens = await _unitOfWork.UserTokenRepository.GetAllAsync();
            var token = userTokens.FirstOrDefault(t => t.RefreshToken == refreshToken && !t.IsRevoked);

            if (token == null)
            {
                throw new ValidationException("Token", "Refresh token inválido.");
            }

            // Verificar se refresh token expirou
            if (token.RefreshExpiresAt.HasValue && token.RefreshExpiresAt.Value < DateTime.UtcNow)
            {
                throw new ValidationException("Token", "Refresh token expirado.");
            }

            // Buscar usuário
            var user = await _unitOfWork.UserRepository.GetOneByIdAsync(token.UserId);
            if (user == null)
            {
                throw new EntityNotFoundException("User", token.UserId);
            }

            // Revogar token antigo
            token.IsRevoked = true;
            await _unitOfWork.UserTokenRepository.UpdateByIdAsync(token.UserTokenId, token);

            // Gerar novos tokens
            var newAccessToken = _tokenService.GenerateAccessToken(user.UserId, user.Email, user.Phone, user.Cpf);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Salvar novo refresh token
            var newUserToken = new UserToken
            {
                UserId = user.UserId,
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2),
                RefreshExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _unitOfWork.UserTokenRepository.CreateAsync(newUserToken);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                Phone = user.Phone,
                Cpf = user.Cpf,
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(2),
                RefreshExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }
    }
}
