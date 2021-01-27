using miauthcore.Entities;
using miauthcore.Infrastructure.Data;
using miauthcore.Infrastructure.Data.DataInitializer;
using miauthcore.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using qckdev.AspNetCore.Identity;
using qckdev.AspNetCore.Identity.Middleware;
using System;

namespace miauthcore
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureService<MiauthUser>(services, this.Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("NoCors");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<HandlerExceptionMiddleware>();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            app.DataInitialization();

        }


        private static void ConfigureService<TUser>(IServiceCollection services, IConfiguration configuration) where TUser : IdentityUser, new()
        {
            var jwtTokenConfiguration = JwtTokenConfiguration.Get(configuration, "Tokens");
            var googleConfiguration = new
            {
                ClientId = configuration["Authentication:Google:ClientId"],
                ClientSecret = configuration["Authentication:Google:ClientSecret"]
            };
            var microsoftConfiguration = new
            {
                TenantId = Guid.Parse(configuration["Authentication:Microsoft:TenantId"]),
                ClientId = configuration["Authentication:Microsoft:ClientId"],
                ClientSecret = configuration["Authentication:Microsoft:ClientSecret"]
            };

            //Cors Policy
            services.AddCors(opt => opt.AddPolicy("NoCors", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            services
                .AddApplication()
                .AddInfrastructure<TUser, MiauthDbContext<TUser>>(options =>
                    options.UseInMemoryDatabase("miauth")
                //  options.UseMySql(Configuration.GetConnectionString("DefaultConnection"),
                //      b => b.MigrationsAssembly(typeof(ApplicationDbContext<TIdentityUser>).Assembly.FullName)
                )
                .AddDataInitializer<DataInitialization>()
            ;

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtTokenConfiguration)
                .AddGoogle(options =>
                {
                    options.ClientId = googleConfiguration.ClientId;
                    options.ClientSecret = googleConfiguration.ClientSecret;
                })
                .AddGoogleAuthorizationFlow()
                .AddMicrosoftAccount("MSAL", microsoftConfiguration.TenantId,
                    options =>
                    {
                        options.ClientId = microsoftConfiguration.ClientId;
                        options.ClientSecret = microsoftConfiguration.ClientSecret;
                    }
                )
                .AddMicrosoftAuthorizationFlow()
            ;

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 0;
            });

            services.AddSwagger();
            services.AddControllers();
        }

    }
}
