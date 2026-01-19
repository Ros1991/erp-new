using ERP.Application.Interfaces.Repositories;

namespace ERP.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //Repositories - All implemented
        ICompanyRepository CompanyRepository { get; }
        IAccountRepository AccountRepository { get; }
        IUserRepository UserRepository { get; }
        IUserTokenRepository UserTokenRepository { get; }
        IRoleRepository RoleRepository { get; }
        ICompanyUserRepository CompanyUserRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        IAccountPayableReceivableRepository AccountPayableReceivableRepository { get; }
        ICostCenterRepository CostCenterRepository { get; }
        ILoanAdvanceRepository LoanAdvanceRepository { get; }
        ILocationRepository LocationRepository { get; }
        IPurchaseOrderRepository PurchaseOrderRepository { get; }
        ISupplierCustomerRepository SupplierCustomerRepository { get; }
        ITaskRepository TaskRepository { get; }
        IFinancialTransactionRepository FinancialTransactionRepository { get; }
        IContractRepository ContractRepository { get; }
        IPayrollRepository PayrollRepository { get; }
        IPayrollEmployeeRepository PayrollEmployeeRepository { get; }
        IPayrollItemRepository PayrollItemRepository { get; }
        IContractBenefitDiscountRepository ContractBenefitDiscountRepository { get; }
        ITransactionCostCenterRepository TransactionCostCenterRepository { get; }

        // Transaction Management
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        
        // Save Changes
        Task<int> SaveChangesAsync();
        
        // Check if there's an active transaction
        bool HasActiveTransaction { get; }
    }
}
