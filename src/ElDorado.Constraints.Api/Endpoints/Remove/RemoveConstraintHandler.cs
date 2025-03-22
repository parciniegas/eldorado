using ElDorado.Constraints.Domain.Contracts;

namespace ElDorado.Constraints.Api.Endpoints.Remove;

public class RemoveConstraintHandler
{
    public static void Map(WebApplication app)
    {
        app.MapDelete("/constraints", HandleAsync)
            .WithTags("Constraints");
    }

    private static async Task HandleAsync(string constraintId, IConstraintManager constraintManager)
    {
        await constraintManager.RemoveConstraintAsync(constraintId);
    }
}