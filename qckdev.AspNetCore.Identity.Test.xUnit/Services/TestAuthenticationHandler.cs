using Microsoft.AspNetCore.Authentication.OAuth;

namespace qckdev.AspNetCore.Identity.Test.xUnit.Services
{
    sealed class TestAuthenticationHandler : OAuthHandler<TestAuthenticationOptions>
    {

        public TestAuthenticationHandler(
            Microsoft.Extensions.Options.IOptionsMonitor<TestAuthenticationOptions> options,
            Microsoft.Extensions.Logging.ILoggerFactory logger,
            System.Text.Encodings.Web.UrlEncoder encoder,
            Microsoft.AspNetCore.Authentication.ISystemClock clock
        ) 
            : base(options, logger, encoder, clock)
        {

        }

    }
}
