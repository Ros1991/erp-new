using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.Domain.Entities;

namespace ERP.Application.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseOrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PurchaseOrderOutputDTO>> GetAllAsync(long companyId)
        {
            var entities = await _unitOfWork.PurchaseOrderRepository.GetAllAsync(companyId);
            return PurchaseOrderMapper.ToPurchaseOrderOutputDTOList(entities);
        }

        public async Task<PagedResult<PurchaseOrderOutputDTO>> GetPagedAsync(long companyId, PurchaseOrderFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.PurchaseOrderRepository.GetPagedAsync(companyId, filters);
            var dtoItems = PurchaseOrderMapper.ToPurchaseOrderOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<PurchaseOrderOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<PurchaseOrderOutputDTO> GetOneByIdAsync(long purchaseOrderId)
        {
            var entity = await _unitOfWork.PurchaseOrderRepository.GetOneByIdAsync(purchaseOrderId);
            if (entity == null)
            {
                throw new EntityNotFoundException("PurchaseOrder", purchaseOrderId);
            }
            return PurchaseOrderMapper.ToPurchaseOrderOutputDTO(entity);
        }

        public async Task<PurchaseOrderOutputDTO> CreateAsync(PurchaseOrderInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var entity = PurchaseOrderMapper.ToEntity(dto, companyId, currentUserId);
            
            var createdEntity = await _unitOfWork.PurchaseOrderRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return PurchaseOrderMapper.ToPurchaseOrderOutputDTO(createdEntity);
        }

        public async Task<PurchaseOrderOutputDTO> UpdateByIdAsync(long purchaseOrderId, PurchaseOrderInputDTO dto, long currentUserId, bool hasProcessPermission)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var existingEntity = await _unitOfWork.PurchaseOrderRepository.GetOneByIdAsync(purchaseOrderId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("PurchaseOrder", purchaseOrderId);
            }

            // Validar permissão de edição
            ValidateEditPermission(existingEntity, currentUserId, hasProcessPermission);

            PurchaseOrderMapper.UpdateEntity(existingEntity, dto, currentUserId);
            
            // Se aprovada, atualizar também a transação financeira
            if (existingEntity.Status == "Aprovado")
            {
                var transaction = await _unitOfWork.FinancialTransactionRepository.GetByPurchaseOrderIdAsync(purchaseOrderId);
                if (transaction != null)
                {
                    transaction.Description = $"Ordem de Compra: {dto.Description}";
                    transaction.AtualizadoPor = currentUserId;
                    transaction.AtualizadoEm = DateTime.UtcNow;
                }
            }
            
            await _unitOfWork.SaveChangesAsync();
            return PurchaseOrderMapper.ToPurchaseOrderOutputDTO(existingEntity);
        }

        public async Task<PurchaseOrderOutputDTO> ProcessAsync(long purchaseOrderId, PurchaseOrderProcessDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var existingEntity = await _unitOfWork.PurchaseOrderRepository.GetOneByIdAsync(purchaseOrderId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("PurchaseOrder", purchaseOrderId);
            }

            if (existingEntity.Status != "Pendente")
            {
                throw new BusinessRuleException("Process", "Apenas ordens pendentes podem ser processadas.");
            }

            var validStatuses = new[] { "Aprovado", "Rejeitado" };
            if (!validStatuses.Contains(dto.Status))
            {
                throw new ValidationException("Status", "Status deve ser 'Aprovado' ou 'Rejeitado'.");
            }

            var now = DateTime.UtcNow;
            var processedAt = DateTime.SpecifyKind(dto.ProcessedAt, DateTimeKind.Utc);

            existingEntity.Status = dto.Status;
            existingEntity.ProcessedMessage = dto.Message;
            existingEntity.ProcessedAt = processedAt;
            existingEntity.UserIdApprover = currentUserId;
            existingEntity.AtualizadoPor = currentUserId;
            existingEntity.AtualizadoEm = now;

            if (dto.Status == "Aprovado")
            {
                // Validar campos obrigatórios para aprovação
                if (!dto.TotalAmount.HasValue || dto.TotalAmount <= 0)
                {
                    throw new ValidationException("TotalAmount", "Valor total é obrigatório para aprovação.");
                }
                if (!dto.AccountId.HasValue)
                {
                    throw new ValidationException("AccountId", "Conta corrente é obrigatória para aprovação.");
                }

                // Descrição da transação: usa a informada ou a da ordem
                var transactionDescription = !string.IsNullOrWhiteSpace(dto.TransactionDescription) 
                    ? dto.TransactionDescription 
                    : $"Ordem de Compra: {existingEntity.Description}";

                // Se a descrição da transação foi alterada, atualiza a descrição da ordem também
                if (!string.IsNullOrWhiteSpace(dto.TransactionDescription) && 
                    dto.TransactionDescription != existingEntity.Description &&
                    !dto.TransactionDescription.StartsWith("Ordem de Compra:"))
                {
                    existingEntity.Description = dto.TransactionDescription;
                }

                existingEntity.TotalAmount = dto.TotalAmount.Value;
                existingEntity.AccountId = dto.AccountId;

                // Criar transação financeira
                var transaction = new FinancialTransaction
                {
                    CompanyId = existingEntity.CompanyId,
                    AccountId = dto.AccountId,
                    PurchaseOrderId = purchaseOrderId,
                    Description = transactionDescription,
                    Type = "Saída",
                    Amount = dto.TotalAmount.Value,
                    TransactionDate = processedAt,
                    CriadoPor = currentUserId,
                    CriadoEm = now
                };

                var createdTransaction = await _unitOfWork.FinancialTransactionRepository.CreateAsync(transaction);

                // Criar rateio de centros de custo se informado
                if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Count > 0)
                {
                    foreach (var cc in dto.CostCenterDistributions)
                    {
                        var transactionCostCenter = new TransactionCostCenter
                        {
                            FinancialTransactionId = createdTransaction.FinancialTransactionId,
                            CostCenterId = cc.CostCenterId,
                            Percentage = cc.Percentage,
                            Amount = cc.Amount,
                            CriadoPor = currentUserId,
                            CriadoEm = now
                        };
                        await _unitOfWork.TransactionCostCenterRepository.CreateAsync(transactionCostCenter);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return PurchaseOrderMapper.ToPurchaseOrderOutputDTO(existingEntity);
        }

        public async Task<bool> DeleteByIdAsync(long purchaseOrderId, long currentUserId, bool hasProcessPermission)
        {
            var existingEntity = await _unitOfWork.PurchaseOrderRepository.GetOneByIdAsync(purchaseOrderId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("PurchaseOrder", purchaseOrderId);
            }

            // Validar permissão de exclusão
            ValidateDeletePermission(existingEntity, currentUserId, hasProcessPermission);

            // Se aprovada, deletar transação financeira relacionada
            if (existingEntity.Status == "Aprovado")
            {
                var transaction = await _unitOfWork.FinancialTransactionRepository.GetByPurchaseOrderIdAsync(purchaseOrderId);
                if (transaction != null)
                {
                    // Deletar rateios primeiro
                    var costCenters = await _unitOfWork.TransactionCostCenterRepository.GetByTransactionIdAsync(transaction.FinancialTransactionId);
                    foreach (var cc in costCenters)
                    {
                        await _unitOfWork.TransactionCostCenterRepository.DeleteAsync(cc.TransactionCostCenterId);
                    }
                    await _unitOfWork.FinancialTransactionRepository.DeleteByIdAsync(transaction.FinancialTransactionId);
                }
            }

            var result = await _unitOfWork.PurchaseOrderRepository.DeleteByIdAsync(purchaseOrderId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        private void ValidateEditPermission(PurchaseOrder entity, long currentUserId, bool hasProcessPermission)
        {
            // Se pendente: criador pode editar OU quem tem canProcess
            if (entity.Status == "Pendente")
            {
                if (entity.CriadoPor != currentUserId && !hasProcessPermission)
                {
                    throw new BusinessRuleException("Edit", "Você só pode editar suas próprias ordens de compra pendentes.");
                }
            }
            // Se já processada: só quem tem canProcess pode editar
            else
            {
                if (!hasProcessPermission)
                {
                    throw new BusinessRuleException("Edit", "Apenas usuários com permissão de processamento podem editar ordens já processadas.");
                }
            }
        }

        private void ValidateDeletePermission(PurchaseOrder entity, long currentUserId, bool hasProcessPermission)
        {
            // Se pendente: criador pode deletar OU quem tem canProcess
            if (entity.Status == "Pendente")
            {
                if (entity.CriadoPor != currentUserId && !hasProcessPermission)
                {
                    throw new BusinessRuleException("Delete", "Você só pode excluir suas próprias ordens de compra pendentes.");
                }
            }
            // Se já processada: só quem tem canProcess pode deletar
            else
            {
                if (!hasProcessPermission)
                {
                    throw new BusinessRuleException("Delete", "Apenas usuários com permissão de processamento podem excluir ordens já processadas.");
                }
            }
        }
    }
}
