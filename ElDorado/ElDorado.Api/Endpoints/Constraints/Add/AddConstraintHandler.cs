using ElDorado.Domain.Constraints;
using Ilse.MinimalApi.EndPoints;
using Microsoft.AspNetCore.Mvc;

namespace ElDorado.Api.Endpoints.Constraints.Add;

public class AddConstraintHandler: IEndpoint
{
    public RouteHandlerBuilder Configure(IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/constraints", HandleAsync)
            .WithTags("Constraints")
            .WithName("Add Constraint")
            .WithDescription("Add a new constraint");
    }

    private static async Task<string> HandleAsync([FromBody]AddConstraintRequest req, CancellationToken ct)
    {
        var id = ConstraintManager.AddConstraint(req.Conditions);

        return await Task.FromResult(id.ToString());
    }
}
