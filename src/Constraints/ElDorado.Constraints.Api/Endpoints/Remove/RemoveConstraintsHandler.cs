using ElDorado.Constraints.Domain.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ElDorado.Constraints.Api.Endpoints.Remove;

public class RemoveConstraintsHandler
{
    public static void Map(WebApplication app)
    {
        app.MapDelete("/constraints/remove-list", HandleAsync)
            .WithTags("Constraints");
    }

    private static async Task HandleAsync([FromBody] RemoveConstraintsRequest request, IConstraintManager constraintManager)
    {
        foreach (var constraintId in request.ConstraintIds) await constraintManager.RemoveConstraintAsync(constraintId);
    }
}