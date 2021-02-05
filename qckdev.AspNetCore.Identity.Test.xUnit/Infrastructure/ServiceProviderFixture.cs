using qckdev.AspNetCore.Identity.Test.xUnit.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using qckdev.AspNetCore.Identity.Infrastructure.Data;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.Threading.Tasks;
using qckdev.AspNetCore.Identity.JwtBearer;
using MediatR;

namespace qckdev.AspNetCore.Identity.Test.xUnit.Infrastructure
{
    class ServiceProviderFixture<TIdentityUser> : IDisposable
        where TIdentityUser : IdentityUser, new()
    {

        public string DatabaseName { get; }
        public ServiceProvider ServiceProvider { get; private set; }


        private ServiceProviderFixture()
        {
            this.DatabaseName = Guid.NewGuid().ToString();
        }

        protected virtual async Task OnCreatingAsync()
        {
            var services = new ServiceCollection();

            services
                .AddApplication()
                .AddMediatR(System.Reflection.Assembly.GetExecutingAssembly())
                .AddInfrastructure<TIdentityUser>(options =>
                    options.UseInMemoryDatabase(this.DatabaseName)
                )
                .AddDataInitializer<Data.DataInitialization>()
                .AddAuthentication()
                    .AddJwtBearer(
                        new JwtTokenConfiguration() { Key = Guid.NewGuid().ToString(), AccessExpireSeconds = 300 }
                    )
                    .AddTest()
                    .AddTestAuthorizationFlow()
                    .Up()
            ;

            // For TESTING.
            services
                .AddSingleton<ICurrentSessionService, TestCurrentSessionService>()
            ;

            this.ServiceProvider = services.BuildServiceProvider();
            foreach (var initializer in ServiceProvider.GetServices<IDataInitializer>())
            {
                await initializer.InitializeAsync(default);
            }
        }

        protected virtual async Task OnCreatedAsync()
        {
            
        }

        protected void MakeActiveOptions<TOptions>(IServiceProvider services, string name) where TOptions : class
        {
            var options = services.GetService<IOptionsMonitor<TOptions>>();
            var cache = services.GetService<IOptionsMonitorCache<TOptions>>();

            cache.TryRemove(Options.DefaultName);
            cache.TryAdd(Options.DefaultName, options.Get(name));
        }

        public void SetAccessToken(string value)
        {
            GetCurrentSessionService().SetAccessToken(value);
        }

        private TestCurrentSessionService GetCurrentSessionService()
        {
            return (TestCurrentSessionService)ServiceProvider.GetService<ICurrentSessionService>();
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
