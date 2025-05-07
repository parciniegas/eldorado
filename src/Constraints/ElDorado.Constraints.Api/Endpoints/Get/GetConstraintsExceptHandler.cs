using System.Text.Json.Nodes;
using ElDorado.Constraints.Domain.Contracts;

namespace ElDorado.Constraints.Api.Endpoints.Get;

public class GetConstraintsExceptHandler
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/constraints/get/{id}", HandleAsync)
            .WithTags("Constraints");
    }

    private static async Task<List<string>> HandleAsync(string id, JsonObject entity,
        IConstraintManager constraintManager)
    {
        var result = await constraintManager.EvaluateConstraintsAsync(entity);
        // Filter out the constraints that are not applicable
        var constraint = await constraintManager.GetConstraintAsync(id);
        if (constraint.IsFailed)
            return [];
        result = result.Value
            .Where(c => c.IsApplicable && c.ConstraintId != id && c.CreateAt > constraint.Value.CreateAt)
            .ToList();
        var ids = result.Value.Select(c => c.ConstraintId).ToList();
        if (ids.Count == 0)
            return [];
        return ids;
    }
}
