using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.Application.DTOs.Base;

namespace ERP.Application.Services
{
    public class LoanAdvanceService : ILoanAdvanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoanAdvanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<LoanAdvanceOutputDTO>> GetAllAsync(long companyId)
        {
            var entities = await _unitOfWork.LoanAdvanceRepository.GetAllAsync(companyId);
            return LoanAdvanceMapper.ToLoanAdvanceOutputDTOList(entities);
        }

        public async Task<PagedResult<LoanAdvanceOutputDTO>> GetPagedAsync(long companyId, LoanAdvanceFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.LoanAdvanceRepository.GetPagedAsync(companyId, filters);
            var dtoItems = LoanAdvanceMapper.ToLoanAdvanceOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<LoanAdvanceOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<LoanAdvanceOutputDTO> GetOneByIdAsync(long loanAdvanceId)
        {
            var entity = await _unitOfWork.LoanAdvanceRepository.GetOneByIdAsync(loanAdvanceId);
            if (entity == null)
            {
                throw new EntityNotFoundException("LoanAdvance", loanAdvanceId);
            }
            return LoanAdvanceMapper.ToLoanAdvanceOutputDTO(entity);
        }

        public async Task<LoanAdvanceOutputDTO> CreateAsync(LoanAdvanceInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var entity = LoanAdvanceMapper.ToEntity(dto, currentUserId);
            
            var createdEntity = await _unitOfWork.LoanAdvanceRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return LoanAdvanceMapper.ToLoanAdvanceOutputDTO(createdEntity);
        }

        public async Task<LoanAdvanceOutputDTO> UpdateByIdAsync(long loanAdvanceId, LoanAdvanceInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var existingEntity = await _unitOfWork.LoanAdvanceRepository.GetOneByIdAsync(loanAdvanceId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("LoanAdvance", loanAdvanceId);
            }

            LoanAdvanceMapper.UpdateEntity(existingEntity, dto, currentUserId);
            
            await _unitOfWork.SaveChangesAsync();
            return LoanAdvanceMapper.ToLoanAdvanceOutputDTO(existingEntity);
        }

        public async Task<bool> DeleteByIdAsync(long loanAdvanceId)
        {
            var result = await _unitOfWork.LoanAdvanceRepository.DeleteByIdAsync(loanAdvanceId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
