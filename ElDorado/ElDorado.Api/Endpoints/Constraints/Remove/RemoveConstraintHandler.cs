using ElDorado.Domain.Constraints;
using FastEndpoints;

namespace ElDorado.Api.Endpoints.Constraints.Remove;

public class RemoveConstraintHandler: Endpoint<RemoveConstraintRequest, RemoveConstraintResponse>
{
    public override void Configure()
    {
        Delete("/api/constraints");
        AllowAnonymous();
    }

    public override Task HandleAsync(RemoveConstraintRequest req, CancellationToken ct)
    {
        ConstraintManager.RemoveConstraint(Guid.Parse(req.ConstraintId));
        return Task.CompletedTask;
    }
}
