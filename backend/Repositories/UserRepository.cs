using backend.Configuration;
using backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using static System.Net.WebRequestMethods;

namespace backend.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _usersCollection;

    public UserRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _usersCollection = database.GetCollection<User>(settings.Value.UsersCollectionName);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _usersCollection.Find(_=> true).ToListAsync();
    }


    public async Task<User?> GetByIdAsync(string  id)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        return await _usersCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Email, email);
        return await _usersCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(User user)
    {
        await _usersCollection.InsertOneAsync(user);
    }

    public async Task UpdateAsync(string id, User user)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        await _usersCollection.ReplaceOneAsync(filter, user);
    }

}
