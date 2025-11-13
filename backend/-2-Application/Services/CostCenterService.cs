using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.Application.DTOs.Base;

namespace ERP.Application.Services
{
    public class CostCenterService : ICostCenterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CostCenterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CostCenterOutputDTO>> GetAllAsync(long companyId)
        {
            var entities = await _unitOfWork.CostCenterRepository.GetAllAsync(companyId);
            return CostCenterMapper.ToCostCenterOutputDTOList(entities);
        }

        public async Task<PagedResult<CostCenterOutputDTO>> GetPagedAsync(long companyId, CostCenterFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.CostCenterRepository.GetPagedAsync(companyId, filters);
            var dtoItems = CostCenterMapper.ToCostCenterOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<CostCenterOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<CostCenterOutputDTO> GetOneByIdAsync(long costCenterId)
        {
            var entity = await _unitOfWork.CostCenterRepository.GetOneByIdAsync(costCenterId);
            if (entity == null)
            {
                throw new EntityNotFoundException("CostCenter", costCenterId);
            }
            return CostCenterMapper.ToCostCenterOutputDTO(entity);
        }

        public async Task<CostCenterOutputDTO> CreateAsync(CostCenterInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var entity = CostCenterMapper.ToEntity(dto, companyId, currentUserId);
            
            var createdEntity = await _unitOfWork.CostCenterRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return CostCenterMapper.ToCostCenterOutputDTO(createdEntity);
        }

        public async Task<CostCenterOutputDTO> UpdateByIdAsync(long costCenterId, CostCenterInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var existingEntity = await _unitOfWork.CostCenterRepository.GetOneByIdAsync(costCenterId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("CostCenter", costCenterId);
            }

            CostCenterMapper.UpdateEntity(existingEntity, dto, currentUserId);
            
            await _unitOfWork.SaveChangesAsync();
            return CostCenterMapper.ToCostCenterOutputDTO(existingEntity);
        }

        public async Task<bool> DeleteByIdAsync(long costCenterId)
        {
            var result = await _unitOfWork.CostCenterRepository.DeleteByIdAsync(costCenterId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
