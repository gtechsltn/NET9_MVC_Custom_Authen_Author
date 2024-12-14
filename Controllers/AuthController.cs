namespace MyMvc.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest registerRequest)
    {
        var user = new User();
        user.Username = registerRequest.Username;
        // Hash the password (for simplicity, we are not using a secure hashing algorithm here)
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);
        _context.Users.Add(user);
        _context.SaveChanges();
        return Ok(user);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        var existingUser = _context.Users.FirstOrDefault(u => u.Username == loginRequest.Username);
        if (existingUser == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, existingUser.PasswordHash))
        {
            return Unauthorized();
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        // Use a secure yourSuperSecretKey that is at least 32 bytes long (256 bits)
        var yourSuperSecretKey = "YourVerySecureAndLongSecretKeyThatIsAtLeast32BytesLong!"; // Ensure this yourSuperSecretKey is at least 32 bytes long
        var yourSuperSecretKeyBytes = Encoding.UTF8.GetBytes(yourSuperSecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.Name, existingUser.Username)
            }),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(yourSuperSecretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }
}