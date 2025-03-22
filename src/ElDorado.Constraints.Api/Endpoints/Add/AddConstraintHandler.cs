using ElDorado.Constraints.Domain.Contracts;

namespace ElDorado.Constraints.Api.Endpoints.Add;

public static class AddConstraintHandler
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/constraints", HandleAsync)
            .WithTags("Constraints");
    }

    private static async Task HandleAsync(AddConstraintRequest request, IConstraintManager constraintManager)
    {
        var constraint = request.ToConstraint();
        await constraintManager.AddConstraintAsync(constraint);
    }
}