using backend.Configuration;
using backend.Models;
using backend.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class DeviceRepository : IDeviceRepository
{
    private readonly IMongoCollection<Device> _devicesCollection;

    public DeviceRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _devicesCollection = database.GetCollection<Device>(settings.Value.DevicesCollectionName);
    }

    public async Task<List<Device>> GetAllAsync()
    {
        return await _devicesCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Device?> GetByIdAsync(string id)
    {
        var filter = Builders<Device>.Filter.Eq(d => d.Id, id);
        return await _devicesCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Device device)
    {
        await _devicesCollection.InsertOneAsync(device);
    }

    public async Task UpdateAsync(string id, Device device)
    {
        var filter = Builders<Device>.Filter.Eq(d => d.Id, id);
        await _devicesCollection.ReplaceOneAsync(filter, device);
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<Device>.Filter.Eq(d => d.Id, id);
        await _devicesCollection.DeleteOneAsync(filter);
    }

}