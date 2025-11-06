using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;

namespace ERP.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AccountOutputDTO>> GetAllAsync(long companyId)
        {
            var entities = await _unitOfWork.AccountRepository.GetAllAsync(companyId);
            return AccountMapper.ToAccountOutputDTOList(entities);
        }

        public async Task<PagedResult<AccountOutputDTO>> GetPagedAsync(long companyId, AccountFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.AccountRepository.GetPagedAsync(companyId, filters);
            var dtoItems = AccountMapper.ToAccountOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<AccountOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<AccountOutputDTO> GetOneByIdAsync(long AccountId)
        {
            var entity = await _unitOfWork.AccountRepository.GetOneByIdAsync(AccountId);
            if (entity == null)
            {
                throw new EntityNotFoundException("Account", AccountId);
            }
            return AccountMapper.ToAccountOutputDTO(entity);
        }

        public async Task<AccountOutputDTO> CreateAsync(AccountInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var entity = AccountMapper.ToEntity(dto, currentUserId);
            // ✅ Força CompanyId do contexto (segurança)
            entity.CompanyId = companyId;
            
            var createdEntity = await _unitOfWork.AccountRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return AccountMapper.ToAccountOutputDTO(createdEntity);
        }

        public async Task<AccountOutputDTO> UpdateByIdAsync(long AccountId, AccountInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // ✅ Busca entidade existente para preservar CriadoPor e CriadoEm
            var existingEntity = await _unitOfWork.AccountRepository.GetOneByIdAsync(AccountId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("Account", AccountId);
            }

            // ✅ Atualiza apenas campos de negócio + auditoria de atualização
            AccountMapper.UpdateEntity(existingEntity, dto, currentUserId);
            
            await _unitOfWork.SaveChangesAsync();
            return AccountMapper.ToAccountOutputDTO(existingEntity);
        }

        public async Task<bool> DeleteByIdAsync(long AccountId)
        {
            var result = await _unitOfWork.AccountRepository.DeleteByIdAsync(AccountId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
