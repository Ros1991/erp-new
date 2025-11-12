using ERP.Application.DTOs.Employee;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class EmployeeMapper
    {
        public static Employee ToEntity(EmployeeInputDTO dto, long companyId, long criadoPor)
        {
            if (dto == null) return null;
            
            // Converter Base64 para byte[] se houver imagem
            byte[]? profileImage = null;
            if (!string.IsNullOrEmpty(dto.ProfileImageBase64))
            {
                try
                {
                    profileImage = Convert.FromBase64String(dto.ProfileImageBase64);
                }
                catch
                {
                    // Se falhar a conversão, ignora a imagem
                    profileImage = null;
                }
            }
            
            return new Employee(
                Param_CompanyId: companyId,
                Param_UserId: dto.UserId,
                Param_EmployeeIdManager: dto.EmployeeIdManager,
                Param_Nickname: dto.Nickname,
                Param_FullName: dto.FullName,
                Param_Email: dto.Email,
                Param_Phone: dto.Phone,
                Param_Cpf: dto.Cpf,
                Param_ProfileImage: profileImage,
                Param_CriadoPor: criadoPor,
                Param_AtualizadoPor: null,
                Param_CriadoEm: DateTime.UtcNow,
                Param_AtualizadoEm: null
            );
        }
        
        public static void UpdateEntity(Employee entity, EmployeeInputDTO dto, long atualizadoPor)
        {
            if (entity == null || dto == null) return;
            
            entity.UserId = dto.UserId;
            entity.EmployeeIdManager = dto.EmployeeIdManager;
            entity.Nickname = dto.Nickname;
            entity.FullName = dto.FullName;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.Cpf = dto.Cpf;
            
            // Atualizar imagem apenas se uma nova foi enviada
            if (!string.IsNullOrEmpty(dto.ProfileImageBase64))
            {
                try
                {
                    entity.ProfileImage = Convert.FromBase64String(dto.ProfileImageBase64);
                }
                catch
                {
                    // Se falhar a conversão, mantém a imagem existente
                }
            }
            
            entity.AtualizadoPor = atualizadoPor;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
        
        public static EmployeeOutputDTO ToOutputDTO(Employee entity)
        {
            if (entity == null) return null;
            
            // Converter byte[] para Base64 se houver imagem
            string? profileImageBase64 = null;
            if (entity.ProfileImage != null && entity.ProfileImage.Length > 0)
            {
                profileImageBase64 = Convert.ToBase64String(entity.ProfileImage);
            }
            
            return new EmployeeOutputDTO
            {
                EmployeeId = entity.EmployeeId,
                CompanyId = entity.CompanyId,
                UserId = entity.UserId,
                EmployeeIdManager = entity.EmployeeIdManager,
                Nickname = entity.Nickname,
                FullName = entity.FullName,
                Email = entity.Email,
                Phone = entity.Phone,
                Cpf = entity.Cpf,
                ProfileImageBase64 = profileImageBase64,
                ManagerNickname = entity.EmployeeManager?.Nickname,
                ManagerFullName = entity.EmployeeManager?.FullName,
                UserEmail = entity.User?.Email,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }
        
        public static List<EmployeeOutputDTO> ToOutputDTOList(IEnumerable<Employee> entities)
        {
            if (entities == null) return new List<EmployeeOutputDTO>();
            return entities.Select(ToOutputDTO).ToList();
        }
    }
}
