using ERP.Domain.Entities;
using ERP.Application.DTOs;//.Company;

namespace ERP.Application.Mappers
{
    public static class CompanyMapper
    {
        public static CompanyOutputDTO ToCompanyOutputDTO(Company entity)
        {
            if (entity == null) return null;

            return new CompanyOutputDTO
            {
    		    CompanyId = entity.CompanyId,
    		    Name = entity.Name,
    		    Document = entity.Document,
    		    UserId = entity.UserId,
    		    CriadoPor = entity.CriadoPor,
    		    AtualizadoPor = entity.AtualizadoPor,
    		    CriadoEm = entity.CriadoEm,
    		    AtualizadoEm = entity.AtualizadoEm
            };
        }

	    public static List<CompanyOutputDTO> ToCompanyOutputDTOList(IEnumerable<Company> entities)
        {
            return entities?.Select(ToCompanyOutputDTO).ToList() ?? new List<CompanyOutputDTO>();
        }


    	public static Company ToEntity(CompanyInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new Company(
             dto.Name,
             dto.Document,
             dto.UserId,
             userId,        // CriadoPor
             userId,        // AtualizadoPor
             now,           // CriadoEm
             now            // AtualizadoEm
            );
        }

        public static void UpdateEntity(Company entity, CompanyInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
			entity.Name = dto.Name;
			entity.Document = dto.Document;
			entity.UserId = dto.UserId;
			entity.AtualizadoPor = userId;
			entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
