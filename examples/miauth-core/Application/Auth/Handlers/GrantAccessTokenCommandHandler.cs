using MediatR;
using miauthcore.Application.Auth.Commands;
using qckdev.AspNetCore.Identity.AuthorizationFlow;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace miauthcore.Application.Auth.Handlers
{
    public class GrantAccessTokenCommandHandler : IRequestHandler<GrantAccessTokenCommand, AuthorizationFlowCredential>
    {

        AuthorizationFlowProvider AuthorizationFlowProvider { get; }

        public GrantAccessTokenCommandHandler(AuthorizationFlowProvider authorizationFlowProvider)
        {
            this.AuthorizationFlowProvider = authorizationFlowProvider;
        }

        public async Task<AuthorizationFlowCredential> Handle(GrantAccessTokenCommand request, CancellationToken cancellationToken)
        {
            var provider = await AuthorizationFlowProvider.GetAuthorizationFlow(request.ProviderName);

            if (provider == null)
            {
                throw new ArgumentOutOfRangeException($"Unknown provider: '{request.ProviderName}'", innerException: null);
            }
            else
            {
                return await provider.OnGetToken(request.AccessCode, request.RedirectUri, request.State);
            }
        }

    }
}
