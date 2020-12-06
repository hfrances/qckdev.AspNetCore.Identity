using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.Exceptions;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Handlers
{
    sealed class LinkExternalLoginCommandHandler : IRequestHandler<LinkExternalLoginCommand>
    {

        ICurrentSessionService CurrentSessionService { get; }
        AuthorizationFlowProvider AuthorizationFlowProvider { get; }
        IIdentityManager IdentityManager { get; }
        IOptionsMonitor<JwtBearerOptions> JwtTokenConfiguration { get; }
        IOptionsMonitor<JwtBearerMoreOptions> JwtBearerMoreOptions { get; }

        public LinkExternalLoginCommandHandler(
            ICurrentSessionService currentSessionService,
            AuthorizationFlowProvider authorizationFlowProvider,
            IIdentityManager identityManager,
            IOptionsMonitor<JwtBearerOptions> jwtTokenConfiguration,
            IOptionsMonitor<JwtBearerMoreOptions> jwtBearerMoreOptions)
        {
            this.CurrentSessionService = currentSessionService;
            this.AuthorizationFlowProvider = authorizationFlowProvider;
            this.IdentityManager = identityManager;
            this.JwtTokenConfiguration = jwtTokenConfiguration;
            this.JwtBearerMoreOptions = jwtBearerMoreOptions;
        }

        public async Task<Unit> Handle(LinkExternalLoginCommand request, CancellationToken cancellationToken)
        {
            var userIdLogged = CurrentSessionService.GetUserNameIdentifier();

            if (!string.IsNullOrWhiteSpace(userIdLogged))
            {
                throw new CurrentSessionException("There is no an active session. Operation cancelled.");
            }
            else
            {
                IdentityUser userLogged = await IdentityManager.FindByNameAsync(userIdLogged);

                if (userLogged == null)
                {
                    throw new CurrentSessionException("User id not found. Operation cancelled.");
                }
                else if (!userLogged.EmailConfirmed)
                {
                    throw new CurrentSessionException("User not confirmed. Operation cancelled.");
                }
                else
                {
                    var authorizationFlow = await AuthorizationFlowProvider.GetAuthorizationFlow(request.Provider);
                    var providerToken = await authorizationFlow.OnGetToken(request.AccessCode, request.RedirectUri, request.State);
                    var userByLogin = await IdentityManager.FindByLoginAsync(request.Provider, providerToken.UserId);


                    if (userByLogin != null && userByLogin.Id == userLogged.Id)
                    {
                        throw new IdentityException("This account is already registered to another user. Operation cancelled.");
                    }
                    else
                    {
                        var loginInfo = new UserLoginInfo(request.Provider, providerToken.UserId, providerToken.Email);
                        IdentityResult result;

                        result = await IdentityManager.AddLoginAsync(userLogged, loginInfo);
                        if (!result.Succeeded)
                        {
                            throw new AggregateException(
                                $"Error adding account to the current user. Operation cancelled. See inner exceptions.",
                                result.Errors.Select(err => new IdentityException(err.Description)));
                        }
                        else
                        {
                            return Unit.Value;
                        }
                    }
                }
            }
        }
    }

}
