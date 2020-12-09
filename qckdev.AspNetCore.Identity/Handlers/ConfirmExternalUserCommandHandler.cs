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
            this.Services = services;
            this.CurrentSessionService = currentSessionService;
            this.IdentityManager = identityManager;
        }

        public async Task<TokenViewModel> Handle(ConfirmExternalUserCommand request, CancellationToken cancellationToken)
        {
            var requestId =
                Guid.Parse(CurrentSessionService.CurrentUser?.Claims
                    .FirstOrDefault(x => x.Type == Constants.REQUESTID_CLAIMTYPE)
                    ?.Value);
            var pendingRequest = UserHelper.PendingToConfirmExternalUsers.Find(x => x.RequestId == requestId);

            UserHelper.PendingToConfirmExternalUsers.Remove(pendingRequest);
            UserHelper.SetUserData(pendingRequest.User, request.NewUserData);
            pendingRequest.User.EmailConfirmed = true;

            await CreateUserHelper.CreateExternalUser(this.Services, pendingRequest.User, pendingRequest.Roles, pendingRequest.UserLoginInfo);
            return await UserHelper.CreateToken(this.Services, pendingRequest.User);
        }
    }
}
