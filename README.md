# qckdev.AspNetCore.Identity
 
```cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using qckdev.AspNetCore.Identity;
using qckdev.AspNetCore.Identity.Middleware;

public void ConfigureServices(IServiceCollection services)
{
    var jwtTokenConfiguration = JwtTokenConfiguration.Get(configuration, "Tokens");

    services
        .AddApplication()
        .AddInfrastructure<TUser, MiauthDbContext<TUser>>(options =>
            options.UseInMemoryDatabase("miauth")
        )
        .AddDataInitializer<DataInitialization>()
    ;

    services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(jwtTokenConfiguration)
        .AddGoogle(options =>
        {
            options.ClientId = configuration["Authentication:Google:ClientId"];
            options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
        })
	.AddGoogleAuthorizationFlow()
        .AddMicrosoftAccount("MSAL", Guid.Parse(configuration["Authentication:Microsoft:TenantId"]),
            options =>
            {
                options.ClientId = configuration["Authentication:Microsoft:ClientId"];
                options.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
            }
        )
        .AddMicrosoftAuthorizationFlow()
    ;

    services.AddControllers();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    (...)

    app.UseMiddleware<HandlerExceptionMiddleware>();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    (...)

    app.DataInitialization();
}
```

```cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using qckdev.AspNetCore.Identity.Infrastructure.Data;

public class MiauthDbContext<TUser> : ApplicationDbContext<TUser>
    where TUser : IdentityUser
{

    public DemoDbContext(DbContextOptions options) : base(options) { }

}
```

```cs
using MediatR;
using Microsoft.Extensions.Configuration;
using qckdev.AspNetCore.Identity.Commands;
using qckdev.AspNetCore.Identity.Infrastructure.Data;
using qckdev.AspNetCore.Identity.Services;

public class DataInitialization : IDataInitializer
{
    public DataInitialization(
            IMediator mediator,
            IServiceProvider services,
            IIdentityManager identityManager,
            IConfiguration configuration, 
            ...)
    {
        (...)
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        (...)
    }
}
```

```json
{
  (...)
  "Tokens": {
    "Key": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "ClientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "Issuer": "localhost.loc",
    "AccessExpireSeconds": "86400"
  },
  "Authentication": {
    "Google": {
      "ClientId": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.apps.googleusercontent.com",
      "ClientSecret": "xxxxxxxxxxxxxxxxxxxxxxxx"
    },
    "Microsoft": {
      "ClientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
      "TenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
      "ClientSecret": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
    }
  },

```
