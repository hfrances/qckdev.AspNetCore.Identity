using MediatR;
using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Handlers
{
    sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Unit>
    {
        IServiceProvider Services { get; }

        public CreateUserCommandHandler(IServiceProvider services)
        {
            this.Services = services;
        }

        public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            await CreateUserHelper.CreateUser(this.Services, request);

            return Unit.Value;
        }

    }
}
