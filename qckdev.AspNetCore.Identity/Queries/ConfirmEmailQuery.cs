using MediatR;
using qckdev.AspNetCore.Identity.ViewModels;

namespace qckdev.AspNetCore.Identity.Queries
{
    class ConfirmEmailQuery : IRequest<TokenViewModel>
    {

        public string Token { get; set; }
        public string Email { get; set; }

    }
}
