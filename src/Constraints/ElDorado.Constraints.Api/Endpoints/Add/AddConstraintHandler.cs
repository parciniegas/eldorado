using ElDorado.Constraints.Domain.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ElDorado.Constraints.Api.Endpoints.Add;

public static class AddConstraintHandler
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/constraints", HandleAsync)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .WithDescription("Add a new constraint")
            .WithSummary("Add a constraint")
            .WithTags("Constraints");
    }

    private static async Task<Results<Ok<string>, BadRequest<string>>> HandleAsync(AddConstraintRequest request, IConstraintManager constraintManager)
    {
        var constraint = request.ToConstraint();
        var result = await constraintManager.AddConstraintAsync(constraint);
        return result.IsSuccess
            ? TypedResults.Ok(result.Value)
            : TypedResults.BadRequest(result.Errors.ToString());
    }
}