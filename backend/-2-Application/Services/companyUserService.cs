using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;

namespace ERP.Application.Services
{
    public class CompanyUserService : ICompanyUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyUserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CompanyUserOutputDTO>> GetAllByCompanyAsync(long companyId)
        {
            var entities = await _unitOfWork.CompanyUserRepository.GetAllAsync(companyId);
            return CompanyUserMapper.ToCompanyUserOutputDTOList(entities);
        }

        public async Task<PagedResult<CompanyUserOutputDTO>> GetPagedAsync(long companyId, CompanyUserFilterDTO filters)
        {
            // ✅ Repository faz TUDO no banco (filtro, ordenação, paginação)
            var pagedEntities = await _unitOfWork.CompanyUserRepository.GetPagedAsync(companyId, filters);
            
            // ✅ Service apenas mapeia Entity → DTO
            var dtoItems = CompanyUserMapper.ToCompanyUserOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<CompanyUserOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<CompanyUserOutputDTO> GetOneByIdAsync(long companyUserId)
        {
            var entity = await _unitOfWork.CompanyUserRepository.GetOneByIdAsync(companyUserId);
            if (entity == null)
            {
                throw new EntityNotFoundException("CompanyUser", companyUserId);
            }
            return CompanyUserMapper.ToCompanyUserOutputDTO(entity);
        }

        public async Task<CompanyUserOutputDTO> AddUserToCompanyAsync(CompanyUserInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // Validar se usuário já existe na company
            var existing = await _unitOfWork.CompanyUserRepository.GetByUserAndCompanyAsync(dto.UserId, companyId);
            if (existing != null)
            {
                throw new ValidationException("UserId", "Usuário já está associado a esta empresa.");
            }

            // Validar se role pertence à company
            var role = await _unitOfWork.RoleRepository.GetOneByIdAsync(dto.RoleId);
            if (role == null || role.CompanyId != companyId)
            {
                throw new ValidationException("RoleId", "Role inválida ou não pertence a esta empresa.");
            }

            var entity = CompanyUserMapper.ToEntity(dto, companyId, currentUserId);
            var createdEntity = await _unitOfWork.CompanyUserRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            
            // Buscar novamente com includes
            var result = await _unitOfWork.CompanyUserRepository.GetOneByIdAsync(createdEntity.CompanyUserId);
            return CompanyUserMapper.ToCompanyUserOutputDTO(result);
        }

        public async Task<CompanyUserOutputDTO> UpdateUserRoleAsync(long companyUserId, CompanyUserInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var existingEntity = await _unitOfWork.CompanyUserRepository.GetOneByIdAsync(companyUserId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("CompanyUser", companyUserId);
            }

            // Validar se role pertence à company
            var role = await _unitOfWork.RoleRepository.GetOneByIdAsync(dto.RoleId);
            if (role == null || role.CompanyId != existingEntity.CompanyId)
            {
                throw new ValidationException("RoleId", "Role inválida ou não pertence a esta empresa.");
            }

            CompanyUserMapper.UpdateEntity(existingEntity, dto, currentUserId);
            
            await _unitOfWork.SaveChangesAsync();
            
            // Buscar novamente com includes
            var result = await _unitOfWork.CompanyUserRepository.GetOneByIdAsync(companyUserId);
            return CompanyUserMapper.ToCompanyUserOutputDTO(result);
        }

        public async Task<bool> RemoveUserFromCompanyAsync(long companyUserId)
        {
            var result = await _unitOfWork.CompanyUserRepository.DeleteByIdAsync(companyUserId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
