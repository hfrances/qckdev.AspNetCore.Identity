using System;
using System.Collections.Generic;
using System.Text;

namespace qckdev.AspNetCore.Identity.Policies
{
    sealed class AllowGuestAuthorizationRequirement : GuestAuthorizationRequirement
    {

        public AllowGuestAuthorizationRequirement(string policyName)
            : base(policyName) { }

    }
}
