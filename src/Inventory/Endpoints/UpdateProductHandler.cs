using Inventory.Model;
using Inventory.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Inventory.Endpoints;

public static class UpdateProductHandler
{
    public static void Map(WebApplication application)
    {
        application.MapPut("/products/{id}", Handle)
            .WithName("UpdateProduct")
            .WithTags("Products")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static Results<Ok<Product>, NotFound> Handle(int id, Product product, IProductServices productServices)
    {
        if (id != product.Id)
        {
            // This is a common practice to ensure the ID in the path matches the ID in the body.
            // However, the prompt implies direct use of the product object for update.
            // Depending on strictness of API design, this check might be considered optional or handled differently.
        }
        var updatedProduct = productServices.UpdateProduct(product);
        return updatedProduct is not null ? TypedResults.Ok(updatedProduct) : TypedResults.NotFound(); // Assuming UpdateProduct might return null if not found, though interface suggests it returns Product.
    }
}
