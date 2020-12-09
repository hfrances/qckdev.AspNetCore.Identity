using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using qckdev.AspNetCore.Identity;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace miauthcore.AuthenticationFlow
{
    public class GoogleAuthorizationFlow : AuthorizationFlow<GoogleOptions, GoogleHandler>
    {

        public GoogleAuthorizationFlow(
                IHttpContextAccessor httpContextAccessor,
                IOptionsMonitor<GoogleOptions> authenticationOptions
            ) : base(httpContextAccessor, authenticationOptions)
        { }

        public override void OnAuthorization(string response_type, string scopes, string redirectUri, string state)
        {
            throw new NotImplementedException();
        }

        public override async Task<AuthorizationFlowCredential> OnGetToken(string code, string redirectUri, string state)
        {
            var googleOptions = AuthenticationOptions.Get(this.SchemeName);
            var authorizationCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = new ClientSecrets()
                {
                    ClientId = googleOptions.ClientId,
                    ClientSecret = googleOptions.ClientSecret,
                },
                IncludeGrantedScopes = true,
            });

            try
            {
                var tokenResponse = await authorizationCodeFlow.ExchangeCodeForTokenAsync(
                    "userId",
                    code,
                    redirectUri,
                    CancellationToken.None);
                var payload = await GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken);

                return new AuthorizationFlowCredential
                {
                    UserId = payload.Subject,
                    Email = payload.Email,
                    DisplayName = payload.Name,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    TokenType = tokenResponse.TokenType,
                    IdToken = tokenResponse.IdToken,
                    AccessToken = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken,
                    IssuedUtc = tokenResponse.IssuedUtc,
                    ExpiresInSeconds = tokenResponse.ExpiresInSeconds,
                    Scope = tokenResponse.Scope,
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
