using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Orders.Model;
using Orders.Services;

namespace Orders.Endpoints;

public static class CreateOrderHandler
{
    public static void Map(WebApplication application)
    {
        application.MapPost("/orders", Handle)
            .WithName("CreateOrder")
            .WithTags("Orders")
            .Produces<Order>(StatusCodes.Status201Created)
            .Produces<ProblemHttpResult>(StatusCodes.Status500InternalServerError)
            .WithMetadata(new EndpointNameMetadata("OrderServices"));
    }

    private static async Task<Results<Created<Order>, ProblemHttpResult>> Handle([FromBody] Order order, IOrderServices orderServices)
    {
        var orderId = await orderServices.CreateOrderAsync(order);
        if (orderId <= 0)
        {
            return TypedResults.Problem("Failed to create order, invalid Order ID returned.", statusCode: StatusCodes.Status500InternalServerError);
        }

        var createdOrder = await orderServices.GetOrderAsync(orderId);
        if (createdOrder == null)
        {
            return TypedResults.Problem(title: "Order created but could not be retrieved.", detail: $"Order ID: {orderId}", statusCode: StatusCodes.Status500InternalServerError);
        }
        return TypedResults.Created($"/orders/{orderId}", createdOrder);
    }
}
