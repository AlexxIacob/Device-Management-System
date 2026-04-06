namespace backend.Services;
using backend.Repositories;
using backend.Models;
public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        
        public UserService (IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _userRepository.GetByIdAsync(id);
         }

    }
