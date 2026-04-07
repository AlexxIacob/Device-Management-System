using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var devices = await _deviceService.GetAllDevicesAsync();
        return Ok(devices);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var device = await _deviceService.GetDeviceByIdAsync(id);
        if (device is null)
            return NotFound();
        return Ok(device);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Device device)
    {
        await _deviceService.CreateDeviceAsync(device);
        return CreatedAtAction(nameof(GetById), new { id = device.Id }, device);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Device device)
    {
        var existing = await _deviceService.GetDeviceByIdAsync(id);
        if (existing is null)
            return NotFound();
        await _deviceService.UpdateDeviceAsync(id, device);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _deviceService.GetDeviceByIdAsync(id);
        if (existing is null)
            return NotFound();
        await _deviceService.DeleteDeviceAsync(id);
        return NoContent();
    }
}
