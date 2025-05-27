using Microsoft.AspNetCore.Http.HttpResults;
using Orders.Services;

namespace Orders.Endpoints;

public static class CompleteOrderHandler
{
    public static void Map(WebApplication application)
    {
        application.MapPut("/orders/{orderId}/complete", Handle)
            .WithName("CompleteOrder")
            .WithTags("Orders")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest) // For cases where order is not in a completable state
            .WithMetadata(new EndpointNameMetadata("OrderServices"));
    }

    private static async Task<Results<Ok, NotFound, BadRequest<string>>> Handle(int orderId, IOrderServices orderServices)
    {
        var success = await orderServices.CompleteOrderAsync(orderId);
        if (!success)
        {
            var order = await orderServices.GetOrderAsync(orderId);
            if (order == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.BadRequest($"Order ID {orderId} could not be completed. It might not be in a processing state.");
        }
        return TypedResults.Ok();
    }
}
