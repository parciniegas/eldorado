using ElDorado.Domain.Constraints.Contracts;

namespace ElDorado.Api.Endpoints.Constraints.Remove;

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
