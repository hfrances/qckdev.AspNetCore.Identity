using System.Security.Claims;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Services
{
    public interface ICurrentSessionService
    {

        ClaimsPrincipal CurrentUser { get; }

        string GetUserNameIdentifier();
        Task<string> GetAuthenticationScheme();

        string GetUriScheme();
        string GetUriByPage(string page = null, string handler = null, object values = null);
        string GetUriByAction(string action = null, string controller = null, object values = null);
    }
}
