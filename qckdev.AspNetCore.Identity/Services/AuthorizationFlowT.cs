using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace qckdev.AspNetCore.Identity.Services
{

    public abstract class AuthorizationFlow<TOptions, TAuthenticationHandler>
            : AuthorizationFlow, IAuthorizationFlow<TAuthenticationHandler>
        where TOptions : AuthenticationSchemeOptions, new()
        where TAuthenticationHandler : AuthenticationHandler<TOptions>
    {

        protected HttpContext HttpContext { get; }
        protected IOptionsMonitor<TOptions> AuthenticationOptions { get; }

        protected AuthorizationFlow(IHttpContextAccessor httpContextAccessor, IOptionsMonitor<TOptions> authenticationOptions)
        {
            this.HttpContext = httpContextAccessor?.HttpContext;
            this.AuthenticationOptions = authenticationOptions;
        }

    }

}
