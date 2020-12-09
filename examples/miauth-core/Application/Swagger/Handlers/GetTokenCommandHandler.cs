using MediatR;
using miauthcore.Application.Swagger.Commands;
using Microsoft.Extensions.Configuration;
using qckdev.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace miauthcore.Application.Swagger.Handlers
{
    class GetTokenCommandHandler : IRequestHandler<GetTokenCommand>
    {

        AuthorizationFlowProvider AuthorizationFlowProvider { get; }
        IConfiguration Configuration { get; }


        public GetTokenCommandHandler(AuthorizationFlowProvider authorizationFlowProvider, IConfiguration configuration)
        {
            this.AuthorizationFlowProvider = authorizationFlowProvider;
            this.Configuration = configuration;
        }


        public async Task<Unit> Handle(GetTokenCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

            //if (code == null)
            //{
            //    return Unauthorized();
            //}
            //else
            //{
            //    var tokenConfig = TokenConfiguration.Get(this.Configuration, "Tokens");
            //    bool createIfNotExists;

            //    /* TODO: 
            //         *  1. Authentication-flow with the provider.
            //         *  2. Create JWT token. 
            //    */

            //    if (ClientByStateDictionary.TryGetValue(state, out string client_id))
            //    {
            //        createIfNotExists = (!string.IsNullOrWhiteSpace(client_id) && client_id == tokenConfig.ClientIdForCreation);
            //    }
            //    else
            //    {
            //        createIfNotExists = false;
            //    }

            //    var authorizationFlow = await AuthorizationFlowProvider.GetAuthorizationFlow(provider);
            //    var accessToken = authorizationFlow.OnGetToken(code, state);

            //    if (redirect_uri == null)
            //    {
            //        return Ok(new
            //        {
            //            authorizationCode = code,
            //            state
            //        });
            //    }
            //    else
            //    {
            //        return Redirect($"{redirect_uri}?code={code}&state={state}");
            //    }
            //}
        }
    }
}
