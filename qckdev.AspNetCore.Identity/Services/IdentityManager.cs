using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Services
{

    sealed class IdentityManager<TUser, TRole> : IIdentityManager
        where TUser : IdentityUser, new()
        where TRole : IdentityRole, new()
    {

        UserManager<TUser> UserManager { get; }
        SignInManager<TUser> SignInManager { get; }
        RoleManager<TRole> RoleManager { get; }


        public IdentityManager(
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            RoleManager<TRole> roleManager)
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
            this.RoleManager = roleManager;
        }


        public IQueryable<IdentityRole> Roles
        {
            get => RoleManager.Roles.AsQueryable<IdentityRole>();
        }


        public IdentityUser CreateInstanceUser()
        {
            return new TUser();
        }

        public IdentityRole CreateInstanceRole()
        {
            return new TRole();
        }

        public async Task<IdentityUser> FindByIdAsync(string userId)
        {
            return await UserManager.FindByIdAsync(userId);
        }

        public async Task<IdentityUser> FindByNameAsync(string userName)
        {
            return await UserManager.FindByNameAsync(userName);
        }

        public async Task<IdentityUser> FindByEmailAsync(string email)
        {
            return await UserManager.FindByEmailAsync(email);
        }

        public async Task<IdentityUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            return await UserManager.FindByLoginAsync(loginProvider, providerKey);
        }

        public async Task<bool> IsInRoleAsync(IdentityUser user, string role)
        {
            return await UserManager.IsInRoleAsync((TUser)user, role);
        }

        public async Task<IdentityResult> CreateUserAsync(IdentityUser user)
        {
            return await UserManager.CreateAsync((TUser)user);
        }

        public async Task<IdentityResult> CreateUserAsync(IdentityUser user, string password)
        {
            return await UserManager.CreateAsync((TUser)user, password);
        }

        public async Task<IdentityResult> AddLoginAsync(IdentityUser user, UserLoginInfo login)
        {
            return await UserManager.AddLoginAsync((TUser)user, login);
        }

        public async Task<IEnumerable<string>> GetRolesAsync(IdentityUser user)
        {
            return await UserManager.GetRolesAsync((TUser)user);
        }

        public async Task<IdentityResult> AddToRoleAsync(IdentityUser user, string role)
        {
            return await UserManager.AddToRoleAsync((TUser)user, role);
        }

        public async Task<IdentityResult> AddToRolesAsync(IdentityUser user, IEnumerable<string> roles)
        {
            return await UserManager.AddToRolesAsync((TUser)user, roles);
        }

        public async Task<string> GenerateUserTokenAsync(IdentityUser user, string tokenProvider, string purpose)
        {
            return await UserManager.GenerateUserTokenAsync((TUser)user, tokenProvider, purpose);
        }

        public async Task<bool> VerifyUserTokenAsync(IdentityUser user, string tokenProvider, string purpose, string token)
        {
            return await UserManager.VerifyUserTokenAsync((TUser)user, tokenProvider, purpose, token);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user)
        {
            return await UserManager.GenerateEmailConfirmationTokenAsync((TUser)user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token)
        {
            return await UserManager.ConfirmEmailAsync((TUser)user, token);
        }

        public async Task SignInAsync(IdentityUser user, bool isPersistent, string authenticationMethod = null)
        {
            await SignInManager.SignInAsync((TUser)user, isPersistent, authenticationMethod);
        }

        public async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
        {
            return await SignInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent);
        }

        public async Task<SignInResult> CheckPasswordSignInAsync(IdentityUser user, string password, bool lockoutOnFailure)
        {
            return await SignInManager.CheckPasswordSignInAsync((TUser)user, password, lockoutOnFailure);
        }

        public async Task<IdentityResult> CreateRoleAsync(IdentityRole role)
        {
            return await RoleManager.CreateAsync((TRole)role);
        }

        public async Task<IdentityResult> DeleteRoleAsync(IdentityRole role)
        {
            return await RoleManager.DeleteAsync((TRole)role);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await RoleManager.RoleExistsAsync(roleName);
        }

    }
}