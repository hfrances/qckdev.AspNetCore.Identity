using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.AuthorizationFlow.Google
{
    public class GoogleAuthorizationFlow : AuthorizationFlow<GoogleOptions, GoogleHandler>
    {

        ILogger<GoogleAuthorizationFlow> Logger { get; }

        public GoogleAuthorizationFlow(
                IHttpContextAccessor httpContextAccessor,
                IOptionsMonitor<GoogleOptions> authenticationOptions,
                ILogger<GoogleAuthorizationFlow> logger
            ) : base(httpContextAccessor, authenticationOptions)
        {
            this.Logger = logger;
        }

        public override void OnAuthorization(string response_type, string scopes, string redirectUri, string state)
        {
            throw new NotImplementedException();
        }

        public override async Task<AuthorizationFlowCredential> OnGetToken(string code, string redirectUri, string state)
        {
            var googleOptions = AuthenticationOptions.Get(this.SchemeName);

            try
            {
                var requestId = $"GoogleAuthorizationFlow-{Guid.NewGuid()}";
                var authorizationCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
                {
                    ClientSecrets = new ClientSecrets()
                    {
                        ClientId = googleOptions.ClientId,
                        ClientSecret = googleOptions.ClientSecret,
                    },
                    IncludeGrantedScopes = true,
                });
                var tokenResponse = await authorizationCodeFlow.ExchangeCodeForTokenAsync(
                    requestId,
                    code,
                    redirectUri,
                    CancellationToken.None);
                var payload = await GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken);

                return new AuthorizationFlowCredential
                {
                    UserId = payload.Subject,
                    Email = payload.Email,
                    DisplayName = payload.Name,
                    GivenName = payload.GivenName,
                    FamilyName = payload.FamilyName,
                    TokenType = tokenResponse.TokenType,
                    IdToken = tokenResponse.IdToken,
                    AccessToken = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken,
                    IssuedUtc = tokenResponse.IssuedUtc,
                    ExpiresInSeconds = tokenResponse.ExpiresInSeconds,
                    Scope = tokenResponse.Scope,
                };
            }
            catch (TokenResponseException ex)
            {
                Logger.LogError(ex, ex.Message);
                throw new AuthorizationFlowException(ex.Error.Error, ex.Error.ErrorDescription, ex.Error.ErrorUri);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
