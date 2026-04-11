using backend.DTO;
using backend.Repositories;
using System.Text.RegularExpressions;

namespace backend.Services;

public class SearchService : ISearchService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IUserRepository _userRepository;

    public SearchService(IDeviceRepository deviceRepository, IUserRepository userRepository)
    {
        _deviceRepository = deviceRepository;
        _userRepository = userRepository;
    }

    public async Task<List<DeviceResponseDto>> SearchDevicesAsync(string query)
    {
        var tokens = NormalizeAndTokenize(query);
        if (tokens.Length == 0)
            return new List<DeviceResponseDto>();

        var devices = await _deviceRepository.GetAllAsync();
        var scoredDevices = new List<(DeviceResponseDto dto, int score)>();

        foreach (var device in devices)
        {
            var score = 0;

            foreach (var token in tokens)
            {
                if (device.Name.ToLower().Contains(token)) score += 4;
                if (device.Manufacturer.ToLower().Contains(token)) score += 3;
                if (device.Processor.ToLower().Contains(token)) score += 2;
                if (device.RAM.ToString().Contains(token)) score += 1;
            }

            if (score > 0)
            {
                string? assignedUserName = null;
                if (device.AssignedUserId is not null)
                {
                    var user = await _userRepository.GetByIdAsync(device.AssignedUserId);
                    assignedUserName = user?.Name;
                }

                scoredDevices.Add((new DeviceResponseDto
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
                }, score));
            }
        }

        return scoredDevices
            .OrderByDescending(x => x.score)
            .Select(x => x.dto)
            .ToList();
    }

    private string[] NormalizeAndTokenize(string query)
    {
        var normalized = query.ToLower().Trim();
        normalized = Regex.Replace(normalized, @"[^\w\s]", " ");
        normalized = Regex.Replace(normalized, @"\s+", " ");
        return normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }
}