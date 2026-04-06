using System;
using backend.Models;

namespace backend.Services;

public interface IDeviceService
{
    Task<List<Device>> GetAllDevicesAsync();
    Task<Device?> GetDeviceByIdAsync(string id);
    Task CreateDeviceAsync(Device device);
    Task UpdateDeviceAsync(string id, Device device);
    Task DeleteDeviceAsync(string id);


}