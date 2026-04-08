using backend.DTO;

namespace backend.Services;

public interface IDeviceService
{
    Task<List<DeviceResponseDto>> GetAllDevicesAsync();
    Task<DeviceResponseDto?> GetDeviceByIdAsync(string id);
    Task CreateDeviceAsync(CreateDeviceDto dto);
    Task UpdateDeviceAsync(string id, CreateDeviceDto dto);
    Task DeleteDeviceAsync(string id);
    Task<bool> AssignDeviceAsync(string deviceId, string userId);
    Task<bool> UnassignDeviceAsync(string deviceId, string userId);
}