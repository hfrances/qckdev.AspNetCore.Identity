
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.AuthorizationFlow
{
    public abstract class AuthorizationFlow
    {

        public string SchemeName { get; private set; }

        internal void SetSchemeName(string value)
        {
            this.SchemeName = value;
        }

        public abstract void OnAuthorization(string response_type, string scopes, string redirectUri, string state);
        public abstract Task<AuthorizationFlowCredential> OnGetToken(string code, string redirectUri, string state);

    }
}
