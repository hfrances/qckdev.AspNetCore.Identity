using System.Collections.Generic;

namespace qckdev.AspNetCore.Identity.ViewModels
{
    public sealed class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
