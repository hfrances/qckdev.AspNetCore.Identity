using Microsoft.AspNetCore.Authentication;

namespace qckdev.AspNetCore.Identity.Test.xUnit.Services
{
    static class DependencyInjection
    {

        public static AuthenticationBuilder AddTest(this AuthenticationBuilder builder)
        {
            return builder
                .AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>(
                    TestAuthenticationDefaults.AuthenticationScheme,
                    options => { }
                );
        }

        public static AuthenticationBuilder AddTestAuthorizationFlow(this AuthenticationBuilder builder)
        {
            return builder
                .AddAuthorizationFlow<TestAuthenticationHandler, TestAuthorizationFlow>();
        }

    }
}
