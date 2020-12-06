using MediatR;
using qckdev.AspNetCore.Identity.Infrastructure.Data;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Test.xUnit.Infrastructure.Data
{
    sealed class DataInitialization : IDataInitializer
    {

        IMediator Mediator { get; }
        IIdentityManager IdentityManager { get; }

        public DataInitialization(IIdentityManager identityManager, IMediator mediator)
        {
            this.IdentityManager = identityManager;
            this.Mediator = mediator;

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
