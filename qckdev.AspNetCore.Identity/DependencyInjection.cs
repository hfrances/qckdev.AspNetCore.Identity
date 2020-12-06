using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using qckdev.AspNetCore.Identity.Policies;
using qckdev.AspNetCore.Identity.Services;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace qckdev.AspNetCore.Identity
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services
                .AddMediatR(Assembly.GetExecutingAssembly())
                .TryAddSingleton<ISystemClock, SystemClock>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddScoped<ICurrentSessionService, CurrentSessionService>()
                .AddGuestAuthorization(Constants.EXTERNALCONFIRMATION_POLICY)
                .AddGuestAuthorization(GuestAuthorizationHandler.POLICY_NAME);
        }

        public static IServiceCollection TryAddSingleton<TService, TImplementation>(this IServiceCollection collection)
            where TService : class
            where TImplementation : class, TService
        {
            collection.TryAddSingleton(typeof(TService), typeof(TImplementation));
            return collection;
        }

        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, JwtTokenConfiguration configuration)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.Key));

            return builder.AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                },
                moreOptions =>
                {
                    moreOptions.TokenLifeTimespan = TimeSpan.FromSeconds(configuration.AccessExpireSeconds);
                    moreOptions.ClientId = configuration.ClientId;
                }
            );
        }

        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, Action<JwtBearerOptions> configureOptions, Action<JwtBearerMoreOptions> configureMoreOptions)
        {
            return AddJwtBearer(builder, JwtBearerDefaults.AuthenticationScheme, configureOptions, configureMoreOptions);
        }
        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, string authenticationScheme, Action<JwtBearerOptions> configureOptions, Action<JwtBearerMoreOptions> configureMoreOptions)
        {
            return AddJwtBearer(builder, authenticationScheme, null, configureOptions, configureMoreOptions);
        }

        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<JwtBearerOptions> configureOptions, Action<JwtBearerMoreOptions> configureMoreOptions)
        {
            builder.Services.Configure(authenticationScheme, configureMoreOptions);
            return builder.AddJwtBearer(authenticationScheme, displayName, configureOptions);
        }

        public static IdentityBuilder AddJwtRefreshTokenProvider(this IdentityBuilder builder)
        {
            var userType = builder.UserType;
            var provider = typeof(JwtRefreshTokenProvider<>).MakeGenericType(userType);

            return builder.AddTokenProvider(nameof(JwtRefreshTokenProvider<IdentityUser>), provider);
        }

        public static IServiceCollection AddGuestAuthorization(this IServiceCollection services,
            string policyName)
        {
            services
                .TryAddSingleton<IAuthorizationHandler, GuestAuthorizationHandler>();
            services
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

        public static AuthorizationPolicy Clone(this AuthorizationPolicy policy, params IAuthorizationRequirement[] additionalRequirements)
        {
            var authorizationRequirements =
                policy.Requirements.Union(additionalRequirements);
            var authenticationSchemes =
                policy.AuthenticationSchemes;

            return new AuthorizationPolicy(
               authorizationRequirements, authenticationSchemes);
        }

        public static IServiceCollection CustomizeAction<TParameter, TValue>(this IServiceCollection services, Action<TParameter, TValue> action)
        {
            return services.CustomizeAction(new GenericCustomizableAction<TParameter, TValue>(action));
        }

        public static IServiceCollection CustomizeAction<TParameter, TValue>(this IServiceCollection services, ICustomizableAction<TParameter, TValue> action)
        {
            return services.AddSingleton(action);
        }

    }
}
