using CAU.Domain.Entities;
using CAU.Application.DTOs;//.User;

namespace CAU.Application.Mappers
{
    public static class UserMapper
    {
        public static UserOutputDTO ToUserOutputDTO(User entity)
        {
            if (entity == null) return null;

            return new UserOutputDTO
            {
    		    UserId = entity.UserId,
    		    Email = entity.Email,
    		    Phone = entity.Phone,
    		    Cpf = entity.Cpf,
    		    PasswordHash = entity.PasswordHash,
    		    ResetToken = entity.ResetToken,
    		    ResetTokenExpiresAt = entity.ResetTokenExpiresAt
            };
        }

	    public static List<UserOutputDTO> ToUserOutputDTOList(IEnumerable<User> entities)
        {
            return entities?.Select(ToUserOutputDTO).ToList() ?? new List<UserOutputDTO>();
        }


    	public static User ToEntity(UserInputDTO dto)
        {
            if (dto == null) return null;
            return new User(
             dto.Email,
             dto.Phone,
             dto.Cpf,
             dto.PasswordHash,
             null,          // ResetToken - não vem do DTO
             null           // ResetTokenExpiresAt - não vem do DTO
            );
        }

        public static void UpdateEntity(User entity, UserInputDTO dto)
        {
            if (entity == null || dto == null) return;
			entity.Email = dto.Email;
			entity.Phone = dto.Phone;
			entity.Cpf = dto.Cpf;
			entity.PasswordHash = dto.PasswordHash;
			// ResetToken e ResetTokenExpiresAt não são atualizados pelo DTO comum
        }
    }
}
