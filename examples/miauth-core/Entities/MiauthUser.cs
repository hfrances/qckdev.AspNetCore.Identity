
namespace miauthcore.Entities
{
    public class MiauthUser : Microsoft.AspNetCore.Identity.IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
