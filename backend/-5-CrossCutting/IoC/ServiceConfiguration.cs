using CAU.Application.Interfaces;
using CAU.Application.Interfaces.Services;
using CAU.Application.Services;
using CAU.Infrastructure.UnitOfWork;

namespace CAU.CrossCutting.IoC
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

            return services;
        }
    }
}
