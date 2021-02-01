using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using miauthcore.Application.Auth.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using qckdev.AspNetCore.Identity.AuthorizationFlow;

namespace miauthcore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private IMediator Mediator { get; }

        public AuthController(IMediator mediator)
        {
            this.Mediator = mediator;
        }

        [HttpPost("grantAccessToken")]
        public async Task<AuthorizationFlowCredential> GrantAccessToken(GrantAccessTokenCommand request)
        {
            return await Mediator.Send(request);
        }

        [HttpGet("grantAccessToken/{provider}")]
        public async Task<AuthorizationFlowCredential> GrantAccessToken(string provider, string code, string redirect_uri, string session_state = null, string client_info = null)
        {
            return await GrantAccessToken(new GrantAccessTokenCommand()
            {
                ProviderName = provider,
                AccessCode = code,
                RedirectUri = redirect_uri,
                State = session_state
            });
        }

    }
}
