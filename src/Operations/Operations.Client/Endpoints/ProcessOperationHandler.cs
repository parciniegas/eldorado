using Microsoft.AspNetCore.Http.HttpResults;
using Operations.Client.Utils;
using Operations.Shared;

namespace Operations.Client.Endpoints;

public class ProcessOperationHandler
{
    public static void Map(WebApplication application)
    {
        _ = application.MapPost("/process", HandleAsync)
            .WithName("ProcessOperation")
            .WithTags("Operations")
            .Produces<Operation>(StatusCodes.Status200OK)
            .WithMetadata(new EndpointNameMetadata("ProcessOperation"));
    }
    private static async Task<Ok<Operation>> HandleAsync(Operation operation, OperationsHttpClient operationsHttpClient)
    {
        while (operation.Status != OperationStatus.Closed && operation.Status != OperationStatus.Failed)
        {
            operation = await operationsHttpClient.SendOperationAsync(operation);
        }

        if (operation.Status == OperationStatus.Closed)
            Console.WriteLine($"Operation {operation.Id} was finish with status {operation.Status}");

        if (operation.Status == OperationStatus.Failed)
            Console.WriteLine($"Operation {operation.Id} failed with status {operation.Status}");

        return TypedResults.Ok(operation);
    }

}
