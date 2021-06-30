using qckdev.AspNetCore.Identity.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.JwtBearer
{

    sealed class JwtRefreshTokenProvider<TUser> : IUserTwoFactorTokenProvider<TUser>
        where TUser : class
    {

        const string PROVIDER = "jwt";

        IAuthenticationService AuthenticationService { get; }

        public JwtRefreshTokenProvider(IAuthenticationService authenticationService)
        {
            this.AuthenticationService = authenticationService;
        }

        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            var tokenValue = JwtGenerator.CreateGenericToken();
            IdentityResult result;

            result = await manager.SetAuthenticationTokenAsync(user, PROVIDER, nameof(JwtRefreshTokenProvider<TUser>), tokenValue);
            if (result.Succeeded)
            {
                return tokenValue;
            }
            else
            {
                throw new IdentityException("Error creating refresh token.", result.Errors);
            }
        }

        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            string storedToken;

            storedToken = await manager.GetAuthenticationTokenAsync(user, PROVIDER, nameof(JwtRefreshTokenProvider<TUser>));
            return (storedToken != null && storedToken == token);
        }
    }
}
