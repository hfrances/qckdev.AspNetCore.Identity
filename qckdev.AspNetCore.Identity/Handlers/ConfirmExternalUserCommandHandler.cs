using MediatR;
using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.ViewModels;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using qckdev.AspNetCore.Identity.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace qckdev.AspNetCore.Identity.Handlers
{
    sealed class ConfirmExternalUserCommandHandler : IRequestHandler<ConfirmExternalUserCommand, TokenViewModel>
    {

        IServiceProvider Services { get; }
        ICurrentSessionService CurrentSessionService { get; }
        IIdentityManager IdentityManager { get; }


        public ConfirmExternalUserCommandHandler(
            IServiceProvider services,
            ICurrentSessionService currentSessionService,
            IIdentityManager identityManager,
            IOptionsMonitor<JwtBearerOptions> jwtBearerOptions,
            IOptionsMonitor<JwtBearerMoreOptions> jwtBearerMoreOptions
        )
        {
            this.Services = Services;
            this.CurrentSessionService = currentSessionService;
            this.IdentityManager = identityManager;
        }

        public async Task<TokenViewModel> Handle(ConfirmExternalUserCommand request, CancellationToken cancellationToken)
        {
            IdentityResult result;
            var requestId =
                Guid.Parse(CurrentSessionService.CurrentUser?.Claims
                    .FirstOrDefault(x => x.Type == Constants.REQUESTID_CLAIMTYPE)
                    ?.Value);
            var pendingRequest = UserHelper.PendingToConfirmExternalUsers.Find(x => x.RequestId == requestId);

            UserHelper.PendingToConfirmExternalUsers.Remove(pendingRequest);
            UserHelper.SetUserData(pendingRequest.User, request.NewUserData);
            pendingRequest.User.EmailConfirmed = true;
            result = await IdentityManager.CreateUserAsync(pendingRequest.User);
            if (result.Succeeded)
            {
                if (result.Succeeded)
                {
                    result = await IdentityManager.AddLoginAsync(pendingRequest.User, pendingRequest.UserLoginInfo);
                    if (result.Succeeded)
                    {
                        return await UserHelper.CreateToken(this.Services, pendingRequest.User);
                    }
                    else
                    {
                        throw new AggregateException(
                            $"Error adding {pendingRequest.UserLoginInfo.LoginProvider} login for {pendingRequest.User.Email}. See inner exceptions.",
                            result.Errors.Select(err => new Exception(err.Description)));
                    }
                }
                else
                {
                    throw new AggregateException(
                        $"Error creating user for '{pendingRequest.User.Email}'. See inner exceptions.",
                        result.Errors.Select(err => new Exception(err.Description)));
                }
            }
            else
            {
                throw new AggregateException(
                    $"Error confirming user for '{pendingRequest.User.Email}'. See inner exceptions.",
                    result.Errors.Select(err => new Exception(err.Description)));
            }
        }
    }
}
