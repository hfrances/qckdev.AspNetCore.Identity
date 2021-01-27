using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using qckdev.AspNetCore.Identity.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.AuthorizationFlow.Microsoft
{
    public class MicrosoftAuthorizationFlow : AuthorizationFlow<MicrosoftAccountOptions, MicrosoftAccountHandler>
    {


        public MicrosoftAuthorizationFlow(
                IHttpContextAccessor httpContextAccessor,
                IOptionsMonitor<MicrosoftAccountOptions> authenticationOptions
            ) : base(httpContextAccessor, authenticationOptions)
        { }

        public override void OnAuthorization(
            string response_type, string scopes, string redirectUri, string state)
        {
            var microsoftOptions = AuthenticationOptions.Get(this.SchemeName);
            var scopes_definitive = string.Join(" ",
                (scopes?.Trim().Split(" ") ?? microsoftOptions.Scope)
                    .Union(new string[] { "openid", "offline_access" })
                    .Distinct());

            var authorizeUrl = $"{microsoftOptions.AuthorizationEndpoint}" +
                    $"?client_id={microsoftOptions.ClientId}" +
                    $"&response_type={response_type}" +
                    $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                    $"&response_mode=query" +
                    $"&scope={Uri.EscapeDataString(scopes_definitive)}" +
                    $"{(state == null ? "" : $"&state={state}")}";

            HttpContext.Response.Redirect(authorizeUrl);
        }

        public override async Task<AuthorizationFlowCredential> OnGetToken(string code, string redirectUri, string state)
        {
            var microsoftOptions = AuthenticationOptions.Get(this.SchemeName);

            try
            {
                var uri = new Uri(microsoftOptions.TokenEndpoint);
                using (var client = new HttpClient()
                {
                    BaseAddress = new Uri(uri.GetLeftPart(UriPartial.Authority))
                })
                {
                    var formData = new Dictionary<string, string>()
                    {
                        { "client_id", microsoftOptions.ClientId },
                        { "client_secret", microsoftOptions.ClientSecret },
                        { "grant_type", "authorization_code" },
                        { "code", code },
                        { "redirect_uri", redirectUri },
                    };
                    if (!string.IsNullOrWhiteSpace(state))
                    {
                        formData.Add("state", state);
                    }

                    // https://docs.microsoft.com/en-us/azure/active-directory/develop/reference-saml-tokens
                    var rdo = await HttpClientHelper.Fetch<dynamic>(
                        client, HttpMethod.Post, uri.LocalPath,
                        new FormUrlEncodedContent(formData));
                    var accessToken = (string)rdo.access_token;
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(accessToken);
                    var email = securityToken.Claims.FirstOrDefault(x => x.Type == "unique_name")?.Value;
                    var name = securityToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
                    var givenName = securityToken.Claims.FirstOrDefault(x => x.Type == "given_name")?.Value;
                    var familyName = securityToken.Claims.FirstOrDefault(x => x.Type == "family_name")?.Value;
                    var subject = securityToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
                    var iat = Convert.ToInt64(securityToken.Claims.FirstOrDefault(x => x.Type == "iat")?.Value);
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                    return new AuthorizationFlowCredential
                    {
                        UserId = subject,
                        Email = email,
                        DisplayName = name,
                        FirstName = givenName,
                        LastName = familyName,
                        TokenType = rdo.token_type,
                        IdToken = rdo.id_token,
                        AccessToken = accessToken,
                        RefreshToken = rdo?.refreshToken,
                        IssuedUtc = epoch.AddSeconds(iat),
                        ExpiresInSeconds = rdo.expires_in,
                        Scope = rdo.scope,
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
