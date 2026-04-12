using backend.DTO;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Moq;

namespace backend.Tests;

public class DeviceServiceTests
{
    private readonly Mock<IDeviceRepository> _mockDeviceRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly DeviceService _service;

    public DeviceServiceTests()
    {
        _mockDeviceRepository = new Mock<IDeviceRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _service = new DeviceService(_mockDeviceRepository.Object, _mockUserRepository.Object);
    }


    [Fact]
    public async Task GetAllDevicesAsync_ShouldReturnAllDevices()
    {
        var devices = new List<Device>
        {
            new Device { Id = "1", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test 1" },
            new Device { Id = "2", Name = "Galaxy S24", Manufacturer = "Samsung", Type = "phone", OS = "Android", OSVersion = "14.0", Processor = "Snapdragon", RAM = 12, Description = "Test 2" }
        };
        _mockDeviceRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(devices);

        var result = await _service.GetAllDevicesAsync();

        Assert.Equal(2, result.Count);
        _mockDeviceRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllDevicesAsync_ShouldReturnEmptyList_WhenNoDevices()
    {
        _mockDeviceRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Device>());

        var result = await _service.GetAllDevicesAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllDevicesAsync_ShouldIncludeAssignedUserName()
    {
        var user = new User { Id = "user1", Name = "John Doe", Email = "john@test.com", PasswordHash = "hash", Role = "Admin", Location = "London" };
        var devices = new List<Device>
        {
            new Device { Id = "1", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test", AssignedUserId = "user1" }
        };
        _mockDeviceRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(devices);
        _mockUserRepository.Setup(r => r.GetByIdAsync("user1")).ReturnsAsync(user);

        var result = await _service.GetAllDevicesAsync();

        Assert.Equal("John Doe", result[0].AssignedUserName);
    }

    [Fact]
    public async Task GetAllDevicesAsync_ShouldReturnNullAssignedUserName_WhenNotAssigned()
    {
        var devices = new List<Device>
        {
            new Device { Id = "1", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test", AssignedUserId = null }
        };
        _mockDeviceRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(devices);

        var result = await _service.GetAllDevicesAsync();

        Assert.Null(result[0].AssignedUserName);
    }


    [Fact]
    public async Task GetDeviceByIdAsync_ShouldReturnCorrectDevice()
    {
        var device = new Device { Id = "1", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test" };
        _mockDeviceRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(device);

        var result = await _service.GetDeviceByIdAsync("1");

        Assert.NotNull(result);
        Assert.Equal("iPhone 15", result.Name);
    }

    [Fact]
    public async Task GetDeviceByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        _mockDeviceRepository.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((Device?)null);

        var result = await _service.GetDeviceByIdAsync("999");

        Assert.Null(result);
    }


    [Fact]
    public async Task CreateDeviceAsync_ShouldCreateDevice_WhenNameIsUnique()
    {
        _mockDeviceRepository.Setup(r => r.GetByNameAsync("iPhone 16")).ReturnsAsync((Device?)null);

        var dto = new CreateDeviceDto { Name = "iPhone 16", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "18.0", Processor = "A18", RAM = 8, Description = "Test" };

        await _service.CreateDeviceAsync(dto);

        _mockDeviceRepository.Verify(r => r.CreateAsync(It.IsAny<Device>()), Times.Once);
    }

    [Fact]
    public async Task CreateDeviceAsync_ShouldThrow_WhenDeviceAlreadyExists()
    {
        var existing = new Device { Id = "1", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test" };
        _mockDeviceRepository.Setup(r => r.GetByNameAsync("iPhone 15")).ReturnsAsync(existing);

        var dto = new CreateDeviceDto { Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateDeviceAsync(dto));
        _mockDeviceRepository.Verify(r => r.CreateAsync(It.IsAny<Device>()), Times.Never);
    }

    [Fact]
    public async Task CreateDeviceAsync_ShouldSetAssignedUserIdToNull()
    {
        _mockDeviceRepository.Setup(r => r.GetByNameAsync("iPhone 16")).ReturnsAsync((Device?)null);

        Device? capturedDevice = null;
        _mockDeviceRepository.Setup(r => r.CreateAsync(It.IsAny<Device>()))
            .Callback<Device>(d => capturedDevice = d)
            .Returns(Task.CompletedTask);

        var dto = new CreateDeviceDto { Name = "iPhone 16", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "18.0", Processor = "A18", RAM = 8, Description = "Test" };
        await _service.CreateDeviceAsync(dto);

        Assert.Null(capturedDevice!.AssignedUserId);
    }


    [Fact]
    public async Task UpdateDeviceAsync_ShouldUpdateDevice_WhenExists()
    {
        var existing = new Device { Id = "1", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Old" };
        _mockDeviceRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(existing);

        var dto = new CreateDeviceDto { Name = "iPhone 15 Updated", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "New" };
        await _service.UpdateDeviceAsync("1", dto);

        _mockDeviceRepository.Verify(r => r.UpdateAsync("1", It.IsAny<Device>()), Times.Once);
    }

    [Fact]
    public async Task UpdateDeviceAsync_ShouldNotUpdate_WhenDeviceNotFound()
    {
        _mockDeviceRepository.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((Device?)null);

        var dto = new CreateDeviceDto { Name = "iPhone 16", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "18.0", Processor = "A18", RAM = 8, Description = "Test" };
        await _service.UpdateDeviceAsync("999", dto);

        _mockDeviceRepository.Verify(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<Device>()), Times.Never);
    }

    [Fact]
    public async Task UpdateDeviceAsync_ShouldPreserveAssignedUserId()
    {
        var existing = new Device { Id = "1", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Old", AssignedUserId = "user1" };
        _mockDeviceRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(existing);

        Device? capturedDevice = null;
        _mockDeviceRepository.Setup(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<Device>()))
            .Callback<string, Device>((id, d) => capturedDevice = d)
            .Returns(Task.CompletedTask);

        var dto = new CreateDeviceDto { Name = "iPhone 15 Updated", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "New" };
        await _service.UpdateDeviceAsync("1", dto);

        Assert.Equal("user1", capturedDevice!.AssignedUserId);
    }


    [Fact]
    public async Task DeleteDeviceAsync_ShouldCallRepository()
    {
        await _service.DeleteDeviceAsync("1");

        _mockDeviceRepository.Verify(r => r.DeleteAsync("1"), Times.Once);
    }

    [Fact]
    public async Task DeleteDeviceAsync_ShouldCallRepository_WithCorrectId()
    {
        string? capturedId = null;
        _mockDeviceRepository.Setup(r => r.DeleteAsync(It.IsAny<string>()))
            .Callback<string>(id => capturedId = id)
            .Returns(Task.CompletedTask);

        await _service.DeleteDeviceAsync("123");

        Assert.Equal("123", capturedId);
    }


    [Fact]
    public async Task AssignDeviceAsync_ShouldReturnTrue_WhenDeviceIsFree()
    {
        var device = new Device { Id = "1", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test", AssignedUserId = null };
        _mockDeviceRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(device);

        var result = await _service.AssignDeviceAsync("1", "user1");

        Assert.True(result);
        _mockDeviceRepository.Verify(r => r.UpdateAsync("1", It.IsAny<Device>()), Times.Once);
    }

    [Fact]
    public async Task AssignDeviceAsync_ShouldReturnFalse_WhenDeviceAlreadyAssigned()
    {
        var device = new Device { Id = "1", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test", AssignedUserId = "otherUser" };
        _mockDeviceRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(device);

        var result = await _service.AssignDeviceAsync("1", "user1");

        Assert.False(result);
        _mockDeviceRepository.Verify(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<Device>()), Times.Never);
    }

    [Fact]
    public async Task AssignDeviceAsync_ShouldReturnFalse_WhenDeviceNotFound()
    {
        _mockDeviceRepository.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((Device?)null);

        var result = await _service.AssignDeviceAsync("999", "user1");

        Assert.False(result);
    }

    [Fact]
    public async Task AssignDeviceAsync_ShouldSetAssignedUserId()
    {
        var device = new Device { Id = "1", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test", AssignedUserId = null };
        _mockDeviceRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(device);

        Device? capturedDevice = null;
        _mockDeviceRepository.Setup(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<Device>()))
            .Callback<string, Device>((id, d) => capturedDevice = d)
            .Returns(Task.CompletedTask);

        await _service.AssignDeviceAsync("1", "user1");

        Assert.Equal("user1", capturedDevice!.AssignedUserId);
    }


    [Fact]
    public async Task UnassignDeviceAsync_ShouldReturnTrue_WhenAssignedToUser()
    {
        var device = new Device { Id = "1", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test", AssignedUserId = "user1" };
        _mockDeviceRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(device);

        var result = await _service.UnassignDeviceAsync("1", "user1");

        Assert.True(result);
        _mockDeviceRepository.Verify(r => r.UpdateAsync("1", It.IsAny<Device>()), Times.Once);
    }

    [Fact]
    public async Task UnassignDeviceAsync_ShouldReturnFalse_WhenAssignedToDifferentUser()
    {
        var device = new Device { Id = "1", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test", AssignedUserId = "otherUser" };
        _mockDeviceRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(device);

        var result = await _service.UnassignDeviceAsync("1", "user1");

        Assert.False(result);
        _mockDeviceRepository.Verify(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<Device>()), Times.Never);
    }

    [Fact]
    public async Task UnassignDeviceAsync_ShouldReturnFalse_WhenDeviceNotFound()
    {
        _mockDeviceRepository.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((Device?)null);

        var result = await _service.UnassignDeviceAsync("999", "user1");

        Assert.False(result);
    }

    [Fact]
    public async Task UnassignDeviceAsync_ShouldSetAssignedUserIdToNull()
    {
        var device = new Device { Id = "1", Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test", AssignedUserId = "user1" };
        _mockDeviceRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(device);

        Device? capturedDevice = null;
        _mockDeviceRepository.Setup(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<Device>()))
            .Callback<string, Device>((id, d) => capturedDevice = d)
            .Returns(Task.CompletedTask);

        await _service.UnassignDeviceAsync("1", "user1");

        Assert.Null(capturedDevice!.AssignedUserId);
    }
}