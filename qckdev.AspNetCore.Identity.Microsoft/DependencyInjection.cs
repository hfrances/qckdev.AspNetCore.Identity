using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class QIdentityMicrosoftDependencyInjection
    {

        public static AuthenticationBuilder AddMicrosoftAccount(this AuthenticationBuilder builder, Guid tenantId)
        {
            return builder.AddMicrosoftAccount(tenantId, options => { });
        }

        public static AuthenticationBuilder AddMicrosoftAccount(this AuthenticationBuilder builder, Guid tenantId, Action<MicrosoftAccountOptions> configureOptions)
        {
            return builder.AddMicrosoftAccount(options =>
            {
                SetTenantId(options, tenantId);
                configureOptions(options);
            });
        }

        public static AuthenticationBuilder AddMicrosoftAccount(this AuthenticationBuilder builder, string authenticationScheme, Guid tenantId, Action<MicrosoftAccountOptions> configureOptions)
        {
            return builder.AddMicrosoftAccount(authenticationScheme,
                options =>
                {
                    SetTenantId(options, tenantId);
                    configureOptions(options);
                }
            );
        }

        public static AuthenticationBuilder AddMicrosoftAccount(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Guid tenantId, Action<MicrosoftAccountOptions> configureOptions)
        {
            return builder.AddMicrosoftAccount(authenticationScheme, displayName,
                options =>
                {
                    SetTenantId(options, tenantId);
                    configureOptions(options);
                }
            );
        }


        private static void SetTenantId(this MicrosoftAccountOptions options, Guid tenantId)
        {
            if (tenantId == Guid.Empty)
            {
                SetTenantId(options, "common");
            }
            else
            {
                SetTenantId(options, tenantId.ToString());
            }
        }

        private static void SetTenantId(this MicrosoftAccountOptions options, string tenantId)
        {
            static string replaceTenantId(string endpoint, string tenantId)
            {
                var uri = new Uri(endpoint);
                var path = uri.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                path[0] = tenantId;
                uri = new Uri(new Uri(uri.GetLeftPart(UriPartial.Authority)), string.Join("/", path));
                return uri.ToString();
            }

            options.AuthorizationEndpoint = replaceTenantId(options.AuthorizationEndpoint, tenantId);
            options.TokenEndpoint = replaceTenantId(options.TokenEndpoint, tenantId);
            options.UserInformationEndpoint = replaceTenantId(options.UserInformationEndpoint, tenantId);
        }

        public static string GetTenantId(this MicrosoftAccountOptions options)
        {
            var uri = new Uri(options.AuthorizationEndpoint);
            var tenantId = uri.LocalPath
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault();

            return tenantId;
        }

    }
}
