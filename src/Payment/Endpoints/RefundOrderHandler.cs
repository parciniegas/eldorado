using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Payment.Services;
using PayModel = Payment.Model.Payment; // Alias to avoid conflict with namespace

namespace Payment.Endpoints;

public class RefundOrderHandler
{
    public static void Map(WebApplication application)
    {
        application.MapPost("/payment/refund", Handle)
            .WithName("RefundOrder")
            .WithTags("Payment")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithMetadata(new EndpointNameMetadata("RefundOrderHandler"));
    }

    private static async Task<Results<Ok, BadRequest<ProblemDetails>, ProblemHttpResult>> Handle(
        [FromBody] PayModel paymentInput,
        IPaymentServices paymentServices,
        ILogger<RefundOrderHandler> logger)
    {
        if (paymentInput == null)
        {
            logger.LogWarning("RefundOrder endpoint received null input.");
            return TypedResults.BadRequest(new ProblemDetails { Title = "Invalid input", Detail = "Payment data for refund cannot be null." });
        }

        try
        {
            bool success = paymentServices.RefundOrder(paymentInput);
            if (success)
            {
                logger.LogInformation("Refund successful for ID: {PaymentId}", paymentInput.Id);
                return TypedResults.Ok();
            }
            else
            {
                logger.LogWarning("Refund failed for ID: {PaymentId}", paymentInput.Id);
                // Consider if a more specific error (e.g., payment not found, not refundable) should be returned.
                return TypedResults.BadRequest(new ProblemDetails { Title = "Refund Failed", Detail = "The refund could not be processed." });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while processing refund for ID: {PaymentId}", paymentInput.Id);
            return TypedResults.Problem(new ProblemDetails { Title = "An unexpected error occurred", Detail = ex.Message, Status = StatusCodes.Status500InternalServerError });
        }
    }
}
