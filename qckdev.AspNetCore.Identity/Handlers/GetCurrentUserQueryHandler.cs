using MediatR;
using qckdev.AspNetCore.Identity.Queries;
using qckdev.AspNetCore.Identity.ViewModels;
using qckdev.AspNetCore.Identity.Services;
using qckdev.AspNetCore.Identity.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace qckdev.AspNetCore.Identity.Handlers
{
    sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserViewModel>
    {

        ICurrentSessionService CurrentSessionService { get; }
        IIdentityManager IdentityManager { get; }

        public GetCurrentUserQueryHandler(
            ICurrentSessionService currentSessionService,
            IIdentityManager identityManager)
        {
            this.CurrentSessionService = currentSessionService;
            this.IdentityManager = identityManager;
        }

        public async Task<UserViewModel> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var userNameLogged = CurrentSessionService.GetUserNameIdentifier();
            IdentityUser userLogged;

            if (string.IsNullOrEmpty(userNameLogged))
            {
                throw new IdentityException("There is no current user."); // TODO: Traducir.
            }
            else if ((userLogged = await IdentityManager.FindByNameAsync(userNameLogged)) == null)
            {
                throw new IdentityException("User not found."); // TODO: Traducir.
            }
            else if (!userLogged.EmailConfirmed)
            {
                throw new IdentityException("Email confirmation pending. Please check your inbox.");
            }
            else
            {
                return new UserViewModel
                {
                    Id = userLogged.Id,
                    UserName = userLogged.UserName,
                    Email = userLogged.Email,
                    Roles = await IdentityManager.GetRolesAsync(userLogged)
                };
            }
        }
    }
}
