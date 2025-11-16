using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.CrossCutting.Services;

namespace ERP.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHashService _passwordHashService;

        public EmployeeService(IUnitOfWork unitOfWork, IPasswordHashService passwordHashService)
        {
            _unitOfWork = unitOfWork;
            _passwordHashService = passwordHashService;
        }

        public async Task<List<EmployeeOutputDTO>> GetAllAsync(long companyId)
        {
            var entities = await _unitOfWork.EmployeeRepository.GetAllAsync(companyId);
            return EmployeeMapper.ToOutputDTOList(entities);
        }

        public async Task<PagedResult<EmployeeOutputDTO>> GetPagedAsync(long companyId, EmployeeFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.EmployeeRepository.GetPagedAsync(companyId, filters);
            var dtoItems = EmployeeMapper.ToOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<EmployeeOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<EmployeeOutputDTO> GetOneByIdAsync(long employeeId)
        {
            var entity = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(employeeId);
            if (entity == null)
            {
                throw new EntityNotFoundException("Employee", employeeId);
            }
            return EmployeeMapper.ToOutputDTO(entity);
        }

        public async Task<EmployeeOutputDTO> CreateAsync(EmployeeInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // Validar manager se fornecido
            if (dto.EmployeeIdManager.HasValue)
            {
                var manager = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(dto.EmployeeIdManager.Value);
                if (manager == null || manager.CompanyId != companyId)
                {
                    throw new ValidationException("EmployeeIdManager", "Gerente inválido ou não pertence à empresa.");
                }
            }

            // Validar usuário se fornecido
            if (dto.UserId.HasValue)
            {
                var user = await _unitOfWork.UserRepository.GetOneByIdAsync(dto.UserId.Value);
                if (user == null)
                {
                    throw new ValidationException("UserId", "Usuário inválido.");
                }
            }

            var entity = EmployeeMapper.ToEntity(dto, companyId, currentUserId);
            
            var createdEntity = await _unitOfWork.EmployeeRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            
            // Recarregar com includes
            createdEntity = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(createdEntity.EmployeeId);
            return EmployeeMapper.ToOutputDTO(createdEntity);
        }

        public async Task<EmployeeOutputDTO> UpdateByIdAsync(long employeeId, EmployeeInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // Busca entidade existente
            var existingEntity = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(employeeId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("Employee", employeeId);
            }

            // Validar manager se fornecido
            if (dto.EmployeeIdManager.HasValue)
            {
                var manager = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(dto.EmployeeIdManager.Value);
                if (manager == null || manager.CompanyId != existingEntity.CompanyId)
                {
                    throw new ValidationException("EmployeeIdManager", "Gerente inválido ou não pertence à empresa.");
                }
                
                // Evitar ciclo de referência
                if (dto.EmployeeIdManager.Value == employeeId)
                {
                    throw new ValidationException("EmployeeIdManager", "Empregado não pode ser gerente de si mesmo.");
                }
            }

            // Validar usuário se fornecido
            if (dto.UserId.HasValue)
            {
                var user = await _unitOfWork.UserRepository.GetOneByIdAsync(dto.UserId.Value);
                if (user == null)
                {
                    throw new ValidationException("UserId", "Usuário inválido.");
                }
            }

            // Atualiza apenas campos de negócio + auditoria
            EmployeeMapper.UpdateEntity(existingEntity, dto, currentUserId);
            
            await _unitOfWork.SaveChangesAsync();
            
            // Recarregar com includes
            existingEntity = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(employeeId);
            return EmployeeMapper.ToOutputDTO(existingEntity);
        }

        public async Task<bool> DeleteByIdAsync(long employeeId)
        {
            // Verificar se há empregados subordinados
            var subordinates = await _unitOfWork.EmployeeRepository.GetAllAsync(0);
            if (subordinates.Any(e => e.EmployeeIdManager == employeeId))
            {
                throw new ValidationException("Employee", "Não é possível excluir empregado que é gerente de outros.");
            }

            var result = await _unitOfWork.EmployeeRepository.DeleteByIdAsync(employeeId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        // ==================== MÉTODOS DE ASSOCIAÇÃO DE USUÁRIO ====================

        public async Task<UserSearchResultDTO> SearchUserForEmployeeAsync(long employeeId, long companyId)
        {
            var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(employeeId);
            if (employee == null)
            {
                throw new EntityNotFoundException("Employee", employeeId);
            }

            // Buscar usuário por email, phone ou cpf
            Domain.Entities.User? foundUser = null;
            
            if (!string.IsNullOrWhiteSpace(employee.Email))
            {
                foundUser = await _unitOfWork.UserRepository.GetByEmailAsync(employee.Email);
            }
            
            if (foundUser == null && !string.IsNullOrWhiteSpace(employee.Phone))
            {
                foundUser = await _unitOfWork.UserRepository.GetByPhoneAsync(employee.Phone);
            }
            
            if (foundUser == null && !string.IsNullOrWhiteSpace(employee.Cpf))
            {
                foundUser = await _unitOfWork.UserRepository.GetByCpfAsync(employee.Cpf);
            }

            // Se não encontrou usuário, retornar result vazio
            if (foundUser == null)
            {
                return new UserSearchResultDTO
                {
                    UserId = null,
                    HasCompanyAccess = false
                };
            }

            // Verificar se o usuário tem acesso à empresa
            var companyUser = await _unitOfWork.CompanyUserRepository.GetByUserAndCompanyAsync(foundUser.UserId, companyId);

            return new UserSearchResultDTO
            {
                UserId = foundUser.UserId,
                Email = foundUser.Email,
                Phone = foundUser.Phone,
                Cpf = foundUser.Cpf,
                HasCompanyAccess = companyUser != null,
                CurrentRoleId = companyUser?.RoleId,
                CurrentRoleName = companyUser?.Role?.Name,
                CurrentRoleIsSystem = companyUser?.Role?.IsSystem
            };
        }

        public async Task<EmployeeOutputDTO> AssociateUserAsync(
            long employeeId, 
            AssociateUserDTO dto, 
            long companyId, 
            long currentUserId)
        {
            var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(employeeId);
            if (employee == null)
            {
                throw new EntityNotFoundException("Employee", employeeId);
            }

            // Verificar se o usuário existe
            var user = await _unitOfWork.UserRepository.GetOneByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new EntityNotFoundException("User", dto.UserId);
            }

            // Se precisa criar CompanyUser
            if (dto.CreateCompanyUser)
            {
                if (dto.RoleId == null)
                {
                    throw new ValidationException("RoleId", "Cargo é obrigatório ao criar acesso à empresa");
                }

                // Verificar se já existe CompanyUser
                var existingCompanyUser = await _unitOfWork.CompanyUserRepository.GetByUserAndCompanyAsync(dto.UserId, companyId);
                if (existingCompanyUser != null)
                {
                    throw new ValidationException("UserId", "Usuário já possui acesso a esta empresa");
                }

                var companyUser = new Domain.Entities.CompanyUser(
                    companyId,
                    dto.UserId,
                    dto.RoleId,
                    currentUserId,
                    null,
                    DateTime.UtcNow,
                    null
                );
                await _unitOfWork.CompanyUserRepository.CreateAsync(companyUser);
            }

            // Associar ao employee
            employee.UserId = dto.UserId;
            employee.AtualizadoPor = currentUserId;
            employee.AtualizadoEm = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return await GetOneByIdAsync(employeeId);
        }

        public async Task<EmployeeOutputDTO> CreateAndAssociateUserAsync(
            long employeeId,
            CreateUserAndAssociateDTO dto,
            long companyId,
            long currentUserId)
        {
            var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(employeeId);
            if (employee == null)
            {
                throw new EntityNotFoundException("Employee", employeeId);
            }

            // Validar que pelo menos um identificador foi fornecido
            if (string.IsNullOrWhiteSpace(dto.Email) && 
                string.IsNullOrWhiteSpace(dto.Phone) && 
                string.IsNullOrWhiteSpace(dto.Cpf))
            {
                throw new ValidationException("User", "Pelo menos um identificador (Email, Telefone ou CPF) é obrigatório");
            }

            // Verificar se já existe usuário com os mesmos dados
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var existingByEmail = await _unitOfWork.UserRepository.GetByEmailAsync(dto.Email);
                if (existingByEmail != null)
                {
                    throw new ValidationException("Email", "Já existe um usuário com este email");
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                var existingByPhone = await _unitOfWork.UserRepository.GetByPhoneAsync(dto.Phone);
                if (existingByPhone != null)
                {
                    throw new ValidationException("Phone", "Já existe um usuário com este telefone");
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.Cpf))
            {
                var existingByCpf = await _unitOfWork.UserRepository.GetByCpfAsync(dto.Cpf);
                if (existingByCpf != null)
                {
                    throw new ValidationException("Cpf", "Já existe um usuário com este CPF");
                }
            }

            // Criar User
            var passwordHash = _passwordHashService.HashPassword(dto.Password);
            var user = new Domain.Entities.User(
                dto.Email,
                dto.Phone,
                dto.Cpf,
                passwordHash,
                null,
                null
            );
            var createdUser = await _unitOfWork.UserRepository.CreateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Criar CompanyUser
            var companyUser = new Domain.Entities.CompanyUser(
                companyId,
                createdUser.UserId,
                dto.RoleId,
                currentUserId,
                null,
                DateTime.UtcNow,
                null
            );
            await _unitOfWork.CompanyUserRepository.CreateAsync(companyUser);

            // Associar ao employee
            employee.UserId = createdUser.UserId;
            employee.AtualizadoPor = currentUserId;
            employee.AtualizadoEm = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return await GetOneByIdAsync(employeeId);
        }

        public async Task<EmployeeOutputDTO> DisassociateUserAsync(
            long employeeId,
            DisassociateUserDTO dto,
            long companyId,
            long currentUserId)
        {
            var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(employeeId);
            if (employee == null)
            {
                throw new EntityNotFoundException("Employee", employeeId);
            }

            if (employee.UserId == null)
            {
                throw new ValidationException("UserId", "Empregado não possui usuário associado");
            }

            var userId = employee.UserId.Value;

            // Remover acesso à empresa se solicitado
            if (dto.RemoveCompanyAccess)
            {
                var companyUser = await _unitOfWork.CompanyUserRepository.GetByUserAndCompanyAsync(userId, companyId);
                if (companyUser != null)
                {
                    await _unitOfWork.CompanyUserRepository.DeleteByIdAsync(companyUser.CompanyUserId);
                }
            }

            // Desassociar do employee
            employee.UserId = null;
            employee.AtualizadoPor = currentUserId;
            employee.AtualizadoEm = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return await GetOneByIdAsync(employeeId);
        }
    }
}
