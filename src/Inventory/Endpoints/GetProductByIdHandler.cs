using Inventory.Model;
using Inventory.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Inventory.Endpoints;

public static class GetProductByIdHandler
{
    public static void Map(WebApplication application)
    {
        application.MapGet("/products/{id}", Handle)
            .WithName("GetProductById")
            .WithTags("Products")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static Results<Ok<Product>, NotFound> Handle(int id, IProductServices productServices)
    {
        var product = productServices.GetProductById(id);
        return product is not null ? TypedResults.Ok(product) : TypedResults.NotFound();
    }
}
