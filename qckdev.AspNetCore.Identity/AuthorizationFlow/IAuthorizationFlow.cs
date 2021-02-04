using Microsoft.AspNetCore.Authentication;

namespace qckdev.AspNetCore.Identity.AuthorizationFlow
{
    public interface IAuthorizationFlow<TAuthenticationHandler>
        where TAuthenticationHandler: IAuthenticationHandler
    {
    }
}
