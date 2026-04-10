using System;
using backend.Models;

namespace backend.Repositories;

public interface IDeviceRepository
{
    Task<List<Device>> GetAllAsync();
    Task<Device?> GetByIdAsync(string id);
    Task CreateAsync(Device device);
    Task UpdateAsync(string id, Device device);
    Task DeleteAsync(string id);

    Task<Device?> GetByNameAsync(string name);
}