using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using qckdev.AspNetCore.Identity.AuthorizationFlow.Google;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QIdentityAuthorizationFlowGoogleDependencyInjection
    {

        public static IServiceCollection AddGoogleAuthorizationFlow(this IServiceCollection services)
        {
            return services
                .AddAuthorizationFlow<GoogleHandler, GoogleAuthorizationFlow>()
            ;
        }

        public static AuthenticationBuilder AddGoogleAuthorizationFlow(this AuthenticationBuilder builder)
        {
            builder.Services
                .AddGoogleAuthorizationFlow();

            return builder;
        }

    }
}
