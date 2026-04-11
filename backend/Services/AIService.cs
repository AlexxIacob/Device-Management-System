using backend.DTO;
using System.Text;
using System.Text.Json;

namespace backend.Services;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private const string OllamaUrl = "http://localhost:11434/api/generate";
    private const string Model = "llama3.2";

    public AIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> ChatAsync(string message)
    {
        var prompt = $"You are a device management assistant. You only answer questions related to devices, " +
             $"technology, specifications, and device management. " +
             $"If the user mentions a specific device (like iPhone 15, Samsung Galaxy S24, etc.), " +
             $"provide its key specifications: processor, RAM, OS, and OS version in a concise format. " +
             $"You can also answer follow-up questions about that device or general device-related questions. " +
             $"Give short, clear answers. " +
             $"If the question is not related to devices or technology, respond only with: " +
             $"'I can only assist with device-related questions.' " +
             $"User question: {message}";

        var requestBody = new
        {
            model = Model,
            prompt = prompt,
            stream = false
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(OllamaUrl, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
        return result.GetProperty("response").GetString() ?? "I could not process your request.";
    }
}