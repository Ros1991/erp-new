using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Repositories;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ErpContext _context;
        private IDbContextTransaction _transaction;
        private bool _disposed = false;

        // ACL Repositories - All implemented

        private ICompanyRepository _CompanyRepository;
        private IAccountRepository _AccountRepository;
        private IUserRepository _UserRepository;
        private IUserTokenRepository _UserTokenRepository;
        private IRoleRepository _RoleRepository;
        private ICompanyUserRepository _CompanyUserRepository;
        private IEmployeeRepository _EmployeeRepository;
        private IAccountPayableReceivableRepository _AccountPayableReceivableRepository;
        private ICostCenterRepository _CostCenterRepository;
        private ILoanAdvanceRepository _LoanAdvanceRepository;
        private ILocationRepository _LocationRepository;
        private IPurchaseOrderRepository _PurchaseOrderRepository;
        private ISupplierCustomerRepository _SupplierCustomerRepository;
        private ITaskRepository _TaskRepository;
        private IFinancialTransactionRepository _FinancialTransactionRepository;

        public UnitOfWork(ErpContext context)
        {
            _context = context;
        }

        public ICompanyRepository CompanyRepository =>
            _CompanyRepository ??= new CompanyRepository(_context);

        public IAccountRepository AccountRepository =>
            _AccountRepository ??= new AccountRepository(_context);

        public IUserRepository UserRepository =>
            _UserRepository ??= new UserRepository(_context);

        public IUserTokenRepository UserTokenRepository =>
            _UserTokenRepository ??= new UserTokenRepository(_context);

        public IRoleRepository RoleRepository =>
            _RoleRepository ??= new RoleRepository(_context);

        public ICompanyUserRepository CompanyUserRepository =>
            _CompanyUserRepository ??= new CompanyUserRepository(_context);

        public IEmployeeRepository EmployeeRepository =>
            _EmployeeRepository ??= new EmployeeRepository(_context);

        public IAccountPayableReceivableRepository AccountPayableReceivableRepository =>
            _AccountPayableReceivableRepository ??= new AccountPayableReceivableRepository(_context);

        public ICostCenterRepository CostCenterRepository =>
            _CostCenterRepository ??= new CostCenterRepository(_context);

        public ILoanAdvanceRepository LoanAdvanceRepository =>
            _LoanAdvanceRepository ??= new LoanAdvanceRepository(_context);

        public ILocationRepository LocationRepository =>
            _LocationRepository ??= new LocationRepository(_context);

        public IPurchaseOrderRepository PurchaseOrderRepository =>
            _PurchaseOrderRepository ??= new PurchaseOrderRepository(_context);

        public ISupplierCustomerRepository SupplierCustomerRepository =>
            _SupplierCustomerRepository ??= new SupplierCustomerRepository(_context);

        public ITaskRepository TaskRepository =>
            _TaskRepository ??= new TaskRepository(_context);

        public IFinancialTransactionRepository FinancialTransactionRepository =>
            _FinancialTransactionRepository ??= new FinancialTransactionRepository(_context);

        // Transaction Management
        public bool HasActiveTransaction => _transaction != null;

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("Uma transação já está ativa.");
            }

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Nenhuma transação ativa para confirmar.");
            }

            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Nenhuma transação ativa para reverter.");
            }

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Dispose Pattern
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Rollback any active transaction before disposing
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction.Dispose();
                    _transaction = null;
                }

                _context?.Dispose();
                _disposed = true;
            }
        }
    }
}
