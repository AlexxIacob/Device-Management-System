namespace backend.Services;

using backend.DTO;
using backend.Models;

public interface IAuthService
{

    Task<string> RegisterAsync(string email, string password);

    Task<string?> LoginAsync(string email, string password);

    Task<bool> UpdateProfileAsync(string userId, UpdateProfileDTO dto);

    Task<User?> GetProfileAsync(string userId);


}
