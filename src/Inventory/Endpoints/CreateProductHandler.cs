using Inventory.Model;
using Inventory.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Inventory.Endpoints;

public static class CreateProductHandler
{
    public static void Map(WebApplication application)
    {
        application.MapPost("/products", Handle)
            .WithName("CreateProduct")
            .WithTags("Products")
            .Produces<Product>(StatusCodes.Status201Created);
    }

    private static Created<Product> Handle(Product product, IProductServices productServices)
    {
        var createdProduct = productServices.CreateProduct(product);
        return TypedResults.Created($"/products/{createdProduct.Id}", createdProduct);
    }
}
