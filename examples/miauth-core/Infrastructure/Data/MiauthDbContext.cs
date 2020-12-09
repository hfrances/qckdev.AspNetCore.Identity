using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using qckdev.AspNetCore.Identity.Infrastructure.Data;

namespace miauthcore.Infrastructure.Data
{
    public class MiauthDbContext<TUser> : ApplicationDbContext<TUser>
        where TUser : IdentityUser
    {

        public MiauthDbContext(DbContextOptions options) : base(options) { }

    }
}
