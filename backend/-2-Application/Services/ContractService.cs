using ERP.Application.DTOs;
using ERP.Application.Interfaces.Services;
using ERP.Application.Interfaces;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.Domain.Entities;

namespace ERP.Application.Services
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContractService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ContractOutputDTO>> GetAllByEmployeeIdAsync(long employeeId)
        {
            var contracts = await _unitOfWork.ContractRepository.GetAllByEmployeeIdAsync(employeeId);
            return ContractMapper.ToContractOutputDTOList(contracts);
        }

        public async Task<ContractOutputDTO> GetActiveByEmployeeIdAsync(long employeeId)
        {
            var contract = await _unitOfWork.ContractRepository.GetActiveByEmployeeIdAsync(employeeId);
            if (contract == null)
            {
                return null;
            }
            return ContractMapper.ToContractOutputDTO(contract);
        }

        public async Task<ContractOutputDTO> GetOneByIdAsync(long contractId)
        {
            var entity = await _unitOfWork.ContractRepository.GetOneByIdAsync(contractId);
            if (entity == null)
            {
                throw new EntityNotFoundException("Contract", contractId);
            }
            return ContractMapper.ToContractOutputDTO(entity);
        }

        public async Task<ContractOutputDTO> CreateAsync(ContractInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // Validar centros de custo se houver
            if (dto.CostCenters != null && dto.CostCenters.Any())
            {
                var totalPercentage = dto.CostCenters.Sum(c => c.Percentage);
                if (Math.Abs(totalPercentage - 100) > 0.01m)
                {
                    throw new ValidationException("CostCenters", "A soma das porcentagens dos centros de custo deve ser 100%");
                }
            }

            // Se for ativo, desativar outros contratos do mesmo funcionário
            if (dto.IsActive)
            {
                await _unitOfWork.ContractRepository.DeactivateOtherContractsAsync(dto.EmployeeId, 0);
            }

            var entity = ContractMapper.ToEntity(dto, currentUserId);
            
            var createdEntity = await _unitOfWork.ContractRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Criar benefícios/descontos
            if (dto.BenefitsDiscounts != null && dto.BenefitsDiscounts.Any())
            {
                var now = DateTime.UtcNow;
                foreach (var benefit in dto.BenefitsDiscounts)
                {
                    var cbd = new ContractBenefitDiscount(
                        createdEntity.ContractId,
                        benefit.Description,
                        benefit.Type,
                        benefit.Application,
                        benefit.Amount,
                        currentUserId,
                        null,
                        now,
                        null
                    )
                    {
                        Month = benefit.Month,
                        HasTaxes = benefit.HasTaxes,
                        IsProportional = benefit.IsProportional
                    };
                    createdEntity.ContractBenefitDiscountList.Add(cbd);
                }
            }

            // Criar centros de custo
            if (dto.CostCenters != null && dto.CostCenters.Any())
            {
                var now = DateTime.UtcNow;
                foreach (var costCenter in dto.CostCenters)
                {
                    var ccc = new ContractCostCenter(
                        createdEntity.ContractId,
                        costCenter.CostCenterId,
                        costCenter.Percentage,
                        currentUserId,
                        null,
                        now,
                        null
                    );
                    createdEntity.ContractCostCenterList.Add(ccc);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            // Recarregar a entidade com as relações
            createdEntity = await _unitOfWork.ContractRepository.GetOneByIdAsync(createdEntity.ContractId);
            return ContractMapper.ToContractOutputDTO(createdEntity);
        }

        public async Task<ContractOutputDTO> UpdateByIdAsync(long contractId, ContractInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // Validar centros de custo se houver
            if (dto.CostCenters != null && dto.CostCenters.Any())
            {
                var totalPercentage = dto.CostCenters.Sum(c => c.Percentage);
                if (Math.Abs(totalPercentage - 100) > 0.01m)
                {
                    throw new ValidationException("CostCenters", "A soma das porcentagens dos centros de custo deve ser 100%");
                }
            }

            var existingEntity = await _unitOfWork.ContractRepository.GetOneByIdAsync(contractId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("Contract", contractId);
            }

            // Se for ativo, desativar outros contratos do mesmo funcionário
            if (dto.IsActive && !existingEntity.IsActive)
            {
                await _unitOfWork.ContractRepository.DeactivateOtherContractsAsync(dto.EmployeeId, contractId);
            }

            ContractMapper.UpdateEntity(existingEntity, dto, currentUserId);

            // Remover benefícios/descontos antigos
            if (existingEntity.ContractBenefitDiscountList != null && existingEntity.ContractBenefitDiscountList.Any())
            {
                existingEntity.ContractBenefitDiscountList.Clear();
            }

            // Criar novos benefícios/descontos
            if (dto.BenefitsDiscounts != null && dto.BenefitsDiscounts.Any())
            {
                var now = DateTime.UtcNow;
                foreach (var benefit in dto.BenefitsDiscounts)
                {
                    var cbd = new ContractBenefitDiscount(
                        contractId,
                        benefit.Description,
                        benefit.Type,
                        benefit.Application,
                        benefit.Amount,
                        currentUserId,
                        null,
                        now,
                        null
                    )
                    {
                        Month = benefit.Month,
                        HasTaxes = benefit.HasTaxes,
                        IsProportional = benefit.IsProportional
                    };
                    existingEntity.ContractBenefitDiscountList.Add(cbd);
                }
            }

            // Remover centros de custo antigos
            if (existingEntity.ContractCostCenterList != null && existingEntity.ContractCostCenterList.Any())
            {
                existingEntity.ContractCostCenterList.Clear();
            }

            // Criar novos centros de custo
            if (dto.CostCenters != null && dto.CostCenters.Any())
            {
                var now = DateTime.UtcNow;
                foreach (var costCenter in dto.CostCenters)
                {
                    var ccc = new ContractCostCenter(
                        contractId,
                        costCenter.CostCenterId,
                        costCenter.Percentage,
                        currentUserId,
                        null,
                        now,
                        null
                    );
                    existingEntity.ContractCostCenterList.Add(ccc);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return ContractMapper.ToContractOutputDTO(existingEntity);
        }

        public async Task<bool> DeleteByIdAsync(long contractId)
        {
            var result = await _unitOfWork.ContractRepository.DeleteByIdAsync(contractId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
