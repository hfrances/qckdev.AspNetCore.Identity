using qckdev.AspNetCore.Identity.Test.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System;
using qckdev.AspNetCore.Identity.Services;

namespace qckdev.AspNetCore.Identity.Test
{
    public class UnitTest2
    {


        [Fact]
        public async Task CreateUser_And_CheckPassword()
        {
            using (var fixture = await ServiceProviderFixture<IdentityUser>.CreateAsync())
            {
                const string PASSWORD = "LaPruebaDelM1ll0n$";
                const string USERROLE = "User";
                var identityManager = fixture.ServiceProvider.GetRequiredService<IIdentityManager>();

                // Create user.
                var request = identityManager.CreateInstanceUser();
                request.UserName = "test";
                request.Email = "test@miauth.loc";
                var result = await identityManager.CreateUserAsync(request, PASSWORD);
                if (result.Succeeded)
                {
                    await identityManager.AddToRoleAsync(request, USERROLE);

                    // Login user.
                    var user = await identityManager.FindByNameAsync(request.UserName);
                    var signInResult = await identityManager.CheckPasswordSignInAsync(user, PASSWORD, lockoutOnFailure: true);
                    if (signInResult.Succeeded)
                    {
                        Assert.Contains(USERROLE, await identityManager.GetRolesAsync(user));
                    }
                    else
                    {
                        throw new Exception("Login failed.");
                    }
                }
                else
                {
                    throw new Exception("Create user failed.");
                }
            }
        }

    }
}
