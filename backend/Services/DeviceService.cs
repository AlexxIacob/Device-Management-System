using System;
using backend.Services;
using backend.Repositories;
using backend.Models;

namespace backend.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<List<Device>> GetAllDevicesAsync()
    {
        return await _deviceRepository.GetAllAsync();
    }

    public async Task<Device?> GetDeviceByIdAsync(string id)
    {
        return await _deviceRepository.GetByIdAsync(id);
    }

    public async Task CreateDeviceAsync(Device device)
    {
        await _deviceRepository.CreateAsync(device);
    }

    public async Task UpdateDeviceAsync(string id, Device device)
    {
        await _deviceRepository.UpdateAsync(id, device);
    }

    public async Task DeleteDeviceAsync(string id)
    {
        await _deviceRepository.DeleteAsync(id);
    }
}