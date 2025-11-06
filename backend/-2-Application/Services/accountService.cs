using CAU.Application.DTOs;
using CAU.Application.Interfaces;
using CAU.Application.Interfaces.Services;
using CAU.Application.Mappers;
using CAU.CrossCutting.Exceptions;

namespace CAU.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AccountOutputDTO>> GetAllAsync()
        {
            var entities = await _unitOfWork.AccountRepository.GetAllAsync();
            return AccountMapper.ToAccountOutputDTOList(entities);
        }

        public async Task<PagedResult<AccountOutputDTO>> GetPagedAsync(AccountFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.AccountRepository.GetPagedAsync(filters);
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

        public async Task<AccountOutputDTO> CreateAsync(AccountInputDTO dto)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // TODO: Substituir por ID do usuário autenticado via JWT
            long currentUserId = 1;

            var entity = AccountMapper.ToEntity(dto, currentUserId);
            var createdEntity = await _unitOfWork.AccountRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return AccountMapper.ToAccountOutputDTO(createdEntity);
        }

        public async Task<AccountOutputDTO> UpdateByIdAsync(long AccountId, AccountInputDTO dto)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // TODO: Substituir por ID do usuário autenticado via JWT
            long currentUserId = 1;

            var entity = AccountMapper.ToEntity(dto, currentUserId);
            var updatedEntity = await _unitOfWork.AccountRepository.UpdateByIdAsync(AccountId, entity);
            await _unitOfWork.SaveChangesAsync();
            return AccountMapper.ToAccountOutputDTO(updatedEntity);
        }

        public async Task<bool> DeleteByIdAsync(long AccountId)
        {
            var result = await _unitOfWork.AccountRepository.DeleteByIdAsync(AccountId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
