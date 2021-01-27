using qckdev.AspNetCore.Identity.Services;
using qckdev.AspNetCore.Identity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using qckdev.Linq;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace qckdev.AspNetCore.Identity.Helpers
{
    static class UserHelper
    {
        
        public static async Task<TokenViewModel> CreateToken(IServiceProvider services, IdentityUser user, string schemeName = JwtBearerDefaults.AuthenticationScheme)
        {
            var identityManager = services.GetService<IIdentityManager>();
            var userRoles = await identityManager.GetRolesAsync(user);

            return await CreateToken(services, user, userRoles, null, schemeName);
        }

        public static async Task<TokenViewModel> CreateToken(IServiceProvider services, IdentityUser user, IEnumerable<string> roles, IEnumerable<Claim> claims = null, string schemeName = JwtBearerDefaults.AuthenticationScheme)
        {
            var jwtBearerOptions = services
                .GetService<IOptionsMonitor<JwtBearerOptions>>()
                ?.Get(schemeName);
            var jwtBearerMoreOptions = services
                .GetService<IOptionsMonitor<JwtBearerMoreOptions>>()
                ?.Get(schemeName);
            var issuerSigningKey = jwtBearerOptions?.TokenValidationParameters.IssuerSigningKey;
            var token = JwtGenerator.CreateToken(
                issuerSigningKey, user, roles, claims,
                jwtBearerMoreOptions?.TokenLifeTimespan
            );
            /*
             var identityManager = services.GetService<IIdentityManager>();
             var refreshToken =
                await identityManager.GenerateUserTokenAsync(
                    user, nameof(JwtRefreshTokenProvider<IdentityUser>), "Refresh");
             var refreshTokenGood = 
                await identityManager.VerifyUserTokenAsync(
                    user, nameof(JwtRefreshTokenProvider<IdentityUser>), null, refreshToken);*/

            return new TokenViewModel()
            {
                AccessToken = token.AccessToken,
                Expired = token.Expired,
                RefreshToken = token.RefreshToken
            };
        }

        public static dynamic GetUserData(IdentityUser user)
        {
            var addToUserData = new Action<dynamic, string, object>((item, key, value) =>
                ((IDictionary<string, object>)item).Add(key, value));
            var genericUserProperties = typeof(IdentityUser).GetProperties();
            var customUserProperties = user.GetType().GetProperties();
            var userDataProperties = customUserProperties
                .LeftJoin(
                    genericUserProperties,
                    x => x.Name, y => y.Name,
                    (x, y) => new { Item = x, IsNew = (y == null) })
                .Where(z => z.IsNew || z.Item.Name == nameof(user.UserName))
                .Select(z => z.Item);
            dynamic userData = new System.Dynamic.ExpandoObject();

            // TODO: la lista de propiedades debería venir dada en el Setup.
            foreach (var prop in userDataProperties)
            {
                var name = prop.Name.Substring(0, 1).ToLower() + prop.Name.Substring(1);

                addToUserData(userData, name, prop.GetValue(user));
            }
            return userData;
        }

        public static void SetUserData(IdentityUser user, dynamic userData)
        {
            var customUserProperties = user.GetType().GetProperties();
            var userDataProperties = userData.GetType().GetProperties();

            foreach (var prop in userDataProperties)
            {
                customUserProperties
                    .FirstOrDefault(x => x.Name == prop.Name)
                    ?.SetValue(user, prop.GetValue(userData));
            }
        }

    }
}
