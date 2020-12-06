using MediatR;

namespace qckdev.AspNetCore.Identity.Commands
{
    public sealed class LinkExternalLoginCommand : IRequest
    {

        public string Provider { get; set; }
        public string AccessCode { get; set; }
        public string RedirectUri { get; set; }
        public string State { get; set; }

    }
}
