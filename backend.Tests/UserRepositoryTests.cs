using backend.Configuration;
using backend.Models;
using backend.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace backend.Tests;

public class UserRepositoryTests : IDisposable
{
    private readonly UserRepository _repository;
    private readonly IMongoCollection<User> _collection;

    public UserRepositoryTests()
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
        _collection = database.GetCollection<User>(settings.Value.UsersCollectionName);
        _repository = new UserRepository(client, settings);
    }

    public void Dispose()
    {
        _collection.DeleteMany(_ => true);
    }


    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        await _collection.InsertManyAsync(new List<User>
        {
            new User { Name = "John Doe", Email = "john@test.com", PasswordHash = "hash1", Role = "Admin", Location = "London" },
            new User { Name = "Jane Smith", Email = "jane@test.com", PasswordHash = "hash2", Role = "Developer", Location = "Berlin" },
            new User { Name = "Bob Johnson", Email = "bob@test.com", PasswordHash = "hash3", Role = "QA", Location = "Cluj" }
        });

        var result = await _repository.GetAllAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsers()
    {
        var result = await _repository.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCorrectFields()
    {
        await _collection.InsertOneAsync(new User
        {
            Name = "John Doe",
            Email = "john@test.com",
            PasswordHash = "hash1",
            Role = "Admin",
            Location = "London"
        });

        var result = await _repository.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("John Doe", result[0].Name);
        Assert.Equal("john@test.com", result[0].Email);
        Assert.Equal("Admin", result[0].Role);
        Assert.Equal("London", result[0].Location);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectUser()
    {
        var user = new User { Name = "John Doe", Email = "john@test.com", PasswordHash = "hash1", Role = "Admin", Location = "London" };
        await _collection.InsertOneAsync(user);

        var result = await _repository.GetByIdAsync(user.Id!);

        Assert.NotNull(result);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("john@test.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserNotFound()
    {
        var result = await _repository.GetByIdAsync("000000000000000000000000");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectUser_WhenMultipleUsersExist()
    {
        var user1 = new User { Name = "John Doe", Email = "john@test.com", PasswordHash = "hash1", Role = "Admin", Location = "London" };
        var user2 = new User { Name = "Jane Smith", Email = "jane@test.com", PasswordHash = "hash2", Role = "Developer", Location = "Berlin" };
        await _collection.InsertManyAsync(new List<User> { user1, user2 });

        var result = await _repository.GetByIdAsync(user2.Id!);

        Assert.NotNull(result);
        Assert.Equal("Jane Smith", result.Name);
        Assert.Equal("jane@test.com", result.Email);
    }


    [Fact]
    public async Task GetByEmailAsync_ShouldReturnCorrectUser()
    {
        var user = new User { Name = "John Doe", Email = "john@test.com", PasswordHash = "hash1", Role = "Admin", Location = "London" };
        await _collection.InsertOneAsync(user);

        var result = await _repository.GetByEmailAsync("john@test.com");

        Assert.NotNull(result);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("john@test.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailNotFound()
    {
        var result = await _repository.GetByEmailAsync("notfound@test.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnCorrectUser_WhenMultipleUsersExist()
    {
        var user1 = new User { Name = "John Doe", Email = "john@test.com", PasswordHash = "hash1", Role = "Admin", Location = "London" };
        var user2 = new User { Name = "Jane Smith", Email = "jane@test.com", PasswordHash = "hash2", Role = "Developer", Location = "Berlin" };
        await _collection.InsertManyAsync(new List<User> { user1, user2 });

        var result = await _repository.GetByEmailAsync("jane@test.com");

        Assert.NotNull(result);
        Assert.Equal("Jane Smith", result.Name);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldBeCaseInsensitive()
    {
        var user = new User { Name = "John Doe", Email = "john@test.com", PasswordHash = "hash1", Role = "Admin", Location = "London" };
        await _collection.InsertOneAsync(user);

        var result = await _repository.GetByEmailAsync("JOHN@TEST.COM");

        Assert.Null(result);
    }


    [Fact]
    public async Task CreateAsync_ShouldInsertUser()
    {
        var user = new User { Name = "John Doe", Email = "john@test.com", PasswordHash = "hash1", Role = "Admin", Location = "London" };

        await _repository.CreateAsync(user);

        var result = await _collection.Find(u => u.Email == "john@test.com").FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.Name);
    }

    [Fact]
    public async Task CreateAsync_ShouldAssignId_AfterInsert()
    {
        var user = new User { Name = "John Doe", Email = "john@test.com", PasswordHash = "hash1", Role = "Admin", Location = "London" };

        await _repository.CreateAsync(user);

        Assert.NotNull(user.Id);
        Assert.NotEmpty(user.Id);
    }

    [Fact]
    public async Task CreateAsync_ShouldInsertMultipleUsers()
    {
        var user1 = new User { Name = "John Doe", Email = "john@test.com", PasswordHash = "hash1", Role = "Admin", Location = "London" };
        var user2 = new User { Name = "Jane Smith", Email = "jane@test.com", PasswordHash = "hash2", Role = "Developer", Location = "Berlin" };

        await _repository.CreateAsync(user1);
        await _repository.CreateAsync(user2);

        var count = await _collection.CountDocumentsAsync(_ => true);
        Assert.Equal(2, count);
    }


    [Fact]
    public async Task UpdateAsync_ShouldModifyUser()
    {
        var user = new User { Name = "John Doe", Email = "john@test.com", PasswordHash = "hash1", Role = "Admin", Location = "London" };
        await _collection.InsertOneAsync(user);

        user.Name = "John Updated";
        await _repository.UpdateAsync(user.Id!, user);

        var result = await _collection.Find(u => u.Id == user.Id).FirstOrDefaultAsync();
        Assert.Equal("John Updated", result!.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyMultipleFields()
    {
        var user = new User { Name = "John Doe", Email = "john@test.com", PasswordHash = "hash1", Role = "Admin", Location = "London" };
        await _collection.InsertOneAsync(user);

        user.Name = "John Updated";
        user.Role = "Developer";
        user.Location = "Berlin";
        await _repository.UpdateAsync(user.Id!, user);

        var result = await _collection.Find(u => u.Id == user.Id).FirstOrDefaultAsync();
        Assert.Equal("John Updated", result!.Name);
        Assert.Equal("Developer", result.Role);
        Assert.Equal("Berlin", result.Location);
    }

    [Fact]
    public async Task UpdateAsync_ShouldNotAffectOtherUsers()
    {
        var user1 = new User { Name = "John Doe", Email = "john@test.com", PasswordHash = "hash1", Role = "Admin", Location = "London" };
        var user2 = new User { Name = "Jane Smith", Email = "jane@test.com", PasswordHash = "hash2", Role = "Developer", Location = "Berlin" };
        await _collection.InsertManyAsync(new List<User> { user1, user2 });

        user1.Name = "John Updated";
        await _repository.UpdateAsync(user1.Id!, user1);

        var result = await _collection.Find(u => u.Id == user2.Id).FirstOrDefaultAsync();
        Assert.Equal("Jane Smith", result!.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPreservePasswordHash()
    {
        var user = new User { Name = "John Doe", Email = "john@test.com", PasswordHash = "original_hash", Role = "Admin", Location = "London" };
        await _collection.InsertOneAsync(user);

        user.Name = "John Updated";
        await _repository.UpdateAsync(user.Id!, user);

        var result = await _collection.Find(u => u.Id == user.Id).FirstOrDefaultAsync();
        Assert.Equal("original_hash", result!.PasswordHash);
    }
}