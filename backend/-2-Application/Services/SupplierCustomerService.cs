using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.Application.DTOs.Base;

namespace ERP.Application.Services
{
    public class SupplierCustomerService : ISupplierCustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupplierCustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SupplierCustomerOutputDTO>> GetAllAsync(long companyId)
        {
            var entities = await _unitOfWork.SupplierCustomerRepository.GetAllAsync(companyId);
            return SupplierCustomerMapper.ToSupplierCustomerOutputDTOList(entities);
        }

        public async Task<PagedResult<SupplierCustomerOutputDTO>> GetPagedAsync(long companyId, SupplierCustomerFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.SupplierCustomerRepository.GetPagedAsync(companyId, filters);
            var dtoItems = SupplierCustomerMapper.ToSupplierCustomerOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<SupplierCustomerOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<SupplierCustomerOutputDTO> GetOneByIdAsync(long supplierCustomerId)
        {
            var entity = await _unitOfWork.SupplierCustomerRepository.GetOneByIdAsync(supplierCustomerId);
            if (entity == null)
            {
                throw new EntityNotFoundException("SupplierCustomer", supplierCustomerId);
            }
            return SupplierCustomerMapper.ToSupplierCustomerOutputDTO(entity);
        }

        public async Task<SupplierCustomerOutputDTO> CreateAsync(SupplierCustomerInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var entity = SupplierCustomerMapper.ToEntity(dto, companyId, currentUserId);
            
            var createdEntity = await _unitOfWork.SupplierCustomerRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return SupplierCustomerMapper.ToSupplierCustomerOutputDTO(createdEntity);
        }

        public async Task<SupplierCustomerOutputDTO> UpdateByIdAsync(long supplierCustomerId, SupplierCustomerInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var existingEntity = await _unitOfWork.SupplierCustomerRepository.GetOneByIdAsync(supplierCustomerId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("SupplierCustomer", supplierCustomerId);
            }

            SupplierCustomerMapper.UpdateEntity(existingEntity, dto, currentUserId);
            
            await _unitOfWork.SaveChangesAsync();
            return SupplierCustomerMapper.ToSupplierCustomerOutputDTO(existingEntity);
        }

        public async Task<bool> DeleteByIdAsync(long supplierCustomerId)
        {
            var result = await _unitOfWork.SupplierCustomerRepository.DeleteByIdAsync(supplierCustomerId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
