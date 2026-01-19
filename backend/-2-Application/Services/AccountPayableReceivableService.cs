using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Application.Services
{
    public class AccountPayableReceivableService : IAccountPayableReceivableService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountPayableReceivableService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AccountPayableReceivableOutputDTO>> GetAllAsync(long companyId)
        {
            var entities = await _unitOfWork.AccountPayableReceivableRepository.GetAllAsync(companyId);
            return AccountPayableReceivableMapper.ToAccountPayableReceivableOutputDTOList(entities);
        }

        public async Task<PagedResult<AccountPayableReceivableOutputDTO>> GetPagedAsync(long companyId, AccountPayableReceivableFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.AccountPayableReceivableRepository.GetPagedAsync(companyId, filters);
            var dtoItems = AccountPayableReceivableMapper.ToAccountPayableReceivableOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<AccountPayableReceivableOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<AccountPayableReceivableOutputDTO> GetOneByIdAsync(long accountPayableReceivableId)
        {
            var entity = await _unitOfWork.AccountPayableReceivableRepository.GetOneByIdAsync(accountPayableReceivableId);
            if (entity == null)
            {
                throw new EntityNotFoundException("AccountPayableReceivable", accountPayableReceivableId);
            }

            // Buscar a transação financeira relacionada (se existir, quando isPaid=true)
            var transaction = await _unitOfWork.FinancialTransactionRepository.GetByAccountPayableReceivableIdAsync(accountPayableReceivableId);
            
            return AccountPayableReceivableMapper.ToAccountPayableReceivableOutputDTO(entity, transaction);
        }

        public async Task<AccountPayableReceivableOutputDTO> CreateAsync(AccountPayableReceivableInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // Se isPaid=true, conta corrente é obrigatória
            if (dto.IsPaid && !dto.AccountId.HasValue)
            {
                throw new ValidationException("AccountId", "Conta corrente é obrigatória quando a conta está marcada como paga.");
            }

            // Validar distribuições de centros de custo se houver
            if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Any())
            {
                var totalPercentage = dto.CostCenterDistributions.Sum(d => d.Percentage);
                if (Math.Abs(totalPercentage - 100) > 0.01m)
                {
                    throw new ValidationException("CostCenterDistributions", "A soma das porcentagens deve ser 100%");
                }
            }

            var now = DateTime.UtcNow;
            var entity = AccountPayableReceivableMapper.ToEntity(dto, companyId, currentUserId);
            
            var createdEntity = await _unitOfWork.AccountPayableReceivableRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Salvar distribuições de centro de custo na tabela própria (sempre)
            if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Any())
            {
                foreach (var distribution in dto.CostCenterDistributions)
                {
                    var aprcc = new Domain.Entities.AccountPayableReceivableCostCenter(
                        createdEntity.AccountPayableReceivableId,
                        distribution.CostCenterId,
                        distribution.Amount ?? 0,
                        distribution.Percentage,
                        currentUserId,
                        null,
                        now,
                        null
                    );
                    createdEntity.CostCenterDistributions.Add(aprcc);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            Domain.Entities.FinancialTransaction createdTransaction = null;

            // Criar transação financeira APENAS se isPaid=true
            if (dto.IsPaid && dto.AccountId.HasValue)
            {
                createdTransaction = await CreateFinancialTransactionAsync(
                    createdEntity, dto, companyId, currentUserId, now);
            }

            return AccountPayableReceivableMapper.ToAccountPayableReceivableOutputDTO(createdEntity, createdTransaction);
        }

        public async Task<AccountPayableReceivableOutputDTO> UpdateByIdAsync(long accountPayableReceivableId, AccountPayableReceivableInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            // Se isPaid=true, conta corrente é obrigatória
            if (dto.IsPaid && !dto.AccountId.HasValue)
            {
                throw new ValidationException("AccountId", "Conta corrente é obrigatória quando a conta está marcada como paga.");
            }

            // Validar distribuições de centros de custo se houver
            if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Any())
            {
                var totalPercentage = dto.CostCenterDistributions.Sum(d => d.Percentage);
                if (Math.Abs(totalPercentage - 100) > 0.01m)
                {
                    throw new ValidationException("CostCenterDistributions", "A soma das porcentagens deve ser 100%");
                }
            }

            var existingEntity = await _unitOfWork.AccountPayableReceivableRepository.GetOneByIdAsync(accountPayableReceivableId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("AccountPayableReceivable", accountPayableReceivableId);
            }

            var now = DateTime.UtcNow;
            var wasPaid = existingEntity.IsPaid;
            
            AccountPayableReceivableMapper.UpdateEntity(existingEntity, dto, currentUserId);

            // Atualizar distribuições de centro de custo na tabela própria
            // Remover antigas
            if (existingEntity.CostCenterDistributions != null && existingEntity.CostCenterDistributions.Any())
            {
                var oldDistributions = existingEntity.CostCenterDistributions.ToList();
                foreach (var old in oldDistributions)
                {
                    existingEntity.CostCenterDistributions.Remove(old);
                }
            }

            // Adicionar novas
            if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Any())
            {
                foreach (var distribution in dto.CostCenterDistributions)
                {
                    var aprcc = new Domain.Entities.AccountPayableReceivableCostCenter(
                        accountPayableReceivableId,
                        distribution.CostCenterId,
                        distribution.Amount ?? 0,
                        distribution.Percentage,
                        currentUserId,
                        null,
                        now,
                        null
                    );
                    existingEntity.CostCenterDistributions.Add(aprcc);
                }
            }

            // Gerenciar transação financeira baseado em isPaid
            var existingTransaction = await _unitOfWork.FinancialTransactionRepository.GetByAccountPayableReceivableIdAsync(accountPayableReceivableId);

            if (dto.IsPaid && dto.AccountId.HasValue)
            {
                if (existingTransaction != null)
                {
                    // Atualizar transação existente
                    await UpdateFinancialTransactionAsync(existingTransaction, existingEntity, dto, currentUserId, now);
                }
                else
                {
                    // Criar nova transação (mudou de não pago para pago)
                    existingTransaction = await CreateFinancialTransactionAsync(
                        existingEntity, dto, existingEntity.CompanyId, currentUserId, now);
                }
            }
            else if (!dto.IsPaid && existingTransaction != null)
            {
                // Deletar transação (mudou de pago para não pago)
                await _unitOfWork.FinancialTransactionRepository.DeleteByIdAsync(existingTransaction.FinancialTransactionId);
                existingTransaction = null;
            }
            
            await _unitOfWork.SaveChangesAsync();
            
            return AccountPayableReceivableMapper.ToAccountPayableReceivableOutputDTO(existingEntity, existingTransaction);
        }

        public async Task<bool> DeleteByIdAsync(long accountPayableReceivableId)
        {
            // Deletar transação financeira relacionada, se existir
            var transaction = await _unitOfWork.FinancialTransactionRepository.GetByAccountPayableReceivableIdAsync(accountPayableReceivableId);
            if (transaction != null)
            {
                await _unitOfWork.FinancialTransactionRepository.DeleteByIdAsync(transaction.FinancialTransactionId);
            }

            var result = await _unitOfWork.AccountPayableReceivableRepository.DeleteByIdAsync(accountPayableReceivableId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        private async Task<Domain.Entities.FinancialTransaction> CreateFinancialTransactionAsync(
            Domain.Entities.AccountPayableReceivable entity,
            AccountPayableReceivableInputDTO dto,
            long companyId,
            long currentUserId,
            DateTime now)
        {
            // Tipo da transação baseado no tipo da conta (Pagar = Saída, Receber = Entrada)
            var transactionType = dto.Type == "Pagar" ? "Saída" : "Entrada";

            // Usar PaymentDate se informada, senão usa DueDate
            var transactionDate = dto.PaymentDate.HasValue 
                ? DateTime.SpecifyKind(dto.PaymentDate.Value, DateTimeKind.Utc)
                : DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc);

            var financialTransaction = new Domain.Entities.FinancialTransaction(
                companyId,
                dto.AccountId!.Value,
                null, // PurchaseOrderId
                entity.AccountPayableReceivableId,
                dto.SupplierCustomerId,
                null, // LoanAdvanceId
                dto.Description,
                transactionType,
                dto.Amount,
                transactionDate,
                currentUserId,
                null,
                now,
                null
            );

            var createdTransaction = await _unitOfWork.FinancialTransactionRepository.CreateAsync(financialTransaction);
            await _unitOfWork.SaveChangesAsync();

            // Copiar distribuições de centro de custo para a transação
            if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Any())
            {
                foreach (var distribution in dto.CostCenterDistributions)
                {
                    var tcc = new Domain.Entities.TransactionCostCenter(
                        createdTransaction.FinancialTransactionId,
                        distribution.CostCenterId,
                        distribution.Amount ?? 0,
                        distribution.Percentage,
                        currentUserId,
                        null,
                        now,
                        null
                    );
                    createdTransaction.TransactionCostCenterList.Add(tcc);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            return createdTransaction;
        }

        private async Task UpdateFinancialTransactionAsync(
            Domain.Entities.FinancialTransaction transaction,
            Domain.Entities.AccountPayableReceivable entity,
            AccountPayableReceivableInputDTO dto,
            long currentUserId,
            DateTime now)
        {
            // Usar PaymentDate se informada, senão usa DueDate
            var transactionDate = dto.PaymentDate.HasValue 
                ? DateTime.SpecifyKind(dto.PaymentDate.Value, DateTimeKind.Utc)
                : DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Utc);

            // Atualizar dados da transação
            transaction.AccountId = dto.AccountId!.Value;
            transaction.Description = dto.Description;
            transaction.Amount = dto.Amount;
            transaction.TransactionDate = transactionDate;
            transaction.SupplierCustomerId = dto.SupplierCustomerId;
            transaction.Type = dto.Type == "Pagar" ? "Saída" : "Entrada";
            transaction.AtualizadoPor = currentUserId;
            transaction.AtualizadoEm = now;

            // Deletar cost centers antigos
            if (transaction.TransactionCostCenterList != null && transaction.TransactionCostCenterList.Any())
            {
                var oldCostCenters = transaction.TransactionCostCenterList.ToList();
                foreach (var oldCc in oldCostCenters)
                {
                    transaction.TransactionCostCenterList.Remove(oldCc);
                }
            }

            // Adicionar novos cost centers
            if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Any())
            {
                foreach (var distribution in dto.CostCenterDistributions)
                {
                    var tcc = new Domain.Entities.TransactionCostCenter(
                        transaction.FinancialTransactionId,
                        distribution.CostCenterId,
                        distribution.Amount ?? 0,
                        distribution.Percentage,
                        currentUserId,
                        null,
                        now,
                        null
                    );
                    transaction.TransactionCostCenterList.Add(tcc);
                }
            }
        }
    }
}
