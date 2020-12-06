using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Policies
{

    // https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-3.1
    // https://geeklearning.io/create-your-own-authorization-requirements-in-asp-net-core/

    sealed class GuestAuthorizationHandler : AuthorizationHandler<GuestAuthorizationRequirement>
    {

        public const string POLICY_NAME = "Guest";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GuestAuthorizationRequirement requirement)
        {
            var endpointMetadata = (context.Resource as RouteEndpoint)?.Metadata;
            var hasGuestPolicy = endpointMetadata
                .GetMetadata<AuthorizeAttribute>()
                ?.Policy
                ?.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                ?.Any(x => x.Equals(requirement.PolicyName, StringComparison.CurrentCultureIgnoreCase))
                ?? false;
            var isGuest = context.User.HasClaim(
                c => c.Type == ClaimTypes.Role && c.Value == requirement.PolicyName
                /*&& c.Issuer == "http://microsoftsecurity"*/);

            if (requirement is AllowGuestAuthorizationRequirement)
            {
                if (hasGuestPolicy || !isGuest)
                {
                    context.Succeed(requirement);
                }
            }
            else if (requirement is DenyGuestAuthorizationRequirement)
            {
                if (hasGuestPolicy == isGuest)
                {
                    context.Succeed(requirement);
                }
            }
            else
            {
                throw new NotSupportedException(
                    $"Unsupported {nameof(GuestAuthorizationRequirement)} of type '{requirement?.ToString() ?? null}'");
            }
            return Task.CompletedTask;
        }
    }
}
