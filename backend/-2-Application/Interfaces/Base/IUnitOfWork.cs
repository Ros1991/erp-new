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
