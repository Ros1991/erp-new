using ERP.Application.DTOs;//.Company;
using ERP.Application.Interfaces.Services;
using ERP.Application.Interfaces;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.Domain.Entities;

namespace ERP.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CompanyOutputDTO>> GetAllAsync()
        {
            var entities = await _unitOfWork.CompanyRepository.GetAllAsync();
            return CompanyMapper.ToCompanyOutputDTOList(entities);
        }

        public async Task<PagedResult<CompanyOutputDTO>> GetPagedAsync(CompanyFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.CompanyRepository.GetPagedAsync(filters);
            var dtoItems = CompanyMapper.ToCompanyOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<CompanyOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<CompanyOutputDTO> GetOneByIdAsync(long CompanyId)
        {
            var entity = await _unitOfWork.CompanyRepository.GetOneByIdAsync(CompanyId);
            if (entity == null)
            {
                throw new EntityNotFoundException("Company", "EntityId");
            }
            return CompanyMapper.ToCompanyOutputDTO(entity);
        }

        public async Task<CompanyOutputDTO> CreateAsync(CompanyInputDTO dto)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // TODO: Substituir por ID do usuário autenticado via JWT
            long currentUserId = 1;

            var entity = CompanyMapper.ToEntity(dto, currentUserId);
            var createdEntity = await _unitOfWork.CompanyRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return CompanyMapper.ToCompanyOutputDTO(createdEntity);
        }

        public async Task<CompanyOutputDTO> UpdateByIdAsync(long CompanyId, CompanyInputDTO dto)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // TODO: Substituir por ID do usuário autenticado via JWT
            long currentUserId = 1;

            var entity = CompanyMapper.ToEntity(dto, currentUserId);
            var updatedEntity = await _unitOfWork.CompanyRepository.UpdateByIdAsync(CompanyId, entity);
            await _unitOfWork.SaveChangesAsync();
            return CompanyMapper.ToCompanyOutputDTO(updatedEntity);
        }

        public async Task<bool> DeleteByIdAsync(long CompanyId)
        {
            var result = await _unitOfWork.CompanyRepository.DeleteByIdAsync(CompanyId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
