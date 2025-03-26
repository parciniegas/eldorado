using System.Text.Json.Nodes;

namespace ElDorado.Operations.Api.EndPoints;

public record OperationTransitRecord(
    string OperationId,
    string OperationType,
    string From,
    string To,
    JsonObject Data);
