using MediatR;
using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.Helpers;
using qckdev.AspNetCore.Identity.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using qckdev.AspNetCore.Identity.Services;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace qckdev.AspNetCore.Identity.Handlers
{
    sealed class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, ITokenViewModel>
    {

        IServiceProvider Services { get; }
        ICurrentSessionService CurrentSessionService { get; }
        AuthorizationFlowProvider AuthorizationFlowProvider { get; }
        IIdentityManager IdentityManager { get; }

        public ExternalLoginCommandHandler(
            IServiceProvider services,
            ICurrentSessionService currentSessionService,
            AuthorizationFlowProvider authorizationFlowProvider,
            IIdentityManager identityManager,
            IOptionsMonitor<JwtBearerOptions> jwtTokenConfiguration,
            IOptionsMonitor<JwtBearerMoreOptions> jwtBearerMoreOptions)
        {
            this.Services = services;
            this.CurrentSessionService = currentSessionService;
            this.AuthorizationFlowProvider = authorizationFlowProvider;
            this.IdentityManager = identityManager;
        }


        public async Task<ITokenViewModel> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var authorizationFlow = await AuthorizationFlowProvider.GetAuthorizationFlow(request.Provider);
                var providerToken = await authorizationFlow.OnGetToken(request.AccessCode, request.RedirectUri, null);

                var user = await IdentityManager.FindByLoginAsync(request.Provider, providerToken.UserId);
                if (user == null)
                {
                    IdentityResult result;
                    var loginInfo = new UserLoginInfo(request.Provider, providerToken.UserId, providerToken.Email);

                    user = await IdentityManager.FindByEmailAsync(providerToken.Email);
                    if (user == null)
                    {
                        var requestId = Guid.NewGuid();

                        user = IdentityManager.CreateInstanceUser();
                        user.Email = providerToken.Email;
                        user.UserName = providerToken.Email;
                        UserHelper.PendingToConfirmExternalUsers.Add(new PendingToConfirmExternalUser()
                        {
                            RequestId = requestId,
                            RequestDate = DateTime.Now,
                            User = user,
                            UserLoginInfo = loginInfo
                        });

                        var createUserToken =
                            await UserHelper.CreateToken(
                                this.Services, user,
                                new[] { Constants.EXTERNALCONFIRMATION_POLICY },
                                new[] { new Claim(Constants.REQUESTID_CLAIMTYPE, requestId.ToString()) }
                            );
                        return new TokenToConfirmExternalUserViewModel()
                        {
                            AccessToken = createUserToken.AccessToken,
                            Expired = createUserToken.Expired,
                            NewUserData = UserHelper.GetUserData(user)
                        };
                    }
                    else
                    {
                        result = IdentityResult.Success;
                    }

                    if (!result.Succeeded)
                    {
                        throw new AggregateException(
                            $"Error creating user for '{user.Email}'. See inner exceptions.",
                            result.Errors.Select(err => new Exception(err.Description)));
                    }
                    else
                    {
                        result = await IdentityManager.AddLoginAsync(user, loginInfo);
                        if (!result.Succeeded)
                        {
                            throw new AggregateException(
                                $"Error adding {loginInfo.LoginProvider} login for {user.Email}. See inner exceptions.",
                                result.Errors.Select(err => new Exception(err.Description)));
                        }
                    }
                }

                return await UserHelper.CreateToken(this.Services, user);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }

    }
}
