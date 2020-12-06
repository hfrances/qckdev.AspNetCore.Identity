using MediatR;
using qckdev.AspNetCore.Identity.ViewModels;

namespace qckdev.AspNetCore.Identity.Queries
{
    class GetCurrentUserQuery : IRequest<UserViewModel>
    {
    }
}
