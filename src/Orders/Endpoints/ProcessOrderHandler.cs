using Microsoft.AspNetCore.Http.HttpResults;
using Orders.Services;

namespace Orders.Endpoints;

public static class ProcessOrderHandler
{
    public static void Map(WebApplication application)
    {
        application.MapPut("/orders/{orderId}/process", Handle)
            .WithName("ProcessOrder")
            .WithTags("Orders")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest) // For cases where order is not in a processable state
            .WithMetadata(new EndpointNameMetadata("OrderServices"));
    }

    private static async Task<Results<Ok, NotFound, BadRequest<string>>> Handle(int orderId, IOrderServices orderServices)
    {
        var success = await orderServices.ProcessOrderAsync(orderId);
        if (!success)
        {
            // Attempt to get the order to see if it was not found or in a bad state
            var order = await orderServices.GetOrderAsync(orderId);
            if (order == null)
            {
                return TypedResults.NotFound();
            }
            // If the order exists but couldn't be processed, it's likely due to its current status
            return TypedResults.BadRequest($"Order ID {orderId} could not be processed. It might not be in a pending state.");
        }
        return TypedResults.Ok();
    }
}
