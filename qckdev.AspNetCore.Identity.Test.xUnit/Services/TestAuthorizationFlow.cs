using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using qckdev.AspNetCore.Identity.AuthorizationFlow;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Test.xUnit.Services
{
    sealed class TestAuthorizationFlow : AuthorizationFlow<TestAuthenticationOptions, TestAuthenticationHandler>
    {

        sealed class UserInfo
        {
            public string Id { get; set; }
            public string Email { get; set; }
        }

        static IDictionary<string, UserInfo> users = new Dictionary<string, UserInfo>();
        static IDictionary<string, string> validAccessCodes = new Dictionary<string, string>();
        static IList<string> usedAccessCodes = new List<string>();

        public TestAuthorizationFlow(
                IHttpContextAccessor httpContextAccessor,
                IOptionsMonitor<TestAuthenticationOptions> authenticationOptions
            ) : base(httpContextAccessor, authenticationOptions)
        { }

        public override void OnAuthorization(string response_type, string scopes, string redirectUri, string state)
        {
            throw new System.NotImplementedException();
        }

        public override Task<AuthorizationFlowCredential> OnGetToken(string code, string redirectUri, string state)
        {
            if (usedAccessCodes.Contains(code))
            {
                throw new ArgumentException("Access code already used.", nameof(code));
            }
            else if (validAccessCodes.TryGetValue(code, out string userId))
            {
                if (users.TryGetValue(userId, out UserInfo userInfo))
                {
                    usedAccessCodes.Add(code);
                    return Task.FromResult(
                        new AuthorizationFlowCredential()
                        {
                            UserId = userInfo.Id,
                            Email = userInfo.Email,
                            AccessToken = $"token-{{{Guid.NewGuid()}}}",
                            TokenType = "Bearer",
                            IssuedUtc = DateTime.UtcNow,
                            ExpiresInSeconds = 300,
                        }
                    );
                }
                else
                {
                    throw new NullReferenceException("User not found.");
                }
            }
            else
            {
                throw new ArgumentException("Invalid access code.", nameof(code));
            }
        }

        public static string CreateAccessCode(string email)
        {
            var accessCode = Guid.NewGuid().ToString();
            var userInfo = users.Values.FirstOrDefault(x => x.Email == email);

            if (userInfo == null)
            {
                userInfo = new UserInfo()
                {
                    Id = $"id-{{{Guid.NewGuid()}}}",
                    Email = email
                };
                users.Add(userInfo.Id, userInfo);
            }
            validAccessCodes.Add(accessCode, userInfo.Id);
            return accessCode;
        }

    }
}
