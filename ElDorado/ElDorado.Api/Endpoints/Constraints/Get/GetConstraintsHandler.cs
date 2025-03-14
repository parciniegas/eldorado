using System.Dynamic;
using ElDorado.Domain.Constraints;
using FastEndpoints;

namespace ElDorado.Api.Endpoints.Constraints.Get;

public class GetConstraintsHandler: Endpoint<ExpandoObject, List<string>>
{
    public override void Configure()
    {
        Get("/api/constraints");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ExpandoObject req, CancellationToken ct)
    {
        var constraints = ConstraintManager.GetConstraints(req);
        var ids = constraints.Select(c => c.Id.ToString()).ToList();

        await SendAsync(ids, cancellation: ct);
    }
}
