using System.Text;
using System.Text.Json;
using Common.Operations;

namespace Manager.Api.Utils;

public class OperationsHttpClient(IConfiguration configuration)
{
    private readonly string Url = configuration["ApiUrl"] 
        ?? throw new ArgumentNullException(nameof(configuration), "Configuration key 'Url' cannot be null.");
    private readonly HttpClient _httpClient = new();

    public async Task<Operation> SendOperationAsync(Operation operation)
    {
        try {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var jsonContent = JsonSerializer.Serialize(operation);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(Url, content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var operationResponse = JsonSerializer.Deserialize<Operation>(responseContent) 
                ?? throw new InvalidOperationException("Failed to deserialize the response into an Operation object.");

            stopwatch.Stop();
            Console.WriteLine($"Operation {operation.Id} with Status {operation.Status.ToString()} processed in {stopwatch.ElapsedMilliseconds} ms");

            return operationResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending operation {operation.Id}: {ex.Message}");
            operation.Status = OperationStatus.Failed;
            return operation;
        }
    }
}
