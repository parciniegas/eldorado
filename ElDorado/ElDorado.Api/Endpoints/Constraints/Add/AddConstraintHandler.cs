using ElDorado.Domain.Constraints;
using FastEndpoints;

namespace ElDorado.Api.Endpoints.Constraints.Add;

public class AddConstraintHandler: Endpoint<AddConstraintRequest, AddConstraintResponse>
{
    public override void Configure()
    {
        Post("/api/constraints");

        AllowAnonymous();
    }

    public override async Task HandleAsync(AddConstraintRequest req, CancellationToken ct)
    {
        var id = ConstraintManager.AddConstraint(req.Conditions);

        await SendAsync(new AddConstraintResponse { Id = id.ToString() }, cancellation: ct);
    }
}
