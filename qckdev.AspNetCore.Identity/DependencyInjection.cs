using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using qckdev.AspNetCore.Identity;
using qckdev.AspNetCore.Identity.Policies;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QIdentityDependencyInjection
    {

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services
                .TryAddSingleton<ISystemClock, SystemClock>()
                .TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddScoped<ICurrentSessionService, CurrentSessionService>()
                .AddGuestAuthorization(Constants.EXTERNALCONFIRMATION_POLICY)
                .AddGuestAuthorization(GuestAuthorizationHandler.POLICY_NAME)
            ;
        }

        public static IServiceCollection AddGuestAuthorization(this IServiceCollection services, string policyName)
        {
            services
                .TryAddSingleton<IAuthorizationHandler, GuestAuthorizationHandler>()
                .AddAuthorization(options =>
                {
                    options.DefaultPolicy =
                        options.DefaultPolicy.Clone(additionalRequirements:
                            new DenyGuestAuthorizationRequirement(policyName)
                        );

                    options.AddPolicy(policyName, policy =>
                        policy.AddRequirements(new AllowGuestAuthorizationRequirement(policyName)));
                });
            return services;
        }

        /// <summary>
        /// Disables cross-origin resource sharing services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <returns>
        /// The <see cref="IServiceCollection"/> so that additional calls can be chained.
        /// </returns>
        public static IServiceCollection AddDisableCors(this IServiceCollection services)
        {
            return services.AddCors(opt => opt.AddPolicy(Constants.DISABLE_CORS_POLICY, builder =>
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            ));
        }

        /// <summary>
        /// Adds a CORS middleware to your web application pipeline to allow all cross domain requests.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> passed to your Configure method.</param>
        /// <returns>The original <paramref name="app"/> parameter</returns>
        public static IApplicationBuilder UseDisableCors(this IApplicationBuilder app)
        {
            return app.UseCors(Constants.DISABLE_CORS_POLICY);
        }

        /// <summary>
        /// Returns a new <see cref="AuthorizationPolicy"/> with the same configuration that the original <paramref name="policy"/> and the specified additional <see cref="IAuthorizationRequirement"/>.
        /// </summary>
        /// <param name="policy">The original policy</param>
        /// <param name="additionalRequirements">An array with the additional <see cref="IAuthorizationRequirement"/>.</param>
        /// <returns></returns>
        public static AuthorizationPolicy Clone(this AuthorizationPolicy policy, params IAuthorizationRequirement[] additionalRequirements)
        {
            var authorizationRequirements =
                policy.Requirements.Union(additionalRequirements);
            var authenticationSchemes =
                policy.AuthenticationSchemes;

            return new AuthorizationPolicy(
               authorizationRequirements, authenticationSchemes);
        }

        /// <summary>
        /// Returns the <see cref="IServiceCollection"/> contained in the current <see cref="AuthenticationBuilder"/> object.
        /// </summary>
        /// <param name="builder">Current object</param>
        /// <returns></returns>
        public static IServiceCollection Up(this AuthenticationBuilder builder)
        {
            return builder.Services;
        }

        /// <summary>
        /// Returns the <see cref="IServiceCollection"/> contained in the current <see cref="IdentityBuilder"/> object.
        /// </summary>
        /// <param name="builder">Current object</param>
        /// <returns></returns>
        public static IServiceCollection Up(this IdentityBuilder builder)
        {
            return builder.Services;
        }

    }
}
