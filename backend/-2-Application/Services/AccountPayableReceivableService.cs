using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.Application.DTOs.Base;

namespace ERP.Application.Services
{
    public class AccountPayableReceivableService : IAccountPayableReceivableService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountPayableReceivableService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AccountPayableReceivableOutputDTO>> GetAllAsync(long companyId)
        {
            var entities = await _unitOfWork.AccountPayableReceivableRepository.GetAllAsync(companyId);
            return AccountPayableReceivableMapper.ToAccountPayableReceivableOutputDTOList(entities);
        }

        public async Task<PagedResult<AccountPayableReceivableOutputDTO>> GetPagedAsync(long companyId, AccountPayableReceivableFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.AccountPayableReceivableRepository.GetPagedAsync(companyId, filters);
            var dtoItems = AccountPayableReceivableMapper.ToAccountPayableReceivableOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<AccountPayableReceivableOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<AccountPayableReceivableOutputDTO> GetOneByIdAsync(long accountPayableReceivableId)
        {
            var entity = await _unitOfWork.AccountPayableReceivableRepository.GetOneByIdAsync(accountPayableReceivableId);
            if (entity == null)
            {
                throw new EntityNotFoundException("AccountPayableReceivable", accountPayableReceivableId);
            }
            return AccountPayableReceivableMapper.ToAccountPayableReceivableOutputDTO(entity);
        }

        public async Task<AccountPayableReceivableOutputDTO> CreateAsync(AccountPayableReceivableInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var entity = AccountPayableReceivableMapper.ToEntity(dto, companyId, currentUserId);
            
            var createdEntity = await _unitOfWork.AccountPayableReceivableRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return AccountPayableReceivableMapper.ToAccountPayableReceivableOutputDTO(createdEntity);
        }

        public async Task<AccountPayableReceivableOutputDTO> UpdateByIdAsync(long accountPayableReceivableId, AccountPayableReceivableInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var existingEntity = await _unitOfWork.AccountPayableReceivableRepository.GetOneByIdAsync(accountPayableReceivableId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("AccountPayableReceivable", accountPayableReceivableId);
            }

            AccountPayableReceivableMapper.UpdateEntity(existingEntity, dto, currentUserId);
            
            await _unitOfWork.SaveChangesAsync();
            return AccountPayableReceivableMapper.ToAccountPayableReceivableOutputDTO(existingEntity);
        }

        public async Task<bool> DeleteByIdAsync(long accountPayableReceivableId)
        {
            var result = await _unitOfWork.AccountPayableReceivableRepository.DeleteByIdAsync(accountPayableReceivableId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
