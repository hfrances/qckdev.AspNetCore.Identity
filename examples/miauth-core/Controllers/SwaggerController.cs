using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using miauthcore.Application.Swagger.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using qckdev.AspNetCore.Identity;

namespace miauthcore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SwaggerController : ControllerBase
    {

        static Dictionary<string, string> ClientByStateDictionary { get; }
            = new Dictionary<string, string>();

        AuthorizationFlowProvider AuthorizationFlowProvider { get; }
        IConfiguration Configuration { get; }
        IMediator Mediator { get; }

        public SwaggerController(AuthorizationFlowProvider authorizationFlowProvider, IConfiguration configuration, IMediator mediator)
        {
            this.AuthorizationFlowProvider = authorizationFlowProvider;
            this.Configuration = configuration;
            this.Mediator = mediator;
        }


        [HttpGet("{provider}"), Obsolete]
        public async void Get(string provider)
        {
            var authorizationFlow = await AuthorizationFlowProvider.GetAuthorizationFlow(provider);
            var redirectUri = new UriBuilder(HttpContext.Request.GetUri())
            {
                Path = $"/api/swagger/{provider}/token"
            };

            authorizationFlow.OnAuthorization(
                "code", null, redirectUri.ToString(), Guid.NewGuid().ToString()
            );
        }

        [HttpGet("{provider}/authorize")]
        public async void GetAuthorization(
            string client_id,
            string provider, string response_type,
            string scope, string redirect_uri, string state)
        {
            var authorizationFlow = await AuthorizationFlowProvider.GetAuthorizationFlow(provider);
            string response_type_definitive, redirect_uri_definitive;

            switch (response_type)
            {
                case "code":
                    response_type_definitive = response_type;
                    redirect_uri_definitive = $"{redirect_uri}";
                    break;

                case "token":
                    var parentRedirectUri = new UriBuilder(HttpContext.Request.GetUri())
                    {
                        Path = $"/api/swagger/{provider}/token",
                        Query = null,
                    };

                    response_type_definitive = "code";
                    redirect_uri_definitive = $"{parentRedirectUri}?redirect_uri={redirect_uri}";
                    break;

                default:
                    throw new NotSupportedException($"response_type={response_type}");
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                ClientByStateDictionary.Add(state, client_id);
            }

            authorizationFlow.OnAuthorization(
            response_type_definitive, scope,
            redirect_uri_definitive,
            state
        );
        }

        [HttpGet("{provider}/token")]
        public async Task<Unit> GetToken(string provider, string code, string redirect_uri, string state)
        {
            return await Mediator.Send(
                new GetTokenCommand()
                {
                    Provider = provider,
                    Code = code,
                    Redirect_uri = redirect_uri,
                    State = state
                }
            );
        }

    }
}
