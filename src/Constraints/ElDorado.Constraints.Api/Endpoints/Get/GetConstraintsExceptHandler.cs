using System.Text.Json.Nodes;
using ElDorado.Constraints.Domain.Constraints.Model;
using ElDorado.Constraints.Domain.Contracts;

namespace ElDorado.Constraints.Api.Endpoints.Get;

public class GetConstraintsExceptHandler
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/constraints/get/{id}", HandleAsync)
            .WithTags("Constraints");
    }

    private static async Task<List<ConstraintResult>> HandleAsync(string id, JsonObject entity,
        IConstraintManager constraintManager)
    {
        var result = await constraintManager.EvaluateConstraintsAsync(entity);
        result = result.Value
            .Where(c => c.IsApplicable && c.ConstraintId != id).ToList();
        return result.Value;
    }
}
