using ElDorado.Constraints.Domain.Contracts;

namespace ElDorado.Constraints.Api.Endpoints.Add;

public static class AddConstraintsHandler
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/constraints/add-list", HandleAsync)
            .WithTags("Constraints");
    }

    private static async Task HandleAsync(AddConstraintsRequest request, IConstraintManager constraintManager)
    {
        foreach (var constraint in request.ConstraintsRequests.Select(constraintRequest =>
                     constraintRequest.ToConstraint())) await constraintManager.AddConstraintAsync(constraint);
    }
}