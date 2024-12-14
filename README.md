# Custom Authentication and Authorization in an ASP.NET Core MVC + Web API using .NET 9.0

https://github.com/gtechsltn/NF48_ConsoleApp_SQLServer_Logging_Debugging

https://github.com/gtechsltn/EFCore_ConnectionString

https://github.com/gtechsltn/DapperSelect

https://github.com/gtechsltn/NET9

https://github.com/gtechsltn/NET9/tree/master/NET9_ConsoleApp_Dapper_EFCore

https://github.com/gtechsltn/NET9/tree/master/NET9_ConsoleApp_Logging_Log4Net

# 1/ Custom Authentication and Authorization

To implement custom Authentication and Authorization in an ASP.NET Core MVC + Web API using .NET 9.0, you'll need to follow several steps.

This example will demonstrate how to create a simple custom authentication scheme where users can log in with a username and password.

Step 1: Create a New ASP.NET Core MVC Web API Project

If you haven't already created a project, you can create a new one using the .NET CLI:

```
dotnet new webapi -n MyApi
cd MyApi
```

Step 2: Add Required NuGet Packages

To set up authentication, you may use the following packages. You can install them using the .NET CLI:

```
dotnet add package Microsoft.AspNetCore.Authentication
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

Step 3: Create a User Model

Create a simple user model that will represent the user in your system.

```
// Models/User.cs
namespace MyApi.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; } // In a real application, this should be hashed
    }
}
```

Step 4: Create a Custom Authentication Handler

You need to implement a custom authentication handler. Here is a simplified example of how to do this using JWT.

```
// Services/CustomAuthenticationHandler.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace MyApi.Services
{
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CustomAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Get the Authorization header
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                return AuthenticateResult.NoResult();
            }

            // Validate the token (this is a simple example)
            var token = authHeader.ToString().Replace("Bearer ", "");
            var user = ValidateToken(token);

            if (user == null)
            {
                return AuthenticateResult.Fail("Invalid token");
            }

            var claims = new[] { new Claim(ClaimTypes.Name, user.Username) };
            var identity = new ClaimsIdentity(claims, "Custom");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Custom");

            return AuthenticateResult.Success(ticket);
        }

        private User ValidateToken(string token)
        {
            // In a real application, validate the token and retrieve the user
            // Here, we simply return a user if the token matches "validtoken"
            if (token == "validtoken")
            {
                return new User { Username = "testuser" };
            }

            return null;
        }
    }
}
```

Step 5: Configure Authentication in Program.cs

Configure the custom authentication scheme in the Program.cs file:

```
// Program.cs
using Microsoft.AspNetCore.Authentication;
using MyApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure custom authentication
builder.Services.AddAuthentication("Custom")
    .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("Custom", null);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthentication(); // Enable authentication
app.UseAuthorization();

app.MapControllers();

app.Run();
```

Step 6: Create a Sample Controller

Create a sample controller to test the custom authentication:

```
// Controllers/ProtectedController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProtectedController : ControllerBase
    {
        [HttpGet]
        [Authorize] // Protect this endpoint
        public IActionResult Get()
        {
            return Ok(new { Message = "This is a protected resource." });
        }
    }
}
```

Step 7: Test the API

Run the Application:

Use the following command to run the application:

```
dotnet run
```

Test Using Postman or CURL:

Make a request to the protected endpoint:

Without Token: 

GET https://localhost:5001/protected

With Token: Add the following Authorization header:

Authorization: Bearer validtoken

You should receive a response from the protected endpoint if the correct token is provided.

## Conclusion

You have successfully implemented custom authentication and authorization in an ASP.NET Core MVC Web API application using .NET 9.0.

This example uses a simplified token validation mechanism for demonstration purposes.

In a real application, you should implement proper token generation, storage, validation, and enhance security by hashing passwords, using HTTPS, and more.

# 2/ ASP.NET Core MVC

```
md MyMvc
cd MyMvc
dotnet new mvc -f net9.0 --use-program-main --use-local-db --auth None
dotnet add package Microsoft.AspNetCore.Authentication
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
dotnet build
dotnet test
dotnet run
```

How To Add JWT Bearer Token Authorization Functionality In Swagger? (Nov 30, 2021)

https://www.c-sharpcorner.com/article/how-to-add-jwt-bearer-token-authorization-functionality-in-swagger/

OAuth Bearer Token with Swagger UI — .NET 6.0 (Jan 6, 2023)

https://medium.com/@deidra108/oauth-bearer-token-with-swagger-ui-net-6-0-86835e616deb

# Testing

https://localhost:7214/swagger/index.html

Click into button Authorize and enter the Bearer JWT as the following:

```
Bearer validtoken
```

# 3/ EF Core + DbContext and Product table

```
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
```

```
//appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyMvc;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;"
  }
}
```

```
// Program.cs
using Microsoft.EntityFrameworkCore;
using MyApi.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
```

```
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost;Database=MyMvc;Trusted_Connection=True;");
    }

    public DbSet<Product> Products { get; set; }
}
```

```
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

## Design-time DbContext Creation: From a design-time factory

https://learn.microsoft.com/vi-vn/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli

```
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=MyMvc;Trusted_Connection=True;");
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
```

```
"DefaultConnection": "Server=localhost;Database=MyMvc;Trusted_Connection=True;"
```

```
"DefaultConnection": "Server=localhost;Database=MyMvc;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;"
```

# 4/ Authentication with User table

```
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
```

```
dotnet ef migrations add Authenticate_User_Table
dotnet ef database update
```

## Register

POST https://localhost:5001/api/auth/register

Content-Type: application/json

```
{
  "username": "Admin",
  "password": "Passw$rd@123"
}
```

## Login

POST https://localhost:5001/api/auth/login

Content-Type: application/json

```
{
  "username": "Admin",
  "password": "Passw$rd@123"
}
```

Token
```
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkFkbWluIiwibmJmIjoxNzM0MTY1NzgwLCJleHAiOjE3MzQyNTIxODAsImlhdCI6MTczNDE2NTc4MH0.V8h2HadS3rbDEIoa1dFXrIn1vMwfVpXMSo0bgS2Tdfc"
}
```

# Multiples Authen (Api Key + JWT)

For JWT Authentication:
```
GET https://localhost:5001/api/jwt
Authorization: Bearer your_jwt_token_here
```

For API Key Authentication:
```
GET https://localhost:5001/api/apikey
X-Api-Key: YourApiKey123
```

# Publish

## PowerShell Run as Administrator
```
md C:\inetpub\wwwroot\MyMvc
cd D:\gtechsltn\NET9_MVC_Custom_Authen_Author
dotnet publish -c Release -o C:\inetpub\wwwroot\MyMvc
```

## Set Permissions:

Ensure that the IIS_IUSRS group has permission to access the published folder.

Right-click the folder, go to Properties, then Security, and add the IIS_IUSRS group with Read & Execute permissions.

## Configure web.config
```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="yourapp.dll" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified"/>
    </handlers>
    <aspNetCore processPath="dotnet" arguments=".\yourapp.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="InProcess"/>
  </system.webServer>
</configuration>
```

## Run swagger in release mode

https://localhost/swagger/index.html

# References

https://workik.com/project/74656/ai_scripts
