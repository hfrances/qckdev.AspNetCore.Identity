using Microsoft.AspNetCore.Authentication;
using qckdev.AspNetCore.Identity.AuthorizationFlow;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QIdentityAuthorizationFlowDependencyInjection
    {

        public static IServiceCollection AddAuthorizationFlow<TAuthenticationHandler, TAuthorizationFlow>(this IServiceCollection services)
            where TAuthenticationHandler : IAuthenticationHandler
            where TAuthorizationFlow : class, IAuthorizationFlow<TAuthenticationHandler>
        {
            return services
                .TryAddScoped<AuthorizationFlowProvider>()
                .AddScoped<IAuthorizationFlow<TAuthenticationHandler>, TAuthorizationFlow>();
        }

        public static AuthenticationBuilder AddAuthorizationFlow<TAuthenticationHandler, TAuthorizationFlow>(this AuthenticationBuilder builder)
            where TAuthenticationHandler : IAuthenticationHandler
            where TAuthorizationFlow : class, IAuthorizationFlow<TAuthenticationHandler>
        {
            builder.Services
                .AddAuthorizationFlow<TAuthenticationHandler, TAuthorizationFlow>();
            return builder;
        }

    }
}
