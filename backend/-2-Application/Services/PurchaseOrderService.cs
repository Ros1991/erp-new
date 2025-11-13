using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.Application.DTOs.Base;

namespace ERP.Application.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PurchaseOrderOutputDTO>> GetAllAsync(long companyId)
        {
            var entities = await _unitOfWork.PurchaseOrderRepository.GetAllAsync(companyId);
            return PurchaseOrderMapper.ToPurchaseOrderOutputDTOList(entities);
        }

        public async Task<PagedResult<PurchaseOrderOutputDTO>> GetPagedAsync(long companyId, PurchaseOrderFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.PurchaseOrderRepository.GetPagedAsync(companyId, filters);
            var dtoItems = PurchaseOrderMapper.ToPurchaseOrderOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<PurchaseOrderOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<PurchaseOrderOutputDTO> GetOneByIdAsync(long purchaseOrderId)
        {
            var entity = await _unitOfWork.PurchaseOrderRepository.GetOneByIdAsync(purchaseOrderId);
            if (entity == null)
            {
                throw new EntityNotFoundException("PurchaseOrder", purchaseOrderId);
            }
            return PurchaseOrderMapper.ToPurchaseOrderOutputDTO(entity);
        }

        public async Task<PurchaseOrderOutputDTO> CreateAsync(PurchaseOrderInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var entity = PurchaseOrderMapper.ToEntity(dto, companyId, currentUserId);
            
            var createdEntity = await _unitOfWork.PurchaseOrderRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return PurchaseOrderMapper.ToPurchaseOrderOutputDTO(createdEntity);
        }

        public async Task<PurchaseOrderOutputDTO> UpdateByIdAsync(long purchaseOrderId, PurchaseOrderInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var existingEntity = await _unitOfWork.PurchaseOrderRepository.GetOneByIdAsync(purchaseOrderId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("PurchaseOrder", purchaseOrderId);
            }

            PurchaseOrderMapper.UpdateEntity(existingEntity, dto, currentUserId);
            
            await _unitOfWork.SaveChangesAsync();
            return PurchaseOrderMapper.ToPurchaseOrderOutputDTO(existingEntity);
        }

        public async Task<bool> DeleteByIdAsync(long purchaseOrderId)
        {
            var result = await _unitOfWork.PurchaseOrderRepository.DeleteByIdAsync(purchaseOrderId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
