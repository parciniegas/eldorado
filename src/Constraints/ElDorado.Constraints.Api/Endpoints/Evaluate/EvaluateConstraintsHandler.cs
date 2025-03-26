using System.Text.Json.Nodes;
using ElDorado.Constraints.Domain.Constraints.Model;
using ElDorado.Constraints.Domain.Contracts;

namespace ElDorado.Constraints.Api.Endpoints.Evaluate;

public class EvaluateConstraintsHandler
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/constraints/evaluate", HandleAsync)
            .WithTags("Constraints");
    }

    private static async Task<List<ConstraintResult>> HandleAsync(JsonObject entity,
        IConstraintManager constraintManager)
    {
        return await constraintManager.EvaluateConstraintsAsync(entity);
    }
}