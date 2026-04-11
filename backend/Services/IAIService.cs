using backend.DTO;

namespace backend.Services;

public interface IAIService
{
    Task<string> ChatAsync(string message);
}