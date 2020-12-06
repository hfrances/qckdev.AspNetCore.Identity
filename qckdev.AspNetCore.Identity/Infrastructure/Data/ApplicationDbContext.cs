using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace qckdev.AspNetCore.Identity.Infrastructure.Data
{
    public class ApplicationDbContext<TUser> : IdentityDbContext<TUser>, IApplicationDbContext
        where TUser : IdentityUser
    {

        public ApplicationDbContext(DbContextOptions options) : base(options) { }

    }
}
