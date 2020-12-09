using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Test.xUnit.Services
{
    sealed class TestCurrentSessionService : ICurrentSessionService
    {
        const string baseUrl = "https://miauth.loc";

        public ClaimsPrincipal CurrentUser { get; private set; } = new ClaimsPrincipal();
        private IOptionsMonitor<JwtBearerOptions> Options { get; }

        public TestCurrentSessionService(IOptionsMonitor<JwtBearerOptions> options)
        {
            this.Options = options;
        }


        public void SetAccessToken(string value)
        {
            var options = Options.Get(JwtBearerDefaults.AuthenticationScheme);
            var tokenHandler = new JwtSecurityTokenHandler();

            this.CurrentUser = tokenHandler.ValidateToken(value, options.TokenValidationParameters, out SecurityToken validatedToken);
        }

        public string GetUserNameIdentifier()
        {
            return this.CurrentUser?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<string> GetAuthenticationScheme()
        {
            return await Task.Run(() => JwtBearerDefaults.AuthenticationScheme);
        }

        public string GetUriScheme()
        {
            return new Uri(baseUrl).Scheme;
        }

        public string GetUriByAction(string action = null, string controller = null, object values = null)
        {
            var uri = new UriBuilder(baseUrl) { Path = action };
            var valueDic = values?
                .GetType()
                .GetProperties()
                .ToDictionary(x => x.Name, y => y.GetValue(values)?.ToString());

            if (valueDic != null)
            {
                return QueryHelpers.AddQueryString(uri.ToString(), valueDic);
            }
            else
            {
                return uri.ToString();
            }
        }

        public string GetUriByPage(string page = null, string handler = null, object values = null)
        {
            throw new NotImplementedException();
        }

    }
}
