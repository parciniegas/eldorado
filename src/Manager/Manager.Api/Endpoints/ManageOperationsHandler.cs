using Common.Operations;
using Manager.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;

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
    private static async Task<Ok<Operation>> HandleAsync(Operation operation, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        var processorType = configuration["ProcessorType"];
        ISyncProcesor procesor = processorType switch
        {
            "SyncProcessorWithEvents" => serviceProvider.GetRequiredKeyedService<ISyncProcesor>("SyncProcessorWithEvents"),
            "SyncProcessorWithRequest" => serviceProvider.GetRequiredKeyedService<ISyncProcesor>("SyncProcessorWithRequest"),
            _ => throw new ArgumentException("Invalid processor type")
        };
        var result = await procesor.Process(operation);
        return TypedResults.Ok(result);
    }

}
