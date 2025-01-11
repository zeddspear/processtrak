using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using processtrak_backend.Api.data;

[Authorize]
[ApiController]
[Route("api/me")]
public class MeController : ControllerBase
{
    private readonly AppDbContext _context;

    public MeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult?> GetMyData()
    {
        // Retrieve the userId from the context
        var userId = HttpContext.Items["User"] as Guid?;

        var user = await _context
            .Users.Where(u => u.id == userId) // Your condition to find the user
            .Select(u => new
            {
                u.id,
                u.name,
                u.email,
                u.phone,
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Return the user data if found
        return Ok(new { user });
    }
}
