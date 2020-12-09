using qckdev.AspNetCore.Identity.Test.xUnit.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.Controllers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System;

namespace qckdev.AspNetCore.Identity.Test.xUnit
{
    public class UnitTest2
    {


        [Fact]
        public async Task CreateUser_And_Login_Get()
        {
            using (var fixture = await ServiceProviderFixture<IdentityUser>.CreateAsync())
            {
                var controller = fixture.ServiceProvider.GetRequiredService<AuthController>();

                // Create user.
                var request = new CreateUserCommand()
                {
                    UserName = "test",
                    Email = "test@miauth.loc",
                    Password = "LaPruebaDelM1ll0n$"
                };
                await controller.CreateUserAsync(request);

                // Login user.
                var token = await controller.LoginAsync(new LoginCommand()
                {
                    Email = request.Email,
                    Password = request.Password
                });

                // Get user data.
                fixture.SetAccessToken(token.AccessToken);
                var user = await controller.GetAsync();
                Assert.Contains("User", user.Roles);
            }
        }

        [Fact]
        public async Task CreateExternalUser_And_Login_Get()
        {
            using (var fixture = await ServiceProviderFixture<IdentityUser>.CreateAsync())
            {
                var controller = fixture.ServiceProvider.GetRequiredService<AuthController>();
                var loginRequest = new ExternalLoginCommand()
                {
                    Provider = Services.TestAuthenticationDefaults.AuthenticationScheme,
                    AccessCode = Services.TestAuthorizationFlow.CreateAccessCode("test@testflow.loc")
                };

                // External login.
                var token = await controller.ExternalLoginAsync(loginRequest);
                if (token is ViewModels.TokenToConfirmExternalUserViewModel confirmToken)
                {
                    var confirmRequest = new ConfirmExternalUserCommand()
                    {
                        NewUserData = confirmToken.NewUserData
                    };

                    // New external user confirmation.
                    fixture.SetAccessToken(token.AccessToken);
                    token = await controller.ConfirmExternalUserAsync(confirmRequest);

                    // Get user data.
                    fixture.SetAccessToken(token.AccessToken);
                    var user = await controller.GetAsync();

                    Assert.Contains("User", user.Roles);
                }
                else
                {
                    // User already exists.
                    throw new InvalidOperationException("Invalid token result.");
                }
            }
        }

    }
}
