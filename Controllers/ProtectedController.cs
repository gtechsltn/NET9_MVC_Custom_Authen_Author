using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyMvc.Controllers;

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
