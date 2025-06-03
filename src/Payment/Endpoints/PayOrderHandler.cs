using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Payment.Services;
using PayModel = Payment.Model.Payment; // Alias to avoid conflict with namespace

namespace Payment.Endpoints;

public class PayOrderHandler
{
    public static void Map(WebApplication application)
    {
        application.MapPost("/payment/pay", Handle)
            .WithName("PayOrder")
            .WithTags("Payment")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .WithMetadata(new EndpointNameMetadata("PayOrderHandler"));
    }

    private static async Task<Results<Ok, BadRequest<ProblemDetails>, ProblemHttpResult>> Handle(
        [FromBody] PayModel paymentInput,
        IPaymentServices paymentServices,
        ILogger<PayOrderHandler> logger)
    {
        if (paymentInput == null)
        {
            logger.LogWarning("PayOrder endpoint received null input.");
            return TypedResults.BadRequest(new ProblemDetails { Title = "Invalid input", Detail = "Payment data cannot be null." });
        }

        try
        {
            bool success = paymentServices.PayOrder(paymentInput);
            if (success)
            {
                logger.LogInformation("Payment successful for ID: {PaymentId}", paymentInput.Id);
                return TypedResults.Ok();
            }
            else
            {
                logger.LogWarning("Payment failed for ID: {PaymentId}", paymentInput.Id);
                return TypedResults.BadRequest(new ProblemDetails { Title = "Payment Failed", Detail = "The payment could not be processed." });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while processing payment for ID: {PaymentId}", paymentInput.Id);
            return TypedResults.Problem(new ProblemDetails { Title = "An unexpected error occurred", Detail = ex.Message, Status = StatusCodes.Status500InternalServerError });
        }
    }
}
