using MediatR;

namespace qckdev.AspNetCore.Identity.Commands
{
    public class CreateUserCommand : IRequest<Unit>
    {

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
