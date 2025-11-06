using CAU.Application.DTOs;//.User;
using CAU.Application.Interfaces.Services;
using CAU.Application.Interfaces;
using CAU.Application.Mappers;
using CAU.CrossCutting.Exceptions;
using CAU.Domain.Entities;

namespace CAU.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<UserOutputDTO>> GetAllAsync()
        {
            var entities = await _unitOfWork.UserRepository.GetAllAsync();
            return UserMapper.ToUserOutputDTOList(entities);
        }

        public async Task<PagedResult<UserOutputDTO>> GetPagedAsync(UserFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.UserRepository.GetPagedAsync(filters);
            var dtoItems = UserMapper.ToUserOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<UserOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<UserOutputDTO> GetOneByIdAsync(long UserId)
        {
            var entity = await _unitOfWork.UserRepository.GetOneByIdAsync(UserId);
            if (entity == null)
            {
                throw new EntityNotFoundException("User", "EntityId");
            }
            return UserMapper.ToUserOutputDTO(entity);
        }

        public async Task<UserOutputDTO> CreateAsync(UserInputDTO dto)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            ValidateAtLeastOneContact(dto);

            var entity = UserMapper.ToEntity(dto);
            var createdEntity = await _unitOfWork.UserRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return UserMapper.ToUserOutputDTO(createdEntity);
        }
        
        public async Task<UserOutputDTO> UpdateByIdAsync(long UserId, UserInputDTO dto)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            ValidateAtLeastOneContact(dto);

            var entity = UserMapper.ToEntity(dto);
            var updatedEntity = await _unitOfWork.UserRepository.UpdateByIdAsync(UserId, entity);
            await _unitOfWork.SaveChangesAsync();
            return UserMapper.ToUserOutputDTO(updatedEntity);
        }

        private void ValidateAtLeastOneContact(UserInputDTO dto)
        {
            bool hasValidEmail = !string.IsNullOrWhiteSpace(dto.Email) && dto.Email.Length >= 5;
            bool hasValidPhone = !string.IsNullOrWhiteSpace(dto.Phone) && dto.Phone.Length >= 8;
            bool hasValidCpf = !string.IsNullOrWhiteSpace(dto.Cpf) && dto.Cpf.Length >= 11;

            if (!hasValidEmail && !hasValidPhone && !hasValidCpf)
            {
                throw new ValidationException(
                    "ContactInfo",
                    "Pelo menos um dos campos Email, Phone ou Cpf deve ser preenchido com valor válido."
                );
            }
        }
        
        public async Task<bool> DeleteByIdAsync(long UserId)
        {
            var result = await _unitOfWork.UserRepository.DeleteByIdAsync(UserId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
