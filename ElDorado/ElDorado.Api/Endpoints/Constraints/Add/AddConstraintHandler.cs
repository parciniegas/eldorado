using System.Data;
using ElDorado.Domain.Constraints.Contracts;

namespace ElDorado.Api.Endpoints.Constraints.Add;

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
