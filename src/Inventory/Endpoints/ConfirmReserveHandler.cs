using Inventory.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc; // Included for consistency with other handlers

// Assumes ReserveRequest is defined in the Inventory.Endpoints namespace
// and is accessible here.

namespace Inventory.Endpoints;

public static class ConfirmReserveHandler
{
    public static void Map(WebApplication application)
    {
        application.MapPost("/products/confirm", Handle)
            .WithName("ConfirmReserve")
            .WithTags("Products")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithMetadata(new EndpointNameMetadata("ConfirmReserveHandler"));
    }

    private static async Task<Results<Ok, BadRequest<string>, NotFound>> Handle(ReserveRequest input, IProductServices productServices)
    {
        try
        {
            productServices.ConfirmReserve(input.ProductId, input.Quantity);
            return TypedResults.Ok();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (ArgumentException ex) // Assuming this is for "Product not found"
        {
            return TypedResults.NotFound();
        }
    }
}
