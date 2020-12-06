using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebUtilities;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Test.xUnit.Services
{
    sealed class CurrentSessionService : ICurrentSessionService
    {
        const string baseUrl = "https://miauth.local";

        string _userId;

        public ClaimsPrincipal CurrentUser =>
            throw new NotImplementedException();

        public string GetUserNameIdentifier()
        {
            return _userId;
        }

        public void SetUserId(string value)
        {
            _userId = value;
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
