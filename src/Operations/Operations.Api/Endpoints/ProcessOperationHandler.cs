using Microsoft.AspNetCore.Http.HttpResults;
using Operations.Api.Services;
using Operations.Shared;

namespace Operations.Api.Endpoints;

public class ProcessOperationHandler
{
    public static void Map(WebApplication application)
    {
        _ = application.MapPost("/changeStatus", Handle)
            .WithName("DoOperation")
            .WithTags("OperationServices")
            .Produces<Operation>(StatusCodes.Status200OK)
            .WithMetadata(new EndpointNameMetadata("OperationServices"));
    }

    private static Ok<Operation> Handle(Operation operation, IOperationsService operationsService)
    {
        Operation result = operation;
        if (operation.Status != OperationStatus.Completed)
            return TypedResults.Ok(operationsService.ProcessOperationAsync(operation));

        return TypedResults.Ok(operationsService.CloseOperationAsync(operation, CancellationToken.None));
    }
}
