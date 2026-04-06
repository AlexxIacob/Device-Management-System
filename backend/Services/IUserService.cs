namespace backend.Services;
using backend.Models;   
    public interface IUserService
    {

        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string id);

    }
