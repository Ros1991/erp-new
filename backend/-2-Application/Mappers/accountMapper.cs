using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class AccountMapper
    {
        public static AccountOutputDTO ToAccountOutputDTO(Account entity)
        {
            if (entity == null) return null;

            return new AccountOutputDTO
            {
                AccountId = entity.AccountId,
                CompanyId = entity.CompanyId,
                Name = entity.Name,
                Type = entity.Type,
                InitialBalance = entity.InitialBalance,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<AccountOutputDTO> ToAccountOutputDTOList(List<Account> entities)
        {
            if (entities == null) return new List<AccountOutputDTO>();
            return entities.Select(ToAccountOutputDTO).ToList();
        }

        public static Account ToEntity(AccountInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new Account(
                dto.CompanyId,
                dto.Name,
                dto.Type,
                dto.InitialBalance,
                userId,        // CriadoPor
                userId,        // AtualizadoPor
                now,           // CriadoEm
                now            // AtualizadoEm
            );
        }
    }
}
