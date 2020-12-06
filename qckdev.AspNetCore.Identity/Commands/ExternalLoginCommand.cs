using MediatR;
using qckdev.AspNetCore.Identity.ViewModels;

namespace qckdev.AspNetCore.Identity.Commands
{
    public sealed class ExternalLoginCommand : IRequest<ITokenViewModel>
    {

        public string Provider { get; set; }
        public string AccessCode { get; set; }
        public string RedirectUri { get; set; }
        public string State { get; set; }

    }
}
