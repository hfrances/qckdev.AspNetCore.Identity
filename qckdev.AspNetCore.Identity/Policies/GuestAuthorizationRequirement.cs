using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace qckdev.AspNetCore.Identity.Policies
{
    abstract class GuestAuthorizationRequirement : IAuthorizationRequirement
    {

        public string PolicyName { get; }

        protected GuestAuthorizationRequirement(string policyName)
        {
            this.PolicyName = policyName;
        }

    }
}
