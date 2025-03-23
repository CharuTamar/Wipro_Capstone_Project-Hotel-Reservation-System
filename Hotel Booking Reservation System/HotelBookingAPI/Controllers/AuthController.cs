using HotelBookingAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var result = await _authService.RegisterUserAsync(model);
        if (result.Contains("successfully"))
            return Ok(new { Message = result });
        return BadRequest(new { Error = result });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel request)
    {
        var token = await _authService.LoginUserAsync(request);

        if (token != null)
        {
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = false,
                Secure = false,
                Domain = "localhost",
                Path = "/",
                SameSite = SameSiteMode.Lax
            });

            return Ok(new { token });
        }

        //  Change 401 to 400 explicitly
        return BadRequest(new { message = "Invalid login attempt!" });
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] RoleAssignmentModel model)
    {
        var result = await _authService.AssignRoleAsync(model.Email, model.Role);
        if (result.Contains("assigned"))
            return Ok(new { Message = result });
        return BadRequest(new { Error = result });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("check-user/{email}")]
    public async Task<IActionResult> CheckUserExists([FromRoute] string email)
    {
        var exists = await _authService.CheckUserExistsAsync(email);
        if (exists)
        {
            return Ok(new { Message = "User exists in the database." });
        }
        else
        {
            return NotFound(new { Message = "User not found." });
        }
    }

    [HttpGet("current-user")]
    [Authorize] //  Ensure user is authenticated
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userId != null)
        {
            return Ok(new
            {
                Id = userId,
                Email = email,
                Role = role
            });
        }

        return Unauthorized();
    }


    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");
        return Ok(new { Message = "Logged out successfully" });
    }


}
