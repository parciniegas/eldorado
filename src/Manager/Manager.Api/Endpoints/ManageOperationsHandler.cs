using Common.Operations;
using Manager.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Manager.Api.Endpoints;

public class ManageOperationsHandler
{
    public static void Map(WebApplication application)
    {
        _ = application.MapPost("/process", HandleAsync)
            .WithName("ProcessOperation")
            .WithTags("Operations")
            .Produces<Operation>(StatusCodes.Status200OK)
            .WithMetadata(new EndpointNameMetadata("ProcessOperation"));
    }
    private static async Task<Ok<Operation>> HandleAsync(Operation operation, ISyncProcesor procesor)
    {
        var result = await procesor.Process(operation);
        return TypedResults.Ok(result);
    }

}
