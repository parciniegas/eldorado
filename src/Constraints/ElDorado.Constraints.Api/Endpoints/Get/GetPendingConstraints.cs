using ElDorado.Constraints.Domain.Contracts;

namespace ElDorado.Constraints.Api.Endpoints.Get;

public class GetPendingConstraintsHandler
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/constraints/pending", HandleAsync)
            .WithTags("Constraints");
    }

    private static async Task<int> HandleAsync(List<string> ids, IConstraintManager constraintManager)
    {
        var result = await constraintManager.GetPendingConstraintsAsync(ids);
        return result.Value;
    }
}
