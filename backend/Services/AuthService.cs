using backend.Configuration;
using backend.DTO;
using backend.Models;
using backend.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace backend.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<string> RegisterAsync(string email, string password)
    {
        if (!IsValidEmail(email))
            return "Invalid email format.";

        if (password.Length < 7)
            return "Password must be at least 7 characters.";

        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser is not null)
            return "Email already in use.";

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Email = email,
            PasswordHash = hashedPassword,
            Name = string.Empty,
            Role = string.Empty,
            Location = string.Empty
        };

        await _userRepository.CreateAsync(user);
        return "User registered successfully.";
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        if (!IsValidEmail(email))
            return null;

        if (password.Length < 7)
            return null;

        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return GenerateJwtToken(user);
    }

    public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDTO dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return false;

        user.Name = dto.Name;
        user.Role = dto.Role;
        user.Location = dto.Location;

        await _userRepository.UpdateAsync(userId, user);
        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id!),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiryHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public async Task<User?> GetProfileAsync(string userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }
}