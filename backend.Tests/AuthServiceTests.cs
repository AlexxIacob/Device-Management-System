using backend.Configuration;
using backend.DTO;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace backend.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();

        var jwtSettings = Options.Create(new JwtSettings
        {
            Secret = "super-secret-key-minimum-32-characters-long",
            Issuer = "DeviceManagementAPI",
            Audience = "DeviceManagementClient",
            ExpiryHours = 24
        });

        _service = new AuthService(_mockRepository.Object, jwtSettings);
    }


    [Fact]
    public async Task RegisterAsync_ShouldReturnError_WhenEmailInvalid()
    {
        var result = await _service.RegisterAsync("notanemail", "password123");

        Assert.Equal("Invalid email format.", result);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnError_WhenPasswordTooShort()
    {
        var result = await _service.RegisterAsync("john@test.com", "abc");

        Assert.Contains("7 characters", result);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnError_WhenEmailAlreadyExists()
    {
        var existingUser = new User { Email = "john@test.com", PasswordHash = "hash", Name = "", Role = "", Location = "" };
        _mockRepository.Setup(r => r.GetByEmailAsync("john@test.com")).ReturnsAsync(existingUser);

        var result = await _service.RegisterAsync("john@test.com", "password123");

        Assert.Equal("Email already in use.", result);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenValidData()
    {
        _mockRepository.Setup(r => r.GetByEmailAsync("john@test.com")).ReturnsAsync((User?)null);

        var result = await _service.RegisterAsync("john@test.com", "password123");

        Assert.Equal("User registered successfully.", result);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldHashPassword_BeforeSaving()
    {
        _mockRepository.Setup(r => r.GetByEmailAsync("john@test.com")).ReturnsAsync((User?)null);

        User? capturedUser = null;
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .Returns(Task.CompletedTask);

        await _service.RegisterAsync("john@test.com", "password123");

        Assert.NotNull(capturedUser);
        Assert.NotEqual("password123", capturedUser.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("password123", capturedUser.PasswordHash));
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WithEmptyNameRoleLocation()
    {
        _mockRepository.Setup(r => r.GetByEmailAsync("john@test.com")).ReturnsAsync((User?)null);

        User? capturedUser = null;
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .Returns(Task.CompletedTask);

        await _service.RegisterAsync("john@test.com", "password123");

        Assert.NotNull(capturedUser);
        Assert.Equal(string.Empty, capturedUser.Name);
        Assert.Equal(string.Empty, capturedUser.Role);
        Assert.Equal(string.Empty, capturedUser.Location);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnError_WhenEmailHasNoAtSign()
    {
        var result = await _service.RegisterAsync("invalidemail.com", "password123");

        Assert.Equal("Invalid email format.", result);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnError_WhenEmailIsEmpty()
    {
        var result = await _service.RegisterAsync("", "password123");

        Assert.Equal("Invalid email format.", result);
    }


    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenEmailInvalid()
    {
        var result = await _service.LoginAsync("notanemail", "password123");

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordTooShort()
    {
        var result = await _service.LoginAsync("john@test.com", "abc");

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
    {
        _mockRepository.Setup(r => r.GetByEmailAsync("john@test.com")).ReturnsAsync((User?)null);

        var result = await _service.LoginAsync("john@test.com", "password123");

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIncorrect()
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new User { Id = "123", Email = "john@test.com", PasswordHash = hashedPassword, Name = "John", Role = "Admin", Location = "London" };
        _mockRepository.Setup(r => r.GetByEmailAsync("john@test.com")).ReturnsAsync(user);

        var result = await _service.LoginAsync("john@test.com", "wrongpassword");

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenValidCredentials()
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User { Id = "123", Email = "john@test.com", PasswordHash = hashedPassword, Name = "John", Role = "Admin", Location = "London" };
        _mockRepository.Setup(r => r.GetByEmailAsync("john@test.com")).ReturnsAsync(user);

        var result = await _service.LoginAsync("john@test.com", "password123");

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnValidJwtToken_WhenValidCredentials()
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User { Id = "123", Email = "john@test.com", PasswordHash = hashedPassword, Name = "John", Role = "Admin", Location = "London" };
        _mockRepository.Setup(r => r.GetByEmailAsync("john@test.com")).ReturnsAsync(user);

        var result = await _service.LoginAsync("john@test.com", "password123");

        Assert.NotNull(result);
        var parts = result.Split('.');
        Assert.Equal(3, parts.Length);
    }


    [Fact]
    public async Task UpdateProfileAsync_ShouldReturnFalse_WhenUserNotFound()
    {
        _mockRepository.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((User?)null);

        var result = await _service.UpdateProfileAsync("999", new UpdateProfileDTO { Name = "John", Role = "Admin", Location = "London" });

        Assert.False(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldReturnTrue_WhenUserExists()
    {
        var user = new User { Id = "123", Email = "john@test.com", PasswordHash = "hash", Name = "John", Role = "Admin", Location = "London" };
        _mockRepository.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(user);

        var result = await _service.UpdateProfileAsync("123", new UpdateProfileDTO { Name = "John Updated", Role = "Developer", Location = "Berlin" });

        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateAsync("123", It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldUpdateCorrectFields()
    {
        var user = new User { Id = "123", Email = "john@test.com", PasswordHash = "hash", Name = "John", Role = "Admin", Location = "London" };
        _mockRepository.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(user);

        User? capturedUser = null;
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<User>()))
            .Callback<string, User>((id, u) => capturedUser = u)
            .Returns(Task.CompletedTask);

        await _service.UpdateProfileAsync("123", new UpdateProfileDTO { Name = "John Updated", Role = "Developer", Location = "Berlin" });

        Assert.NotNull(capturedUser);
        Assert.Equal("John Updated", capturedUser.Name);
        Assert.Equal("Developer", capturedUser.Role);
        Assert.Equal("Berlin", capturedUser.Location);
    }

    [Fact]
    public async Task UpdateProfileAsync_ShouldPreserveEmailAndPasswordHash()
    {
        var user = new User { Id = "123", Email = "john@test.com", PasswordHash = "original_hash", Name = "John", Role = "Admin", Location = "London" };
        _mockRepository.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(user);

        User? capturedUser = null;
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<User>()))
            .Callback<string, User>((id, u) => capturedUser = u)
            .Returns(Task.CompletedTask);

        await _service.UpdateProfileAsync("123", new UpdateProfileDTO { Name = "John Updated", Role = "Developer", Location = "Berlin" });

        Assert.NotNull(capturedUser);
        Assert.Equal("john@test.com", capturedUser.Email);
        Assert.Equal("original_hash", capturedUser.PasswordHash);
    }
}