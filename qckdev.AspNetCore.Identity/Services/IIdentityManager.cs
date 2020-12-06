using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Services
{
    public interface IIdentityManager
    {

        IQueryable<IdentityRole> Roles { get; }


        IdentityUser CreateInstanceUser();
        IdentityRole CreateInstanceRole();

        Task<IdentityResult> AddLoginAsync(IdentityUser user, UserLoginInfo login);
        Task<IdentityResult> AddToRoleAsync(IdentityUser user, string role);
        Task<IdentityResult> AddToRolesAsync(IdentityUser user, IEnumerable<string> roles);
        Task<IdentityResult> CreateUserAsync(IdentityUser user);
        Task<IdentityResult> CreateUserAsync(IdentityUser user, string password);
        Task<IdentityUser> FindByEmailAsync(string email);
        Task<IdentityUser> FindByIdAsync(string userId);
        Task<IEnumerable<string>> GetRolesAsync(IdentityUser user);
        Task<bool> IsInRoleAsync(IdentityUser user, string role);
        Task<IdentityUser> FindByLoginAsync(string loginProvider, string providerKey);
        Task<IdentityUser> FindByNameAsync(string userName);
        Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user);
        Task<string> GenerateUserTokenAsync(IdentityUser user, string tokenProvider, string purpose);
        Task<bool> VerifyUserTokenAsync(IdentityUser user, string tokenProvider, string purpose, string token);
        Task SignInAsync(IdentityUser user, bool isPersistent, string authenticationMethod = null);
        Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent);
        Task<SignInResult> CheckPasswordSignInAsync(IdentityUser user, string password, bool lockoutOnFailure);
        Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token);
        Task<IdentityResult> CreateRoleAsync(IdentityRole role);

        Task<IdentityResult> DeleteRoleAsync(IdentityRole role);

        Task<bool> RoleExistsAsync(string roleName);

    }
}