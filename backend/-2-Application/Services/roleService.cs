using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;

namespace ERP.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RoleOutputDTO>> GetAllByCompanyAsync(long companyId)
        {
            var entities = await _unitOfWork.RoleRepository.GetAllAsync(companyId);
            return RoleMapper.ToRoleOutputDTOList(entities);
        }

        public async Task<RoleOutputDTO> GetOneByIdAsync(long roleId)
        {
            var entity = await _unitOfWork.RoleRepository.GetOneByIdAsync(roleId);
            if (entity == null)
            {
                throw new EntityNotFoundException("Role", roleId);
            }
            return RoleMapper.ToRoleOutputDTO(entity);
        }

        public async Task<RoleOutputDTO> CreateAsync(RoleInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var entity = RoleMapper.ToEntity(dto, companyId, currentUserId);
            var createdEntity = await _unitOfWork.RoleRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return RoleMapper.ToRoleOutputDTO(createdEntity);
        }

        public async Task<RoleOutputDTO> UpdateByIdAsync(long roleId, RoleInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var existingEntity = await _unitOfWork.RoleRepository.GetOneByIdAsync(roleId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("Role", roleId);
            }

            // Validar se é role do sistema (não pode ser editada)
            if (existingEntity.IsSystem)
            {
                throw new ValidationException("Role", "Roles do sistema (Owner/Admin) não podem ser editadas.");
            }

            RoleMapper.UpdateEntity(existingEntity, dto, currentUserId);
            
            await _unitOfWork.SaveChangesAsync();
            return RoleMapper.ToRoleOutputDTO(existingEntity);
        }

        public async Task<bool> DeleteByIdAsync(long roleId)
        {
            // Buscar role para validar antes de deletar
            var existingEntity = await _unitOfWork.RoleRepository.GetOneByIdAsync(roleId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("Role", roleId);
            }

            // Validar se é role do sistema (não pode ser deletada)
            if (existingEntity.IsSystem)
            {
                throw new ValidationException("Role", "Roles do sistema (Owner/Admin) não podem ser deletadas.");
            }

            var result = await _unitOfWork.RoleRepository.DeleteByIdAsync(roleId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
