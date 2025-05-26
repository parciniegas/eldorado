using Inventory.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Endpoints;

public static class ReleaseReserveHandler
{
    public static void Map(WebApplication application)
    {
        application.MapPost("/products/release", Handle)
            .WithName("ReleaseReserve")
            .WithTags("Products")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithMetadata(new EndpointNameMetadata("ProductServices"));
    }

    private static async Task<Results<Ok, BadRequest<string>, NotFound>> Handle(ReserveRequest input, IProductServices productServices)
    {
        try
        {
            productServices.ReleaseReserve(input.ProductId, input.Quantity);
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
    }
}

// QuantityInput is already defined in ReserveProductHandler.cs, ensure it's accessible or defined in a shared location if not.
// For this example, assuming it's accessible or will be moved.
