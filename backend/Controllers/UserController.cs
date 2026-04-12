namespace backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using backend.Services;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Get all users</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>Get user by ID</summary>
    [HttpGet("{id}")]   
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }
        else
            return Ok(user);
    }
 }
