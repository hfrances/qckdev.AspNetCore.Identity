using qckdev.AspNetCore.Identity.Test.xUnit.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.Controllers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace qckdev.AspNetCore.Identity.Test.xUnit
{
    public class UnitTest2
    {

        public UnitTest2()
        {
        }

        [Fact]
        public async Task CreateUser_And_Get()
        {
            using (var fixture = await ServiceProviderFixture<IdentityUser>.CreateAsync())
            {
                var controller = fixture.ServiceProvider.GetRequiredService<AuthController>();
                var request = new CreateUserCommand()
                {
                    UserName = "test",
                    Email = "test@miauth.com",
                    Password = "LaPruebaDelM1ll0n$"
                };
                await controller.CreateUserAsync(request);

                fixture.SetUserId(request.UserName);
                var user = await controller.GetAsync();

                Assert.True(user.Roles.Contains("User") && user.Roles.Contains("Guest"));
            }
        }

        [Fact]
        public async Task CreateUser_And_Login()
        {
            using (var fixture = await ServiceProviderFixture<IdentityUser>.CreateAsync())
            {
                var controller = fixture.ServiceProvider.GetRequiredService<AuthController>();
                var request = new CreateUserCommand()
                {
                    UserName = "test",
                    Email = "test@miauth.com",
                    Password = "LaPruebaDelM1ll0n$"
                };
                await controller.CreateUserAsync(request);

                var token = await controller.LoginAsync(new LoginCommand()
                {
                    Email = request.Email,
                    Password = request.Password
                });

                Assert.NotNull(token);
            }
        }

    }
}
