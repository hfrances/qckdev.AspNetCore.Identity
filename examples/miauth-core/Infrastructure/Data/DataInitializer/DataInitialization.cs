using MediatR;
using Microsoft.Extensions.Configuration;
using qckdev.AspNetCore.Identity.Infrastructure.Data;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace miauthcore.Infrastructure.Data.DataInitializer
{
    public class DataInitialization : IDataInitializer
    {

        IMediator Mediator { get; }
        IServiceProvider Services { get; }
        IIdentityManager IdentityManager { get; }
        IConfiguration Configuration { get; }

        public DataInitialization(
            IMediator mediator,
            IServiceProvider services,
            IIdentityManager identityManager,
            IConfiguration configuration)
        {
            this.Mediator = mediator;
            this.Services = services;
            this.IdentityManager = identityManager;
            this.Configuration = configuration;
        }


        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            await SeedRolesAsync();
        }

        private async Task SeedRolesAsync()
        {
            var defaultRoles = new string[] { "Root", "User", "Guest" };

            foreach (var roleName in defaultRoles)
            {
                if (!await IdentityManager.RoleExistsAsync(roleName))
                {
                    var role = IdentityManager.CreateInstanceRole();

                    role.Name = roleName;
                    await IdentityManager.CreateRoleAsync(role);
                }
            }
        }

    }
}
