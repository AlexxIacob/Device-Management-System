using backend.DTO;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    /// <summary>Get all devices</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var devices = await _deviceService.GetAllDevicesAsync();
        return Ok(devices);
    }


    /// <summary>Get device by ID</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var device = await _deviceService.GetDeviceByIdAsync(id);
        if (device is null)
            return NotFound();
        return Ok(device);
    }

    /// <summary>Create a new device</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeviceDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Manufacturer) ||
            string.IsNullOrWhiteSpace(dto.Type) || string.IsNullOrWhiteSpace(dto.OS) ||
            string.IsNullOrWhiteSpace(dto.OSVersion) || string.IsNullOrWhiteSpace(dto.Processor))
            return BadRequest("All fields are required.");

        try
        {
            await _deviceService.CreateDeviceAsync(dto);
            return Ok("Device created successfully.");
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>Update an existing device</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CreateDeviceDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Manufacturer) ||
            string.IsNullOrWhiteSpace(dto.Type) || string.IsNullOrWhiteSpace(dto.OS) ||
            string.IsNullOrWhiteSpace(dto.OSVersion) || string.IsNullOrWhiteSpace(dto.Processor))
            return BadRequest("All fields are required.");

        var existing = await _deviceService.GetDeviceByIdAsync(id);
        if (existing is null)
            return NotFound();

        await _deviceService.UpdateDeviceAsync(id, dto);
        return NoContent();
    }

    /// <summary>Delete a device</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _deviceService.GetDeviceByIdAsync(id);
        if (existing is null)
            return NotFound();

        await _deviceService.DeleteDeviceAsync(id);
        return NoContent();
    }

    /// <summary>Assign device to current user</summary>
    [HttpPost("{id}/assign")]
    public async Task<IActionResult> Assign(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        var result = await _deviceService.AssignDeviceAsync(id, userId);
        if (!result)
            return BadRequest("Device not found or already assigned.");

        return Ok("Device assigned successfully.");
    }

    /// <summary>Unassign device from current user</summary>
    [HttpPost("{id}/unassign")]
    public async Task<IActionResult> Unassign(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized();

        var result = await _deviceService.UnassignDeviceAsync(id, userId);
        if (!result)
            return BadRequest("Device not found or not assigned to you.");

        return Ok("Device unassigned successfully.");
    }

    /// <summary>Chat with AI assistant about devices</summary>
    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatDto dto, [FromServices] IAIService aiService)
    {
        if (string.IsNullOrWhiteSpace(dto.Message))
            return BadRequest("Message is required.");

        var reply = await aiService.ChatAsync(dto.Message);
        return Ok(new { reply });
    }

    /// <summary>Search devices by free text query</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, [FromServices] ISearchService searchService)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Query cannot be empty.");

        var results = await searchService.SearchDevicesAsync(q);
        return Ok(results);
    }

}