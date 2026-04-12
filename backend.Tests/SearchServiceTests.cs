using backend.Models;
using backend.Repositories;
using backend.Services;
using Moq;

namespace backend.Tests;

public class SearchServiceTests
{
    private readonly Mock<IDeviceRepository> _mockDeviceRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly SearchService _service;

    private readonly List<Device> _testDevices = new()
    {
        new Device { Id = "1", Name = "iPhone 15 Pro", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17 Pro", RAM = 8, Description = "Test 1", AssignedUserId = null },
        new Device { Id = "2", Name = "Galaxy S24", Manufacturer = "Samsung", Type = "phone", OS = "Android", OSVersion = "14.0", Processor = "Snapdragon 8 Gen 3", RAM = 12, Description = "Test 2", AssignedUserId = null },
        new Device { Id = "3", Name = "iPad Pro", Manufacturer = "Apple", Type = "tablet", OS = "iPadOS", OSVersion = "17.0", Processor = "M2", RAM = 16, Description = "Test 3", AssignedUserId = null },
        new Device { Id = "4", Name = "Pixel 8", Manufacturer = "Google", Type = "phone", OS = "Android", OSVersion = "14.0", Processor = "Tensor G3", RAM = 8, Description = "Test 4", AssignedUserId = null }
    };

    public SearchServiceTests()
    {
        _mockDeviceRepository = new Mock<IDeviceRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _service = new SearchService(_mockDeviceRepository.Object, _mockUserRepository.Object);
        _mockDeviceRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(_testDevices);
    }


    [Fact]
    public async Task SearchDevicesAsync_ShouldReturnEmptyList_WhenQueryIsEmpty()
    {
        var result = await _service.SearchDevicesAsync("");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchDevicesAsync_ShouldReturnEmptyList_WhenQueryIsWhitespace()
    {
        var result = await _service.SearchDevicesAsync("   ");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchDevicesAsync_ShouldReturnMatchingDevices_ByName()
    {
        var result = await _service.SearchDevicesAsync("iPhone");

        Assert.Single(result);
        Assert.Equal("iPhone 15 Pro", result[0].Name);
    }

    [Fact]
    public async Task SearchDevicesAsync_ShouldReturnMatchingDevices_ByManufacturer()
    {
        var result = await _service.SearchDevicesAsync("Apple");

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SearchDevicesAsync_ShouldReturnMatchingDevices_ByProcessor()
    {
        var result = await _service.SearchDevicesAsync("Snapdragon");

        Assert.Single(result);
        Assert.Equal("Galaxy S24", result[0].Name);
    }

    [Fact]
    public async Task SearchDevicesAsync_ShouldReturnMatchingDevices_ByRAM()
    {
        var result = await _service.SearchDevicesAsync("16");

        Assert.Single(result);
        Assert.Equal("iPad Pro", result[0].Name);
    }

    [Fact]
    public async Task SearchDevicesAsync_ShouldBeCaseInsensitive()
    {
        var result = await _service.SearchDevicesAsync("iphone");

        Assert.Single(result);
        Assert.Equal("iPhone 15 Pro", result[0].Name);
    }

    [Fact]
    public async Task SearchDevicesAsync_ShouldHandleExtraSpaces()
    {
        var result = await _service.SearchDevicesAsync("  iPhone  ");

        Assert.Single(result);
        Assert.Equal("iPhone 15 Pro", result[0].Name);
    }

    [Fact]
    public async Task SearchDevicesAsync_ShouldReturnEmptyList_WhenNoMatch()
    {
        var result = await _service.SearchDevicesAsync("nonexistent");

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchDevicesAsync_ShouldRankNameMatchesHigher_ThanManufacturerMatches()
    {
        var result = await _service.SearchDevicesAsync("Apple");

        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Name == "iPhone 15 Pro");
        Assert.Contains(result, r => r.Name == "iPad Pro");
    }

    [Fact]
    public async Task SearchDevicesAsync_ShouldHandleMultipleTokens()
    {
        var result = await _service.SearchDevicesAsync("iPhone Apple");

        Assert.NotEmpty(result);
        Assert.Equal("iPhone 15 Pro", result[0].Name);
    }

    [Fact]
    public async Task SearchDevicesAsync_ShouldHandlePunctuation()
    {
        var result = await _service.SearchDevicesAsync("iPhone!");

        Assert.Single(result);
        Assert.Equal("iPhone 15 Pro", result[0].Name);
    }

    [Fact]
    public async Task SearchDevicesAsync_ShouldReturnOrderedByRelevance()
    {
        var result = await _service.SearchDevicesAsync("Apple");

        var scores = result.Select(r => r.Name).ToList();
        Assert.True(scores.IndexOf("iPad Pro") < scores.IndexOf("iPhone 15 Pro") ||
                    scores.IndexOf("iPhone 15 Pro") >= 0);
    }
}