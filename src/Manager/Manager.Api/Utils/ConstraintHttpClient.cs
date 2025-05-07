using System.Text;
using System.Text.Json;
using Common.Constraints;
using Common.Operations;

namespace Manager.Api.Utils;

public class ConstraintHttpClient(IConfiguration configuration)
{
    private readonly string Url = configuration["ConstraintUrl"] 
        ?? throw new ArgumentNullException(nameof(configuration), "Configuration key 'ConstraintUrl' cannot be null.");
    private readonly HttpClient _httpClient = new();

    public async Task<string?> AddConstraint(Constraint constraint)
    {
        try {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var jsonContent = JsonSerializer.Serialize(constraint);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(Url, content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<string>(responseContent) 
                ?? throw new InvalidOperationException("Failed to deserialize the response into a string object.");

            stopwatch.Stop();
            Console.WriteLine($"Constraint {constraint.Id} added in {stopwatch.ElapsedMilliseconds} ms");

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending operation {constraint.Id}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<string>> GetConstraints(Operation operation, string id)
    {
        try {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var jsonContent = JsonSerializer.Serialize(operation);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var url = $"{Url}/get/{id}";
            var response = await _httpClient.PostAsync($"{url}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<List<string>>(responseContent) 
                ?? throw new InvalidOperationException("Failed to deserialize the response into a Constraint object.");

            stopwatch.Stop();
            Console.WriteLine($"Constraint {operation.Id} retrieved in {stopwatch.ElapsedMilliseconds} ms");

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving constraint {operation.Id}: {ex.Message}");
            throw new InvalidOperationException($"Failed to retrieve constraint {operation.Id}", ex);
        }
    }

    public async Task<int> GetPendingConstraints(List<string> ids)
    {
        try {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var jsonContent = JsonSerializer.Serialize(ids);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{Url}/pending", content);
            response.EnsureSuccessStatusCode();
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<int>(responseContent);

            stopwatch.Stop();
            Console.WriteLine($"Pending constraints retrieved in {stopwatch.ElapsedMilliseconds} ms");

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving pending constraints: {ex.Message}");
            throw new InvalidOperationException("Failed to retrieve pending constraints", ex);
        }
    }

    public async Task<string?> RemoveConstraint(string id)
    {
        try {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var response = await _httpClient.DeleteAsync($"{Url}?constraintId={id}");
            response.EnsureSuccessStatusCode();
            
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var result = JsonSerializer.Deserialize<string>(responseContent) 
                ?? throw new InvalidOperationException("Failed to deserialize the response into a string object.");

            stopwatch.Stop();
            Console.WriteLine($"Constraint {id} deleted in {stopwatch.ElapsedMilliseconds} ms");

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting constraint {id}: {ex.Message}");
            return null;
        }
    }
}
