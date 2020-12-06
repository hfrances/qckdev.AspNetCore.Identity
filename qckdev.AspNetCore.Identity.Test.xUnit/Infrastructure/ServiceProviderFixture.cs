using qckdev.AspNetCore.Identity.Test.xUnit.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.Controllers;
using qckdev.AspNetCore.Identity.Helpers;
using qckdev.AspNetCore.Identity.Infrastructure;
using qckdev.AspNetCore.Identity.Infrastructure.Data;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Test.xUnit.Infrastructure
{
    class ServiceProviderFixture<TIdentityUser> : IDisposable
        where TIdentityUser : IdentityUser, new()
    {

        public string DatabaseName { get; }
        public Services.HttpContextAccessor HttpContextAccessor { get; }
        public ServiceProvider ServiceProvider { get; private set; }

        CurrentSessionService CurrentSessionService { get; }

        private ServiceProviderFixture()
        {
            this.HttpContextAccessor = new Services.HttpContextAccessor();
            this.CurrentSessionService = new CurrentSessionService();
            this.DatabaseName = Guid.NewGuid().ToString();
        }

        protected virtual async Task OnCreatingAsync()
        {
            var services = new ServiceCollection();

            services
                    .AddApplication()
                    .AddInfrastructure<TIdentityUser>(options =>
                        options.UseInMemoryDatabase(this.DatabaseName)
                    )
                    .AddDataInitializer<Data.DataInitialization>()
                    .AddAuthentication()
                        .AddJwtBearer(
                            new JwtTokenConfiguration() { Key = Guid.NewGuid().ToString(), AccessExpireSeconds = 300 }
                        );
            services
                .CustomizeAction<CreateUserCommand, CreateUserArgs<TIdentityUser>>(
                        (request, args) =>
                        {
                            args.User.EmailConfirmed = true;
                            args.Roles = new string[] { "User", "Guest" };
                        })
                ;

            // Para TESTING.
            services.AddSingleton<ICurrentSessionService>(this.CurrentSessionService);
            services.AddTransient<AuthController>();

            this.ServiceProvider = services.BuildServiceProvider();
            foreach (var initializer in ServiceProvider.GetServices<IDataInitializer>())
            {
                await initializer.InitializeAsync(default);
            }
        }

        protected virtual async Task OnCreatedAsync()
        {
            await Task.Run(() =>
            {
                //MakeActiveOptions<JwtBearerOptions>(this.ServiceProvider, JwtBearerDefaults.AuthenticationScheme);
                //MakeActiveOptions<JwtBearerMoreOptions>(this.ServiceProvider, JwtBearerDefaults.AuthenticationScheme);
            });
        }

        protected void MakeActiveOptions<TOptions>(IServiceProvider services, string name) where TOptions : class
        {
            var options = services.GetService<IOptionsMonitor<TOptions>>();
            var cache = services.GetService<IOptionsMonitorCache<TOptions>>();

            cache.TryRemove(Options.DefaultName);
            cache.TryAdd(Options.DefaultName, options.Get(name));
        }

        public void SetUserId(string value)
        {
            CurrentSessionService.SetUserId(value);
        }

        public void Dispose()
        {
            ServiceProvider
                .GetService<IApplicationDbContext>()
                .Dispose();
        }

        public static async Task<ServiceProviderFixture<TIdentityUser>> CreateAsync()
        {
            var fixture = new ServiceProviderFixture<TIdentityUser>();

            await fixture.OnCreatingAsync();
            await fixture.OnCreatedAsync();
            return fixture;
        }

    }
}
