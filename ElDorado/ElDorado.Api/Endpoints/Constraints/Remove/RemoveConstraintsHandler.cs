using ElDorado.Domain.Constraints.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ElDorado.Api.Endpoints.Constraints.Remove;

public class RemoveConstraintsHandler
{
    public static void Map(WebApplication app)
    {
        app.MapDelete("/constraints/remove-list", HandleAsync)
            .WithTags("Constraints");
    }

    private static async Task HandleAsync([FromBody]List<string> constraints, IConstraintManager constraintManager)
    {
        foreach (var constraintId in constraints)
        {
            await constraintManager.RemoveConstraintAsync(constraintId);
        }
    }
}
