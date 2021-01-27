using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using qckdev.AspNetCore.Identity.AuthorizationFlow.Microsoft;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QIdentityAuthenticationFlowMicrosoftDependencyInjection
    {

        public static IServiceCollection AddMicrosoftAuthorizationFlow(this IServiceCollection services)
        {
            return services
                .AddAuthorizationFlow<MicrosoftAccountHandler, MicrosoftAuthorizationFlow>()
            ;
        }

        public static AuthenticationBuilder AddMicrosoftAuthorizationFlow(this AuthenticationBuilder builder)
        {
            builder.Services
                .AddMicrosoftAuthorizationFlow();

            return builder;
        }

    }
}
