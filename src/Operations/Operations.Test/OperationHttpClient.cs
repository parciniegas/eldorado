using System.Text;
using System.Text.Json;
using Operations.Shared;

namespace Operations.Test;

public class OperationHttpClient(HttpClient httpClient)
{
    private readonly string Url = "http://localhost:5294/process";
    private readonly HttpClient _httpClient = httpClient;

    public Operation SendOperation(Operation operation)
    {
        try {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var jsonContent = JsonSerializer.Serialize(operation);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(Url, content).Result;
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
            return operation;
        }
    }

    public async Task SendOperationAsync(Operation operation)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var jsonContent = JsonSerializer.Serialize(operation);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(Url, content);
        stopwatch.Stop();
        Console.WriteLine($"Operation {operation.Id} send in {stopwatch.ElapsedMilliseconds} ms");

        response.EnsureSuccessStatusCode();
    }
}
