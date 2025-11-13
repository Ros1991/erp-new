using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.Application.DTOs.Base;

namespace ERP.Application.Services
{
    public class FinancialTransactionService : IFinancialTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FinancialTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<FinancialTransactionOutputDTO>> GetAllAsync(long companyId)
        {
            var entities = await _unitOfWork.FinancialTransactionRepository.GetAllAsync(companyId);
            return FinancialTransactionMapper.ToFinancialTransactionOutputDTOList(entities);
        }

        public async Task<PagedResult<FinancialTransactionOutputDTO>> GetPagedAsync(long companyId, FinancialTransactionFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.FinancialTransactionRepository.GetPagedAsync(companyId, filters);
            var dtoItems = FinancialTransactionMapper.ToFinancialTransactionOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<FinancialTransactionOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<FinancialTransactionOutputDTO> GetOneByIdAsync(long financialTransactionId)
        {
            var entity = await _unitOfWork.FinancialTransactionRepository.GetOneByIdAsync(financialTransactionId);
            if (entity == null)
            {
                throw new EntityNotFoundException("FinancialTransaction", financialTransactionId);
            }
            return FinancialTransactionMapper.ToFinancialTransactionOutputDTO(entity);
        }

        public async Task<FinancialTransactionOutputDTO> CreateAsync(FinancialTransactionInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // Validar distribuições se houver
            if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Any())
            {
                var totalPercentage = dto.CostCenterDistributions.Sum(d => d.Percentage);
                if (Math.Abs(totalPercentage - 100) > 0.01m)
                {
                    throw new ValidationException("CostCenterDistributions", "A soma das porcentagens deve ser 100%");
                }
            }

            var entity = FinancialTransactionMapper.ToEntity(dto, companyId, currentUserId);
            
            var createdEntity = await _unitOfWork.FinancialTransactionRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Criar distribuições de centro de custo
            if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Any())
            {
                var now = DateTime.UtcNow;
                foreach (var distribution in dto.CostCenterDistributions)
                {
                    var tcc = new Domain.Entities.TransactionCostCenter(
                        createdEntity.FinancialTransactionId,
                        distribution.CostCenterId,
                        distribution.Amount ?? 0,
                        distribution.Percentage,
                        currentUserId,
                        null,
                        now,
                        null
                    );
                    createdEntity.TransactionCostCenterList.Add(tcc);
                }
                await _unitOfWork.SaveChangesAsync();

                // Recarregar a entidade com as distribuições
                createdEntity = await _unitOfWork.FinancialTransactionRepository.GetOneByIdAsync(createdEntity.FinancialTransactionId);
            }

            return FinancialTransactionMapper.ToFinancialTransactionOutputDTO(createdEntity);
        }

        public async Task<FinancialTransactionOutputDTO> UpdateByIdAsync(long financialTransactionId, FinancialTransactionInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // Validar distribuições se houver
            if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Any())
            {
                var totalPercentage = dto.CostCenterDistributions.Sum(d => d.Percentage);
                if (Math.Abs(totalPercentage - 100) > 0.01m)
                {
                    throw new ValidationException("CostCenterDistributions", "A soma das porcentagens deve ser 100%");
                }
            }

            var existingEntity = await _unitOfWork.FinancialTransactionRepository.GetOneByIdAsync(financialTransactionId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("FinancialTransaction", financialTransactionId);
            }

            FinancialTransactionMapper.UpdateEntity(existingEntity, dto, currentUserId);

            // Remover distribuições antigas
            if (existingEntity.TransactionCostCenterList != null && existingEntity.TransactionCostCenterList.Any())
            {
                existingEntity.TransactionCostCenterList.Clear();
            }

            // Criar novas distribuições
            if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Any())
            {
                var now = DateTime.UtcNow;
                foreach (var distribution in dto.CostCenterDistributions)
                {
                    var tcc = new Domain.Entities.TransactionCostCenter(
                        financialTransactionId,
                        distribution.CostCenterId,
                        distribution.Amount ?? 0,
                        distribution.Percentage,
                        currentUserId,
                        null,
                        now,
                        null
                    );
                    existingEntity.TransactionCostCenterList.Add(tcc);
                }
            }
            
            await _unitOfWork.SaveChangesAsync();
            return FinancialTransactionMapper.ToFinancialTransactionOutputDTO(existingEntity);
        }

        public async Task<bool> DeleteByIdAsync(long financialTransactionId)
        {
            var result = await _unitOfWork.FinancialTransactionRepository.DeleteByIdAsync(financialTransactionId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
