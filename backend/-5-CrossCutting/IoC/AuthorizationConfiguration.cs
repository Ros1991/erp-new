using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace ERP.CrossCutting.IoC
{
    public static class AuthorizationConfiguration
    {
        /// <summary>
        /// Configura política de autorização global - todos os endpoints exigem autenticação por padrão
        /// </summary>
        public static IServiceCollection AddGlobalAuthorization(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                // ✅ Política global: todos os endpoints exigem autenticação
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                    
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            return services;
        }
    }
}
