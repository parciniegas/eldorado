using ElDorado.Domain.Constraints;
using Ilse.MinimalApi.EndPoints;
using Microsoft.AspNetCore.Mvc;

namespace ElDorado.Api.Endpoints.Constraints.Remove;

public class RemoveConstraintHandler: IEndpoint
{
    public RouteHandlerBuilder Configure(IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapDelete("/constraints/{constraintId}", HandleAsync)
            .WithTags("Constraints")
            .WithName("Remove Constraint")
            .WithDescription("Remove a constraint");
    }

    public static async Task HandleAsync([FromBody]RemoveConstraintRequest req, CancellationToken ct)
    {
        ConstraintManager.RemoveConstraint(Guid.Parse(req.ConstraintId));
        await Task.CompletedTask;
    }
}
