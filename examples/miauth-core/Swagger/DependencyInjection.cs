using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace miauthcore.Swagger
{
    public static class DependencyInjection
    {

        /// <remarks>
        /// https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.1&tabs=visual-studio
        /// https://stackoverflow.com/questions/58834430/c-sharp-net-core-swagger-trying-to-use-multiple-api-versions-but-all-end-point
        /// https://stackoverflow.com/questions/56678763/different-docinclusionpredicate-for-different-swagger-document-with-swashbuckle
        /// </remarks>
        public static void AddSwagger(this IServiceCollection services)
        {

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Default API",
                    Description = "ASP.NET Core Web API 3.1",
                    Contact = new OpenApiContact
                    {
                        Name = "Héctor Francés",
                        Email = "trancos_agent@hotmail.com",
                    },
                });

                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "Other API"
                });

                c.DocInclusionPredicate((name, description) =>
                {
                    bool rdo;

                    switch (name)
                    {
                        case "v1":
                            rdo = true;
                            break;
                        case "v2":
                            rdo = false;
                            break;
                        default:
                            rdo = false;
                            break;
                    }
                    return rdo;
                });

                //c.AddMiauthSecurityDefinition();
                //c.AddMicrosoftSecurityDefinition();
                //c.AddGoogleSecurityDefinition();
                //c.AddBasicSecurityDefinition();
                c.AddJwtSecurityDefinition();

                // Set the comments path for the Swagger JSON and UI.
                /*var xmlFile = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".xml");
                if (File.Exists(xmlFile))
                {
                    c.IncludeXmlComments(xmlFile);
                }*/
            });
        }

        public static void UseSwagger(this IApplicationBuilder app)
        {

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(null);

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Default V1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Other V2");
                c.RoutePrefix = string.Empty;

                c.OAuthAppName("Mi Auth");
                //c.OAuthClientId("GO");
                //c.OAuthClientId("F6E7206D-9B26-4992-A3F2-1546F03F108A");
                //c.OAuthClientId("903c9810-7b80-44cb-965c-81304a9a2812");
                //c.OAuthClientSecret("VN2y7ts.UFI-7Os__t.9puU_jJXH0ZseSH");
                c.OAuthUsePkce();
            });
        }

        private static void AddMiauthSecurityDefinition(this SwaggerGenOptions c)
        {

            c.AddSecurityDefinition("miauth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Description = "Miauth authentication",
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"/api/swagger/msal/authorize", UriKind.Relative),
                        //TokenUrl = new Uri($"/api/swagger/msal/token", UriKind.Relative),
                        Scopes = new Dictionary<string, string>
                        {
                            ["User.Read"] = "Read user",
                            ["https://graph.microsoft.com/mail.read"] = "Read mail"
                        },
                    },
                }
            }); ; ; ;

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "miauth"
                            }
                        },
                        new string[] { }
                    }
            });
        }

        private static void AddMicrosoftSecurityDefinition(this SwaggerGenOptions c)
        {
            const string baseUrl = "https://login.microsoftonline.com";
            const string tenantId = "df50f6b0-9284-40db-9be0-0b15101bd980";

            // https://thecodebuzz.com/oauth2-authorize-ioperationfilter-swaggeropenapi-asp-net-core/
            // https://stackoverflow.com/questions/58589312/net-core-web-api-azure-ad-and-swagger-not-authenticating
            // https://www.taithienbo.com/configure-oauth2-implicit-flow-for-swagger-ui/
            c.AddSecurityDefinition("oauth2-msal", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                In = ParameterLocation.Cookie,
                Description = "Microsoft authentication",
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{baseUrl}/{tenantId}/oauth2/authorize"),
                        //TokenUrl = new Uri($"{baseUrl}/{tenantId}/oauth2/token"),
                        TokenUrl = new Uri($"/api/swagger/msal/{tenantId}/token", UriKind.Relative),
                        Scopes = new Dictionary<string, string>
                        {
                            ["User.Read"] = "Read user",
                            ["https://graph.microsoft.com/mail.read"] = "Read mail"
                        },
                    },
                }
            });; ; ;

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2-msal"
                            }
                        },
                        new string[] { }
                    }
            });
        }

        private static void AddGoogleSecurityDefinition(this SwaggerGenOptions c)
        {
            // https://stackoverflow.com/questions/56982278/swagger-with-google-oauth-2-0-authorization
            c.AddSecurityDefinition("oauth2-gapi", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                        Scopes = new Dictionary<string, string>
                            {
                                { "readAccess", "Access read operations" },
                                { "writeAccess", "Access write operations" }
                            }
                    }
                },
                Description = "Google authentication"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2-gapi"
                        }
                    },
                    new string[] { }
                }
            });
        }

        private static void AddBasicSecurityDefinition(this SwaggerGenOptions c)
        {
            // https://thecodebuzz.com/basic-authentication-swagger-asp-net-core-3-0/
            c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                In = ParameterLocation.Header,
                Description = "Basic Authorization header using the Bearer scheme."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "basic"
                        }
                    },
                    new string[] { }
                }
            });
        }

        private static void AddJwtSecurityDefinition(this SwaggerGenOptions c)
        {
            // Configure swagger to accept JWT
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Bearer",
                BearerFormat = "JWT",
                Scheme = "bearer",
                Description = "Specify the authorization token.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        }
    }
}
