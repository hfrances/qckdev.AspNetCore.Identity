using MediatR;
using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Handlers
{
    sealed class CreateRootUserCommandHandler : IRequestHandler<CreateRootUserCommand, Unit>
    {
        IServiceProvider Services { get; }

        public CreateRootUserCommandHandler(IServiceProvider services)
        {
            this.Services = services;
        }

        public async Task<Unit> Handle(CreateRootUserCommand request, CancellationToken cancellationToken)
        {
            await CreateUserHelper.CreateUser(this.Services, request);

            return Unit.Value;
        }

    }
}
