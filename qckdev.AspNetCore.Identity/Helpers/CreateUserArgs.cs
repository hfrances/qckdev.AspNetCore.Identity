using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace qckdev.AspNetCore.Identity.Helpers
{
    public class CreateUserArgs<TUser> : ICreateUserArgs
        where TUser : IdentityUser
    {

        public TUser User { get; }
        public IEnumerable<string> Roles { get; set; }

        IdentityUser ICreateUserArgs.User { get => this.User; }

        internal CreateUserArgs(TUser user, IEnumerable<string> roles)
        {
            this.User = user;
            this.Roles = roles;
        }

    }
}
