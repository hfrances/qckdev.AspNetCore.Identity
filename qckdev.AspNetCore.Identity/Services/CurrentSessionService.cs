using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Services
{
    sealed class CurrentSessionService : ICurrentSessionService
    {

        private HttpContext HttpContext { get; }
        private LinkGenerator LinkGenerator { get; }
        private IAuthenticationSchemeProvider AuthenticationSchemeProvider { get; }
        public ClaimsPrincipal CurrentUser { get; }

        public CurrentSessionService(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator, IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            this.HttpContext = httpContextAccessor.HttpContext;
            this.LinkGenerator = linkGenerator;
            this.AuthenticationSchemeProvider = authenticationSchemeProvider;
            this.CurrentUser = this.HttpContext?.User;
        }

        public string GetUserNameIdentifier()
        {
            return this.CurrentUser?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<string> GetAuthenticationScheme()
        {
            var schemeHandlers = await AuthenticationSchemeProvider.GetAllSchemesAsync();

            foreach (var scheme in schemeHandlers)
            {
                var authResult = await HttpContext.AuthenticateAsync(scheme.Name);
                if (authResult.Succeeded)
                {
                    return scheme.Name;
                }
            }
            return null;
        }

        public string GetUriScheme()
        {
            return this.HttpContext.Request.Scheme;
        }

        public string GetUriByPage(string page = null, string handler = null, object values = null)
        {
            return this.LinkGenerator.GetUriByPage(this.HttpContext, page, handler, values);
        }

        public string GetUriByAction(string action = null, string controller = null, object values = null)
        {
            return this.LinkGenerator.GetUriByAction(this.HttpContext, action, controller, values);
        }

    }
}
