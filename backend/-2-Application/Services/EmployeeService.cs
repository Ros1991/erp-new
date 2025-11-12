using ERP.Application.DTOs.Employee;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.Application.DTOs.Base;

namespace ERP.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
