using System.Text.Json.Serialization;

namespace Operations.Shared;

public class Operation()
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("status")]
    public required OperationStatus Status { get; set; }
    public string? Details { get; private set; }

    public OperationStatus SetNextStatus()
    {
        var status = Status;
        status = status switch
        {
            OperationStatus.Requested => OperationStatus.Created,
            OperationStatus.Created => OperationStatus.Pending,
            OperationStatus.Pending => OperationStatus.InProgress,
            OperationStatus.InProgress => OperationStatus.Completed,
            OperationStatus.Completed => OperationStatus.Closed,
            _ => OperationStatus.Failed,
        };
        Details = $"Status changed from {Status} to {status}";
        Status = status;
        
        return status;
    }
}
