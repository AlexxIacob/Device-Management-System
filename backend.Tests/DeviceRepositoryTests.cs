using backend.Configuration;
using backend.Models;
using backend.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Tests;

public class DeviceRepositoryTests : IDisposable
{
    private readonly DeviceRepository _repository;
    private readonly IMongoCollection<Device> _collection;

    public DeviceRepositoryTests()
    {
        var settings = Options.Create(new MongoDbSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "DeviceManagementTestDB",
            DevicesCollectionName = "Devices",
            UsersCollectionName = "Users"
        });

        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<Device>(settings.Value.DevicesCollectionName);
        _repository = new DeviceRepository(client, settings);
    }

    public void Dispose()
    {
        _collection.DeleteMany(_ => true);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDevices()
    {
        await _collection.InsertManyAsync(new List<Device>
        {
            new Device { Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test device 1" },
            new Device { Name = "Galaxy S24", Manufacturer = "Samsung", Type = "phone", OS = "Android", OSVersion = "14.0", Processor = "Snapdragon", RAM = 12, Description = "Test device 2" }
        });

        var result = await _repository.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectDevice()
    {
        var device = new Device { Name = "iPhone 15", Manufacturer = "Apple", Type = "phone", OS = "iOS", OSVersion = "17.0", Processor = "A17", RAM = 8, Description = "Test device" };
        await _collection.InsertOneAsync(device);

        var result = await _repository.GetByIdAsync(device.Id!);


        Assert.NotNull(result);
        Assert.Equal("iPhone 15", result.Name);
    }

    [Fact]
    public async Task CreateAsync_ShouldInsertDevice()
    {
        var device = new Device { Name = "Pixel 8", Manufacturer = "Google", Type = "phone", OS = "Android", OSVersion = "14.0", Processor = "Tensor G3", RAM = 8, Description = "Test device" };

        await _repository.CreateAsync(device);

        var result = await _collection.Find(d => d.Name == "Pixel 8").FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.Equal("Pixel 8", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyDevice()
    {
        var device = new Device { Name = "iPad Pro", Manufacturer = "Apple", Type = "tablet", OS = "iPadOS", OSVersion = "17.0", Processor = "M2", RAM = 16, Description = "Test device" };
        await _collection.InsertOneAsync(device);

        device.Name = "iPad Pro Updated";
        await _repository.UpdateAsync(device.Id!, device);

        var result = await _collection.Find(d => d.Id == device.Id).FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.Equal("iPad Pro Updated", result.Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveDevice()
    {
        var device = new Device { Name = "Surface Pro", Manufacturer = "Microsoft", Type = "tablet", OS = "Windows", OSVersion = "11", Processor = "Intel Core i5", RAM = 16, Description = "Test device" };
        await _collection.InsertOneAsync(device);

        await _repository.DeleteAsync(device.Id!);

        var result = await _collection.Find(d => d.Id == device.Id).FirstOrDefaultAsync();
        Assert.Null(result);
    }
}