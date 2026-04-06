using System;
using backend.Models;
using backend.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using backend.Repositories;

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
}
