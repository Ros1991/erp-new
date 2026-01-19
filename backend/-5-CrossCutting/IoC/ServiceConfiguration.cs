using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Services;
using ERP.CrossCutting.Services;
using ERP.Infrastructure.UnitOfWork;

namespace ERP.CrossCutting.IoC
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Unit of Work Pattern
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Services
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICompanyUserService, CompanyUserService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<IAccountPayableReceivableService, AccountPayableReceivableService>();
            services.AddScoped<ICostCenterService, CostCenterService>();
            services.AddScoped<ILoanAdvanceService, LoanAdvanceService>();
            services.AddScoped<IFinancialTransactionService, FinancialTransactionService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            services.AddScoped<ISupplierCustomerService, SupplierCustomerService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IPayrollService, PayrollService>();
            services.AddScoped<IReportService, ReportService>();

            // Register Infrastructure Services
            services.AddSingleton<IPasswordHashService, PasswordHashService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddSingleton<IModuleConfigurationService, ModuleConfigurationService>();

            // Register HttpContextAccessor para acessar o contexto HTTP nos services
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
