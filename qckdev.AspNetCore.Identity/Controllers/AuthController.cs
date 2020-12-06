using System.Threading.Tasks;
using MediatR;
using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.Queries;
using qckdev.AspNetCore.Identity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using qckdev.AspNetCore.Identity.Policies;

namespace qckdev.AspNetCore.Identity.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize]
    public class AuthController : ControllerBase
    {
        IMediator Mediator { get; }

        public AuthController(IMediator mediator)
        {
            this.Mediator = mediator;
        }


        [HttpGet]
        public async Task<UserViewModel> GetAsync()
        {
            return await Mediator.Send(new GetCurrentUserQuery());
        }

        [AllowAnonymous, HttpPost("createuser")]
        public async Task<Unit> CreateUserAsync(CreateUserCommand command)
        {
            return await Mediator.Send(command);
        }

        [AllowAnonymous, HttpGet("confirmemail")]
        public async Task<TokenViewModel> ConfirmEmailAsync(string token, string email)
        {
            return await Mediator.Send(new ConfirmEmailQuery() { Token = token, Email = email });
        }

        [AllowAnonymous, HttpPost("login")]
        public async Task<TokenViewModel> LoginAsync(LoginCommand command)
        {
            return await Mediator.Send(command);
        }

        [AllowAnonymous, HttpPost("externallogin")]
        public async Task<ITokenViewModel> ExternalLoginAsync(ExternalLoginCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost("linkexternallogin")]
        public async Task<Unit> LinkExternalLoginAsync(LinkExternalLoginCommand command)
        {
            return await Mediator.Send(command);
        }

        [Authorize(Policy = Constants.EXTERNALCONFIRMATION_POLICY), HttpPost("confirmexternaluser")]
        public async Task<TokenViewModel> ConfirmExternalUserAsync(ConfirmExternalUserCommand command)
        {
            return await Mediator.Send(command);
        }


        [Authorize(Policy = GuestAuthorizationHandler.POLICY_NAME), HttpGet("checkguestpolicy")]
        public async Task<IActionResult> CheckGuestPolicyAsync()
        {
            return await Task.FromResult(Ok());
        }

    }
}
