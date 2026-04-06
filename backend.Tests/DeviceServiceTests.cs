using backend.Models;
using backend.Repositories;
using backend.Services;
using Moq;

namespace backend.Tests;

public class DeviceServiceTests
{
    private readonly Mock<IDeviceRepository> _mockRepository;
    private readonly DeviceService _service;

    public DeviceServiceTests()
    {
        _mockRepository = new Mock<IDeviceRepository>();
        _service = new DeviceService(_mockRepository.Object);
    }


    [Fact]
    public async Task GetAllDevicesAsync_ShouldReturnAllDevices()
    {
        var devices = new List<Device>
        {
            new Device { Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test 1" },
            new Device { Name = "Galaxy S24", Manufacturer = "Samsung", Type = "phone", OS = "Android", OSVersion = "14.0", Processor = "Snapdragon", RAM = 12, Description = "Test 2" },
            new Device { Name = "iPad Pro", Manufacturer = "Apple", Type = "tablet", OS = "iPadOS", OSVersion = "17.0", Processor = "M2", RAM = 16, Description = "Test 3" }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(devices);

        var result = await _service.GetAllDevicesAsync();

        Assert.Equal(3, result.Count);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllDevicesAsync_ShouldReturnEmptyList_WhenNoDevices()
    {
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Device>());

        var result = await _service.GetAllDevicesAsync();

        Assert.Empty(result);
    }


    [Fact]
    public async Task GetDeviceByIdAsync_ShouldReturnCorrectDevice()
    {
        var device = new Device { Id = "123", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test" };
        _mockRepository.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(device);

        var result = await _service.GetDeviceByIdAsync("123");

        Assert.NotNull(result);
        Assert.Equal("iPhone 15", result.Name);
        Assert.Equal("Apple", result.Manufacturer);
    }

    [Fact]
    public async Task GetDeviceByIdAsync_ShouldReturnNull_WhenDeviceNotFound()
    {
        _mockRepository.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((Device?)null);

        var result = await _service.GetDeviceByIdAsync("999");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetDeviceByIdAsync_ShouldCallRepositoryOnce()
    {
        _mockRepository.Setup(r => r.GetByIdAsync("123")).ReturnsAsync((Device?)null);

        await _service.GetDeviceByIdAsync("123");

        _mockRepository.Verify(r => r.GetByIdAsync("123"), Times.Once);
    }


    [Fact]
    public async Task CreateDeviceAsync_ShouldCallRepository()
    {
        var device = new Device { Name = "Pixel 8", Manufacturer = "Google", Type = "phone", OS = "Android", OSVersion = "14.0", Processor = "Tensor G3", RAM = 8, Description = "Test" };

        await _service.CreateDeviceAsync(device);

        _mockRepository.Verify(r => r.CreateAsync(device), Times.Once);
    }

    [Fact]
    public async Task CreateDeviceAsync_ShouldCallRepository_WithCorrectDevice()
    {
        Device? capturedDevice = null;
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Device>()))
            .Callback<Device>(d => capturedDevice = d)
            .Returns(Task.CompletedTask);

        var device = new Device { Name = "Pixel 8", Manufacturer = "Google", Type = "phone", OS = "Android", OSVersion = "14.0", Processor = "Tensor G3", RAM = 8, Description = "Test" };
        await _service.CreateDeviceAsync(device);

        Assert.NotNull(capturedDevice);
        Assert.Equal("Pixel 8", capturedDevice.Name);
        Assert.Equal("Google", capturedDevice.Manufacturer);
    }


    [Fact]
    public async Task UpdateDeviceAsync_ShouldCallRepository()
    {
        var device = new Device { Id = "123", Name = "iPad Pro", Manufacturer = "Apple", Type = "tablet", OS = "iPadOS", OSVersion = "17.0", Processor = "M2", RAM = 16, Description = "Test" };

        await _service.UpdateDeviceAsync("123", device);

        _mockRepository.Verify(r => r.UpdateAsync("123", device), Times.Once);
    }

    [Fact]
    public async Task UpdateDeviceAsync_ShouldCallRepository_WithCorrectIdAndDevice()
    {
        string? capturedId = null;
        Device? capturedDevice = null;
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<Device>()))
            .Callback<string, Device>((id, d) => { capturedId = id; capturedDevice = d; })
            .Returns(Task.CompletedTask);

        var device = new Device { Id = "123", Name = "iPad Pro Updated", Manufacturer = "Apple", Type = "tablet", OS = "iPadOS", OSVersion = "17.0", Processor = "M4", RAM = 32, Description = "Updated" };
        await _service.UpdateDeviceAsync("123", device);

        Assert.Equal("123", capturedId);
        Assert.Equal("iPad Pro Updated", capturedDevice!.Name);
        Assert.Equal("M4", capturedDevice.Processor);
    }


    [Fact]
    public async Task DeleteDeviceAsync_ShouldCallRepository()
    {
        await _service.DeleteDeviceAsync("123");

        _mockRepository.Verify(r => r.DeleteAsync("123"), Times.Once);
    }

    [Fact]
    public async Task DeleteDeviceAsync_ShouldCallRepository_WithCorrectId()
    {
        string? capturedId = null;
        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<string>()))
            .Callback<string>(id => capturedId = id)
            .Returns(Task.CompletedTask);

        await _service.DeleteDeviceAsync("456");

        Assert.Equal("456", capturedId);
    }

    [Fact]
    public async Task DeleteDeviceAsync_ShouldNotCallRepository_WithWrongId()
    {
        await _service.DeleteDeviceAsync("123");

        _mockRepository.Verify(r => r.DeleteAsync("999"), Times.Never);
    }
}