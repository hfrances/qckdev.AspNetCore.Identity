using Microsoft.AspNetCore.Identity;
using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.Helpers;
using qckdev.AspNetCore.Identity.Services;

namespace miauthcore.Infrastructure.Data.DataInitializer
{
    public class CreateRootUserCustomization<TUser> :
        CustomizableAction<CreateRootUserCommand, CreateUserArgs<TUser>>
        where TUser : IdentityUser
    {

        public override void Customize(CreateRootUserCommand request, CreateUserArgs<TUser> value)
        {
            value.User.EmailConfirmed = true;
            value.Roles = new string[] { "Root" };
        }

    }

}
