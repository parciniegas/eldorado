using Inventory.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Inventory.Endpoints;

public static class ReserveProductHandler
{
    public static void Map(WebApplication application)
    {
        application.MapPost("/products/reserve", Handle)
            .WithName("ReserveProduct")
            .WithTags("Products")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithMetadata(new EndpointNameMetadata("ReserveProductHandler"));
    }

    private static async Task<Results<Ok, BadRequest<string>, NotFound>> Handle(ReserveRequest input, IProductServices productServices)
    {
        try
        {
            productServices.ReserveProduct(input.ProductId, input.Quantity);
            return TypedResults.Ok();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (ArgumentException ex) // Catches product not found from service
        {
            return TypedResults.NotFound();
        }
        catch (InvalidOperationException ex) // Catches insufficient stock
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
