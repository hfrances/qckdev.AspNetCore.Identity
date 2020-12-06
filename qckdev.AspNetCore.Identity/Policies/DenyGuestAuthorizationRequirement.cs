using System;
using System.Collections.Generic;
using System.Text;

namespace qckdev.AspNetCore.Identity.Policies
{
    sealed class DenyGuestAuthorizationRequirement : GuestAuthorizationRequirement
    {

        public DenyGuestAuthorizationRequirement(string policyName) 
            : base(policyName) { }

    }
}
