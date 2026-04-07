namespace backend.Services;

using backend.DTO;
public interface IAuthService
{

    Task<string> RegisterAsync(string email, string password);

    Task<string?> LoginAsync(string email, string password);

    Task<bool> UpdateProfileAsync(string userId, UpdateProfileDTO dto);



}
