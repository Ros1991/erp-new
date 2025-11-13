using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class SupplierCustomerMapper
    {
        public static SupplierCustomerOutputDTO ToSupplierCustomerOutputDTO(SupplierCustomer entity)
        {
            if (entity == null) return null;

            return new SupplierCustomerOutputDTO
            {
                SupplierCustomerId = entity.SupplierCustomerId,
                CompanyId = entity.CompanyId,
                Name = entity.Name,
                Document = entity.Document,
                Email = entity.Email,
                Phone = entity.Phone,
                Address = entity.Address,
                IsActive = entity.IsActive,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<SupplierCustomerOutputDTO> ToSupplierCustomerOutputDTOList(List<SupplierCustomer> entities)
        {
            if (entities == null) return new List<SupplierCustomerOutputDTO>();
            return entities.Select(ToSupplierCustomerOutputDTO).ToList();
        }

        public static SupplierCustomer ToEntity(SupplierCustomerInputDTO dto, long companyId, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new SupplierCustomer(
                companyId,
                dto.Name,
                dto.Document,
                dto.Email,
                dto.Phone,
                dto.Address,
                dto.IsActive,
                userId,        // CriadoPor
                null,          // AtualizadoPor
                now,           // CriadoEm
                null           // AtualizadoEm
            );
        }

        public static void UpdateEntity(SupplierCustomer entity, SupplierCustomerInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.Name = dto.Name;
            entity.Document = dto.Document;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.Address = dto.Address;
            entity.IsActive = dto.IsActive;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
            // ✅ CriadoPor e CriadoEm NÃO são alterados
        }
    }
}
