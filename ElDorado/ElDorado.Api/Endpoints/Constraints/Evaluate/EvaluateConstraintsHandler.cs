using System.Text.Json.Nodes;
using ElDorado.Domain.Constraints;
using ElDorado.Domain.Constraints.Contracts;
using ElDorado.Domain.Constraints.Model;

namespace ElDorado.Api.Endpoints.Constraints.Evaluate;

public class EvaluateConstraintsHandler
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/constraints/evaluate", HandleAsync)
            .WithTags("Constraints");
    }

    private static async Task<List<ConstraintResult>> HandleAsync(JsonObject entity, IConstraintManager constraintManager)
    {
        return await constraintManager.EvaluateConstraintsAsync(entity);
    }
}
