using Microsoft.AspNetCore.Http.HttpResults;
using Orders.Model;
using Orders.Services;

namespace Orders.Endpoints;

public static class GetOrdersHandler
{
    public static void Map(WebApplication application)
    {
        application.MapGet("/orders", Handle)
            .WithName("GetOrders")
            .WithTags("Orders")
            .Produces<List<Order>>(StatusCodes.Status200OK)
            .WithMetadata(new EndpointNameMetadata("OrderServices"));
    }

    private static async Task<Ok<List<Order>>> Handle(int page, int pageSize, IOrderServices orderServices)
    {
        var orders = await orderServices.GetOrdersAsync(page, pageSize);
        return TypedResults.Ok(orders);
    }
}
