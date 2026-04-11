using backend.DTO;

namespace backend.Services;

public interface ISearchService
{
    Task<List<DeviceResponseDto>> SearchDevicesAsync(string query);
}