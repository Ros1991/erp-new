using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;
using ERP.CrossCutting.Helpers;

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

            // Validar distribuições de centros de custo se houver
            if (dto.CostCenterDistributions != null && dto.CostCenterDistributions.Any())
            {
                var totalPercentage = dto.CostCenterDistributions.Sum(d => d.Percentage);
                if (Math.Abs(totalPercentage - 100) > 0.01m)
                {
                    throw new ValidationException("CostCenterDistributions", "A soma das porcentagens deve ser 100%");
                }
            }

            // Criar o empréstimo
            var entity = LoanAdvanceMapper.ToEntity(dto, currentUserId);
            var createdEntity = await _unitOfWork.LoanAdvanceRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Buscar o apelido do empregado para a descrição
            var employee = await _unitOfWork.EmployeeRepository.GetOneByIdAsync(dto.EmployeeId);
            var employeeNickname = employee?.Nickname ?? employee?.FullName ?? "Empregado";

            // Determinar se é Empréstimo ou Adiantamento
            var now = DateTimeHelper.UtcNow;
            var startDate = DateTimeHelper.ToUtc(dto.StartDate);
            var isToday = startDate.Date == now.Date;
            var isSingleInstallment = dto.Installments == 1;
            
            // Se tem mais de 1 parcela OU não começa hoje = Empréstimo
            // Se tem 1 parcela E começa hoje = Adiantamento
            var transactionType = (isSingleInstallment && isToday) ? "Adiantamento" : "Empréstimo";
            var description = $"{transactionType} - {employeeNickname}";

            // Criar transação financeira automaticamente (Saída de dinheiro)
            // A data da transação é sempre HOJE, independente do início da cobrança
            var financialTransaction = new Domain.Entities.FinancialTransaction(
                companyId,
                dto.AccountId,
                null, // PurchaseOrderId
                null, // AccountPayableReceivableId
                null, // SupplierCustomerId
                createdEntity.LoanAdvanceId, // LoanAdvanceId - vincula transação ao empréstimo
                description,
                "Saída",
                dto.Amount,
                now, // Data da transação é HOJE
                currentUserId,
                null,
                now,
                null
            );

            var createdTransaction = await _unitOfWork.FinancialTransactionRepository.CreateAsync(financialTransaction);
            await _unitOfWork.SaveChangesAsync();

            // Criar distribuições de centro de custo
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

        public async Task<bool> DeleteByIdAsync(long loanAdvanceId, long companyId)
        {
            // Buscar e deletar transações financeiras relacionadas ao empréstimo
            var transactions = await _unitOfWork.FinancialTransactionRepository.GetAllAsync(companyId);
            var relatedTransactions = transactions.Where(t => t.LoanAdvanceId == loanAdvanceId).ToList();
            
            foreach (var transaction in relatedTransactions)
            {
                // Deletar a transação financeira
                // Os TransactionCostCenter serão deletados automaticamente via CASCADE no banco
                await _unitOfWork.FinancialTransactionRepository.DeleteByIdAsync(transaction.FinancialTransactionId);
            }
            
            // Agora deletar o empréstimo
            var result = await _unitOfWork.LoanAdvanceRepository.DeleteByIdAsync(loanAdvanceId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
