using MediatR;
using qckdev.AspNetCore.Identity.ViewModels;

namespace qckdev.AspNetCore.Identity.Commands
{
    public sealed class LoginCommand : IRequest<TokenViewModel>
    {

        public string Email { get; set; }
        public string Password { get; set; }

    }
}
