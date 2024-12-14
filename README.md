# Custom authentication and authorization in an ASP.NET Core MVC Web API application using .NET 9.0

To implement custom authentication and authorization in an ASP.NET Core MVC Web API application using .NET 9.0, you'll need to follow several steps. This example will demonstrate how to create a simple custom authentication scheme where users can log in with a username and password.

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

# Conclusion

You have successfully implemented custom authentication and authorization in an ASP.NET Core MVC Web API application using .NET 9.0.

This example uses a simplified token validation mechanism for demonstration purposes.

In a real application, you should implement proper token generation, storage, validation, and enhance security by hashing passwords, using HTTPS, and more.

# ASP.NET Core MVC

```
md MyMvc
cd MyMvc
dotnet new mvc -f net9.0 --use-program-main --use-local-db --auth None
dotnet add package Microsoft.AspNetCore.Authentication
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet build
dotnet test
dotnet run
```

http://localhost:5131/
