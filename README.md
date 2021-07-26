<a href="https://www.nuget.org/packages/qckdev.AspNetCore.Identity"><img src="https://img.shields.io/nuget/v/qckdev.AspNetCore.Identity.svg" alt="NuGet Version"/></a>
<a href="https://sonarcloud.io/dashboard?id=qckdev.AspNetCore.Identity"><img src="https://sonarcloud.io/api/project_badges/measure?project=qckdev.AspNetCore.Identity&metric=alert_status" alt="Quality Gate"/></a>
<a href="https://sonarcloud.io/dashboard?id=qckdev.AspNetCore.Identity"><img src="https://sonarcloud.io/api/project_badges/measure?project=qckdev.AspNetCore.Identity&metric=coverage" alt="Code Coverage"/></a>
<a><img src="https://hfrances.visualstudio.com/Main/_apis/build/status/qckdev.AspNetCore.Identity?branchName=master" alt="Azure Pipelines Status"/></a>


# qckdev.AspNetCore.Identity

```cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using qckdev.AspNetCore.Identity;

public void ConfigureServices(IServiceCollection services)
{
	var jwtTokenConfiguration = JwtTokenConfiguration.Get(configuration, "Tokens");

	services
		.AddInfrastructure<TUser, DemoDbContext<TUser>>(options =>
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

	app.UseJsonExceptionHandler();
	app.UseRouting();
	app.UseAuthentication();
	app.UseAuthorization();

	(...)

	app.UseDataInitializer();
}
```

```cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using qckdev.AspNetCore.Identity.Infrastructure.Data;

public class DemoDbContext<TUser> : ApplicationDbContext<TUser>
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
