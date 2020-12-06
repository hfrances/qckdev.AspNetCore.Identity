using MediatR;
using qckdev.AspNetCore.Identity.ViewModels;

namespace qckdev.AspNetCore.Identity.Commands
{
    public sealed class ConfirmExternalUserCommand : IRequest<TokenViewModel>
    {

        public dynamic NewUserData { get; set; }

    }
}
