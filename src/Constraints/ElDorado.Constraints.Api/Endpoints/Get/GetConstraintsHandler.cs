using System.Text.Json.Nodes;
using ElDorado.Constraints.Domain.Contracts;
using ElDorado.Constraints.Domain.Model;

namespace ElDorado.Constraints.Api.Endpoints.Get;

public static class GetConstraintsHandler
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/constraints/get", HandleAsync)
            .WithTags("Constraints");
    }

    private static async Task<List<ConstraintResult>> HandleAsync(JsonObject entity,
        IConstraintManager constraintManager)
    {
        var result = await constraintManager.EvaluateConstraintsAsync(entity);
        result = result.Value.Where(c => c.IsApplicable).ToList();
        return result.Value;
    }
}