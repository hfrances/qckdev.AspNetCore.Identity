using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace qckdev.AspNetCore.Identity.Helpers
{
    sealed class PendingToConfirmExternalUser
    {

        public Guid RequestId { get; set; }
        public DateTime RequestDate { get; set; }
        public IdentityUser User { get; set; }
        public UserLoginInfo UserLoginInfo { get; set; }
        public IEnumerable<string> Roles { get; set; }

    }
}
