using Microsoft.AspNetCore.Authentication;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.AuthorizationFlow
{
    public sealed class AuthorizationFlowProvider
    {

        IServiceProvider ServiceProvider { get; }

        IAuthenticationSchemeProvider AuthenticationSchemeProvider { get; }

        public AuthorizationFlowProvider(
            IServiceProvider serviceProvider,
            IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            this.ServiceProvider = serviceProvider;
            this.AuthenticationSchemeProvider = authenticationSchemeProvider;
        }


        public async Task<AuthorizationFlow> GetAuthorizationFlow(string schemeName)
        {
            var schemeProviders = await AuthenticationSchemeProvider.GetAllSchemesAsync();
            var schemeProvider = schemeProviders.FirstOrDefault(x =>
                    x.Name.Equals(schemeName, StringComparison.InvariantCultureIgnoreCase));

            if (schemeProvider == null)
            {
                throw new ArgumentOutOfRangeException(nameof(schemeName), $"Provider not found: '{schemeName}'");
            }
            else
            {
                var authorizationFlowType = typeof(IAuthorizationFlow<>).MakeGenericType(schemeProvider.HandlerType);
                var authorizationFlow = (AuthorizationFlow)ServiceProvider.GetService(authorizationFlowType);

                authorizationFlow.SetSchemeName(schemeProvider.Name);
                return authorizationFlow;
            }
        }

    }
}
