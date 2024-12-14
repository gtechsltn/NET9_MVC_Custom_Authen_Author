namespace MyMvc.Services;

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