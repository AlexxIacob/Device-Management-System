using backend.DTO;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Email and password are required.");

        var result = await _authService.RegisterAsync(dto.Email, dto.Password);

        if (result != "User registered successfully.")
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Email and password are required.");

        var token = await _authService.LoginAsync(dto.Email, dto.Password);

        if (token is null)
            return Unauthorized("Invalid email or password.");

        return Ok(new { token });
    }

    [HttpPost("signout")]
    public IActionResult LogOut()
    {
        return Ok("Signed out successfully.");
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Role) || string.IsNullOrWhiteSpace(dto.Location))
            return BadRequest("All fields are required.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        var result = await _authService.UpdateProfileAsync(userId, dto);

        if (!result)
            return NotFound("User not found.");

        return Ok("Profile updated successfully.");
    }
}