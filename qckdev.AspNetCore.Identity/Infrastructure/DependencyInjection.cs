using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using qckdev.AspNetCore.Identity.Infrastructure.Data;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure<TUser>(
                this IServiceCollection services,
                Action<DbContextOptionsBuilder> contextOptions = null,
                Action<IdentityOptions> identityOptions = null)
            where TUser : IdentityUser, new()
        {
            return AddInfrastructure<TUser, IdentityRole, ApplicationDbContext<TUser>>(
                services, contextOptions, identityOptions);
        }

        public static IServiceCollection AddInfrastructure<TUser, TApplicationDbContext>(
                this IServiceCollection services,
                Action<DbContextOptionsBuilder> contextOptions = null,
                Action<IdentityOptions> identityOptions = null)
            where TUser : IdentityUser, new()
            where TApplicationDbContext : DbContext, IApplicationDbContext
        {
            return AddInfrastructure<TUser, IdentityRole, TApplicationDbContext>(
                services, contextOptions, identityOptions);
        }

        public static IServiceCollection AddInfrastructure<TUser, TRole, TApplicationDbContext>(
                this IServiceCollection services,
                Action<DbContextOptionsBuilder> contextOptions = null,
                Action<IdentityOptions> identityOptions = null)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
            where TApplicationDbContext : DbContext, IApplicationDbContext
        {
            return AddInfrastructure<TUser, TRole, TApplicationDbContext, SignInManager<TUser>>(
                services, contextOptions, identityOptions);
        }

        public static IServiceCollection AddInfrastructure<TUser, TRole, TApplicationDbContext, TSignInManager>(
                this IServiceCollection services,
                Action<DbContextOptionsBuilder> contextOptions = null,
                Action<IdentityOptions> identityOptions = null)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
            where TSignInManager : SignInManager<TUser>
            where TApplicationDbContext : DbContext, IApplicationDbContext
        {
            services
                .AddDbContext<TApplicationDbContext>(contextOptions)
                .AddScoped<IApplicationDbContext>(
                    provider => provider.GetService<TApplicationDbContext>())
                .AddIdentityCore<TUser>(identityOptions)
                    .AddRoles<TRole>()
                    .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<TUser, TRole>>()
                    .AddEntityFrameworkStores<TApplicationDbContext>()
                    .AddSignInManager<TSignInManager>()
                    .AddDefaultTokenProviders()
                    .AddJwtRefreshTokenProvider()
                    .Up()
                .AddScoped<IIdentityManager, IdentityManager<TUser, TRole>>();

            return services;
        }


        public static IServiceCollection AddDataInitializer<TDataInitializer>(this IServiceCollection services)
            where TDataInitializer : class, IDataInitializer
        {
            return services.AddScoped<IDataInitializer, TDataInitializer>();
        }

        public static void DataInitialization(this IApplicationBuilder app)
        {

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var tasks = new List<Task>();
                var dataInitializers = scope
                    .ServiceProvider
                           .GetServices<IDataInitializer>();

                foreach (var initializer in dataInitializers)
                {
                    tasks.Add(
                        initializer.InitializeAsync(CancellationToken.None)
                    );
                }
                Task.WhenAll(tasks);
            }
        }

    }
}
