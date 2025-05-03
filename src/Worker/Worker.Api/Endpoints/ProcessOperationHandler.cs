using Common.Operations;
using Microsoft.AspNetCore.Http.HttpResults;
using Worker.Api.Services;

namespace Worker.Api.Endpoints;

public class ProcessOperationHandler
{
    public static void Map(WebApplication application)
    {
        _ = application.MapPost("/changeStatus", Handle)
            .WithName("DoOperation")
            .WithTags("Operation")
            .Produces<Operation>(StatusCodes.Status200OK)
            .WithMetadata(new EndpointNameMetadata("OperationServices"));
    }

    private static Results<Ok<Operation>, BadRequest<string>> Handle(Operation operation, IOperationsService operationsService)
    {
        if (operation.Status == OperationStatus.Failed)
            return TypedResults.BadRequest("Operation is already failed");

        if (operation.Status == OperationStatus.Closed)
            return TypedResults.BadRequest("Operation is already closed");

        Operation result = operation;
        return TypedResults.Ok(operationsService.ChangeStatus(operation));
    }
}
