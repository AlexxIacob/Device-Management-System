using backend.DTO;
using backend.Models;
using backend.Repositories;

namespace backend.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IUserRepository _userRepository;

    public DeviceService(IDeviceRepository deviceRepository, IUserRepository userRepository)
    {
        _deviceRepository = deviceRepository;
        _userRepository = userRepository;
    }

    public async Task<List<DeviceResponseDto>> GetAllDevicesAsync()
    {
        var devices = await _deviceRepository.GetAllAsync();
        var result = new List<DeviceResponseDto>();

        foreach (var device in devices)
        {
            var dto = await MapToResponseDto(device);
            result.Add(dto);
        }

        return result;
    }

    public async Task<DeviceResponseDto?> GetDeviceByIdAsync(string id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);
        if (device is null)
            return null;

        return await MapToResponseDto(device);
    }

    public async Task CreateDeviceAsync(CreateDeviceDto dto)
    {
        var existing = await _deviceRepository.GetByNameAsync(dto.Name);
        if (existing is not null)
            throw new InvalidOperationException("Device already exists.");

        var device = new Device
        {
            Name = dto.Name,
            Manufacturer = dto.Manufacturer,
            Type = dto.Type,
            OS = dto.OS,
            OSVersion = dto.OSVersion,
            Processor = dto.Processor,
            RAM = dto.RAM,
            Description = dto.Description,
            AssignedUserId = null
        };

        await _deviceRepository.CreateAsync(device);
    }

    public async Task UpdateDeviceAsync(string id, CreateDeviceDto dto)
    {
        var existing = await _deviceRepository.GetByIdAsync(id);
        if (existing is null)
            return;

        existing.Name = dto.Name;
        existing.Manufacturer = dto.Manufacturer;
        existing.Type = dto.Type;
        existing.OS = dto.OS;
        existing.OSVersion = dto.OSVersion;
        existing.Processor = dto.Processor;
        existing.RAM = dto.RAM;
        existing.Description = dto.Description;

        await _deviceRepository.UpdateAsync(id, existing);
    }

    public async Task DeleteDeviceAsync(string id)
    {
        await _deviceRepository.DeleteAsync(id);
    }

    public async Task<bool> AssignDeviceAsync(string deviceId, string userId)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId);
        if (device is null)
            return false;

        if (device.AssignedUserId is not null)
            return false;

        device.AssignedUserId = userId;
        await _deviceRepository.UpdateAsync(deviceId, device);
        return true;
    }

    public async Task<bool> UnassignDeviceAsync(string deviceId, string userId)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId);
        if (device is null)
            return false;

        if (device.AssignedUserId != userId)
            return false;

        device.AssignedUserId = null;
        await _deviceRepository.UpdateAsync(deviceId, device);
        return true;
    }

    private async Task<DeviceResponseDto> MapToResponseDto(Device device)
    {
        string? assignedUserName = null;

        if (device.AssignedUserId is not null)
        {
            var user = await _userRepository.GetByIdAsync(device.AssignedUserId);
            assignedUserName = user?.Name;
        }

        return new DeviceResponseDto
        {
            Id = device.Id!,
            Name = device.Name,
            Manufacturer = device.Manufacturer,
            Type = device.Type,
            OS = device.OS,
            OSVersion = device.OSVersion,
            Processor = device.Processor,
            RAM = device.RAM,
            Description = device.Description,
            AssignedUserId = device.AssignedUserId,
            AssignedUserName = assignedUserName
        };
    }
}