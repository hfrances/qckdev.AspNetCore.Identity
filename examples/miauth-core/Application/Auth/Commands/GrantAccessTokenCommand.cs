using MediatR;
using qckdev.AspNetCore.Identity.AuthorizationFlow;

namespace miauthcore.Application.Auth.Commands
{

    public sealed class GrantAccessTokenCommand : IRequest<AuthorizationFlowCredential>
    {

        public string ProviderName { get; set; }
        public string AccessCode { get; set; }
        public string RedirectUri { get; set; }
        public string State { get; set; }

    }

}
