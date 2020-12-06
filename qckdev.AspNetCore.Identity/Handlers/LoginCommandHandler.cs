using MediatR;
using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.Exceptions;
using qckdev.AspNetCore.Identity.Helpers;
using qckdev.AspNetCore.Identity.ViewModels;
using qckdev.AspNetCore.Identity.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Handlers
{
    sealed class LoginCommandHandler : IRequestHandler<LoginCommand, TokenViewModel>
    {

        ICurrentSessionService CurrentSessionService { get; }
        IIdentityManager IdentityManager { get; }
        IServiceProvider Services { get; }

        public LoginCommandHandler(
            ICurrentSessionService currentSessionService,
            IIdentityManager identityManager,
            IServiceProvider services
        )
        {
            this.CurrentSessionService = currentSessionService;
            this.IdentityManager = identityManager;
            this.Services = services;
        }

        public async Task<TokenViewModel> Handle(LoginCommand request, CancellationToken cancellationToken)
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
                else if (!user.EmailConfirmed)
                {
                    throw new IdentityException("Authentication failed: Email confirmation pending. Please check your inbox."); // TODO: Traducir. 
                }
                else
                {
                    SignInResult result;

                    result = await IdentityManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        return await UserHelper.CreateToken(this.Services, user);
                    }
                    else if (result.IsLockedOut)
                    {
                        throw new IdentityException("Authentication failed: account locked."); // TODO: Traducir.
                    }
                    else if (result.IsNotAllowed)
                    {
                        throw new IdentityException("Authentication failed: user is not allowed to sign-in."); // TODO: Traducir.
                    }
                    else if (result.RequiresTwoFactor)
                    {
                        throw new IdentityException("Authentication failed: Two factor verification required."); // TODO: traducir.
                    }
                    else
                    {
                        throw new IdentityException("Authentication failed. Please check your username/password or contact with your administrator."); // TODO: traducir.
                    }
                }
            }
        }
    }
}
