using Microsoft.AspNetCore.Http.HttpResults;
using Orders.Model;
using Orders.Services;

namespace Orders.Endpoints;

public static class GetOrderHandler
{
    public static void Map(WebApplication application)
    {
        application.MapGet("/orders/{orderId}", Handle)
            .WithName("GetOrder")
            .WithTags("Orders")
            .Produces<Order>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithMetadata(new EndpointNameMetadata("OrderServices"));
    }

    private static async Task<Results<Ok<Order>, NotFound>> Handle(int orderId, IOrderServices orderServices)
    {
        var order = await orderServices.GetOrderAsync(orderId);
        return order is not null ? TypedResults.Ok(order) : TypedResults.NotFound();
    }
}
