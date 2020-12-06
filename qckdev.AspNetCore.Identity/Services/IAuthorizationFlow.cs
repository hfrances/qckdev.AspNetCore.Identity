using Microsoft.AspNetCore.Authentication;

namespace qckdev.AspNetCore.Identity.Services
{
    public interface IAuthorizationFlow<TAuthenticationHandler>
        where TAuthenticationHandler: IAuthenticationHandler
    {
    }
}
