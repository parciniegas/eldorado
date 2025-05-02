using System.Text;
using System.Text.Json;

namespace Operations.Shared;

public class OperationHttpClient()
{
    private readonly string Url = "http://localhost:5033/Operations/Process";
    private readonly HttpClient _httpClient = new();

    public async Task SendOperationAsync<T>(T entity)
    {
        var jsonContent = JsonSerializer.Serialize(entity);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(Url, content);

        response.EnsureSuccessStatusCode();
    }
}
