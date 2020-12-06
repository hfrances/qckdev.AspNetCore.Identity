using MediatR;
using qckdev.AspNetCore.Identity.Exceptions;
using qckdev.AspNetCore.Identity.Helpers;
using qckdev.AspNetCore.Identity.Queries;
using qckdev.AspNetCore.Identity.Services;
using qckdev.AspNetCore.Identity.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Handlers
{
    sealed class ConfirmEmailQueryHandler : IRequestHandler<ConfirmEmailQuery, TokenViewModel>
    {

        ICurrentSessionService CurrentSessionService { get; }
        IIdentityManager IdentityManager { get; }
        IServiceProvider Services { get; }

        public ConfirmEmailQueryHandler(
            ICurrentSessionService currentSessionService,
            IIdentityManager identityManager,
            IServiceProvider services
        )
        {
            this.CurrentSessionService = currentSessionService;
            this.IdentityManager = identityManager;
            this.Services = services;
        }

        public async Task<TokenViewModel> Handle(ConfirmEmailQuery request, CancellationToken cancellationToken)
        {
            var userNameLogged = CurrentSessionService.GetUserNameIdentifier();

            if (!string.IsNullOrEmpty(userNameLogged)
                && await IdentityManager.FindByNameAsync(userNameLogged) != null)
            {
                throw new IdentityException("Authentication failed: There is already another user logged in."); // TODO: Traducir.
            }
            else
            {
                var user = await IdentityManager.FindByEmailAsync(request.Email);

                if (user == null)
                {
                    throw new IdentityException("User does not exist."); // TODO: Traducir.
                }
                else if (user.EmailConfirmed)
                {
                    throw new IdentityException("Account already activated."); // TODO: Traducir. 
                }
                else
                {
                    IdentityResult result;

                    result = await IdentityManager.ConfirmEmailAsync(user, request.Token);
                    if (result.Succeeded)
                    {
                        return await UserHelper.CreateToken(this.Services, user);
                    }
                    else
                    {
                        throw new IdentityException("Error during error confirmation.", result.Errors); // TODO: Traducir.
                    }
                }
            }
        }
    }
}
