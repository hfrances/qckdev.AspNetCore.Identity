using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace qckdev.AspNetCore.Identity.Helpers
{
    public interface ICreateUserArgs
    {

        public IdentityUser User { get; }

        public IEnumerable<string> Roles { get; set; }

    }
}
