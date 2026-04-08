namespace backend.DTO;

public class DeviceResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string OS { get; set; } = string.Empty;
    public string OSVersion { get; set; } = string.Empty;
    public string Processor { get; set; } = string.Empty;
    public int RAM { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? AssignedUserName { get; set; }
    public string? AssignedUserId { get; set; }
}