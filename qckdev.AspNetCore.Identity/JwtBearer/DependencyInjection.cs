using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using qckdev.AspNetCore.Identity.JwtBearer;
using System;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QIdentityJwtBearerDependencyInjection
    {

        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, JwtTokenConfiguration configuration)
        {
            return AddJwtBearer(builder, JwtBearerDefaults.AuthenticationScheme, configuration);
        }

        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder builder, string authenticationScheme, JwtTokenConfiguration configuration)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.Key));

            return builder.AddJwtBearer(authenticationScheme,
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
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


    }
}
